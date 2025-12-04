using DominatorHouseCore.Enums;
using YoutubeDominatorCore.Interface;
using YoutubeDominatorCore.YDFactories;
using YoutubeDominatorCore.YDUtility;

namespace YoutubeDominatorUI.Utility.SubscribeUtility
{
    public class SubscribeBaseFactory : IYdBaseFactory
    {
        public IYdUtilityFactory YdUtilityFactory()
        {
            var utilityFactory = new YdUtilityFactory();

            var builder = new YdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Subscribe)
                .AddReportFactory(new SubscribeReports())
                .AddViewCampaignFactory(new SubscribeViewCampaign());

            return builder.YdUtilityFactory;
        }
    }
}