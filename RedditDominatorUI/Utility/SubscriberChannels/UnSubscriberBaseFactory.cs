using DominatorHouseCore.Enums;
using RedditDominatorCore.RDFactories;
using RedditDominatorCore.Utility;

namespace RedditDominatorUI.Utility.SubscriberChannels
{
    internal class UnSubscriberBaseFactory : IRdBaseFactory
    {
        public IRdUtilityFactory RdUtilityFactory()
        {
            var utilityFactory = new RdUtilityFactory();

            var builder = new RdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Unsubscribe)
                .AddReportFactory(new UnSubscribeReport())
                .AddViewCampaignFactory(new UnSubscriberViewCampaign());

            return builder.RdUtilityFactory;
        }
    }
}