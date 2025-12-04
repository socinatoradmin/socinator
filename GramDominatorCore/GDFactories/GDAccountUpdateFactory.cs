using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using GramDominatorCore.GDLibrary;
using GramDominatorCore.GDModel;
using GramDominatorCore.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity;

namespace GramDominatorCore.GDFactories
{
    public interface IGDAccountUpdateFactory : IAccountUpdateFactoryAsync
    {
        // bool CheckStatus(DominatorAccountModel accountModel);
        //Task<bool> CheckStatusAsync(DominatorAccountModel accountModel, CancellationToken token);
        //void UpdateDetails(DominatorAccountModel accountModel);
        //Task UpdateDetailsAsync(DominatorAccountModel accountModel, CancellationToken token);
        bool AddToDailyGrowth(string accountId, int followers, int followings, int uploads);
       // List<DailyStatisticsViewModel> GetDailyGrowthForAccount(string accountId, GrowthChartPeriod period);
    }
    public class GDAccountUpdateFactory : IGDAccountUpdateFactory
    {

        private static GDAccountUpdateFactory _instance;

        private DbOperations dbGrowthOperations;

        public IGdHttpHelper httpHelper;
        public IGdLogInProcess iGdLogInProcess;
        public IInstaFunction instaFunction;
        private IAccountUpdateProcess AccountUpdateProcess;
        private readonly IAccountScopeFactory _accountScopeFactory;

        public static GDAccountUpdateFactory Instance => _instance ?? (_instance = InstanceProvider.GetInstance<GDAccountUpdateFactory>());

        public GDAccountUpdateFactory(IAccountScopeFactory accountScopeFactory)
        {
            _accountScopeFactory = accountScopeFactory;
           // _networks = SocialNetworks.Twitter;
        }

        public bool CheckStatus(DominatorAccountModel accountModel)
        {
            try
            {
                return CheckStatusAsync(accountModel, accountModel.Token).Result;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        public async Task<bool> CheckStatusAsync(DominatorAccountModel accountModel, CancellationToken token)
        {           
            GlobusLogHelper.log.Info(Log.AccountLogin, accountModel.AccountBaseModel.AccountNetwork, accountModel.UserName);
            if(accountModel.UpgradeVersion<2)
            {
                var deviceId = accountModel.DeviceDetails.DeviceId;
                accountModel.DeviceDetails = new DeviceGenerator();
                accountModel.DeviceDetails.DeviceId = deviceId;
                accountModel.UpgradeVersion = 2;
                accountModel.UserAgentMobile = accountModel.DeviceDetails.Useragent;
               
            }
            //accountModel.UserAgentMobile =string.IsNullOrEmpty(accountModel.UserAgentWeb)?accountModel.UserAgentMobile: accountModel.UserAgentWeb;
            
            try
            {
                var gdLogInProcess = _accountScopeFactory[accountModel.AccountId].Resolve<IGdLogInProcess>();//InstanceProvider.GetInstance<IGdLogInProcess>();
                var isLoggedIn =  await gdLogInProcess.CheckLoginAsync(accountModel, token,loginType:LoginType.CheckLogin);
         //       return isLoggedIn;
                if (isLoggedIn)
                {
                    AccountModel accountBaseModel = new AccountModel(accountModel);
                    if (!InstagramDataScraper(accountModel, accountBaseModel))
                    {
                        accountModel.AccountBaseModel.Status = accountModel.AccountBaseModel.Status != AccountStatus.InvalidCredentials ? AccountStatus.Success:accountModel.AccountBaseModel.Status;
                        return isLoggedIn;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }
        public void UpdateDetails(DominatorAccountModel accountModel)
        {
            try
            {
                UpdateDetailsAsync(accountModel, accountModel.Token).Wait();
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public async Task UpdateDetailsAsync(DominatorAccountModel accountModel, CancellationToken token)
        {
            try
            {
                
                iGdLogInProcess = _accountScopeFactory[accountModel.AccountId].Resolve<IGdLogInProcess>();
                AccountUpdateProcess = _accountScopeFactory[accountModel.AccountId].Resolve<IAccountUpdateProcess>();
                if (accountModel.IsUserLoggedIn)
                   await AccountUpdateProcess.UpdateAccountAsync(accountModel, token);
                else
                {
                   // httpHelper = ((LogInProcess) AccountUpdateProcess).GetHttpHelper();      
                    if(!accountModel.IsUserLoggedIn)
                        await iGdLogInProcess.CheckLoginAsync(accountModel, token);
                    
                    await AccountUpdateProcess.UpdateAccountAsync(accountModel, token);                    
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public bool AddToDailyGrowth(string accountId, int followers, int followings, int uploads)
        {
            bool success = true;
            try
            {
                dbGrowthOperations = new DbOperations(accountId, SocialNetworks.Instagram, ConstantVariable.GetAccountDb);

                //var existingDataForToday = dbGrowthOperations.GetSingle<DailyStatitics>(
                //                            x => x.Date.Year == DateTime.Today.Year && x.Date.Month == DateTime.Today.Month && x.Date.Day == DateTime.Today.Day);
                var existingDataForToday = dbGrowthOperations.Get<DailyStatitics>().FirstOrDefault(
                    x => x.Date.Year == DateTime.Today.Year && x.Date.Month == DateTime.Today.Month && x.Date.Day == DateTime.Today.Day);
                if (existingDataForToday != null)
                {
                    existingDataForToday.Date = DateTime.Now;
                    existingDataForToday.Followers = followers;
                    existingDataForToday.Followings = followings;
                    existingDataForToday.Uploads = uploads;
                    dbGrowthOperations.Update(existingDataForToday);
                }
                else
                {
                    if (dbGrowthOperations.Get<DailyStatitics>().Count == 0)
                    {
                        dbGrowthOperations.Add(new DailyStatitics()
                        {
                            Date = DateTime.Now.AddDays(-1),
                            Followers = followers,
                            Followings = followings,
                            Uploads = uploads
                        });
                        dbGrowthOperations.Add(new DailyStatitics()
                        {
                            Date = DateTime.Now,
                            Followers = followers,
                            Followings = followings,
                            Uploads = uploads
                        });
                    }
                    else
                    {
                        dbGrowthOperations.Add(new DailyStatitics()
                        {
                            Date = DateTime.Now,
                            Followers = followers,
                            Followings = followings,
                            Uploads = uploads
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

        DailyStatisticsViewModel IAccountUpdateFactory.GetDailyGrowth(string accountId, string username, GrowthPeriod period)
        {
            var response = new DailyStatisticsViewModel();
            List<DailyStatitics> growthList;
            try
            {

                dbGrowthOperations = new DbOperations(accountId, SocialNetworks.Instagram, ConstantVariable.GetAccountDb);

                if (period == GrowthPeriod.Daily)
                    growthList = dbGrowthOperations.Get<DailyStatitics>().Where(x => x.Date >= DateTime.Today.AddDays(-1)).OrderBy(x => x.Date).ToList();               
                else if (period == GrowthPeriod.Weekly)
                    growthList = dbGrowthOperations.Get<DailyStatitics>().Where(x => x.Date >= DateTime.Today.AddDays(-7)).OrderBy(x => x.Date).ToList();               
                else if (period == GrowthPeriod.Monthly)
                    growthList = dbGrowthOperations.Get<DailyStatitics>().Where(x => x.Date >= DateTime.Today.AddMonths(-1)).OrderBy(x => x.Date).ToList();             
                else
                    growthList = dbGrowthOperations.Get<DailyStatitics>().OrderBy(x => x.Date).ToList();
                
                if (growthList.Count > 0)
                {
                    var oldRecord = growthList.FirstOrDefault();
                    var LatestRecord = growthList.LastOrDefault();
                    if (LatestRecord != null)
                    {
                        if (oldRecord != null)
                        {
                            response.GrowthColumnValue1 = LatestRecord.Followers - oldRecord.Followers;
                            response.GrowthColumnValue2 = LatestRecord.Followings - oldRecord.Followings;
                            response.GrowthColumnValue4 = LatestRecord.Uploads - oldRecord.Uploads;
                        }
                    }
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
            List<DailyStatitics> growthList;
            var responseList = new List<DailyStatisticsViewModel>();
            try
            {
                dbGrowthOperations = new DbOperations(accountId, SocialNetworks.Instagram, ConstantVariable.GetAccountDb);

                int counter;
                if (period == GrowthChartPeriod.PastDay)
                {
                    growthList = dbGrowthOperations.Get<DailyStatitics>().Where(x => x.Date >= DateTime.Today.AddDays(-1)).OrderBy(x => x.Date).ToList();
                    counter = 2;

                }
                else if (period == GrowthChartPeriod.Past3Months)
                {
                    counter = 3;
                    isMonthly = true;
                    growthList = dbGrowthOperations.Get<DailyStatitics>().Where(x => x.Date >= DateTime.Today.AddMonths(-3)).OrderBy(x => x.Date).ToList();

                    var q = from gt in growthList
                            let dt = gt.Date
                            group gt by new { m = dt.Month } into g
                            select g.OrderByDescending(t => t.Date).FirstOrDefault();
                    growthList = q.ToList();
                }
                else if (period == GrowthChartPeriod.Past30Days)
                {
                    counter = 30;
                    growthList = dbGrowthOperations.Get<DailyStatitics>().Where(x => x.Date >= DateTime.Today.AddDays(-30)).OrderBy(x => x.Date).ToList();
                    var q = from gt in growthList
                            let dt = gt.Date
                            group gt by new { y = dt.Year, m = dt.Month, d = dt.Day } into g
                            select g.OrderByDescending(t => t.Date).LastOrDefault();
                    growthList = q.ToList();
                }
                else if (period == GrowthChartPeriod.Past6Months)
                {
                    counter = 6;
                    isMonthly = true;
                    growthList = dbGrowthOperations.Get<DailyStatitics>().Where(x => x.Date >= DateTime.Today.AddMonths(-6)).OrderBy(x => x.Date).ToList();

                    var q = from gt in growthList
                            let dt = gt.Date
                            group gt by new { m = dt.Month } into g
                            select g.OrderByDescending(t => t.Date).LastOrDefault();
                    growthList = q.ToList();
                }
                else if (period == GrowthChartPeriod.PastWeek)
                {
                    counter = 7;

                    growthList = dbGrowthOperations.Get<DailyStatitics>().Where(x => x.Date >= DateTime.Today.AddDays(-7)).OrderBy(x => x.Date).ToList();
                    var q = from gt in growthList
                            let dt = gt.Date
                            group gt by new { y = dt.Year, m = dt.Month, d = dt.Day } into g
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
                                group gt by new { m = dt.Month } into g
                                select g.OrderByDescending(t => t.Date).LastOrDefault();
                        growthList = q.ToList();
                        isMonthly = true;
                    }
                    else
                    {
                        var q = from gt in growthList
                                let dt = gt.Date
                                group gt by new { y = dt.Year, m = dt.Month, d = dt.Day } into g
                                select g.OrderByDescending(t => t.Date).LastOrDefault();
                        growthList = q.ToList();
                    }
                    counter = growthList.Count;
                }
                if (growthList.Count > 0)
                {
                    var lastAvailableRecord = new DailyStatitics();
                    var currentDate = DateTime.Now.Date;
                    for (int i = counter - 1; i >= 0; i--)
                    {
                        var setToZero = false;
                        var growthToAdd = new DailyStatisticsViewModel();

                        var record = isMonthly ? growthList.FirstOrDefault(x => x.Date.Month == currentDate.AddMonths(-i).Month) : growthList.FirstOrDefault(x => x.Date.Date == currentDate.AddDays(-i).Date);
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

                        growthToAdd.GrowthColumnValue1 = setToZero || record == null ? 0 : record.Followers;
                        growthToAdd.GrowthColumnValue2 = setToZero || record == null ? 0 : record.Followings;
                        growthToAdd.GrowthColumnValue3 = setToZero || record == null ? 0 : record.Uploads;
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

        public bool SolveCaptchaManually(DominatorAccountModel accountModel)
        {
            accountModel.AccountBaseModel.Status = AccountStatus.FoundCaptcha;
            try
            {
                return SolveCaptchaManuallyAsync(accountModel, accountModel.Token).Result;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        private async Task<bool> SolveCaptchaManuallyAsync(DominatorAccountModel accountModel, CancellationToken token)
        {
            GlobusLogHelper.log.Info(Log.AccountLogin, accountModel.AccountBaseModel.AccountNetwork, accountModel.UserName);
            accountModel.UserAgentMobile = string.IsNullOrEmpty(accountModel.UserAgentMobile) ? accountModel.DeviceDetails.Useragent : accountModel.UserAgentMobile;
            accountModel.UserAgentMobile = string.IsNullOrEmpty(accountModel.UserAgentWeb) ? accountModel.UserAgentMobile : accountModel.UserAgentWeb;

            try
            {
                var gdLogInProcess = _accountScopeFactory[accountModel.AccountId].Resolve<IGdLogInProcess>();//InstanceProvider.GetInstance<IGdLogInProcess>();
                return await gdLogInProcess.ManualCaptchaAsync(accountModel, token);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        private bool InstagramDataScraper(DominatorAccountModel dominatorAccountModel, AccountModel accountModel)
        {
            try
            {
                if (dominatorAccountModel.IsUserLoggedIn)
                {
//#if !DEBUG
//                    var instaAdsScrappers = _accountScopeFactory[dominatorAccountModel.AccountId].Resolve<IInstaAdScrappers>();
//                    Thread Ads = new Thread(() => instaAdsScrappers.InstaDataScraper(dominatorAccountModel, accountModel));
//                    Ads.Start();
//#endif
                    return false;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return true;
        }
    }
}
