using System;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDEnums;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.GDViewModel.InstaPoster;
using MahApps.Metro.Controls.Dialogs;

namespace GramDominatorUI.GDViews.Tools.Repost
{
    public class RepostConfigurationBase : ModuleSettingsUserControl<RePosterViewModel, RePosterModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.RePosterModel =
                        templateModel.ActivitySettings.GetActivityModel<RePosterModel>(ObjViewModel.Model);
                else
                    ObjViewModel = new RePosterViewModel();
                ObjViewModel.RePosterModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for RepostConfiguration.xaml
    /// </summary>
    public partial class RepostConfiguration : RepostConfigurationBase
    {
        private RepostConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Reposter,
                Enums.GdMainModule.Poster.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: RePosterSearchQueryControl
            );
            VideoTutorialLink = ConstantHelpDetails.RePosterVideoTutorialsLink;
        }

        private static RepostConfiguration CurrentRepostConfiguration { get; set; }

        /// <summary>
        ///     USING THIS METHOD WE WILL GET SINGELTON OBJECTT OF RepostConfiguration
        /// </summary>
        /// <returns></returns>
        public static RepostConfiguration GetSingeltonObjectRepostConfiguration()
        {
            return CurrentRepostConfiguration ?? (CurrentRepostConfiguration = new RepostConfiguration());
        }
    }
}