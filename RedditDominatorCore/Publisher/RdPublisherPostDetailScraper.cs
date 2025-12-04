using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using RedditDominatorCore.RDLibrary;
using RedditDominatorCore.RDLibrary.BrowserManager;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.Response;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using Unity;

namespace RedditDominatorCore.Publisher
{
    public class RdPublisherPostDetailScraper : PostScraper
    {
        private IRdBrowserManager _browserManager;
        private IRedditFunction _redditFunct;

        public RdPublisherPostDetailScraper(string CampaignId
            , CancellationTokenSource campaignCancellationToken, PublisherPostFetchModel postFetchModel) :
            base(CampaignId, campaignCancellationToken, postFetchModel)
        {
        }
        public override void ScrapeFdKeywords(string accountId, string campaignId, SharePostModel rdKeywordScraperDetails, CancellationTokenSource cancellationTokenSource, int count = 10)
        {
            cancellationTokenSource.Token.ThrowIfCancellationRequested();
            var totalCount = 0;
            if (string.IsNullOrEmpty(accountId) || string.IsNullOrEmpty(campaignId) || rdKeywordScraperDetails == null)
                return;

            var accountFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            var dominatorAccountModel = accountFileManager.GetAccountById(accountId);

            cancellationTokenSource.Token.ThrowIfCancellationRequested();
            if (!LoginForScrapePost(dominatorAccountModel, campaignId, cancellationTokenSource))
                return;

            var postSource = Regex.Split(rdKeywordScraperDetails.AddKeywords, "\r\n");
            postSource.Shuffle();
            foreach (var item in postSource)
            {
                cancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (Utilities.GetBetween(item, "[", "]") == "K" || !string.IsNullOrEmpty(item))
                    ScrapeRdKeywordPost(dominatorAccountModel, campaignId, ref totalCount, item, rdKeywordScraperDetails,
                        cancellationTokenSource, count);
            }
        }
        public override void ScrapePosts(string accountId, string campaignId, ScrapePostModel scrapePostModel,
            CancellationTokenSource cancellationTokenSource, int count = 10)
        {
            cancellationTokenSource.Token.ThrowIfCancellationRequested();
            var totalCount = 0;
            if (string.IsNullOrEmpty(accountId) || string.IsNullOrEmpty(campaignId) || scrapePostModel == null)
                return;

            var accountFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            var dominatorAccountModel = accountFileManager.GetAccountById(accountId);

            cancellationTokenSource.Token.ThrowIfCancellationRequested();
            if (!LoginForScrapePost(dominatorAccountModel, campaignId, cancellationTokenSource))
                return;

            var postSource = Regex.Split(scrapePostModel.AddRdPostSource, "\r\n");
            postSource.Shuffle();
            foreach (var item in postSource)
            {
                cancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (Utilities.GetBetween(item, "[", "]") == "K")
                    ScrapeKeywordPost(dominatorAccountModel, campaignId, ref totalCount, item, scrapePostModel,
                        cancellationTokenSource, count);

                if (Utilities.GetBetween(item, "[", "]") == "U")
                    ScrapeUserPost(dominatorAccountModel, campaignId, ref totalCount, item, scrapePostModel,
                        cancellationTokenSource, count);

                if (Utilities.GetBetween(item, "[", "]") == "S")
                    ScrapeSubredditPost(dominatorAccountModel, campaignId, ref totalCount, item, scrapePostModel,
                        cancellationTokenSource, count);
            }
        }

        private void ScrapeSubredditPost(DominatorAccountModel dominatorAccountModel, string campaignId,
            ref int totalCount, string item,
            ScrapePostModel scrapePostModel, CancellationTokenSource cancellationTokenSource, int count)
        {
            try
            {
                cancellationTokenSource.Token.ThrowIfCancellationRequested();

                var lstRedditPost = new List<RedditPost>();
                RedditPostResponseHandler redditPostResponseHandler = null;
                var queryValue = item.Replace("[S]", "").TrimStart();
                if (!queryValue.Contains("www.reddit.com/r") || !queryValue.Contains("www.reddit.com/r"))
                    queryValue = $"https://www.reddit.com/r/{queryValue}/";
                var queryInfo = new QueryInfo { QueryType = "Subreddit Post", QueryValue = queryValue };
                while (totalCount < count)
                {
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (!dominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        redditPostResponseHandler = _redditFunct.ScrapePostsByUrl(dominatorAccountModel, queryValue,
                            queryInfo, redditPostResponseHandler, true);
                        lstRedditPost.AddRange(FilterRedditPostList(scrapePostModel,
                            redditPostResponseHandler.LstRedditPost));
                    }
                    //For browser automation
                    else
                    {
                        if (redditPostResponseHandler == null)
                        {
                            var response = _browserManager.SearchByCustomUrl(dominatorAccountModel, queryValue);
                            redditPostResponseHandler = new RedditPostResponseHandler(response, false, null);
                            lstRedditPost.AddRange(FilterRedditPostList(scrapePostModel,
                                redditPostResponseHandler.LstRedditPost));
                        }
                        //For browser pagination
                        else
                        {
                            var response =
                                _browserManager.ScrollWindowAndGetNextPageDataForCustomUrlScraper(dominatorAccountModel,
                                    15);
                            redditPostResponseHandler = new RedditPostResponseHandler(response, true, null);
                            lstRedditPost.AddRange(FilterRedditPostList(scrapePostModel,
                                redditPostResponseHandler.LstRedditPost));
                        }
                    }

                    if (dominatorAccountModel.IsRunProcessThroughBrowser)
                        _browserManager.CloseBrowser();
                    FilterDuplicatePost(lstRedditPost, dominatorAccountModel);
                    if (lstRedditPost.Count == 0)
                        break;

                    foreach (var redditPost in lstRedditPost)
                    {
                        cancellationTokenSource.Token.ThrowIfCancellationRequested();
                        if (totalCount >= count)
                            break;

                        if (scrapePostModel.IgnoreTextOnlyPosts &&
                            string.IsNullOrEmpty(Utilities.GetBetween(redditPost.Preview, "url\": \"", "\""))) continue;

                        SaveScrapeRedditPost(campaignId, redditPost);
                        totalCount++;
                        _publisherInitialize.UpdatePostCounts(campaignId);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void ScrapeUserPost(DominatorAccountModel dominatorAccountModel, string campaignId, ref int totalCount,
            string item,
            ScrapePostModel scrapePostModel, CancellationTokenSource cancellationTokenSource, int count)
        {
            try
            {
                cancellationTokenSource.Token.ThrowIfCancellationRequested();
                var lstRedditPost = new List<RedditPost>();
                RedditPostResponseHandler redditPostResponseHandler = null;
                var queryValue = item.Replace("[U]", "").TrimStart();
                if (!queryValue.Contains("www.reddit.com/user") || !queryValue.Contains("www.reddit.com/user"))
                    queryValue = $"https://www.reddit.com/user/{queryValue}/submitted/";
                var queryInfo = new QueryInfo { QueryType = "Specific User's Post", QueryValue = queryValue };
                while (totalCount < count)
                {
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (!dominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        redditPostResponseHandler = _redditFunct.ScrapePostsByUrl(dominatorAccountModel, queryValue,
                            queryInfo, redditPostResponseHandler, true, "new");
                        lstRedditPost.AddRange(FilterRedditPostList(scrapePostModel,
                            redditPostResponseHandler.LstRedditPost));
                    }
                    //For browser automation
                    else
                    {
                        var SearchByCustomUrlFailedCount = 0;
                        if (redditPostResponseHandler == null)
                        {
                            var response = _browserManager.SearchByCustomUrl(dominatorAccountModel, queryValue);
                            while (SearchByCustomUrlFailedCount++ < 3 && string.IsNullOrEmpty(response.Response))
                                response = _browserManager.SearchByCustomUrl(dominatorAccountModel, queryValue);
                            redditPostResponseHandler = new RedditPostResponseHandler(response, false, null);
                            lstRedditPost.AddRange(FilterRedditPostList(scrapePostModel,
                                redditPostResponseHandler.LstRedditPost));
                        }
                        //For browser pagination
                        else
                        {
                            var response =
                                _browserManager.ScrollWindowAndGetNextPageDataForCustomUrlScraper(dominatorAccountModel,
                                    15);
                            redditPostResponseHandler = new RedditPostResponseHandler(response, true, null);
                            lstRedditPost.AddRange(FilterRedditPostList(scrapePostModel,
                                redditPostResponseHandler.LstRedditPost));
                        }
                    }

                    if (dominatorAccountModel.IsRunProcessThroughBrowser)
                        _browserManager.CloseBrowser();
                    FilterDuplicatePost(lstRedditPost, dominatorAccountModel);
                    if (lstRedditPost.Count == 0 && !redditPostResponseHandler.HasMoreResults)
                    {
                        GlobusLogHelper.log.Info(Log.NoMoreDataToPerform,
                                dominatorAccountModel.AccountBaseModel.AccountNetwork, dominatorAccountModel.AccountBaseModel.UserName, "Scrape Post",
                                $"Post Not Found  {queryValue}");
                        break;
                    }

                    foreach (var redditPost in lstRedditPost)
                    {
                        cancellationTokenSource.Token.ThrowIfCancellationRequested();
                        if (totalCount >= count)
                            break;

                        if (scrapePostModel.IgnoreTextOnlyPosts &&
                            string.IsNullOrEmpty(Utilities.GetBetween(redditPost.Preview, "url\": \"", "\"")))
                        {
                        }
                        else
                        {
                            SaveScrapeRedditPost(campaignId, redditPost);
                            totalCount++;
                            _publisherInitialize.UpdatePostCounts(campaignId);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void ScrapeKeywordPost(DominatorAccountModel dominatorAccountModel, string campaignId,
            ref int totalCount, string item,
            ScrapePostModel scrapePostModel, CancellationTokenSource cancellationTokenSource, int count)
        {
            try
            {
                cancellationTokenSource.Token.ThrowIfCancellationRequested();

                var lstRedditPost = new List<RedditPost>();
                RedditPostResponseHandler redditPostResponseHandler = null;
                var queryInfo = new QueryInfo { QueryType = "Keyword", QueryValue = item.Replace("[K]", "").TrimStart() };
                while (totalCount < count)
                {
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (!dominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        redditPostResponseHandler = _redditFunct.ScrapePostsByKeywords(dominatorAccountModel,
                            queryInfo.QueryValue, queryInfo, redditPostResponseHandler);
                        lstRedditPost.AddRange(FilterRedditPostList(scrapePostModel,
                            redditPostResponseHandler.LstRedditPost));
                    }
                    //For browser automation
                    else
                    {
                        var SearchByCustomUrlFailedCount = 0;
                        if (redditPostResponseHandler == null)
                        {
                            var searchUrl = $"https://www.reddit.com/search?q={queryInfo.QueryValue}&type=link";
                            var response = _browserManager.SearchByCustomUrl(dominatorAccountModel, searchUrl);
                            while (SearchByCustomUrlFailedCount++ < 3 && string.IsNullOrEmpty(response.Response))
                                response = _browserManager.SearchByCustomUrl(dominatorAccountModel, searchUrl);
                            redditPostResponseHandler = new RedditPostResponseHandler(response, false, null);
                            lstRedditPost.AddRange(FilterRedditPostList(scrapePostModel,
                                redditPostResponseHandler.LstRedditPost));
                        }
                        //For browser pagination
                        else
                        {
                            var response = _browserManager.ScrollWindowAndGetNextPageData(dominatorAccountModel);
                            redditPostResponseHandler = new RedditPostResponseHandler(response, true, null);
                            lstRedditPost.AddRange(FilterRedditPostList(scrapePostModel,
                                redditPostResponseHandler.LstRedditPost));
                        }
                    }

                    if (dominatorAccountModel.IsRunProcessThroughBrowser)
                        _browserManager.CloseBrowser();
                    FilterDuplicatePost(lstRedditPost, dominatorAccountModel);
                    if (lstRedditPost.Count == 0)
                        break;

                    foreach (var redditPost in lstRedditPost)
                    {
                        cancellationTokenSource.Token.ThrowIfCancellationRequested();
                        if (totalCount >= count)
                            break;

                        if (scrapePostModel.IgnoreTextOnlyPosts &&
                            string.IsNullOrEmpty(Utilities.GetBetween(redditPost.Preview, "url\": \"", "\"")))
                        {
                        }
                        else
                        {
                            SaveScrapeRedditPost(campaignId, redditPost);
                            totalCount++;
                            _publisherInitialize.UpdatePostCounts(campaignId);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception)
            {
            }
        }
        private void ScrapeRdKeywordPost(DominatorAccountModel dominatorAccountModel, string campaignId,
            ref int totalCount, string item,
            SharePostModel sharePostModel, CancellationTokenSource cancellationTokenSource, int count)
        {
            try
            {
                cancellationTokenSource.Token.ThrowIfCancellationRequested();

                var lstRedditPost = new List<RedditPost>();
                RedditPostResponseHandler redditPostResponseHandler = null;
                var queryInfo = new QueryInfo { QueryType = "Keyword", QueryValue = item.Replace("[K]", "").TrimStart() };
                while (totalCount < count)
                {
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (!dominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        redditPostResponseHandler = _redditFunct.ScrapePostsByKeywords(dominatorAccountModel,
                            queryInfo.QueryValue, queryInfo, redditPostResponseHandler);
                    }
                    //For browser automation
                    else
                    {
                        var SearchByCustomUrlFailedCount = 0;
                        if (redditPostResponseHandler == null)
                        {
                            var searchUrl = $"https://www.reddit.com/search?q={queryInfo.QueryValue}&type=link";
                            var response = _browserManager.SearchByCustomUrl(dominatorAccountModel, searchUrl);
                            while (SearchByCustomUrlFailedCount++ < 3 && string.IsNullOrEmpty(response.Response))
                                response = _browserManager.SearchByCustomUrl(dominatorAccountModel, searchUrl);
                            redditPostResponseHandler = new RedditPostResponseHandler(response, false, null);
                        }
                        //For browser pagination
                        else
                        {
                            var response = _browserManager.ScrollWindowAndGetNextPageData(dominatorAccountModel);
                            redditPostResponseHandler = new RedditPostResponseHandler(response, true, null);
                        }
                    }
                    //lstRedditPost.AddRange(FilterRedditPostList(scrapePostModel,
                    //            redditPostResponseHandler.LstRedditPost));
                    lstRedditPost.AddRange(redditPostResponseHandler.LstRedditPost);
                    if (dominatorAccountModel.IsRunProcessThroughBrowser)
                        _browserManager.CloseBrowser();
                    FilterDuplicatePost(lstRedditPost, dominatorAccountModel);
                    if (lstRedditPost.Count == 0)
                        break;
                    foreach (var redditPost in lstRedditPost)
                    {
                        cancellationTokenSource.Token.ThrowIfCancellationRequested();
                        if (totalCount >= count)
                            break;
                        SaveScrapeRedditPost(campaignId, redditPost);
                        totalCount++;
                        _publisherInitialize.UpdatePostCounts(campaignId);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception)
            {
            }
        }

        private void FilterDuplicatePost(List<RedditPost> lstRedditPost, DominatorAccountModel dominatorAccount)
        {
            try
            {
                var SkippedPostCount = 0;
                if ((SkippedPostCount = lstRedditPost.RemoveAll(x => CheckForDuplicatePost(x.PostId))) > 0)
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                dominatorAccount.AccountBaseModel.AccountNetwork, dominatorAccount.AccountBaseModel.UserName, "Scrape Post",
                                $"Skipped {SkippedPostCount} Already Interacted Post.");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private List<RedditPost> FilterRedditPostList(ScrapePostModel scrapePostModel, List<RedditPost> lstRedditPost)
        {
            if (scrapePostModel.IsScrapePostOlderThanXXDays)
                return PostListOlderThanXXDays(scrapePostModel, lstRedditPost);
            return lstRedditPost;
        }

        private List<RedditPost> PostListOlderThanXXDays(ScrapePostModel scrapePostModel,
            List<RedditPost> lstRedditPost)
        {
            var LstPostOlderThanXXDays = new List<RedditPost>();
            try
            {
                var currentDateTime = DateTime.Parse(DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss"));
                currentDateTime =
                    currentDateTime.Subtract(
                        TimeSpan.FromDays(scrapePostModel.DoNotScrapePostOlderThanNDays.StartValue));

                foreach (var items in lstRedditPost)
                {
                    var postDateTime = items.Created.EpochToDateTimeUtc();

                    var dateCompare = DateTime.Compare(postDateTime, currentDateTime);

                    if (dateCompare >= 0)
                        LstPostOlderThanXXDays.Add(items);
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return LstPostOlderThanXXDays;
        }

        private void SaveScrapeRedditPost(string campaignId, RedditPost redditPost)
        {
            try
            {
                var publisherPostlistModel = new PublisherPostlistModel();
                publisherPostlistModel.GenerateClonePostId();
                publisherPostlistModel.CampaignId = campaignId;
                publisherPostlistModel.ExpiredTime = DateTime.Now.AddYears(2);
                publisherPostlistModel.PostSource = PostSource.ScrapedPost;
                publisherPostlistModel.PostQueuedStatus = PostQueuedStatus.Pending;

                publisherPostlistModel.PublisherInstagramTitle = redditPost.Title;
                publisherPostlistModel.FetchedPostIdOrUrl = redditPost.PostId;
                publisherPostlistModel.RedditScrapedMediaType = redditPost.MediaType;
                publisherPostlistModel.RedditScrapedVideoUrl = redditPost.PostVideoUrl;

                if (!string.IsNullOrEmpty(Utilities.GetBetween(redditPost.Preview, "url\": \"", "\"")))
                    publisherPostlistModel.MediaList.Add(Utilities.GetBetween(redditPost.Preview, "url\": \"", "\""));

                PostlistFileManager.Add(campaignId, publisherPostlistModel);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private bool LoginForScrapePost(DominatorAccountModel dominatorAccountModel, string campaignId,
            CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                var accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
                var rdLoginProcess = accountScopeFactory[$"{dominatorAccountModel.AccountId}_{campaignId}_Publisher"]
                    .Resolve<IRedditLogInProcess>();

                _redditFunct = accountScopeFactory[$"{dominatorAccountModel.AccountId}_{campaignId}_Publisher"]
                    .Resolve<IRedditFunction>();

                cancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (dominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    rdLoginProcess._browserManager =
                        accountScopeFactory[$"{dominatorAccountModel.AccountId}_{campaignId}_Publisher_ScrapePost"]
                            .Resolve<IRdBrowserManager>();
                    _browserManager = rdLoginProcess._browserManager;
                }

                var isLoggedIn = rdLoginProcess
                    .CheckLoginAsync(dominatorAccountModel, dominatorAccountModel.Token, true).Result;

                return isLoggedIn;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}