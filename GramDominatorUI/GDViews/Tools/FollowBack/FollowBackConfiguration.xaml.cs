using System;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDEnums;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.GDViewModel.GrowFollower;
using MahApps.Metro.Controls.Dialogs;

namespace GramDominatorUI.GDViews.Tools.FollowBack
{
    public class FollowBackConfigurationBase : ModuleSettingsUserControl<FollowBackViewModel, FollowBackModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.FollowBackModel =
                        templateModel.ActivitySettings.GetActivityModel<FollowBackModel>(ObjViewModel.Model);
                else
                    ObjViewModel = new FollowBackViewModel();
                ObjViewModel.FollowBackModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for FollowBackConfiguration.xaml
    /// </summary>
    public partial class FollowBackConfiguration : FollowBackConfigurationBase
    {
        private FollowBackConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.FollowBack,
                Enums.GdMainModule.GrowFollower.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );
            VideoTutorialLink = ConstantHelpDetails.FollowBackVideoTutorialsLink;
        }

        private static FollowBackConfiguration CurrentFollowBackConfiguration { get; set; }

        public static FollowBackConfiguration GetSingeltonObjectFollowBackConfiguration()
        {
            return CurrentFollowBackConfiguration ?? (CurrentFollowBackConfiguration = new FollowBackConfiguration());
        }
    }
}