using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.RdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
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
using CampaignTables = DominatorHouseCore.DatabaseHandler.RdTables.Campaigns;

namespace RedditDominatorCore.RDLibrary.Actions
{
    internal class DownVoteProcess : RdJobProcessInteracted<InteractedPost>
    {
        private readonly IAccountScopeFactory _accountScopeFactory;
        private readonly IRdBrowserManager _browserManager;
        private readonly IDbCampaignService _campaignService;
        private readonly IDbAccountServiceScoped _dbAccountServiceScoped;
        private readonly IRedditFunction _redditFunction;
        private int _activityFailedCount = 1;
        private IRdBrowserManager _newBrowserWindow;

        /// <summary>
        ///     Intialize the dominator account model, per week, per day , per hour
        /// </summary>
        /// <param name="processScopeModel"></param>
        /// <param name="queryScraperFactor"></param>
        /// <param name="redditLogInProcess"></param>
        /// <param name="rdHttpHelper"></param>
        /// <param name="dbAccountServiceScoped"></param>
        /// <param name="campaignService"></param>
        /// <param name="redditFunction"></param>
        public DownVoteProcess(IProcessScopeModel processScopeModel, IRdQueryScraperFactory queryScraperFactor,
            IRedditLogInProcess redditLogInProcess, IRdHttpHelper rdHttpHelper,
            IDbAccountServiceScoped dbAccountServiceScoped,
            IDbCampaignService campaignService, IRedditFunction redditFunction)
            : base(processScopeModel, queryScraperFactor, redditLogInProcess, rdHttpHelper)
        {
            _dbAccountServiceScoped = dbAccountServiceScoped;
            _campaignService = campaignService;
            _redditFunction = redditFunction;
            DownvoteModel = processScopeModel.GetActivitySettingsAs<DownvoteModel>();
            _browserManager = redditLogInProcess._browserManager;
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
        }

        /// <summary>
        ///     Intialize the down vote  model
        /// </summary>

        public DownvoteModel DownvoteModel { get; set; }

        /// <summary>
        ///     It will scrape the down vote information with keyword and custom url's
        /// </summary>
        /// <param name="scrapeResult"></param>
        /// <returns></returns>

        // ReSharper disable once InheritdocConsiderUsage
        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            var newRedditPost = (RedditPost)scrapeResult.ResultPost;

            if (newRedditPost.IsArchived || newRedditPost.VoteState == -1) return jobProcessResult;
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    "Started Process to Downvote " + newRedditPost.Permalink);
            try
            {
                if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    var response = _redditFunction.NewDownvote(DominatorAccountModel, newRedditPost);

                    if (response.Success)
                    {
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, newRedditPost.Permalink);
                        IncrementCounters();
                        AddDownVotedDataToDataBase(scrapeResult);
                        jobProcessResult.IsProcessSuceessfull = true;
                    }
                    else
                    {
                        RemoveFailedDownvoteDataFromDataBase(scrapeResult);
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, newRedditPost.Permalink,
                            "Reason: Blocked");

                        //Reschedule if action block
                        StopActivityAfterFailedAttemptAndReschedule();
                    }
                }
                //For browser automation
                else
                {
                    if (scrapeResult.QueryInfo.QueryType == "Keywords" ||
                        scrapeResult.QueryInfo.QueryType == "Community Url" ||
                        scrapeResult.QueryInfo.QueryType == "Specific User's Post")
                    {
                        if (_browserManager.DownVote(ActivityType, newRedditPost.Permalink,newRedditPost.Id))
                        {
                            GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, newRedditPost.Permalink);
                            IncrementCounters();
                            AddDownVotedDataToDataBase(scrapeResult);
                            jobProcessResult.IsProcessSuceessfull = true;
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.ActivityFailed,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, newRedditPost.Permalink,
                                "Reason: Blocked");

                            //Reschedule if action block
                            StopActivityAfterFailedAttemptAndReschedule();
                        }
                    }
                    else
                    {
                        if (_browserManager.DownVote(ActivityType, newRedditPost.Permalink,newRedditPost.Id))
                        {
                            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, newRedditPost.Permalink);
                            IncrementCounters();
                            AddDownVotedDataToDataBase(scrapeResult);
                            jobProcessResult.IsProcessSuceessfull = true;
                            _browserManager.CloseBrowser();
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.ActivityFailed,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, newRedditPost.Permalink,
                                "Reason: Blocked");
                            _browserManager.CloseBrowser();

                            //Reschedule if action block
                            StopActivityAfterFailedAttemptAndReschedule();
                        }
                    }
                }
                if (DownvoteModel != null && DownvoteModel.IsEnableAdvancedUserMode && DownvoteModel.EnableDelayBetweenPerformingActionOnSamePost)
                    DelayBeforeNextActivity(DownvoteModel.DelayBetweenPerformingActionOnSamePost.GetRandom());
                else
                    DelayBeforeNextActivity();
            }
            catch (OperationCanceledException)
            {
                _browserManager.CloseBrowser();
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return jobProcessResult;
        }

        /// <summary>
        ///     It will stored the down voted data into database
        /// </summary>
        /// <param name="scrapeResult"></param>
        private void AddDownVotedDataToDataBase(ScrapeResultNew scrapeResult)
        {
            try
            {
                var post = (RedditPost)scrapeResult.ResultPost;
                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleConfiguration =
                    jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (moduleConfiguration != null && moduleConfiguration.IsTemplateMadeByCampaignMode)
                    _campaignService.Add(new CampaignTables.InteractedPost
                    {
                        ActivityType = ActivityType.ToString(),
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        Query = scrapeResult.QueryInfo.QueryValue,
                        PostId = post.Id,
                        Created = post.Created,
                        Caption = post.Caption,
                        IsCrosspostable = post.IsCrosspostable,
                        IsStickied = post.IsStickied,
                        Saved = post.Saved,
                        NumComments = post.NumComments,
                        InteractedUserName = post.Author,
                        NumCrossposts = post.NumCrossposts,
                        IsSponsored = post.IsSponsored,
                        IsLocked = post.IsLocked,
                        Score = post.Score,
                        IsArchived = post.IsArchived,
                        Hidden = post.Hidden,
                        Preview = post.Preview,
                        IsRoadblock = post.IsRoadblock,
                        SendReplies = post.SendReplies,
                        GoldCount = post.GoldCount,
                        IsSpoiler = post.IsSpoiler,
                        IsNsfw = post.IsNsfw,
                        IsMediaOnly = post.IsMediaOnly,
                        IsBlank = post.IsBlank,
                        Status = "Success",
                        SinAccUsername = DominatorAccountModel.AccountBaseModel.UserName,
                        SinAccId = DominatorAccountModel.AccountBaseModel.AccountId,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        InteractionDateTime = DateTime.Now,
                        Permalink = post.Permalink,
                        Title = post.Title,
                        ViewCount = post.ViewCount
                    });

                _dbAccountServiceScoped.Add(new InteractedPost
                {
                    ActivityType = ActivityType.ToString(),
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    Query = scrapeResult.QueryInfo.QueryValue,
                    PostId = post.Id,
                    Caption = post.Caption,
                    IsCrosspostable = post.IsCrosspostable,
                    IsStickied = post.IsStickied,
                    Saved = post.Saved,
                    Created = post.Created,
                    NumComments = post.NumComments,
                    InteractedUserName = post.Author,
                    NumCrossposts = post.NumCrossposts,
                    IsSponsored = post.IsSponsored,
                    IsLocked = post.IsLocked,
                    Score = post.Score,
                    IsArchived = post.IsArchived,
                    Hidden = post.Hidden,
                    Preview = post.Preview,
                    IsRoadblock = post.IsRoadblock,
                    SendReplies = post.SendReplies,
                    GoldCount = post.GoldCount,
                    IsSpoiler = post.IsSpoiler,
                    IsNsfw = post.IsNsfw,
                    IsMediaOnly = post.IsMediaOnly,
                    IsBlank = post.IsBlank,
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    InteractionDateTime = DateTime.Now,
                    Permalink = post.Permalink,
                    Title = post.Title,
                    ViewCount = post.ViewCount
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        /// <summary>
        ///     Other configration for down vote
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
        ///     To remove failed votes from database
        /// </summary>
        /// <param name="scrapeResult"></param>
        public void RemoveFailedDownvoteDataFromDataBase(ScrapeResultNew scrapeResult)
        {
            try
            {
                var dboperationCampaign =
                    new DbOperations(CampaignId, SocialNetworks.Reddit, ConstantVariable.GetCampaignDb);
                var post = (RedditPost)scrapeResult.ResultPost;
                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleConfiguration =
                    jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (moduleConfiguration == null || !moduleConfiguration.IsTemplateMadeByCampaignMode) return;
                if (!ModuleSetting.EnableDelayBetweenPerformingActionOnSamePost &&
                    !ModuleSetting.IsPerformActionFromRandomPercentageOfAccounts)
                    return;
                var interactedPost =
                    dboperationCampaign.GetSingle<CampaignTables.InteractedPost>(
                        x => x.Permalink == post.Permalink &&
                             x.SinAccUsername == DominatorAccountModel.AccountBaseModel.UserName &&
                             (x.Status == "Pending" || x.Status == "Working"));
                if (interactedPost != null) dboperationCampaign.Remove(interactedPost);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void StopActivityAfterFailedAttemptAndReschedule()
        {
            if (DownvoteModel.IsChkStopActivityAfterXXFailed &&
                _activityFailedCount++ == DownvoteModel.ActivityFailedCount)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    $"{"LangKeyStopActivityAfter".FromResourceDictionary()} {_activityFailedCount - 1} " +
                    $"{"LangKeyFailedAttemptFor".FromResourceDictionary()} {DownvoteModel.FailedActivityReschedule} " +
                    $"{"LangKeyHour".FromResourceDictionary()}");

                StopAndRescheduleJob(DownvoteModel.FailedActivityReschedule);
            }
        }
    }
}