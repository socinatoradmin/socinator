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
    public class
        CommnetrRepliesScraperBase : ModuleSettingsUserControl<CommentRepliesScraperViewModel,
            CommentRepliesScraperModel>
    {
    }

    /// <summary>
    ///     Interaction logic for CommnetrRepliesScraper.xaml
    /// </summary>
    public partial class CommnetrRepliesScraper
    {
        private CommnetrRepliesScraper()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: HeaderGrid,
                footer: CommnetrRepliesScraperFooter,
                queryControl: CommnetrRepliesControl,
                MainGrid: MainGrid,
                activityType: ActivityType.CommentRepliesScraper,
                moduleName: FdMainModule.Scraper.ToString()
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.CommentScraperVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.CommentScraperKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static CommnetrRepliesScraper CurrentCommnetrRepliesScraper { get; set; }

        public static CommnetrRepliesScraper GetSingeltonObjectCommnetrRepliesScraper()
        {
            return CurrentCommnetrRepliesScraper ?? (CurrentCommnetrRepliesScraper = new CommnetrRepliesScraper());
        }
    }
}