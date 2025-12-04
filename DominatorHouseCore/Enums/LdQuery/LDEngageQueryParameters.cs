#region

using System.ComponentModel;

#endregion

namespace DominatorHouseCore.Enums.LdQuery
{
    public enum LDEngageQueryParameters
    {
        [Description("LangKeyKeyword")] Keyword,
        [Description("LangKeySomeonesPostS")] SomeonesPosts,

        [Description("LangKeyMyConnectionsPostS")]
        MyConnectionsPosts,
        [Description("LangKeyMyGroupsPostS")] MyGroupsPosts,

        [Description("LangKeyCustomPostsList")]
        CustomPosts,
        [Description("LangkeyGroupUrlPosts")] GroupsUrlPosts,
        [Description("LangKeyCompanyPostUrl")] CompanyUrlPost,
        [Description("LangKeyHashtagPostUrl")] HashtagUrlPost
    }
}