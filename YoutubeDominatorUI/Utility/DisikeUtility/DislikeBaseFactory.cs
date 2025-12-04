using DominatorHouseCore.Enums;
using YoutubeDominatorCore.Interface;
using YoutubeDominatorCore.YDFactories;
using YoutubeDominatorCore.YDUtility;

namespace YoutubeDominatorUI.Utility.DisikeUtility
{
    public class DislikeBaseFactory : IYdBaseFactory
    {
        public IYdUtilityFactory YdUtilityFactory()
        {
            var utilityFactory = new YdUtilityFactory();

            var builder = new YdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Dislike)
                .AddReportFactory(new DislikeReports())
                .AddViewCampaignFactory(new DislikeViewCampaign());

            return builder.YdUtilityFactory;
        }
    }
}