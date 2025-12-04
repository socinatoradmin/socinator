#region

using System.ComponentModel.DataAnnotations.Schema;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.TumblrTables.Campaign
{
    public class InteractedUser
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }

        /// <summary>
        ///     EmailId of the Account from which Interaction has been done
        /// </summary>
        [Index("Pk_AccountEmail_ActivityType_UserProfileUrl", 1, IsUnique = true)]
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
        /// Describes Activity 
        /// </summary>
        [Index("Pk_AccountEmail_ActivityType_UserProfileUrl", 2, IsUnique = true)]
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
        [Index("Pk_AccountEmail_ActivityType_UserProfileUrl", 3, IsUnique = true)]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string UserProfileUrl { get; set; }

        /// <summary>
        ///     Contains Detailed Info of the Interacted User in the Form of Jason String
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string DetailedUserInfo { get; set; }

        /// <summary>
        ///     Contains TimeStamp when interacted with the User
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public int InteractionTimeStamp { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public string UserName { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public string InteractedUsername { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public string TemplateId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public string DirectMessage { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public string PageUrl { get; set; }
    }
}