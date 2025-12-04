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
        MarketPlaceScraperBase : ModuleSettingsUserControl<MarketPlaceScraperViewModel, MarketPlaceScraperModel>
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
    ///     Interaction logic for ProfileScraper.xaml
    /// </summary>
    public partial class MarketPlaceScraper
    {
        public MarketPlaceScraper()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: MarketplaceScraperHeader,
                footer: MarketplaceScraperFooter,
                queryControl: MarketplaceSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.MarketPlaceScraper,
                moduleName: FdMainModule.Scraper.ToString()
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.MarketplaceScraperVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.MarketplaceScraperKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            base.SetDataContext();
            //DialogParticipation.SetRegister(this, this);
        }

        private static MarketPlaceScraper CurrentProfileScraper { get; set; }

        public static MarketPlaceScraper GetSingeltonObjectMarketplaceScraper()
        {
            return CurrentProfileScraper ?? (CurrentProfileScraper = new MarketPlaceScraper());
        }


        #region OldEvents

        //private void HeaderControl_OnInfoChanged(object sender, RoutedEventArgs e)
        //{
        //    HelpFlyout.IsOpen = true;
        //}


        ////Type queryParametereType = typeof(FdProfileUserQUeryParameters);

        //void ProfileScraperFooter_SelectAccountChanged(object sender, RoutedEventArgs e)
        //    => base.FooterControl_OnSelectAccountChanged(sender, e);
        //void ProfileScraperFooter_CreateCampaignChanged(object sender, RoutedEventArgs e)

        //  => base.CreateCampaign();

        //void ProfileScraperFooter_UpdateCampaignChanged(object sender, RoutedEventArgs e)
        //=> UpdateCampaign();

        //private void ProfilesSearchControl_OnAddQuery(object sender, RoutedEventArgs e)
        //    => base.SearchQueryControl_OnAddQuery(sender, e, typeof(FdUserQueryParameters));

        //private void ProfilesSearchControl_OnCustomFilterChanged(object sender, RoutedEventArgs e)
        //{
        //    //GenderAndLocationFilter objUserFiltersControl = new GenderAndLocationFilter();

        //    //objUserFiltersControl.IsSaveCloseButtonVisisble = true;

        //    //objUserFiltersControl.GenderandLocationFilter = new FdGenderAndLocationFilterModel();

        //    //Dialog objDialog = new Dialog();

        //    //var FilterWindow = objDialog.GetMetroWindow(objUserFiltersControl, "Filter");

        //    //objUserFiltersControl.SaveButton.Click += (senders, Events) =>
        //    //{

        //    //    var UserFilter = objUserFiltersControl.GenderandLocationFilter;
        //    //    var SerializeCustomFilter = JsonConvert.SerializeObject(UserFilter);
        //    //    ProfilesSearchControl.CurrentQuery.CustomFilters = SerializeCustomFilter;

        //    //    FilterWindow.Close();
        //    //};

        //    //FilterWindow.ShowDialog();
        //}

        //private void HeaderOnCancelEdit_Click(object sender, RoutedEventArgs e)
        //{
        //    base.HeaderControl_OnCancelEditClick(sender, e);
        //    TabSwitcher.GoToCampaign();
        //} 

        #endregion
    }
}