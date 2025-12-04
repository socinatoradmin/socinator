using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.DatabaseHandler.RdTables.Accounts;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using RedditDominatorCore.RDFactories;
using RedditDominatorCore.RDLibrary.BrowserManager;
using RedditDominatorCore.RDLibrary.DAL;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDRequest;
using System;

namespace RedditDominatorCore.RDLibrary.Actions
{
    internal class FollowProcess : RdJobProcessInteracted<InteractedUsers>
    {
        private readonly IAccountScopeFactory _accountScopeFactory;
        private readonly IDbCampaignService _campaignService;
        private readonly IDbAccountServiceScoped _dbAccountServiceScoped;
        private readonly IDominatorScheduler _dominatorScheduler;
        private readonly IJobActivityConfigurationManager _jobActivityConfigurationManager;
        private readonly IRedditFunction _redditFunction;
        private int _activityFailedCount = 1;
        private IRdBrowserManager _browserManager;

        public FollowProcess(IProcessScopeModel processScopeModel, IRdQueryScraperFactory queryScraperFactor,
            IRedditLogInProcess redditLogInProcess, IRdHttpHelper rdHttpHelper, IRedditFunction redditFunction,
            IJobActivityConfigurationManager jobActivityConfigurationManager, IDominatorScheduler dominatorScheduler,
            IDbAccountServiceScoped dbAccountServiceScoped, IDbCampaignService campaignService)
            : base(processScopeModel, queryScraperFactor, redditLogInProcess, rdHttpHelper)
        {
            _redditFunction = redditFunction;
            _jobActivityConfigurationManager = jobActivityConfigurationManager;
            _dominatorScheduler = dominatorScheduler;
            _dbAccountServiceScoped = dbAccountServiceScoped;
            _campaignService = campaignService;
            FollowModel = processScopeModel.GetActivitySettingsAs<FollowModel>();
            blackListWhitelistHandler =
                new BlackListWhitelistHandler(ModuleSetting, DominatorAccountModel, ActivityType);
            _browserManager = redditLogInProcess._browserManager;
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
        }

        private BlackListWhitelistHandler blackListWhitelistHandler { get; }
        public FollowModel FollowModel { get; set; }

        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {
            try
            {
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

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            try
            {
                var newRedditUser = (RedditUser)scrapeResult.ResultUser;
                var IsSuccess = false;
                var ErrorMessage = string.Empty;
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    "Started Process to Follow " + newRedditUser.Username);
                if (newRedditUser == null ? false : newRedditUser.IsFollowing)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    $"Already {ActivityType} to {newRedditUser.Username}");
                    return jobProcessResult;
                }
                if (FollowModel.IsChkSkipBlackListedUser)
                {
                    var blackListUser = blackListWhitelistHandler.GetBlackListUsers();
                    var IsSkipped = blackListUser != null && (blackListUser.RemoveAll(user => newRedditUser.DisplayText.Equals(user))) > 0;
                    if (IsSkipped)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                $"Skipped User {newRedditUser.Username} Present in Blacklist ");
                        return jobProcessResult;
                    }
                }

                if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var response = _redditFunction.NewFollow(DominatorAccountModel, newRedditUser);
                    IsSuccess = response != null && response.Success;
                }
                //For browser automation
                else
                {
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var FollowResponse = _browserManager.Follow(DominatorAccountModel);
                    IsSuccess = FollowResponse != null && FollowResponse.Status;
                    ErrorMessage = FollowResponse.ResponseMessage;
                }
                if (IsSuccess)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, newRedditUser.Username);
                    IncrementCounters();
                    AddFollowedDataToDataBase(scrapeResult);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    ErrorMessage = newRedditUser.Username + (string.IsNullOrEmpty(ErrorMessage) ? string.Empty : $"=>{ErrorMessage}");
                    GlobusLogHelper.log.Info(Log.ActivityFailed,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, ErrorMessage);
                    //Reschedule if action block
                    StopActivityAfterFailedAttemptAndReschedule();
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally { if (_browserManager != null && _browserManager.BrowserWindow != null && _browserManager.BrowserWindow.LastCurrentCount > 1) _browserManager.CloseBrowser(); DelayBeforeNextActivity(); }
            return jobProcessResult;
        }

        public void AddFollowedDataToDataBase(ScrapeResultNew scrapeResult)
        {
            try
            {
                var user = (RedditUser)scrapeResult.ResultUser;
                var moduleConfiguration =
                    _jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (moduleConfiguration != null && moduleConfiguration.IsTemplateMadeByCampaignMode)
                    _campaignService.Add(new DominatorHouseCore.DatabaseHandler.RdTables.Campaigns.InteractedUsers
                    {
                        ActivityType = ActivityType.ToString(),
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        InteractedUsername = user.Username,
                        Date = DateTimeUtilities.GetEpochTime(),
                        InteractedUserId = user.Id,
                        UpdatedTime = DateTimeUtilities.GetEpochTime(),
                        AccountIcon = user.AccountIcon,
                        CommentKarma = user.CommentKarma,
                        Created = user.Created,
                        DisplayName = user.DisplayName,
                        DisplayNamePrefixed = user.DisplayNamePrefixed,
                        DisplayText = user.DisplayText,
                        HasUserProfile = user.HasUserProfile,
                        IsEmployee = user.IsEmployee,
                        IsFollowing = user.IsFollowing,
                        IsGold = user.IsGold,
                        IsMod = user.IsMod,
                        IsNsfw = user.IsNsfw,
                        PrefShowSnoovatar = user.PrefShowSnoovatar,
                        PostKarma = user.PostKarma,
                        Url = user.Url,
                        SinAccId = AccountId,
                        SinAccUsername = AccountName,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        InteractionDateTime = DateTime.Now
                    });


                _dbAccountServiceScoped.Add(new InteractedUsers
                {
                    ActivityType = ActivityType.ToString(),
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    InteractedUsername = user.Username,
                    Date = DateTimeUtilities.GetEpochTime(),
                    InteractedUserId = user.Id,
                    UpdatedTime = DateTimeUtilities.GetEpochTime(),
                    AccountIcon = user.AccountIcon,
                    CommentKarma = user.CommentKarma,
                    Created = user.Created,
                    DisplayName = user.DisplayName,
                    DisplayNamePrefixed = user.DisplayNamePrefixed,
                    DisplayText = user.DisplayText,
                    HasUserProfile = user.HasUserProfile,
                    IsEmployee = user.IsEmployee,
                    IsFollowing = user.IsFollowing,
                    IsGold = user.IsGold,
                    IsMod = user.IsMod,
                    IsNsfw = user.IsNsfw,
                    PrefShowSnoovatar = user.PrefShowSnoovatar,
                    PostKarma = user.PostKarma,
                    Url = user.Url,
                    InteractionDateTime = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void StopActivityAfterFailedAttemptAndReschedule()
        {
            if (FollowModel.IsChkStopActivityAfterXXFailed && _activityFailedCount++ == FollowModel.ActivityFailedCount)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    $"{"LangKeyStopActivityAfter".FromResourceDictionary()} {_activityFailedCount - 1} " +
                    $"{"LangKeyFailedAttemptFor".FromResourceDictionary()} {FollowModel.FailedActivityReschedule} " +
                    $"{"LangKeyHour".FromResourceDictionary()}");

                StopAndRescheduleJob(FollowModel.FailedActivityReschedule);
            }
        }
    }
}