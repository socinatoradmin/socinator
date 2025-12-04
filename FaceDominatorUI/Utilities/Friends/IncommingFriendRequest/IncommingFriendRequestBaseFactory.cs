using DominatorHouseCore.Enums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;

namespace FaceDominatorUI.Utilities.Friends.IncommingFriendRequest
{
    public class IncommingFriendRequestBaseFactory : IFdBaseFactory
    {
        public IFdUtilityFactory FdUtilityFactory()
        {
            var utilityFactory = new FdUtilityFactory();

            var builder = new FdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.IncommingFriendRequest)
                .AddReportFactory(new IncommingFriendRequestReports())
                .AddViewCampaignFactory(new IncommingFriendRequestViewCampaign());

            return builder.FdUtilityFactory;
        }
    }
}