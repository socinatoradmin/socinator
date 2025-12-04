using DominatorHouseCore.Enums;
using GramDominatorCore.GDFactories;
using GramDominatorCore.Interface;
using GramDominatorCore.Utility;

namespace GramDominatorUI.Utility.InstaScrape.UserScraper
{
    internal class UserScraperBaseFactory : IGdBaseFactory
    {
        public IGdUtilityFactory GdUtilityFactory()
        {
            var utilityFactory = new GdUtilityFactory();

            var builder = new GdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.UserScraper)
                .AddReportFactory(new UserScraperReports())
                .AddViewCampaignFactory(new UserScraperViewCampaign());

            return builder.GdUtilityFactory;
        }
    }
}