#region

using System;
using System.ComponentModel.DataAnnotations.Schema;
using DominatorHouseCore.DatabaseHandler.Common;
using DominatorHouseCore.Enums;

#endregion

namespace DominatorHouseCore.DatabaseHandler.TumblrTables.Account
{
    public class InteractedPosts : Entity, IActivityTypeEntity
    {
        /// <summary>
        ///     Contains QueryType For Interaction
        /// </summary>
        [Column(Order = 2)]
        public string QueryType { get; set; }

        /// <summary>
        ///     Contains QueryValue For Interaction
        /// </summary>
        [Column(Order = 3)]
        public string QueryValue { get; set; }

        /// <summary>
        ///     Describes Activity
        /// </summary>
        [Index("Pk_ActivityType_ContentId", 1, IsUnique = true)]
        [Column(Order = 4)]
        public string ActivityType { get; set; }

        /// <summary>
        ///     Image or Video or Text
        /// </summary>

        [Column(Order = 5)]
        public MediaType MediaType { get; set; }

        /// <summary>
        ///     Contains ContentId of the Post being interacted
        /// </summary>
        [Index("Pk_ActivityType_ContentId", 2, IsUnique = true)]
        [Column(Order = 6)]
        public string ContentId { get; set; }

        /// <summary>
        ///     Contains PostTitle of the Post being interacted
        /// </summary>
        [Column(Order = 7)]
        public string PostTitle { get; set; }

        /// <summary>
        ///     Contains PostDiscussion of the Post being interacted
        /// </summary>
        [Column(Order = 8)]
        public string PostDescription { get; set; }

        /// <summary>
        ///     Contains LikesCount On the Post being interacted
        /// </summary>
        [Column(Order = 9)]
        public string Likes { get; set; }

        /// <summary>
        ///     Contains CommentsCount On the Post being interacted
        /// </summary>
        [Column(Order = 10)]
        public string Comments { get; set; }

        /// <summary>
        ///     TimeStamp when interacted with the Post
        /// </summary>
        [Column(Order = 11)]
        public int InteractionTimeStamp { get; set; }

        [Column(Order = 12)] public string CommentId { get; set; }
        [Column(Order = 13)] public string InteractedUserName { get; set; }

        [Column(Order = 14)] public string NotesCount { get; set; }
        [Column(Order = 15)] public string LikeCount { get; set; }
        [Column(Order = 16)] public string ReblogCount { get; set; }
        [Column(Order = 17)] public string PostUrl { get; set; }
        [Column(Order = 18)] public string ProfileUrl { get; set; }

        [Column(Order = 19)] public string Type { get; set; }

        [Column(Order = 20)] public string CommentText { get; set; }

        [Column(Order = 21)] public string ReblogText { get; set; }
        [Column(Order = 22)] public string AccountName { get; set; }

        [Column(Order = 23)] public string DataRootId { get; set; }
        [Column(Order = 24)] public string DataKey { get; set; }
        [Column(Order = 25)] public string ReblogUrl { get; set; }

        public ActivityType GetActivityType()
        {
            return (ActivityType) Enum.Parse(typeof(ActivityType), ActivityType);
        }
    }
}