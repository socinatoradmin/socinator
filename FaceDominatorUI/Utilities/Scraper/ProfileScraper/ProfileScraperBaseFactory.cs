using DominatorHouseCore.Enums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;

namespace FaceDominatorUI.Utilities.Scraper.ProfileScraper
{
    public class ProfileScraperBaseFactory : IFdBaseFactory
    {
        public IFdUtilityFactory FdUtilityFactory()
        {
            var utilityFactory = new FdUtilityFactory();

            var builder = new FdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.ProfileScraper)
                .AddReportFactory(new ProfileScraperReports())
                .AddViewCampaignFactory(new ProfileScraperViewCampaign());

            return builder.FdUtilityFactory;
        }
    }
}