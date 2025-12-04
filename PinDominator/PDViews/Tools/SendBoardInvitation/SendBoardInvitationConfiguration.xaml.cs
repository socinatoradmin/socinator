using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using PinDominatorCore.PDEnums;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.PDViewModel.Boards;
using System;
using System.Linq;
using System.Windows;

namespace PinDominator.PDViews.Tools.SendBoardInvitation
{
    public class
        SendBoardInvitationConfigurationBase : ModuleSettingsUserControl<SendBoardInvitationViewModel,
            SendBoardInvitationModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (Model.BoardCollaboratorDetails.Count == 0)
            {
                Dialog.ShowDialog(this, "Error",
                    "Please add at least one board Collaborator.");
                return false;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.SendBoardInvitationModel =
                        templateModel.ActivitySettings.GetActivityModel<SendBoardInvitationModel>(ObjViewModel.Model,
                            true);
                else if (ObjViewModel == null)
                    ObjViewModel = new SendBoardInvitationViewModel();

                ObjViewModel.SendBoardInvitationModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for SendBoardInvitationConfiguration.xaml
    /// </summary>
    public partial class SendBoardInvitationConfiguration
    {
        public SendBoardInvitationConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.SendBoardInvitation,
                Enums.PdMainModule.Boards.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.SendBoardInvitationVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.SendMessageToNewFollowersKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static SendBoardInvitationConfiguration CurrentSendBoardInvitationConfiguration { get; set; }

        /// <summary>
        ///     GetSingletonObjectSendBoardInvitationConfiguration is used to get the object of the current user control,
        ///     if object is already created then its wont create a new object object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static SendBoardInvitationConfiguration GetSingletonObjectSendBoardInvitationConfiguration()
        {
            return CurrentSendBoardInvitationConfiguration ??
                   (CurrentSendBoardInvitationConfiguration = new SendBoardInvitationConfiguration());
        }

        private void SendBoardInvitation_OnLoaded(object sender, RoutedEventArgs e)
        {
            var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            var accounts = accountsFileManager.GetAll(SocialNetworks.Pinterest)
                .Where(account => account.AccountBaseModel.Status == AccountStatus.Success);

            ObjViewModel.SendBoardInvitationModel.ListAccounts =
                accounts.Select(account => account.AccountBaseModel.UserName).ToList();
        }
    }
}