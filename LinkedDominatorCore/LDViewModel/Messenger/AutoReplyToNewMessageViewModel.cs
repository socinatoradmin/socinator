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
using LinkedDominatorCore.LDModel.Messenger;

namespace LinkedDominatorCore.LDViewModel.Messenger
{
    public class AutoReplyToNewMessageViewModel : BindableBase
    {
        private AutoReplyToNewMessageModel _autoReplyToNewMessageModel = new AutoReplyToNewMessageModel();

        public AutoReplyToNewMessageViewModel()
        {
            AutoReplyToNewMessageModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfMessagesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfMessagesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfMessagesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfMessagesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxConnectionsToMessagePerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            AddQueries();

            ReplyToMessagesThatContainSpecificWordCommand =
                new BaseCommand<object>(sender => true, ReplyToMessagesThatContainSpecificWord);
            SaveSpecificWordCommand = new BaseCommand<object>(sender => true, SaveSpecificWord);
            AddMessagesCommand = new BaseCommand<object>(sender => true, AddMessages);
        }

        public AutoReplyToNewMessageModel AutoReplyToNewMessageModel
        {
            get => _autoReplyToNewMessageModel;
            set
            {
                if ((_autoReplyToNewMessageModel == null) & (_autoReplyToNewMessageModel == value))
                    return;
                SetProperty(ref _autoReplyToNewMessageModel, value);
            }
        }

        public AutoReplyToNewMessageModel Model => AutoReplyToNewMessageModel;

        public ICommand ReplyToMessagesThatContainSpecificWordCommand { get; set; }
        public ICommand SaveSpecificWordCommand { get; set; }
        public ICommand AddMessagesCommand { get; set; }

        public void ReplyToMessagesThatContainSpecificWord(object sender)
        {
            try
            {
                // AutoReplyToNewMessageModel.SpecificWord = string.Empty;
                if (!AutoReplyToNewMessageModel.IsReplyToMessagesThatContainSpecificWord﻿Checked)
                {
                    AutoReplyToNewMessageModel.SpecificWord = string.Empty;
                    var count = AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.Count;
                    while (count > 1)
                    {
                        var Content = AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries[count - 1].Content;
                        if (Content.QueryValue != "All" &&
                            Content.QueryValue !=
                            Application.Current.FindResource("LangKeyReplyToAllMessages").ToString() &&
                            Content.QueryValue != Application.Current
                                .FindResource("LangKeyReplyToAllUserMessageWhodidnotreply")
                                .ToString())
                            AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.RemoveAt(count - 1);
                        count--;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void SaveSpecificWord(object sender)
        {
            try
            {
                var LstSpecificWords = Regex.Split(AutoReplyToNewMessageModel.SpecificWord, ",").ToList();

                var count = AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.Count;

                while (count > 1)
                {
                    var Content = AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries[count - 1].Content;
                    if (Content.QueryValue != "All" &&
                        Content.QueryValue !=
                        Application.Current.FindResource("LangKeyReplyToAllMessages").ToString() &&
                        Content.QueryValue != Application.Current
                            .FindResource("LangKeyReplyToAllUserMessageWhodidnotreply")
                            .ToString()) AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.RemoveAt(count - 1);
                    count--;
                }

                LstSpecificWords.ForEach(x =>
                {
                    if (AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.All(y => y.Content.QueryValue != x))
                        AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.Add(new QueryContent
                        {
                            Content = new QueryInfo
                            {
                                QueryValue = x
                            }
                        });
                });

                GlobusLogHelper.log.Info(LstSpecificWords.Count +
                                         " specific words saved and added to query sucessfully!");
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        public void AddMessages(object sender)
        {
            try
            {
                var MessageData = sender as MessageMediaControl;

                #region Validations Before Adding Message to the list

                if (string.IsNullOrEmpty(MessageData.Messages.MessagesText?.Trim()) &&
                    string.IsNullOrEmpty(MessageData.Messages.MediaPath))
                    return;

                MessageData.Messages.SelectedQuery =
                    new ObservableCollection<QueryContent>(
                        MessageData.Messages.LstQueries.Where(x => x.IsContentSelected));

                var lstQuery = MessageData.Messages.SelectedQuery.Select(x => x.Content.QueryValue).ToList();


                if (!AutoReplyToNewMessageModel.IsReplyToAllMessagesChecked &&
                    lstQuery.Contains(Application.Current.FindResource("LangKeyReplyToAllMessages")))
                {
                    Dialog.ShowDialog("Error",
                        "Please make sure you have selected " +
                        Application.Current.FindResource("LangKeyReplyToAllMessages") + " as " +
                        Application.Current.FindResource("LangKeyMessageFilter") + " before adding to message list");
                    return;
                }

                #endregion

                MessageData.Messages.SelectedQuery.Remove(
                    MessageData.Messages.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));

                AutoReplyToNewMessageModel.LstDisplayManageMessagesModel.Add(MessageData.Messages);

                var lastQueries = MessageData.Messages.LstQueries;
                MessageData.Messages = new ManageMessagesModel();

                MessageData.Messages.LstQueries = lastQueries;
                MessageData.Messages.LstQueries.Select(query =>
                {
                    query.IsContentSelected = false;
                    return query;
                }).ToList();

                AutoReplyToNewMessageModel.ManageMessagesModel = MessageData.Messages;
                MessageData.ComboBoxQueries.ItemsSource = AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries;

                AddQueries();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void AddQuery(string keyResource)
        {
            try
            {
                var queryValue = keyResource.Equals("All")
                    ? "All"
                    : Application.Current.FindResource(keyResource).ToString();
                if (AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.All(x =>
                    x.Content.QueryValue != queryValue))
                    AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.Add(new QueryContent
                    {
                        Content = new QueryInfo
                        {
                            QueryValue = queryValue
                        }
                    });
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
        }

        public void AddQueries()
        {
            AddQuery("All");
            AddQuery("LangKeyReplyToAllMessages");
            AddQuery("LangKeyReplyToAllUserMessageWhodidnotreply");
        }
    }
}