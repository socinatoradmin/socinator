using DominatorHouseCore.Enums;
using RedditDominatorCore.RDFactories;
using RedditDominatorCore.Utility;

namespace RedditDominatorUI.Utility.Messanger
{
    class AutoReplyBaseFactory : IRdBaseFactory
    {
        public IRdUtilityFactory RdUtilityFactory()
        {
            var utilityFactory = new RdUtilityFactory();

            var builder = new RdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.AutoReplyToNewMessage)
                .AddReportFactory(new AutoReplyReports())
                .AddViewCampaignFactory(new AutoReplyViewCampaign());

            return builder.RdUtilityFactory;
        }
    }
}
