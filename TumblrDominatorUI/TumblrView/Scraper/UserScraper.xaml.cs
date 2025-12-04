using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using TumblrDominatorCore.Enums;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.ViewModels.Scraper;

namespace TumblrDominatorUI.TumblrView.Scraper
{
    public class UserScraperBase : ModuleSettingsUserControl<UserScraperViewModel, UserScraperModel>
    {
        protected override bool ValidateCampaign()
        {
            // Check queries
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            return base.ValidateCampaign();
        }
    }

    public sealed partial class UserScraper
    {
        private UserScraper()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: HeaderControl,
                footer: UserScraperFooter,
                queryControl: UserScraperSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.UserScraper,
                moduleName: Enums.TmbMainModule.Scraper.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.UserScraperVideoTutorialLink;
            KnowledgeBaseLink = ConstantHelpDetails.UserScraperKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static UserScraper _currentUserScraper;

        public static UserScraper GetSingletonObjectUserScraperConfig()
        {
            return _currentUserScraper ?? (_currentUserScraper = new UserScraper());
        }
    }
}