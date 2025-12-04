using DominatorHouseCore.Enums;
using LinkedDominatorCore.Interfaces;

namespace LinkedDominatorCore.Utility
{
    public class LdBaseFactoryBuilder
    {
        public LdBaseFactoryBuilder(ILdUtilityFactory ldUtilityFactory)
        {
            LdUtilityFactory = ldUtilityFactory;
        }

        public ILdUtilityFactory LdUtilityFactory { get; set; }

        public LdBaseFactoryBuilder AddModuleName(ActivityType moduleName)
        {
            LdUtilityFactory.ModuleName = moduleName;
            return this;
        }

        public LdBaseFactoryBuilder AddReportFactory(ILdReportFactory reportFactory)
        {
            LdUtilityFactory.LdReportFactory = reportFactory;
            return this;
        }

        public LdBaseFactoryBuilder AddViewCampaignFactory(ILdViewCampaign LdViewCampaign)
        {
            LdUtilityFactory.LdViewCampaign = LdViewCampaign;
            return this;
        }
    }
}