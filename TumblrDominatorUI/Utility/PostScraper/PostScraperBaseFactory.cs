using DominatorHouseCore.Enums;
using TumblrDominatorCore.Interface;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.TumblrFactory;

namespace TumblrDominatorUI.Utility.PostScraper
{
    public class PostScraperBaseFactory : ITumblrBaseFactory
    {
        public ITumblrUtilityFactory TumblrUtilityFactory()
        {
            var utilityFactory = new TumblrUtilityFactory();

            var builder = new TumblrBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.PostScraper)
                .AddReportFactory(new PostScraperReports())
                .AddViewCampaignFactory(new PostScraperViewCampaign());

            return builder.TumblrUtilityFactory;
        }
    }
}