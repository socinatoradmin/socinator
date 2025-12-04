#region

using System;
using DominatorHouseCore.Enums;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.FdTables.Campaigns
{
    public class InteractedPosts
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }

        /// <summary>
        ///     EmailId of the Account from which Interaction has been done
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string AccountEmail { get; set; }

        /// <summary>
        ///     Contains QueryType For Interaction
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string QueryType { get; set; }

        /// <summary>
        ///     Contains QueryValue For Interaction
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string QueryValue { get; set; }

        /// <summary>
        ///     Describes Activity
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string ActivityType { get; set; }

        /// <summary>
        ///     Image or Video or Text
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public MediaType MediaType { get; set; }

        /// <summary>
        ///     Contains ContentId of the Post being interacted
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string PostId { get; set; }


        /// <summary>
        ///     Contains PostTitle of the Post being interacted
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string PostTitle { get; set; }

        /// <summary>
        ///     Contains PostDiscussion of the Post being interacted
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string PostDescription { get; set; }

        /// <summary>
        ///     Contains LikesCount On the Post being interacted
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public string Likes { get; set; }

        /// <summary>
        ///     Contains Comments Count On the Post being interacted
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public string Comments { get; set; }


        /// <summary>
        ///     Contains Share Count On the Post being interacted
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public string Shares { get; set; }


        /// <summary>
        ///     TimeStamp when interacted with the Post
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public int InteractionTimeStamp { get; set; }


        /// <summary>
        ///     TimeStamp when interacted with the Post
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public string LikeType { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public string Comment { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public DateTime InteractionDateTime { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        public string CommentId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 18)]
        public string PostUrl { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 19)]
        public string Mentions { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 20)]
        public string WatchPartInvitedTo { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 21)]
        public string WatchPartInvitedToUserName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 22)]
        public bool IsMoreCommentsNeeded { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 23)]
        public string OwnerId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 24)]
        public string Status { get; set; }
    }
}