using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using Newtonsoft.Json;
using RedditDominatorCore.RDEnums;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDViewModel;
using System;

namespace RedditDominatorUI.RDViews.Tools
{
    /// <summary>
    ///     Interaction logic for CommentScraperConfiguration.xaml
    /// </summary>
    public partial class CommentScraperConfiguration : CommentScraperConfigurationBase
    {
        public CommentScraperConfiguration()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                MainGrid,
                ActivityType.CommentScraper,
                Enums.RdMainModule.CommentScraper.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: CommentScraperSearchControl
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


    public class
        CommentScraperConfigurationBase : ModuleSettingsUserControl<CommentScraperViewModel, CommentScraperModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (Model.SavedQueries.Count != 0) return true;
            Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
            return false;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (templateModel != null && !string.IsNullOrEmpty(templateModel.ActivitySettings))
                    ObjViewModel.CommentScraperModel =
                        JsonConvert.DeserializeObject<CommentScraperModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
                    ObjViewModel = new CommentScraperViewModel();

                ObjViewModel.CommentScraperModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}