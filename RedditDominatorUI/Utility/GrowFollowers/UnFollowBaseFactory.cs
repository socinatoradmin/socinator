using DominatorHouseCore.Enums;
using RedditDominatorCore.RDFactories;
using RedditDominatorCore.Utility;

namespace RedditDominatorUI.Utility.GrowFollowers
{
    public class UnFollowBaseFactory : IRdBaseFactory
    {
        public IRdUtilityFactory RdUtilityFactory()
        {
            var utilityFactory = new RdUtilityFactory();

            var builder = new RdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Unfollow)
                .AddReportFactory(new UnFollowReports())
                .AddViewCampaignFactory(new UnFollowViewCampaign());
            return builder.RdUtilityFactory;
        }
    }
}