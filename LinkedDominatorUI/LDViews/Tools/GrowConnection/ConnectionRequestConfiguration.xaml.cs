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
    public class
        ConnectionRequestConfigurationBase : ModuleSettingsUserControl<ConnectionRequestViewModel,
            ConnectionRequestModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.ConnectionRequestModel =
                        templateModel.ActivitySettings.GetActivityModel<ConnectionRequestModel>(ObjViewModel.Model);
                else
                    ObjViewModel = new ConnectionRequestViewModel();
                ObjViewModel.ConnectionRequestModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        protected override bool ValidateExtraProperty()
        {
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            return true;
        }
    }

    /// <summary>
    ///     Interaction logic for ConnectionRequestConfiguration.xaml
    /// </summary>
    public partial class ConnectionRequestConfiguration : ConnectionRequestConfigurationBase
    {
        private ConnectionRequestConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.ConnectionRequest,
                LdMainModules.GrowConnection.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: ConnectionSearchControl
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.ConnectionRequestVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.ConnectionRequestKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            //AccountGrowthHeader.SelectedItem = SocinatorInitialize.GetSocialLibrary(SocialNetworks.LinkedIn).GetNetworkCoreFactory().AccountUserControlTools.RecentlySelectedAccount;
        }

        private static ConnectionRequestConfiguration CurrentConnectionRequestConfiguration { get; set; }

        public static ConnectionRequestConfiguration GetSingeltonObjectConnectionRequestConfiguration()
        {
            return CurrentConnectionRequestConfiguration ??
                   (CurrentConnectionRequestConfiguration = new ConnectionRequestConfiguration());
        }
    }
}