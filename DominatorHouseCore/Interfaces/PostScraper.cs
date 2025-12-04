#region

using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.Interfaces
{
    public abstract class PostScraper
    {
        private readonly string _CampaignId;
        private readonly PublisherPostFetchModel _CurrentPostFetcher;
        private readonly CancellationTokenSource _PostFetcherCancellationToken;

        public static ConcurrentDictionary<string, object> _UpdatingLock { get; set; } =
            new ConcurrentDictionary<string, object>();

        private static ConcurrentDictionary<string, int> _campaignPostCount { get; } =
            new ConcurrentDictionary<string, int>();

        private static ConcurrentDictionary<string, string> _lstCampaignPostId { get; } =
            new ConcurrentDictionary<string, string>();

        protected PublisherInitialize _publisherInitialize = PublisherInitialize.GetInstance;

        protected PostScraper(string CampaignId, CancellationTokenSource campaignCancellationToken,
            PublisherPostFetchModel postFetcherModel)
        {
            _CampaignId = CampaignId;
            _CurrentPostFetcher = postFetcherModel;
            _PostFetcherCancellationToken = campaignCancellationToken;

            var postList = PostlistFileManager.GetAll(_CampaignId);
            var pendingPostCount = PostlistFileManager.GetAll
                (_CampaignId)?.Where(x => x.PostQueuedStatus == PostQueuedStatus.Pending).ToList();
            _campaignPostCount.AddOrUpdate(_CampaignId, pendingPostCount?.Count ?? 0
                , (id, count) => pendingPostCount?.Count ?? 0);

            postList.ForEach(x => { _lstCampaignPostId.TryAdd(x.FetchedPostIdOrUrl, x.CampaignId); });
        }


        protected void CheckForMaxPostCount()
        {
            var updatelock = _UpdatingLock.GetOrAdd(_CampaignId, _lock => new object());

            lock (updatelock)
            {
                int runningCount;

                _campaignPostCount.TryGetValue(_CampaignId, out runningCount);
                if (runningCount > 0 && runningCount >= _CurrentPostFetcher.MaximumPostLimitToStore)
                {
                    _PostFetcherCancellationToken.Cancel();
                }
                else
                {
                    runningCount++;
                    _campaignPostCount.AddOrUpdate(_CampaignId, runningCount, (id, count) =>
                    {
                        if (count < 0)
                            // ReSharper disable once RedundantAssignment
                            count = 0;
                        count = runningCount;
                        return count;
                    });
                }
            }
        }


        protected bool CheckForDuplicatePost(string postId)
        {
            var updatelock = _UpdatingLock.GetOrAdd($"{_CampaignId}_Duplicates", _lock => new object());

            lock (updatelock)
            {
                if (_lstCampaignPostId.TryAdd(postId, _CampaignId))
                {
                    CheckForMaxPostCount();

                    _PostFetcherCancellationToken.Token.ThrowIfCancellationRequested();

                    return false;
                }

                return true;
            }
        }

        #region Post Scrapers

        /// <summary>
        ///     To fetch the posts from selected destinations
        /// </summary>
        /// <param name="accountId">Account Id</param>
        /// <param name="campaignId">Campaign Id</param>
        /// <param name="scrapePostDetails"><see cref="ScrapePostModel" />Neccessary given input for scrape Posts</param>
        /// <param name="cancellationTokenSource">Cancellation Token Source for stop the running task</param>
        /// <param name="count">Specifies how many need to scrape</param>
        public virtual void ScrapePosts(string accountId, string campaignId, ScrapePostModel scrapePostDetails,
            CancellationTokenSource cancellationTokenSource, int count = 10)
        {
        }

        #endregion

        #region Scrape page post url 

        /// <summary>
        ///     To Scrape the post from facebook page
        /// </summary>
        /// <param name="accountId">Account Id</param>
        /// <param name="campaignId">Campaign Id</param>
        /// <param name="fdPagePostUrlScraperDetails">Scraping Details <see cref="SharePostModel" /></param>
        /// <param name="cancellationTokenSource">Cancellation Token Source for stop the running task</param>
        /// <param name="count">Specifies how many need to scrape</param>
        // Note : Only for Facebook
        public virtual void ScrapeFdPagePostUrl(string accountId, string campaignId,
            SharePostModel fdPagePostUrlScraperDetails, CancellationTokenSource cancellationTokenSource, int count = 10)
        {
        }

        public virtual void ScrapeFdKeywords(string accountId, string campaignId,
            SharePostModel fdKeywordScraperDetails, CancellationTokenSource cancellationTokenSource, int count = 10)
        {
        }

        #endregion

        #region Rss feed

        /// <summary>
        ///     Strating scraping Rss Post details
        /// </summary>
        /// <param name="campaignId">Campaign Id</param>
        /// <param name="rssFeedModels"> Collection of <see cref="PublisherRssFeedModel" /> rss feed models.</param>
        /// <param name="cancellationTokenSource">Cancellation Token Source for stop the running task</param>
        /// <param name="maximumPostLimitToStore">Maximum post limit for the postlists</param>
        /// <param name="campaignName">Campaign Name</param>
        public void ScrapeRssPosts(string campaignId, ObservableCollection<PublisherRssFeedModel> rssFeedModels,
            CancellationTokenSource cancellationTokenSource, int maximumPostLimitToStore, string campaignName)
        {
            ThreadFactory.Instance.Start(() =>
            {
                //Create a object of Rss Feed Utilities
                var rssFeedUtilities = new RssFeedUtilities();
                rssFeedModels.ForEach(async x =>
                {
                    // Call the scraper methods for Rss Post Fetcing
                    await rssFeedUtilities.RssFeedFetchMethod(x.FeedUrl, x.FeedTemplate, x.PostDetailsModel, campaignId,
                        cancellationTokenSource, maximumPostLimitToStore, campaignName);
                });
            }, cancellationTokenSource.Token);
        }

        public void ScrapeImagePosts(string campaignId, ScrapePostModel postFetcherModel,
            CancellationTokenSource cancellationTokenSource, int maximumPostLimitToStore, string campaignName)
        {
            ThreadFactory.Instance.Start(() =>
            {
                //Create a object of Rss Feed Utilities
                var mediaFetcherUtilities = new MediaFetcherUtilities();
                mediaFetcherUtilities.FetchDetailsFromLink(postFetcherModel, campaignId, cancellationTokenSource,
                    maximumPostLimitToStore, campaignName);
            }, cancellationTokenSource.Token);
        }

        #endregion

        #region MonitorFolder

        /// <summary>
        /// </summary>
        /// <param name="campaignId">Campaign Id</param>
        /// <param name="monitorFolderModels">Collection of <see cref="PublisherMonitorFolderModel" /> monitor folder models.</param>
        /// <param name="cancellationTokenSource">Cancellation Token Source for stop the running task</param>
        /// <param name="maximumPostLimitToStore">Maximum post limit for the postlists</param>
        /// <param name="campaignName">Campaign Name</param>
        public void FetchMonitorFoldersPosts(string campaignId,
            ObservableCollection<PublisherMonitorFolderModel> monitorFolderModels,
            CancellationTokenSource cancellationTokenSource, int maximumPostLimitToStore, string campaignName)
        {
            ThreadFactory.Instance.Start(() =>
            {
                //Create a object of Monitor Folder Utilities
                var monitorFolderUtilites = new MonitorFolderUtilites();
                monitorFolderModels.ForEach(x =>
                {
                    // Call the scraper methods for folders Post details
                    monitorFolderUtilites.GetFoldersFileDetails(x.FolderPath, campaignId, x.FolderTemplate,
                        x.PostDetailsModel, cancellationTokenSource, maximumPostLimitToStore, campaignName);
                });
            }, cancellationTokenSource.Token);
        }

        #endregion
    }
}