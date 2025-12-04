#region

using System;
using System.ComponentModel.DataAnnotations.Schema;
using DominatorHouseCore.DatabaseHandler.Common;
using DominatorHouseCore.Enums;

#endregion

namespace DominatorHouseCore.DatabaseHandler.LdTables.Account
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

        [Column(Order = 6)]
        public string PostLink { get; set; }

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
        ///     Contains MyComment On the Post being interacted
        /// </summary>
        [Column(Order = 9)]
        public string MyComment { get; set; }

        /// <summary>
        ///     Contains LikesCount On the Post being interacted
        /// </summary>
        [Column(Order = 10)]
        public string LikeCount { get; set; }

        /// <summary>
        ///     Contains Comments Count On the Post being interacted
        /// </summary>
        [Column(Order = 11)]
        public string CommentCount { get; set; }

        [Column(Order = 12)] public string ShareCount { get; set; }

        /// <summary>
        ///     Contains name of the user who created the Post
        /// </summary>
        [Column(Order = 13)]
        public string PostOwnerFullName { get; set; }

        [Column(Order = 14)] public string PostOwnerProfileUrl { get; set; }

        [Column(Order = 15)] public ConnectionType ConnectionType { get; set; }

        [Column(Order = 16)] public DateTime PostedDateTime { get; set; }

        /// <summary>
        ///     TimeStamp when interacted with the Post
        /// </summary>
        [Column(Order = 17)]
        public DateTime InteractionDatetime { get; set; }

        [Column(Order = 18)] public string CommentId { get; set; }

        public ActivityType GetActivityType()
        {
            return (ActivityType) Enum.Parse(typeof(ActivityType), ActivityType);
        }
    }
}