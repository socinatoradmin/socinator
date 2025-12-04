using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.ScraperModel;
using FaceDominatorCore.FDViewModel.ScraperViewModel;
using MahApps.Metro.Controls.Dialogs;

namespace FaceDominatorUI.FDViews.FbScraper
{
    public class ProfileScraperBase : ModuleSettingsUserControl<ProfileScraperViewModel, ProfileScraperModel>
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

            if (Model.GenderAndLocationFilter.IsLocationFilterChecked
                && Model.GenderAndLocationFilter.ListLocationUrl.Count == 0
                && Model.GenderAndLocationFilter.ListLocationUrlPair.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectLocationAndSave".FromResourceDictionary());
                return false;
            }

            return base.ValidateCampaign();
        }
    }


    /// <summary>
    ///     Interaction logic for ProfileScraper.xaml
    /// </summary>
    public partial class ProfileScraper
    {
        public ProfileScraper()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: ProfileScraperHeader,
                footer: ProfileScraperFooter,
                queryControl: ProfilesSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.ProfileScraper,
                moduleName: FdMainModule.Scraper.ToString()
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.ProfileScraperVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.ProfileScraperKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static ProfileScraper CurrentProfileScraper { get; set; }

        public static ProfileScraper GetSingeltonObjectProfileScraper()
        {
            return CurrentProfileScraper ?? (CurrentProfileScraper = new ProfileScraper());
        }
    }
}