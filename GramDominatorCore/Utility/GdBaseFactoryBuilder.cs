using DominatorHouseCore.Enums;
using GramDominatorCore.Interface;

namespace GramDominatorCore.Utility
{
    public class GdBaseFactoryBuilder
    {
        public IGdUtilityFactory GdUtilityFactory { get; set; }

        public GdBaseFactoryBuilder(IGdUtilityFactory gdUtilityFactory)
        {
            GdUtilityFactory = gdUtilityFactory;
        }

        public GdBaseFactoryBuilder AddModuleName(ActivityType moduleName)
        {
            GdUtilityFactory.ModuleName = moduleName;
            return this;
        }

        public GdBaseFactoryBuilder AddReportFactory(IGdReportFactory reportFactory)
        {
            GdUtilityFactory.GdReportFactory = reportFactory;
            return this;
        }

        public GdBaseFactoryBuilder AddViewCampaignFactory(IGdViewCampaign gdViewCampaign)
        {
            GdUtilityFactory.GdViewCampaign = gdViewCampaign;
            return this;
        }
    }
}
