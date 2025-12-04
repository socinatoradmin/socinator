using System;

namespace TwtDominatorCore.TDUtility
{
    public class AccountDetails
    {
        public string UserName { get; set; }
        public string ProfileName { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public int FollowerCount { get; set; }
        public int FollowingCount { get; set; }
        public int TweetCount { get; set; }
        public bool IsPrivate { get; set; }
        public Int64 PhoneNumber  = 0;
    }
}