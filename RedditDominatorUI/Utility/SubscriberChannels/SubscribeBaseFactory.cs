using DominatorHouseCore.Enums;
using RedditDominatorCore.RDFactories;
using RedditDominatorCore.Utility;

namespace RedditDominatorUI.Utility.SubscriberChannels
{
    internal class SubscribeBaseFactory : IRdBaseFactory
    {
        public IRdUtilityFactory RdUtilityFactory()
        {
            var utilityFactory = new RdUtilityFactory();

            var builder = new RdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Subscribe)
                .AddReportFactory(new SubscribeReport())
                .AddViewCampaignFactory(new SubscriberViewCampaign());

            return builder.RdUtilityFactory;
        }
    }
}