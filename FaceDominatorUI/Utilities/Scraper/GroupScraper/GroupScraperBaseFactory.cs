using DominatorHouseCore.Enums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;

namespace FaceDominatorUI.Utilities.Scraper.GroupScraper
{
    public class GroupScraperBaseFactory : IFdBaseFactory
    {
        public IFdUtilityFactory FdUtilityFactory()
        {
            var utilityFactory = new FdUtilityFactory();

            var builder = new FdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.GroupScraper)
                .AddReportFactory(new GroupScraperReports())
                .AddViewCampaignFactory(new GroupScraperViewCampaign());

            return builder.FdUtilityFactory;
        }
    }
}