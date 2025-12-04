using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.RdTables.Accounts;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using RedditDominatorCore.RDFactories;
using RedditDominatorCore.RDLibrary.DAL;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDRequest;
using System;
using InteractedPost = DominatorHouseCore.DatabaseHandler.RdTables.Campaigns.InteractedPost;

namespace RedditDominatorCore.RDLibrary.Actions
{
    internal class CommentScraperProcess : RdJobProcessInteracted<InteractedSubreddit>
    {
        private readonly IDbCampaignService _campaignService;
        private readonly IDbAccountServiceScoped _dbAccountServiceScoped;

        /// <summary>
        ///     Intialize the dominator account model, per week, per day , per hour
        /// </summary>
        /// <param name="processScopeModel"></param>
        /// <param name="queryScraperFactor"></param>
        public CommentScraperProcess(IProcessScopeModel processScopeModel, IRdQueryScraperFactory queryScraperFactor,
            IRedditLogInProcess redditLogInProcess, IRdHttpHelper rdHttpHelper,
            IDbAccountServiceScoped dbAccountServiceScoped, IDbCampaignService campaignService)
            : base(processScopeModel, queryScraperFactor, redditLogInProcess, rdHttpHelper)
        {
            _dbAccountServiceScoped = dbAccountServiceScoped;
            _campaignService = campaignService;
            CommentScraperModel = processScopeModel.GetActivitySettingsAs<CommentScraperModel>();
        }

        /// <summary>
        ///     Intialize the comment scraper model
        /// </summary>

        public CommentScraperModel CommentScraperModel { get; set; }


        /// <summary>
        ///     Other configration for channel scraper
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
        ///     It will scrape the channel information with keyword and custom url's
        /// </summary>
        /// <param name="scrapeResult"></param>
        /// <returns></returns>

        // ReSharper disable once InheritdocConsiderUsage
        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    ((RedditPost)scrapeResult.ResultPost).Commenttext);

                IncrementCounters();
                AddCommentScrappedDataToDataBase(scrapeResult);
                jobProcessResult.IsProcessSuceessfull = true;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            DelayBeforeNextActivity();

            return jobProcessResult;
        }

        /// <summary>
        ///     It will stored the Scraped channel data into database
        /// </summary>
        /// <param name="scrapeResult"></param>
        public void AddCommentScrappedDataToDataBase(ScrapeResultNew scrapeResult)
        {
            try
            {
                var post = (RedditPost)scrapeResult.ResultPost;
                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleConfiguration =
                    jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (moduleConfiguration != null && moduleConfiguration.IsTemplateMadeByCampaignMode)
                    _campaignService.Add(new InteractedPost
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
                        CommentText = post.Commenttext,
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
                        ViewCount = post.ViewCount
                    });

                _dbAccountServiceScoped.Add(new DominatorHouseCore.DatabaseHandler.RdTables.Accounts.InteractedPost
                {
                    ActivityType = ActivityType.ToString(),
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    Query = scrapeResult.QueryInfo.QueryValue,
                    PostId = post.Id,
                    Caption = post.Caption,
                    Created = post.Created,
                    IsCrosspostable = post.IsCrosspostable,
                    IsStickied = post.IsStickied,
                    Saved = post.Saved,
                    NumComments = post.NumComments,
                    InteractedUserName = post.Author,
                    NumCrossposts = post.NumCrossposts,
                    CommentText = post.Commenttext,
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
    }
}