using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using TwtDominatorCore.TDModels;

namespace TwtDominatorCore.TDViewModel.TwtMessenger
{
    public class MessageViewModel : BindableBase
    {
        public static bool IsBroadCastMessageModule;
        private MessageModel _messageModel = new MessageModel();

        public MessageViewModel()
        {
            try
            {
                MessageModel.JobConfiguration = new JobConfiguration
                {
                    ActivitiesPerJobDisplayName = "LangKeyNumberOfMessagesPerJob".FromResourceDictionary(),
                    ActivitiesPerHourDisplayName = "LangKeyNumberOfMessagesPerHour".FromResourceDictionary(),
                    ActivitiesPerDayDisplayName = "LangKeyNumberOfMessagesPerDay".FromResourceDictionary(),
                    ActivitiesPerWeekDisplayName = "LangKeyNumberOfMessagesPerWeek".FromResourceDictionary(),
                    IncreaseActivityDisplayName = "LangKeyMaxMessagePerDay".FromResourceDictionary(),
                    RunningTime = RunningTimes.DayWiseRunningTimes,
                    Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
                    //"Default" "Random Follower" "Custom"
                };

                AddMediaMessageCommand = new BaseCommand<object>(sender => true, AddMediaMessageWithQueryExecute);
                //  AddMediaMessageWithoutQueryCommand = new BaseCommand<object>((sender) => true, AddMediaMessageWithoutQueryExecute);
                SplitInputToListCommand = new BaseCommand<object>(sender => true, SplitInputToListExecute);
                SpecificWordListCommand = new BaseCommand<object>(sender => true, SpecificWordListExecute);

                AddToList("Default");

                // load queries for broadcast module
                if (IsBroadCastMessageModule)
                {
                    AddBroadCastQueries();
                    IsBroadCastMessageModule = false;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        // public ICommand AddMediaMessageWithoutQueryCommand { get; set; }
        public ICommand AddMediaMessageCommand { get; set; }
        public ICommand SplitInputToListCommand { get; set; }
        public ICommand SpecificWordListCommand { get; set; }

        public MessageModel MessageModel
        {
            get => _messageModel;
            set => SetProperty(ref _messageModel, value);
        }

        public MessageModel Model => MessageModel;

        private void AddMediaMessageWithQueryExecute(object sender)
        {
            try
            {
                var messageData = sender as MessageMediaControl;

                if (ValidateQuery(messageData))
                    return;
                messageData.Messages.SelectedQuery =
                    new ObservableCollection<QueryContent>(
                        messageData.Messages.LstQueries.Where(x => x.IsContentSelected));
                // messageData.Messages.MessageId = ObjViewModel.BroadcastMessagesModel.LstDisplayManageMessageModel.Count + 1;
                messageData.Messages.SelectedQuery.Remove(
                    messageData.Messages.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));

                Model.LstDisplayManageMessageModel.Add(messageData.Messages);
                var unselectQueries = Model.ManageMessagesModel.LstQueries;
                unselectQueries.ForEach(x => x.IsContentSelected = false);
                messageData.Messages = new ManageMessagesModel
                {
                    LstQueries = unselectQueries
                };

                messageData.Messages.LstQueries.Select(query =>
                {
                    query.IsContentSelected = false;
                    return query;
                }).ToList();

                Model.ManageMessagesModel = messageData.Messages;
                messageData.ComboBoxQueries.ItemsSource = Model.ManageMessagesModel.LstQueries;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void SplitInputToListExecute(object sender)
        {
            try
            {
                MessageModel.MessageSetting.CustomFollowersList = Regex
                    .Split(MessageModel.MessageSetting.CustomFollowers.Trim(), "\r\n").Select(x => x.Trim()).ToList();
                if (MessageModel.MessageSetting.CustomFollowersList.Count != 0)
                    MessageModel.MessageSetting.CustomFollowersList =
                        MessageModel.MessageSetting
                            .CustomFollowersList; //.Select(x => Uri.UnescapeDataString(Uri.EscapeDataString(x).Replace("%20%E2%80%8F", "")).Trim()).ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void SpecificWordListExecute(object obj)
        {
            try
            {
                var SpecificWordList = MessageModel.SpecificWord.Split(',').Select(x => x.Trim()).ToList();

                MessageModel.ManageMessagesModel.LstQueries.Clear();

                AddToList("Default");

                SpecificWordList.ForEach(word =>
                {
                    if (!MessageModel.ManageMessagesModel.LstQueries.ToList().Any(x =>
                            x.Content.QueryValue.ToLower().Trim().Equals(word.ToLower().Trim())) &&
                        !string.IsNullOrEmpty(word.Trim()))
                        AddToList(word);
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private bool ValidateQuery(MessageMediaControl messageData)
        {
            var IsInValid = true;
            try
            {
                IsInValid = messageData == null ||
                            string.IsNullOrEmpty(messageData.Messages?.MessagesText?.Trim()) &&
                            string.IsNullOrEmpty(messageData.Messages?.MediaPath?.Trim()) || messageData.Messages
                                .LstQueries.Where(x => x.IsContentSelected).ToList().Count == 0;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return IsInValid;
        }

        #region BroadCast Queries

        public void AddBroadCastQueries()
        {
            AddToList("Random Follower");
            AddToList("Custom");
        }

        private void AddToList(string keyword)
        {
            try
            {
                if (MessageModel.ManageMessagesModel.LstQueries.All(x => x.Content.QueryValue != keyword))
                    MessageModel.ManageMessagesModel.LstQueries.Add(new QueryContent
                    {
                        Content = new QueryInfo
                        {
                            QueryValue =
                                keyword /*QueryType=FindResource("TdLangReplyToContainSpecificWord").ToString()*/
                        }
                    });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion
    }
}