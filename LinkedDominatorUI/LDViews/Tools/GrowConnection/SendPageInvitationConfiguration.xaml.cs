using System;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.Enums;
using LinkedDominatorCore.LDModel.GrowConnection;
using LinkedDominatorCore.LDViewModel.GrowConnection;
using LinkedDominatorUI.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace LinkedDominatorUI.LDViews.Tools.GrowConnection
{
    /// <summary>
    ///     Interaction logic for SendPageInvitationConfiguration.xaml
    /// </summary>
    public class SendPageInvitationConfigurationBase : ModuleSettingsUserControl<InviteMemberToFollowPageViewModel,
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

            return base.ValidateExtraProperty();
        }
    }

    public partial class SendPageInvitationConfiguration : SendPageInvitationConfigurationBase
    {
        public SendPageInvitationConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);
            InitializeBaseClass
            (
                MainGrid,
                ActivityType.SendPageInvitations,
                LdMainModules.GrowConnection.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.RemoveConnectionsVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.RemoveConnectionsKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static SendPageInvitationConfiguration CurrentSendPageInvitationConfiguration { get; set; }

        public static SendPageInvitationConfiguration GetSingeltonObjectSendPageInvitationConfiguration()
        {
            return CurrentSendPageInvitationConfiguration ??
                   (CurrentSendPageInvitationConfiguration = new SendPageInvitationConfiguration());
        }
    }
}