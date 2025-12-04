using DominatorHouseCore.DatabaseHandler.YdTables.Accounts;

namespace YoutubeDominatorCore.Report
{
    public class InteractedPostsReport
    {
        public int Id { get; set; }

        public string AccountUsername { get; set; }

        public string AccountChannelId { get; set; }

        public string QueryType { get; set; }

        public string QueryValue { get; set; }

        public string ActivityType { get; set; }

        public string InteractionTime { get; set; }

        public string ChannelName { get; set; }

        public string ChannelId { get; set; }

        public string ViewsCount { get; set; }

        public int RepeatedWatchCount { get; set; }

        public string LikeCount { get; set; }

        public string DislikeCount { get; set; }

        /// <summary>
        ///     Like Status
        /// </summary>
        public InteractedPosts.LikeStatus Reaction { get; set; }

        public string PublishedDate { get; set; }

        public string PostDescription { get; set; }

        public string SubscribeCount { get; set; }

        public string CommentCount { get; set; }

        public string VideoUrl { get; set; }

        public string VideoDuration { get; set; }

        public bool IsSubscribed { get; set; }

        public string MyCommentedText { get; set; }

        public string CommentId { get; set; }

        public string VideoTitle { get; set; }

        public string InteractedCommentUrl { get; set; }

        public string SelectedOptionToVideoReport { get; set; }

        public string SelectedTimeStampToVideoReport { get; set; }
    }
}