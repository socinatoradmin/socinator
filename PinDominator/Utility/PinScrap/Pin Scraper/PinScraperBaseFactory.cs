using DominatorHouseCore.Enums;
using PinDominatorCore.Interface;
using PinDominatorCore.PDFactories;
using PinDominatorCore.Utility;

namespace PinDominator.Utility.PinScrap.Pin_Scraper
{
    public class PinScraperBaseFactory : IPdBaseFactory
    {
        public IPdUtilityFactory PdUtilityFactory()
        {
            var utilityFactory = new PdUtilityFactory();

            var builder = new PdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.PinScraper)
                .AddReportFactory(new PinScraperReports())
                .AddViewCampaignFactory(new PinScraperViewCampaigns());

            return builder.PdUtilityFactory;
        }
    }
}