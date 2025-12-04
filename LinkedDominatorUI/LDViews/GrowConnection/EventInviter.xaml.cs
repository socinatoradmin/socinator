using DominatorHouseCore.Enums;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.Enums;
using LinkedDominatorCore.LDModel.GrowConnection;
using LinkedDominatorCore.LDViewModel.GrowConnection;
using MahApps.Metro.Controls.Dialogs;
using DominatorHouseCore.Utility;
using LinkedDominatorUI.Utility;
namespace LinkedDominatorUI.LDViews.GrowConnection
{
    public class EventInviterBase: ModuleSettingsUserControl<InviteMemberToFollowPageViewModel,
        InviteMemberToFollowPageModel>
    {
        protected override bool ValidateCampaign()
        {
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
    /// Interaction logic for EventInviter.xaml
    /// </summary>
    public partial class EventInviter : EventInviterBase
    {
        private static EventInviter _objSendPageInvitation;
        public EventInviter()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: HeaderControl,
                footer: SendEventInvitationFooter,
                MainGrid: MainGrid,
                activityType: ActivityType.EventInviter,
                moduleName: LdMainModules.GrowConnection.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.EventInviterVideoUrl;
            KnowledgeBaseLink = ConstantHelpDetails.EventInviterKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            DialogParticipation.SetRegister(this, this);
            SetDataContext();
        }
        public static EventInviter GetSingletonSendPageInvitation()
        {
            return _objSendPageInvitation ?? (_objSendPageInvitation = new EventInviter());
        }
    }
}
