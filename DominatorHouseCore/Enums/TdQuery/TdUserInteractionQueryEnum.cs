#region

using System.ComponentModel;

#endregion

namespace DominatorHouseCore.Enums.TdQuery
{
    public enum TdUserInteractionQueryEnum
    {
        [Description("LangKeyKeywords")] Keywords = 1,
        [Description("LangKeyHashtags")] Hashtags = 2,
        [Description("LangKeyLocationUsers")] LocationUsers = 3,
        [Description("LangKeyNearMyLocation")] NearMyLocation = 4,

        [Description("LangKeyCustomUsersList")]
        CustomUsersList = 5,

        [Description("LangKeySomeonesFollowers")]
        SomeonesFollowers = 6,

        [Description("LangKeySomeonesFollowings")]
        SomeonesFollowings = 7,

        [Description("LangKeyFollowersOfSomeonesFollowings")]
        FollowersOfFollowings = 8,

        [Description("LangKeyFollowersOfSomeonesFollowers")]
        FollowersOfFollowers = 9,

        //[Description("LangKeyUsersWhoLikedTweet")]
        //UsersWhoLikedOnTweet = 10,

        [Description("LangKeyUsersWhoCommentedOnTweet")]
        UsersWhoCommentedOnTweet = 10,

        [Description("LangKeyUsersWhoRetweetedTweet")]
        UsersWhoRetweetedTweet = 11,

        [Description("LangKeySocinatorUserScraperCampaign")]
        UserScraperCampaign = 12
    }
}