using System;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using PinDominatorCore.PDEnums;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.PDViewModel.Boards;

namespace PinDominator.PDViews.Tools.AcceptBoardInvitation
{
    public class AcceptBoardInvitationConfigurationBase : ModuleSettingsUserControl<AcceptBoardInvitationViewModel,
        AcceptBoardInvitationModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.AcceptBoardInvitationModel =
                        templateModel.ActivitySettings.GetActivityModel<AcceptBoardInvitationModel>(ObjViewModel.Model,
                            true);
                else if (ObjViewModel == null)
                    ObjViewModel = new AcceptBoardInvitationViewModel();

                ObjViewModel.AcceptBoardInvitationModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for AcceptBoardInvitationConfiguration.xaml
    /// </summary>
    public partial class AcceptBoardInvitationConfiguration
    {
        public AcceptBoardInvitationConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.AcceptBoardInvitation,
                Enums.PdMainModule.Boards.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.AcceptBoardInvitationVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.AcceptBoardInvitationKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static AcceptBoardInvitationConfiguration CurrentAcceptBoardInvitationConfiguration { get; set; }

        /// <summary>
        ///     GetSingletonObjectAcceptBoardInvitationConfiguration is used to get the object of the current user control,
        ///     if object is already created then its wont create a new object object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static AcceptBoardInvitationConfiguration GetSingletonObjectAcceptBoardInvitationConfiguration()
        {
            return CurrentAcceptBoardInvitationConfiguration ??
                   (CurrentAcceptBoardInvitationConfiguration = new AcceptBoardInvitationConfiguration());
        }
    }
}