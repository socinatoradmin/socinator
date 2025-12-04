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
    public class LikeConfigurationBase : ModuleSettingsUserControl<LikeViewModel, LikeModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.LikeModel =
                        templateModel.ActivitySettings.GetActivityModel<LikeModel>(ObjViewModel.Model);
                else
                    ObjViewModel = new LikeViewModel();
                ObjViewModel.LikeModel.IsAccountGrowthActive = isToggleActive;
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
    public partial class LikeConfiguration : LikeConfigurationBase
    {
        public LikeConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Like,
                LdMainModules.Engage.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: LikeSearchControl
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.LikeVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.LikeKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static LikeConfiguration CurrentLikeConfiguration { get; set; }

        public static LikeConfiguration GetSingletonObjectLikeConfiguration()
        {
            return CurrentLikeConfiguration ?? (CurrentLikeConfiguration = new LikeConfiguration());
        }
    }
}