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


    /// <summary>
    ///     Interaction logic for CommentScraper.xaml
    /// </summary>
    public partial class CommentScraper
    {
        private CommentScraper()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: HeaderGrid,
                footer: CommentScraperFooter,
                queryControl: CommentSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.CommentScraper,
                moduleName: FdMainModule.Scraper.ToString()
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.CommentScraperVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.CommentScraperKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static CommentScraper CurrentCommentScraper { get; set; }

        public static CommentScraper GetSingeltonObjectCommentScraper()
        {
            return CurrentCommentScraper ?? (CurrentCommentScraper = new CommentScraper());
        }

        #region OldEvents

        //private void HeaderControl_OnInfoChanged(object sender, RoutedEventArgs e)
        //{
        //    HelpFlyout.IsOpen = true;
        //}


        //void CommentScraperFooter_SelectAccountChanged(object sender, RoutedEventArgs e)
        //    => base.FooterControl_OnSelectAccountChanged(sender, e);


        //void CommentScraperFooter_CreateCampaignChanged(object sender, RoutedEventArgs e)
        //    => base.CreateCampaign();


        //void CommentScraperFooter_UpdateCampaignChanged(object sender, RoutedEventArgs e)
        //  => UpdateCampaign();


        //private void CommentSearchControl_OnAddQuery(object sender, RoutedEventArgs e)
        //=> base.SearchQueryControl_OnAddQuery(sender, e, typeof(CommentScraperParameter));


        //private void CommentSearchControl_OnCustomFilterChanged(object sender, RoutedEventArgs e)
        //{

        //}

        //private void HeaderOnCancelEdit_Click(object sender, RoutedEventArgs e)
        //{
        //    base.HeaderControl_OnCancelEditClick(sender, e);
        //    TabSwitcher.GoToCampaign();
        //} 

        #endregion
    }
}