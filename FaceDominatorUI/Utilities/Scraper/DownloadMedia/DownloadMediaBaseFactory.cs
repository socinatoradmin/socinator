using DominatorHouseCore.Enums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;

namespace FaceDominatorUI.Utilities.Scraper.DownloadMedia
{
    public class DownloadMediaBaseFactory : IFdBaseFactory
    {
        public IFdUtilityFactory FdUtilityFactory()
        {
            var utilityFactory = new FdUtilityFactory();

            var builder = new FdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.DownloadScraper)
                .AddReportFactory(new DownloadMediaReports())
                .AddViewCampaignFactory(new DownloadMediaViewCampaign());

            return builder.FdUtilityFactory;
        }
    }
}