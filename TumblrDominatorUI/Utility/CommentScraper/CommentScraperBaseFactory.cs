using DominatorHouseCore.Enums;
using TumblrDominatorCore.Interface;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.TumblrFactory;
using TumblrDominatorUI.Utility.PostScraper;

namespace TumblrDominatorUI.Utility.CommentScraper
{
    public class CommentScraperBaseFactory : ITumblrBaseFactory
    {
        public ITumblrUtilityFactory TumblrUtilityFactory()
        {
            var utilityFactory = new TumblrUtilityFactory();

            var builder = new TumblrBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.CommentScraper)
                .AddReportFactory(new CommentScraperReports())
                .AddViewCampaignFactory(new CommentScraperViewCampaign());

            return builder.TumblrUtilityFactory;
        }
    }
}