using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.Enums;
using LinkedDominatorCore.LDModel.SalesNavigatorScraper;
using LinkedDominatorCore.LDViewModel.SalesNavigatorScraper;
using LinkedDominatorUI.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace LinkedDominatorUI.LDViews.SalesNavigatorScraper
{
    public class
        SalesNaigatorCompanyScraperBase : ModuleSettingsUserControl<CompanyScraperViewModel, CompanyScraperModel>
    {
    }

    /// <summary>
    ///     Interaction logic for CompanyScraper.xaml
    /// </summary>
    public partial class CompanyScraper : SalesNaigatorCompanyScraperBase
    {
        public CompanyScraper()
        {
            InitializeComponent();
            InitializeBaseClass(
                header: CompanyScraperHeader,
                footer: CompanyScraperFooter,
                queryControl: CompanyScraperSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.SalesNavigatorCompanyScraper,
                moduleName: LdMainModules.SalesNavigatorScraper.ToString());

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.SalesCompanyScraperVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.SalesCompanyScraperKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            DialogParticipation.SetRegister(this, this);
            SetDataContext();
        }

        private static CompanyScraper CurrentCompanyScraper { get; set; }

        public static CompanyScraper GetSingletonObjectCompanyScraper()
        {
            return CurrentCompanyScraper ?? (CurrentCompanyScraper = new CompanyScraper());
        }
    }
}