#region

using System;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.FdTables.Accounts
{
    public class InteractedPages
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }

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
        ///     Contains Name of the Group being interacted
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public string PageId { get; set; }


        /// <summary>
        ///     Contains Name of the Group being interacted
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string PageName { get; set; }

        /// <summary>
        ///     Contains Id of the Group being interacted
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string PageUrl { get; set; }

        /// <summary>
        ///     Contains TotalLikers in the Group being interacted
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string TotalLikers { get; set; }

        /// <summary>
        ///     Describes CommunityType of the Group being interacted
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public string PageType { get; set; }

        /// <summary>
        ///     Describes Membership Status For this Account in the Group being interacted
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public PageMemberShip MembershipStatus { get; set; }

        /// <summary>
        ///     TimeStamp when interacted with the Group
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public int InteractionTimeStamp { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public string PageFullDetails { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public DateTime InteractionDateTime { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public string MessageText { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public string UploadedMediaPath { get; set; }
    }

    public enum PageMemberShip
    {
        LikedPage = 1,

        OwnPage = 2,
        NotLiked = 3
    }
}