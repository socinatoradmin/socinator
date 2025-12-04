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
        ExportConnectionConfigurationBase : ModuleSettingsUserControl<ExportConnectionViewModel, ExportConnectionModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.ExportConnectionModel =
                        templateModel.ActivitySettings
                            .GetActivityModel<ExportConnectionModel>(ObjViewModel.Model, true);
                else
                    ObjViewModel = new ExportConnectionViewModel();
                ObjViewModel.ExportConnectionModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        protected override bool ValidateExtraProperty()
        {
            if (!ObjViewModel.ExportConnectionModel.IsCheckedBySoftware
                && !ObjViewModel.ExportConnectionModel.IsCheckedOutSideSoftware
                && !ObjViewModel.ExportConnectionModel.IsCheckedLangKeyCustomUserList
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
    ///     Interaction logic for ExportConnectionConfiguration.xaml
    /// </summary>
    public partial class ExportConnectionConfiguration : ExportConnectionConfigurationBase
    {
        private ExportConnectionConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.ExportConnection,
                LdMainModules.GrowConnection.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.ExportConnectionVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.ExportConnectionKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static ExportConnectionConfiguration CurrentExportConnectionConfiguration { get; set; }

        public static ExportConnectionConfiguration GetSingeltonObjectExportConnectionConfiguration()
        {
            return CurrentExportConnectionConfiguration ??
                   (CurrentExportConnectionConfiguration = new ExportConnectionConfiguration());
        }
    }
}