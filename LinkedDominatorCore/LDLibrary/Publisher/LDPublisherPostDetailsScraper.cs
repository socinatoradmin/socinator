using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.Response;

namespace LinkedDominatorCore.LDLibrary.Publisher
{
    public class LDPublisherPostDetailsScraper : PostScraper
    {
        private readonly IAccountsFileManager accountsFileManager;
        private readonly ILdLogInProcess logInProcess;
        private readonly ILdFunctions ldFunctions;
        public LDPublisherPostDetailsScraper(string CampaignId, CancellationTokenSource campaignCancellationToken,
            PublisherPostFetchModel postFetchModel) :
            base(CampaignId, campaignCancellationToken, postFetchModel)
        {
            accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            logInProcess = InstanceProvider.GetInstance<ILdLogInProcess>();
            ldFunctions = InstanceProvider.GetInstance<ILdFunctions>();
        }
        public override void ScrapeFdKeywords(string accountId, string campaignId, SharePostModel LdKeywordScraperDetails, CancellationTokenSource cancellationTokenSource, int count = 10)
        {
            cancellationTokenSource.Token.ThrowIfCancellationRequested();
            var totalCount = 0;
            if (string.IsNullOrEmpty(accountId) || string.IsNullOrEmpty(campaignId) || LdKeywordScraperDetails == null)
                return;
            var dominatorAccountModel = accountsFileManager.GetAccountById(accountId);
            cancellationTokenSource.Token.ThrowIfCancellationRequested();
            if (!LoginForScrapePost(dominatorAccountModel, campaignId, cancellationTokenSource).Result)
                return;
            var postSource = Regex.Split(LdKeywordScraperDetails.AddKeywords, "\r\n");
            postSource.Shuffle();
            foreach (var item in postSource)
            {
                cancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (Utilities.GetBetween(item, "[", "]") == "K" || !string.IsNullOrEmpty(item))
                    ScrapeLDKeywordPost(dominatorAccountModel, campaignId, ref totalCount, item, LdKeywordScraperDetails,
                        cancellationTokenSource, count);
            }
        }

        private async Task<bool> LoginForScrapePost(DominatorAccountModel dominatorAccountModel, string campaignId, CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                return await logInProcess.CheckLoginAsync(dominatorAccountModel, cancellationTokenSource.Token, true);
            }catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }

        private void ScrapeLDKeywordPost(DominatorAccountModel dominatorAccountModel, string campaignId, ref int totalCount, string queryValue, SharePostModel LdKeywordScraperDetails, CancellationTokenSource cancellationTokenSource, int count)
        {
            try
            {
                ScrapePostResponseHandler postResponseHandler= null;
                cancellationTokenSource.Token.ThrowIfCancellationRequested();
                var postCollection = new List<LinkedinPost>();
                while (totalCount < count)
                {
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    postResponseHandler = ldFunctions.ScrapeKeywordPost(dominatorAccountModel, queryValue, postResponseHandler == null ? 0 : postResponseHandler.PaginationCount).Result;
                    postCollection = postResponseHandler != null && postResponseHandler.PostCollection.Count > 0?postResponseHandler.PostCollection:postCollection; 
                    FilterDuplicatePost(postCollection, dominatorAccountModel);
                    foreach (var Post in postCollection)
                    {
                        cancellationTokenSource.Token.ThrowIfCancellationRequested();
                        if (totalCount >= count)
                            break;
                        SaveScrapeLDPost(campaignId, Post);
                        totalCount++;
                        _publisherInitialize.UpdatePostCounts(campaignId);
                    }
                    if (postResponseHandler is null || !postResponseHandler.HasMoreResults)
                        break;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void SaveScrapeLDPost(string campaignId, LinkedinPost post)
        {
            try
            {
                var publisherPostlistModel = new PublisherPostlistModel();
                publisherPostlistModel.GenerateClonePostId();
                publisherPostlistModel.CampaignId = campaignId;
                publisherPostlistModel.ExpiredTime = DateTime.Now.AddYears(2);
                publisherPostlistModel.PostSource = PostSource.ScrapedPost;
                publisherPostlistModel.PostQueuedStatus = PostQueuedStatus.Pending;
                publisherPostlistModel.PublisherInstagramTitle = post.PostTitle;
                publisherPostlistModel.FetchedPostIdOrUrl = post.Id;
                publisherPostlistModel.ShareUrl = post.PostLink;
                PostlistFileManager.Add(campaignId, publisherPostlistModel);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void FilterDuplicatePost(List<LinkedinPost> postCollection, DominatorAccountModel dominatorAccountModel)
        {
            var SkippedPostCount = 0;
            if ((SkippedPostCount = postCollection.RemoveAll(x => CheckForDuplicatePost(x.Id))) > 0)
                GlobusLogHelper.log.Info(Log.CustomMessage,
                            dominatorAccountModel.AccountBaseModel.AccountNetwork, dominatorAccountModel.AccountBaseModel.UserName, "Scrape Post",
                            $"Skipped {SkippedPostCount} Already Interacted Post.");
        }
    }
}