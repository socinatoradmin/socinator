using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.Enums;
using LinkedDominatorCore.LDModel.GrowConnection;
using LinkedDominatorCore.LDViewModel.GrowConnection;
using LinkedDominatorUI.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace LinkedDominatorUI.LDViews.GrowConnection
{
    public class SendPageInvitationBase : ModuleSettingsUserControl<InviteMemberToFollowPageViewModel,
        InviteMemberToFollowPageModel>
    {
        protected override bool ValidateCampaign()
        {
            // Check queries
            if (!ObjViewModel.InviteMemberToFollowPageModel.IsCheckedLangKeyCustomAdminPageList)

            {
                Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                    "LangKeyPleaseSelectAtleastOneOfTheConnectionSources".FromResourceDictionary());
                return false;
            }

            if (ObjViewModel.InviteMemberToFollowPageModel.IsCheckedLangKeyCustomAdminPageList &&
                (ObjViewModel.InviteMemberToFollowPageModel.UrlList == null ||
                 ObjViewModel.InviteMemberToFollowPageModel.UrlList.Count == 0))
            {
                if (string.IsNullOrEmpty(ObjViewModel.InviteMemberToFollowPageModel.UrlInput))
                    Dialog.ShowDialog("Error", "please input at least one custom Admin page url");
                else
                    Dialog.ShowDialog("Error", "please save your custom Admin page url");

                return false;
            }

            return base.ValidateCampaign();
        }
    }

    /// <summary>
    ///     Interaction logic for SendPageInvitation.xaml
    /// </summary>
    public partial class SendPageInvitation : SendPageInvitationBase
    {
        private static SendPageInvitation _objSendPageInvitation;


        public SendPageInvitation()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: HeaderControl,
                footer: SendPageInvitationFooter,
                MainGrid: MainGrid,
                activityType: ActivityType.SendPageInvitations,
                moduleName: LdMainModules.GrowConnection.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.ConnectionRequestVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.ConnectionRequestKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            DialogParticipation.SetRegister(this, this);
            SetDataContext();
        }

        public static SendPageInvitation GetSingletonSendPageInvitation()
        {
            return _objSendPageInvitation ?? (_objSendPageInvitation = new SendPageInvitation());
        }
    }
}