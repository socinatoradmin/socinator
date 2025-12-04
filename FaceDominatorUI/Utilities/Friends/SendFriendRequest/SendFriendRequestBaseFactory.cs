using DominatorHouseCore.Enums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;

namespace FaceDominatorUI.Utilities.Friends.SendFriendRequest
{
    public class SendFriendRequestBaseFactory : IFdBaseFactory
    {
        public IFdUtilityFactory FdUtilityFactory()
        {
            var utilityFactory = new FdUtilityFactory();

            var builder = new FdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.SendFriendRequest)
                .AddReportFactory(new SendFriendRequestReports())
                .AddViewCampaignFactory(new SendFriendRequestViewCampaigny());

            return builder.FdUtilityFactory;
        }
    }
}