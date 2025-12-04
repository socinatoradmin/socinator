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

namespace GramDominatorUI.GDViews.Tools.BlockFollower
{
    public class BlockFollowerConfigurationBase : ModuleSettingsUserControl<BlockFollowerViewModel, BlockFollowerModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.BlockFollowerModel =
                        templateModel.ActivitySettings.GetActivityModel<BlockFollowerModel>(ObjViewModel.Model);
                else
                    ObjViewModel = new BlockFollowerViewModel();
                ObjViewModel.BlockFollowerModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for BlockFollowerConfiguration.xaml
    /// </summary>
    public partial class BlockFollowerConfiguration : BlockFollowerConfigurationBase
    {
        public BlockFollowerConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.BlockFollower,
                Enums.GdMainModule.GrowFollower.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );
            VideoTutorialLink = ConstantHelpDetails.BlockFollowerVideoTutorialsLink;
        }

        private static BlockFollowerConfiguration CurrentBlockFollowerConfiguration { get; set; }

        public static BlockFollowerConfiguration GetSingeltonObjectBlockFollowerConfiguration()
        {
            return CurrentBlockFollowerConfiguration ??
                   (CurrentBlockFollowerConfiguration = new BlockFollowerConfiguration());
        }
    }
}