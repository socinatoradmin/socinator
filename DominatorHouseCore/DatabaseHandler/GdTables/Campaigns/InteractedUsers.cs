#region

using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.GdTables.Campaigns
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
        public int Date { get; set; }

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
        public int Time { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public bool IsPrivate { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public bool IsBusiness { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public bool IsVerified { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public bool? IsProfilePicAvailable { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        public string ProfilePicUrl { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 18)]
        public string Status { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 19)]
        public string RequiredData { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 20)]
        public string TaggedUser { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 21)]
        public string Gender { get; set; }
    }
}