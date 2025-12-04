using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.Enums;
using LinkedDominatorCore.LDModel.SalesNavigatorScraper;
using LinkedDominatorCore.LDViewModel.SalesNavigatorScraper;
using LinkedDominatorUI.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace LinkedDominatorUI.LDViews.SalesNavigatorUserScraper
{
    /// <summary>
    ///     Interaction logic for SalesUserScraper.xaml
    /// </summary>
    public class SalesNavigatorUserScraperBase : ModuleSettingsUserControl<UserScraperViewModel, UserScraperModel>
    {
    }

    /// <summary>
    ///     Interaction logic for SalesUserScraper.xaml
    /// </summary>
    public partial class UserScraper : SalesNavigatorUserScraperBase
    {
        public UserScraper()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: Header,
                footer: UserScraperFooter,
                queryControl: UserScraperSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.SalesNavigatorUserScraper,
                moduleName: LdMainModules.SalesNavigatorScraper.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.SalesUserScraperVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.SalesUserScraperKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            DialogParticipation.SetRegister(this, this);
            SetDataContext();
        }

        private static UserScraper ObjCurrentUserScraper { get; set; }

        /// <summary>
        ///     GetSingeltonObjectSalesUserScraper is used to get the object of the current user control,
        ///     if object is already created then its won't create a new object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static UserScraper GetSingeltonObjectSalesUserScraper()
        {
            return ObjCurrentUserScraper ?? (ObjCurrentUserScraper = new UserScraper());
        }
    }
}