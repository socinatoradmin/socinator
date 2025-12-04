using DominatorHouseCore.Enums;
using PinDominatorCore.Interface;
using PinDominatorCore.PDFactories;
using PinDominatorCore.Utility;

namespace PinDominator.Utility.Grow_Followers.Follow_Back
{
    public class FollowBackBaseFactory : IPdBaseFactory
    {
        public IPdUtilityFactory PdUtilityFactory()
        {
            var utilityFactory = new PdUtilityFactory();

            var builder = new PdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.FollowBack)
                .AddReportFactory(new FollowBackReports())
                .AddViewCampaignFactory(new FollowBackViewCampaign());

            return builder.PdUtilityFactory;
        }
    }
}