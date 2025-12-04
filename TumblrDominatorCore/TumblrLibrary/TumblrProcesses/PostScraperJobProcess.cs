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
    public class PostScraperJobProcess : TumblrJobProcessInteracted<InteractedPosts>
    {
        private readonly SocialNetworks _networks;
        private readonly ITumblrFunct TumblrFunct;
        public PostFilterModel PostFilterModel = new PostFilterModel();

        public PostScraperJobProcess(IProcessScopeModel processScopeModel, IDbAccountServiceScoped _accountService,
            IDbGlobalService _dbGlobalService, IExecutionLimitsManager executionLimitsManager,
            ITumblrFunct _tumblrFunct, ITumblrQueryScraperFactory queryScraperFactory, ITumblrHttpHelper _httpHelper,
            ITumblrLoginProcess _tumblrLoginProcess) :
            base(processScopeModel, _accountService, _dbGlobalService, executionLimitsManager, queryScraperFactory,
                _httpHelper, _tumblrLoginProcess)
        {
            TumblrFunct = _tumblrFunct;
            _networks = SocialNetworks.Tumblr;
            PostScraperModel = processScopeModel.GetActivitySettingsAs<PostScraperModel>();
        }

        public PostScraperModel PostScraperModel { get; set; }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResultNew)
        {
            var scrapeResult = (TumblrScrapeResult)scrapeResultNew;
            var tumblrPost = (TumblrPost)scrapeResult.ResultPost;
            PostScraperResponse response = null;
            var status = "";
            var isPostDetail = false;
            var jobProcessResult = new JobProcessResult();
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType.ToString(), $"Trying To Get Details Of ==> {tumblrPost?.PostUrl}");
            if (!DominatorAccountModel.IsRunProcessThroughBrowser)
            {
                response = TumblrFunct.PostScraper(DominatorAccountModel, tumblrPost, scrapeResult.TumblrFormKey);
            }
            else
            {
                status = _browserManager.SearchPostDetails(DominatorAccountModel, ref tumblrPost);
                isPostDetail = !string.IsNullOrEmpty(status) && status.Contains("true") ? true : false;
            }

            if (response != null && response.Success || isPostDetail)
            {
                GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType.ToString(),
                    " Post => " + tumblrPost.PostUrl);
                jobProcessResult.IsProcessCompleted = true;
                scrapeResult.ResultPost = tumblrPost;
                IncrementCounters();

                AddFollowedDataToDataBase(scrapeResult, DominatorAccountModel.AccountBaseModel.UserName);
                var AccountModel = new AccountModel(DominatorAccountModel);
                if (AccountModel.LstPost == null)
                    AccountModel.LstPost = new List<TumblrPost>();
                AccountModel.LstPost.Add(tumblrPost);
                jobProcessResult.IsProcessSuceessfull = true;
            }
            else if (!isPostDetail && !string.IsNullOrEmpty(status))
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType.ToString(), "Successfully Skipped As : " + status);
                jobProcessResult.IsProcessSuceessfull = false;
            }
            else
            {
                GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType.ToString(), tumblrPost.PostUrl);
                jobProcessResult.IsProcessSuceessfull = false;
            }

            DelayBeforeNextActivity();
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
                var post = (TumblrPost)scrapeResult.ResultPost;


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
                        InteractedUserName = post.OwnerUsername,
                        ContentId = post.Id,
                        ProfileUrl = post.ProfileUrl,
                        PostUrl = post.PostUrl,
                        NotesCount = post.NotesCount,
                        LikeCount = post.LikeCount,
                        ReblogCount = post.ReblogCount
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
                    InteractedUserName = post.OwnerUsername,
                    ProfileUrl = post.ProfileUrl,
                    PostUrl = post.PostUrl,
                    NotesCount = post.NotesCount,
                    LikeCount = post.LikeCount,
                    ReblogCount = post.ReblogCount
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