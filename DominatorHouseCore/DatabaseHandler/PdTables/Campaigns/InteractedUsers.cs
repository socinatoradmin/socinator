#region

using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.PdTables.Campaigns
{
    public class InteractedUsers
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
        public int FollowedBack { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public int FollowedBackDate { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public int InteractionTime { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string ActivityType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string Username { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string InteractedUsername { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public string DirectMessage { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public string InteractedUserId { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public int UpdatedTime { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public int FollowersCount { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public int FollowingsCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public int PinsCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public int TriesCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        public string FullName { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 18)]
        public bool? HasAnonymousProfilePicture { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 19)]
        public bool IsVerified { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 20)]
        public string ProfilePicUrl { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 21)]
        public string Website { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 22)]
        public string Bio { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 23)]
        public string BoardDescription { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 24)]
        public string BoardUrl { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 25)]
        public string BoardName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 26)]
        public string Type { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 27)]
        public string SinAccId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 28)]
        public string SinAccUsername { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 29)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public bool IsFollowedByMe { get; set; }
    }
}