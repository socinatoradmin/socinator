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
    public class UserScraperConfigBase : ModuleSettingsUserControl<UserScraperViewModel, UserScraperModel>
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
                    ObjViewModel.UserScraperModel =
                        JsonConvert.DeserializeObject<UserScraperModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
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
    ///     Interaction logic for UserScraperConfig.xaml
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public partial class UserScraperConfig : UserScraperConfigBase
    {
        public UserScraperConfig()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.UserScraper,
                Enums.RdMainModule.UserScraper.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: UserScraperSearchControl
            );

            VideoTutorialLink = ConstantHelpDetails.UserScraperVideoTutorialsLink;
        }

        private static UserScraperConfig CurrentUserScraperConfig { get; set; }

        public static UserScraperConfig GetSingeltonObjectUserScraperConfig()
        {
            return CurrentUserScraperConfig ?? (CurrentUserScraperConfig = new UserScraperConfig());
        }
    }
}