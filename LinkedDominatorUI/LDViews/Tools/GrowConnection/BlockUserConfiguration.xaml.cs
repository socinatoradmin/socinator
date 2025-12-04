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
    ///     Interaction logic for BlockUserConfiguration.xaml
    /// </summary>
    public class BlockUserConfigurationBase : ModuleSettingsUserControl<BlockUserViewModel, BlockUserModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.BlockUserModel =
                        templateModel.ActivitySettings.GetActivityModel<BlockUserModel>(ObjViewModel.Model, true);
                else
                    ObjViewModel = new BlockUserViewModel();
                ObjViewModel.BlockUserModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        protected override bool ValidateExtraProperty()
        {
            if (!string.IsNullOrEmpty(ObjViewModel.BlockUserModel.UrlInput?.Trim()))
                return true;
            Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                "LangKeyPleaseAddProfileUrls".FromResourceDictionary());
            return false;
        }
    }

    public partial class BlockUserConfiguration : BlockUserConfigurationBase
    {
        private BlockUserConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.BlockUser,
                LdMainModules.GrowConnection.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.BlockUserVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.BlockUserKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static BlockUserConfiguration CurrentBlockUserConfiguration { get; set; }

        public static BlockUserConfiguration GetSingletonObjectBlockUserConfiguration()
        {
            return CurrentBlockUserConfiguration ?? (CurrentBlockUserConfiguration = new BlockUserConfiguration());
        }
    }
}