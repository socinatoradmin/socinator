using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;
using TumblrDominatorCore.Enums;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.ViewModels.Scraper;

namespace TumblrDominatorUI.TumblrView.Activity.PostScraper
{
    public class PostScraperConfigBase : ModuleSettingsUserControl<PostScraperViewModel, PostScraperModel>
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

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.PostScraperModel =
                        JsonConvert.DeserializeObject<PostScraperModel>(templateModel.ActivitySettings);
                else
                    ObjViewModel = new PostScraperViewModel();
                ObjViewModel.PostScraperModel.IsAccountGrowthActive = isToggleActive;
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
    public partial class PostScraperConfig
    {
        public PostScraperConfig()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.PostScraper,
                Enums.TmbMainModule.Scraper.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: PostScraperConfigSearchControl
            );
            VideoTutorialLink = ConstantHelpDetails.PostScraperVideoTutorialLink;
        }

        private static PostScraperConfig _currentPostScraperConfig;

        public static PostScraperConfig GetSingletonObjectPostScraperConfig()
        {
            return _currentPostScraperConfig ?? (_currentPostScraperConfig = new PostScraperConfig());
        }
    }
}