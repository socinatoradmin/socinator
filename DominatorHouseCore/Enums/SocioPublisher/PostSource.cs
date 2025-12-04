namespace DominatorHouseCore.Enums.SocioPublisher
{
    public enum PostSource
    {
        /// <summary>
        ///     To Specify the Normal Post or Direct post
        /// </summary>
        NormalPost = 1,

        /// <summary>
        ///     To Specify the Share Post only for Facebook
        /// </summary>
        SharePost = 2,

        /// <summary>
        ///     To Specify the scraped Post
        /// </summary>
        ScrapedPost = 3,

        /// <summary>
        ///     To Specify the rss feed Post
        /// </summary>
        RssFeedPost = 4,

        /// <summary>
        ///     To Specify the Monitor folder Post
        /// </summary>
        MonitorFolderPost = 5,

        /// <summary>
        ///     To Specify the Scraped Images
        /// </summary>
        ScrapeImages = 6
    }
}