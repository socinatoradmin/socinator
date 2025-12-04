#region

using System;
using System.ComponentModel.DataAnnotations.Schema;
using DominatorHouseCore.DatabaseHandler.Common;
using DominatorHouseCore.Enums;

#endregion

namespace DominatorHouseCore.DatabaseHandler.YdTables.Accounts
{
    public class InteractedPosts : Entity, IActivityTypeEntity
    {
        [Column(Order = 2)] public string AccountUsername { get; set; }

        [Column(Order = 3)] public string QueryType { get; set; }

        [Column(Order = 4)] public string QueryValue { get; set; }

        [Column(Order = 5)] public string ActivityType { get; set; }

        [Column(Order = 6)] public int InteractionTimeStamp { get; set; }

        [Column(Order = 7)] public string ChannelName { get; set; }

        [Column(Order = 8)] public string ChannelId { get; set; }

        [Column(Order = 9)] public string ViewsCount { get; set; }

        [Column(Order = 10)] public string LikeCount { get; set; }

        [Column(Order = 11)] public string DislikeCount { get; set; }

        [Column(Order = 12)] public LikeStatus ReactionOnPost { get; set; }

        [Column(Order = 13)] public string PublishedDate { get; set; }

        [Column(Order = 14)] public string PostDescription { get; set; }

        [Column(Order = 15)] public string SubscribeCount { get; set; }

        [Column(Order = 16)] public string CommentCount { get; set; }

        [Column(Order = 17)] public string VideoUrl { get; set; }

        [Column(Order = 18)] public int VideoLength { get; set; }

        [Column(Order = 19)] public bool IsSubscribed { get; set; }

        /// <summary>
        ///     use it as CommentText/ReplyCommentText/LikedCommentText/ReportText
        /// </summary>
        [Column(Order = 20)]
        public string MyCommentedText { get; set; }

        [Column(Order = 21)] public string CommentId { get; set; }

        [Column(Order = 22)] public string MyChannelId { get; set; }
        [Column(Order = 23)] public string MyChannelPageId { get; set; }
        [Column(Order = 24)] public int ViewingCountPerAccount { get; set; }

        [Column(Order = 25)] public string InteractedChannelUsername { get; set; }

        [Column(Order = 26)] public string CommenterChannelId { get; set; }

        [Column(Order = 27)] public string PostTitle { get; set; }

        [Column(Order = 28)] public string InteractedCommentUrl { get; set; }

        [Column(Order = 29)] public string SelectedOptionToVideoReport { get; set; }

        [Column(Order = 30)] public string SelectedTimeStampToVideoReport { get; set; }

        public ActivityType GetActivityType()
        {
            return (ActivityType) Enum.Parse(typeof(ActivityType), ActivityType);
        }

        public enum LikeStatus
        {
            Like,
            Dislike,
            Indifferent
        }
    }
}