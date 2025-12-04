#region

using System.ComponentModel;

#endregion

namespace DominatorHouseCore.Enums.TtdQuery
{
    public enum TtdPostQuery
    {
        [Description("LangKeySpecificUserPost")]
        SpecificUserPost,

        [Description("LangKeySomeonesFollowersPostS")]
        SomeoneFollowersPost,

        [Description("LangKeySomeonesFollowingsPostS")]
        SomeoneFollowingPost,

        [Description("LangKeyKeywordUserPost")]
        KeywordUserPost,
        [Description("LangKeyHashtagPostS")] HashTagPost,

        [Description("LangKeyHashtagUsersPostS")]
        HashTagUserPost,
        [Description("LangKeyKeywordPostS")]
        KeywordPost
    }
}