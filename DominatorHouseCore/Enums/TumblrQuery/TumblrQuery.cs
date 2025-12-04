#region

using System.ComponentModel;

#endregion

namespace DominatorHouseCore.Enums.TumblrQuery
{
    public enum TumblrQuery
    {
        [Description("LangKeyKeywords")] Keyword,

        [Description("LangKeySomeonesFollowings")]
        UserFollowing,
        [Description("LangKeyHashtagUserS")] HashtagUsers,

        [Description("LangKeyUsersWhoCommentedOnPosts")]
        UserCommentedOnPost,

        [Description("LangKeyUsersWhoLikedPosts")]
        UserLikedThePost,
        [Description("LangKeyReblogPost")] UserReblogedThePost,

        [Description("LangKeyReblogLikerCommenter")]
        UserLikedCommentedReblogedThePost,
        [Description("LangKeyPostOwner")] PostOwner,
        [Description("LangKeyCustomUsersList")] CustomUsersList
    }

    public enum TumblrPostQuery
    {
        [Description("LangKeyKeywords")] Keyword,

        // ReSharper disable once UnusedMember.Global
        [Description("LangKeyHashtagUserS")] HashtagUsers
        //TODO
        //[Description("TumlangNewsFeed")]
        //Dashboard,
        //[Description("TumlangUsername")]
        //Username,
    }
}