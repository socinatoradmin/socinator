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
    public class GroupScraperBase : ModuleSettingsUserControl<GroupScraperViewModel, GroupScraperModel>
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
    ///     Interaction logic for GroupScraper.xaml
    /// </summary>
    public partial class GroupScraper
    {
        private GroupScraper()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: HeaderGrid,
                footer: GroupScraperFooter,
                queryControl: GroupsSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.GroupScraper,
                moduleName: FdMainModule.Scraper.ToString()
            );
            // Help control links. 
            VideoTutorialLink = FdConstants.GroupScraperVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.GroupScraperKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            base.SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static GroupScraper CurrentGroupScraper { get; set; }

        public static GroupScraper GetSingeltonObjectGroupScraper()
        {
            return CurrentGroupScraper ?? (CurrentGroupScraper = new GroupScraper());
        }

        #region OldEvents

        //private void HeaderControl_OnInfoChanged(object sender, RoutedEventArgs e)
        //   => HelpFlyout.IsOpen = true;

        //private void GroupScraper_SelectAccountChanged(object sender, RoutedEventArgs e)
        //    => base.FooterControl_OnSelectAccountChanged(sender, e);

        //private void GroupScraper_CreateCampaignChanged(object sender, RoutedEventArgs e)
        //    => base.CreateCampaign();

        //private void GroupScraper_UpdateCampaignChanged(object sender, RoutedEventArgs e)
        // => UpdateCampaign();


        //private void GroupsSearchControl_OnAddQuery(object sender, RoutedEventArgs e)
        //    => base.SearchQueryControl_OnAddQuery(sender, e, typeof(GroupScraperParameter));

        //private void GroupsSearchControl_OnCustomFilterChanged(object sender, RoutedEventArgs e)
        //{
        //    GroupFilterControl objUserFiltersControl = new GroupFilterControl();

        //    objUserFiltersControl.IsSaveCloseButtonVisisble = true;

        //    objUserFiltersControl.IsUnjoinModel = true;

        //    if (!string.IsNullOrEmpty(GroupsSearchControl.CurrentQuery.CustomFilters))
        //    {
        //        try
        //        {
        //            objUserFiltersControl.GroupFilter = JsonConvert.DeserializeObject<FdGroupFilterModel>(GroupsSearchControl.CurrentQuery.CustomFilters);

        //        }
        //        catch (Exception ex)
        //        {
        //            ex.DebugLog();
        //        }
        //    }
        //    else
        //    {

        //        objUserFiltersControl.GroupFilter = new FdGroupFilterModel();
        //    }

        //    Dialog objDialog = new Dialog();

        //    var FilterWindow = objDialog.GetMetroWindow(objUserFiltersControl, "Filter");

        //    objUserFiltersControl.SaveButton.Click += (senders, Events) =>
        //    {

        //        var UserFilter = objUserFiltersControl.GroupFilter;
        //        var SerializeCustomFilter = JsonConvert.SerializeObject(UserFilter);
        //        GroupsSearchControl.CurrentQuery.CustomFilters = SerializeCustomFilter;

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