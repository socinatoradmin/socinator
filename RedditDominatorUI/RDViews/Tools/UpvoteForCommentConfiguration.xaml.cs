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
    public class UpvoteForCommentConfigurationBase : ModuleSettingsUserControl<UpvoteForCommentViewModel, UpvoteModel>
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
                    ObjViewModel.UpvoteModel =
                        JsonConvert.DeserializeObject<UpvoteModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
                    ObjViewModel = new UpvoteForCommentViewModel();

                ObjViewModel.UpvoteModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }


    /// <summary>
    ///     Interaction logic for UpvoteConfiguration.xaml
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public partial class UpvoteForCommentConfiguration
    {
        private UpvoteForCommentConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);
            InitializeBaseClass
            (
                MainGrid,
                ActivityType.UpvoteComment,
                Enums.RdMainModule.GrowUpvote.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: UpvoteConfigurationSearchControl
            );

            VideoTutorialLink = ConstantHelpDetails.UpVoteForCommentVideoTutorialsLink;
        }

        private static UpvoteForCommentConfiguration CurrentUpvoteConfiguration { get; set; }

        public static UpvoteForCommentConfiguration GetSingeltonObjectUpvoteForCommentConfiguration()
        {
            return CurrentUpvoteConfiguration ?? (CurrentUpvoteConfiguration = new UpvoteForCommentConfiguration());
        }
    }
}