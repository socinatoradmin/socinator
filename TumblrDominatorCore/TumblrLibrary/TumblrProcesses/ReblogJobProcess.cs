using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.TumblrTables.Account;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.TumblrFactory;
using TumblrDominatorCore.TumblrLibrary.DAL;
using TumblrDominatorCore.TumblrLibrary.TumblrFunction;
using TumblrDominatorCore.TumblrRequest;
using TumblrDominatorCore.TumblrResponseHandler;

namespace TumblrDominatorCore.TumblrLibrary.TumblrProcesses
{
    public class ReblogJobProcess : TumblrJobProcessInteracted<InteractedPosts>
    {
        private readonly ITumblrFunct TumblrFunct;

        public ReblogJobProcess(IProcessScopeModel processScopeModel, IDbAccountServiceScoped _accountService,
            IDbGlobalService _dbGlobalService, IExecutionLimitsManager executionLimitsManager,
            ITumblrFunct _tumblrFunct, ITumblrQueryScraperFactory queryScraperFactory, ITumblrHttpHelper _httpHelper,
            ITumblrLoginProcess _tumblrLoginProcess) : base(processScopeModel, _accountService, _dbGlobalService,
            executionLimitsManager, queryScraperFactory, _httpHelper,
            _tumblrLoginProcess)
        {
            TumblrFunct = _tumblrFunct;
            ReblogModel = processScopeModel.GetActivitySettingsAs<ReblogModel>();
            CheckJobProcessLimitsReached();
        }

        public ReblogModel ReblogModel { get; set; }

        /// <summary>
        ///     REBLOG POSTS
        /// </summary>
        /// <param name="scrapeResultNew"></param>
        /// <returns></returns>
        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResultNew)
        {
            var scrapeResult = (TumblrScrapeResult)scrapeResultNew;
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            var jobProcessResult = new JobProcessResult();
            ReblogPostResponse response = null;

            var tumblrPost = (TumblrPost)scrapeResult.ResultPost;
            if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                response = TumblrFunct.ReblogPost(DominatorAccountModel, tumblrPost, scrapeResult.TumblrFormKey);
            else
                response = _browserManager.ReblogPost(DominatorAccountModel, tumblrPost);
            if (response != null && response.Success)
            {
                tumblrPost.ReblogUrl = ConstantHelpDetails.GetPostUrlByUserNameAndPostId(DominatorAccountModel.AccountBaseModel.UserId, response.rebloggedPostId);
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    $"{tumblrPost.PostUrl} Reblogged As ==> {tumblrPost.ReblogUrl}");
                IncrementCounters();
                scrapeResult.ResultPost = tumblrPost;
                AddFollowedDataToDataBase(scrapeResult, DominatorAccountModel.AccountBaseModel.UserName);
                var AccountModel = new AccountModel(DominatorAccountModel);
                if (AccountModel.LstPost == null)
                    AccountModel.LstPost = new List<TumblrPost>();
                AccountModel.LstPost.Add(tumblrPost);

                jobProcessResult.IsProcessSuceessfull = true;
            }
            else if (response != null && !response.Success && !tumblrPost.CanReblog)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                   DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                   " User Can not Reblog => " + tumblrPost.PostUrl);
                jobProcessResult.IsProcessSuceessfull = false;
            }
            else
            {
                GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, tumblrPost.PostUrl);
                jobProcessResult.IsProcessSuceessfull = false;
            }
            if (!ReblogModel.EnableDelayBetweenPerformingActionOnSamePost)
                DelayBeforeNextActivity();
            if (ReblogModel.EnableDelayBetweenPerformingActionOnSamePost)
                DelayBeforeNextActivity(ReblogModel.DelayBetweenPerformingActionOnSamePost.GetRandom());


            return jobProcessResult;
        }

        /// <summary>
        ///     Saving data Into DataBase
        /// </summary>
        /// <param name="scrapeResult"></param>
        /// <param name="username"></param>
        private void AddFollowedDataToDataBase(ScrapeResultNew scrapeResult, string username)
        {
            try
            {
                var instaUser = (TumblrPost)scrapeResult.ResultPost;

                if (instaUser.PostType.Contains("photo")) instaUser.PostType = "Image";

                if (!string.IsNullOrEmpty(CampaignId))
                {
                    IDbCampaignService _campaignService = new DbCampaignService(CampaignId);
                    _campaignService.Add(new DominatorHouseCore.DatabaseHandler.TumblrTables.Campaign.InteractedPosts
                    {
                        AccountEmail = username,
                        ActivityType = ActivityType.ToString(),
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        ContentId = instaUser.Id,
                        PostUrl = instaUser.PostUrl,
                        ReblogUrl = instaUser.ReblogUrl
                    });
                }

                DbAccountService.Add(new InteractedPosts
                {
                    ActivityType = ActivityType.ToString(),
                    Comments = DominatorAccountModel.AccountBaseModel.UserId,
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    InteractedUserName = instaUser.OwnerUsername,
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    ContentId = instaUser.Id,
                    PostUrl = instaUser.PostUrl,
                    ReblogUrl = instaUser.ReblogUrl
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}