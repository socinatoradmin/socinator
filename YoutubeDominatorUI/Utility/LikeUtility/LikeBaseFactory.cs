using DominatorHouseCore.Enums;
using YoutubeDominatorCore.Interface;
using YoutubeDominatorCore.YDFactories;
using YoutubeDominatorCore.YDUtility;

namespace YoutubeDominatorUI.Utility.LikeUtility
{
    public class LikeBaseFactory : IYdBaseFactory
    {
        public IYdUtilityFactory YdUtilityFactory()
        {
            var utilityFactory = new YdUtilityFactory();

            var builder = new YdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Like)
                .AddReportFactory(new LikeReports())
                .AddViewCampaignFactory(new LikeViewCampaign());

            return builder.YdUtilityFactory;
        }
    }
}