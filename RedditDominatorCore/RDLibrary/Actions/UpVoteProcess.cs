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

namespace RedditDominatorCore.RDLibrary.Actions
{
    internal class UpVoteProcess : RdJobProcessInteracted<InteractedPost>
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
        public UpVoteProcess(IProcessScopeModel processScopeModel, IRdQueryScraperFactory queryScraperFactor,
            IRedditLogInProcess redditLogInProcess, IRdHttpHelper rdHttpHelper,
            IDbAccountServiceScoped dbAccountServiceScoped,
            IDbCampaignService campaignService, IRedditFunction redditFunction)
            : base(processScopeModel, queryScraperFactor, redditLogInProcess, rdHttpHelper)
        {
            _dbAccountServiceScoped = dbAccountServiceScoped;
            _campaignService = campaignService;
            _redditFunction = redditFunction;
            UpvoteModel = processScopeModel.GetActivitySettingsAs<UpvoteModel>();
            _browserManager = redditLogInProcess._browserManager;
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
        }

        /// <summary>
        ///     Intialize the up vote model
        /// </summary>

        public UpvoteModel UpvoteModel { get; set; }

        /// <summary>
        ///     It will up vote with keyword and custom url's to particular user
        /// </summary>
        /// <param name="scrapeResult"></param>
        /// <returns></returns>

        // ReSharper disable once InheritdocConsiderUsage
        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            var newRedditPost = (RedditPost)scrapeResult.ResultPost;
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    "Started Process to Upvote " + newRedditPost.Permalink);
            try
            {
                if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    var response = _redditFunction.NewUpvote(DominatorAccountModel, newRedditPost);
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (response != null && response.Success)
                    {
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, newRedditPost.Permalink);
                        IncrementCounters();

                        AddUpvoteDataToDataBase(scrapeResult);
                        jobProcessResult.IsProcessSuceessfull = true;
                    }
                    else
                    {
                        //RemoveFailedUpvoteDataFromDataBase(scrapeResult);
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, response.ErrorMessage);

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
                        if (_browserManager.UpVote(ActivityType, newRedditPost.Permalink,id: newRedditPost.Id).Item1)
                        {
                            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, newRedditPost.Permalink);
                            IncrementCounters();
                            AddUpvoteDataToDataBase(scrapeResult);
                            jobProcessResult.IsProcessSuceessfull = true;
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.ActivityFailed,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, newRedditPost.Permalink);

                            //Reschedule if action block
                            StopActivityAfterFailedAttemptAndReschedule();
                        }
                    }
                    else
                    {
                        if (_browserManager.UpVote(ActivityType, newRedditPost.Permalink, id: newRedditPost.Id).Item1)
                        {
                            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, newRedditPost.Permalink);
                            IncrementCounters();
                            AddUpvoteDataToDataBase(scrapeResult);
                            jobProcessResult.IsProcessSuceessfull = true;
                            _browserManager.CloseBrowser();
                        }
                        else
                        {
                            RemoveFailedUpvoteDataFromDataBase(scrapeResult);
                            GlobusLogHelper.log.Info(Log.ActivityFailed,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, newRedditPost.Permalink);
                            _browserManager.CloseBrowser();

                            //Reschedule if action block
                            StopActivityAfterFailedAttemptAndReschedule();
                        }
                    }
                }
                if (UpvoteModel != null && UpvoteModel.IsEnableAdvancedUserMode && UpvoteModel.EnableDelayBetweenPerformingActionOnSamePost)
                    DelayBeforeNextActivity(UpvoteModel.DelayBetweenPerformingActionOnSamePost.GetRandom());
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
        ///     Other configration for up vote
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
        ///     It will stored the up voted user data into database
        /// </summary>
        /// <param name="scrapeResult"></param>
        public void AddUpvoteDataToDataBase(ScrapeResultNew scrapeResult)
        {
            try
            {
                var post = (RedditPost)scrapeResult.ResultPost;
                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleConfiguration =
                    jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (moduleConfiguration != null && moduleConfiguration.IsTemplateMadeByCampaignMode)
                {
                    if (ModuleSetting.EnableDelayBetweenPerformingActionOnSamePost ||
                        ModuleSetting.IsPerformActionFromRandomPercentageOfAccounts)
                    {
                        var interactedPost =
                            _campaignService.GetSingleInteractedPost(post.Permalink, ActivityType,
                                DominatorAccountModel);
                        if (interactedPost != null)
                        {
                            interactedPost.PostId = post.Id;
                            interactedPost.Caption = post.Caption;
                            interactedPost.SinAccUsername = DominatorAccountModel.AccountBaseModel.UserName;
                            interactedPost.IsCrosspostable = post.IsCrosspostable;
                            interactedPost.IsStickied = post.IsStickied;
                            interactedPost.Saved = post.Saved;
                            interactedPost.NumComments = post.NumComments;
                            interactedPost.InteractedUserName = post.Author;
                            interactedPost.NumCrossposts = post.NumCrossposts;
                            interactedPost.IsSponsored = post.IsSponsored;
                            interactedPost.IsLocked = post.IsLocked;
                            interactedPost.Score = post.Score;
                            interactedPost.IsArchived = post.IsArchived;
                            interactedPost.Hidden = post.Hidden;
                            interactedPost.Preview = post.Preview;
                            interactedPost.IsRoadblock = post.IsRoadblock;
                            interactedPost.SendReplies = post.SendReplies;
                            interactedPost.GoldCount = post.GoldCount;
                            interactedPost.IsSpoiler = post.IsSpoiler;
                            interactedPost.IsNsfw = post.IsNsfw;
                            interactedPost.IsMediaOnly = post.IsMediaOnly;
                            interactedPost.IsBlank = post.IsBlank;
                            interactedPost.Status = "Success";
                            interactedPost.InteractionTimeStamp = DateTimeUtilities.GetEpochTime();
                            interactedPost.InteractionDateTime = DateTime.Now;
                            interactedPost.Created = post.Created;
                            interactedPost.Title = post.Title;
                            _campaignService.Update(interactedPost);
                        }
                        else
                        {
                            _campaignService.Add(
                                new DominatorHouseCore.DatabaseHandler.RdTables.Campaigns.InteractedPost
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
                                    SinAccUsername = DominatorAccountModel.AccountBaseModel.UserName,
                                    SinAccId = DominatorAccountModel.AccountBaseModel.AccountId,
                                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                                    InteractionDateTime = DateTime.Now,
                                    Permalink = post.Permalink,
                                    Title = post.Title,
                                    ViewCount = post.ViewCount,
                                    Status = "Success",
                                    Created = post.Created
                                });
                        }
                    }
                    else
                    {
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
                            SinAccUsername = DominatorAccountModel.AccountBaseModel.UserName,
                            SinAccId = DominatorAccountModel.AccountBaseModel.AccountId,
                            InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                            InteractionDateTime = DateTime.Now,
                            Permalink = post.Permalink,
                            Title = post.Title,
                            ViewCount = post.ViewCount,
                            Status = "Success",
                            Created = post.Created
                        });
                    }
                }

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
                    //SinAccUsername = DominatorAccountModel.AccountBaseModel.UserName,
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    InteractionDateTime = DateTime.Now,
                    Permalink = post.Permalink,
                    Title = post.Title,
                    ViewCount = post.ViewCount,
                    Created = post.Created
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
        public void RemoveFailedUpvoteDataFromDataBase(ScrapeResultNew scrapeResult)
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
                    !ModuleSetting.IsPerformActionFromRandomPercentageOfAccounts) return;
                var interactedPost =
                    dboperationCampaign.GetSingle<DominatorHouseCore.DatabaseHandler.RdTables.Campaigns.InteractedPost>(
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
            if (UpvoteModel.IsChkStopActivityAfterXXFailed && _activityFailedCount++ == UpvoteModel.ActivityFailedCount)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    $"{"LangKeyStopActivityAfter".FromResourceDictionary()} {_activityFailedCount - 1} " +
                    $"{"LangKeyFailedAttemptFor".FromResourceDictionary()} {UpvoteModel.FailedActivityReschedule} " +
                    $"{"LangKeyHour".FromResourceDictionary()}");

                StopAndRescheduleJob(UpvoteModel.FailedActivityReschedule);
            }
        }
    }
}