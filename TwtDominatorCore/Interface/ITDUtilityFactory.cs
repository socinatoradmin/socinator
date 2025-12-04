using DominatorHouseCore.Enums;

namespace TwtDominatorCore.Interface
{
    public interface ITDUtilityFactory
    {
        ActivityType ModuleName { get; set; }

        ITDReportFactory TDReportFactory { get; set; }

        ITDViewCampaign TDViewCampaign { get; set; }
    }
}