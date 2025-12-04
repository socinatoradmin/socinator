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
    public class GroupUnJoinerNewBase : ModuleSettingsUserControl<GroupUnjoinerViewModelNew, GroupUnjoinerModelNew>
    {
        protected override bool ValidateCampaign()
        {
            if (!Model.UnfriendOptionModel.IsAddedThroughSoftware && !Model.UnfriendOptionModel.IsAddedOutsideSoftware)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyselectAtLeastOneSource".FromResourceDictionary());
                return false;
            }

            if (Model.UnfriendOptionModel.IsFilterApplied &&
                (Model.UnfriendOptionModel.DaysBefore == 0 && Model.UnfriendOptionModel.HoursBefore == 0))
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectValidSourceFilter".FromResourceDictionary());
                return false;
            }

            return base.ValidateCampaign();
        }
    }


    /// <summary>
    ///     Interaction logic for GroupUnjoinerNew.xaml
    /// </summary>
    public partial class GroupUnjoinerNew
    {
        public GroupUnjoinerNew()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: GroupJoinerHeader,
                footer: GroupJoinerFooter,
                queryControl: null,
                MainGrid: MainGrid,
                activityType: ActivityType.GroupUnJoiner,
                moduleName: FdMainModule.Groups.ToString()
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.GroupUnJoinerVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.GroupUnJoinerKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static GroupUnjoinerNew CurrentGroupUnjoinerNew { get; set; }

        public static GroupUnjoinerNew GetSingeltonObjectGroupUnjoinerNew()
        {
            return CurrentGroupUnjoinerNew ?? (CurrentGroupUnjoinerNew = new GroupUnjoinerNew());
        }

        #region OldEvents

        //private void GroupJoinerFooter_UpdateCampaignChanged(object sender, RoutedEventArgs e)
        //   => UpdateCampaign();

        //private void GroupJoinerFooter_CreateCampaignChanged(object sender, RoutedEventArgs e)
        //    => base.CreateCampaign();


        //private void GroupJoinerFooter_SelectAccountChanged(object sender, RoutedEventArgs e)
        //    => base.FooterControl_OnSelectAccountChanged(sender, e);

        //private void HeaderControl_OnInfoChanged(object sender, RoutedEventArgs e)
        //{
        //    HelpFlyout.IsOpen = true;
        //}

        //private void HeaderOnCancelEdit_Click(object sender, RoutedEventArgs e)
        //{
        //    base.HeaderControl_OnCancelEditClick(sender, e);
        //    TabSwitcher.GoToCampaign();
        //} 

        #endregion
    }
}