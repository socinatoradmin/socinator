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
    public class CommentConfigurationBase : ModuleSettingsUserControl<CommentViewModel, CommentModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check for query value
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "Input Error",
                    "Please add at least one query.");
                return false;
            }

            // Check for comment value
            if (Model.LstManageCommentModel.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyAddAtLeastOneComment".FromResourceDictionary());
                return false;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (templateModel != null && !string.IsNullOrEmpty(templateModel.ActivitySettings))
                    ObjViewModel.CommentModel =
                        JsonConvert.DeserializeObject<CommentModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
                    ObjViewModel = new CommentViewModel();

                ObjViewModel.CommentModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for CommentConfiguration.xaml
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public partial class CommentConfiguration
    {
        public CommentConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);
            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Comment,
                Enums.RdMainModule.GrowComment.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: CommentConfigurationSearchControl
            );

            VideoTutorialLink = ConstantHelpDetails.CommentVideoTutorialsLink;
        }

        private static CommentConfiguration CurrentCommentConfiguration { get; set; }

        public static CommentConfiguration GetSingeltonObjectCommentConfiguration()
        {
            return CurrentCommentConfiguration ?? (CurrentCommentConfiguration = new CommentConfiguration());
        }
    }
}