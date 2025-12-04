using DominatorHouseCore.Enums;
using TumblrDominatorCore.Interface;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.TumblrFactory;

namespace TumblrDominatorUI.Utility.Like
{
    public class LikeBaseFactory : ITumblrBaseFactory
    {
        public ITumblrUtilityFactory TumblrUtilityFactory()
        {
            var utilityFactory = new TumblrUtilityFactory();

            var builder = new TumblrBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Like)
                .AddReportFactory(new LikeReports())
                .AddViewCampaignFactory(new LikeViewCampaign());

            return builder.TumblrUtilityFactory;
        }
    }
}