using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using GramDominatorCore.Factories;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDModel;
using GramDominatorCore.Request;
using GramDominatorCore.Response;
using System;

namespace GramDominatorCore.GDLibrary
{
    public class LikeCommentProcess : GdJobProcessInteracted<InteractedPosts>
    {
        public LikeCommentProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IGdQueryScraperFactory queryScraperFactory,
            IGdHttpHelper httpHelper, IGdBrowserManager gdBrowser, IDelayService _delayService)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, httpHelper, gdBrowser, _delayService)
        {
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            var commentPost = $"https://www.instagram.com/p/{scrapeResult?.ResultPost?.Code}/c/{scrapeResult?.ResultPostComment?.CommentId}/";
            GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, commentPost);
            JobProcessResult jobProcessResult = new JobProcessResult();
            try
            {
                var response = instaFunct.LikeOnComment(DominatorAccountModel, new AccountModel(DominatorAccountModel), JobCancellationTokenSource.Token, scrapeResult?.ResultPost?.Code, scrapeResult?.ResultPostComment?.CommentId).Result;
                var visited = instaFunct.GdBrowserManager.VisitPage(DominatorAccountModel, commentPost).Result;
                if (response != null && response.Success)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        $"{scrapeResult.ResultPostComment.Text} ==> {commentPost}");

                    IncrementCounters();

                    AddLikedDataToDataBase(scrapeResult);

                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else if (response.ToString().Contains("login_required") && response != null && response.Issue != null &&
                         response.Issue.Status == "Response Response Login session expired")
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        $"Log in required please update your account");
                    Stop();
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultPost.Code,
                        scrapeResult.ResultPostComment.Text);
                    jobProcessResult.IsProcessSuceessfull = false;
                }

                DelayBeforeNextActivity();

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return jobProcessResult;
        }

        private void AddLikedDataToDataBase(ScrapeResultNew scrapeResult)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            ResultCommentItemUser resultCommentItemUser = (ResultCommentItemUser) scrapeResult.ResultPostComment;
            InstagramPost instagramPost = (InstagramPost) scrapeResult.ResultPost;
            // Add data to respected campaign InteractedPosts table
            if (!string.IsNullOrEmpty(CampaignId))
            {
                //   var dboperationCampaign = new DbOperations(CampaignId, SocialNetworks.Instagram, ConstantVariable.GetCampaignDb);

                CampaignDbOperation?.Add(new DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts()
                {
                    InteractionDate = DateTimeUtilities.GetEpochTime(),
                    MediaType = instagramPost.MediaType,
                    ActivityType = ActivityType.LikeComment,
                    PkOwner = instagramPost.Code,
                    UsernameOwner = instagramPost.User?.Username,
                    Username = DominatorAccountModel.AccountBaseModel.UserName,
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    CommentId = resultCommentItemUser.CommentId,
                    Comment = resultCommentItemUser.Text,
                    Status = "Success"
                });
            }

            AccountDbOperation.Add(
                new DominatorHouseCore.DatabaseHandler.GdTables.Accounts.InteractedPosts()
                {
                    InteractionDate = DateTimeUtilities.GetEpochTime(),
                    MediaType = instagramPost.MediaType,
                    ActivityType = ActivityType.LikeComment,
                    PkOwner = instagramPost.Code,
                    UsernameOwner = instagramPost.User?.Username,
                    Username = DominatorAccountModel.AccountBaseModel.UserName,
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    CommentId = resultCommentItemUser.CommentId,
                    Comment = resultCommentItemUser.Text,
                    Status = "Success"
                });
        }
    }
}
