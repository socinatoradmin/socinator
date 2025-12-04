#region

using System;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.QdTables.Campaigns
{
    public class InteractedUsers
    {
        [PrimaryKey]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        [Indexed]
        [AutoIncrement]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string SinAccUsername { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string QueryType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string QueryValue { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string ActivityType { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public string InteractedUsername { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string InteractedUserId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string InteractedUserFullName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public int FollowStatus { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public int FollowBackStatus { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public int InteractionTimeStamp { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public string DirectMessage { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public int UpdatedTime { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public int FollowersCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public int FollowingsCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public int AnswerCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        public int QuestionCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 18)]
        public int HasAnonymousProfilePicture { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 19)]
        public int IsPrivate { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 20)]
        public string ProfilePicUrl { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 21)]
        public int JoinedDate { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 22)]
        public string Location { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 23)]
        public string Website { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 24)]
        public string Bio { get; set; }

        /// <summary>
        ///     Describes wheather the activity is done in Activity process or after activity process
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 25)]
        public string ProcessType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 26)]
        public DateTime InteractionDateTime { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 27)]
        public int PostCount { get; set; }
    }
}