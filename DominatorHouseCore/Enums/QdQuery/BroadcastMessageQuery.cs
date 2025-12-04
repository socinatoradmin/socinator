#region

using System.ComponentModel;

#endregion

namespace DominatorHouseCore.Enums.QdQuery
{
    public enum BroadcastMessageQuery
    {
        [Description("LangKeyKeywords")] Keywords,

        [Description("LangKeySomeonesFollowers")]
        SomeonesFollowers,

        [Description("LangKeySomeonesFollowings")]
        SomeonesFollowings,

        [Description("LangKeyFollowersOfSomeonesFollowings")]
        FollowersOfFollowings,

        [Description("LangKeyFollowersOfSomeonesFollowers")]
        FollowersOfFollowers,

        [Description("LangKeyCustomUsersList")]
        CustomUsers

        //[Description("LangKeyCustomURLS")]
        //CustomUrl
    }
}