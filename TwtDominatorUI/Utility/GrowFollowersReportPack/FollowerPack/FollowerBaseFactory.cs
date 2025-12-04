using DominatorHouseCore.Enums;
using TwtDominatorCore.Interface;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorUI.Utility.GrowFollowersReportPack.FollowerPack
{
    public class FollowerBaseFactory : ITDBaseFactory
    {
        public ITDUtilityFactory TDUtilityFactory()
        {
            var utilityFactory = new TDUtilityFactory();

            var builder = new TDBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Follow)
                .AddReportFactory(new FollowerReport())
                .AddViewCampaignFactory(new FollowerViewCampaign());

            return builder.TDUtilityFactory;
        }
    }
}