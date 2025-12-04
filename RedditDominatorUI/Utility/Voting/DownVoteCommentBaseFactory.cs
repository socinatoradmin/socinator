using DominatorHouseCore.Enums;
using RedditDominatorCore.RDFactories;
using RedditDominatorCore.Utility;

namespace RedditDominatorUI.Utility.Voting
{
    internal class DownVoteCommentBaseFactory : IRdBaseFactory
    {
        public IRdUtilityFactory RdUtilityFactory()
        {
            var utilityFactory = new RdUtilityFactory();

            var builder = new RdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.DownvoteComment)
                .AddReportFactory(new DownVoteCommentReport())
                .AddViewCampaignFactory(new DownVoteCommentViewCampaign());

            return builder.RdUtilityFactory;
        }
    }
}