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
    public class
        IncommingFriendRequestBase : ModuleSettingsUserControl<IncommingFriendRequestViewModel,
            IncommingFriendRequestModel>
    {
        protected override bool ValidateCampaign()
        {
            if (!Model.ManageFriendsModel.IsAcceptRequest && !Model.ManageFriendsModel.IsCancelReceivedRequest)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectAtleastOneOption".FromResourceDictionary());
                return false;
            }

            if (Model.IsAcceptFilterVisible && (Model.GenderAndLocationFilter.IsFilterByGender &&
                                                !Model.GenderAndLocationFilter.SelectMaleUser &&
                                                !Model.GenderAndLocationFilter.SelectFemaleUser))
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectAcceptGenderFilter".FromResourceDictionary());
                return false;
            }

            if (Model.IsCancelFilterVisible && (Model.GenderAndLocationCancelFilter.IsFilterByGender &&
                                                !Model.GenderAndLocationCancelFilter.SelectMaleUser &&
                                                !Model.GenderAndLocationCancelFilter.SelectFemaleUser))
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectCancelGenderFilter".FromResourceDictionary());
                return false;
            }

            if (Model.IsAcceptFilterVisible && (Model.GenderAndLocationFilter.IsLocationFilterChecked
                                                && (Model.GenderAndLocationFilter.ListLocationUrl.Count == 0
                                                    && Model.GenderAndLocationFilter.ListLocationUrlPair.Count == 0)))
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyEnterCancelLocationFilter".FromResourceDictionary());
                return false;
            }

            if (Model.IsCancelFilterVisible && (Model.GenderAndLocationCancelFilter.IsLocationFilterChecked
                                                && Model.GenderAndLocationFilter.ListLocationUrl.Count == 0
                                                && Model.GenderAndLocationCancelFilter.ListLocationUrlPair.Count == 0))
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyEnterCancelLocationFilter".FromResourceDictionary());
                return false;
            }


            if (Model.GenderAndLocationFilter.IsMutualFriendsCountFilterSelected &&
                !Model.GenderAndLocationFilter.IsNoOfMutualFriend &&
                !Model.GenderAndLocationFilter.IsNoOfMutualFriendSmallerThan)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectMutualFriendsCountFilter".FromResourceDictionary());
                return false;
            }

            if (Model.GenderAndLocationCancelFilter.IsMutualFriendsCountFilterSelected &&
                !Model.GenderAndLocationCancelFilter.IsNoOfMutualFriend &&
                !Model.GenderAndLocationCancelFilter.IsNoOfMutualFriendSmallerThan)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectMutualFriendsCountFilter".FromResourceDictionary());
                return false;
            }

            if (Model.GenderAndLocationFilter.IsNoOfMutualFriend &&
                Model.GenderAndLocationFilter.TotalNoOfMutualFriend == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyEnterNoOfMutualFriendFilter".FromResourceDictionary());
                return false;
            }

            if (Model.GenderAndLocationCancelFilter.IsNoOfMutualFriend &&
                Model.GenderAndLocationCancelFilter.TotalNoOfMutualFriend == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyEnterNoOfMutualFriendFilter".FromResourceDictionary());
                return false;
            }

            if (Model.GenderAndLocationFilter.IsFriendOfFriend && Model.GenderAndLocationFilter.ListFriends.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LAngKeyEnterFriendOfFriendFilter".FromResourceDictionary());
                return false;
            }

            if (Model.GenderAndLocationCancelFilter.IsFriendOfFriend &&
                Model.GenderAndLocationCancelFilter.ListFriends.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LAngKeyEnterFriendOfFriendFilter".FromResourceDictionary());
                return false;
            }

            return base.ValidateCampaign();
        }
    }


    /// <summary>
    ///     Interaction logic for IncommingFriendRequest.xaml
    /// </summary>
    public partial class IncommingFriendRequest
    {
        private IncommingFriendRequest()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: HeaderGrid,
                footer: ManageFriendsFooter,
                queryControl: null,
                MainGrid: MainGrid,
                activityType: ActivityType.IncommingFriendRequest,
                moduleName: FdMainModule.Friends.ToString()
            );
            // Help control links. 
            VideoTutorialLink = FdConstants.IncommingFriendsVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.IncommingFriendsKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static IncommingFriendRequest CurrentIncommingFriendRequest { get; set; }

        public static IncommingFriendRequest GetSingeltonObjectIncommingFriendRequest()
        {
            return CurrentIncommingFriendRequest ?? (CurrentIncommingFriendRequest = new IncommingFriendRequest());
        }


        #region OldEvents

        //private void HeaderControl_OnInfoChanged(object sender, RoutedEventArgs e)
        //{

        //    HelpFlyout.IsOpen = true;
        //}

        //private void ManageFriendsFooter_SelectAccountChanged(object sender, RoutedEventArgs e)
        //    => base.FooterControl_OnSelectAccountChanged(sender, e);

        //private void ManageFriendsFooter_CreateCampaignChanged(object sender, RoutedEventArgs e)
        //    => base.CreateCampaign();


        //private void ManageFriendsFooter_UpdateCampaignChanged(object sender, RoutedEventArgs e)
        //    => UpdateCampaign();


        //private void HeaderOnCancelEdit_Click(object sender, RoutedEventArgs e)
        //{
        //    base.HeaderControl_OnCancelEditClick(sender, e);
        //    TabSwitcher.GoToCampaign();
        //} 

        #endregion
    }
}