using DominatorHouseCore.Enums;
using RedditDominatorCore.RDFactories;
using RedditDominatorCore.Utility;

namespace RedditDominatorUI.Utility.Voting
{
    public class UpVoteBaseFactory : IRdBaseFactory
    {
        public IRdUtilityFactory RdUtilityFactory()
        {
            var utilityFactory = new RdUtilityFactory();

            var builder = new RdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Upvote)
                .AddReportFactory(new UpVotingReport())
                .AddViewCampaignFactory(new UpVotingViewCampaign());

            return builder.RdUtilityFactory;
        }
    }
}