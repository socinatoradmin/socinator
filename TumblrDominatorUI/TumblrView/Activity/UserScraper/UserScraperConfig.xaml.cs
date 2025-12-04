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

namespace TumblrDominatorUI.TumblrView.Activity.UserScraper
{
    public class UserScraperConfigBase : ModuleSettingsUserControl<UserScraperViewModel, UserScraperModel>
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
                    ObjViewModel.UserScraperModel =
                        JsonConvert.DeserializeObject<UserScraperModel>(templateModel.ActivitySettings);
                else
                    ObjViewModel = new UserScraperViewModel();
                ObjViewModel.UserScraperModel.IsAccountGrowthActive = isToggleActive;
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
    public partial class UserScraperConfig
    {
        public UserScraperConfig()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.UserScraper,
                Enums.TmbMainModule.Scraper.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: UserScraperConfigSearchControl
            );

            VideoTutorialLink = ConstantHelpDetails.UserScraperVideoTutorialLink;
        }

        private static UserScraperConfig _currentUserScraperConfig;

        public static UserScraperConfig GetSingletonObjectUserScraperConfig()
        {
            return _currentUserScraperConfig ?? (_currentUserScraperConfig = new UserScraperConfig());
        }
    }
}