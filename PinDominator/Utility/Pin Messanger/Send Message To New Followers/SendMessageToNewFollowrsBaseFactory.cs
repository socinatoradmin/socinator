using DominatorHouseCore.Enums;
using PinDominatorCore.Interface;
using PinDominatorCore.PDFactories;
using PinDominatorCore.Utility;

namespace PinDominator.Utility.Pin_Messanger.Send_Message_To_New_Followers
{
    public class SendMessageToNewFollowrsBaseFactory : IPdBaseFactory
    {
        public IPdUtilityFactory PdUtilityFactory()
        {
            var utilityFactory = new PdUtilityFactory();

            var builder = new PdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.SendMessageToFollower)
                .AddReportFactory(new SendMessageToNewFollowrsReports())
                .AddViewCampaignFactory(new SendMessageToNewFollowrsViewCampaigns());

            return builder.PdUtilityFactory;
        }
    }
}