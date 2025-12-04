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
    public class CompanyScraperBase : ModuleSettingsUserControl<CompanyScraperViewModel, CompanyScraperModel>
    {
    }

    /// <summary>
    ///     Interaction logic for CompanyScraper.xaml
    /// </summary>
    public partial class CompanyScraper : CompanyScraperBase
    {
        public CompanyScraper()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: CompanyScraperHeader,
                footer: CompanyScraperFooter,
                queryControl: CompanyScraperSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.CompanyScraper,
                moduleName: LdMainModules.Scraper.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.CompanyScraperVideoTutorialsLink;

            KnowledgeBaseLink = ConstantHelpDetails.CompanyScraperKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            DialogParticipation.SetRegister(this, this);
            SetDataContext();
        }

        private static CompanyScraper CurrentCompanyScraper { get; set; }

        /// <summary>
        ///     GetSingeltonObjectCompanyScraper is used to get the object of the current user control,
        ///     if object is already created then its won't create a new object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static CompanyScraper GetSingeltonObjectCompanyScraper()
        {
            return CurrentCompanyScraper ?? (CurrentCompanyScraper = new CompanyScraper());
        }
    }
}