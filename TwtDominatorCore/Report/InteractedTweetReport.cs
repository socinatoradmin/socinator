using System;

namespace TwtDominatorCore.Report
{
    public class InteractedTweetReport
    {
        public int SlNo { get; set; }

        public string SinAccUsername { get; set; }

        public string QueryType { get; set; }

        public string QueryValue { get; set; }

        public string TweetId { get; set; }

        public string TweetOwnerId { get; set; }

        public string TweetOwnerName { get; set; }

        public string MediaPath { get; set; }

        public string TweetedText { get; set; }

        public int LikeCount { get; set; }

        public int RetweetCount { get; set; }

        public int CommentCount { get; set; }

        public string Retweet { get; set; }

        public string TweetedDate { get; set; }


        public string CommentedText { get; set; }

        public string FollowStatus { get; set; }

        public string FollowBackStatus { get; set; }

        public string Category { get; set; }


        public string ProcessType { get; set; }

        public DateTime InteractionDate { get; set; }

        public string AlreadyLiked { get; set; }
        public string AlreadyRetweeted { get; set; }
    }
}