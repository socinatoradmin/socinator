using DominatorHouseCore.Enums;
using GramDominatorCore.GDFactories;
using GramDominatorCore.Interface;
using GramDominatorCore.Utility;

namespace GramDominatorUI.Utility.InstaPoster.Reposter
{
    internal class ReposterBaseFactory : IGdBaseFactory
    {
        public IGdUtilityFactory GdUtilityFactory()
        {
            var utilityFactory = new GdUtilityFactory();

            var builder = new GdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Reposter)
                .AddReportFactory(new ReposterReports())
                .AddViewCampaignFactory(new ReposterViewCampaign());

            return builder.GdUtilityFactory;
        }
    }
}