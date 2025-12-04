#region

using System;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.PdTables.Accounts
{
    public class Friendships
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public int DetailedInfoHasBeenRetrievedAtleastOnce { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        // ReSharper disable once UnusedMember.Global
        // need to keep it to support existing data model
        public int DetailedInfoWillNotBeRetrieved { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public int Followers { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public int Followings { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public int PinsCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public int BoardsCount { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string FullName { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public bool? HasAnonymousProfilePicture { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public bool IsPrivate { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public bool IsVerified { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public string ProfilePicUrl { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        //[Unique]
        public string Username { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public string UserId { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public int Time { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public FollowType FollowType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        public string Website { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 18)]
        public string Bio { get; set; }
    }

    [Flags]
    public enum FollowType
    {
        Following = 1,
        FollowingBack = 2,
        Mutual = 3,
        Pending = 4
    }
}