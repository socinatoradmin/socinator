using DominatorHouseCore.Enums;
using RedditDominatorCore.RDFactories;
using RedditDominatorCore.Utility;

namespace RedditDominatorUI.Utility.Voting
{
    internal class RemoveVoteCommentBaseFactory : IRdBaseFactory
    {
        public IRdUtilityFactory RdUtilityFactory()
        {
            var utilityFactory = new RdUtilityFactory();

            var builder = new RdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.RemoveVoteComment)
                .AddReportFactory(new RemoveVoteCommentReport())
                .AddViewCampaignFactory(new RemoveCommentVotingViewCampaign());

            return builder.RdUtilityFactory;
        }
    }
}