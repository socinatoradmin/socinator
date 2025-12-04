using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.PDViewModel.PinScraper;
using static PinDominatorCore.PDEnums.Enums;

namespace PinDominator.PDViews.PinScrape
{
    public class UserScraperBase : ModuleSettingsUserControl<UserScraperViewModel, UserScraperModel>
    {
    }

    /// <summary>
    ///     Interaction logic for UserScraper.xaml
    /// </summary>
    public sealed partial class UserScraper
    {
        private UserScraperViewModel _objUserScraperViewModel;

        private UserScraper()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: UserScraperHeaderControl,
                footer: UserScraperFooterControl,
                queryControl: UserScraperSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.UserScraper,
                moduleName: PdMainModule.Scraper.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.UserScraperVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.UserScraperKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static UserScraper CurrentUserScraper { get; set; }

        public UserScraperViewModel ObjUserScraperViewModel
        {
            get => _objUserScraperViewModel;
            set
            {
                _objUserScraperViewModel = value;
                OnPropertyChanged(nameof(ObjUserScraperViewModel));
            }
        }

        public static UserScraper GetSingeltonObjectUserScraper()
        {
            return CurrentUserScraper ?? (CurrentUserScraper = new UserScraper());
        }
    }
}