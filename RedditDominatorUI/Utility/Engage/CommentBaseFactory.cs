using DominatorHouseCore.Enums;
using RedditDominatorCore.RDFactories;
using RedditDominatorCore.Utility;

namespace RedditDominatorUI.Utility.Engage
{
    public class CommentBaseFactory : IRdBaseFactory
    {
        public IRdUtilityFactory RdUtilityFactory()
        {
            var utilityFactory = new RdUtilityFactory();

            var builder = new RdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Comment)
                .AddReportFactory(new CommentReport())
                .AddViewCampaignFactory(new CommentViewCampaign());

            return builder.RdUtilityFactory;
        }
    }
}