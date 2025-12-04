using DominatorHouseCore.Enums;
using RedditDominatorCore.RDFactories;
using RedditDominatorCore.Utility;

namespace RedditDominatorUI.Utility.Voting
{
    public class UpVoteCommentBaseFactory : IRdBaseFactory
    {
        public IRdUtilityFactory RdUtilityFactory()
        {
            var utilityFactory = new RdUtilityFactory();

            var builder = new RdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.UpvoteComment)
                .AddReportFactory(new UpVoteCommentReport())
                .AddViewCampaignFactory(new UpVoteCommentViewCampaign());

            return builder.RdUtilityFactory;
        }
    }
}