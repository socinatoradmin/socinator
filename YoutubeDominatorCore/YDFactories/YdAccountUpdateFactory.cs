using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.DatabaseHandler.YdTables.Accounts;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using EmbeddedBrowser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Unity;
using YoutubeDominatorCore.Response;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeLibrary.Processes;
using YoutubeDominatorCore.YoutubeLibrary.YdFunctions;

namespace YoutubeDominatorCore.YDFactories
{
    public interface IYdAccountUpdateFactory : IAccountUpdateFactoryAsync
    {
    }

    public class YdAccountUpdateFactory : IYdAccountUpdateFactory
    {
        public static SemaphoreSlim YdCheckSemaphoreSlim = new SemaphoreSlim(2, 2);
        public static SemaphoreSlim YdUpdateSemaphoreSlim = new SemaphoreSlim(2, 2);
        private static IYdAccountUpdateFactory _instance;
        private readonly IAccountScopeFactory _accountScopeFactory;
        private readonly SocialNetworks _networks;

        public YdAccountUpdateFactory(IAccountScopeFactory accountScopeFactory)
        {
            _accountScopeFactory = accountScopeFactory;
            _networks = SocialNetworks.YouTube;
        }

        public static IYdAccountUpdateFactory Instance
            => _instance ?? (_instance = InstanceProvider.GetInstance<IYdAccountUpdateFactory>());

        public bool CheckStatus(DominatorAccountModel accountModel)
        {
            return Task.Factory.StartNew(() => CheckStatusAsync(accountModel, accountModel.Token).Result).Result;
        }

        public void UpdateDetails(DominatorAccountModel accountModel)
        {
            Task.Factory.StartNew(() => UpdateDetailsAsync(accountModel, accountModel.Token).Wait(accountModel.Token));
        }

        public async Task<bool> CheckStatusAsync(DominatorAccountModel accountModel, CancellationToken token)
        {
            // return false;
            if (accountModel?.AccountBaseModel == null)
                return false;

            GlobusLogHelper.log.Info(Log.AccountLogin, accountModel.AccountBaseModel?.AccountNetwork,
                accountModel.AccountBaseModel?.UserName);

            if (!accountModel.IsVerificationCodeSent)
            {
                accountModel.AccountBaseModel.Status = AccountStatus.TryingToLogin;
            }
            else if (accountModel.AccountBaseModel.Status == AccountStatus.PhoneVerification &&
                     (string.IsNullOrEmpty(accountModel.AccountBaseModel.PhoneNumber)
                      || accountModel.AccountBaseModel.PhoneNumber.Contains("•")))
            {
                accountModel.AccountBaseModel.Status = AccountStatus.AddPhoneNumberToYourAccount;
                await Task.Run(() => UpdateFailed(accountModel));
                return false;
            }

            try
            {
                await YdCheckSemaphoreSlim.WaitAsync();
                var logInProcess = _accountScopeFactory[accountModel.AccountId].Resolve<IYoutubeLogInProcess>();

                accountModel.Token.ThrowIfCancellationRequested();
                if (!await logInProcess.CheckLoginAsync(accountModel, token) &&
                    !accountModel.IsRunProcessThroughBrowser &&
                    accountModel.AccountBaseModel.Status != AccountStatus.ProxyNotWorking)
                {
                    accountModel.Token.ThrowIfCancellationRequested();


                    if (!accountModel.IsVerificationCodeSent ||
                        accountModel.AccountBaseModel.Status == AccountStatus.Failed)
                        accountModel.AccountBaseModel.Status = AccountStatus.TryingToLogin;
                    await Task.Run(() => logInProcess.BrowserLogin(accountModel, token));

                    accountModel.IsVerificationCodeSent = false;

                    if (accountModel.IsUserLoggedIn)
                        GlobusLogHelper.log.Info(Log.SuccessfulLogin, accountModel.AccountBaseModel.AccountNetwork,
                            accountModel.AccountBaseModel.UserName);
                    else
                        await Task.Run(() => UpdateFailed(accountModel));

                    return accountModel.IsUserLoggedIn;
                }

                accountModel.IsVerificationCodeSent = false;
                if (accountModel.AccountBaseModel.Status == AccountStatus.Success)
                    GlobusLogHelper.log.Info(Log.SuccessfulLogin, accountModel.AccountBaseModel.AccountNetwork,
                        accountModel.AccountBaseModel.UserName);
            }
            catch (OperationCanceledException)
            {
                accountModel.IsVerificationCodeSent = false;
                if (accountModel.AccountBaseModel.Status == AccountStatus.TryingToLogin)
                    accountModel.AccountBaseModel.Status = AccountStatus.Failed;

                SocinatorAccountBuilder.Instance(accountModel.AccountBaseModel.AccountId)
                    .AddOrUpdateDominatorAccountBase(accountModel.AccountBaseModel)
                    .AddOrUpdateLoginStatus(accountModel.IsUserLoggedIn)
                    .SaveToBinFile();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally
            {
                await Task.Run(() => UpdateGlobalAccountDetails(accountModel));
                YdCheckSemaphoreSlim.Release();
            }

            return accountModel.IsUserLoggedIn;
        }

        public async Task UpdateDetailsAsync(DominatorAccountModel accountModel, CancellationToken token)
        {
            IYoutubeLogInProcess logInProcess = null;
            try
            {
                await YdUpdateSemaphoreSlim.WaitAsync();
                if (accountModel.AccountBaseModel.Status != AccountStatus.Success) return;
                accountModel.AccountBaseModel.Status = AccountStatus.UpdatingDetails;
                GlobusLogHelper.log.Info(Log.UpdatingDetails, accountModel.AccountBaseModel.AccountNetwork,
                    accountModel.AccountBaseModel.UserName, "Account Details");
                OwnChannelScraperResponseHandler channels = null;
                if (accountModel.IsRunProcessThroughBrowser)
                {
                    try
                    {
                        logInProcess = _accountScopeFactory[accountModel.AccountId].Resolve<IYoutubeLogInProcess>();
                        await Task.Run(() => logInProcess.BrowserManager.SetAccount(accountModel));
                        await Task.Run(() => logInProcess.BrowserManager.LoadPageSource(accountModel, "https://www.youtube.com/channel_switcher?next=%2Faccount&feature=settings", timeSec: 6));
                        await Task.Run(() => channels = new OwnChannelScraperResponseHandler(new ResponseParameter { Response = logInProcess.BrowserManager.CurrentData }));
                        await Task.Delay(3000, token);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
                else
                {
                    var youTubeFunc = _accountScopeFactory[accountModel.AccountId].Resolve<IYoutubeFunctionality>();
                    //youTubeFunc.HttpHelper.GetRequestParameter().Cookies = accountModel.Cookies;
                    channels = await youTubeFunc.ScrapOwnChannels(accountModel);
                }

                accountModel.DisplayColumnValue1 = channels.ListChannels.Count;
                accountModel.DisplayColumnValue2 = null;
                accountModel.DisplayColumnValue3 = null;

                var currentChannel =
                    channels.ListChannels.FirstOrDefault(x => x.ChannelId == accountModel.AccountBaseModel.ProfileId);
                if (currentChannel != null)
                    accountModel.AccountBaseModel.UserFullName = currentChannel.ChannelName;

                await Task.Run(() => AddToDailyGrowth(accountModel.AccountId, 0, 0, channels.ListChannels.Count));
                var dbAccountService =
                     InstanceProvider.ResolveAccountDbOperations(accountModel.AccountId, _networks);
                dbAccountService.RemoveAll<OwnChannels>();
                await Task.Run(() => channels.ListChannels.ForEach(x =>
                  {
                      x = logInProcess.BrowserManager.GetChannelDetails(x.ChannelUrl).YoutubeChannel;
                      try
                      {
                          var ownChannels = new OwnChannels
                          {
                              ChannelName = x.ChannelUsername,
                              SubscribersCount = x.SubscriberCount,
                              VideosCount = x.VideosCount,
                              PageId = x.ChannelId,
                              IsSelected = accountModel.AccountBaseModel.UserId.Equals(x.ChannelId)
                          };
                          if (ownChannels.IsSelected && !string.IsNullOrEmpty(ownChannels.ChannelName))
                              accountModel.AccountBaseModel.ProfileId = ownChannels.ChannelName;
                          dbAccountService.Add(ownChannels);
                      }
                      catch (Exception ex)
                      {
                          ex.DebugLog();
                      }
                  }));

                SocinatorAccountBuilder.Instance(accountModel.AccountBaseModel.AccountId)
                    .UpdateLastUpdateTime(DateTime.Now.ConvertToEpoch())
                    .SaveToBinFile();
                GlobusLogHelper.log.Info(Log.DetailsUpdated,
                    accountModel.AccountBaseModel.AccountNetwork,
                    accountModel.AccountBaseModel.UserName, "Account Details");
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally
            {
                logInProcess?.BrowserManager.CloseBrowser();
                if (accountModel.AccountBaseModel.Status == AccountStatus.UpdatingDetails)
                    accountModel.AccountBaseModel.Status = AccountStatus.Success;
                accountModel.AccountBaseModel.NeedToCloseBrowser = true;
                YdUpdateSemaphoreSlim.Release();
            }
        }

        public DailyStatisticsViewModel GetDailyGrowth(string accountId, string username, GrowthPeriod period)
        {
            var response = new DailyStatisticsViewModel();
            try
            {
                var dbGrowthOperations = InstanceProvider.ResolveAccountDbOperations(accountId, _networks);
                List<DailyStatitics> growthList;
                if (period == GrowthPeriod.Daily)
                    growthList = dbGrowthOperations.Get<DailyStatitics>()
                        .Where(x => x.Date >= DateTime.Today.AddDays(-1)).OrderBy(x => x.Date).ToList();
                else if (period == GrowthPeriod.Weekly)
                    growthList = dbGrowthOperations.Get<DailyStatitics>()
                        .Where(x => x.Date >= DateTime.Today.AddDays(-7)).OrderBy(x => x.Date).ToList();
                else if (period == GrowthPeriod.Monthly)
                    growthList = dbGrowthOperations.Get<DailyStatitics>()
                        .Where(x => x.Date >= DateTime.Today.AddMonths(-1)).OrderBy(x => x.Date).ToList();
                else
                    growthList = dbGrowthOperations.Get<DailyStatitics>().OrderBy(x => x.Date).ToList();
                if (growthList.Count > 0)
                {
                    var oldRecord = growthList.FirstOrDefault();
                    var latestRecord = growthList.LastOrDefault();
                    response.GrowthColumnValue1 = latestRecord.Views - oldRecord.Views;
                    response.GrowthColumnValue2 = latestRecord.Subscribers - oldRecord.Subscribers;
                    response.GrowthColumnValue4 = latestRecord.Channels - oldRecord.Channels;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                response = null;
            }

            return response;
        }

        public List<DailyStatisticsViewModel> GetDailyGrowthForAccount(string accountId, GrowthChartPeriod period)
        {
            var isMonthly = false;
            var responseList = new List<DailyStatisticsViewModel>();
            try
            {
                var dbGrowthOperations = InstanceProvider.ResolveAccountDbOperations(accountId, _networks);
                int counter;
                List<DailyStatitics> growthList;
                if (period == GrowthChartPeriod.PastDay)
                {
                    growthList = dbGrowthOperations.Get<DailyStatitics>()
                        .Where(x => x.Date >= DateTime.Today.AddDays(-1)).OrderBy(x => x.Date).ToList();
                    counter = 2;
                }
                else if (period == GrowthChartPeriod.Past3Months)
                {
                    counter = 3;
                    isMonthly = true;
                    growthList = dbGrowthOperations.Get<DailyStatitics>()
                        .Where(x => x.Date >= DateTime.Today.AddMonths(-3)).OrderBy(x => x.Date).ToList();

                    var q = from gt in growthList
                            let dt = gt.Date
                            group gt by new { m = dt.Month }
                        into g
                            select g.OrderByDescending(t => t.Date).FirstOrDefault();
                    growthList = q.ToList();
                }
                else if (period == GrowthChartPeriod.Past30Days)
                {
                    counter = 30;
                    growthList = dbGrowthOperations.Get<DailyStatitics>()
                        .Where(x => x.Date >= DateTime.Today.AddDays(-30)).OrderBy(x => x.Date).ToList();
                    var q = from gt in growthList
                            let dt = gt.Date
                            group gt by new { y = dt.Year, m = dt.Month, d = dt.Day }
                        into g
                            select g.OrderByDescending(t => t.Date).LastOrDefault();
                    growthList = q.ToList();
                }
                else if (period == GrowthChartPeriod.Past6Months)
                {
                    counter = 6;
                    isMonthly = true;
                    growthList = dbGrowthOperations.Get<DailyStatitics>()
                        .Where(x => x.Date >= DateTime.Today.AddMonths(-6)).OrderBy(x => x.Date).ToList();

                    var q = from gt in growthList
                            let dt = gt.Date
                            group gt by new { m = dt.Month }
                        into g
                            select g.OrderByDescending(t => t.Date).LastOrDefault();
                    growthList = q.ToList();
                }
                else if (period == GrowthChartPeriod.PastWeek)
                {
                    counter = 7;

                    growthList = dbGrowthOperations.Get<DailyStatitics>()
                        .Where(x => x.Date >= DateTime.Today.AddDays(-7)).OrderBy(x => x.Date).ToList();
                    var q = from gt in growthList
                            let dt = gt.Date
                            group gt by new { y = dt.Year, m = dt.Month, d = dt.Day }
                        into g
                            select g.OrderByDescending(t => t.Date).LastOrDefault();
                    growthList = q.ToList();
                }
                else
                {
                    growthList = dbGrowthOperations.Get<DailyStatitics>().OrderBy(x => x.Date).ToList();
                    if (growthList.FirstOrDefault()?.Date < growthList.LastOrDefault()?.Date.AddDays(-30))
                    {
                        var q = from gt in growthList
                                let dt = gt.Date
                                group gt by new { m = dt.Month }
                            into g
                                select g.OrderByDescending(t => t.Date).LastOrDefault();
                        growthList = q.ToList();
                        isMonthly = true;
                    }
                    else
                    {
                        var q = from gt in growthList
                                let dt = gt.Date
                                group gt by new { y = dt.Year, m = dt.Month, d = dt.Day }
                            into g
                                select g.OrderByDescending(t => t.Date).LastOrDefault();
                        growthList = q.ToList();
                    }

                    counter = growthList.Count;
                }

                if (growthList.Count > 0)
                {
                    var lastAvailableRecord = new DailyStatitics();
                    var currentDate = DateTime.Now.Date;
                    for (var i = counter - 1; i >= 0; i--)
                    {
                        var setToZero = false;
                        var growthToAdd = new DailyStatisticsViewModel();

                        var record = isMonthly
                            ? growthList.FirstOrDefault(x => x.Date.Month == currentDate.AddMonths(-i).Month)
                            : growthList.FirstOrDefault(x => x.Date.Date == currentDate.AddDays(-i).Date);
                        if (record != null)
                        {
                            growthToAdd.Date = record.Date;
                            lastAvailableRecord = record;
                        }
                        else
                        {
                            if (lastAvailableRecord.Id != 0)
                            {
                                record = growthList.FirstOrDefault(x => x.Date.Date > lastAvailableRecord.Date.Date);
                            }
                            else
                            {
                                setToZero = true;
                                record = new DailyStatitics();
                            }

                            growthToAdd.Date = currentDate.AddDays(-i);
                        }

                        growthToAdd.GrowthColumnValue1 = setToZero || record == null ? 0 : record.Channels;
                        growthToAdd.GrowthColumnValue2 = setToZero || record == null ? 0 : record.Views;
                        growthToAdd.GrowthColumnValue3 = setToZero || record == null ? 0 : record.Subscribers;
                        responseList.Add(growthToAdd);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return responseList;
        }

        private void UpdateFailed(DominatorAccountModel accountModel, bool showLog = true)
        {
            if (accountModel.AccountBaseModel.Status == AccountStatus.TryingToLogin)
                accountModel.AccountBaseModel.Status = AccountStatus.Failed;
            accountModel.Cookies = new CookieCollection();
            accountModel.IsVerificationCodeSent = false;
            SocinatorAccountBuilder.Instance(accountModel.AccountBaseModel.AccountId)
                .AddOrUpdateDominatorAccountBase(accountModel.AccountBaseModel)
                .AddOrUpdateLoginStatus(accountModel.IsUserLoggedIn)
                .UpdateLastUpdateTime(DateTime.Now.ConvertToEpoch())
                .SaveToBinFile();

            if (showLog)
                GlobusLogHelper.log.Info(Log.LoginFailed, accountModel.AccountBaseModel.AccountNetwork,
                    accountModel.AccountBaseModel.UserName, accountModel.AccountBaseModel.Status);
        }

        private void UpdateGlobalAccountDetails(DominatorAccountModel dominatorAccountModel)
        {
            try
            {
                var globalDbOperation = new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection());

                globalDbOperation.UpdateAccountDetails(dominatorAccountModel);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        public void BrowserLogin(DominatorAccountModel dominatorAccountModel)
        {
            try
            {
                if (dominatorAccountModel.Token.IsCancellationRequested)
                    return;
                lock (YdStatic.BrowserLock)
                {
                    if (dominatorAccountModel.Token.IsCancellationRequested)
                        return;

                    if (++YdStatic.BrowserOpening > 6)
                        Monitor.Wait(YdStatic.BrowserLock);
                }

                dominatorAccountModel.Token.ThrowIfCancellationRequested();

                OpenBrowserToLogin(dominatorAccountModel);
            }
            catch (OperationCanceledException)
            {
                lock (YdStatic.BrowserLock)
                {
                    --YdStatic.BrowserOpening;
                    Monitor.Pulse(YdStatic.BrowserLock);
                }
            }
        }

        private void OpenBrowserToLogin(DominatorAccountModel dominatorAccountModel)
        {
            BrowserWindow browserWindow = null;
            try
            {
                var model = dominatorAccountModel;
                dominatorAccountModel.Token.ThrowIfCancellationRequested();
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    browserWindow = new BrowserWindow(model)
                    {
                        //Visibility = Visibility.Visible
                        Visibility = Visibility.Hidden
                    };
                    await browserWindow.SetCookie();
                    browserWindow.Show();
                });

                var last3Min = DateTime.Now;
                while (true)
                {
                    dominatorAccountModel.Token.ThrowIfCancellationRequested();
                    if (!browserWindow.VerifyingAccount &&
                        browserWindow?.DominatorAccountModel?.AccountBaseModel?.Status !=
                        AccountStatus.TryingToLogin || last3Min.AddMinutes(3.5) < DateTime.Now)
                    {
                        if (browserWindow.DominatorAccountModel != null)
                            // ReSharper disable once RedundantAssignment
                            dominatorAccountModel = browserWindow.DominatorAccountModel;
                        break;
                    }

                    Thread.Sleep(2000);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally
            {
                try
                {
                    if (browserWindow != null)
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            browserWindow.Close();
                            browserWindow.Dispose();
                        });

                    lock (YdStatic.BrowserLock)
                    {
                        --YdStatic.BrowserOpening;
                        Monitor.Pulse(YdStatic.BrowserLock);
                    }
                }
                catch (Exception e)
                {
                    e.DebugLog();
                }
            }
        }

        public bool AddToDailyGrowth(string accountId, int views, int subscribers, int channels)
        {
            var success = true;
            try
            {
                var dbGrowthOperations = InstanceProvider.ResolveAccountDbOperations(accountId, _networks);
                var existingDataForToday = dbGrowthOperations.GetSingle<DailyStatitics>(x => x.Date == DateTime.Today);
                if (existingDataForToday != null)
                {
                    existingDataForToday.Date = DateTime.Now;
                    existingDataForToday.Views = views;
                    existingDataForToday.Subscribers = subscribers;
                    existingDataForToday.Channels = channels;
                    dbGrowthOperations.Update(existingDataForToday);
                }
                else
                {
                    if (dbGrowthOperations.Get<DailyStatitics>().Count == 0)
                    {
                        dbGrowthOperations.Add(new DailyStatitics
                        {
                            Date = DateTime.Now.AddDays(-1),
                            Views = views,
                            Subscribers = subscribers,
                            Channels = channels
                        });
                        dbGrowthOperations.Add(new DailyStatitics
                        {
                            Date = DateTime.Now,
                            Views = views,
                            Subscribers = subscribers,
                            Channels = channels
                        });
                    }
                    else
                    {
                        dbGrowthOperations.Add(new DailyStatitics
                        {
                            Date = DateTime.Now,
                            Views = views,
                            Subscribers = subscribers,
                            Channels = channels
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                success = false;
            }

            return success;
        }

        public bool SolveCaptchaManually(DominatorAccountModel accountModel)
        {
            return false;
        }
    }
}