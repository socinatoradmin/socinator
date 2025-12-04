#region

using System;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.FdTables.Campaigns
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
        public string UserId { get; set; }

        /// <summary>
        ///     Contains ProfileUrl Of the Interacted User
        /// </summary>
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

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public string Username { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public DateTime InteractionDateTime { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public string ScrapedProfileUrl { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public bool IsPublishedToWall { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public string PostDescription { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public string PublishedUrl { get; set; }

        /// <summary>
        /// Contains Gender of the InteractedUser
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public string Gender { get; set; }

        /// <summary>
        /// Contains University Name of the InteractedUser
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        public string University { get; set; }

        /// <summary>
        /// Contians Workplace of the InteractedUser
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 18)]
        public string Workplace { get; set; }

        /// <summary>
        /// Contains CurrentCity of the InteractedUser
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 19)]
        public string CurrentCity { get; set; }

        /// <summary>
        /// Contains HomeTown of the InteractedUser
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 20)]
        public string HomeTown { get; set; }

        /// <summary>
        /// Contains BirthDate of the InteractedUser
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 21)]
        public string BirthDate { get; set; }

        /// <summary>
        /// Contains ContactNo of the InteractedUser
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 22)]
        public string ContactNo { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 23)]
        public string ProfilePic { get; set; }
    }
}