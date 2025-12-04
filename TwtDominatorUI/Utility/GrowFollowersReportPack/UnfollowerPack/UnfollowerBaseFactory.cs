using DominatorHouseCore.Enums;
using TwtDominatorCore.Interface;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorUI.Utility.GrowFollowersReportPack.UnfollowerPack
{
    public class UnfollowerBaseFactory : ITDBaseFactory
    {
        public ITDUtilityFactory TDUtilityFactory()
        {
            var utilityFactory = new TDUtilityFactory();

            var builder = new TDBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Unfollow)
                .AddReportFactory(new UnfollowerReport())
                .AddViewCampaignFactory(new UnfollowerViewCampaign());

            return builder.TDUtilityFactory;
        }
    }
}