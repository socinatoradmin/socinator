using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using RedditDominatorCore.RDEnums;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDViewModel;
using System;

namespace RedditDominatorUI.RDViews.Tools
{
    public class FollowerConfigurationBase : ModuleSettingsUserControl<FollowViewModel, FollowModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (Model.SavedQueries.Count != 0) return true;
            Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
            return false;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (templateModel != null && !string.IsNullOrEmpty(templateModel.ActivitySettings))
                    ObjViewModel.FollowModel =
                        JsonConvert.DeserializeObject<FollowModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
                    ObjViewModel = new FollowViewModel();

                ObjViewModel.FollowModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for FollowConfiguration.xaml
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public partial class FollowConfiguration
    {
        private FollowConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Follow,
                Enums.RdMainModule.GrowFollower.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: FollowConfigurationSearchControl
            );

            VideoTutorialLink = ConstantHelpDetails.FollowVideoTutorialsLink;
        }

        private static FollowConfiguration CurrentFollowConfiguration { get; set; }

        public static FollowConfiguration GetSingeltonObjectFolloowConfiguration()
        {
            return CurrentFollowConfiguration ?? (CurrentFollowConfiguration = new FollowConfiguration());
        }
    }
}