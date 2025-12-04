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
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ThreadUtils;
using CampaignTables = DominatorHouseCore.DatabaseHandler.RdTables.Campaigns;

namespace RedditDominatorCore.RDLibrary.Actions
{
    internal class CommentProcess : RdJobProcessInteracted<InteractedPost>
    {
        public static readonly object LockUniqueFromEachAccount = new object();

        /// <summary>
        ///     Intialize the dominator model , per week, per day , per hour
        /// </summary>
        /// <param name="processScopeModel"></param>
        /// <param name="queryScraperFactor"></param>
        /// <param name="redditLogInProcess"></param>
        /// <param name="rdHttpHelper"></param>
        /// <param name="dbAccountServiceScoped"></param>
        /// <param name="campaignService"></param>
        /// <param name="redditFunction"></param>
        /// <param name="delayService"></param>
        public CommentProcess(IProcessScopeModel processScopeModel, IRdQueryScraperFactory queryScraperFactor,
            IRedditLogInProcess redditLogInProcess, IRdHttpHelper rdHttpHelper,
            IDbAccountServiceScoped dbAccountServiceScoped,
            IDbCampaignService campaignService, IRedditFunction redditFunction, IDelayService delayService)
            : base(processScopeModel, queryScraperFactor, redditLogInProcess, rdHttpHelper)
        {
            _dbAccountServiceScoped = dbAccountServiceScoped;
            _campaignService = campaignService;
            _redditFunction = redditFunction;
            CommentModel = processScopeModel.GetActivitySettingsAs<CommentModel>();
            _campaignId = processScopeModel.TemplateId;
            _browserManager = redditLogInProcess._browserManager;
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            _delayService = delayService;
            blackListWhitelistHandler =
                new BlackListWhitelistHandler(ModuleSetting, DominatorAccountModel, ActivityType);
        }

        private void StopActivityAfterFailedAttemptAndReschedule()
        {
            if (CommentModel.IsChkStopActivityAfterXXFailed &&
                _activityFailedCount++ == CommentModel.ActivityFailedCount)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    $"{"LangKeyStopActivityAfter".FromResourceDictionary()} {_activityFailedCount - 1} " +
                    $"{"LangKeyFailedAttemptFor".FromResourceDictionary()} {CommentModel.FailedActivityReschedule} " +
                    $"{"LangKeyHour".FromResourceDictionary()}");

                StopAndRescheduleJob(CommentModel.FailedActivityReschedule);
            }
        }

        #region Private fields

        private BlackListWhitelistHandler blackListWhitelistHandler { get; }
        private readonly IDbAccountServiceScoped _dbAccountServiceScoped;
        private readonly IDbCampaignService _campaignService;
        private readonly IRedditFunction _redditFunction;
        private readonly string _campaignId;
        private readonly IDelayService _delayService;
        private readonly Random _random = new Random();
        private readonly IRdBrowserManager _browserManager;
        private readonly IAccountScopeFactory _accountScopeFactory;
        private IRdBrowserManager _newBrowserWindow;
        private int _activityFailedCount = 1;

        #endregion

        #region Public properties

        public CommentModel CommentModel { get; set; }

        public static List<KeyValuePair<string, string>> UniqueCommentForPostEachAccount =
            new List<KeyValuePair<string, string>>();

        #endregion

        #region Public Methods

        /// <summary>
        ///     To perform other configuration for comment
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
        ///     It will comment on perticular user
        /// </summary>
        /// <param name="scrapeResult"></param>
        /// <returns></returns>
        // ReSharper disable once InheritdocConsiderUsage
        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var allcommentText = string.Empty;
            var jobProcessResult = new JobProcessResult();
            try
            {
                var newRedditPost = (RedditPost)scrapeResult.ResultPost;
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (CommentModel.SkipBlacklist.IsSkipBlackListUsers && CommentModel.SkipBlacklist.IsSkipPrivateBlackListUser || CommentModel.SkipBlacklist.IsSkipGroupBlackListUsers)
                {
                    var blackListUser = blackListWhitelistHandler.GetBlackListUsers();
                    if (blackListUser != null && blackListUser.Count > 0 && blackListUser.Any(user => user.Equals(newRedditPost.Author)))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, "Skip User " + newRedditPost.Author + ", Present in Blacklist ");
                        return jobProcessResult;
                    }
                }
                // Getting all comments
                var lstCommentText = GetCommentAsPerQuery(scrapeResult.QueryInfo);
                lstCommentText.Shuffle();
                foreach (var Items in lstCommentText)
                {
                    var comment = new List<string>();
                    comment = CommentModel.MakeCommentAsSpinText ? SpinTexHelper.GetSpinMessageCollection(Items) : Regex.Split(Items?.Replace("\r", ""), "\n").ToList();
                    if (CommentModel.IsUniqueComment)
                    {
                        #region  Is Unique

                        if (CommentModel.IsChkPostUniqueCommentOnPostFromEachAccount)
                        {
                            _delayService.ThreadSleep(_random.Next(5000, 10000));
                            lock (LockUniqueFromEachAccount)
                                allcommentText = GetUniqueCommentPostForEachAccount(newRedditPost, comment);
                        }
                        else if (CommentModel.IsChkCommentOnceFromEachAccount)
                            allcommentText = GetCommentOnceFromEachAccount(comment);
                        if (string.IsNullOrEmpty(allcommentText))
                        {
                            var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                            dominatorScheduler.ChangeAccountsRunningStatus(false, AccountId, ActivityType);
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType,
                                "Please add more comments for configuration('Use specific comment only once from a account.')");
                            return jobProcessResult;
                        }
                        #endregion
                    }
                    else
                        allcommentText = CommentModel.MakeCommentAsSpinText ? SpinTexHelper.GetSpinText(Items) : comment.FirstOrDefault(x => x.ToString() != string.Empty);
                    //setting comment
                    newRedditPost.Commenttext = allcommentText;
                    var campaignIdWithPost = _campaignId + newRedditPost.PostId;
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType,
                        $"Trying To Comment {newRedditPost.Commenttext} On ==> {newRedditPost.Permalink}");
                    if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        var response = _redditFunction.NewComment(DominatorAccountModel, newRedditPost);
                        if (response.Success)
                        {
                            GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                newRedditPost.Commenttext + " - " + newRedditPost.Permalink);
                            IncrementCounters();
                            AddScrapedPostsToDataBase(scrapeResult);
                            jobProcessResult.IsProcessSuceessfull = true;
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.ActivityFailed,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, response.ErrorMessage);

                            UniqueCommentForPostEachAccount.RemoveAll(x =>
                                x.Key == campaignIdWithPost && x.Value == allcommentText);

                            //Reschedule if action block
                            StopActivityAfterFailedAttemptAndReschedule();
                        }
                    }
                    //For browser automation
                    else
                    {
                        if (scrapeResult.QueryInfo.QueryType == "Keywords")
                        {
                            var CommentResponse = _browserManager.Comment(DominatorAccountModel, newRedditPost.Commenttext,newRedditPost.Id);
                            if (CommentResponse.Status)
                            {
                                GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    newRedditPost.Commenttext + " - " + newRedditPost.Permalink);
                                IncrementCounters();
                                AddScrapedPostsToDataBase(scrapeResult);
                                jobProcessResult.IsProcessSuceessfull = true;
                            }
                            else
                            {
                                GlobusLogHelper.log.Info(Log.ActivityFailed,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, CommentResponse.ResponseMessage);

                                //Reschedule if action block
                                StopActivityAfterFailedAttemptAndReschedule();
                            }
                        }
                        else
                        {
                            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            if (newRedditPost.Permalink != _browserManager.BrowserWindow.CurrentUrl())
                                _browserManager.SearchByCustomUrl(DominatorAccountModel, newRedditPost.Permalink);
                            var CommentResponse = (scrapeResult.QueryInfo.QueryType == "Socinator Publisher Campaign") ?
                                _browserManager.CommentInPublisherPost(DominatorAccountModel, newRedditPost.Commenttext) :
                                _browserManager.Comment(DominatorAccountModel, newRedditPost.Commenttext, newRedditPost.Id);
                            if (CommentResponse.Status)
                            {
                                GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    newRedditPost.Commenttext + " - " + newRedditPost.Permalink);
                                IncrementCounters();
                                AddScrapedPostsToDataBase(scrapeResult);
                                jobProcessResult.IsProcessSuceessfull = true;
                                _browserManager.CloseBrowser();
                            }
                            else
                            {
                                GlobusLogHelper.log.Info(Log.ActivityFailed,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, CommentResponse.ResponseMessage);
                                _browserManager.CloseBrowser();

                                //Reschedule if action block
                                StopActivityAfterFailedAttemptAndReschedule();
                            }
                        }
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
                    Created = post.Created,
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

        #endregion

        #region Private Methods

        /// <summary>
        ///     Other configuration for unique comment
        /// </summary>
        private string GetUniqueCommentPostForEachAccount(RedditPost reddit, List<string> listCommentedText)
        {
            var comment = "";
            try
            {
                var campaignIdWithPost = _campaignId + reddit.Id;
                var commentsOnCurrentPost = UniqueCommentForPostEachAccount.Where(x => x.Key == campaignIdWithPost)
                    .Select(y => y.Value).ToList();
                listCommentedText.RemoveAll(x => commentsOnCurrentPost.Contains(x));
                comment = listCommentedText.FirstOrDefault();
                UniqueCommentForPostEachAccount.Add(new KeyValuePair<string, string>(campaignIdWithPost, comment));

                if (UniqueCommentForPostEachAccount.Count > 1500) UniqueCommentForPostEachAccount.RemoveRange(0, 500);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return comment;
        }

        private string GetCommentOnceFromEachAccount(List<string> listCommentedText)
        {
            var comment = string.Empty;
            try
            {
                var listMessages = _campaignService.GetInteractedPost(DominatorAccountModel.AccountBaseModel.UserName)
                    .Select(y => y.CommentText).ToList();
                listCommentedText.RemoveAll(x => listMessages.Contains(x));
                comment = listCommentedText.FirstOrDefault();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return comment;
        }

        private List<string> GetCommentAsPerQuery(QueryInfo queryInfo)
        {
            var listCommentedText = new List<string>();
            try
            {
                CommentModel.LstManageCommentModel.ForEach(comment =>
                {
                    if (!listCommentedText.Contains(comment.CommentText))
                        comment.SelectedQuery.ForEach(query =>
                        {
                            if (query.Content.QueryValue == queryInfo.QueryValue &&
                                query.Content.QueryType == queryInfo.QueryType)
                                listCommentedText.Add(comment.CommentText);
                        });
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return listCommentedText;
        }

        #endregion
    }
}