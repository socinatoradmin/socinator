using DominatorHouseCore.Enums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;

namespace FaceDominatorUI.Utilities.Scraper.CommentScraper
{
    public class CommentScraperBaseFactory : IFdBaseFactory
    {
        public IFdUtilityFactory FdUtilityFactory()
        {
            var utilityFactory = new FdUtilityFactory();

            var builder = new FdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.CommentScraper)
                .AddReportFactory(new CommentScraperReports())
                .AddViewCampaignFactory(new CommentScraperViewCampaign());

            return builder.FdUtilityFactory;
        }
    }
}