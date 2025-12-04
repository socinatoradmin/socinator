using DominatorHouseCore.Enums;
using GramDominatorCore.GDFactories;
using GramDominatorCore.Interface;
using GramDominatorCore.Utility;

namespace GramDominatorUI.Utility.InstaScrape.DownloadPhotos
{
    internal class DownloadPhotosBaseFactory : IGdBaseFactory
    {
        public IGdUtilityFactory GdUtilityFactory()
        {
            var utilityFactory = new GdUtilityFactory();

            var builder = new GdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.PostScraper)
                .AddReportFactory(new DownloadPhotosReports())
                .AddViewCampaignFactory(new DownloadPhotosViewCampaign());

            return builder.GdUtilityFactory;
        }
    }
}