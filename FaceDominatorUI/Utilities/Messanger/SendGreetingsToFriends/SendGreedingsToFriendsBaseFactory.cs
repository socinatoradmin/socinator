using DominatorHouseCore.Enums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;

namespace FaceDominatorUI.Utilities.Messanger.SendGreetingsToFriends
{
    public class SendGreetingsToFriendsBaseFactory : IFdBaseFactory
    {
        public IFdUtilityFactory FdUtilityFactory()
        {
            var utilityFactory = new FdUtilityFactory();

            var builder = new FdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.SendGreetingsToFriends)
                .AddReportFactory(new SendGreetingsToFriendsReport())
                .AddViewCampaignFactory(new SendGreetingsToFriendsViewCampaign());

            return builder.FdUtilityFactory;
        }
    }
}