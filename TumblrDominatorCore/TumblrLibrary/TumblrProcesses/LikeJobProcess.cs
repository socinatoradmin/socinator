using DominatorHouseCore.DatabaseHandler.TumblrTables.Account;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TumblrFactory;
using TumblrDominatorCore.TumblrLibrary.DAL;
using TumblrDominatorCore.TumblrLibrary.TumblrFunction;
using TumblrDominatorCore.TumblrRequest;
using TumblrDominatorCore.TumblrResponseHandler;

namespace TumblrDominatorCore.TumblrLibrary.TumblrProcesses
{
    public class LikeJobProcess : TumblrJobProcessInteracted<InteractedPosts>
    {
        private readonly ITumblrFunct TumblrFunct;

        public LikeJobProcess(IProcessScopeModel processScopeModel, IDbAccountServiceScoped _accountService,
            IDbGlobalService _dbGlobalService, IExecutionLimitsManager executionLimitsManager,
            ITumblrFunct _tumblrFunct, ITumblrQueryScraperFactory queryScraperFactory, ITumblrHttpHelper _httpHelper,
            ITumblrLoginProcess _tumblrLoginProcess) :
            base(processScopeModel, _accountService, _dbGlobalService, executionLimitsManager, queryScraperFactory,
                _httpHelper, _tumblrLoginProcess)
        {
            TumblrFunct = _tumblrFunct;
            LikeModel = processScopeModel.GetActivitySettingsAs<LikeModel>();
        }

        public LikeModel LikeModel { get; set; }


        /// <summary>
        ///     LIKE POST: to like Post of Tumblr Blog's
        /// </summary>
        /// <param name="scrapeResultNew"></param>
        /// <returns></returns>
        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResultNew)
        {
            var scrapeResult = (TumblrScrapeResult)scrapeResultNew;
            var tumblrPost = (TumblrPost)scrapeResult.ResultPost;
            var jobProcessResult = new JobProcessResult();
            try
            {
                LikePostResponse response = null;
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, tumblrPost?.PostUrl);
                var isLiked = false;
                if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                    response = TumblrFunct.LikePost(DominatorAccountModel, tumblrPost, scrapeResult.TumblrFormKey);
                else
                    response = _browserManager.LikePost(DominatorAccountModel, tumblrPost);
                if (response != null && response.Success || isLiked)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType.ToString(),
                        " Post => " + tumblrPost.PostUrl);
                    jobProcessResult.IsProcessCompleted = true;
                    IncrementCounters();
                    scrapeResult.ResultPost = tumblrPost;
                    AddFollowedDataToDataBase(scrapeResult, DominatorAccountModel.AccountBaseModel.UserName);
                    var AccountModel = new AccountModel(DominatorAccountModel);
                    if (AccountModel.LstPost == null)
                        AccountModel.LstPost = new List<TumblrPost>();
                    AccountModel.LstPost.Add(tumblrPost);

                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else if (DominatorAccountModel.IsRunProcessThroughBrowser && tumblrPost.IsLiked && !isLiked)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        " Can Not Like this post => " + tumblrPost.PostUrl + "As Already Liked ");
                    jobProcessResult.IsProcessSuceessfull = false;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType.ToString(), tumblrPost.PostUrl);
                    jobProcessResult.IsProcessSuceessfull = false;
                }
            }
            catch (Exception)
            {
            }
            if (LikeModel.EnableDelayBetweenPerformingActionOnSamePost)
                DelayBeforeNextActivity(LikeModel.DelayBetweenPerformingActionOnSamePost.GetRandom());
            else DelayBeforeNextActivity();
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            return jobProcessResult;
        }

        /// <summary>
        ///     Add Followed User Data to DB
        /// </summary>
        /// <param name="scrapeResult"></param>
        /// <param name="username"></param>
        private void AddFollowedDataToDataBase(ScrapeResultNew scrapeResult, string username)
        {
            try
            {
                var instaUser = (TumblrPost)scrapeResult.ResultPost;

                if (instaUser.PostType != null && instaUser.PostType.Contains("photo"))
                    instaUser.MediaType = MediaType.Image;
                else if (instaUser.PostType != null && instaUser.PostType.Contains("video")) instaUser.MediaType = MediaType.Video;
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
                        ContentId = instaUser.Id,
                        MediaType = instaUser.MediaType,
                        PostUrl = instaUser.PostUrl,
                        DataRootId = instaUser.Uuid,
                        DataKey = instaUser.RebloggedRootId
                    });
                }

                // Add data to respected account friendship table
                DbAccountService.Add(new InteractedPosts
                {
                    ActivityType = ActivityType.ToString(),
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    InteractedUserName = instaUser.OwnerUsername,
                    ContentId = instaUser.Id,
                    MediaType = instaUser.MediaType,
                    PostUrl = instaUser.PostUrl,
                    DataRootId = instaUser.Uuid,
                    DataKey = instaUser.RebloggedRootId
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