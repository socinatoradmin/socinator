using DominatorHouseCore.DatabaseHandler.TumblrTables.Account;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class CommentProcess : TumblrJobProcessInteracted<InteractedPosts>
    {
        private readonly ITumblrFunct TumblrFunct;

        public CommentProcess(IProcessScopeModel processScopeModel, IDbAccountServiceScoped _accountService,
            IDbGlobalService _dbGlobalService, IExecutionLimitsManager executionLimitsManager,
            ITumblrFunct _tumblrFunct, ITumblrQueryScraperFactory queryScraperFactory, ITumblrHttpHelper _httpHelper,
            ITumblrLoginProcess _tumblrLoginProcess) : base(processScopeModel, _accountService, _dbGlobalService,
            executionLimitsManager, queryScraperFactory, _httpHelper,
            _tumblrLoginProcess)
        {
            TumblrFunct = _tumblrFunct;
            CommentModel = processScopeModel.GetActivitySettingsAs<CommentModel>();
        }

        public CommentModel CommentModel { get; set; }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResultNew)
        {
            var scrapeResult = (TumblrScrapeResult)scrapeResultNew;
            var tumblrPost = (TumblrPost)scrapeResult.ResultPost;
            CommentPostResponse response = null;
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            var isCommented = false;
            var jobProcessResult = new JobProcessResult();
            try
            {
                var msgList = new List<string>();
                CommentModel.LstDisplayManageCommentModel.ForEach(y =>
                {
                    var temp = y.SelectedQuery.Count;
                    for (var i = 0; i < temp; i++)
                        if (y.SelectedQuery[i].Content.QueryValue == scrapeResult.QueryInfo.QueryValue)
                            msgList.Add(y.CommentText);
                });
                if (msgList.Count == 0)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName
                        , "Please mention query type with the comment");
                    return null;
                }

                var msg = msgList.GetRandomItem(); //palaniru
                if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                    response = TumblrFunct.CommentPost(DominatorAccountModel, tumblrPost, scrapeResult.TumblrFormKey,
                        msg);
                else
                    isCommented = _browserManager.Comment(DominatorAccountModel, msg, ref tumblrPost);
                if (response != null && response.Success || isCommented)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType.ToString(),
                        "On Post => " + tumblrPost.PostUrl);
                    IncrementCounters();
                    scrapeResult.ResultPost = tumblrPost;
                    AddFollowedDataToDataBase(scrapeResult, DominatorAccountModel.AccountBaseModel.UserName, msg);
                    var AccountModel = new AccountModel(DominatorAccountModel);
                    if (AccountModel.LstPost == null)
                        AccountModel.LstPost = new List<TumblrPost>();
                    AccountModel.LstPost.Add(tumblrPost);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else if ((DominatorAccountModel.IsRunProcessThroughBrowser && !tumblrPost.CanReply && !isCommented)
                    || (!DominatorAccountModel.IsRunProcessThroughBrowser && !tumblrPost.CanReply &&
                    response.commentResponse.Response.Contains("403")))
                {
                    GlobusLogHelper.log.Info("Cannot Access to Comment on Post => " + tumblrPost.PostUrl +
                                             " with account => " + DominatorAccountModel.AccountBaseModel.UserName +
                                             " module => " + ActivityType);
                    jobProcessResult.IsProcessSuceessfull = false;
                }
                else
                {
                    GlobusLogHelper.log.Info("Unable to Comment on Post => " + tumblrPost.PostUrl +
                                             " with account => " + DominatorAccountModel.AccountBaseModel.UserName +
                                             " module => " + ActivityType);
                    jobProcessResult.IsProcessSuceessfull = false;
                }
                if (CommentModel.EnableDelayBetweenPerformingActionOnSamePost)
                    DelayBeforeNextActivity(CommentModel.DelayBetweenPerformingActionOnSamePost.GetRandom());
                else DelayBeforeNextActivity();
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception)
            {
                return null;
            }

            return jobProcessResult;
        }

        private void AddFollowedDataToDataBase(ScrapeResultNew scrapeResult, string username, string message)
        {
            try
            {
                var instaUser = (TumblrPost)scrapeResult.ResultPost;
                if (instaUser.PostType.Contains("photo")) instaUser.PostType = "Image";

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
                        PostUrl = instaUser.PostUrl,
                        PostDescription = scrapeResult.ResultPost.Caption,
                        Comments = message
                    });
                }

                //var dbAccountOperation = new DbOperations(AccountId, SocialNetworks.Tumblr, ConstantVariable.GetAccountDb);
                // Add data to respected account friendship table
                DbAccountService.Add(new InteractedPosts
                {
                    ActivityType = ActivityType.ToString(),
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    ContentId = instaUser.Id,
                    PostUrl = instaUser.PostUrl,
                    PostTitle = scrapeResult.ResultPost.Caption,
                    PostDescription = scrapeResult.ResultPost.Caption,
                    Comments = message
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