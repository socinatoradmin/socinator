using DominatorHouseCore.Enums;
using RedditDominatorCore.RDFactories;
using RedditDominatorCore.Utility;

namespace RedditDominatorUI.Utility.Voting
{
    internal class DownVotingBaseFactory : IRdBaseFactory
    {
        public IRdUtilityFactory RdUtilityFactory()
        {
            var utilityFactory = new RdUtilityFactory();

            var builder = new RdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Downvote)
                .AddReportFactory(new DownVotingReports())
                .AddViewCampaignFactory(new DownVotingViewCampaign());

            return builder.RdUtilityFactory;
        }
    }
}