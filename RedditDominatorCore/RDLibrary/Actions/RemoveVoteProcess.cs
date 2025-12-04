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
using System;

namespace RedditDominatorCore.RDLibrary.Actions
{
    internal class RemoveVoteProcess : RdJobProcessInteracted<InteractedPost>
    {
        private readonly IAccountScopeFactory _accountScopeFactory;
        private readonly IRdBrowserManager _browserManager;
        private readonly IDbCampaignService _campaignService;
        private readonly IDbAccountServiceScoped _dbAccountServiceScoped;
        private readonly IJobActivityConfigurationManager _jobActivityConfigurationManager;
        private readonly IRedditFunction _redditFunction;
        private int _activityFailedCount = 1;
        private IRdBrowserManager _newBrowserWindow;

        /// <summary>
        ///     Intialize the dominator account model, per week, per day , per hour
        /// </summary>
        /// <param name="processScopeModel"></param>
        /// <param name="queryScraperFactor"></param>
        public RemoveVoteProcess(IProcessScopeModel processScopeModel, IRdQueryScraperFactory queryScraperFactor,
            IRedditLogInProcess redditLogInProcess, IRdHttpHelper rdHttpHelper,
            IDbAccountServiceScoped dbAccountServiceScoped,
            IDbCampaignService campaignService, IRedditFunction redditFunction,
            IJobActivityConfigurationManager jobActivityConfigurationManager)
            : base(processScopeModel, queryScraperFactor, redditLogInProcess, rdHttpHelper)
        {
            _dbAccountServiceScoped = dbAccountServiceScoped;
            _campaignService = campaignService;
            _redditFunction = redditFunction;
            _jobActivityConfigurationManager = jobActivityConfigurationManager;
            RemoveVoteModel = processScopeModel.GetActivitySettingsAs<RemoveVoteModel>();
            _browserManager = redditLogInProcess._browserManager;
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
        }

        /// <summary>
        ///     Intialize the Remove vote model
        /// </summary>

        public RemoveVoteModel RemoveVoteModel { get; set; }

        /// <summary>
        ///     It will remove the vote with keyword and custom url's on particular user
        /// </summary>
        /// <param name="scrapeResult"></param>
        /// <returns></returns>

        // ReSharper disable once InheritdocConsiderUsage
        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            var newRedditPost = (RedditPost)scrapeResult.ResultPost;
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            try
            {
                if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    var response = _redditFunction.NewRemoveVote(DominatorAccountModel, newRedditPost);
                    if (response.Success)
                    {
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, newRedditPost.Permalink);
                        IncrementCounters();
                        AddRemoveVoteDataToDataBase(scrapeResult);
                        jobProcessResult.IsProcessSuceessfull = true;
                    }
                    else
                    {
                        RemoveFailedRemoveVoteDataFromDataBase(scrapeResult);
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
                        if (_browserManager.RemoveVote(ActivityType,newRedditPost))
                        {
                            GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, newRedditPost.Permalink);
                            IncrementCounters();
                            AddRemoveVoteDataToDataBase(scrapeResult);
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
                        if (_browserManager.RemoveVote(ActivityType,newRedditPost))
                        {
                            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, newRedditPost.Permalink);
                            IncrementCounters();
                            AddRemoveVoteDataToDataBase(scrapeResult);
                            jobProcessResult.IsProcessSuceessfull = true;
                            _browserManager.CloseBrowser();
                        }
                        else
                        {
                            RemoveFailedRemoveVoteDataFromDataBase(scrapeResult);
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

                if (RemoveVoteModel != null && RemoveVoteModel.IsEnableAdvancedUserMode && RemoveVoteModel.EnableDelayBetweenPerformingActionOnSamePost)
                    DelayBeforeNextActivity(RemoveVoteModel.DelayBetweenPerformingActionOnSamePost.GetRandom());
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
        ///     Other configration for remove vote
        /// </summary>
        /// <param name="scrapeResult"></param>

        // ReSharper disable once InheritdocConsiderUsage
        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
        }

        /// <summary>
        ///     It will stored the Scraped channel data into database
        /// </summary>
        /// <param name="scrapeResult"></param>
        public void AddRemoveVoteDataToDataBase(ScrapeResultNew scrapeResult)
        {
            try
            {
                var post = (RedditPost)scrapeResult.ResultPost;
                var moduleConfiguration =
                    _jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (moduleConfiguration != null && moduleConfiguration.IsTemplateMadeByCampaignMode)
                    _campaignService.Add(new DominatorHouseCore.DatabaseHandler.RdTables.Campaigns.InteractedPost
                    {
                        ActivityType = ActivityType.ToString(),
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        Query = scrapeResult.QueryInfo.QueryValue,
                        PostId = post.Id,
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
        ///     To remove failed votes from database
        /// </summary>
        /// <param name="scrapeResult"></param>
        public void RemoveFailedRemoveVoteDataFromDataBase(ScrapeResultNew scrapeResult)
        {
            try
            {
                var post = (RedditPost)scrapeResult.ResultPost;
                var moduleConfiguration =
                    _jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (moduleConfiguration == null || !moduleConfiguration.IsTemplateMadeByCampaignMode) return;
                if (!ModuleSetting.EnableDelayBetweenPerformingActionOnSamePost &&
                    !ModuleSetting.IsPerformActionFromRandomPercentageOfAccounts) return;
                var interactedPost =
                    _campaignService.GetSingle<DominatorHouseCore.DatabaseHandler.RdTables.Campaigns.InteractedPost>(
                        x => x.Permalink == post.Permalink &&
                             x.SinAccUsername == DominatorAccountModel.AccountBaseModel.UserName &&
                             (x.Status == "Pending" || x.Status == "Working"));
                if (interactedPost != null) _campaignService.Remove(interactedPost);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void StopActivityAfterFailedAttemptAndReschedule()
        {
            if (RemoveVoteModel.IsChkStopActivityAfterXXFailed &&
                _activityFailedCount++ == RemoveVoteModel.ActivityFailedCount)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    $"{"LangKeyStopActivityAfter".FromResourceDictionary()} {_activityFailedCount - 1} " +
                    $"{"LangKeyFailedAttemptFor".FromResourceDictionary()} {RemoveVoteModel.FailedActivityReschedule} " +
                    $"{"LangKeyHour".FromResourceDictionary()}");

                StopAndRescheduleJob(RemoveVoteModel.FailedActivityReschedule);
            }
        }
    }
}