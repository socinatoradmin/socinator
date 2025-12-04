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
using System;

namespace GramDominatorCore.GDLibrary
{
    public class CommentScraperProcess : GdJobProcessInteracted<InteractedPosts>
    {
        public CommentScraperProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IGdQueryScraperFactory queryScraperFactory, IGdHttpHelper httpHelper, IGdBrowserManager gdBrowser, IDelayService _delayService) : 
            base(processScopeModel, accountServiceScoped, queryScraperFactory, httpHelper, gdBrowser,_delayService)
        {
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultPost.Code);
            JobProcessResult jobProcessResult = new JobProcessResult();
            try
            {
                if (scrapeResult.ResultPostComment != null)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultPostComment.Text);

                    IncrementCounters();

                    AddScrapedUserDataIntoDataBase(scrapeResult);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                    jobProcessResult.IsProcessSuceessfull = false;
                
                DelayBeforeNextActivity();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return jobProcessResult;
        }

        private void AddScrapedUserDataIntoDataBase(ScrapeResultNew scrapeResult)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            InstagramPost instagramPost = (InstagramPost)scrapeResult.ResultPost;
            ResultCommentItemUser resultCommentItemUser = (ResultCommentItemUser)scrapeResult.ResultPostComment;
            // Add data to respected campaign InteractedUsers table
            if (!string.IsNullOrEmpty(CampaignId))
            {
                CampaignDbOperation?.Add(new DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts()
                {
                    TakenAt =resultCommentItemUser.CreatedAt,
                    MediaType = instagramPost.MediaType,
                    ActivityType = ActivityType.CommentScraper,
                    PkOwner = instagramPost.Code,
                    UsernameOwner = instagramPost.User?.Username,
                    Username = DominatorAccountModel.AccountBaseModel.UserName,
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    CommentId = resultCommentItemUser.CommentId,
                    Comment = resultCommentItemUser.Text,
                    CommentOwnerName= resultCommentItemUser.ItemUser.Username,
                    CommentOwnerId = resultCommentItemUser.ItemUser.Pk,
                    Status = "Scraped",
                    InteractionDate = DateTimeUtilities.GetEpochTime()
                });
            }

            // Add data to respected Account InteractedUsers table          
            AccountDbOperation.Add(
                     new InteractedPosts()
                     {
                         TakenAt = resultCommentItemUser.CreatedAt,
                         MediaType = instagramPost.MediaType,
                         ActivityType = ActivityType.CommentScraper,
                         PkOwner = instagramPost.Code,
                         UsernameOwner = instagramPost.User?.Username,
                         Username = DominatorAccountModel.AccountBaseModel.UserName,
                         QueryType = scrapeResult.QueryInfo.QueryType,
                         QueryValue = scrapeResult.QueryInfo.QueryValue,
                         CommentId = resultCommentItemUser.CommentId,
                         Comment = resultCommentItemUser.Text,
                         CommentOwnerName = resultCommentItemUser.ItemUser.Username,
                         CommentOwnerId = resultCommentItemUser.ItemUser.Pk,
                         Status = "Scraped",
                         InteractionDate = DateTimeUtilities.GetEpochTime()
                     });
        }
    }
}
