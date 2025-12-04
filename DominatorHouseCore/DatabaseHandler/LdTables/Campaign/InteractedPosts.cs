#region

using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.Enums;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.LdTables.Campaign
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
        public string PostLink { get; set; }


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
        ///     Contains MyComment On the Post being interacted
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public string MyComment { get; set; }

        /// <summary>
        ///     Contains LikesCount On the Post being interacted
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public string LikeCount { get; set; }

        /// <summary>
        ///     Contains Comments Count On the Post being interacted
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public string CommentCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public string ShareCount { get; set; }

        /// <summary>
        ///     Contains name of the user who created the Post
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public string PostOwnerFullName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public string PostOwnerProfileUrl { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public ConnectionType ConnectionType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        public long PostedTime { get; set; }

        /// <summary>
        ///     TimeStamp when interacted with the Post
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 18)]
        public int InteractionTimeStamp { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 19)]
        public string CommentId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 20)]
        public string Status { get; set; }
    }
}