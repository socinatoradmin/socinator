using System.Linq;
using System.Windows;
using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
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
        SendBoardInvitationBase : ModuleSettingsUserControl<SendBoardInvitationViewModel, SendBoardInvitationModel>
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
    }

    /// <summary>
    ///     Interaction logic for SendBoardInvitation.xaml
    /// </summary>
    public partial class SendBoardInvitation
    {
        public SendBoardInvitation()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: SendBoardInvitationHeaderControl,
                footer: SendBoardInvitationFooterControl,
                MainGrid: MainGrid,
                moduleName: PdMainModule.Boards.ToString(),
                activityType: ActivityType.SendBoardInvitation
            );
            VideoTutorialLink = ConstantHelpDetails.SendBoardInvitationVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.SendBoardInvitationKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            DialogParticipation.SetRegister(this, this);
            SetDataContext();
        }

        private static SendBoardInvitation CurrentSendBoardInvitation { get; set; }

        /// <summary>
        ///     GetSingletonObjectRePin﻿ is used to get the object of the current user control,
        ///     if object is already created then its wont create a new object object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static SendBoardInvitation GetSingletonObjectSendBoardInvitation()
        {
            return CurrentSendBoardInvitation ?? (CurrentSendBoardInvitation = new SendBoardInvitation());
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