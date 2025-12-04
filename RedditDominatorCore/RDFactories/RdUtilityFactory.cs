using DominatorHouseCore.Enums;
using RedditDominatorCore.Utility;

namespace RedditDominatorCore.RDFactories
{
    public class RdUtilityFactory : IRdUtilityFactory
    {
        public ActivityType ModuleName { get; set; }

        public IRdReportFactory RdReportFactory { get; set; }

        public IRdViewCampaign RdViewCampaign { get; set; }
    }
}