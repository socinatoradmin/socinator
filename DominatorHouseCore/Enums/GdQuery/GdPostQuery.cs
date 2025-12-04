#region

using System.ComponentModel;

#endregion

namespace DominatorHouseCore.Enums.GdQuery
{
    public enum GdPostQuery
    {
        [Description("LangKeySuggestedUsersPosts")]
        SuggestedUsersPosts,
        [Description("LangKeyHashtagPostS")] HashtagPost,

        [Description("LangKeyHashtagUsersPostS")]
        HashtagUsersPost,
        [Description("LangKeyLocationPosts")] LocationPosts,

        [Description("LangKeyLocationUsersPosts")]
        LocationUsersPost,
        [Description("LangKeyCustomPhotos")] CustomPhotos,

        [Description("LangKeyPostsOfUsersWhoLikedPost")]
        PostOfUsersWhoLikedPost,

        [Description("LangKeyPostsOfUsersWhoCommentedOnPost")]
        PostOfUsersWhoCommentedOnPost,

        [Description("LangKeySpecificUsersPosts")]
        SpecificUsersPosts,

        [Description("LangKeySocinatorPublisherCampaign")]
        SocinatorPublisherCampaign,
        [Description("LangKeyScrapAllLikes")] ScrapAllLikedPost
    }
}