using DominatorHouseCore.Enums;
using GramDominatorCore.GDFactories;
using GramDominatorCore.Interface;
using GramDominatorCore.Utility;

namespace GramDominatorUI.Utility.GrowFollowers.Follower
{
    public class FollowerBaseFactory : IGdBaseFactory
    {
        public IGdUtilityFactory GdUtilityFactory()
        {
            var utilityFactory = new GdUtilityFactory();

            var builder = new GdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Follow)
                .AddReportFactory(new FollowerReports())
                .AddViewCampaignFactory(new FollowerViewCampaign());

            return builder.GdUtilityFactory;
        }
    }
}