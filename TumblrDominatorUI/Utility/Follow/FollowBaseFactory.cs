using DominatorHouseCore.Enums;
using TumblrDominatorCore.Interface;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.TumblrFactory;

namespace TumblrDominatorUI.Utility.Follow
{
    public class FollowBaseFactory : ITumblrBaseFactory
    {
        public ITumblrUtilityFactory TumblrUtilityFactory()
        {
            var utilityFactory = new TumblrUtilityFactory();

            var builder = new TumblrBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Follow)
                .AddReportFactory(new FollowReports())
                .AddViewCampaignFactory(new FollowViewCampaign());

            return builder.TumblrUtilityFactory;
        }
    }
}