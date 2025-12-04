using DominatorHouseCore.DatabaseHandler.TumblrTables.Account;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.TumblrFactory;
using TumblrDominatorCore.TumblrLibrary.DAL;
using TumblrDominatorCore.TumblrLibrary.TumblrFunction;
using TumblrDominatorCore.TumblrRequest;
using TumblrDominatorCore.TumblrResponseHandler;

namespace TumblrDominatorCore.TumblrLibrary.TumblrProcesses
{
    public class CommentScraperJobProcess : TumblrJobProcessInteracted<InteractedPosts>
    {
        private readonly ITumblrFunct TumblrFunct;

        protected readonly IDbAccountService _dbAccountService;
        public CommentScraperJobProcess(IProcessScopeModel processScopeModel, IDbAccountServiceScoped _accountService,
            IDbGlobalService _dbGlobalService, IExecutionLimitsManager executionLimitsManager,
            ITumblrFunct _tumblrFunct, ITumblrQueryScraperFactory queryScraperFactory, ITumblrHttpHelper _httpHelper,
            ITumblrLoginProcess _tumblrLoginProcess) :
            base(processScopeModel, _accountService, _dbGlobalService, executionLimitsManager, queryScraperFactory,
                _httpHelper, _tumblrLoginProcess)
        {
            TumblrFunct = _tumblrFunct;
            CommentScraperModel = processScopeModel.GetActivitySettingsAs<CommentScraperModel>();
            _dbAccountService = _accountService;
        }

        public CommentScraperModel CommentScraperModel { get; set; }
        public List<TumblrComments> lstComments { get; set; }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResultNew)
        {
            var scrapeResult = (TumblrScrapeResult)scrapeResultNew;
            var tumblrPost = (TumblrPost)scrapeResult.ResultPost;
            CommentScraperResponse response;
            var jobProcessResult = new JobProcessResult();
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            GlobusLogHelper.log.Info(Log.CustomMessage,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType.ToString(),
                        $"Trying To Get Details of ==> {tumblrPost?.PostUrl} ");
            if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                response = TumblrFunct.CommentScraper(DominatorAccountModel, tumblrPost, scrapeResult.TumblrFormKey);
            else
            {
                var status = browserManager.SearchPostDetails(DominatorAccountModel, ref tumblrPost);
                var QueryActivity = TumblrUtility.GetActivityByQuery(scrapeResult.QueryInfo.QueryType);
                var FeedResponse = browserManager.GetFeedDetails(DominatorAccountModel, tumblrPost, QueryActivity.Item1, QueryActivity.Item2, QueryActivity.Item3);
                tumblrPost.ListComments = QueryActivity.Item1 ? FeedResponse.Item1 : QueryActivity.Item2 ? FeedResponse.Item2 : FeedResponse.Item3;
                response = new CommentScraperResponse(new ResponseParameter(), DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    post = tumblrPost
                };
            }
            if (response.Success)
            {
                scrapeResult.ResultPost = tumblrPost;
                var skippedCount = 0;
                foreach (var comment in response.post.ListComments)
                {
                    var alreadyUsed = _dbAccountService.GetInteractedPosts(ActivityType);
                    var resultIds = alreadyUsed.Select(x => x.CommentId).ToList();
                    var resultUserNames = alreadyUsed.Select(x => x.InteractedUserName).ToList();
                    if (resultIds.Contains(comment.CommentId) && resultUserNames.Contains(comment.commenter.Username))
                    { skippedCount++; continue; }
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType.ToString(),
                        " of " + comment.commenter.Username + " For Post => " + tumblrPost.PostUrl);

                    IncrementCounters();

                    AddScrapedCommentsDataToDataBase(scrapeResult, DominatorAccountModel.AccountBaseModel.UserName,
                        comment);
                    var AccountModel = new AccountModel(DominatorAccountModel);
                    if (AccountModel.LstPost == null)
                        AccountModel.LstPost = new List<TumblrPost>();
                    AccountModel.LstPost.Add(tumblrPost);
                    jobProcessResult.IsProcessSuceessfull = true;
                    DelayBeforeNextActivity();
                }
                if (skippedCount > 0)
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                       DominatorAccountModel.AccountBaseModel.AccountNetwork,
                       DominatorAccountModel.AccountBaseModel.UserName, ActivityType.ToString(),
                       $"Successfully Skipped {skippedCount} Comments as Already Scrapped");
                if (response.post.ListComments.Count == 0)
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                       DominatorAccountModel.AccountBaseModel.AccountNetwork,
                       DominatorAccountModel.AccountBaseModel.UserName, ActivityType.ToString(),
                       $"Found 0 Comments for This Post.");
            }
            else
            {
                GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType.ToString(), tumblrPost.PostUrl);
                jobProcessResult.IsProcessSuceessfull = false;
            }
            return jobProcessResult;
        }

        /// <summary>
        ///     Add Followed User Data to DB
        /// </summary>
        /// <param name="scrapeResult"></param>
        /// <param name="username"></param>
        private void AddScrapedCommentsDataToDataBase(ScrapeResultNew scrapeResult, string username, TumblrComments comment)
        {
            try
            {
                var post = (TumblrPost)scrapeResult.ResultPost;
                if (post.PostType != null && post.PostType.Contains("photo")) post.PostType = "Image";
                if (!string.IsNullOrEmpty(CampaignId))
                {
                    // Add data to respected campaign InteractedUsers table
                    IDbCampaignService _campaignService = new DbCampaignService(CampaignId);
                    _campaignService.Add(new DominatorHouseCore.DatabaseHandler.TumblrTables.Campaign.InteractedPosts
                    {
                        AccountEmail = username,
                        ActivityType = ActivityType.ToString(),
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        ProfileUrl = post.ProfileUrl,
                        InteractedUserName = comment.commenter.Username,
                        ContentId = post.Id,
                        PostUrl = post.PostUrl,
                        Type = comment.Type,
                        CommentText = comment.CommentText,
                        ReblogText = comment.AddedText,
                        CommentId = comment.CommentId

                    });
                }

                // Add data to respected account friendship table
                DbAccountService.Add(new InteractedPosts
                {
                    AccountName = username,
                    ActivityType = ActivityType.ToString(),
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    ContentId = post.Id,
                    InteractedUserName = comment.commenter.Username,
                    ProfileUrl = post.ProfileUrl,
                    PostUrl = post.PostUrl,
                    Type = comment.Type,
                    CommentText = comment.CommentText,
                    ReblogText = comment.AddedText,
                    CommentId = comment.CommentId
                });
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ex.Message + " " + ex.StackTrace);
            }
        }
    }
}