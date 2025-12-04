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
    public class JobScraperBase : ModuleSettingsUserControl<JobScraperViewModel, JobScraperModel>
    {
    }

    /// <summary>
    ///     Interaction logic for JobScraper.xaml
    /// </summary>
    public partial class JobScraper : JobScraperBase
    {
        public JobScraper()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: JobScraperHeader,
                footer: JobScraperFooter,
                queryControl: JobScraperSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.JobScraper,
                moduleName: LdMainModules.Scraper.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.JobScraperVideoTutorialsLink;

            KnowledgeBaseLink = ConstantHelpDetails.JobScraperKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            DialogParticipation.SetRegister(this, this);
            SetDataContext();
        }

        private static JobScraper CurrentJobScraper { get; set; }

        /// <summary>
        ///     GetSingeltonObjectJobScraper is used to get the object of the current user control,
        ///     if object is already created then its won't create a new object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static JobScraper GetSingeltonObjectJobScraper()
        {
            return CurrentJobScraper ?? (CurrentJobScraper = new JobScraper());
        }
    }
}