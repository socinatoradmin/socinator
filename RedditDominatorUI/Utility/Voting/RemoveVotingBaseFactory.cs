using DominatorHouseCore.Enums;
using RedditDominatorCore.RDFactories;
using RedditDominatorCore.Utility;

namespace RedditDominatorUI.Utility.Voting
{
    internal class RemoveVotingBaseFactory : IRdBaseFactory
    {
        public IRdUtilityFactory RdUtilityFactory()
        {
            var utilityFactory = new RdUtilityFactory();

            var builder = new RdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.RemoveVote)
                .AddReportFactory(new RemoveVotingReport())
                .AddViewCampaignFactory(new RemoveVotingViewCampaign());

            return builder.RdUtilityFactory;
        }
    }
}