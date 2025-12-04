using DominatorHouseCore.Enums;
using PinDominatorCore.Interface;
using PinDominatorCore.PDFactories;
using PinDominatorCore.Utility;

namespace PinDominator.Utility.Boards.Send_Board_Invitation
{
    public class SendBoardInvitationBaseFactory : IPdBaseFactory
    {
        public IPdUtilityFactory PdUtilityFactory()
        {
            var utilityFactory = new PdUtilityFactory();

            var builder = new PdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.SendBoardInvitation)
                .AddReportFactory(new SendBoardInvitationReports())
                .AddViewCampaignFactory(new SendBoardInvitationViewCampaign());

            return builder.PdUtilityFactory;
        }
    }
}