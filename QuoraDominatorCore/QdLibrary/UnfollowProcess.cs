using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.QdTables.Accounts;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using QuoraDominatorCore.Factories;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QDFactories;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.Request;
using QuoraDominatorCore.Response;
using System;
using System.Linq;
using InteractedUserCampaign = DominatorHouseCore.DatabaseHandler.QdTables.Campaigns;

namespace QuoraDominatorCore.QdLibrary
{
    internal class UnfollowProcess : QdJobProcessInteracted<UnfollowedUsers>
    {
        private IQuoraBrowserManager _browser;
        private readonly IQdHttpHelper _httpHelper;
        private readonly SocialNetworks _networks;
        private readonly UnfollowerModel _unFollowModel;
        public IQuoraFunctions quoraFunct;

        private int _stopUnFollowToolWhenReachedCount = 1;
        private IQDBrowserManagerFactory managerFactory;
        public UnfollowProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped,
            IExecutionLimitsManager executionLimitsManager, IQuoraFunctions qdFunc,
            IQdQueryScraperFactory queryScraperFactory, IQdHttpHelper qdHttpHelper, IQdLogInProcess qdLogInProcess)
            : base(processScopeModel, accountServiceScoped, executionLimitsManager, queryScraperFactory, qdHttpHelper,
                qdLogInProcess)
        {
            _httpHelper = qdHttpHelper;
            quoraFunct = qdFunc;
            _unFollowModel = processScopeModel.GetActivitySettingsAs<UnfollowerModel>();
            _networks = SocialNetworks.Quora;
            managerFactory = qdLogInProcess.managerFactory;
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            _browser = managerFactory.QdBrowserManager();
            var quoraUser = (QuoraUser) scrapeResult.ResultUser;

            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultUser.Username);

            var jobProcessResult = new JobProcessResult();
            try
            {
                var linkresp = "";
                UnFollowResponseHandler response = null;
                if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    var request = _httpHelper.GetRequestParameter();
                    request.UserAgent =
                        "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.132 Safari/537.36";
                    request.Accept =
                        "text / html,application / xhtml + xml,application / xml; q = 0.9,image / webp,image / apng,*/*;q=0.8,application/signed-exchange;v=b3";
                    request.Cookies = DominatorAccountModel.Cookies;
                    _httpHelper.SetRequestParameter(request);
                    var ProfileUrl = $"{QdConstants.HomePageUrl}/profile/{scrapeResult.ResultUser.Username}";
                    linkresp = _httpHelper
                        .GetRequest(ProfileUrl).Response;
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    response = quoraFunct.UnFollow(DominatorAccountModel, linkresp, ProfileUrl).Result;
                    if (response.Success)
                    {
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, quoraUser.Username);

                        AddUnFollowedDataToDataBase(scrapeResult, quoraUser);

                        if (_unFollowModel.IsChkAddToBlackList)
                            AddToBlackList(quoraUser.Username, quoraUser.UserId);
                        PostUnFollowProcess(quoraUser, scrapeResult.QueryInfo);
                        jobProcessResult.IsProcessSuceessfull = true;
                        IncrementCounters();
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            scrapeResult.ResultUser.Username, response.Issue.Error);
                        jobProcessResult.IsProcessSuceessfull = false;
                    }
                }
                else
                {
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    linkresp = _browser.SearchByCustomUrl(DominatorAccountModel,
                        $"{QdConstants.HomePageUrl}/profile/" + scrapeResult.ResultUser.Username).Response;
                    _browser.Unfollow().TryGetValue("UnFollow", out string UnFollowResponse);
                    if (string.IsNullOrEmpty(UnFollowResponse))
                    {
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, quoraUser.Username);

                        AddUnFollowedDataToDataBase(scrapeResult, quoraUser);

                        if (_unFollowModel.IsChkAddToBlackList)
                            AddToBlackList(quoraUser.Username, quoraUser.UserId);
                        jobProcessResult.IsProcessSuceessfull = true;
                        PostUnFollowProcess(quoraUser, scrapeResult.QueryInfo);
                        IncrementCounters();
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            scrapeResult.ResultUser.Username + "{ "+UnFollowResponse+" }");
                        jobProcessResult.IsProcessSuceessfull = false;
                    }
                }

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            DelayBeforeNextActivity();

            return jobProcessResult;
        }

        private void AddUnFollowedDataToDataBase(ScrapeResultNew scrapeResult, QuoraUser quorauser)
        {
            try
            {
                #region Add to Account DB

                var dbAccountService =
                    InstanceProvider.ResolveAccountDbOperations(DominatorAccountModel.AccountId, _networks);

                dbAccountService.Add(new UnfollowedUsers
                {
                    AccountUsername = DominatorAccountModel.AccountBaseModel.UserName,
                    FollowType = FollowType.Unfollowed,
                    InteractionDate = GetEpochTime(),
                    UnfollowedUsername = scrapeResult.ResultUser.Username,
                    FollowedBack = quorauser.FollowedBack
                });

                #endregion

                #region Add to campaign Db

                if (!string.IsNullOrEmpty(CampaignId))
                {
                    IDbCampaignService dbCampaignService = new DbCampaignService(CampaignId);
                    dbCampaignService.Add(new InteractedUserCampaign.UnfollowedUsers
                    {
                        InteractionDate = GetEpochTime(),
                        Username = scrapeResult.ResultUser.Username,
                        FollowedBack = quorauser.FollowedBack,
                        FollowedBackDate = GetEpochTime(),
                        FullName = DominatorAccountModel.UserName
                    });
                }

                #endregion

                #region Add to PrivateBlacklist DB

                if (_unFollowModel.IsChkPrivateList)
                    dbAccountService.Add(
                        new PrivateBlacklist
                        {
                            UserName = quorauser.Username,
                            UserId = quorauser.UserId,
                            InteractionTimeStamp = DateTime.Now.GetCurrentEpochTime()
                        });

                #endregion

                #region Add to GroupBlacklist DB

                if (_unFollowModel.IsChkGroupList)
                {
                    IDbGlobalService dbGlobalService = new DbGlobalService();
                    dbGlobalService.Add(new BlackListUser
                    {
                        UserName = quorauser.Username,
                        UserId = quorauser.UserId,
                        AddedDateTime = DateTime.Now
                    });
                }

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AddToBlackList(string username, string userId)
        {
            try
            {
                IDbGlobalService dbGlobalService = new DbGlobalService();
                dbGlobalService.Add(new BlackListUser
                {
                    UserName = username,
                    UserId = userId,
                    AddedDateTime = DateTime.Now
                });
                GlobusLogHelper.log.Info("{0}\t {1}\t Add to Blacklisted " + username,
                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                GlobusLogHelper.log.Info("{0}\t {1}\t could not add to Blacklist " + username,
                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName);
            }
        }

        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {
            try
            {
                GlobusLogHelper.log.Info(Log.OtherConfigurationStarted,
                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType.Unfollow);

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                var accountUserName = quoraFunct.GetProfileUrl(DominatorAccountModel)
                    .Replace($"{QdConstants.HomePageUrl}/profile/", "");
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                var currentUserInfo =
                    quoraFunct.UserInfo(DominatorAccountModel, $"{QdConstants.HomePageUrl}/profile/" + accountUserName);
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                SocinatorAccountBuilder.Instance(DominatorAccountModel.AccountBaseModel.AccountId)
                    .AddOrUpdateDisplayColumn2(currentUserInfo.FollowingCount)
                    .SaveToBinFile();
                var currentAccount = AccountCustomControl.GetAccountCustomControl(SocialNetworks.Quora)
                    .DominatorAccountViewModel
                    .LstDominatorAccountModel.FirstOrDefault(x => x.AccountId == DominatorAccountModel.AccountId);
                if (currentAccount != null) currentAccount.DisplayColumnValue2 = currentUserInfo.FollowingCount;

                if (_unFollowModel.IsChkEnableAutoFollowUnfollowChecked)
                    if (_unFollowModel.IsChkStopUnFollowToolWhenReachChecked && currentUserInfo.FollowingCount >=
                        _unFollowModel.StopFollowToolWhenReachValue.GetRandom() ||
                        _unFollowModel.IsChkWhenNoUsersToUnfollow && currentUserInfo.FollowingCount == 0 ||
                        currentUserInfo.FollowerCount / currentUserInfo.FollowingCount >=
                        _unFollowModel.FollowerFollowingsGreaterThanValue)
                    {
                        var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                        dominatorScheduler.ChangeAccountsRunningStatus(false, DominatorAccountModel.AccountId,
                            ActivityType.Unfollow);
                    }

                if (_unFollowModel.IsChkStartFollowWithoutStoppingUnfollow)
                {
                    var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                    dominatorScheduler.ChangeAccountsRunningStatus(true, DominatorAccountModel.AccountId,
                        ActivityType.Follow);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        private void PostUnFollowProcess(QuoraUser quoraUser, QueryInfo queryInfo)
        {
            //JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            //if ( _unFollowModel.IsChkStopUnFollowToolWhenReachChecked && _stopUnFollowToolWhenReachedCount++ >= _unFollowModel.StopFollowToolWhenReachValue.GetRandom())
            //{

            //    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
            //        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
            //        $"{"LangKeyStopActivityAfter".FromResourceDictionary()} {_stopUnFollowToolWhenReachedCount - 1} " + "UnFollows");

            //    StopAndRescheduleJob();

            //}
        }
    }
}