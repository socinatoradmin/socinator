using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.Enums;
using LinkedDominatorCore.LDModel.Messenger;
using LinkedDominatorCore.LDViewModel.Messenger;
using LinkedDominatorUI.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace LinkedDominatorUI.LDViews.Messenger
{
    public class SendMessageToNewConnectionBase : ModuleSettingsUserControl<SendMessageToNewConnectionViewModel,
        SendMessageToNewConnectionModel>
    {
        protected override bool ValidateCampaign()
        {
            // Check queries
            //if (string.IsNullOrWhiteSpace(ObjViewModel.SendMessageToNewConnectionModel.Message))
            //{
            //    Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
            //        "LangKeyPleaseEnterAMesageAndPressTheSaveButton".FromResourceDictionary());
            //    return false;
            //}
            if (ObjViewModel.SendMessageToNewConnectionModel.LstDisplayManageMessagesModel.Count == 0)
            {
                Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                    "LangKeyPleaseInputAtleastOneMessage".FromResourceDictionary());
                return false;
            }

            return base.ValidateCampaign();
        }
    }

    /// <summary>
    ///     Interaction logic for SendMessageToNewConnection.xaml
    /// </summary>
    public partial class SendMessageToNewConnection : SendMessageToNewConnectionBase
    {
        private static SendMessageToNewConnection ObjSendMessageToNewConnection;

        public SendMessageToNewConnection()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: HeaderControl,
                footer: SendMessageToNewConnectionFooter,
                MainGrid: MainGrid,
                activityType: ActivityType.SendMessageToNewConnection,
                moduleName: LdMainModules.Messenger.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.SendMessageToNewConnectionVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.SendMessageToNewConnectionKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        public static SendMessageToNewConnection GetSingeltonSendMessageToNewConnection()
        {
            return ObjSendMessageToNewConnection ?? (ObjSendMessageToNewConnection = new SendMessageToNewConnection());
        }
    }
}