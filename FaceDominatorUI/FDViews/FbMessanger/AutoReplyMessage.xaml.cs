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
    public class AutoReplyMessageBase : ModuleSettingsUserControl<AutoReplyMessageViewModel, AutoReplyMessageModel>
    {
        protected override bool ValidateCampaign()
        {
            // Check queries
            if (!Model.AutoReplyOptionModel.IsFriendsMessageChecked &&
                !Model.AutoReplyOptionModel.IsMessageRequestChecked &&
                !Model.AutoReplyOptionModel.IsReplyToPageMessagesChecked)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyselectAtLeastOneSource".FromResourceDictionary());
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
            //if (Model.AutoReplyOptionModel.IsReplyToPageMessagesChecked && string.IsNullOrEmpty(Model.AutoReplyOptionModel.OwnPages))
            //{
            //    Dialog.ShowDialog(this, "Error", "Please Enter atleast One Own Page Url",
            //        MessageDialogStyle.Affirmative);
            //    return false;
            //}

            if (Model.LstDisplayManageMessageModel.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyAddAtLeastOneMessage".FromResourceDictionary());
                return false;
            }


            if (Model.AutoReplyOptionModel.IsFilterApplied &&
                (Model.AutoReplyOptionModel.DaysBefore == 0 && Model.AutoReplyOptionModel.HoursBefore == 0))
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectValidSourceFilter".FromResourceDictionary());
                return false;
            }


            return base.ValidateCampaign();
        }
    }


    /// <summary>
    ///     Interaction logic for AutoReplyMessage.xaml
    /// </summary>
    public partial class AutoReplyMessage
    {
        public AutoReplyMessage()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: SendRequestHeader,
                footer: SendRequestFooter,
                queryControl: null,
                MainGrid: MainGrid,
                activityType: ActivityType.AutoReplyToNewMessage,
                moduleName: FdMainModule.Messanger.ToString()
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.ReplyMessageVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.ReplyMessageKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            base.SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static AutoReplyMessage CurrentAutoReplyMessage { get; set; }

        public static AutoReplyMessage GetSingeltonObjectAutoReplyMessage()
        {
            return CurrentAutoReplyMessage ?? (CurrentAutoReplyMessage = new AutoReplyMessage());
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
        // => UpdateCampaign();


        //private void HeaderOnCancelEdit_Click(object sender, RoutedEventArgs e)
        //{
        //    base.HeaderControl_OnCancelEditClick(sender, e);
        //    TabSwitcher.GoToCampaign();
        //}


        //private void MessageContol_AddMessagesToList(object sender, RoutedEventArgs e)
        //{
        //    QueryContent QueryContent = new QueryContent { Content = new QueryInfo() { QueryValue = "All" } };

        //    var MessageData = sender as MessageMediaControl;

        //    if (MessageData.Messages.SelectedQuery.FirstOrDefault(x => x.Content.QueryType == "Reply to Message Requests") != null)
        //    {
        //        if (!ObjViewModel.AutoReplyMessageModel.AutoReplyOptionModel.IsMessageRequestChecked)
        //        {
        //            Dialog.ShowDialog(Application.Current.MainWindow, "Warning",
        //            "Please select the respective message source!!");
        //            return;
        //        }
        //    }

        //    if (MessageData.Messages.SelectedQuery.FirstOrDefault(x => x.Content.QueryType == "Reply to Connected Friends") != null)
        //    {
        //        if (!ObjViewModel.AutoReplyMessageModel.AutoReplyOptionModel.IsFriendsMessageChecked)
        //        {
        //            Dialog.ShowDialog(Application.Current.MainWindow, "Warning",
        //            "Please select the respective message source!!");
        //            return;
        //        }
        //    }


        //    // MessageData.Messages.SerialNo = ObjViewModel.AutoReplyMessageModel.LstManageMessagesModel.Count + 1;
        //    MessageData.Messages.SelectedQuery.Remove(QueryContent);
        //    ObjViewModel.AutoReplyMessageModel.LstManageMessagesModel.Add(MessageData.Messages);
        //    try
        //    {

        //        MessageData.Messages = new ManageMessagesModel();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageData.Messages = new ManageMessagesModel();
        //    }

        //    MessageData.Messages.LstQueries = ObjViewModel.AutoReplyMessageModel.ManageMessagesModel.LstQueries;
        //    MessageData.Messages.LstQueries.Select(query => { query.IsContentSelected = false; return query; }).ToList();

        //    ObjViewModel.AutoReplyMessageModel.ManageMessagesModel = MessageData.Messages;
        //    MessageData.ComboBoxQueries.ItemsSource = ObjViewModel.AutoReplyMessageModel.ManageMessagesModel.LstQueries;

        //}

        #endregion
    }
}