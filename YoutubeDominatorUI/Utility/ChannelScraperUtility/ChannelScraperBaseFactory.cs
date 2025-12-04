using DominatorHouseCore.Enums;
using YoutubeDominatorCore.Interface;
using YoutubeDominatorCore.YDFactories;
using YoutubeDominatorCore.YDUtility;

namespace YoutubeDominatorUI.Utility.ChannelScraperUtility
{
    public class ChannelScraperBaseFactory : IYdBaseFactory
    {
        public IYdUtilityFactory YdUtilityFactory()
        {
            var utilityFactory = new YdUtilityFactory();

            var builder = new YdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.ChannelScraper)
                .AddReportFactory(new ChannelScraperReports())
                .AddViewCampaignFactory(new ChannelScraperViewCampaign());

            return builder.YdUtilityFactory;
        }
    }
}