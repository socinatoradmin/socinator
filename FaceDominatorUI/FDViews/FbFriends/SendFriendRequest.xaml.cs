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
    public class SendFriendRequestBase : ModuleSettingsUserControl<SendFrinedRequestViewModel, SendFriendRequestModel>
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

            if (Model.GenderAndLocationFilter.IsLocationFilterChecked
                && Model.GenderAndLocationFilter.ListLocationUrl.Count == 0
                && Model.GenderAndLocationFilter.ListLocationUrlPair.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectLocationAndSave".FromResourceDictionary());
                return false;
            }


            if (Model.ChkCommentOnUserLatestPostsChecked)
                if (string.IsNullOrEmpty(Model.UploadComment))
                {
                    Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                        "LangKeyAddCommentSaveIt".FromResourceDictionary());
                    return false;
                }

            if (Model.ChkSendDirectMessageAfterFollowChecked)
                if (string.IsNullOrEmpty(Model.Message))
                {
                    Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                        "LangKeyTypeMessageSaveIt".FromResourceDictionary());
                    return false;
                }

            return base.ValidateCampaign();
        }
    }


    /// <summary>
    ///     Interaction logic for SendFriendRequest.xaml
    /// </summary>
    public partial class SendFriendRequest
    {
        private SendFriendRequest()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: SendRequestHeader,
                footer: SendRequestFooter,
                queryControl: FindFriendsSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.SendFriendRequest,
                moduleName: FdMainModule.Friends.ToString()
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.SendRequestVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.SendRequestKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static SendFriendRequest CurrentSendFriendRequest { get; set; }

        public static SendFriendRequest GetSingeltonObjectSendFriendRequest()
        {
            return CurrentSendFriendRequest ?? (CurrentSendFriendRequest = new SendFriendRequest());
        }

        #region Old Events

        //private void HeaderControl_OnInfoChanged(object sender, RoutedEventArgs e)
        //{
        //    HelpFlyout.IsOpen = true;
        //}


        //void SendRequestFooter_SelectAccountChanged(object sender, RoutedEventArgs e)
        //    => base.FooterControl_OnSelectAccountChanged(sender, e);

        //void SendRequestFooter_CreateCampaignChanged(object sender, RoutedEventArgs e)
        //    => base.CreateCampaign();

        //void SendRequestFooter_UpdateCampaignChanged(object sender, RoutedEventArgs e)
        //    => base.UpdateCampaign();

        //private void FindFriendsSearchControl_OnAddQuery(object sender, RoutedEventArgs e)
        //    => base.SearchQueryControl_OnAddQuery(sender, e, typeof(FdUserQueryParameters));

        //private void FindFriendsSearchControl_OnCustomFilterChanged(object sender, RoutedEventArgs e)
        //{
        //    GenderFilterControl objUserFiltersControl = new GenderFilterControl();

        //    objUserFiltersControl.IsSaveCloseButtonVisisble = true;

        //    objUserFiltersControl.GenderFilter = new FdGenderFilterModel();

        //    Dialog objDialog = new Dialog();

        //    var FilterWindow = objDialog.GetMetroWindow(objUserFiltersControl, "Filter");

        //    objUserFiltersControl.SaveButton.Click += (senders, Events) =>
        //    {

        //        var UserFilter = objUserFiltersControl.GenderFilter;
        //        var SerializeCustomFilter = JsonConvert.SerializeObject(UserFilter);
        //        FindFriendsSearchControl.CurrentQuery.CustomFilters = SerializeCustomFilter;

        //        FilterWindow.Close();
        //    };

        //    FilterWindow.ShowDialog();
        //}

        //private void HeaderOnCancelEdit_Click(object sender, RoutedEventArgs e)
        //{
        //    base.HeaderControl_OnCancelEditClick(sender, e);
        //    TabSwitcher.GoToCampaign();
        //}

        //private void UploadCommentInputBox_OnGetInputClick(object sender, RoutedEventArgs e)
        //{

        //}

        //private void AddMessageInputBox_OnGetInputClick(object sender, RoutedEventArgs e)
        //{

        //} 

        #endregion
    }
}