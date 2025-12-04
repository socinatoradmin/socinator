#region

using System.ComponentModel;

#endregion

namespace DominatorHouseCore.Enums.TumblrQuery
{
    public enum TumblrBroadcastMessageQuery
    {
        [Description("LangKeyKeywords")] Keyword,

        [Description("LangKeySomeonesFollowings")]
        UserFollowing,
        [Description("LangKeyOwnFollowers")] OwnFollowers,
        [Description("LangKeyHashtagUserS")] HashtagUsers,

        [Description("LangKeyUsersWhoCommentedOnPosts")]
        UserCommentedOnPost,

        [Description("LangKeyUsersWhoLikedPosts")]
        UserLikedThePost,
        [Description("LangKeyReblogPost")] UserReblogedThePost,

        [Description("LangKeyReblogLikerCommenter")]
        UserLikedCommentedReblogedThePost
    }
}