using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.GDViewModel.InstaScraper;
using MahApps.Metro.Controls.Dialogs;
using static GramDominatorCore.GDEnums.Enums;

namespace GramDominatorUI.GDViews.InstaScrape
{
    /// <summary>
    ///     Interaction logic for CommentScraper.xaml
    /// </summary>
    public class CommentScraperBase : ModuleSettingsUserControl<CommentScraperViewModel, CommentScraperModel>
    {
        protected override bool ValidateCampaign()
        {
            // Check queries
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            return base.ValidateCampaign();
        }
    }

    public partial class CommentScraper : CommentScraperBase
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
                moduleName: GdMainModule.Scraper.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.CommentScraperVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.CommentScraperKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static CommentScraper CurrentUserScraper { get; set; }

        public static CommentScraper GetSingeltonObjectCommentScraper()
        {
            return CurrentUserScraper ?? (CurrentUserScraper = new CommentScraper());
        }
    }
}