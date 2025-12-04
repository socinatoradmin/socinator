using System;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.Enums;
using LinkedDominatorCore.LDModel.Engage;
using LinkedDominatorCore.LDViewModel.Engage;
using LinkedDominatorUI.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace LinkedDominatorUI.LDViews.Tools.Engage
{
    /// <summary>
    ///     Interaction logic for ShareConfiguration.xaml
    /// </summary>
    public class ShareConfigurationBase : ModuleSettingsUserControl<ShareViewModel, ShareModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.ShareModel =
                        templateModel.ActivitySettings.GetActivityModel<ShareModel>(ObjViewModel.Model);
                else
                    ObjViewModel = new ShareViewModel();
                ObjViewModel.ShareModel.IsAccountGrowthActive = isToggleActive;
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
    ///     Interaction logic for LikeConfiguration.xaml
    /// </summary>
    public partial class ShareConfiguration : ShareConfigurationBase
    {
        public ShareConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Share,
                LdMainModules.Engage.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: ShareSearchControl
            );


            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.ShareVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.ShareKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static ShareConfiguration CurrentShareConfiguration { get; set; }

        public static ShareConfiguration GetSingletonObjectShareConfiguration()
        {
            return CurrentShareConfiguration ?? (CurrentShareConfiguration = new ShareConfiguration());
        }
    }
}