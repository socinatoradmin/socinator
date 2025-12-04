using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.Enums;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDViewModel.Scraper;
using LinkedDominatorUI.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace LinkedDominatorUI.LDViews.Scraper
{
    public class UserScraperBase : ModuleSettingsUserControl<UserScraperViewModel, UserScraperModel>
    {
    }

    /// <summary>
    ///     Interaction logic for UserScraper.xaml
    /// </summary>
    public partial class UserScraper : UserScraperBase
    {
        public UserScraper()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: UserScraperHeader,
                footer: UserScraperFooter,
                queryControl: UserScraperSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.UserScraper,
                moduleName: LdMainModules.Scraper.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.UserScraperVideoTutorialsLink;

            KnowledgeBaseLink = ConstantHelpDetails.UserScraperKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            DialogParticipation.SetRegister(this, this);
            SetDataContext();
        }

        private static UserScraper CurrentUserScraper { get; set; }

        /// <summary>
        ///     GetSingeltonObjectUserScraper is used to get the object of the current user control,
        ///     if object is already created then its won't create a new object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static UserScraper GetSingeltonObjectUserScraper()
        {
            return CurrentUserScraper ?? (CurrentUserScraper = new UserScraper());
        }
    }
}