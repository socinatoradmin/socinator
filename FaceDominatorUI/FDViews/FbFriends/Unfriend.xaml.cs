using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.FriendsModel;
using FaceDominatorCore.FDViewModel.FriendsViewModel;
using MahApps.Metro.Controls.Dialogs;

namespace FaceDominatorUI.FDViews.FbFriends
{
    public class UnfriendBase : ModuleSettingsUserControl<UnfriendViewModel, UnfriendModel>
    {
        protected override bool ValidateCampaign()
        {
            if (!Model.UnfriendOptionModel.IsAddedThroughSoftware && !Model.UnfriendOptionModel.IsAddedOutsideSoftware
                                                                  && !Model.UnfriendOptionModel.IsCustomUserList &&
                                                                  !Model.UnfriendOptionModel.IsMutualFriends)
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

            if (Model.UnfriendOptionModel.IsCustomUserList &&
                (Model.UnfriendOptionModel.LstCustomUsers == null ||
                 Model.UnfriendOptionModel.LstCustomUsers.Count == 0))
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyEnterTheUserUrls".FromResourceDictionary());
                return false;
            }

            if (Model.GenderAndLocationFilter.IsLocationFilterChecked
                && (Model.GenderAndLocationFilter.ListLocationUrl.Count == 0
                    && Model.GenderAndLocationFilter.ListLocationUrlPair.Count == 0))
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectLocationAndSave".FromResourceDictionary());
                return false;
            }

            return base.ValidateCampaign();
        }
    }

    public partial class Unfriend
    {
        public Unfriend()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: UnfriendHeader,
                footer: UnfriendFooter,
                queryControl: null,
                MainGrid: MainGrid,
                activityType: ActivityType.Unfriend,
                moduleName: FdMainModule.Friends.ToString()
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.UnfriendVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.UnfriendKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static Unfriend CurrentUnfriend { get; set; }

        public static Unfriend GetSingeltonObjectUnfriend()
        {
            return CurrentUnfriend ?? (CurrentUnfriend = new Unfriend());
        }


        #region OldEvents

        //void UnfriendFooter_SelectAccountChanged(object sender, RoutedEventArgs e)
        //   => base.FooterControl_OnSelectAccountChanged(sender, e);
        //void UnfriendFooter_CreateCampaignChanged(object sender, RoutedEventArgs e)
        //    => base.CreateCampaign();

        //void UnfriendFooter_UpdateCampaignChanged(object sender, RoutedEventArgs e)
        //    => UpdateCampaign();


        //private void HeaderOnCancelEdit_Click(object sender, RoutedEventArgs e)
        //{
        //    base.HeaderControl_OnCancelEditClick(sender, e);
        //    TabSwitcher.GoToCampaign();
        //}

        #endregion
    }
}