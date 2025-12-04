#region

using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.GplusTables.Campaigns
{
    public class InteractedUsersReport
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string Query { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string QueryType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string ActivityType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string UserId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public string FullName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public int FollowedBack { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public int Date { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public int FollowerCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public int FollowType { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public bool? HasAnonymousProfilePicture { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public string ProfilePicUrl { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public int IsVerified { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public string Biography { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public int Gender { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public string ProfileUrl { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public int BlockedStatus { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 18)]
        public string InteractedUserId { get; set; }
    }
}