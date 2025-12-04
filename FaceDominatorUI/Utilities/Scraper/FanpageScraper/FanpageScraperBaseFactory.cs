using DominatorHouseCore.Enums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;

namespace FaceDominatorUI.Utilities.Scraper.FanpageScraper
{
    public class FanpageScraperBaseFactory : IFdBaseFactory
    {
        public IFdUtilityFactory FdUtilityFactory()
        {
            var utilityFactory = new FdUtilityFactory();

            var builder = new FdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.FanpageScraper)
                .AddReportFactory(new FanpageScraperReports())
                .AddViewCampaignFactory(new FanpageScraperViewCampaign());

            return builder.FdUtilityFactory;
        }
    }
}