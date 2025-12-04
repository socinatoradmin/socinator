using DominatorHouseCore.Enums;

namespace RedditDominatorCore.Utility
{
    public class RdBaseFactoryBuilder
    {
        public RdBaseFactoryBuilder(IRdUtilityFactory rdUtilityFactory)
        {
            RdUtilityFactory = rdUtilityFactory;
        }

        public IRdUtilityFactory RdUtilityFactory { get; set; }

        public RdBaseFactoryBuilder AddModuleName(ActivityType moduleName)
        {
            RdUtilityFactory.ModuleName = moduleName;
            return this;
        }

        public RdBaseFactoryBuilder AddReportFactory(IRdReportFactory reportFactory)
        {
            RdUtilityFactory.RdReportFactory = reportFactory;
            return this;
        }

        public RdBaseFactoryBuilder AddViewCampaignFactory(IRdViewCampaign rdViewCampaign)
        {
            RdUtilityFactory.RdViewCampaign = rdViewCampaign;
            return this;
        }
    }
}