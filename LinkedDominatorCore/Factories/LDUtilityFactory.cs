using DominatorHouseCore.Enums;
using LinkedDominatorCore.Interfaces;

namespace LinkedDominatorCore.Factories
{
    public class LdUtilityFactory : ILdUtilityFactory
    {
        public ActivityType ModuleName { get; set; }

        public ILdReportFactory LdReportFactory { get; set; }

        public ILdViewCampaign LdViewCampaign { get; set; }
    }
}