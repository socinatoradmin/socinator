using DominatorHouseCore.Enums;
using YoutubeDominatorCore.Interface;
using YoutubeDominatorCore.YDFactories;
using YoutubeDominatorCore.YDUtility;

namespace YoutubeDominatorUI.Utility.PostScraperUtility
{
    public class PostScraperBaseFactory : IYdBaseFactory
    {
        public IYdUtilityFactory YdUtilityFactory()
        {
            var utilityFactory = new YdUtilityFactory();

            var builder = new YdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.PostScraper)
                .AddReportFactory(new PostScraperReports())
                .AddViewCampaignFactory(new PostScraperViewCampaign());

            return builder.YdUtilityFactory;
        }
    }
}