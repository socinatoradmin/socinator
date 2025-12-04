using DominatorHouseCore.Enums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;

namespace FaceDominatorUI.Utilities.Scraper.MarketPlaceScaper
{
    // ReSharper disable once UnusedMember.Global
    public class MarketPlaceScraperBaseFactory : IFdBaseFactory
    {
        public IFdUtilityFactory FdUtilityFactory()
        {
            var utilityFactory = new FdUtilityFactory();

            var builder = new FdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.MarketPlaceScraper)
                .AddReportFactory(new MarketPlaceScraperReports())
                .AddViewCampaignFactory(new MarketPlaceScraperViewCampaign());

            return builder.FdUtilityFactory;
        }
    }
}