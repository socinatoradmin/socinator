using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.LDModel.Messenger;

namespace LinkedDominatorCore.LDViewModel.Messenger
{
    public class SendGreetingsToConnectionsViewModel : BindableBase
    {
        private SendGreetingsToConnectionsModel
            _SendGreetingsToConnectionsModel = new SendGreetingsToConnectionsModel();


        public SendGreetingsToConnectionsViewModel()
        {
            SendGreetingsToConnectionsModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfConnectionsToGreetPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfConnectionsToGreetPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfConnectionsToGreetPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfConnectionsToGreetPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyNumberOfConnectionsToGreetPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            #region Adding Default Query to MessagesControl

            #endregion

            //BirthdayGreetingCommand = new BaseCommand<object>((sender) => true, BirthdayGreeting);
            //NewJobGreetingCommand = new BaseCommand<object>((sender) => true, NewJobGreeting);
            //WorkAnniversaryGreetingCommand = new BaseCommand<object>((sender) => true, WorkAnniversaryGreeting);
            AddMessagesCommand = new BaseCommand<object>(sender => true, AddMessages);
            AddQueries();
        }

        public SendGreetingsToConnectionsModel SendGreetingsToConnectionsModel
        {
            get => _SendGreetingsToConnectionsModel;
            set => SetProperty(ref _SendGreetingsToConnectionsModel, value);
        }

        public SendGreetingsToConnectionsModel Model => SendGreetingsToConnectionsModel;

        public ICommand BirthdayGreetingCommand { get; set; }
        public ICommand NewJobGreetingCommand { get; set; }
        public ICommand WorkAnniversaryGreetingCommand { get; set; }
        public ICommand AddMessagesCommand { get; set; }

        public void BirthdayGreeting(object sender)
        {
            try
            {
                if (SendGreetingsToConnectionsModel.IsCheckedBirthdayGreeting)
                {
                    if (!SendGreetingsToConnectionsModel.ManageMessagesModel.LstQueries.Any(x =>
                        x.Content.QueryValue == Application.Current.FindResource("LangKeyBirthdayGreeting").ToString()))
                        SendGreetingsToConnectionsModel.ManageMessagesModel.LstQueries.Add(new QueryContent
                        {
                            Content = new QueryInfo
                            {
                                QueryValue = Application.Current.FindResource("LangKeyBirthdayGreeting").ToString()
                            }
                        });
                }
                else
                {
                    var QueryValue = SendGreetingsToConnectionsModel.ManageMessagesModel.LstQueries.FirstOrDefault(x =>
                        x.Content.QueryValue == Application.Current.FindResource("LangKeyBirthdayGreeting").ToString());
                    SendGreetingsToConnectionsModel.ManageMessagesModel.LstQueries.Remove(QueryValue);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void NewJobGreeting(object sender)
        {
            try
            {
                if (SendGreetingsToConnectionsModel.IsCheckedNewJobGreeting)
                {
                    if (!SendGreetingsToConnectionsModel.ManageMessagesModel.LstQueries.Any(x =>
                        x.Content.QueryValue == Application.Current.FindResource("LangKeyNewJobGreeting").ToString()))
                        SendGreetingsToConnectionsModel.ManageMessagesModel.LstQueries.Add(new QueryContent
                        {
                            Content = new QueryInfo
                            {
                                QueryValue = Application.Current.FindResource("LangKeyNewJobGreeting").ToString()
                            }
                        });
                }
                else
                {
                    var QueryValue = SendGreetingsToConnectionsModel.ManageMessagesModel.LstQueries.FirstOrDefault(x =>
                        x.Content.QueryValue == Application.Current.FindResource("LangKeyNewJobGreeting").ToString());
                    SendGreetingsToConnectionsModel.ManageMessagesModel.LstQueries.Remove(QueryValue);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void WorkAnniversaryGreeting(object sender)
        {
            try
            {
                if (SendGreetingsToConnectionsModel.IsCheckedWorkAnniversaryGreeting)
                {
                    if (!SendGreetingsToConnectionsModel.ManageMessagesModel.LstQueries.Any(x =>
                        x.Content.QueryValue == Application.Current.FindResource("LangKeyWorkAnniversaryGreeting")
                            .ToString()))
                        SendGreetingsToConnectionsModel.ManageMessagesModel.LstQueries.Add(new QueryContent
                        {
                            Content = new QueryInfo
                            {
                                QueryValue = Application.Current.FindResource("LangKeyWorkAnniversaryGreeting")
                                    .ToString()
                            }
                        });
                }
                else
                {
                    var QueryValue = SendGreetingsToConnectionsModel.ManageMessagesModel.LstQueries.FirstOrDefault(x =>
                        x.Content.QueryValue == Application.Current.FindResource("LangKeyWorkAnniversaryGreeting")
                            .ToString());
                    SendGreetingsToConnectionsModel.ManageMessagesModel.LstQueries.Remove(QueryValue);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void AddMessages(object sender)
        {
            try
            {
                var messageData = sender as MessagesControl;

                if (messageData == null) return;


                #region Validations Before Adding Message to the list

                
                if (string.IsNullOrEmpty(messageData.Messages.MessagesText?.Trim()) &&
                    string.IsNullOrEmpty(messageData.Messages.MediaPath))
                    return;

                messageData.Messages.SelectedQuery =
                    new ObservableCollection<QueryContent>(
                        messageData.Messages.LstQueries.Where(x => x.IsContentSelected));

                var lstQuery = messageData.Messages.SelectedQuery.Select(x => x.Content.QueryValue).ToList();

                if (lstQuery.Contains("All") &&
                    (!SendGreetingsToConnectionsModel.IsCheckedBirthdayGreeting ||
                     !SendGreetingsToConnectionsModel.IsCheckedNewJobGreeting ||
                     !SendGreetingsToConnectionsModel.IsCheckedWorkAnniversaryGreeting))
                {
                    Dialog.ShowDialog("Error",
                        "Please make sure you have selected all available connection source before adding to message list");
                    return;
                }

                if (!SendGreetingsToConnectionsModel.IsCheckedBirthdayGreeting &&
                    lstQuery.Contains(Application.Current.FindResource("LangKeyBirthdayGreeting")))
                {
                    Dialog.ShowDialog("Error",
                        "Please make sure you have selected " +
                        Application.Current.FindResource("LangKeyBirthdayGreeting") +
                        " as connection source before adding to message list");
                    
                    return;
                }

                if (!SendGreetingsToConnectionsModel.IsCheckedNewJobGreeting &&
                    lstQuery.Contains(Application.Current.FindResource("LangKeyNewJobGreeting")))

                {
                    Dialog.ShowDialog("Error",
                        "Please make sure you have selected " +
                        Application.Current.FindResource("LangKeyNewJobGreeting") +
                        " as connection source before adding to message list");
                   
                    return;
                }

                
                if (!SendGreetingsToConnectionsModel.IsCheckedWorkAnniversaryGreeting &&
                    lstQuery.Contains(Application.Current.FindResource("LangKeyWorkAnniversaryGreeting")))
                {
                    Dialog.ShowDialog("Error",
                        "Please make sure you have selected " +
                        Application.Current.FindResource("LangKeyWorkAnniversaryGreeting") +
                        " as connection source before adding to message list");

                    return;
                }
                #endregion

                messageData.Messages.SelectedQuery =
                    new ObservableCollection<QueryContent>(
                        messageData.Messages.LstQueries.Where(x => x.IsContentSelected));

                messageData.Messages.SelectedQuery.Remove(
                    messageData.Messages.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));

                SendGreetingsToConnectionsModel.LstDisplayManageMessagesModel.Add(messageData.Messages);

                messageData.Messages = new ManageMessagesModel
                {
                    LstQueries = SendGreetingsToConnectionsModel.ManageMessagesModel.LstQueries
                };
                AddQueries();
                messageData.Messages.LstQueries.Select(query =>
                {
                    query.IsContentSelected = false;
                    return query;
                }).ToList();

                SendGreetingsToConnectionsModel.ManageMessagesModel = messageData.Messages;
                messageData.ComboBoxQueries.ItemsSource =
                    SendGreetingsToConnectionsModel.ManageMessagesModel.LstQueries;
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
                
                if (SendGreetingsToConnectionsModel.ManageMessagesModel.LstQueries.All(x =>
                    x.Content.QueryValue != queryValue))
                    SendGreetingsToConnectionsModel.ManageMessagesModel.LstQueries.Add(new QueryContent
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
            AddQuery("LangKeyBirthdayGreeting");
            AddQuery("LangKeyNewJobGreeting");
            AddQuery("LangKeyWorkAnniversaryGreeting");
        }
    }
}