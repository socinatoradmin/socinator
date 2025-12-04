using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.QdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using QuoraDominatorCore.QdLibrary;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using QuoraDominatorCore.QdUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity;

namespace QuoraDominatorCore.QDFactories
{
    public interface IQDAccountUpdateFactory : IAccountUpdateFactoryAsync
    {
        Task UpdateDetailsAsync(DominatorAccountModel accountModel, CancellationToken token);
    }

    public class QdAccountUpdateFactory : IQDAccountUpdateFactory
    {
        private static QdAccountUpdateFactory _instance;
        private DbOperations _dbGrowthOperations;

        public QdAccountUpdateFactory(IAccountScopeFactory accountScopeFactory)
        {
            _accountScopeFactory = accountScopeFactory;
            _networks = SocialNetworks.Quora;
        }

        private QdAccountUpdateFactory()
        {
        }

        public static QdAccountUpdateFactory Instance
            => _instance ?? (_instance = new QdAccountUpdateFactory());

        public bool CheckStatus(DominatorAccountModel accountModel)
        {
            return CheckStatusAsync(accountModel, accountModel.Token).Result;
        }

        public void UpdateDetails(DominatorAccountModel accountModel)
        {
            UpdateDetailsAsync(accountModel, accountModel.Token);
        }

        public async Task<bool> CheckStatusAsync(DominatorAccountModel accountModel, CancellationToken token)
        {
            try
            {
                var login = _accountScopeFactory[accountModel.AccountId].Resolve<IQdLogInProcess>();
                if (!accountModel.IsRunProcessThroughBrowser)
                    await login.LoginUsingGlobusHttpQuoraAsync(accountModel, token);

                else
                    login.LoginWithBrowserMethod(accountModel, token);
                if (accountModel.AccountBaseModel.Status == AccountStatus.Success)
                    return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        public Task UpdateDetailsAsync(DominatorAccountModel accountModel, CancellationToken token)
        {
            return UpdateAccountFollowerFollowing(accountModel, token);
        }

        public DailyStatisticsViewModel GetDailyGrowth(string accountId, string username, GrowthPeriod period)
        {
            var response = new DailyStatisticsViewModel();
            try
            {
                _dbGrowthOperations = new DbOperations(accountId, SocialNetworks.Quora, ConstantVariable.GetAccountDb);

                List<DailyStatitics> growthList;
                if (period == GrowthPeriod.Daily)
                    growthList = _dbGrowthOperations.Get<DailyStatitics>()
                        .Where(x => x.Date >= DateTime.Today.AddDays(-1)).OrderBy(x => x.Date).ToList();
                else if (period == GrowthPeriod.Weekly)
                    growthList = _dbGrowthOperations.Get<DailyStatitics>()
                        .Where(x => x.Date >= DateTime.Today.AddDays(-7)).OrderBy(x => x.Date).ToList();
                else if (period == GrowthPeriod.Monthly)
                    growthList = _dbGrowthOperations.Get<DailyStatitics>()
                        .Where(x => x.Date >= DateTime.Today.AddMonths(-1)).OrderBy(x => x.Date).ToList();
                else
                    growthList = _dbGrowthOperations.Get<DailyStatitics>().OrderBy(x => x.Date).ToList();
                if (growthList.Count > 0)
                {
                    var oldRecord = growthList.FirstOrDefault();
                    var latestRecord = growthList.LastOrDefault();
                    if (latestRecord != null && oldRecord != null)
                    {
                        response.GrowthColumnValue1 = latestRecord.Followers - oldRecord.Followers;
                        response.GrowthColumnValue2 = latestRecord.Followings - oldRecord.Followings;
                        response.GrowthColumnValue3 = latestRecord.Uploads - oldRecord.Uploads;
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
            var responseList = new List<DailyStatisticsViewModel>();
            try
            {
                _dbGrowthOperations = new DbOperations(accountId, SocialNetworks.Quora, ConstantVariable.GetAccountDb);

                int counter;
                List<DailyStatitics> growthList;
                if (period == GrowthChartPeriod.PastDay)
                {
                    growthList = _dbGrowthOperations.Get<DailyStatitics>()
                        .Where(x => x.Date >= DateTime.Today.AddDays(-1)).OrderBy(x => x.Date).ToList();
                    counter = 2;
                }
                else if (period == GrowthChartPeriod.Past3Months)
                {
                    counter = 3;
                    isMonthly = true;
                    growthList = _dbGrowthOperations.Get<DailyStatitics>()
                        .Where(x => x.Date >= DateTime.Today.AddMonths(-3)).OrderBy(x => x.Date).ToList();

                    var q = from gt in growthList
                        let dt = gt.Date
                        group gt by new {m = dt.Month}
                        into g
                        select g.OrderByDescending(t => t.Date).FirstOrDefault();
                    growthList = q.ToList();
                }
                else if (period == GrowthChartPeriod.Past30Days)
                {
                    counter = 30;
                    growthList = _dbGrowthOperations.Get<DailyStatitics>()
                        .Where(x => x.Date >= DateTime.Today.AddDays(-30)).OrderBy(x => x.Date).ToList();

                    var q = from gt in growthList
                        let dt = gt.Date
                        group gt by new {y = dt.Year, m = dt.Month, d = dt.Day}
                        into g
                        select g.OrderByDescending(t => t.Date).LastOrDefault();
                    growthList = q.ToList();
                }
                else if (period == GrowthChartPeriod.Past6Months)
                {
                    counter = 6;
                    isMonthly = true;
                    growthList = _dbGrowthOperations.Get<DailyStatitics>()
                        .Where(x => x.Date >= DateTime.Today.AddMonths(-6)).OrderBy(x => x.Date).ToList();

                    var q = from gt in growthList
                        let dt = gt.Date
                        group gt by new {m = dt.Month}
                        into g
                        select g.OrderByDescending(t => t.Date).LastOrDefault();
                    growthList = q.ToList();
                }
                else if (period == GrowthChartPeriod.PastWeek)
                {
                    counter = 7;

                    growthList = _dbGrowthOperations.Get<DailyStatitics>()
                        .Where(x => x.Date >= DateTime.Today.AddDays(-7)).OrderBy(x => x.Date).ToList();
                    var q = from gt in growthList
                        let dt = gt.Date
                        group gt by new {y = dt.Year, m = dt.Month, d = dt.Day}
                        into g
                        select g.OrderByDescending(t => t.Date).LastOrDefault();
                    growthList = q.ToList();
                }
                else
                {
                    growthList = _dbGrowthOperations.Get<DailyStatitics>().OrderBy(x => x.Date).ToList();
                    if (growthList.FirstOrDefault()?.Date < growthList.LastOrDefault()?.Date.AddDays(-30))
                    {
                        var q = from gt in growthList
                            let dt = gt.Date
                            group gt by new {m = dt.Month}
                            into g
                            select g.OrderByDescending(t => t.Date).LastOrDefault();
                        growthList = q.ToList();
                        isMonthly = true;
                    }
                    else
                    {
                        var q = from gt in growthList
                            let dt = gt.Date
                            group gt by new {y = dt.Year, m = dt.Month, d = dt.Day}
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

        private async Task<bool> UpdateAccountFollowerFollowing(DominatorAccountModel objDominatorAccountModel,
            CancellationToken cancellationToken)
        {
            try
            {
                if (!(objDominatorAccountModel.IsUserLoggedIn || AccountStatus.Success==objDominatorAccountModel.AccountBaseModel.Status))
                {
                    var logInProcess = _accountScopeFactory[objDominatorAccountModel.AccountId]
                        .Resolve<IQdLogInProcess>();
                    cancellationToken.ThrowIfCancellationRequested();
                    await logInProcess.LoginUsingGlobusHttpQuoraAsync(objDominatorAccountModel, cancellationToken);
                }
                var IsBrowser = objDominatorAccountModel.IsRunProcessThroughBrowser;
                var dbAccountService =
                    InstanceProvider.ResolveAccountDbOperations(objDominatorAccountModel.AccountId, _networks);
                var quoraFunct = _accountScopeFactory[objDominatorAccountModel.AccountId].Resolve<IQuoraFunctions>();
                var browser = _accountScopeFactory[objDominatorAccountModel.AccountId].Resolve<IQuoraBrowserManager>();
                var profileUrl = IsBrowser ?
                     browser.GetUserProfile(objDominatorAccountModel)
                    :quoraFunct.GetProfileUrl(objDominatorAccountModel);
                objDominatorAccountModel.AccountBaseModel.UserFullName =
                    profileUrl.Replace($"{QdConstants.HomePageUrl}/profile/", "");

                GlobusLogHelper.log.Info(Log.CustomMessage, objDominatorAccountModel.AccountBaseModel.AccountNetwork,
                    objDominatorAccountModel.UserName, "Friendship update", "Start updating Friendship count");

                cancellationToken.ThrowIfCancellationRequested();
                var userInfo =
                    IsBrowser ?
                    browser.GetUserInfo(objDominatorAccountModel, profileUrl,true)
                    :await quoraFunct.UserInfoAsync(objDominatorAccountModel, profileUrl);
                cancellationToken.ThrowIfCancellationRequested();
                if (IsBrowser)
                    browser?.CloseBrowser();
                objDominatorAccountModel.DisplayColumnValue1 = userInfo.FollowerCount;
                objDominatorAccountModel.DisplayColumnValue2 = userInfo.FollowingCount;
                objDominatorAccountModel.DisplayColumnValue3 = userInfo.UserPostsCount;

                cancellationToken.ThrowIfCancellationRequested();
                AddToDailyGrowth(objDominatorAccountModel.AccountBaseModel.UserId, objDominatorAccountModel.UserName,
                    userInfo.FollowerCount, userInfo.FollowingCount, userInfo.UserPostsCount, objDominatorAccountModel);

                cancellationToken.ThrowIfCancellationRequested();
                SocinatorAccountBuilder.Instance(objDominatorAccountModel.AccountBaseModel.AccountId)
                    .AddOrUpdateDominatorAccountBase(objDominatorAccountModel.AccountBaseModel)
                    .AddOrUpdateDisplayColumn1(objDominatorAccountModel.DisplayColumnValue1)
                    .AddOrUpdateDisplayColumn2(objDominatorAccountModel.DisplayColumnValue2)
                    .AddOrUpdateDisplayColumn3(objDominatorAccountModel.DisplayColumnValue3)
                    .SaveToBinFile();

                GlobusLogHelper.log.Info(Log.CustomMessage, objDominatorAccountModel.AccountBaseModel.AccountNetwork,
                    objDominatorAccountModel.UserName, "Friendship update", "Successfully updated the friendship");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return true;
            }

            return true;
        }

        public bool AddToDailyGrowth(string userId, string username, int followers, int following, int posts,
            DominatorAccountModel dominatorAccountModel)
        {
            var success = !(string.IsNullOrEmpty(userId.Trim()) && string.IsNullOrEmpty(username.Trim()));

            try
            {
                var dbAccountService =
                    InstanceProvider.ResolveAccountDbOperations(dominatorAccountModel.AccountId, _networks);
                dbAccountService.Add(new DailyStatitics
                {
                    Date = DateTime.Now,
                    Followers = followers,
                    Followings = following,
                    Uploads = posts
                });
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
            return true;
        }

        #region Private fields

        private readonly IAccountScopeFactory _accountScopeFactory;
        private readonly SocialNetworks _networks;

        #endregion
    }
}