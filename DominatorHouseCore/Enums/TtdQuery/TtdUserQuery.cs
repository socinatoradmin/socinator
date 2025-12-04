#region

using System.ComponentModel;

#endregion

namespace DominatorHouseCore.Enums.TtdQuery
{
    public enum TtdUserQuery
    {
        [Description("LangKeyKeywords")] Keywords,

        [Description("LangKeyHashtagUserS")] HashTagUsers,

        [Description("LangKeySomeonesFollowers")]
        SomeonesFollowers,

        [Description("LangKeySomeonesFollowings")]
        SomeonesFollowings,

        [Description("LangKeyFollowersOfSomeonesFollowers")]
        FollowersOfFollowers,

        [Description("LangKeyFollowersOfSomeonesFollowings")]
        FollowersOfFollowings,

        [Description("LangKeyCustomUsersList")]
        CustomUsers
    }
}