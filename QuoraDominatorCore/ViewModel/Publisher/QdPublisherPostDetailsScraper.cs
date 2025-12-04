using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QdLibrary;
using QuoraDominatorCore.Response;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace QuoraDominatorCore.ViewModel.Publisher
{
    internal class QdPublisherPostDetailsScraper : PostScraper
    {
        private readonly IQdLogInProcess logInProcess;
        private readonly IQuoraFunctions quoraFunctions; 
        public QdPublisherPostDetailsScraper(string CampaignId
            , CancellationTokenSource campaignCancellationToken, PublisherPostFetchModel postFetchModel) :
            base(CampaignId, campaignCancellationToken, postFetchModel)
        {
            logInProcess = InstanceProvider.GetInstance<IQdLogInProcess>();
            quoraFunctions=InstanceProvider.GetInstance<IQuoraFunctions>();
        }
        public override void ScrapeFdKeywords(string accountId, string campaignId, SharePostModel QdKeywordScraperDetails, CancellationTokenSource cancellationTokenSource, int count = 10)
        {
            cancellationTokenSource.Token.ThrowIfCancellationRequested();
            var totalCount = 0;
            if (string.IsNullOrEmpty(accountId) || string.IsNullOrEmpty(campaignId) || QdKeywordScraperDetails == null)
                return;

            var accountFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            var dominatorAccountModel = accountFileManager.GetAccountById(accountId);

            cancellationTokenSource.Token.ThrowIfCancellationRequested();
            if (!LoginForScrapePost(dominatorAccountModel, campaignId, cancellationTokenSource).Result)
                return;

            var postSource = Regex.Split(QdKeywordScraperDetails.AddKeywords, "\r\n");
            postSource.Shuffle();
            foreach (var item in postSource)
            {
                cancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (Utilities.GetBetween(item, "[", "]") == "K" || !string.IsNullOrEmpty(item))
                    ScrapeQdKeywordPost(dominatorAccountModel, campaignId, ref totalCount, item, QdKeywordScraperDetails,
                        cancellationTokenSource, count);
            }
        }

        private async Task<bool> LoginForScrapePost(DominatorAccountModel dominatorAccountModel, string campaignId, CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                return await logInProcess.CheckLoginAsync(dominatorAccountModel, dominatorAccountModel.Token, true);
            }catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }

        private void ScrapeQdKeywordPost(DominatorAccountModel dominatorAccountModel, string campaignId, ref int totalCount, string item, SharePostModel qdKeywordScraperDetails, CancellationTokenSource cancellationTokenSource, int count)
        {
            try
            {
                ScrapePostResponseHandler KeywordPostResponse = null;
                cancellationTokenSource.Token.ThrowIfCancellationRequested();
                var lstQuoraPost = new List<PostDetails>();
                var queryInfo = new QueryInfo { QueryType = "Keyword", QueryValue = item.Replace("[K]", "").TrimStart() };
                while (totalCount < count)
                {
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    KeywordPostResponse = quoraFunctions.ScrapeKeyWordPost(dominatorAccountModel, queryInfo.QueryValue, KeywordPostResponse == null ? -1 : KeywordPostResponse.PaginationCount).Result;
                    lstQuoraPost.AddRange(KeywordPostResponse.PostCollection);
                    FilterDuplicatePost(lstQuoraPost, dominatorAccountModel);
                    foreach (var Post in lstQuoraPost)
                    {
                        cancellationTokenSource.Token.ThrowIfCancellationRequested();
                        if (totalCount >= count)
                            break;
                        SaveScrapeQuoraPost(campaignId, Post);
                        totalCount++;
                        _publisherInitialize.UpdatePostCounts(campaignId);
                    }
                    if (KeywordPostResponse is null || !KeywordPostResponse.HasMoreResult)
                        break;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void SaveScrapeQuoraPost(string campaignId, PostDetails redditPost)
        {
            try
            {
                var publisherPostlistModel = new PublisherPostlistModel();
                publisherPostlistModel.GenerateClonePostId();
                publisherPostlistModel.CampaignId = campaignId;
                publisherPostlistModel.ExpiredTime = DateTime.Now.AddYears(2);
                publisherPostlistModel.PostSource = PostSource.ScrapedPost;
                publisherPostlistModel.PostQueuedStatus = PostQueuedStatus.Pending;
                publisherPostlistModel.PublisherInstagramTitle = redditPost.PostTitle;
                publisherPostlistModel.FetchedPostIdOrUrl = redditPost.PostId;
                publisherPostlistModel.CreatedTime = redditPost.Created;
                publisherPostlistModel.ShareUrl = redditPost.PostUrl;
                PostlistFileManager.Add(campaignId, publisherPostlistModel);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void FilterDuplicatePost(List<PostDetails> lstQuoraPost, DominatorAccountModel dominatorAccountModel)
        {
            var SkippedPostCount = 0;
            if ((SkippedPostCount = lstQuoraPost.RemoveAll(x => CheckForDuplicatePost(x.PostId))) > 0)
                GlobusLogHelper.log.Info(Log.CustomMessage,
                            dominatorAccountModel.AccountBaseModel.AccountNetwork, dominatorAccountModel.AccountBaseModel.UserName, "Scrape Post",
                            $"Skipped {SkippedPostCount} Already Interacted Post.");
        }

        public override void ScrapeFdPagePostUrl(string accountId, string campaignId,
            SharePostModel fdPagePostUrlScraperDetails, CancellationTokenSource cancellationTokenSource, int count = 10)
        {
            if (string.IsNullOrEmpty(accountId) || string.IsNullOrEmpty(campaignId) ||
                fdPagePostUrlScraperDetails == null)
                return;

            base.ScrapeFdPagePostUrl(accountId, campaignId, fdPagePostUrlScraperDetails, cancellationTokenSource,
                count);
        }

        public override void ScrapePosts(string accountId, string campaignId, ScrapePostModel scrapePostDetails,
            CancellationTokenSource cancellationTokenSource, int count = 10)
        {
            if (string.IsNullOrEmpty(accountId) || string.IsNullOrEmpty(campaignId) || scrapePostDetails == null)
                return;

            base.ScrapePosts(accountId, campaignId, scrapePostDetails, cancellationTokenSource, count);

            PostlistFileManager.Add(campaignId, new PublisherPostlistModel());
        }
    }
}