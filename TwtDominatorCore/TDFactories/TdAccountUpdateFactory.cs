using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.TdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process.JobConfigurations;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using Newtonsoft.Json;
using TwtDominatorCore.Database;
using TwtDominatorCore.TDLibrary;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;
using Unity;
using static TwtDominatorCore.TDEnums.Enums;

namespace TwtDominatorCore.TDFactories
{
    public interface ITDAccountUpdateFactory : IAccountUpdateFactoryAsync
    {
        Task UpdatePostsAsync(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken, IDbAccountService dbAccountService);

        Task UpdateFollowersAsync(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken);

        void UpdateFollowers(DominatorAccountModel dominatorAccountModel, ITwitterFunctions twtFunc,
            int currentAlreadyInsertedCount = 0, int maxAlreadyInsertedCount = 50);

        Task UpdateFollowingsAsync(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken);

        void CheckIsFollowersUpdated(DominatorAccountModel dominatorAccountModel, int lastFollowersUpdatedTime,
            CancellationToken cancellationToken);

        void CheckIsFollowingsUpdated(DominatorAccountModel dominatorAccountModel, int lastFollowingsUpdatedTime,
            CancellationToken cancellationToken);

        Task UpdatePosts(DominatorAccountModel dominatorAccountModel,
            ITwitterFunctions twtFunc, IDbAccountService dbAccountService,bool IsTweetWithReply=true);

        Task UpdateAccountFollowCount(DominatorAccountModel dominatorAccountModel, AccountModel accountModel,
            string logInPageResponse = "");
    }

    public class TDAccountUpdateFactory : ITDAccountUpdateFactory
    {
        private void UpdateGlobalAccountDetails(DominatorAccountModel dominatorAccountModel)
        {
            var dataBaseConnectionGlb = SocinatorInitialize.GetGlobalDatabase();
            var dbOperationsGlobal = new DbOperations(dataBaseConnectionGlb.GetSqlConnection());
            dbOperationsGlobal.UpdateAccountDetails(dominatorAccountModel);
        }

        private void AddToDailyGrowth(string accountId, int followers, int following, int tweet)
        {
            try
            {
                var date = DateTime.Today;
                var dbGrowthOperations =
                    InstanceProvider.ResolveAccountDbOperations(accountId, SocialNetworks.Twitter);

                var existingDataForToday = dbGrowthOperations.GetSingle<DailyStatitics>(x => x.Date == date);
                if (existingDataForToday != null)
                {
                    existingDataForToday.Date = DateTime.Now;
                    existingDataForToday.Followers = followers;
                    existingDataForToday.Followings = following;
                    existingDataForToday.Tweets = tweet;
                    dbGrowthOperations.Update(existingDataForToday);
                }
                else
                {
                    if (dbGrowthOperations.Get<DailyStatitics>().Count == 0)
                    {
                        dbGrowthOperations.Add(new DailyStatitics
                        {
                            Date = DateTime.Now.AddDays(-1),
                            Followers = followers,
                            Followings = following,
                            Tweets = tweet
                        });
                        dbGrowthOperations.Add(new DailyStatitics
                        {
                            Date = DateTime.Now,
                            Followers = followers,
                            Followings = following,
                            Tweets = tweet
                        });
                    }
                    else
                    {
                        dbGrowthOperations.Add(new DailyStatitics
                        {
                            Date = DateTime.Now,
                            Followers = followers,
                            Followings = following,
                            Tweets = tweet
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static void IsInsertedFollowerToDb(DbInsertionHelper objDbInsertion, TwitterUser twtUser)
        {
            try
            {
                objDbInsertion.AddFriendshipData(twtUser, FollowType.Followers, 0);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #region Public fields

        public static List<string> UpdatingAccount = new List<string>();

        public TDAccountUpdateFactory(IAccountScopeFactory accountScopeFactory,
            IAccountsFileManager accountsFileManager, IDelayService threadUtility)
        {
            _accountScopeFactory = accountScopeFactory;
            _accountsFileManager = accountsFileManager;
            _networks = SocialNetworks.Twitter;
            _delayService = threadUtility;
            //var softwareSetting = InstanceProvider.GetInstance<ISoftwareSettingsFileManager>().GetSoftwareSettings();
        }

        #endregion

        #region Private fields

        private static TDAccountUpdateFactory _instance;
        private readonly IAccountScopeFactory _accountScopeFactory;
        private readonly IAccountsFileManager _accountsFileManager;
        private readonly SocialNetworks _networks;

        private readonly IDelayService _delayService;
        //private bool IsBrowser = false;

        #endregion


        #region Public properties

        /// <summary>
        ///     update account status after when already logged in successfully
        /// </summary>
        //public bool IsAlreadyLoggedIn { get; set; }

        public string LogInPageResponse { get; set; } = string.Empty;

        public static TDAccountUpdateFactory Instance =>
            _instance ?? (_instance = InstanceProvider.GetInstance<TDAccountUpdateFactory>());

        #endregion

        #region Public methods

        public bool CheckStatus(DominatorAccountModel dominatorAccountModel)
        {
            return CheckStatusAsync(dominatorAccountModel, dominatorAccountModel.Token).Result;
        }

        public bool SolveCaptchaManually(DominatorAccountModel accountModel)
        {
            return false;
        }

        /// <summary>
        ///     LogInPageResponse is empty then it check account
        ///     LogInPageResponse is non empty then update follower following after login pages
        /// </summary>
        /// <param name="dominatorAccountModel"></param>
        /// <param name="сancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> CheckStatusAsync(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken)
        {
            var logInProcess = _accountScopeFactory[dominatorAccountModel.AccountId].Resolve<ITwtLogInProcess>();
            var accountModel = new AccountModel(dominatorAccountModel);
            if (!logInProcess.CheckLoginAsync(dominatorAccountModel, cancellationToken).Result)
            {
                if (dominatorAccountModel.IsRunProcessThroughBrowser)
                    TdAccountsBrowserDetails.CloseBrowser(dominatorAccountModel,
                        BrowserInstanceType.CheckAccountStatus);
                return false;
            }

            await UpdateAccountFollowCount(dominatorAccountModel, accountModel);
            if (dominatorAccountModel.IsRunProcessThroughBrowser)
                TdAccountsBrowserDetails.CloseBrowser(dominatorAccountModel, BrowserInstanceType.CheckAccountStatus);
            return true;
        }

        public async Task UpdateAccountFollowCount(DominatorAccountModel dominatorAccountModel,
            AccountModel accountModel, string logInPageResponse = "")
        {
            try
            {
                if (dominatorAccountModel.AccountBaseModel.Status != AccountStatus.Success)
                    return;
                //getting account updated details
                var twitterFunctions = _accountScopeFactory[dominatorAccountModel.AccountId]
                    .Resolve<ITwitterFunctionFactory>().TwitterFunctions;
                var objAccountDetails =
                    await twitterFunctions.GetAccountDetailsAsync(dominatorAccountModel, dominatorAccountModel.Token,
                        LogInPageResponse);
                dominatorAccountModel.AccountBaseModel.UserId = objAccountDetails.UserId;
                dominatorAccountModel.AccountBaseModel.ProfileId = objAccountDetails.UserName;
                dominatorAccountModel.AccountBaseModel.UserFullName = objAccountDetails.ProfileName;
                accountModel.Email = objAccountDetails.Email;
                accountModel.IsPrivate = objAccountDetails.IsPrivate;


                dominatorAccountModel.DisplayColumnValue1 =
                    accountModel.FollowersCount = objAccountDetails.FollowerCount;
                dominatorAccountModel.DisplayColumnValue2 =
                    accountModel.FollowingCount = objAccountDetails.FollowingCount;
                dominatorAccountModel.DisplayColumnValue3 = accountModel.TweetsCount = objAccountDetails.TweetCount;

                dominatorAccountModel.Token.ThrowIfCancellationRequested();
                var privateDetailsValue = JsonConvert.SerializeObject(accountModel);
                AddToDailyGrowth(dominatorAccountModel.AccountBaseModel.AccountId, objAccountDetails.FollowerCount,
                    objAccountDetails.FollowingCount, objAccountDetails.TweetCount);

                #region  setting profile details saved in protobuf

                // if account ask for retype email or phone number we can verify internally 
                // using these saved details
                var serializeUserProfileData = "";
                try
                {
                    var logInProcess = _accountScopeFactory[dominatorAccountModel.AccountId]
                        .Resolve<ITwtLogInProcess>();
                    if (LogInProcess.HasUserProfileDetails(dominatorAccountModel))
                        await logInProcess.UpdatingUserProfileDetails(dominatorAccountModel);

                    var dominatorAccUserDetails = _accountsFileManager.GetAccountById(dominatorAccountModel.AccountId)
                        .ExtraParameters;
                    dominatorAccUserDetails.TryGetValue(ModuleExtraDetails.UserProfileDetails.ToString(),
                        out serializeUserProfileData);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion

                SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                    .AddOrUpdateDominatorAccountBase(dominatorAccountModel.AccountBaseModel)
                    .AddOrUpdateDisplayColumn1(dominatorAccountModel.DisplayColumnValue1)
                    .AddOrUpdateDisplayColumn2(dominatorAccountModel.DisplayColumnValue2)
                    .AddOrUpdateDisplayColumn3(dominatorAccountModel.DisplayColumnValue3)
                    .AddOrUpdateCookies(dominatorAccountModel.Cookies)
                    .AddOrUpdateExtraParameter(ModuleExtraDetails.ModulePrivateDetails.ToString(),
                        privateDetailsValue)
                    .AddOrUpdateExtraParameter(ModuleExtraDetails.UserProfileDetails.ToString(),
                        serializeUserProfileData)
                    .SaveToBinFile();
                UpdateGlobalAccountDetails(dominatorAccountModel);
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


        public void UpdateDetails(DominatorAccountModel dominatorAccountModel)
        {
            UpdateDetailsAsync(dominatorAccountModel, dominatorAccountModel.Token).Wait();
        }

        public async Task UpdateDetailsAsync(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken)
        {
            try
            {
                var logInProcess = _accountScopeFactory[dominatorAccountModel.AccountId].Resolve<ITwtLogInProcess>();
                var accountModel = new AccountModel(dominatorAccountModel);
                logInProcess.IsCheckAccountStatus = true;
                if (dominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    GlobusLogHelper.log.Info(Log.AccountLogin, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        dominatorAccountModel.UserName);
                    logInProcess.LoginWithBrowserMethod(dominatorAccountModel, cancellationToken);
                    if (!dominatorAccountModel.IsUserLoggedIn)
                        return;
                    else
                        GlobusLogHelper.log.Info(Log.SuccessfulLogin,
                            dominatorAccountModel.AccountBaseModel.AccountNetwork,
                            dominatorAccountModel.AccountBaseModel.UserName);
                }
                // remove this commented line from "if" condition because it check all failed account.
                //dominatorAccountModel.AccountBaseModel.Status == AccountStatus.Success
                else if (!dominatorAccountModel.IsUserLoggedIn &&
                         !logInProcess.CheckLogin(dominatorAccountModel, cancellationToken))
                {
                    return;
                }

                //Commented this code because they call again checkstatus and show again n again Attemplogin in logger,Top avoid this we added  Above code and check condition.

                #region commented code

                //if (!logInProcess.CheckLogin(dominatorAccountModel))
                //    return;
                //if (!dominatorAccountModel.IsUserLoggedIn &&
                //    dominatorAccountModel.AccountBaseModel.Status == AccountStatus.Success)


                // This code is commented if since browser is get close in CheckStatusAsync hence we opening again browser for login
                // and update account details likes following, followers etc.S

                #endregion
                var dbAccountService = new DbAccountService(dominatorAccountModel);
                await UpdateFollowersAsync(dominatorAccountModel, cancellationToken);
                await UpdateAccountFollowCount(dominatorAccountModel, accountModel);
                await UpdateFollowingsAsync(dominatorAccountModel, cancellationToken);
                await UpdatePostsAsync(dominatorAccountModel, cancellationToken, dbAccountService);
                GlobusLogHelper.log.Info(Log.DetailsUpdated,
                    dominatorAccountModel.AccountBaseModel.AccountNetwork,
                    dominatorAccountModel.AccountBaseModel.UserName,
                    "LangKeyAccountAllDetails".FromResourceDictionary());

                //  Instance.CheckStatusAsync(dominatorAccountModel, cancellationToken).Wait(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally
            {
                if (dominatorAccountModel.IsRunProcessThroughBrowser)
                    TdAccountsBrowserDetails.CloseBrowser(dominatorAccountModel,
                        BrowserInstanceType.CheckAccountStatus);
            }
        }

        //Method to fetch daily Growth
        public DailyStatisticsViewModel GetDailyGrowth(string accountId, string username, GrowthPeriod period)
        {
            var response = new DailyStatisticsViewModel();
            var growthList = new List<DailyStatitics>();
            try
            {
                var dbGrowthOperations =
                    InstanceProvider.ResolveAccountDbOperations(accountId, SocialNetworks.Twitter);

                if (period == GrowthPeriod.Daily)
                {
                    var startDate = DateTime.Today.AddDays(-1);
                    growthList = dbGrowthOperations.Get<DailyStatitics>(x => x.Date >= startDate).OrderBy(x => x.Date)
                        .ToList();
                }
                else if (period == GrowthPeriod.Weekly)
                {
                    var startDate = DateTime.Today.AddDays(-7);
                    growthList = dbGrowthOperations.Get<DailyStatitics>(x => x.Date >= startDate).OrderBy(x => x.Date)
                        .ToList();
                }
                else if (period == GrowthPeriod.Monthly)
                {
                    var startDate = DateTime.Today.AddMonths(-1);
                    growthList = dbGrowthOperations.Get<DailyStatitics>(x => x.Date >= startDate).OrderBy(x => x.Date)
                        .ToList();
                }
                else
                {
                    growthList = dbGrowthOperations.Get<DailyStatitics>().OrderBy(x => x.Date).ToList();
                }

                if (growthList != null && growthList.Count > 0)
                {
                    var oldRecord = growthList.FirstOrDefault();
                    var latestRecord = growthList.LastOrDefault();
                    response.GrowthColumnValue1 = latestRecord.Followers - oldRecord.Followers;
                    response.GrowthColumnValue2 = latestRecord.Followings - oldRecord.Followings;
                    response.GrowthColumnValue4 = latestRecord.Tweets - oldRecord.Tweets;
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
            var counter = 0;
            var isAllTime = false;
            var isMonthly = false;
            var responseList = new List<DailyStatisticsViewModel>();
            try
            {
                var dbGrowthOperations =
                    InstanceProvider.ResolveAccountDbOperations(accountId, SocialNetworks.Twitter);

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
                        group gt by new {m = dt.Month}
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
                        group gt by new {y = dt.Year, m = dt.Month, d = dt.Day}
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
                        group gt by new {m = dt.Month}
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
                        group gt by new {y = dt.Year, m = dt.Month, d = dt.Day}
                        into g
                        select g.OrderByDescending(t => t.Date).LastOrDefault();
                    growthList = q.ToList();
                }
                else
                {
                    isAllTime = true;
                    growthList = dbGrowthOperations.Get<DailyStatitics>().OrderBy(x => x.Date).ToList();
                    if (growthList.FirstOrDefault().Date < growthList.LastOrDefault().Date.AddDays(-30))
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
                    var setToZero = false;
                    if (!isAllTime)
                        for (var i = counter - 1; i >= 0; i--)
                        {
                            setToZero = false;
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
                                    record = growthList.FirstOrDefault(x =>
                                        x.Date.Date > lastAvailableRecord.Date.Date);
                                }
                                else
                                {
                                    setToZero = true;
                                    record = new DailyStatitics();
                                }

                                growthToAdd.Date = isMonthly ? currentDate.AddMonths(-i) : currentDate.AddDays(-i);
                            }

                            growthToAdd.GrowthColumnValue1 = setToZero || record == null ? 0 : record.Followers;
                            growthToAdd.GrowthColumnValue2 = setToZero || record == null ? 0 : record.Followings;
                            growthToAdd.GrowthColumnValue3 = setToZero || record == null ? 0 : record.Tweets;
                            responseList.Add(growthToAdd);
                        }
                    else
                        foreach (var growth in growthList)
                        {
                            var growthToAdd = new DailyStatisticsViewModel();
                            growthToAdd.GrowthColumnValue1 = setToZero || growth == null ? 0 : growth.Followers;
                            growthToAdd.GrowthColumnValue2 = setToZero || growth == null ? 0 : growth.Followings;
                            growthToAdd.GrowthColumnValue3 = setToZero || growth == null ? 0 : growth.Tweets;
                            growthToAdd.Date = growth.Date;
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

        public async Task UpdateFollowingsAsync(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken)
        {
            try
            {
                if (dominatorAccountModel.AccountBaseModel.Status != AccountStatus.Success)
                    return;
                var logInProcess = _accountScopeFactory[dominatorAccountModel.AccountId].Resolve<ITwtLogInProcess>();

                var twitterFunction = _accountScopeFactory[dominatorAccountModel.AccountId]
                    .Resolve<ITwitterFunctionFactory>().TwitterFunctions;


                var maxAlreadyInsertedCount = 100;
                var currentAlreadyInsertedCount = 0;

                if (!dominatorAccountModel.IsUserLoggedIn &&
                    dominatorAccountModel.AccountBaseModel.Status == AccountStatus.Success)
                    if (!logInProcess.CheckLogin(dominatorAccountModel, cancellationToken))
                        return;

                GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                    dominatorAccountModel.UserName, ActivityType.Follow, "Updating followings");
                var accountModel = new AccountModel(dominatorAccountModel);

                //new DbInsertionHelper(dominatorAccountModel);//

                var objDbInsertion =
                    _accountScopeFactory[dominatorAccountModel.AccountId].Resolve<IDbInsertionHelper>();

                var objFollowingResponseHandler = await twitterFunction.GetUserFollowingsAsync(dominatorAccountModel,
                    dominatorAccountModel.AccountBaseModel.ProfileId, cancellationToken);

                while (objFollowingResponseHandler.Success)
                {
                    currentAlreadyInsertedCount += objDbInsertion.AddFriendshipListData(
                        objFollowingResponseHandler.ListOfTwitterUser, FollowType.Following,
                        accountModel.FollowingsUpdated ? 1 : 0);

                    await _delayService.DelayAsync(TdConstants.ConsecutiveGetReqInteval, cancellationToken);

                    dominatorAccountModel.Token.ThrowIfCancellationRequested();

                    if (!objFollowingResponseHandler.HasMoreResults ||
                        currentAlreadyInsertedCount >= maxAlreadyInsertedCount)
                        break;

                    objFollowingResponseHandler = await twitterFunction.GetUserFollowingsAsync(dominatorAccountModel,
                        dominatorAccountModel.AccountBaseModel.ProfileId, cancellationToken,
                        objFollowingResponseHandler.MinPosition);
                }

                accountModel.FollowingsUpdated = true;
                accountModel.LastFollowingsUpdatedTime = DateTime.UtcNow.GetCurrentEpochTime();

                dominatorAccountModel.Token.ThrowIfCancellationRequested();
                SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId);
                GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                    dominatorAccountModel.UserName, ActivityType.Follow,
                    "LangKeyFollowingsUpdated".FromResourceDictionary());
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


        public async Task UpdateFollowersAsync(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken)
        {
            #region checking already updating follower of this account

            try
            {
                if (UpdatingAccount.Contains(dominatorAccountModel.UserName))
                    return;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion

            try
            {
                var logInProcess = _accountScopeFactory[dominatorAccountModel.AccountId].Resolve<ITwtLogInProcess>();
                var twitterFunction = _accountScopeFactory[dominatorAccountModel.AccountId]
                    .Resolve<ITwitterFunctionFactory>().TwitterFunctions;

                UpdatingAccount.Add(dominatorAccountModel.UserName);

                if (!dominatorAccountModel.IsUserLoggedIn &&
                    dominatorAccountModel.AccountBaseModel.Status == AccountStatus.Success)
                    if (!logInProcess.CheckLogin(dominatorAccountModel, cancellationToken))
                        return;

                UpdateFollowers(dominatorAccountModel, twitterFunction);
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
            finally
            {
                try
                {
                    if (UpdatingAccount.Contains(dominatorAccountModel.UserName))
                        UpdatingAccount.Remove(dominatorAccountModel.UserName);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }

        public void UpdateFollowers(DominatorAccountModel dominatorAccountModel, ITwitterFunctions twtFunc,
            int currentAlreadyInsertedCount = 0, int maxAlreadyInsertedCount = 50)
        {
            var cancellationToken = dominatorAccountModel.Token;
            GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                dominatorAccountModel.UserName, ActivityType.Follow,
                "LangKeyUpdatingFollowers".FromResourceDictionary());
            ResolvingDbInsertionHelper(dominatorAccountModel);
            var accountModel = new AccountModel(dominatorAccountModel);
            //new DbInsertionHelper(dominatorAccountModel); //

            var objDbInsertion =
                _accountScopeFactory[dominatorAccountModel.AccountId]
                    .Resolve<IDbInsertionHelper>(); // new DbInsertionHelper(dominatorAccountModel);

            var objFollowerResponseHandler = twtFunc.GetUserFollowersAsync(dominatorAccountModel,
                ResolveProfileIdOrUsername(dominatorAccountModel.AccountBaseModel), cancellationToken).Result;


            while (objFollowerResponseHandler.Success)
            {
                currentAlreadyInsertedCount += objDbInsertion.AddFriendshipListData(
                    objFollowerResponseHandler.ListOfTwitterUser, FollowType.Followers, 0);

                //_delayService.Delay(TdConstants.ConsecutiveGetReqInteval, cancellationToken);
                _delayService.ThreadSleep(TdConstants.ConsecutiveGetReqInteval);

                dominatorAccountModel.Token.ThrowIfCancellationRequested();

                if (!objFollowerResponseHandler.HasMoreResults ||
                    currentAlreadyInsertedCount >= maxAlreadyInsertedCount)
                    break;
                objFollowerResponseHandler = twtFunc.GetUserFollowersAsync(dominatorAccountModel,
                    dominatorAccountModel.AccountBaseModel.ProfileId, cancellationToken,
                    objFollowerResponseHandler.MinPosition).Result;
            }

            accountModel.FollowersUpdated = true;
            accountModel.LastFollowersUpdatedTime = DateTime.UtcNow.GetCurrentEpochTime();

            dominatorAccountModel.Token.ThrowIfCancellationRequested();
            // Instance.CheckStatusAsync(dominatorAccountModel, new CancellationToken()).Wait();

            GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                dominatorAccountModel.UserName, ActivityType.Follow,
                "LangKeyFollowersUpdated".FromResourceDictionary());
        }
        public string ResolveProfileIdOrUsername(DominatorAccountBaseModel dominatorAccountBase)
        {
            if (IsNumericId(dominatorAccountBase.ProfileId))
            {
                // It's an ID → get username
                return !string.IsNullOrWhiteSpace(dominatorAccountBase.UserName) ? 
                    dominatorAccountBase.UserName:dominatorAccountBase.ProfileId;
            }
            else
            {
                // It's a username → get ID
                return !string.IsNullOrWhiteSpace(dominatorAccountBase.ProfileId)
                    ? dominatorAccountBase.ProfileId
                    : dominatorAccountBase.UserName;
            }
        }
        private bool IsNumericId(string input)
        {
            return ulong.TryParse(input, out _); // Accepts very large numeric IDs
        }

        public void ResolvingDbInsertionHelper(DominatorAccountModel dominatorAccountModel)
        {
            try
            {
                var commonJobConfiguration = new CommonJobConfiguration(new JobConfiguration(), null, false);
                _accountScopeFactory[dominatorAccountModel.AccountId].RegisterInstance<IProcessScopeModel>
                (new ProcessScopeModel(dominatorAccountModel, ActivityType.Follow, null,
                    null, SocialNetworks.Twitter, null, null, commonJobConfiguration, null));
                _accountScopeFactory[dominatorAccountModel.AccountId]
                    .RegisterInstance<IDbCampaignService>(new DbCampaignService(null));
                // InstanceProvider.ResolveWithDominatorAccount<TDLibrary.GeneralLibrary.DAL.IDbAccountService>(dominatorAccountModel);
                _accountScopeFactory[dominatorAccountModel.AccountId]
                    .RegisterInstance<IDbInsertionHelper>(_accountScopeFactory[dominatorAccountModel.AccountId]
                        .Resolve<DbInsertionHelper>());
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void CheckIsFollowersUpdated(DominatorAccountModel dominatorAccountModel, int lastFollowersUpdatedTime,
            CancellationToken cancellationToken)
        {
            var сurrentEpoch = DateTime.UtcNow.GetCurrentEpochTime();
            if (сurrentEpoch - lastFollowersUpdatedTime > TdConstants.UpdateDbInterval)
                UpdateFollowersAsync(dominatorAccountModel, cancellationToken).Wait();
        }

        public void CheckIsFollowingsUpdated(DominatorAccountModel dominatorAccountModel, int lastFollowingsUpdatedTime,
            CancellationToken cancellationToken)
        {
            var сurrentEpoch = DateTime.UtcNow.GetCurrentEpochTime();
            if (сurrentEpoch - lastFollowingsUpdatedTime > TdConstants.UpdateDbInterval)
                UpdateFollowingsAsync(dominatorAccountModel, cancellationToken).Wait();
        }

        public async Task UpdatePostsAsync(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken,IDbAccountService dbAccountService)
        {
            try
            {
                if (dominatorAccountModel.AccountBaseModel.Status != AccountStatus.Success)
                    return;
                var logInProcess = _accountScopeFactory[dominatorAccountModel.AccountId].Resolve<ITwtLogInProcess>();
                var twitterFunction = _accountScopeFactory[dominatorAccountModel.AccountId]
                    .Resolve<ITwitterFunctionFactory>().TwitterFunctions;

                if (!dominatorAccountModel.IsUserLoggedIn &&
                    dominatorAccountModel.AccountBaseModel.Status == AccountStatus.Success)
                    if (!logInProcess.CheckLogin(dominatorAccountModel, cancellationToken))
                        return;

                await UpdatePosts(dominatorAccountModel, twitterFunction, dbAccountService);
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

        public async Task UpdatePosts(DominatorAccountModel dominatorAccountModel,
            ITwitterFunctions twtFunc, IDbAccountService dbAccountService, bool IsTweetWithReply = true)
        {
            var cancellationToken = dominatorAccountModel.Token;
            var listOfFeeds = dbAccountService.GetList<FeedInfoes>();
            var maxAlreadyInsertedCount = 100;
            var currentAlreadyInsertedCount = 0;
            GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                dominatorAccountModel.UserName, ActivityType.Tweet, "LangKeyUpdatingFeeds".FromResourceDictionary());
            ResolvingDbInsertionHelper(dominatorAccountModel);
            var accountModel = new AccountModel(dominatorAccountModel);

            var objDbInsertion =
                _accountScopeFactory[dominatorAccountModel.AccountId]
                    .Resolve<IDbInsertionHelper>(); //new DbInsertionHelper(dominatorAccountModel);
            //if (listOfFeeds.Count < dominatorAccountModel.DisplayColumnValue3.Value)
                   dbAccountService.RemoveAll<FeedInfoes>();
            var UserName = string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.ProfileId) ?dominatorAccountModel.AccountBaseModel.UserName: dominatorAccountModel.AccountBaseModel.ProfileId;
            var objUserFeedResponseHandler = await twtFunc.GetTweetsFromUserFeedAsync(dominatorAccountModel,
                UserName, cancellationToken,string.Empty,ActivityType.Tweet,IsTweetWithReply);

            while (objUserFeedResponseHandler.Success)
            {
                // var RecordInserted = false;
                currentAlreadyInsertedCount +=
                    objDbInsertion.AddListFeedsInfo(objUserFeedResponseHandler.UserTweetsDetail);

                await _delayService.DelayAsync(TdConstants.ConsecutiveGetReqInteval, cancellationToken);
                if (!objUserFeedResponseHandler.hasmore || /*(accountModel.FeedsUpdated && !RecordInserted) ||*/
                    currentAlreadyInsertedCount >= maxAlreadyInsertedCount)
                    break;

                dominatorAccountModel.Token.ThrowIfCancellationRequested();
                objUserFeedResponseHandler = await twtFunc.GetTweetsFromUserFeedAsync(dominatorAccountModel,
                    dominatorAccountModel.AccountBaseModel.ProfileId, cancellationToken,
                    objUserFeedResponseHandler.MinPosition, ActivityType.Tweet, IsTweetWithReply);
            }

            accountModel.FeedsUpdated = true;
            accountModel.LastFeedsUpdatedTime = DateTime.UtcNow.GetCurrentEpochTime();

            dominatorAccountModel.Token.ThrowIfCancellationRequested();
            GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                dominatorAccountModel.UserName, ActivityType.Tweet, "LangKeyFeedsUpdated".FromResourceDictionary());
        }
 

        #endregion


    }
}