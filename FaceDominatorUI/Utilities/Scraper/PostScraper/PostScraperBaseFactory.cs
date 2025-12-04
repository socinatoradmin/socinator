using DominatorHouseCore.Enums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;

namespace FaceDominatorUI.Utilities.Scraper.PostScraper
{
    public class PostScraperBaseFactory : IFdBaseFactory
    {
        public IFdUtilityFactory FdUtilityFactory()
        {
            var utilityFactory = new FdUtilityFactory();

            var builder = new FdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.PostScraper)
                .AddReportFactory(new PostScraperReports())
                .AddViewCampaignFactory(new PostScraperViewCampaign());

            return builder.FdUtilityFactory;
        }
    }
}