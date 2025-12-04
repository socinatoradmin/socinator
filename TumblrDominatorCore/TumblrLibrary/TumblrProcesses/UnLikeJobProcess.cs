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
using UnLikedPosts = DominatorHouseCore.DatabaseHandler.TumblrTables.Campaign.UnLikedPosts;

namespace TumblrDominatorCore.TumblrLibrary.TumblrProcesses
{
    public class UnLikeJobProcess : TumblrJobProcessInteracted<InteractedPosts>
    {
        private readonly SocialNetworks _networks;
        private readonly ITumblrFunct TumblrFunct;

        public UnLikeJobProcess(IProcessScopeModel processScopeModel, IDbAccountServiceScoped _accountService,
            IDbGlobalService _dbGlobalService, IExecutionLimitsManager executionLimitsManager,
            ITumblrFunct _tumblrFunct, ITumblrQueryScraperFactory queryScraperFactory, ITumblrHttpHelper _httpHelper,
            ITumblrLoginProcess _tumblrLoginProcess) :
            base(processScopeModel, _accountService, _dbGlobalService, executionLimitsManager, queryScraperFactory,
                _httpHelper, _tumblrLoginProcess)
        {
            TumblrFunct = _tumblrFunct;
            _networks = SocialNetworks.Tumblr;
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
            UnLikePostResponse response = null;
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                response = TumblrFunct.UnLikePost(DominatorAccountModel, tumblrPost, scrapeResult.TumblrFormKey);
            else
                response = _browserManager.UnLikePost(DominatorAccountModel, tumblrPost);
            if (response != null && response.Success)
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType.ToString(),
                    " Post => " + tumblrPost.PostUrl);

                IncrementCounters();
                scrapeResult.ResultPost = tumblrPost;
                StartOtherConfiguration(scrapeResult);
                AddFollowedDataToDataBase(scrapeResult, DominatorAccountModel.AccountBaseModel.UserName);
                var AccountModel = new AccountModel(DominatorAccountModel);
                if (AccountModel.LstPost == null)
                    AccountModel.LstPost = new List<TumblrPost>();
                AccountModel.LstPost.Add(tumblrPost);

                jobProcessResult.IsProcessSuceessfull = true;
            }
            else
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType.ToString(), tumblrPost.PostUrl);
                jobProcessResult.IsProcessSuceessfull = false;
            }
            if (!LikeModel.EnableDelayBetweenPerformingActionOnSamePost)
                DelayBeforeNextActivity();
            if (LikeModel.EnableDelayBetweenPerformingActionOnSamePost)
                DelayBeforeNextActivity(LikeModel.DelayBetweenPerformingActionOnSamePost.GetRandom());
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

                if (!string.IsNullOrEmpty(CampaignId))
                {
                    // Add data to respected campaign InteractedUsers table
                    IDbCampaignService _campaignService = new DbCampaignService(CampaignId);
                    _campaignService.Add(new UnLikedPosts
                    {
                        UserName = username,
                        ActivityType = ActivityType.ToString(),
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        PostUrl = instaUser.PostUrl
                    });
                }

                // Add data to respected account friendship table
                DbAccountService.Add(new DominatorHouseCore.DatabaseHandler.TumblrTables.Account.UnLikedPosts
                {
                    UserName = username,
                    ActivityType = ActivityType.ToString(),
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    PostUrl = instaUser.PostUrl
                });
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ex.Message + " " + ex.StackTrace);
            }

            //  MediaType = (MediaType)Enum.Parse(typeof(MediaType), instaUser.PostType),
        }
    }
}