using DominatorHouseCore.Enums;
using RedditDominatorCore.RDFactories;
using RedditDominatorCore.Utility;

namespace RedditDominatorUI.Utility.GrowFollowers
{
    public class FollowBaseFactory : IRdBaseFactory
    {
        public IRdUtilityFactory RdUtilityFactory()
        {
            var utilityFactory = new RdUtilityFactory();

            var builder = new RdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Follow)
                .AddReportFactory(new FollowReports())
                .AddViewCampaignFactory(new FollowViewCampaign());

            return builder.RdUtilityFactory;
        }
    }
}