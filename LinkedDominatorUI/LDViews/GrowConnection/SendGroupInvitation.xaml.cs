using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.Enums;
using LinkedDominatorCore.LDModel.GrowConnection;
using LinkedDominatorCore.LDViewModel.GrowConnection;
using LinkedDominatorUI.Utility;
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Controls;

namespace LinkedDominatorUI.LDViews.GrowConnection
{   

    public class SendGroupInvitationBase : ModuleSettingsUserControl<SendInvitationToGroupMemberViewModel,
        SendInvitationToGroupMemberModel>
    {
        protected override bool ValidateCampaign()
        {
            // Check queries
            if (!ObjViewModel.SendInvitationToGroupMemberModel.IsCheckedLangKeyCustomAdminGroupList)

            {
                Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                    "LangKeyPleaseSelectAtleastOneOfTheConnectionSources".FromResourceDictionary());
                return false;
            }

            if (ObjViewModel.SendInvitationToGroupMemberModel.IsCheckedLangKeyCustomAdminGroupList &&
                (ObjViewModel.SendInvitationToGroupMemberModel.UrlList == null ||
                 ObjViewModel.SendInvitationToGroupMemberModel.UrlList.Count == 0))
            {
                if (string.IsNullOrEmpty(ObjViewModel.SendInvitationToGroupMemberModel.UrlInput))
                    Dialog.ShowDialog("Error", "please input at least one custom Admin Group url");
                else
                    Dialog.ShowDialog("Error", "please save your custom Admin Group url");

                return false;
            }

            return base.ValidateCampaign();
        }
    }

    /// <summary>
    ///     Interaction logic for SendGroupInvitation.xaml
    /// </summary>
    public partial class SendGroupInvitation : SendGroupInvitationBase
    {
        private static SendGroupInvitation _objSendGroupInvitation;


        public SendGroupInvitation()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: HeaderControl,
                footer:  SendGroupInvitationFooter,
                MainGrid: MainGrid,
                activityType: ActivityType.SendGroupInvitations,
                moduleName: LdMainModules.GrowConnection.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.ConnectionRequestVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.ConnectionRequestKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            DialogParticipation.SetRegister(this, this);
            SetDataContext();
        }

        public static SendGroupInvitation GetSingletonSendGroupInvitation()
        {
            return _objSendGroupInvitation ?? (_objSendGroupInvitation = new SendGroupInvitation());
        }
    }


}
