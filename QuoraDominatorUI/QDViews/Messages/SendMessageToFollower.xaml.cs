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
        SendMessageToFollowerBase : ModuleSettingsUserControl<SendMessageToFollowerViewModel, SendMessageToFollowerModel
        >
    {
        protected override bool ValidateQuery()
        {
            return true;
        }

        protected override bool ValidateExtraProperty()
        {
            if (!string.IsNullOrEmpty(ObjViewModel.SendMessageToFollowerModel.Message))
                return true;
            Dialog.ShowDialog("Error", "Please type some message.");
            return false;
        }
    }

    /// <summary>
    ///     Interaction logic for SendMessageToFollower.xaml
    /// </summary>
    public partial class SendMessageToFollower
    {
        private static SendMessageToFollower _objSendMessageToFollower;

        public SendMessageToFollower()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: HeaderControl,
                footer: SendMessageToFollowFooter,
                MainGrid: MainGrid,
                activityType: ActivityType.SendMessageToFollower,
                moduleName: QdMainModule.Messages.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.SendMessageToFollowerVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.SendMessageToFollowerKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        public static SendMessageToFollower GetSingeltonSendMessageToFollower()
        {
            return _objSendMessageToFollower ?? (_objSendMessageToFollower = new SendMessageToFollower());
        }
    }
}