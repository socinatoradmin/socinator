using DominatorHouseCore.Interfaces;
using System;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.TDModels
{
    public class TagDetails : BaseUser, IPost
    {
        public bool IsAlreadyLiked { get; set; }
        public bool IsAlreadyRetweeted { get; set; }
        public bool IsAlreadyReposted { get; set; }
        public int LikeCount { get; set; }
        public int RetweetCount { get; set; }
        public int CommentCount { get; set; }
        public bool IsRetweet { get; set; }
        public DateTime DateTime { get; set; }
        public string OriginalTweetId {  get; set; }
        public bool IsRetweetedOwnTweet {  get; set; }
        public int DaysCount => TdUtility.GetDateDifferenceFromTimeStamp(TweetedTimeStamp);

        public int HoursCount => TdUtility.GetHourDifferenceFromTimeStamp(DateTime);

        public int TweetedTimeStamp { get; set; }
        public bool FollowStatus { get; set; }
        public bool FollowBackStatus { get; set; }
        public bool IsPrivate { get; set; }
        public bool HasProfilePic { get; set; }
        public bool IsComment { get; set; }
        public string CommentedOnTweetId { get; set; }
        public string CommentedOnTweetOwner { get; set; }
        public bool IsMuted { get; set; }
        public bool IsVerified { get; set; }
        public bool IsTweetContainedVideo { get; set; }

        /// <summary>
        ///     Id refers TweetID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     Code refers Image/Video Url path
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        ///     Caption refers TweetText
        /// </summary>
        public string Caption { get; set; }
        public bool IsUser => twitterUser != null;
        public TwitterUser twitterUser { get; set; }
    }
}