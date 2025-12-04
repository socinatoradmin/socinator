using DominatorHouseCore.Enums;
using TumblrDominatorCore.Interface;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.TumblrFactory;

namespace TumblrDominatorUI.Utility.UserScraper
{
    public class UserScraperBaseFactory : ITumblrBaseFactory
    {
        public ITumblrUtilityFactory TumblrUtilityFactory()
        {
            var utilityFactory = new TumblrUtilityFactory();

            var builder = new TumblrBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.UserScraper)
                .AddReportFactory(new UserScraperReports())
                .AddViewCampaignFactory(new UserScraperViewCampaign());

            return builder.TumblrUtilityFactory;
        }
    }
}