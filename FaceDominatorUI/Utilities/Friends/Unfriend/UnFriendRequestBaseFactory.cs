using DominatorHouseCore.Enums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;

namespace FaceDominatorUI.Utilities.Friends.Unfriend
{
    public class UnFriendRequestBaseFactory : IFdBaseFactory
    {
        public IFdUtilityFactory FdUtilityFactory()
        {
            var utilityFactory = new FdUtilityFactory();

            var builder = new FdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Unfriend)
                .AddReportFactory(new UnFriendRequestReports())
                .AddViewCampaignFactory(new UnFriendRequestViewCampaign());

            return builder.FdUtilityFactory;
        }
    }
}