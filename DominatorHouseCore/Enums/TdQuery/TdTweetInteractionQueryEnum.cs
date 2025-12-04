#region

using System.ComponentModel;

#endregion

namespace DominatorHouseCore.Enums.TdQuery
{
    public enum TdTweetInteractionQueryEnum
    {
        [Description("LangKeyKeywords")] Keywords = 1,
        [Description("LangKeyHashtags")] Hashtags = 2,
        [Description("LangKeyLocationTweets")] LocationTweets = 3,
        [Description("LangKeyNearMyLocation")] NearMyLocation = 4,

        [Description("LangKeyCustomTweetLists")]
        CustomTweetsList = 5,

        [Description("LangKeySomeonesFollowersTweets")]
        SomeonesFollowers = 6,

        [Description("LangKeySomeonesFollowingsTweets")]
        SomeonesFollowings = 7,

        [Description("LangKeyFollowersOfSomeonesFollowingsTweets")]
        FollowersOfFollowings = 8,

        [Description("LangKeyFollowersOfSomeonesFollowersTweets")]
        FollowersOfFollowers = 9,

        //[Description("LangKeyLikedUsersTweets")]
        //UsersWhoLikedOnTweet = 10,

        [Description("LangKeyCommentedUsersTweets")]
        UsersWhoCommentedOnTweet = 10,

        [Description("LangKeyRetweetedUsersTweets")]
        UsersWhoRetweetedTweet = 11,

        [Description("LangKeyCommentedTweets")]
        CommentedTweet = 12,

        [Description("LangKeySocinatorPublisherCampaign")]
        SocinatorPublisherCampaign = 13,

        //[Description("LangKeyTweetsLikedBySpecificUser")]
        //TweetsLikedBySpecificUser = 14,

        [Description("LangKeySpecificUserTweets")]
        SpecificUserTweets = 14,

        [Description("LangKeySocinatorTweetScraperCampaign")]
        TweetScraperCampaign = 15
    }
}