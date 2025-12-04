using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using PinDominatorCore.PDModel;

namespace PinDominatorCore.PDViewModel.PinMessenger
{
    public class AutoReplyToNewMessageViewModel : BindableBase
    {
        private AutoReplyToNewMessageModel _autoReplyToNewMessegeModel = new AutoReplyToNewMessageModel();

        public AutoReplyToNewMessageViewModel()
        {
            AutoReplyToNewMessageModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfAutoReplyToNewMessegePerJob".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfAutoReplyToNewMessegePerDay".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfAutoReplyToNewMessegePerHour".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfAutoReplyToNewMessegePerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxAutoReplyToNewMessegePerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.Add(new QueryContent
            {
                Content = new QueryInfo
                {
                    QueryType = "All",
                    QueryValue = "All"
                }
            });
            AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.Add(new QueryContent
            {
                Content = new QueryInfo
                {
                    QueryType = "Message Filter",
                    QueryValue = Application.Current
                        .FindResource(
                            "LangKeyReplyToNewPendingMessagesReplyOnlyMessageSentByUsersThatDontFollowYourAccount")
                        ?.ToString()
                }
            });
            AddMessagesCommand = new BaseCommand<object>(sender => true, AddMessages);
            AddSpecificWordTextCommand = new BaseCommand<object>(sender => true, UploadSpecificWordText);
            ReplyToNewPendingMessagesCommand = new BaseCommand<object>(sender => true, ReplyToNewPendingMessage);
            ReplyToAllMessagesCommand = new BaseCommand<object>(sender => true, ReplyToAllMessage);
        }

        public AutoReplyToNewMessageModel Model => AutoReplyToNewMessageModel;

        public AutoReplyToNewMessageModel AutoReplyToNewMessageModel
        {
            get => _autoReplyToNewMessegeModel;
            set
            {
                if ((_autoReplyToNewMessegeModel == null) & (_autoReplyToNewMessegeModel == value))
                    return;
                SetProperty(ref _autoReplyToNewMessegeModel, value);
            }
        }

        #region Commands

        public ICommand AddMessagesCommand { get; set; }
        public ICommand AddSpecificWordTextCommand { get; set; }
        public ICommand ReplyToNewPendingMessagesCommand { get; set; }
        public ICommand ReplyToAllMessagesCommand { get; set; }

        #endregion

        #region Methods

        private void AddMessages(object sender)
        {
            var messageData = sender as MessagesControl;

            if (messageData == null) return;

            messageData.Messages.SelectedQuery =
                new ObservableCollection<QueryContent>(messageData.Messages.LstQueries.Where(x => x.IsContentSelected));

            messageData.Messages.MessagesText = messageData.Messages.MessagesText.Trim();
            if (messageData.Messages.SelectedQuery.Count == 0 ||
                string.IsNullOrEmpty(messageData.Messages.MessagesText))
            {
                Dialog.ShowDialog(Application.Current.MainWindow, "LangKeyWarning".FromResourceDictionary(),
                    "LangKeyPleaseTypeSomeMessage".FromResourceDictionary());
                return;
            }

            // messageData.Messages.MessageId = ObjViewModel.BroadcastMessagesModel.LstDisplayManageMessageModel.Count + 1;
            messageData.Messages.SelectedQuery.Remove(
                messageData.Messages.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));

            var messageList = messageData.Messages.MessagesText.Split('\n').Where(x => !string.IsNullOrEmpty(x.Trim()))
                .Select(y => y.Trim()).Distinct().ToList();

            if (AutoReplyToNewMessageModel.IsChkAddMultipleMessages)
                messageList.ForEach(message =>
                {
                    messageData.Messages.MessageId = Utilities.GetGuid();
                    AddToMessageList(messageData.Messages, message);
                });
            else
                Model.LstDisplayManageMessagesModel.Add(messageData.Messages);

            messageData.Messages = new ManageMessagesModel
            {
                LstQueries = Model.ManageMessagesModel.LstQueries
            };
            Model.ManageMessagesModel = messageData.Messages;
            messageData.ComboBoxQueries.ItemsSource = Model.ManageMessagesModel.LstQueries;
        }

        private void AddToMessageList(ManageMessagesModel messageModel, string messageText)
        {
            try
            {
                var isContain = false;
                AutoReplyToNewMessageModel.LstDisplayManageMessagesModel.ForEach(lstMessage =>
                {
                    if (lstMessage.MessagesText.ToLower().Equals(messageText.ToLower()))
                        isContain = lstMessage.SelectedQuery.Any(x => messageModel.SelectedQuery.Contains(x));
                });

                if (!isContain)
                    AutoReplyToNewMessageModel.LstDisplayManageMessagesModel.Add(new ManageMessagesModel
                    {
                        MessagesText = messageText,
                        SelectedQuery =
                            new ObservableCollection<QueryContent>(messageModel.LstQueries
                                .Where(x => x.IsContentSelected).ToList()),
                        MessageId = messageModel.MessageId,
                        LstQueries = messageModel.LstQueries
                    });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void UploadSpecificWordText(object sender)
        {
            try
            {
                AutoReplyToNewMessageModel.LstMessagesContainsSpecificWords = Regex.Split(
                    AutoReplyToNewMessageModel.ReplyToMessagesThatContainsSpecificWordText, "\r\n").ToList();
                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest, "",
                    ActivityType.AutoReplyToNewMessage,
                    "LangKeySuccessfullyUploaded".FromResourceDictionary());
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void ReplyToNewPendingMessage(object sender)
        {
            if (!AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.Any(x => x.Content.QueryValue.Equals(Application.Current
                         .FindResource(
                             "LangKeyReplyToNewPendingMessagesReplyOnlyMessageSentByUsersThatDontFollowYourAccount")
                         ?.ToString())))
                AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.Add(new QueryContent
                {
                    Content = new QueryInfo
                    {
                        QueryType = "Message Filter",
                        QueryValue = Application.Current
                            .FindResource(
                                "LangKeyReplyToNewPendingMessagesReplyOnlyMessageSentByUsersThatDontFollowYourAccount")
                            ?.ToString()
                    }
                });
            var delete = AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.FirstOrDefault(x =>
                x.Content.QueryValue == Application.Current.FindResource("LangKeyReplyToAllMessages")?.ToString());

            AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.Remove(delete);
            AutoReplyToNewMessageModel.LstDisplayManageMessagesModel.ForEach(item =>
                item.SelectedQuery.Remove(delete));

            if (AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.Any(x => x.IsContentSelected == false))
                AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.FirstOrDefault(x => x.Content.QueryType.Equals("All")).IsContentSelected = false;
        }

        private void ReplyToAllMessage(object sender)
        {
            if (!AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.Any(x => x.Content.QueryValue.Equals(Application.Current
                         .FindResource(
                             "LangKeyReplyToAllMessages")
                         ?.ToString())))
                AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.Add(new QueryContent
                {
                    Content = new QueryInfo
                    {
                        QueryType = "Message Filter",
                        QueryValue = Application.Current.FindResource("LangKeyReplyToAllMessages")?.ToString()
                    }
                });

            var delete = AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.FirstOrDefault(x =>
                x.Content.QueryValue == Application.Current
                    .FindResource(
                        "LangKeyReplyToNewPendingMessagesReplyOnlyMessageSentByUsersThatDontFollowYourAccount")
                    ?.ToString());

            AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.Remove(delete);
            AutoReplyToNewMessageModel.LstDisplayManageMessagesModel.ForEach(item =>
                item.SelectedQuery.Remove(delete));

            if (AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.Any(x => x.IsContentSelected == false))
                AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.FirstOrDefault(x => x.Content.QueryType.Equals("All")).IsContentSelected = false;
        }
        #endregion
    }
}