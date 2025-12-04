using DominatorHouseCore.Enums;
using PinDominatorCore.Interface;
using PinDominatorCore.PDFactories;
using PinDominatorCore.Utility;

namespace PinDominator.Utility.Grow_Followers.Unfollow
{
    public class UnfollowBaseFactory : IPdBaseFactory
    {
        public IPdUtilityFactory PdUtilityFactory()
        {
            var utilityFactory = new PdUtilityFactory();

            var builder = new PdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Unfollow)
                .AddReportFactory(new UnfollowReports())
                .AddViewCampaignFactory(new UnfollowViewCampaign());

            return builder.PdUtilityFactory;
        }
    }
}