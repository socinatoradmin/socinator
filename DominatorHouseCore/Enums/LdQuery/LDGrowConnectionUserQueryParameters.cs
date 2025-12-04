#region

using System.ComponentModel;

#endregion

namespace DominatorHouseCore.Enums.LdQuery
{
    public enum LDGrowConnectionUserQueryParameters
    {
        [Description("LangKeyKeyword")] Keyword,
        [Description("LangKeyEmail")] Email,
        [Description("LangKeyProfileUrl")] ProfileUrl,
        [Description("LangKeySearchUrl")] SearchUrl,
        [Description("LangKeyJoinedGroupUrl")] JoinedGroupUrl,

        [Description("LangKeySalesNavUserScraperCampaign")]
        SalesNavUserScraperCampaign,

        [Description("LangKeySalesNavigatorSearchUrl")]
        SalesNavigatorSearchUrl,

        [Description("LangKeyLinkedinPageUrl")]
        PageUrl,

        [Description("LangKeyJobScraperCampaign")]
        JobScraperCampaign
    }
}