using DominatorHouseCore.Enums;
using YoutubeDominatorCore.Interface;
using YoutubeDominatorCore.YDFactories;
using YoutubeDominatorCore.YDUtility;

namespace YoutubeDominatorUI.Utility.CommentScraperUtility
{
    public class CommentScraperBaseFactory : IYdBaseFactory
    {
        public IYdUtilityFactory YdUtilityFactory()
        {
            var utilityFactory = new YdUtilityFactory();

            var builder = new YdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.CommentScraper)
                .AddReportFactory(new CommentScraperReports())
                .AddViewCampaignFactory(new CommentScraperViewCampaign());

            return builder.YdUtilityFactory;
        }
    }
}