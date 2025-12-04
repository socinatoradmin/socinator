#region

using System;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.LdTables.Campaign
{
    public class InteractedPage
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }

        /// <summary>
        ///     Contains QueryType For Interaction
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string AccountEmail { get; set; }

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
        public string PageName { get; set; }

        /// <summary>
        ///     Contains Url of the Group being interacted
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string PageUrl { get; set; }

        /// <summary>
        ///     Contains TotalMembers in the Group being interacted
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string FollowerCount { get; set; }

        /// <summary>
        ///     Describes CommunityType of the Group being interacted
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string PageId { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public string IsFollowed { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public string TotalEmployees { get; set; }

        /// <summary>
        ///     TimeStamp when interacted with the Group
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public DateTime InteractionDatetime { get; set; }
    }
}