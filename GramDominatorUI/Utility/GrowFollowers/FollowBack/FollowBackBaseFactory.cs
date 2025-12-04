using DominatorHouseCore.Enums;
using GramDominatorCore.GDFactories;
using GramDominatorCore.Interface;
using GramDominatorCore.Utility;

namespace GramDominatorUI.Utility.GrowFollowers.FollowBack
{
    internal class FollowBackBaseFactory : IGdBaseFactory
    {
        public IGdUtilityFactory GdUtilityFactory()
        {
            var utilityFactory = new GdUtilityFactory();

            var builder = new GdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.FollowBack)
                .AddReportFactory(new FollowBackReports())
                .AddViewCampaignFactory(new FollowBackViewCampaign());

            return builder.GdUtilityFactory;
        }
    }
}