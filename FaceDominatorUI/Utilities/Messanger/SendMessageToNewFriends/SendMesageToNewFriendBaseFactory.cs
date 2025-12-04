using DominatorHouseCore.Enums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;

namespace FaceDominatorUI.Utilities.Messanger.SendMessageToNewFriends
{
    public class SendMesageToNewFriendBaseFactory : IFdBaseFactory
    {
        public IFdUtilityFactory FdUtilityFactory()
        {
            var utilityFactory = new FdUtilityFactory();

            var builder = new FdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.SendMessageToNewFriends)
                .AddReportFactory(new SendMessageToNewFriendsReport())
                .AddViewCampaignFactory(new SendMessageToNewFriendsViewCampaign());

            return builder.FdUtilityFactory;
        }
    }
}