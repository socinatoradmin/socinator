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
        RemoveConnectionConfigurationBase : ModuleSettingsUserControl<RemoveConnectionViewModel, RemoveConnectionModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.RemoveConnectionModel =
                        templateModel.ActivitySettings
                            .GetActivityModel<RemoveConnectionModel>(ObjViewModel.Model, true);
                else
                    ObjViewModel = new RemoveConnectionViewModel();
                ObjViewModel.RemoveConnectionModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        protected override bool ValidateExtraProperty()
        {
            if (!ObjViewModel.RemoveConnectionModel.IsCheckedBySoftware
                && !ObjViewModel.RemoveConnectionModel.IsCheckedOutSideSoftware
                && !ObjViewModel.RemoveConnectionModel.IsCheckedLangKeyCustomUserList
            )

            {
                Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                    "LangKeyPleaseSelectAtleastOneOfTheConnectionSources".FromResourceDictionary());
                return false;
            }

            return true;
        }
    }

    /// <summary>
    ///     Interaction logic for RemoveOrWithdrawConnectionsConfiguration.xaml
    /// </summary>
    public partial class RemoveConnectionConfiguration : RemoveConnectionConfigurationBase
    {
        private RemoveConnectionConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.RemoveConnections,
                LdMainModules.GrowConnection.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.RemoveConnectionsVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.RemoveConnectionsKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static RemoveConnectionConfiguration CurrentRemoveConnectionConfiguration { get; set; }

        public static RemoveConnectionConfiguration GetSingeltonObjectRemoveConnectionsConfiguration()
        {
            return CurrentRemoveConnectionConfiguration ??
                   (CurrentRemoveConnectionConfiguration = new RemoveConnectionConfiguration());
        }
    }
}