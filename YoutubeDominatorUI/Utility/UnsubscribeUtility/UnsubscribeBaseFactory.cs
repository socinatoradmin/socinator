using DominatorHouseCore.Enums;
using YoutubeDominatorCore.Interface;
using YoutubeDominatorCore.YDFactories;
using YoutubeDominatorCore.YDUtility;

namespace YoutubeDominatorUI.Utility.UnsubscribeUtility
{
    public class UnsubscribeBaseFactory : IYdBaseFactory
    {
        public IYdUtilityFactory YdUtilityFactory()
        {
            var utilityFactory = new YdUtilityFactory();

            var builder = new YdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.UnSubscribe)
                .AddReportFactory(new UnsubscribeReports())
                .AddViewCampaignFactory(new UnsubscribeViewCampaign());

            return builder.YdUtilityFactory;
        }
    }
}