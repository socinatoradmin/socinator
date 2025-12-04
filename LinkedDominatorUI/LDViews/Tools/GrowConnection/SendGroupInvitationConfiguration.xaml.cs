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
    /// Interaction logic for SendGroupInvitationConfiguration.xaml
    /// </summary>

    public class SendGroupInvitationConfigurationBase : ModuleSettingsUserControl<SendInvitationToGroupMemberViewModel,
        SendInvitationToGroupMemberModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.SendInvitationToGroupMemberModel =
                        templateModel.ActivitySettings.GetActivityModel<SendInvitationToGroupMemberModel>(ObjViewModel
                            .Model);
                else
                    ObjViewModel = new SendInvitationToGroupMemberViewModel();
                ObjViewModel.SendInvitationToGroupMemberModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        protected override bool ValidateExtraProperty()
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
                    Dialog.ShowDialog("Error", "please input at least one custom group url");
                else
                    Dialog.ShowDialog("Error", "please save your custom Admin group url");

                return false;
            }

            return base.ValidateExtraProperty();
        }
    }

    public partial class SendGroupInvitationConfiguration : SendGroupInvitationConfigurationBase
    {
        public SendGroupInvitationConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);
            InitializeBaseClass
            (
                MainGrid,
                ActivityType.SendGroupInvitations,
                LdMainModules.GrowConnection.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.RemoveConnectionsVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.RemoveConnectionsKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static SendGroupInvitationConfiguration CurrentSendGroupInvitationConfiguration { get; set; }

        public static SendGroupInvitationConfiguration GetSingeltonObjectSendGroupInvitationConfiguration()
        {
            return CurrentSendGroupInvitationConfiguration ??
                   (CurrentSendGroupInvitationConfiguration = new SendGroupInvitationConfiguration());
        }
    }
}
