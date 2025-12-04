using DominatorHouseCore.Enums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;

namespace FaceDominatorUI.Utilities.Messanger.SendGreetingsToFriends
{
    public class MessageToFanpagesBaseFactory : IFdBaseFactory
    {
        public IFdUtilityFactory FdUtilityFactory()
        {
            var utilityFactory = new FdUtilityFactory();

            var builder = new FdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.MessageToFanpages)
                .AddReportFactory(new MessageToFanpagesReport())
                .AddViewCampaignFactory(new MessageToFanpagesViewCampaign());

            return builder.FdUtilityFactory;
        }
    }
}