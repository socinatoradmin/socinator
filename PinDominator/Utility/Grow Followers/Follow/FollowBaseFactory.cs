using DominatorHouseCore.Enums;
using PinDominatorCore.Interface;
using PinDominatorCore.PDFactories;
using PinDominatorCore.Utility;

namespace PinDominator.Utility.Grow_Followers.Follow
{
    public class FollowBaseFactory : IPdBaseFactory
    {
        public IPdUtilityFactory PdUtilityFactory()
        {
            var utilityFactory = new PdUtilityFactory();

            var builder = new PdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Follow)
                .AddReportFactory(new FollowReports())
                .AddViewCampaignFactory(new FollowViewCampaign());

            return builder.PdUtilityFactory;
        }
    }
}