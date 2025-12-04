using DominatorHouseCore.Enums;
using RedditDominatorCore.RDFactories;
using RedditDominatorCore.Utility;

namespace RedditDominatorUI.Utility.Scraper
{
    public class UrlScraperBaseFactory : IRdBaseFactory
    {
        public IRdUtilityFactory RdUtilityFactory()
        {
            var utilityFactory = new RdUtilityFactory();

            var builder = new RdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.UrlScraper)
                .AddReportFactory(new PostScraperReports())
                .AddViewCampaignFactory(new UrlScraperViewCampaign());

            return builder.RdUtilityFactory;
        }
    }
}