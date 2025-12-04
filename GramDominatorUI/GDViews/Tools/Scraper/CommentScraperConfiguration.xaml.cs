using System;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDEnums;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.GDViewModel.InstaScraper;
using MahApps.Metro.Controls.Dialogs;

namespace GramDominatorUI.GDViews.Tools.Scraper
{
    /// <summary>
    ///     Interaction logic for CommentScraperConfiguration.xaml
    /// </summary>
    public class
        CommentScraperConfigurationBase : ModuleSettingsUserControl<CommentScraperViewModel, CommentScraperModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            if (Model.PostFilterModel.PostCategory.FilterPostCategory)
                if (Model.PostFilterModel.PostCategory.IgnorePostImages &&
                    Model.PostFilterModel.PostCategory.IgnorePostVideos &&
                    Model.PostFilterModel.PostCategory.IgnorePostAlbums)
                {
                    Dialog.ShowDialog(this, "Input Error",
                        "Please check maximum two options for Post Type filteration inside Post Filter category.");
                    return false;
                }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.CommentScraperModel =
                        templateModel.ActivitySettings.GetActivityModel<CommentScraperModel>(ObjViewModel.Model);
                else
                    ObjViewModel = new CommentScraperViewModel();
                ObjViewModel.CommentScraperModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    public partial class CommentScraperConfiguration : CommentScraperConfigurationBase
    {
        public CommentScraperConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.CommentScraper,
                Enums.GdMainModule.Scraper.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: CommentScraperSearchQueryControl
            );
            VideoTutorialLink = ConstantHelpDetails.CommentScraperVideoTutorialsLink;
        }

        private static CommentScraperConfiguration CurrentCommentScraperConfiguration { get; set; }

        public static CommentScraperConfiguration GetSingeltonObjectCommentScraperConfiguration()
        {
            return CurrentCommentScraperConfiguration ??
                   (CurrentCommentScraperConfiguration = new CommentScraperConfiguration());
        }
    }
}