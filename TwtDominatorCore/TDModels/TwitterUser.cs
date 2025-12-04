using System;
using System.Collections.Generic;

namespace TwtDominatorCore.TDModels
{
    public class TwitterUser : BaseUser
    {
        public string UserBio { get; set; }
        public string UserLocation { get; set; }
        public string WebPageURL { get; set; }
        public DateTime JoiningDate { get; set; }
        public bool HasProfilePic { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsVerified { get; set; }
        public bool IsMuted { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingsCount { get; set; }

        public string Message { get; set; }

        public int TweetsCount { get; set; }
        public int LikesCount { get; set; }

        /// <summary>
        ///     True if you are Following to user
        /// </summary>
        public bool FollowStatus { get; set; }

        /// <summary>
        ///     True if User is Following you back
        /// </summary>
        public bool FollowBackStatus { get; set; }

        public int LastTweetedDaycount { get; set; }
        public TagDetails TagDetail { get; set; }
        public List<TagDetails> ListTag { get; set; } = new List<TagDetails>();

        public string UserStatus { get; set; }
        public long MessageTimeStamp { get; set; }
        public string MessageRecievedUserId { get; set; }
        public string MessageRecievedUserName { get; set; }
    }
}