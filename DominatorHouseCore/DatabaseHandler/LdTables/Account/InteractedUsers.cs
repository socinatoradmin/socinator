#region

using System;
using System.ComponentModel.DataAnnotations.Schema;
using DominatorHouseCore.DatabaseHandler.Common;

#endregion

namespace DominatorHouseCore.DatabaseHandler.LdTables.Account
{
    public class InteractedUsers : Entity
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
        ///     Contains FullName Of the Interacted User
        /// </summary>
        [Column(Order = 5)]
        public string UserFullName { get; set; }

        /// <summary>
        ///     Contains ProfileUrl Of the Interacted User
        /// </summary>

        [Column(Order = 6)]
        public string UserProfileUrl { get; set; }

        [Column(Order = 7)] public string ProfileId { get; set; }

        /// <summary>
        ///     Contains Detailed Info of the Interacted User in the Form of Jason String
        /// </summary>
        [Column(Order = 8)]
        public string DetailedUserInfo { get; set; }

        /// <summary>
        ///     Contains TimeStamp when interacted with the User
        /// </summary>
        [Column(Order = 9)]
        public DateTime InteractionDatetime { get; set; }

        [Column(Order = 10)] public string Status { get; set; }
        [Column(Order = 11)] public string ConnectedTime { get; set; }

        [Column(Order = 12)] public string AttachmentId { get; set; }

        [Column(Order = 13)] public string PublicIdentifer { get; set; }
    }
}