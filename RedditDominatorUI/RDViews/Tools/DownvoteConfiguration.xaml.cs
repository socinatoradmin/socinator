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
    public class DownvoteConfigurationBase : ModuleSettingsUserControl<DownvoteViewModel, DownvoteModel>
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
                    ObjViewModel.DownvoteModel =
                        JsonConvert.DeserializeObject<DownvoteModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
                    ObjViewModel = new DownvoteViewModel();

                ObjViewModel.DownvoteModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for DownvoteConfiguration.xaml
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public partial class DownvoteConfiguration
    {
        private DownvoteConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);
            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Downvote,
                Enums.RdMainModule.GrowUpvote.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: DownvoteConfigurationSearchControl
            );

            VideoTutorialLink = ConstantHelpDetails.DownvoteVideoTutorialsLink;
        }

        private static DownvoteConfiguration CurrentDownvoteConfiguration { get; set; }

        public static DownvoteConfiguration GetSingeltonObjectDownvoteConfiguration()
        {
            return CurrentDownvoteConfiguration ?? (CurrentDownvoteConfiguration = new DownvoteConfiguration());
        }
    }
}