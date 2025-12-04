using System;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.DatabaseHandler.TdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.ExecutionCounters;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Database;
using TwtDominatorCore.Factories;
using TwtDominatorCore.Requests;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;


// ReSharper disable once CheckNamespace
namespace TwtDominatorCore.TDLibrary
{
    // ReSharper disable once IdentifierTypo
    public class Unfollowprocess : TdJobProcessInteracted<UnfollowedUsers>
    {
        private readonly IBlackWhiteListHandler _blackWhiteListHandler;
        private readonly IDbInsertionHelper _dbInsertionHelper;
        private readonly ITwitterFunctionFactory _twitterFunctionsFactory;

        public Unfollowprocess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ITdQueryScraperFactory queryScraperFactory,
            IBlackWhiteListHandler blackWhiteListHandler, ITdHttpHelper tdHttpHelper, ITwtLogInProcess twtLogInProcess,
            ITwitterFunctionFactory twitterFunctionFactory, IDbInsertionHelper dbInsertionHelper)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                tdHttpHelper, twtLogInProcess)
        {
            _blackWhiteListHandler = blackWhiteListHandler;
            _twitterFunctionsFactory = twitterFunctionFactory;
            _dbInsertionHelper = dbInsertionHelper;
            UnfollowerModel = processScopeModel.GetActivitySettingsAs<UnfollowerModel>();
        }

        private ITwitterFunctions _twitterFunctions => _twitterFunctionsFactory.TwitterFunctions;
        public UnfollowerModel UnfollowerModel { get; set; }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            try
            {
                var twitterUser = (TwitterUser) scrapeResult.ResultUser;
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var unFollowResponse =
                    _twitterFunctions.Unfollow(DominatorAccountModel, twitterUser.UserId, twitterUser.Username);
                if (unFollowResponse.Success)
                {
                    // Update following count in UI by decreasing count after each unfollow
                    LogInProcess.UpdateDominatorAccountModel(DominatorAccountModel, -1);

                    IncrementCounters();

                    _dbInsertionHelper.AddUnfollowedDataInAccountDb(twitterUser,
                        ActivityType.ToString(), scrapeResult);

                    // Updated from normal mode

                    #region  GetModuleSetting

                    var jobActivityConfigurationManager =
                        InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                    var moduleModeSetting =
                        jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                    if (moduleModeSetting == null) return jobProcessResult;

                    #endregion

                    if (moduleModeSetting.IsTemplateMadeByCampaignMode)
                        _dbInsertionHelper.AddUnfollowedDataInCampaignDb(twitterUser,
                            scrapeResult);

                    _dbInsertionHelper.UpdateFriendshipData(twitterUser.UserId);

                    jobProcessResult.IsProcessSuceessfull = true;

                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                        ActivityType.Unfollow, twitterUser.Username);

                    PostUnfollowProcess(twitterUser);

                    StartOtherConfigurationAfterEachUnfollow();
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.UserName, ActivityType.Unfollow,
                        $"{TdUtility.GetProfileUrl(twitterUser.Username)} {unFollowResponse.Issue?.Message}");
                    GlobusLogHelper.log.Debug(unFollowResponse.Issue?.Message);
                    jobProcessResult.IsProcessSuceessfull = false;
                    if (!string.IsNullOrEmpty(unFollowResponse.Issue?.Message) &&
                        unFollowResponse.Issue.Message.Contains("temporarily locked"))
                        TdUtility.StopActivity(DominatorAccountModel.AccountId, ActivityType);

                    // adding to interacted users list so, again come for unfollow
                    if (unFollowResponse.Issue?.Message == "Sorry, that page does not exist.")
                        _dbInsertionHelper.UpdateAccountStatus(twitterUser.UserId, twitterUser.Username,
                            TdConstants.AccountSuspended);
                }

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                DelayBeforeNextActivity();
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                if (DominatorAccountModel?.AccountBaseModel?.UserName != null)
                    ex.DebugLog(
                        $"TwtDominator : [Account: {DominatorAccountModel.AccountBaseModel.UserName}]   (Module => {ActivityType.ToString()})");
            }

            return jobProcessResult;
        }

        private void PostUnfollowProcess(TwitterUser user)
        {
            if (ModuleSetting.ManageBlackWhiteListModel.IsAddToBlackListOnceUnfollowed &&
                (ModuleSetting.ManageBlackWhiteListModel.IsAddToPrivateBlackList ||
                 ModuleSetting.ManageBlackWhiteListModel.IsAddToGroupBlackList))
                _blackWhiteListHandler.AddToBlackList(user.UserId, user.Username);
        }

        private void StartOtherConfigurationAfterEachUnfollow(bool isNoMoreUserToUnFollow = false)
        {
            try
            {
                if (!UnfollowerModel.IsChkEnableAutoFollowUnfollowChecked)
                    return;

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                var jobCountersManager = InstanceProvider.GetInstance<IJobCountersManager>();
                var currentFollowingCount = AccountModel.FollowingCount - jobCountersManager[Id];

                #region  Follow and unfollow

                var followFollowingRatio =
                    (int) Math.Ceiling(AccountModel.FollowersCount / (double) currentFollowingCount);
                var reachedStartStopFollowingLimit =
                    UnfollowerModel.StartFollowToolBetwen.GetRandom() >=
                    currentFollowingCount; //InRange(FollowFollowingRatio);
                var reachedOnlyStopFollowingLimit =
                    UnfollowerModel.StopFollowToolWhenReachValue.GetRandom() >=
                    currentFollowingCount; //InRange(FollowFollowingRatio);

                if (UnfollowerModel.IsChkStartFollowTool)
                {
                    if (UnfollowerModel.IsChkStartFollowBetween && reachedStartStopFollowingLimit ||
                        UnfollowerModel.IsChkWhenFollowerFollowingsGreater && followFollowingRatio >
                        UnfollowerModel.FollowerFollowingsGreaterThanValue
                        || UnfollowerModel.IsChkWhenNoUsersToUnfollow && isNoMoreUserToUnFollow)
                        StartFollow();
                }
                else if (UnfollowerModel.IsChkStopUnFollowTool)
                {
                    if (UnfollowerModel.IsChkStopFollowToolWhenReachChecked && reachedOnlyStopFollowingLimit ||
                        UnfollowerModel.IsChkWhenFollowerFollowingsGreater && followFollowingRatio >
                        UnfollowerModel.FollowerFollowingsGreaterThanValue ||
                        UnfollowerModel.IsChkWhenNoUsersToUnfollow && isNoMoreUserToUnFollow)
                        try
                        {
                            TdUtility.StopActivity(DominatorAccountModel.AccountId, ActivityType);
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.UserName, ActivityType,
                                "LangKeyMeetStopUnFollowingConfigMessage".FromResourceDictionary());

                            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        }
                        catch (OperationCanceledException)
                        {
                            throw new OperationCanceledException();
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                }

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {
            try
            {
                StartOtherConfigurationAfterEachUnfollow(true);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void StartFollow()
        {
            try
            {
                try
                {
                    var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                    dominatorScheduler.EnableDisableModules(ActivityType.Unfollow, ActivityType.Follow,
                        DominatorAccountModel.AccountId);
                }
                catch (InvalidOperationException ex)
                {
                    if (ex.Message.Contains("1001"))
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                            ActivityType,
                            "LangKeyUnfollowActivityHasMetAutoEnableConfigurationForFollowMessage"
                                .FromResourceDictionary());
                    else
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                            ActivityType, "");
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}