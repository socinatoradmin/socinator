#region

using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.TtdTables.Accounts
{
    public class Friendships
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string Username { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string UserId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string FullName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string ProfilePicUrl { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public bool IsVerified { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public bool IsBlocked { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public bool IsBlocking { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public int Gender { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public string Birthday { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public string Signature { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public string FollowingsCount { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public string FollowersCount { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public string FeedsCount { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public string Country { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public FollowType FollowType { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        public bool IsFollowBySoftware { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 18)]
        public int UpdateTime { get; set; }
    }
}