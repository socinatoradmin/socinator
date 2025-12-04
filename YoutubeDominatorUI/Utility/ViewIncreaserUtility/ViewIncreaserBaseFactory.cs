using DominatorHouseCore.Enums;
using YoutubeDominatorCore.Interface;
using YoutubeDominatorCore.YDFactories;
using YoutubeDominatorCore.YDUtility;

namespace YoutubeDominatorUI.Utility.ViewIncreaserUtility
{
    public class ViewIncreaserBaseFactory : IYdBaseFactory
    {
        public IYdUtilityFactory YdUtilityFactory()
        {
            var utilityFactory = new YdUtilityFactory();

            var builder = new YdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.ViewIncreaser)
                .AddReportFactory(new ViewIncreaserReports())
                .AddViewCampaignFactory(new ViewIncreaserViewCampaign());

            return builder.YdUtilityFactory;
        }
    }
}