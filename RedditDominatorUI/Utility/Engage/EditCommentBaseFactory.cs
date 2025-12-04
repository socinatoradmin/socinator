using DominatorHouseCore.Enums;
using RedditDominatorCore.RDFactories;
using RedditDominatorCore.Utility;

namespace RedditDominatorUI.Utility.Engage
{
    public class EditCommentBaseFactory : IRdBaseFactory
    {
        public IRdUtilityFactory RdUtilityFactory()
        {
            var utilityFactory = new RdUtilityFactory();

            var builder = new RdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.EditComment)
                .AddReportFactory(new EditCommentReport())
                .AddViewCampaignFactory(new EditCommentViewCampaign());

            return builder.RdUtilityFactory;
        }
    }
}