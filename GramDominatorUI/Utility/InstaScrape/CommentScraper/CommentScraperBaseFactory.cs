using DominatorHouseCore.Enums;
using GramDominatorCore.GDFactories;
using GramDominatorCore.Interface;
using GramDominatorCore.Utility;

namespace GramDominatorUI.Utility.InstaScrape.CommentScraper
{
    public class CommentScraperBaseFactory : IGdBaseFactory
    {
        public IGdUtilityFactory GdUtilityFactory()
        {
            var utilityFactory = new GdUtilityFactory();

            var builder = new GdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.CommentScraper)
                .AddReportFactory(new CommentScraperReports())
                .AddViewCampaignFactory(new CommentScraperViewCampaign());

            return builder.GdUtilityFactory;
        }
    }
}