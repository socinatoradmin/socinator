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
    public class CancelSentRequestBase : ModuleSettingsUserControl<CancenSentRequestViewModel, CancelSentRequestModel>
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

            if (Model.GenderAndLocationFilter.IsLocationFilterChecked
                && Model.GenderAndLocationFilter.ListLocationUrl.Count == 0
                && Model.GenderAndLocationFilter.ListLocationUrlPair.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectLocationAndSave".FromResourceDictionary());
                return false;
            }

            return base.ValidateCampaign();
        }
    }


    /// <summary>
    ///     Interaction logic for CancelSentRequest.xaml
    /// </summary>
    public partial class CancelSentRequest
    {
        public CancelSentRequest()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: CancelRequestHeader,
                footer: CancelRequestFooter,
                queryControl: null,
                MainGrid: MainGrid,
                activityType: ActivityType.WithdrawSentRequest,
                moduleName: FdMainModule.Friends.ToString()
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.WithDrawVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.WithDrawKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }


        private static CancelSentRequest CurrentCancelSentRequest { get; set; }

        public static CancelSentRequest GetSingeltonObjectCancelSentRequest()
        {
            return CurrentCancelSentRequest ?? (CurrentCancelSentRequest = new CancelSentRequest());
        }

        #region Old Events

        //private void HeaderControl_OnInfoChanged(object sender, RoutedEventArgs e)
        //{
        //    HelpFlyout.IsOpen = true;
        //}

        //private void HeaderOnCancelEdit_Click(object sender, RoutedEventArgs e)
        //{
        //    base.HeaderControl_OnCancelEditClick(sender, e);
        //    TabSwitcher.GoToCampaign();
        //}

        //private void CancelRequestFooter_SelectAccountChanged(object sender, RoutedEventArgs e)
        //    => base.FooterControl_OnSelectAccountChanged(sender, e);

        //private void CancelRequestFooter_CreateCampaignChanged(object sender, RoutedEventArgs e)
        //    => base.CreateCampaign();

        //private void CancelRequestFooter_UpdateCampaignChanged(object sender, RoutedEventArgs e)
        //    => base.UpdateCampaign(); 

        #endregion
    }
}