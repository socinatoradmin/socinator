using DominatorHouseCore.Annotations;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeModel;
using YoutubeDominatorCore.YoutubeViewModel;
using YoutubeDominatorUI.YDViews.GrowSubscribers;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using YoutubeDominatorUI.Utility;
using static YoutubeDominatorCore.YDEnums.Enums;
using DominatorHouseCore.Enums.YdQuery;
using MahApps.Metro.Controls.Dialogs;
using DominatorHouseCore;

namespace YoutubeDominatorUI.YDViews.GrowSubscribers
{
    public class Message_Base : ModuleSettingsUserControl<MessageViewModel, MessageModel>
    {
        protected override bool ValidateCampaign()
        {
            // Check queries
            if (Model.SavedQueries.Count == 0)
            {
                DialogCoordinator.Instance.ShowModalMessageExternal(this, "Error", "Please add at least one query.",
                    MessageDialogStyle.Affirmative);
                return false;
            }
            return base.ValidateCampaign();
        }
    }

    public partial class Message : Message_Base
    {

        private static Message Current_Message { get; set; } = null;
        public static Message GetSingeltonObjectMessage()
        {
            return Current_Message ?? (Current_Message = new Message());
        }
        public Message()
        {

            InitializeComponent();

            base.InitializeBaseClass
            (
                header: HeaderGrid,
                footer: MessageFooter,
                queryControl: MessageSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.BroadcastMessages,
                moduleName: YdMainModule.Message.ToString()
            );

            base.VideoTutorialLink = ConstantHelpDetails.MessageVideoTutorialsLink;
            base.KnowledgeBaseLink = ConstantHelpDetails.MessageKnowledgeBaseLink;
            base.ContactSupportLink = ConstantHelpDetails.ContactSupportLink;

            Current_Message = this;
            try
            {
                base.SetDataContext();

                DialogParticipation.SetRegister(this, this);
                ObjViewModel.messageModel.ManageMessagesModel.LstQueries.Add(new QueryContent
                {
                    Content = new QueryInfo
                    {
                        QueryValue = "All",
                        QueryType = "All"
                    }
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }



        private MessageViewModel _objMessageViewModel;

        public MessageViewModel ObjMessageViewModel
        {
            get
            {
                return _objMessageViewModel;
            }
            set
            {
                _objMessageViewModel = value;
                OnPropertyChanged(nameof(ObjMessageViewModel));
            }
        }





        private void HeaderGrid_OnCancelEditClick(object sender, RoutedEventArgs e)
        {
            IsEditCampaignName = true;
            CancelEditVisibility = Visibility.Collapsed;
            MessageFooter.CampaignManager = ConstantVariable.CreateCampaign;
            SetDataContext();
            TabSwitcher.GoToCampaign();
        }

        private void MessageSearchControl_OnCustomFilterChanged(object sender, RoutedEventArgs e)
            => base.SearchQueryControl_OnCustomFilterChanged(sender, e);


        private void HeaderControl_OnInfoChanged(object sender, RoutedEventArgs e)
        {
            HelpFlyout.IsOpen = true;
        }

        private void MessageFooter_SelectAccountChanged(object sender, RoutedEventArgs e)
            => base.FooterControl_OnSelectAccountChanged(sender, e);


        private void MessageFooter_CreateCampaignChanged(object sender, RoutedEventArgs e)
            => base.CreateCampaign();
            //=> base.FooterControl_OnCreateCampaignChanged(sender, e, Application.Current.FindResource("LangKeyTheseAccountsAreAlreadyRunningWithMessageConfigurationFromAnotherCampaignSavingThisSettingsWillOverridePreviousSettingsAndRemoveThisAccountFromTheCampaign").ToString(), ObjViewModel.Model.JobConfiguration.RunningTime);
        //=> base.FooterControl_OnCreateCampaignChanged(sender, e);

        private void MessageFooter_OnUpdateCampaignChanged(object sender, RoutedEventArgs e)
            => UpdateCampaign();
      //=> base.FooterControl_OnUpdateCampaignChanged(sender, e);

        //private void MessageSearchControl_OnAddQuery(object sender, RoutedEventArgs e)
        //    => base.SearchQueryControl_OnAddQuery(sender, e, typeof(YdScraperParameters));


        private void MessageSearchControl_OnAddQuery(object sender, RoutedEventArgs e)
        {
            ObjViewModel.messageModel.ManageMessagesModel.LstQueries.Add(new QueryContent
            {
                Content = new QueryInfo
                {
                    QueryValue = MessageSearchControl.CurrentQuery.QueryValue,
                    QueryType = MessageSearchControl.CurrentQuery.QueryType

                }
            });
            base.SearchQueryControl_OnAddQuery(sender, e, typeof(YdScraperParameters));
        }

        private void MessagesControl_AddMessagesToListChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                var messageData = sender as MessagesControl;

                //commentData.Comments.SerialNo = ObjViewModel.messageModel.LstDisplayManageCommentModel.Count + 1;

                //Todo : please check below statement
                // commentData.Comments.SelectedQuery.Remove("All");

                ObjViewModel.messageModel.LstDisplayManagemessageModel.Add(messageData.Messages);

                messageData.Messages = new ManageMessagesModel
                {
                    LstQueries = ObjViewModel.messageModel.ManageMessagesModel.LstQueries
                };

                messageData.Messages.LstQueries = ObjViewModel.messageModel.ManageMessagesModel.LstQueries;
                messageData.Messages.LstQueries.Select(query => { query.IsContentSelected = false; return query; }).ToList();

                ObjViewModel.messageModel.ManageMessagesModel = messageData.Messages;
                messageData.ComboBoxQueries.ItemsSource = ObjViewModel.messageModel.ManageMessagesModel.LstQueries;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
