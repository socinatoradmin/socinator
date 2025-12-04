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
    public class
        DownvoteForCommentConfigurationBase : ModuleSettingsUserControl<DownvoteForCommentViewModel, DownvoteModel>
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
                    ObjViewModel = new DownvoteForCommentViewModel();

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
    public partial class DownvoteForCommentConfiguration
    {
        private DownvoteForCommentConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);
            InitializeBaseClass
            (
                MainGrid,
                ActivityType.DownvoteComment,
                Enums.RdMainModule.GrowUpvote.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: DownvoteConfigurationSearchControl
            );

            VideoTutorialLink = ConstantHelpDetails.DownVoteForCommentVideoTutorialsLink;
        }

        private static DownvoteForCommentConfiguration CurrentDownvoteConfiguration { get; set; }

        public static DownvoteForCommentConfiguration GetSingeltonObjectDownvoteForCommentConfiguration()
        {
            return CurrentDownvoteConfiguration ??
                   (CurrentDownvoteConfiguration = new DownvoteForCommentConfiguration());
        }
    }
}