#region

using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.GdTables.Accounts
{
    public class Friendships
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string CountryCode { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public int DetailedInfoHasBeenRetrievedAtleastOnce { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        // ReSharper disable once UnusedMember.Global
        public int DetailedInfoWillNotBeRetrieved { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public int Followers { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public int Followings { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public int PostsPerWeek { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public int Uploads { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string FullName { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public bool? HasAnonymousProfilePicture { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public bool IsPrivate { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public bool IsVerified { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public string ProfilePicUrl { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        // [Unique]
        public string Username { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public bool IsBusiness { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public string UserId { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        public int Time { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 18)]
        public FollowType FollowType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 19)]
        public bool IsFollowBySoftware { get; set; }
    }
}