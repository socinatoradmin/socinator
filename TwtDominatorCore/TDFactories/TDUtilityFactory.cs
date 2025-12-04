using DominatorHouseCore.Enums;
using TwtDominatorCore.Interface;

namespace TwtDominatorCore.TDFactories
{
    public class TDUtilityFactory : ITDUtilityFactory
    {
        public ActivityType ModuleName { get; set; }

        public ITDReportFactory TDReportFactory { get; set; }

        public ITDViewCampaign TDViewCampaign { get; set; }
    }
}