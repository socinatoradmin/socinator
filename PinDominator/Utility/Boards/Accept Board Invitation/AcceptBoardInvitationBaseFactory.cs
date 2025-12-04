using DominatorHouseCore.Enums;
using PinDominatorCore.Interface;
using PinDominatorCore.PDFactories;
using PinDominatorCore.Utility;

namespace PinDominator.Utility.Boards.Accept_Board_Invitation
{
    public class AcceptBoardInvitationBaseFactory : IPdBaseFactory
    {
        public IPdUtilityFactory PdUtilityFactory()
        {
            var utilityFactory = new PdUtilityFactory();

            var builder = new PdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.AcceptBoardInvitation)
                .AddReportFactory(new AcceptBoardInvitationReports())
                .AddViewCampaignFactory(new AcceptBoardInvitationViewCampaign());

            return builder.PdUtilityFactory;
        }
    }
}