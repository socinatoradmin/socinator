using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.Enums;
using LinkedDominatorCore.LDModel.GrowConnection;
using LinkedDominatorCore.LDViewModel.GrowConnection;
using MahApps.Metro.Controls.Dialogs;
using System;
using LinkedDominatorUI.Utility;
namespace LinkedDominatorUI.LDViews.Tools.GrowConnection
{
    public class SendEventInvitationBaseUI : ModuleSettingsUserControl<InviteMemberToFollowPageViewModel,
        InviteMemberToFollowPageModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.InviteMemberToFollowPageModel =
                        templateModel.ActivitySettings.GetActivityModel<InviteMemberToFollowPageModel>(ObjViewModel
                            .Model);
                else
                    ObjViewModel = new InviteMemberToFollowPageViewModel();
                ObjViewModel.InviteMemberToFollowPageModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }
        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (!ObjViewModel.InviteMemberToFollowPageModel.IsCheckedLangKeyCustomAdminPageList)

            {
                Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                    "Please select an event source.");
                return false;
            }

            if (ObjViewModel.InviteMemberToFollowPageModel.IsCheckedLangKeyCustomAdminPageList &&
                (ObjViewModel.InviteMemberToFollowPageModel.UrlList == null ||
                 ObjViewModel.InviteMemberToFollowPageModel.UrlList.Count == 0))
            {
                if (string.IsNullOrEmpty(ObjViewModel.InviteMemberToFollowPageModel.UrlInput))
                    Dialog.ShowDialog("Error", "Please input at least one event url");
                else
                    Dialog.ShowDialog("Error", "Please save your event url");
                return false;
            }

            return base.ValidateExtraProperty();
        }
    }
    /// <summary>
    /// Interaction logic for SendEventInvitationBase.xaml
    /// </summary>
    public partial class SendEventInvitationBase : SendEventInvitationBaseUI
    {
        public SendEventInvitationBase()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);
            InitializeBaseClass
            (
                MainGrid,
                ActivityType.EventInviter,
                LdMainModules.GrowConnection.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.EventInviterVideoUrl;
            KnowledgeBaseLink = ConstantHelpDetails.EventInviterKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }
        private static SendEventInvitationBase Instance { get; set; }

        public static SendEventInvitationBase GetSingleton()
        {
            return Instance ??
                   (Instance = new SendEventInvitationBase());
        }
    }
    
}
