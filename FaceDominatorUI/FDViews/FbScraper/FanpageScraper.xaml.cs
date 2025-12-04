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
    public class FanpageScraperBase : ModuleSettingsUserControl<FanpageScraperViewModel, FanpageScraperModel>
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
    ///     Interaction logic for FanpageScraper.xaml
    /// </summary>
    public partial class FanpageScraper
    {
        private FanpageScraper()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: HeaderGrid,
                footer: FanpageScraperFooter,
                queryControl: FanpageSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.FanpageScraper,
                moduleName: FdMainModule.Scraper.ToString()
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.FanpageScraperVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.FanpageScraperKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            base.SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static FanpageScraper CurrentFanpageScraper { get; set; }

        public static FanpageScraper GetSingeltonObjectFanpageScraper()
        {
            return CurrentFanpageScraper ?? (CurrentFanpageScraper = new FanpageScraper());
        }

        #region OldEvents

        //private void HeaderControl_OnInfoChanged(object sender, RoutedEventArgs e)
        //{
        //    HelpFlyout.IsOpen = true;
        //}


        //private void FanpageScraperFooter_SelectAccountChanged(object sender, RoutedEventArgs e)
        //=> base.FooterControl_OnSelectAccountChanged(sender, e);

        //private void FanpageScraperFooter_CreateCampaignChanged(object sender, RoutedEventArgs e)
        //=> base.CreateCampaign();

        //private void FanpageScraperFooter_UpdateCampaignChanged(object sender, RoutedEventArgs e)
        // => UpdateCampaign();


        //private void FanpageSearchControl_OnAddQuery(object sender, RoutedEventArgs e)
        //=> base.SearchQueryControl_OnAddQuery(sender, e, typeof(FanpageLikerQueryParameters));

        //private void FanpageSearchControl_OnCustomFilterChanged(object sender, RoutedEventArgs e)
        //{
        //    FanpageFilterControl objUserFiltersControl = new FanpageFilterControl();

        //    objUserFiltersControl.IsSaveCloseButtonVisisble = true;

        //    if (!string.IsNullOrEmpty(FanpageSearchControl.CurrentQuery.CustomFilters))
        //    {
        //        try
        //        {
        //            objUserFiltersControl.FanpageFilter = JsonConvert.DeserializeObject<FdFanpageFilterModel>(FanpageSearchControl.CurrentQuery.CustomFilters);

        //        }
        //        catch (Exception ex)
        //        {
        //            ex.DebugLog();
        //        }
        //    }
        //    else
        //    {

        //        objUserFiltersControl.FanpageFilter = new FdFanpageFilterModel();
        //    }

        //    Dialog objDialog = new Dialog();

        //    var FilterWindow = objDialog.GetMetroWindow(objUserFiltersControl, "Filter");

        //    objUserFiltersControl.SaveButton.Click += (senders, Events) =>
        //    {

        //        var UserFilter = objUserFiltersControl.FanpageFilter;
        //        var SerializeCustomFilter = JsonConvert.SerializeObject(UserFilter);
        //        FanpageSearchControl.CurrentQuery.CustomFilters = SerializeCustomFilter;

        //        FilterWindow.Close();
        //    };

        //    FilterWindow.ShowDialog();
        //}

        //private void HeaderOnCancelEdit_Click(object sender, RoutedEventArgs e)
        //{
        //    base.HeaderControl_OnCancelEditClick(sender, e);
        //    TabSwitcher.GoToCampaign();
        //} 

        #endregion
    }
}