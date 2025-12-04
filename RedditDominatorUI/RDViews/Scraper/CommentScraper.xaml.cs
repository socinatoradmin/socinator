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
    public class CommentScraperBase : ModuleSettingsUserControl<CommentScraperViewModel, CommentScraperModel>
    {
        protected override bool ValidateCampaign()
        {
            if (Model.SavedQueries.Count != 0) return base.ValidateCampaign();
            Dialog.ShowDialog("Error", "Please add atleast one query");
            return false;
        }
    }


    /// <summary>
    ///     Interaction logic for CommentScraper.xaml
    /// </summary>
    public sealed partial class CommentScraper : CommentScraperBase
    {
        public CommentScraper()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: CommentScraperHeader,
                footer: CommentScraperFooter,
                queryControl: CommentScraperSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.CommentScraper,
                moduleName: RdMainModule.CommentScraper.ToString()
            );
            // Help control links. 
            KnowledgeBaseLink = ConstantHelpDetails.CommentScraperKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            VideoTutorialLink = ConstantHelpDetails.CommentScraperVideoTutorialsLink;
            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }


        private static CommentScraper CurrentCommentScraper { get; set; }

        public static CommentScraper GetSingletonObjectCommentScraper()
        {
            return CurrentCommentScraper ?? (CurrentCommentScraper = new CommentScraper());
        }
    }
}