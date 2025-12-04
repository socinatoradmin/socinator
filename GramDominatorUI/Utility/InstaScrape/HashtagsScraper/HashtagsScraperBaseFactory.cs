using DominatorHouseCore.Enums;
using GramDominatorCore.GDFactories;
using GramDominatorCore.Interface;
using GramDominatorCore.Utility;

namespace GramDominatorUI.Utility.InstaScrape.HashtagsScraper
{
    internal class HashtagsScraperBaseFactory : IGdBaseFactory
    {
        public IGdUtilityFactory GdUtilityFactory()
        {
            var utilityFactory = new GdUtilityFactory();

            var builder = new GdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.HashtagsScraper)
                .AddReportFactory(new HashtagsScraperReports())
                .AddViewCampaignFactory(new HashtagsScraperViewCampaign());

            return builder.GdUtilityFactory;
        }
    }
}