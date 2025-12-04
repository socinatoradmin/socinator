using DominatorHouseCore.Enums;
using PinDominatorCore.Interface;
using PinDominatorCore.PDFactories;
using PinDominatorCore.Utility;

namespace PinDominator.Utility.Pin_Messanger.Broadcast_Messages
{
    public class BroadCastMessagesBaseFactory : IPdBaseFactory
    {
        public IPdUtilityFactory PdUtilityFactory()
        {
            var utilityFactory = new PdUtilityFactory();

            var builder = new PdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.BroadcastMessages)
                .AddReportFactory(new BroadCastMessagesReports())
                .AddViewCampaignFactory(new BroadCastMessagesViewCampaigns());

            return builder.PdUtilityFactory;
        }
    }
}