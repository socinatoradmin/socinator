#region

using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.TdTables.Accounts
{
    public class FeedInfoes
    {
        [PrimaryKey]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        [Indexed]
        [AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        ///     Id of the Tweet
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string TweetId { get; set; }

        /// <summary>
        ///     path of the media
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string MediaId { get; set; }

        /// <summary>
        ///     Description of the tweet
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string TwtMessage { get; set; }

        /// <summary>
        ///     Like Count Of The Tweet
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public int LikeCount { get; set; }

        /// <summary>
        ///     Retweet Count Of The Tweet
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public int RetweetCount { get; set; }

        /// <summary>
        ///     Comment Count Of The Tweet
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public int CommentCount { get; set; }

        /// <summary>
        ///     True if the tweet has been retweeted
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public int IsRetweet { get; set; }

        /// <summary>
        ///     Time when the tweet has been posted in TimeStamp
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public int TweetedTimeStamp { get; set; }

        /// <summary>
        ///     Duration of the video tweets
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public double VideoDuration { get; set; }

        /// <summary>
        ///     View count of the video
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public int ViewCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public int IsComment { get; set; }

        /// <summary>
        ///     tweet id on which comment is done
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public string CommentedOnTweetID { get; set; }

        /// <summary>
        ///     owner of the tweet on which comment or retweet is done
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public string TweetOwnerIfRetweetedOrCommented { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public string SourceTweetId { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public bool IsRetweetedOwnTweet { get; set; }
    }
}