using DominatorHouseCore.Enums;
using PinDominatorCore.Interface;
using PinDominatorCore.PDFactories;
using PinDominatorCore.Utility;

namespace PinDominator.Utility.PinScrap.Board_Scraper
{
    public class BoardScraperBaseFactory : IPdBaseFactory
    {
        public IPdUtilityFactory PdUtilityFactory()
        {
            var utilityFactory = new PdUtilityFactory();

            var builder = new PdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.BoardScraper)
                .AddReportFactory(new BoardScraperReports())
                .AddViewCampaignFactory(new BoardScraperViewCampagns());

            return builder.PdUtilityFactory;
        }
    }
}