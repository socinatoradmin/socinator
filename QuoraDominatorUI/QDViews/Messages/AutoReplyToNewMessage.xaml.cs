using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using QuoraDominatorCore.Enums;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.ViewModel.Messages;

namespace QuoraDominatorUI.QDViews.Messages
{
    public class
        AutoReplyToNewMessageBase : ModuleSettingsUserControl<AutoReplyToNewMessageViewModel, AutoReplyToNewMessageModel
        >
    {
        protected override bool ValidateExtraProperty()
        {
            if ((ObjViewModel.AutoReplyToNewMessageModel.IsReplyToAllMessagesChecked ||
                 ObjViewModel.AutoReplyToNewMessageModel.IsReplyToMessagesThatContainSpecificWord﻿Checked ||
                 ObjViewModel.AutoReplyToNewMessageModel.IsReplyToPendingMessages﻿﻿Checked)
                && !string.IsNullOrEmpty(ObjViewModel.AutoReplyToNewMessageModel.Message))
                return true;
            Dialog.ShowDialog("Error", "You didn't select any Message Filter or you didn't type message.");
            return false;
        }

        protected override bool ValidateQuery()
        {
            return true;
        }
    }

    /// <summary>
    ///     Interaction logic for AutoReplyToNewMessage.xaml
    /// </summary>
    public partial class AutoReplyToNewMessage
    {
        private static AutoReplyToNewMessage _objAutoReplyToNewMessage;

        public AutoReplyToNewMessage()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: HeaderControl,
                footer: AutoReplyToNewMessageFooter,
                MainGrid: MainGrid,
                activityType: ActivityType.AutoReplyToNewMessage,
                moduleName: QdMainModule.Messages.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.AutoReplyToNewMessageVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.AutoReplyToNewMessageKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        public static AutoReplyToNewMessage GetSingeltonAutoReplyToNewMessage()
        {
            return _objAutoReplyToNewMessage ?? (_objAutoReplyToNewMessage = new AutoReplyToNewMessage());
        }
    }
}