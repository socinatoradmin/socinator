using DominatorHouseCore.Enums;
using YoutubeDominatorCore.Interface;
using YoutubeDominatorCore.YDFactories;
using YoutubeDominatorCore.YDUtility;

namespace YoutubeDominatorUI.Utility.LikeCommentUtility
{
    public class LikeCommentBaseFactory : IYdBaseFactory
    {
        public IYdUtilityFactory YdUtilityFactory()
        {
            var utilityFactory = new YdUtilityFactory();

            var builder = new YdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.LikeComment)
                .AddReportFactory(new LikeCommentReports())
                .AddViewCampaignFactory(new LikeCommentViewCampaign());

            return builder.YdUtilityFactory;
        }
    }
}