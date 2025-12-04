using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using QuoraDominatorCore.Enums;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.ViewModel.Scrape;

namespace QuoraDominatorUI.QDViews.Scrape
{
    public class UserScraperbase : ModuleSettingsUserControl<UserScraperViewModel, UserScraperModel>
    {
        protected override bool ValidateCampaign()
        {
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            return base.ValidateCampaign();
        }
    }

    /// <summary>
    ///     Interaction logic for UserScraper.xaml
    /// </summary>
    public partial class UserScraper
    {
        private static UserScraper _currentUserScraper;

        public UserScraper()
        {
            InitializeComponent();
            InitializeBaseClass(
                header: UserScraperHeader,
                footer: UserScraperFooter,
                queryControl: UserScraperSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.UserScraper,
                moduleName: QdMainModule.Scrape.ToString()
            );

            VideoTutorialLink = ConstantHelpDetails.UserScrapersContactLink;
            KnowledgeBaseLink = ConstantHelpDetails.UserScrapersKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        public static UserScraper GetSingeltonObjectUserScraper()
        {
            return _currentUserScraper ?? (_currentUserScraper = new UserScraper());
        }
    }
}