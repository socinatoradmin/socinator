using DominatorHouseCore.Enums;
using RedditDominatorCore.RDFactories;
using RedditDominatorCore.Utility;

namespace RedditDominatorUI.Utility.Scraper
{
    internal class ChannelScraperBaseFactory : IRdBaseFactory
    {
        public IRdUtilityFactory RdUtilityFactory()
        {
            var utilityFactory = new RdUtilityFactory();

            var builder = new RdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.ChannelScraper)
                .AddReportFactory(new ChannelScraperReport())
                .AddViewCampaignFactory(new ChannelScraperViewCampaign());

            return builder.RdUtilityFactory;
        }
    }
}