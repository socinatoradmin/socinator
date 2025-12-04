using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.PDViewModel.Boards;
using static PinDominatorCore.PDEnums.Enums;

namespace PinDominator.PDViews.Boards
{
    public class
        AcceptBoardInvitationBase : ModuleSettingsUserControl<AcceptBoardInvitationViewModel, AcceptBoardInvitationModel
        >
    {
    }

    /// <summary>
    ///     Interaction logic for AcceptBoardInvitation.xaml
    /// </summary>
    public partial class AcceptBoardInvitation
    {
        private static AcceptBoardInvitation _objAcceptBoardInvitation;

        public AcceptBoardInvitation()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: AcceptBoardInvitationHeader,
                footer: AcceptBoardInvitationFooter,
                MainGrid: MainGrid,
                moduleName: PdMainModule.Boards.ToString(),
                activityType: ActivityType.AcceptBoardInvitation
            );
            VideoTutorialLink = ConstantHelpDetails.AcceptBoardInvitationVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.AcceptBoardInvitationKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            DialogParticipation.SetRegister(this, this);
            SetDataContext();
        }

        public static AcceptBoardInvitation GetSingletonObjectAcceptBoardInvitation()
        {
            return _objAcceptBoardInvitation ?? (_objAcceptBoardInvitation = new AcceptBoardInvitation());
        }
    }
}