using System;
using System.Collections.Generic;
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
    public class BroadcastMessagesViewModel : BindableBase
    {
        private BroadcastMessagesModel _broadcastMessagesModel = new BroadcastMessagesModel();

        public BroadcastMessagesViewModel()
        {
            BroadcastMessagesModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfMessagesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfMessagesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfMessagesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfMessagesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxConnectionsToMessagePerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };


            SaveCustomUserListCommand = new BaseCommand<object>(sender => true, SaveCustomUsers);
            SaveCustomGroupListCommand = new BaseCommand<object>(sender => true, SaveCustomGroup);
            AddMessagesCommand = new BaseCommand<object>(sender => true, AddMessages);
            //CheckedBySoftwareCommand = new BaseCommand<object>((sender) => true, CheckedBySoftware);
            //CheckedByOutSideSoftwareCommand = new BaseCommand<object>((sender) => true, CheckedByOutSideSoftware);
            //CheckedByCustomUserListCommand = new BaseCommand<object>((sender) => true, CheckedByCustomUserList);
            AddQueries();
        }

        public BroadcastMessagesModel BroadcastMessagesModel
        {
            get => _broadcastMessagesModel;
            set => SetProperty(ref _broadcastMessagesModel, value);
        }

        public BroadcastMessagesModel Model => BroadcastMessagesModel;

        public ICommand SaveCustomUserListCommand { get; set; }
        public ICommand SaveCustomGroupListCommand { get; set; }
        public ICommand AddMessagesCommand { get; set; }
        public ICommand CheckedBySoftwareCommand { get; set; }
        public ICommand CheckedByOutSideSoftwareCommand { get; set; }
        public ICommand CheckedByCustomUserListCommand { get; set; }

        private void SaveCustomUsers(object sender)
        {
            try
            {
                if (BroadcastMessagesModel.UrlInput.Contains("\r\n"))
                {
                    BroadcastMessagesModel.UrlList =
                        Regex.Split(BroadcastMessagesModel.UrlInput, "\r\n").Where(x => !string.IsNullOrEmpty(x.Trim()))
                            .Distinct().ToList();

                    GlobusLogHelper.log.Info("" + BroadcastMessagesModel.UrlList.Count +
                                             " profile urls saved sucessfully");
                }
                else
                {
                    BroadcastMessagesModel.UrlList = new List<string>();
                    BroadcastMessagesModel.UrlList.Add(BroadcastMessagesModel.UrlInput);
                    GlobusLogHelper.log.Info("One profile url saved sucessfully");
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void SaveCustomGroup(object sender)
        {
            try
            {
                if (BroadcastMessagesModel.GroupUrlInput.Contains("\r\n"))
                {
                    BroadcastMessagesModel.GroupUrlList =
                        Regex.Split(BroadcastMessagesModel.GroupUrlInput, "\r\n")
                            .Where(x => !string.IsNullOrEmpty(x.Trim())).Distinct().ToList();

                    GlobusLogHelper.log.Info("" + BroadcastMessagesModel.GroupUrlList.Count +
                                             " group urls saved sucessfully");
                }
                else
                {
                    BroadcastMessagesModel.GroupUrlList = new List<string> {BroadcastMessagesModel.UrlInput};
                    GlobusLogHelper.log.Info("One group url saved sucessfully");
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
                var MessageData = sender as MessageMediaControl;

                #region Validations Before Adding Message to the list

                
                if (string.IsNullOrEmpty(MessageData.Messages.MessagesText?.Trim()) &&
                    string.IsNullOrEmpty(MessageData.Messages.MediaPath))
                    return;

                MessageData.Messages.SelectedQuery =
                    new ObservableCollection<QueryContent>(
                        MessageData.Messages.LstQueries.Where(x => x.IsContentSelected));

                var lstQuery = MessageData.Messages.SelectedQuery.Select(x => x.Content.QueryValue).ToList();

                if (lstQuery.Contains("All") && BroadcastMessagesModel.IsGroup)
                {
                    Dialog.ShowDialog("Error",
                        "Please make sure you have selected all available connection source before adding to message list or  select group source only");
                    return;
                }

                if (lstQuery.Any(x => x.Contains("Software") || x.Contains("Users List")) &&
                    BroadcastMessagesModel.IsGroup)
                {
                    Dialog.ShowDialog("Error",
                        "Please make sure you have selected all available connection source before adding to message list or  select group source only");
                    return;
                }


                if (lstQuery.Contains("All") &&
                    (!BroadcastMessagesModel.IsCheckedBySoftware ||
                     !BroadcastMessagesModel.IsCheckedOutSideSoftware ||
                     !BroadcastMessagesModel.IsCheckedLangKeyCustomUserList))
                {
                    Dialog.ShowDialog("Error",
                        "Please make sure you have selected all available connection source before adding to message list or  select group source only");
                    return;
                }

                if (!BroadcastMessagesModel.IsCheckedBySoftware &&
                    lstQuery.Contains(Application.Current.FindResource("LangKeyBySoftware")))
                {
                    Dialog.ShowDialog("Error",
                        "Please make sure you have selected " + Application.Current.FindResource("LangKeyBySoftware") +
                        " as connection source before adding to message list ");
                    return;
                }

                if (!BroadcastMessagesModel.IsCheckedOutSideSoftware &&
                    lstQuery.Contains(Application.Current.FindResource("LangKeyOutsideSoftware")))

                {
                    Dialog.ShowDialog("Error",
                        "Please make sure you have selected " +
                        Application.Current.FindResource("LangKeyOutsideSoftware") +
                        " as connection source before adding to message list");
                    return;
                }

                
                if (!BroadcastMessagesModel.IsCheckedLangKeyCustomUserList &&
                    lstQuery.Contains(Application.Current.FindResource("LangKeyCustomUsersList")))
                {
                    Dialog.ShowDialog("Error",
                        "Please make sure you have selected " +
                        Application.Current.FindResource("LangKeyCustomUsersList") +
                        " as connection source before adding to message list");
                    return;
                }

                if (BroadcastMessagesModel.IsCheckedLangKeyCustomUserList &&
                    string.IsNullOrEmpty(BroadcastMessagesModel.UrlInput))
                {
                    Dialog.ShowDialog("Error", "please input at least one custom user profile url");
                    return;
                }

                #endregion

                MessageData.Messages.SelectedQuery.Remove(
                    MessageData.Messages.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));

                BroadcastMessagesModel.LstDisplayManageMessagesModel.Add(MessageData.Messages);

                MessageData.Messages = new ManageMessagesModel();

                MessageData.Messages.LstQueries = BroadcastMessagesModel.ManageMessagesModel.LstQueries;
                MessageData.Messages.LstQueries.Select(query =>
                {
                    query.IsContentSelected = false;
                    return query;
                }).ToList();
                AddQueries();
                BroadcastMessagesModel.ManageMessagesModel = MessageData.Messages;
                MessageData.ComboBoxQueries.ItemsSource = BroadcastMessagesModel.ManageMessagesModel.LstQueries;
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
                if (BroadcastMessagesModel.ManageMessagesModel.LstQueries.All(x => x.Content.QueryValue != queryValue))
                    BroadcastMessagesModel.ManageMessagesModel.LstQueries.Add(new QueryContent
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
            AddQuery("LangKeyBySoftware");
            AddQuery("LangKeyOutsideSoftware");
            AddQuery("LangKeyCustomUsersList");
            AddQuery("LangKeyCustomGroupUrl");
            //AddQuery("LangKeyOwnFollowers");
        }

        #region last code

        //public void CheckedBySoftware(object sender)
        //{
        //    try
        //    {
        //        if (BroadcastMessagesModel.IsCheckedBySoftware)
        //        {
        //            if (!BroadcastMessagesModel.ManageMessagesModel.LstQueries.Any(x => x.Content.QueryValue == Application.Current.FindResource("LangKeyBySoftware")?.ToString()))
        //            {
        //                BroadcastMessagesModel.ManageMessagesModel.LstQueries.Add(new QueryContent
        //                {
        //                    Content = new QueryInfo
        //                    {
        //                        QueryValue = Application.Current.FindResource("LangKeyBySoftware")?.ToString()
        //                        //QueryType = FindResource("LangKeyGreetingOptions").ToString().Replace(" :", "")
        //                    }
        //                });

        //            }
        //        }
        //        else
        //        {
        //            var queryValue = BroadcastMessagesModel.ManageMessagesModel.LstQueries.FirstOrDefault(x => x.Content.QueryValue == Application.Current.FindResource("LangKeyBySoftware")?.ToString());
        //            BroadcastMessagesModel.ManageMessagesModel.LstQueries.Remove(queryValue);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //    }
        //}

        //public void CheckedByOutSideSoftware(object sender)
        //{
        //    try
        //    {
        //        if (BroadcastMessagesModel.IsCheckedOutSideSoftware)
        //        {
        //            if (!BroadcastMessagesModel.ManageMessagesModel.LstQueries.Any(x => x.Content.QueryValue == Application.Current.FindResource("LangKeyOutsideSoftware").ToString()))
        //            {
        //                BroadcastMessagesModel.ManageMessagesModel.LstQueries.Add(new QueryContent
        //                {
        //                    Content = new QueryInfo
        //                    {
        //                        QueryValue = Application.Current.FindResource("LangKeyOutsideSoftware")?.ToString()
        //                    }
        //                });

        //            }
        //        }
        //        else
        //        {
        //            var queryValue = BroadcastMessagesModel.ManageMessagesModel.LstQueries.FirstOrDefault(x => x.Content.QueryValue == Application.Current.FindResource("LangKeyOutsideSoftware")?.ToString());
        //            BroadcastMessagesModel.ManageMessagesModel.LstQueries.Remove(queryValue);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //    }
        //}

        //public void CheckedByCustomUserList(object sender)
        //{
        //    try
        //    {
        //        if (BroadcastMessagesModel.IsCheckedLangKeyCheckedLangKeyCustomUserListList)
        //        {
        //            if (!BroadcastMessagesModel.ManageMessagesModel.LstQueries.Any(x => x.Content.QueryValue == Application.Current.FindResource("LangKeyCustomUsersList").ToString()))
        //            {
        //                BroadcastMessagesModel.ManageMessagesModel.LstQueries.Add(new QueryContent
        //                {
        //                    Content = new QueryInfo
        //                    {
        //                        QueryValue = Application.Current.FindResource("LangKeyCustomUsersList")?.ToString()
        //                        //QueryType = FindResource("LangKeyGreetingOptions").ToString().Replace(" :", "")
        //                    }
        //                });

        //            }
        //        }
        //        else
        //        {
        //            var queryValue = BroadcastMessagesModel.ManageMessagesModel.LstQueries.FirstOrDefault(x => x.Content.QueryValue == Application.Current.FindResource("LangKeyCustomUsersList")?.ToString());
        //            BroadcastMessagesModel.ManageMessagesModel.LstQueries.Remove(queryValue);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //    }
        //} 

        #endregion
    }
}