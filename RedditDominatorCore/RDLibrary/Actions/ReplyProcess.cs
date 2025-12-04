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
using System.Linq;
using System.Text.RegularExpressions;
using CampaignTables = DominatorHouseCore.DatabaseHandler.RdTables.Campaigns;
namespace RedditDominatorCore.RDLibrary.Actions
{
    internal class ReplyProcess : RdJobProcessInteracted<InteractedPost>
    {
        private readonly IRdBrowserManager _browserManager;
        private readonly IDbCampaignService _campaignService;
        private readonly IDbAccountServiceScoped _dbAccountServiceScoped;
        private readonly IJobActivityConfigurationManager _jobActivityConfigurationManager;
        private readonly IRedditFunction _redditFunction;
        private int _activityFailedCount = 1;

        public ReplyProcess(IProcessScopeModel processScopeModel, IRdQueryScraperFactory queryScraperFactor,
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
            ReplyModel = processScopeModel.GetActivitySettingsAs<ReplyModel>();
            _browserManager = redditLogInProcess._browserManager;
            blackListWhitelistHandler =
                new BlackListWhitelistHandler(ModuleSetting, DominatorAccountModel, ActivityType);
        }

        private BlackListWhitelistHandler blackListWhitelistHandler { get; }

        public ReplyModel ReplyModel { get; set; }


        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            try
            {
                var newRedditPost = (RedditPost)scrapeResult.ResultPost;

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (ReplyModel.SkipBlacklist.IsSkipBlackListUsers && ReplyModel.IsChkBroadCastPrivateBlacklist || ReplyModel.IsChkBroadCastGroupBlacklist)
                {
                    var blackListUser = blackListWhitelistHandler.GetBlackListUsers();
                    if (blackListUser != null && blackListUser.Count > 0 && blackListUser.Any(user => user.Equals(newRedditPost.Author)))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                "Skip User " + newRedditPost.Author + ", Present in Blacklist ");
                        return jobProcessResult;
                    }
                }
                var lstCommentedText = GetCommentAsPerQuery(scrapeResult.QueryInfo);
                foreach (var Items in lstCommentedText)
                {
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    string allcommentText;
                    if (ReplyModel.MakeReplyAsSpinText)
                    {
                        allcommentText = SpinTexHelper.GetSpinText(Items);
                    }

                    else
                    {
                        var comment = Regex.Split(Items?.Replace("\r", ""), "\n").ToList();
                        allcommentText = comment.FirstOrDefault(x => x.ToString() != string.Empty);
                    }

                    //setting comment
                    newRedditPost.Commenttext = allcommentText;

                    if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        var response = _redditFunction.NewReply(DominatorAccountModel, newRedditPost);

                        if (response.Success)
                        {
                            GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                newRedditPost.Commenttext);
                            IncrementCounters();
                            AddScrapedPostsToDataBase(scrapeResult);
                            jobProcessResult.IsProcessSuceessfull = true;
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.ActivityFailed,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, "Reason: Blocked");
                            StopActivityAfterFailedAttemptAndReschedule();
                        }
                    }
                    //For browser automation
                    else
                    {
                        if (_browserManager.Reply(DominatorAccountModel, newRedditPost.Commenttext))
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
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, "Reason: Blocked");
                            _browserManager.CloseBrowser();

                            //Reschedule if action block
                            StopActivityAfterFailedAttemptAndReschedule();
                        }
                    }

                    DelayBeforeNextActivity();
                }
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

        public void AddScrapedPostsToDataBase(ScrapeResultNew scrapeResult)
        {
            try
            {
                var post = (RedditPost)scrapeResult.ResultPost;
                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleConfiguration =
                    _jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

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

        private List<string> GetCommentAsPerQuery(QueryInfo queryInfo)
        {
            var listCommentedText = new List<string>();
            try
            {
                ReplyModel.LstManageCommentModel.ForEach(comment =>
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

        private void StopActivityAfterFailedAttemptAndReschedule()
        {
            if (ReplyModel.IsChkStopActivityAfterXXFailed && _activityFailedCount++ == ReplyModel.ActivityFailedCount)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    $"{"LangKeyStopActivityAfter".FromResourceDictionary()} {_activityFailedCount - 1} " +
                    $"{"LangKeyFailedAttemptFor".FromResourceDictionary()} {ReplyModel.FailedActivityReschedule} " +
                    $"{"LangKeyHour".FromResourceDictionary()}");

                StopAndRescheduleJob(ReplyModel.FailedActivityReschedule);
            }
        }
    }
}