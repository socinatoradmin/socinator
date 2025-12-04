using CommonServiceLocator;
using DominatorHouseCore;
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
using RedditDominatorCore.Response;
using System;
using System.Linq;
using CampaignTables = DominatorHouseCore.DatabaseHandler.RdTables.Campaigns;

namespace RedditDominatorCore.RDLibrary.Actions
{
    internal class BroadcastMessageProcess : RdJobProcessInteracted<InteractedUsers>
    {
        private readonly IDbCampaignService _campaignService;
        private readonly IDbAccountServiceScoped _dbAccountServiceScoped;
        private readonly IRedditFunction _redditFunction;
        private readonly IRdBrowserManager _browserManager;

        /// <summary>
        ///     Intialize the broadcast message model, dominator account model, per week, per day,per hour
        /// </summary>
        /// <param name="processScopeModel"></param>
        /// <param name="queryScraperFactor"></param>
        public BroadcastMessageProcess(IProcessScopeModel processScopeModel, IRdQueryScraperFactory queryScraperFactor,
            IRedditLogInProcess redditLogInProcess, IRdHttpHelper rdHttpHelper,
            IDbAccountServiceScoped dbAccountServiceScoped,
            IDbCampaignService campaignService, IRedditFunction redditFunction)
            : base(processScopeModel, queryScraperFactor, redditLogInProcess, rdHttpHelper)
        {
            _dbAccountServiceScoped = dbAccountServiceScoped;
            _campaignService = campaignService;
            _redditFunction = redditFunction;
            _browserManager = redditLogInProcess._browserManager;
            BroadcastMessageModel = processScopeModel.GetActivitySettingsAs<BrodcastMessageModel>();
            blackListWhitelistHandler =
                new BlackListWhitelistHandler(ModuleSetting, DominatorAccountModel, ActivityType);
        }

        private BlackListWhitelistHandler blackListWhitelistHandler { get; }

        /// <summary>
        ///     Intialize the Broadcast message model
        /// </summary>

        public BrodcastMessageModel BroadcastMessageModel { get; set; }

        /// <summary>
        ///     To perform other configuration
        /// </summary>
        /// <param name="scrapeResult"></param>

        // ReSharper disable once InheritdocConsiderUsage
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

        /// <summary>
        ///     It will send messages to the particular user
        /// </summary>
        /// <param name="scrapeResult"></param>
        /// <returns></returns>

        // ReSharper disable once InheritdocConsiderUsage
        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            var redditUser = (RedditUser)scrapeResult.ResultUser;
            if (redditUser == null) return jobProcessResult;

            GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, redditUser.Username);

            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            if (BroadcastMessageModel.IsChkBroadCastPrivateBlacklist || BroadcastMessageModel.IsChkBroadCastGroupBlacklist)
            {
                var blackListUser = blackListWhitelistHandler.GetBlackListUsers();
                if (blackListUser != null && blackListUser.Count > 0 && blackListUser.Any(user => user.Equals(redditUser.DisplayText)))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            "Skip User " + redditUser.Username + ", Present in Blacklist ");
                    return jobProcessResult;
                }
            }

            try
            {
                if (ActivityType.ToString() == "AutoReplyToNewMessage")
                {
                    if (redditUser != null && redditUser.IsPending)
                    {
                        var Accepted = _browserManager.AcceptPendingMessageRequest(DominatorAccountModel, redditUser).Result;
                    }
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var AutoReplyResult = _redditFunction.BroadCastMessage(ActivityType, DominatorAccountModel, redditUser);

                    if (AutoReplyResult.Status)
                    {
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, redditUser.Username);
                        IncrementCounters();

                        AddBroadcastMessageDataToDataBase(scrapeResult);
                        jobProcessResult.IsProcessSuceessfull = true;
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, redditUser.Username,
                            string.IsNullOrEmpty(AutoReplyResult.ResponseMessage) ? "Reason: Blocked" : AutoReplyResult.ResponseMessage);
                    }
                }
                if (ActivityType.ToString() == "BroadcastMessages")
                {
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    //var url = Utils.GetUrlFromUserName(redditUser.Url);
                    var url = redditUser.Url;
                    redditUser.Url = url;
                    var currentMessage = BroadcastMessageModel.LstManageMessagesModel.GetRandomItem().MessagesText.Trim();
                    if (BroadcastMessageModel.IsSpintax && !string.IsNullOrEmpty(currentMessage))
                        currentMessage = SpinTexHelper.GetSpinText(currentMessage);
                    redditUser.Text = currentMessage.ToString();

                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    ActivityResposneHandler result = new ActivityResposneHandler();
                    //if (DominatorAccountModel.IsRunProcessThroughBrowser)
                        //result = _browserManager.BroadCastMessage(ActivityType, DominatorAccountModel, redditUser);
                    //else
                        result = _redditFunction.BroadCastMessage(ActivityType, DominatorAccountModel, redditUser);
                    if (result.Status)
                    {
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, redditUser.Username);
                        IncrementCounters();

                        AddBroadcastMessageDataToDataBase(scrapeResult);
                        jobProcessResult.IsProcessSuceessfull = true;
                    }
                    else
                    {
                        var message = string.IsNullOrEmpty(result.ResponseMessage) ? "Reason: Unable to invite" : result.ResponseMessage;
                        GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"{redditUser?.Username} ==> {message}");
                    }
                }

                DelayBeforeNextActivity();
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return jobProcessResult;
        }

        /// <summary>
        ///     It will store the user information into the database
        /// </summary>
        /// <param name="scrapeResult"></param>
        public void AddBroadcastMessageDataToDataBase(ScrapeResultNew scrapeResult)
        {
            try
            {
                var user = (RedditUser)scrapeResult.ResultUser;
                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleConfiguration =
                    jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (moduleConfiguration != null && moduleConfiguration.IsTemplateMadeByCampaignMode)
                    _campaignService.Add(new CampaignTables.InteractedUsers
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
                        InteractionDateTime = DateTime.Now,
                        Message = user.Text
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
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    InteractionDateTime = DateTime.Now,
                    Message = user.Text
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}