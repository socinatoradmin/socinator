using DominatorHouseCore.Enums;
using RedditDominatorCore.RDFactories;
using RedditDominatorCore.Utility;

namespace RedditDominatorUI.Utility.Engage
{
    public class ReplyBaseFactory : IRdBaseFactory
    {
        public IRdUtilityFactory RdUtilityFactory()
        {
            var utilityFactory = new RdUtilityFactory();

            var builder = new RdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Reply)
                .AddReportFactory(new ReplyReport())
                .AddViewCampaignFactory(new ReplyViewCampaign());

            return builder.RdUtilityFactory;
        }
    }
}