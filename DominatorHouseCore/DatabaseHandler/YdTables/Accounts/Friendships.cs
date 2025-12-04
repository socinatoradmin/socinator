#region

using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.YdTables.Accounts
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
        public int DetailedInfoHasBeenRetrievedAtleastOnce { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        // ReSharper disable once UnusedMember.Global
        public int DetailedInfoWillNotBeRetrieved { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string Username { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        //[Unique]
        public string UserId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public string FullName { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public int FollowersCount { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public int FollowingsCount { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public int TweetsCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public int LikesCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public int HasAnonymousProfilePicture { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public int IsPrivate { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public int IsVerified { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public string ProfilePicUrl { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public int Time { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public FollowType FollowType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        public int JoinedDate { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 18)]
        public string Location { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 19)]
        public string Website { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 20)]
        public string Bio { get; set; }
    }


    public enum FollowType
    {
        Following = 1,
        Followers = 2,
        Mutual = 3
    }
}