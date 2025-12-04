using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;
using TumblrDominatorCore.Enums;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.ViewModels.Scraper;

namespace TumblrDominatorUI.TumblrView.Activity.CommentScraper
{
    public class CommentScraperConfigBase : ModuleSettingsUserControl<CommentScraperViewModel, CommentScraperModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.CommentScraperModel =
                        JsonConvert.DeserializeObject<CommentScraperModel>(templateModel.ActivitySettings);
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

    /// <summary>
    ///     Interaction logic for BroadcastMessagesConfig.xaml
    /// </summary>
    public partial class CommentScraperConfig
    {
        public CommentScraperConfig()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.CommentScraper,
                Enums.TmbMainModule.Scraper.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: CommentScraperConfigSearchControl
            );
            VideoTutorialLink = ConstantHelpDetails.CommentScraperTutorialLink;
        }

        private static CommentScraperConfig _currentCommentScraperConfig;

        public static CommentScraperConfig GetSingletonObjectCommentScraperConfig()
        {
            return _currentCommentScraperConfig ?? (_currentCommentScraperConfig = new CommentScraperConfig());
        }
    }
}