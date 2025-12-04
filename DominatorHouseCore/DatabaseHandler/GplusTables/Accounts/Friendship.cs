#region

using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.GplusTables.Accounts
{
    public class Friendships
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
        public string ProfileUrl { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public int Time { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        //public  FollowType FollowType
        public int FollowType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public int FollowedMeBack { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public int BlockedStatus { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public int ActiveStatus { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public int FollowerCount { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public string Biography { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public int Gender { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public bool? HasAnonymousProfilePicture { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        public string ProfilePicUrl { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 18)]
        public int IsVerified { get; set; }
    }

    //public enum FollowType : int
    //{
    //    Following, NotFollowing, Unfollowed
    //}       
}