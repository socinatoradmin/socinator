using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.GroupsModel;
using FaceDominatorCore.FDViewModel.Groups;
using MahApps.Metro.Controls.Dialogs;

namespace FaceDominatorUI.FDViews.FbGroups
{
    public class GroupJoinerBase : ModuleSettingsUserControl<GroupJoinerViewModel, GroupJoinerModel>
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
    ///     Interaction logic for GroupJoiner.xaml
    /// </summary>
    public partial class GroupJoiner
    {
        public GroupJoiner()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: GroupJoinerHeader,
                footer: GroupJoinerFooter,
                queryControl: GroupsSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.GroupJoiner,
                moduleName: FdMainModule.Groups.ToString()
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.GroupJoinerVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.GroupJoinerKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }


        private static GroupJoiner CurrentGroupJoiner { get; set; }

        public static GroupJoiner GetSingeltonObjectGroupJoiner()
        {
            return CurrentGroupJoiner ?? (CurrentGroupJoiner = new GroupJoiner());
        }

        #region OldEvents

        //private void HeaderControl_OnInfoChanged(object sender, RoutedEventArgs e)
        //{
        //    HelpFlyout.IsOpen = true;
        //}

        //void GroupJoinerFooter_SelectAccountChanged(object sender, RoutedEventArgs e)
        //    => base.FooterControl_OnSelectAccountChanged(sender, e);

        //void GroupJoinerFooter_CreateCampaignChanged(object sender, RoutedEventArgs e)
        //    => base.CreateCampaign();

        //void GroupJoinerFooter_UpdateCampaignChanged(object sender, RoutedEventArgs e)
        //    => UpdateCampaign();

        //private void GroupsSearchControl_OnAddQuery(object sender, RoutedEventArgs e)
        //    => base.SearchQueryControl_OnAddQuery(sender, e, typeof(GroupJoinerParameter));

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