using DominatorHouseCore.DatabaseHandler.YdTables.Accounts;
using DominatorHouseCore.Interfaces;
using System.Collections.Generic;

namespace YoutubeDominatorCore.YoutubeModels
{
    public class YoutubePost : IPost
    {
        public string PostUrl { get; set; }
        public string Title { get; set; }
        public string PublishedDate { get; set; }
        public string Type { get; set; }
        public string ViewsCount { get; set; }
        public int VideoLength { get; set; }

        public InteractedPosts.LikeStatus Reaction { get; set; }
        public bool LikeEnabled { get; set; }
        public bool DislikeEnabled { get; set; }
        public bool CommentEnabled { get; set; }
        public string LikeCount { get; set; }
        public string DislikeCount { get; set; }
        public int CommentLikeIndex { get; set; }
        public string CommentCount { get; set; }

        public string InteractingCommentId { get; set; }
        public string CommentActionParam { get; set; }
        public string InteractingCommentText { get; set; }
        public string InteractingCommenterName { get; set; }
        public string InteractingCommenterChannelId { get; set; }
        public InteractedPosts.LikeStatus InteractingCommentLikeStatus { get; set; }
        public List<YoutubePostCommentModel> ListOfCommentsInfo { get; set; }

        public string ChannelTitle { get; set; }
        public string ChannelId { get; set; }
        public string ChannelUsername { get; set; }
        public string ChannelViewsCount { get; set; }
        public string ChannelSubscriberCount { get; set; }
        public bool IsChannelSubscribed { get; set; }

        /// <summary>
        ///     Total Watching Count of a video by the same account till now
        /// </summary>
        public int TotalWatchingCount { get; set; }

        public PostDataElements PostDataElements { get; set; }
        public HeadersElements HeadersElements { get; set; }
        public string Id { get; set; }

        /// <summary>
        ///     Video Id of the Post
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        ///     Post's Description
        /// </summary>
        public string Caption { get; set; }
    }
}