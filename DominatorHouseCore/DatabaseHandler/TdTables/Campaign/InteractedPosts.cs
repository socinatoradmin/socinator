#region

using System;
using DominatorHouseCore.Enums;
using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.TdTables.Campaign
{
    public class InteractedPosts
    {
        [PrimaryKey]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        [Indexed]
        [AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        ///     UserName of the Account from which Interaction is done
        /// </summary>
        //[Index("IX_AccountTweetIdActivityType", 1, IsUnique = true)]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string SinAccUsername { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string QueryType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string QueryValue { get; set; }

        //[Index("IX_AccountTweetIdActivityType", 2, IsUnique = true)]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string ActivityType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public int InteractionTimeStamp { get; set; }

        /// <summary>
        ///     Id of the tweet
        /// </summary>
        //[Index("IX_AccountTweetIdActivityType", 3, IsUnique = true)]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string TweetId { get; set; }

        /// <summary>
        ///     UserId of the tweet Owner
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string UserId { get; set; }

        /// <summary>
        ///     UserName of the tweet owner
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string Username { get; set; }

        /// <summary>
        ///     Image/Video url of tweet
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public string MediaId { get; set; }

        /// <summary>
        ///     Message/Description of the tweet
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public string TwtMessage { get; set; }

        /// <summary>
        ///     Like Count Of The Tweet
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public int LikeCount { get; set; }

        /// <summary>
        ///     Retweet Count Of The Tweet
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public int RetweetCount { get; set; }

        /// <summary>
        ///     Comment Count Of The Tweet
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public int CommentCount { get; set; }

        /// <summary>
        ///     True if the tweet has been retweeted
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public int IsRetweet { get; set; }

        /// <summary>
        ///     Time when the tweet has been posted in TimeStamp
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public int TweetedTimeStamp { get; set; }

        /// <summary>
        ///     Duration of the video tweets
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        public double VideoDuration { get; set; }

        /// <summary>
        ///     View Count of the video tweets
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 18)]
        public int ViewCount { get; set; }

        /// <summary>
        ///     Image or Video or Text
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 19)]
        public MediaType MediaType { get; set; }

        /// <summary>
        ///     If Interaction Type is Comment Interaction
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 20)]
        public string CommentedText { get; set; }

        /// <summary>
        ///     1(true) if following Tweet Owner
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 21)]
        public int FollowStatus { get; set; }

        /// <summary>
        ///     1(true) Tweet Owner follows back
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 22)]
        public int FollowBackStatus { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 23)]
        public int IsAlreadyLiked { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 24)]
        public int IsAlreadyRetweeted { get; set; }

        /// <summary>
        ///     Describes wheather the activity is done in Activity process or after activity process
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 25)]
        public string ProcessType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 26)]
        public DateTime InteractionDate { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 27)]
        public string CommentId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 28)]
        public string Status { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 29)]
        public DateTime TwtPostDateTime { get; set; }
    }
}