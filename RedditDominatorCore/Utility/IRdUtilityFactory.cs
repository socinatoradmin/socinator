using DominatorHouseCore.Enums;

namespace RedditDominatorCore.Utility
{
    public interface IRdUtilityFactory
    {
        ActivityType ModuleName { get; set; }
        IRdReportFactory RdReportFactory { get; set; }
        IRdViewCampaign RdViewCampaign { get; set; }
    }
}