using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDViewModel;
using RedditDominatorUI.RDViews.Tools;
using static RedditDominatorCore.RDEnums.Enums;

namespace RedditDominatorUI.RDViews.UrlScraper
{
    public class UrlScraperBase : ModuleSettingsUserControl<UrlScraperViewModel, UrlScraperModel>
    {
        protected override bool ValidateCampaign()
        {
            if (Model.SavedQueries.Count != 0) return base.ValidateCampaign();
            Dialog.ShowDialog("Error", "Please add atleast one query");
            return false;
        }
    }


    /// <summary>
    ///     Interaction logic for UrlScraper.xaml
    /// </summary>
    public sealed partial class UrlScraper : UrlScraperBase
    {
        public UrlScraper()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: UrlScraperHeader,
                footer: UrlScraperFooter,
                queryControl: UrlScraperSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.UrlScraper,
                moduleName: RdMainModule.UrlScraper.ToString()
            );
            // Help control links. 
            KnowledgeBaseLink = ConstantHelpDetails.UrlScraperKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            VideoTutorialLink = ConstantHelpDetails.UrlScraperVideoTutorialsLink;
            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static UrlScraper CurrentUrlScraper { get; set; }

        public static UrlScraper GetSingletonObjectUrlScraper()
        {
            return CurrentUrlScraper ?? (CurrentUrlScraper = new UrlScraper());
        }
    }
}