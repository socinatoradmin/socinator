#region

using System.ComponentModel.DataAnnotations.Schema;
using DominatorHouseCore.Enums;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.TumblrTables.Campaign
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
        [Index("Pk_AccountEmail_ActivityType_ContentId", 1, IsUnique = true)]
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
        [Index("Pk_AccountEmail_ActivityType_ContentId", 2, IsUnique = true)]
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
        [Index("Pk_AccountEmail_ActivityType_ContentId", 3, IsUnique = true)]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string ContentId { get; set; }


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
        ///     TimeStamp when interacted with the Post
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public int InteractionTimeStamp { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public string CommentId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public string InteractedUserName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public string Status { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public string NotesCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        public string LikeCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 18)]
        public string ReblogCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 19)]
        public string PostUrl { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 20)]
        public string ProfileUrl { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 21)]
        public string Type { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 22)]
        public string CommentText { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 23)]
        public string ReblogText { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 24)]
        public string DataRootId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 25)]
        public string DataKey { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 26)]
        public string ReblogUrl { get; set; }
    }
}