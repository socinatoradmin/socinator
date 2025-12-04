using DominatorHouseCore.Enums;
using TwtDominatorCore.Interface;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorUI.Utility.GrowFollowersReportPack.FollowBackPack
{
    public class FollowBackBaseFactory : ITDBaseFactory
    {
        public ITDUtilityFactory TDUtilityFactory()
        {
            var utilityFactory = new TDUtilityFactory();

            var builder = new TDBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.FollowBack)
                .AddReportFactory(new FollowBackReport())
                .AddViewCampaignFactory(new FollowBackViewCampaign());

            return builder.TDUtilityFactory;
        }
    }
}