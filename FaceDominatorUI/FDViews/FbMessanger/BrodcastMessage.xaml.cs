using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.MessageModel;
using FaceDominatorCore.FDViewModel.MessageViewModel;
using MahApps.Metro.Controls.Dialogs;

namespace FaceDominatorUI.FDViews.FbMessanger
{
    public class BrodcastMessageBase : ModuleSettingsUserControl<BrodcastMessageViewModel, BrodcastMessageModel>
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

            if (Model.LstDisplayManageMessageModel.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyAddAtLeastOneMessage".FromResourceDictionary());
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


    public partial class BrodcastMessage
    {
        private BrodcastMessage()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: BrodCastMessageHeader,
                footer: BrodCastMessageFooter,
                queryControl: BroadcastMessagesSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.BroadcastMessages,
                moduleName: FdMainModule.Messanger.ToString()
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.BrodcastMessageVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.BrodcastMessageKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }


        private static BrodcastMessage CurrentBrodcastMessage { get; set; }

        public static BrodcastMessage GetSingeltonObjectBrodcastMessage()
        {
            return CurrentBrodcastMessage ?? (CurrentBrodcastMessage = new BrodcastMessage());
        }

        #region Old Events

        //private void HeaderControl_OnInfoChanged(object sender, RoutedEventArgs e)
        //{
        //    HelpFlyout.IsOpen = true;
        //}

        //private void MessageContol_AddMessagesToList(object sender, RoutedEventArgs e)
        //{

        //    var MessageData = sender as MessageMediaControl;//MessagesControl;
        //    MessageData.Messages.SelectedQuery.Remove(QueryContent);
        //    ObjViewModel.BrodcastMessageModel.LstManageMessagesModel.Add(MessageData.Messages);
        //    try
        //    {

        //        MessageData.Messages = new ManageMessagesModel();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageData.Messages = new ManageMessagesModel();
        //    }

        //    MessageData.Messages.LstQueries = ObjViewModel.BrodcastMessageModel.ManageMessagesModel.LstQueries;
        //    MessageData.Messages.LstQueries.Select(query => { query.IsContentSelected = false; return query; }).ToList();

        //    ObjViewModel.BrodcastMessageModel.ManageMessagesModel = MessageData.Messages;
        //    MessageData.ComboBoxQueries.ItemsSource = ObjViewModel.BrodcastMessageModel.ManageMessagesModel.LstQueries;
        //}


        //void SendRequestFooter_SelectAccountChanged(object sender, RoutedEventArgs e)
        //    => base.FooterControl_OnSelectAccountChanged(sender, e);

        //void SendRequestFooter_CreateCampaignChanged(object sender, RoutedEventArgs e)
        //{
        //    base.CreateCampaign();
        //    ObjViewModel.BrodcastMessageModel.ManageMessagesModel.LstQueries.Clear();
        //}

        //void SendRequestFooter_UpdateCampaignChanged(object sender, RoutedEventArgs e)
        //{
        //    UpdateCampaign();
        //    ObjViewModel.BrodcastMessageModel.ManageMessagesModel.LstQueries.Clear();
        //}

        //private void FindFriendsSearchControl_OnAddQuery(object sender, RoutedEventArgs e)
        //{
        //    ObjViewModel.BrodcastMessageModel.ManageMessagesModel.LstQueries.Add(new QueryContent { Content = FindFriendsSearchControl.CurrentQuery });
        //    base.SearchQueryControl_OnAddQuery(sender, e, typeof(FdUserQueryParameters));
        //}
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

        //private void BrodcastMessage_OnDeleteQuery(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        var queryToDelete = ObjViewModel.BrodcastMessageModel.ManageMessagesModel.LstQueries.FirstOrDefault(x =>
        //                x.Content.QueryValue == FindFriendsSearchControl.CurrentQuery.QueryValue
        //                && x.Content.QueryType == FindFriendsSearchControl.CurrentQuery.QueryType);
        //        ObjViewModel.BrodcastMessageModel.ManageMessagesModel.LstQueries.Remove(queryToDelete);

        //        bool isAnyQueryLeft = true;

        //        while (isAnyQueryLeft)
        //        {
        //            if (ObjViewModel.BrodcastMessageModel.LstManageMessagesModel.FirstOrDefault(x => x.LstQueries.FirstOrDefault(y => y.Content.Id == queryToDelete.Content.Id) != null) != null)
        //            {
        //                var messagemodel = ObjViewModel.BrodcastMessageModel.LstManageMessagesModel.FirstOrDefault(x => x.LstQueries.FirstOrDefault(y => y.Content.Id == queryToDelete.Content.Id) != null);
        //                ObjViewModel.BrodcastMessageModel.LstManageMessagesModel.Remove(messagemodel);
        //            }
        //            else
        //            {
        //                isAnyQueryLeft = false;
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //}

        //private void BrodcastMessage_OnLoad(object sender, RoutedEventArgs e)
        //{

        //} 

        #endregion
    }
}