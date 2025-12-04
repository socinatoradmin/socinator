using DominatorHouseCore.Enums;
using TumblrDominatorCore.Interface;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.TumblrFactory;

namespace TumblrDominatorUI.Utility.Unlike
{
    public class UnLikeBaseFactory : ITumblrBaseFactory
    {
        public ITumblrUtilityFactory TumblrUtilityFactory()
        {
            var utilityFactory = new TumblrUtilityFactory();

            var builder = new TumblrBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Unlike)
                .AddReportFactory(new UnLikeReports())
                .AddViewCampaignFactory(new UnLikeViewCampaign());

            return builder.TumblrUtilityFactory;
        }
    }
}