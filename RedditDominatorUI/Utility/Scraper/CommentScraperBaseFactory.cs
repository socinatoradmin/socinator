using DominatorHouseCore.Enums;
using RedditDominatorCore.RDFactories;
using RedditDominatorCore.Utility;

namespace RedditDominatorUI.Utility.Scraper
{
    public class CommentScraperBaseFactory : IRdBaseFactory
    {
        public IRdUtilityFactory RdUtilityFactory()
        {
            var utilityFactory = new RdUtilityFactory();

            var builder = new RdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.CommentScraper)
                .AddReportFactory(new CommentScraperReport())
                .AddViewCampaignFactory(new CommentScraperViewCampaign());

            return builder.RdUtilityFactory;
        }
    }
}