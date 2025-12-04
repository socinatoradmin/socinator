using DominatorHouseCore.Enums;
using PinDominatorCore.Interface;

namespace PinDominatorCore.Utility
{
    public class PdBaseFactoryBuilder
    {
        public PdBaseFactoryBuilder(IPdUtilityFactory pdUtilityFactory)
        {
            PdUtilityFactory = pdUtilityFactory;
        }

        public IPdUtilityFactory PdUtilityFactory { get; set; }

        public PdBaseFactoryBuilder AddModuleName(ActivityType moduleName)
        {
            PdUtilityFactory.ModuleName = moduleName;
            return this;
        }

        public PdBaseFactoryBuilder AddReportFactory(IPdReportFactory reportFactory)
        {
            PdUtilityFactory.PdReportFactory = reportFactory;
            return this;
        }

        public PdBaseFactoryBuilder AddViewCampaignFactory(IPdViewCampaign pdViewCampaign)
        {
            PdUtilityFactory.PdViewCampaign = pdViewCampaign;
            return this;
        }
    }
}