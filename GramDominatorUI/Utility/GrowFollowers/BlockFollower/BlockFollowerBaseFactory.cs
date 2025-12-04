using DominatorHouseCore.Enums;
using GramDominatorCore.GDFactories;
using GramDominatorCore.Interface;
using GramDominatorCore.Utility;

namespace GramDominatorUI.Utility.GrowFollowers.BlockFollower
{
    internal class BlockFollowerBaseFactory : IGdBaseFactory
    {
        public IGdUtilityFactory GdUtilityFactory()
        {
            var utilityFactory = new GdUtilityFactory();

            var builder = new GdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.BlockFollower)
                .AddReportFactory(new BlockFollowerReports())
                .AddViewCampaignFactory(new BlockFollowerViewCampaign());

            return builder.GdUtilityFactory;
        }
    }
}