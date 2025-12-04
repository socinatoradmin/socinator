using DominatorHouseCore.Enums;
using GramDominatorCore.GDFactories;
using GramDominatorCore.Interface;
using GramDominatorCore.Utility;

namespace GramDominatorUI.Utility.GrowFollowers.Unfollower
{
    internal class UnfollowerBaseFactory : IGdBaseFactory
    {
        public IGdUtilityFactory GdUtilityFactory()
        {
            var utilityFactory = new GdUtilityFactory();

            var builder = new GdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Unfollow)
                .AddReportFactory(new UnfollowerReports())
                .AddViewCampaignFactory(new UnfollowerViewCampaign());

            return builder.GdUtilityFactory;
        }
    }
}