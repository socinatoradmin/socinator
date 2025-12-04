#region

using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.YdTables.Campaign
{
    public class InteractedPosts
    {
        [PrimaryKey]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        [Indexed]
        [AutoIncrement]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string AccountUsername { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string QueryType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string QueryValue { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string ActivityType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public int InteractionTimeStamp { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string ChannelName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string ChannelId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string ViewsCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public string LikeCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public string DislikeCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public Accounts.InteractedPosts.LikeStatus ReactionOnPost { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public string PublishedDate { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public string PostDescription { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public string SubscribeCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public string CommentCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        public string VideoUrl { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 18)]
        public int VideoLength { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 19)]
        public bool IsSubscribed { get; set; }

        /// <summary>
        ///     use it as CommentText/ReplyCommentText/LikedCommentText/ReportText
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 20)]
        public string MyCommentedText { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 21)]
        public string CommentId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 22)]
        public string MyChannelId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 23)]
        public string MyChannelPageId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 24)]
        public int ViewingCountPerAccount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 25)]
        public string InteractedChannelUsername { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 26)]
        public string CommenterChannelId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 27)]
        public string Status { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 28)]
        public string PostTitle { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 29)]
        public string InteractedCommentUrl { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 30)]
        public string SelectedOptionToVideoReport { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 31)]
        public string SelectedTimeStampToVideoReport { get; set; }
    }
}