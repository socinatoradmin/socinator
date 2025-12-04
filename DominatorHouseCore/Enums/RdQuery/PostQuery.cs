#region

using System.ComponentModel;

#endregion

namespace DominatorHouseCore.Enums.RdQuery
{
    public enum PostQuery
    {
        [Description("LangKeyKeywords")] Keywords = 1,
        [Description("LangKeyCustomurl")] CustomUrl = 2,

        [Description("LangKeySocinatorPublisherCampaign")]
        SocinatorPublisherCampaign = 3,
        [Description("LangKeyCommunityUrl")] CommunityUrl = 4,

        [Description("LangKeySpecificUserPost")]
        SpecificUserPost = 5
    }
}