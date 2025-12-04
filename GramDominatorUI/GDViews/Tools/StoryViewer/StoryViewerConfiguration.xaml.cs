using System;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDEnums;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDViewModel.StoryViewer;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;

namespace GramDominatorUI.GDViews.Tools.StoryViewer
{
    public class StoryViewerConfigurationBase : ModuleSettingsUserControl<StoryViewModel, StoryModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.StoryModel =
                        JsonConvert.DeserializeObject<StoryModel>(templateModel.ActivitySettings);
                else
                    ObjViewModel = new StoryViewModel();
                ObjViewModel.StoryModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for StoryViewerConfiguration.xaml
    /// </summary>
    public partial class StoryViewerConfiguration : StoryViewerConfigurationBase
    {
        private StoryViewerConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.StoryViewer,
                Enums.GdMainModule.StoryViewer.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: StoryViewerSearchControl
            );
        }

        public static StoryViewerConfiguration currentSingleton { get; set; }

        public static StoryViewerConfiguration StoryConfigurationSingleton()
        {
            return currentSingleton ?? (currentSingleton = new StoryViewerConfiguration());
        }
    }
}