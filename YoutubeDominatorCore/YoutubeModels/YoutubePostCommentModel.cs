using DominatorHouseCore.DatabaseHandler.YdTables.Accounts;

namespace YoutubeDominatorCore.YoutubeModels
{
    /// <summary>
    ///     Model of Comments in a youtube post
    /// </summary>
    public class YoutubePostCommentModel
    {
        /// <summary>
        ///     Comment Text
        /// </summary>
        public string CommentText { get; set; }

        public string CommentId { get; set; }
        public string CommentActionParam { get; set; }
        public string CommentTime { get; set; }

        /// <summary>
        ///     Likes count on the comment
        /// </summary>
        public string CommentLikesCount { get; set; }

        public string CommenterChannelName { get; set; }
        public string CommenterChannelId { get; set; }

        public int LikeIndex { get; set; }

        /// <summary>
        ///     Comment Like or Dislike status by the logged-in account
        /// </summary>
        public InteractedPosts.LikeStatus CommentLikeStatus { get; set; }

        /// <summary>
        ///     TrackingParams (element needed for creating Post Data)
        /// </summary>
        public string TrackingParams { get; set; }

        public string CreateReplyParams { get; set; }
    }
}