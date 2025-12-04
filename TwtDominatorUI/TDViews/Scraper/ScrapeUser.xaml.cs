using DominatorHouseCore.Enums;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;
using TwtDominatorCore.TDViewModel.Scraper;
using static TwtDominatorCore.TDEnums.Enums;

namespace TwtDominatorUI.TDViews.Scraper
{
    public class ScrapeUserBase : ModuleSettingsUserControl<ScrapeUserViewModel, ScrapeUserModel>
    {
        protected override bool ValidateExtraProperty()
        {
            return ValidateSavedQueries();
        }
    }


    /// <summary>
    ///     Interaction logic for ScrapeUser.xaml
    /// </summary>
    public partial class ScrapeUser : ScrapeUserBase
    {
        private ScrapeUser()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: ScrapeUserHeader,
                footer: ScrapeUserFooter,
                queryControl: ScrapeUserSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.UserScraper,
                moduleName: TdMainModule.Scraper.ToString()
            );

            // Help control links. 
            VideoTutorialLink = TDHelpDetails.ScrapeUserVideoTutorialsLink;
            KnowledgeBaseLink = TDHelpDetails.ScrapeUserKnowledgeBaseLink;
            ContactSupportLink = TDHelpDetails.ContactLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        #region SingletonObject Creation

        private static ScrapeUser objScrapeUser;

        public static ScrapeUser GetSingletonObjectScrapeUser()
        {
            return objScrapeUser ?? (objScrapeUser = new ScrapeUser());
        }

        #endregion
    }
}