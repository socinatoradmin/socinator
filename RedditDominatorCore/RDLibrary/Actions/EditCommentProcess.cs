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
using System.Collections.Generic;
using ThreadUtils;
using CampaignTables = DominatorHouseCore.DatabaseHandler.RdTables.Campaigns;

namespace RedditDominatorCore.RDLibrary.Actions
{
    internal class EditCommentProcess : RdJobProcessInteracted<InteractedPost>
    {
        public EditCommentProcess(IProcessScopeModel processScopeModel, IRdQueryScraperFactory queryScraperFactor,
            IRedditLogInProcess redditLogInProcess, IRdHttpHelper rdHttpHelper,
            IDbAccountServiceScoped dbAccountServiceScoped,
            IDbCampaignService campaignService, IRedditFunction redditFunction, IDelayService delayService)
            : base(processScopeModel, queryScraperFactor, redditLogInProcess, rdHttpHelper)
        {
            _dbAccountServiceScoped = dbAccountServiceScoped;
            _campaignService = campaignService;
            _redditFunction = redditFunction;
            EditCommentModel = processScopeModel.GetActivitySettingsAs<EditCommentModel>();
            _campaignId = processScopeModel.TemplateId;
            _browserManager = redditLogInProcess._browserManager;
            _delayService = delayService;
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            var newRedditPost = (RedditPost)scrapeResult.ResultPost;
            try
            {
                if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    var response = _redditFunction.EditComment(DominatorAccountModel, newRedditPost);

                    if (response.Success)
                    {
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, newRedditPost.Commenttext);
                        IncrementCounters();
                        AddScrapedPostsToDataBase(scrapeResult);
                        jobProcessResult.IsProcessSuceessfull = true;
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, response.ErrorMessage);
                    }
                }
                //For browser automation
                else
                {
                    if (_browserManager.EditComment(DominatorAccountModel, newRedditPost.Commenttext))
                    {
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, newRedditPost.Commenttext);
                        IncrementCounters();
                        AddScrapedPostsToDataBase(scrapeResult);
                        jobProcessResult.IsProcessSuceessfull = true;
                        _browserManager.CloseBrowser();
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, "Failed to EditComment");
                        _browserManager.CloseBrowser();
                    }
                }

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
        ///     It will add the commented user information
        /// </summary>
        /// <param name="scrapeResult"></param>
        public void AddScrapedPostsToDataBase(ScrapeResultNew scrapeResult)
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
                        Caption = post.Caption,
                        IsCrosspostable = post.IsCrosspostable,
                        IsStickied = post.IsStickied,
                        Saved = post.Saved,
                        NumComments = post.NumComments,
                        InteractedUserName = post.Author,
                        NumCrossposts = post.NumCrossposts,
                        CommentText = post.Commenttext,
                        OldComment = post.OldComment,
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
                    CommentText = post.Commenttext,
                    OldComment = post.OldComment,
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

        #region Private fields

        private readonly IDbAccountServiceScoped _dbAccountServiceScoped;
        private readonly IDbCampaignService _campaignService;
        private readonly IRedditFunction _redditFunction;
        private readonly string _campaignId;
        private readonly IDelayService _delayService;
        private readonly IRdBrowserManager _browserManager;

        #endregion

        #region Public properties

        public EditCommentModel EditCommentModel { get; set; }

        public static List<KeyValuePair<string, string>> UniqueCommentForPostEachAccount =
            new List<KeyValuePair<string, string>>();

        public static readonly object LockUniqueFromEachAccount = new object();

        #endregion
    }
}