#region

using System.ComponentModel;

#endregion

namespace DominatorHouseCore.Enums.GdQuery
{
    public enum GdUserQuery
    {
        [Description("LangKeyKeywords")] Keywords,
        [Description("LangKeySuggestedUsers")] SuggestedUsers,
        [Description("LangKeyHashtagUserS")] HashtagUsers,

        [Description("LangKeySomeonesFollowers")]
        SomeonesFollowers,

        [Description("LangKeySomeonesFollowings")]
        SomeonesFollowings,
        [Description("LangKeyLocationUsers")] LocationUsers,

        [Description("LangKeyCustomUsersList")]
        CustomUsers,

        [Description("LangKeyUsersWhoLikedPosts")]
        UsersWhoLikedPost,

        [Description("LangKeyUsersWhoCommentedOnPosts")]
        UsersWhoCommentedOnPost,

        [Description("LangKeyScrapUsersWhoMessagedUs")]
        ScrapUserWhoMessagedUs,
        [Description("LangKeyOwnFollowers")] OwnFollowers,
        [Description("LangKeyOwnFollowings")] OwnFollowings,

        [Description("LangKeyScrapUserWhomWeMessaged")]
        ScrapeUsersToWhomWeMessaged
    }
    public enum GdReelScraperQuery
    {
        [Description("LangKeyCustomUsersLists")]
        CustomUsers,
        [Description("LangKeyRandomUsersLists")]
        RandomUsers
    }
}