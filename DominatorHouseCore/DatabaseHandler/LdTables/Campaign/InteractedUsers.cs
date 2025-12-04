#region

using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.LdTables.Campaign
{
    public class InteractedUsers
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
        ///     Contains FullName Of the Interacted User
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public string UserFullName { get; set; }

        /// <summary>
        ///     Contains ProfileUrl Of the Interacted User
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string UserProfileUrl { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string ProfileId { get; set; }

        /// <summary>
        ///     Contains Detailed Info of the Interacted User in the Form of Jason String
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string DetailedUserInfo { get; set; }

        /// <summary>
        ///     Contains TimeStamp when interacted with the User
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public int InteractionTimeStamp { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public string Status { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public string ConnectedTime { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public string AttachmentId { get; set; }
    }
}