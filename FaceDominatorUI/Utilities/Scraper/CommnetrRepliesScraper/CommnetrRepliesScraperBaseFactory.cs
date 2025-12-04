using DominatorHouseCore.Enums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;

namespace FaceDominatorUI.Utilities.Scraper.CommnetrRepliesScraper
{
    public class CommnetrRepliesScraperBaseFactory : IFdBaseFactory
    {
        public IFdUtilityFactory FdUtilityFactory()
        {
            var utilityFactory = new FdUtilityFactory();

            var builder = new FdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.CommentRepliesScraper)
                .AddReportFactory(new CommnetrRepliesScraperReports())
                .AddViewCampaignFactory(new CommnetrRepliesScraperViewCampaign());

            return builder.FdUtilityFactory;
        }
    }
}