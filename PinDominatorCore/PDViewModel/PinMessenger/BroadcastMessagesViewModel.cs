using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.PdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;

namespace PinDominatorCore.PDViewModel.PinMessenger
{
    public class BroadcastMessagesViewModel : BindableBase
    {
        private BroadcastMessagesModel _broadcastMessegesModel = new BroadcastMessagesModel();

        public BroadcastMessagesViewModel()
        {
            BroadcastMessagesModel.ListQueryType.Clear();

            Enum.GetValues(typeof(PDUsersQueries)).Cast<PDUsersQueries>().ToList().ForEach(query =>
            {
                if(query!= PDUsersQueries.UsersWhoTriedPins)
                    BroadcastMessagesModel.ListQueryType.Add(Application.Current
                        .FindResource(query.GetDescriptionAttr())?.ToString());
            });
            BroadcastMessagesModel.ListQueryType.Remove(Application.Current
                .FindResource(PDUsersQueries.BoardsbyKeywords.GetDescriptionAttr())?.ToString());
            BroadcastMessagesModel.ListQueryType.Remove(Application.Current
                .FindResource(PDUsersQueries.CustomBoard.GetDescriptionAttr())?.ToString());

            // Load job configuration values
            BroadcastMessagesModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfBroadcastmessegesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfBroadcastmessegesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfBroadcastmessegesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfBroadcastmessegesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxBroadcastmessegesPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
            AddMessagesCommand = new BaseCommand<object>(sender => true, AddMessages);
            DeleteMulipleCommand = new BaseCommand<object>(sender => true, DeleteMuliple);
        }

        public BroadcastMessagesModel Model => BroadcastMessagesModel;

        public BroadcastMessagesModel BroadcastMessagesModel
        {
            get => _broadcastMessegesModel;
            set
            {
                if ((_broadcastMessegesModel == null) & (_broadcastMessegesModel == value))
                    return;
                SetProperty(ref _broadcastMessegesModel, value);
            }
        }

        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand AddMessagesCommand { get; set; }
        public ICommand DeleteMulipleCommand { get; set; }

        #endregion

        #region Methods

        private void CustomFilter(object sender)
        {
            try
            {
                var moduleSettingsUserControl =
                    sender as ModuleSettingsUserControl<BroadcastMessagesViewModel, BroadcastMessagesModel>;
                moduleSettingsUserControl?.CustomFilter();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AddQuery(object sender)
        {
            try
            {
                var moduleSettingsUserControl =
                    sender as ModuleSettingsUserControl<BroadcastMessagesViewModel, BroadcastMessagesModel>;

                if (moduleSettingsUserControl == null ||
                    string.IsNullOrEmpty(moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue.Trim()) &&
                    !moduleSettingsUserControl._queryControl.QueryCollection.Any())
                    return;

                var splittedQueries = moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue.Contains(",")
                    ? moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue.Split(',')
                        .Where(x => !string.IsNullOrEmpty(x.Trim())).ToList()
                    : new List<string> { moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue };
                SocialNetworks socialNetwork = DominatorHouseCore.Diagnostics.SocinatorInitialize.ActiveSocialNetwork;

                if (string.IsNullOrEmpty(moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue) &&
                    moduleSettingsUserControl._queryControl.QueryCollection.Count != 0)
                    foreach (var queryValue in moduleSettingsUserControl._queryControl.QueryCollection)
                    {
                        if (Model.ManageMessagesModel.LstQueries.Any(x =>
                            x.Content.QueryValue == queryValue &&
                            x.Content.QueryType == moduleSettingsUserControl._queryControl.CurrentQuery.QueryType))
                            continue;
                        {
                            var lstquery = queryValue.Split('\t').ToList();
                            var addNew = new QueryContent();
                            if (lstquery.Count > 1)
                                if (socialNetwork.ToString() == lstquery[0] && lstquery[1] == ActivityType.BroadcastMessages.ToString())
                                {
                                    addNew = new QueryContent
                                    {
                                        Content = new QueryInfo
                                        {
                                            AddedDateTime = moduleSettingsUserControl._queryControl.CurrentQuery.AddedDateTime,
                                            Id = moduleSettingsUserControl._queryControl.CurrentQuery.Id,
                                            QueryPriority = Model.SavedQueries.Count + 1,
                                            QueryType = lstquery[2],
                                            QueryTypeDisplayName = lstquery[2],
                                            QueryValue = lstquery[3]
                                        }
                                    };
                                }
                                else
                                    return;
                            else
                            {
                                addNew = new QueryContent
                                {
                                    Content = new QueryInfo
                                    {
                                        AddedDateTime = moduleSettingsUserControl._queryControl.CurrentQuery.AddedDateTime,
                                        Id = moduleSettingsUserControl._queryControl.CurrentQuery.Id,
                                        QueryPriority = moduleSettingsUserControl._queryControl.CurrentQuery.QueryPriority,
                                        QueryType = moduleSettingsUserControl._queryControl.CurrentQuery.QueryType,
                                        QueryTypeDisplayName = moduleSettingsUserControl._queryControl.CurrentQuery
                                       .QueryTypeDisplayName,
                                        QueryValue = queryValue
                                    }
                                };
                            }

                            if (Model.ManageMessagesModel.LstQueries.All(x => !(x.Content.QueryType.Equals(addNew.Content.QueryType) &&
                                                                             x.Content.QueryValue.Equals(addNew.Content.QueryValue))))
                                Model.ManageMessagesModel.LstQueries.Add(addNew);
                            Model.LstDisplayManageMessagesModel.ForEach(x =>
                            {
                                if (!x.LstQueries.Any(y =>
                                    addNew.Content.QueryType ==
                                    moduleSettingsUserControl._queryControl.CurrentQuery.QueryType &&
                                    y.Content.QueryValue == addNew.Content.QueryValue))
                                    x.LstQueries.Add(addNew);
                            });
                        }
                    }
                else
                    foreach (var queryValue in splittedQueries)
                    {
                        if (Model.ManageMessagesModel.LstQueries.Any(x =>
                            x.Content.QueryType == moduleSettingsUserControl._queryControl.CurrentQuery.QueryType &&
                            x.Content.QueryValue == queryValue.Trim())) continue;
                        {
                            var addNew = new QueryContent
                            {
                                Content = new QueryInfo
                                {
                                    QueryValue = queryValue.Trim(),
                                    QueryType = moduleSettingsUserControl._queryControl.CurrentQuery.QueryType
                                }
                            };
                            Model.ManageMessagesModel.LstQueries.Add(addNew);

                            Model.LstDisplayManageMessagesModel.ForEach(x =>
                            {
                                if (!x.LstQueries.Any(y =>
                                    addNew.Content.QueryType ==
                                    moduleSettingsUserControl._queryControl.CurrentQuery.QueryType &&
                                    y.Content.QueryValue == addNew.Content.QueryValue))
                                    x.LstQueries.Add(addNew);
                            });
                        }
                    }

                AddQueryAll();
                moduleSettingsUserControl.AddQuery(typeof(PDPinQueries));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        /// <summary>
        ///     Add query "All" if ListOfQueries has more than 1 query (add if query "All" already doesn't exist)
        /// </summary>
        private void AddQueryAll()
        {
            if (Model.ManageMessagesModel.LstQueries.Count > 1 &&
                !Model.ManageMessagesModel.LstQueries.Any(x =>
                    x.Content.QueryValue == "All" && x.Content.QueryType == "All"))
                Model.ManageMessagesModel.LstQueries.Insert(0, new QueryContent
                {
                    Content = new QueryInfo
                    {
                        QueryType = "All",
                        QueryValue = "All"
                    }
                });
        }

        private void DeleteQuery(object sender)
        {
            try
            {
                var currentQuery = sender as QueryInfo;

                var queryToDelete = Model.ManageMessagesModel.LstQueries.FirstOrDefault(x =>
                    currentQuery != null && x.Content.QueryValue == currentQuery.QueryValue &&
                    x.Content.QueryType == currentQuery.QueryType);


                if (Model.SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                    Model.SavedQueries.Remove(currentQuery);


                Model.ManageMessagesModel.LstQueries.Remove(queryToDelete);
                foreach (var message in Model.LstDisplayManageMessagesModel.ToList())
                {
                    message.SelectedQuery.Remove(message.SelectedQuery.GetDeletingQuery(queryToDelete));

                    if (message.SelectedQuery.Count == 0)
                        Model.LstDisplayManageMessagesModel.Remove(message);

                    message.LstQueries.Remove(message.LstQueries.GetDeletingQuery(queryToDelete));
                }

                RemoveQueryAll();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        /// <summary>
        ///     Remove query "All" if only 1 or 0 query left in ListOfQueries
        /// </summary>
        private void RemoveQueryAll()
        {
            if (Model.ManageMessagesModel.LstQueries.Count < 3 &&
                Model.ManageMessagesModel.LstQueries.Any(x =>
                    x.Content.QueryValue == "All" && x.Content.QueryType == "All"))
                Model.ManageMessagesModel.LstQueries.Remove(Model.ManageMessagesModel.LstQueries.First(y =>
                    y.Content.QueryValue == "All" && y.Content.QueryType == "All"));
        }

        private void AddMessages(object sender)
        {
            var messageData = sender as MessagesControl;

            if (messageData == null) return;

            messageData.Messages.MessagesText = messageData.Messages.MessagesText.Trim();
            messageData.Messages.SelectedQuery =
                new ObservableCollection<QueryContent>(messageData.Messages.LstQueries.Where(x => x.IsContentSelected));

            if (messageData.Messages.SelectedQuery.Count == 0 ||
                string.IsNullOrEmpty(messageData.Messages.MessagesText))
            {
                Dialog.ShowDialog(Application.Current.MainWindow, "LangKeyWarning".FromResourceDictionary(),
                    "LangKeyPleaseTypeSomeMessage".FromResourceDictionary());
                return;
            }

            messageData.Messages.SelectedQuery.Remove(
                messageData.Messages.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));

            var messageList = messageData.Messages.MessagesText.Split('\n').Where(x => !string.IsNullOrEmpty(x.Trim()))
                .Select(y => y.Trim()).Distinct().ToList();

            if (BroadcastMessagesModel.IsChkAddMultipleMessages)
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

            messageData.Messages.LstQueries.ForEach(query => { query.IsContentSelected = false; });

            Model.ManageMessagesModel = messageData.Messages;
            messageData.ComboBoxQueries.ItemsSource = Model.ManageMessagesModel.LstQueries;
        }

        private void AddToMessageList(ManageMessagesModel messageModel, string messageText)
        {
            try
            {
                var isContain = false;
                BroadcastMessagesModel.LstDisplayManageMessagesModel.ForEach(lstMessage =>
                {
                    if (lstMessage.MessagesText.ToLower().Equals(messageText.ToLower()))
                        isContain = lstMessage.SelectedQuery.Any(x => messageModel.SelectedQuery.Contains(x));
                });

                if (!isContain)
                    BroadcastMessagesModel.LstDisplayManageMessagesModel.Add(new ManageMessagesModel
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

        private void DeleteMuliple(object sender)
        {
            var selectedQuery = Model.SavedQueries.Where(x => x.IsQuerySelected).ToList();
            try
            {
                foreach (var currentQuery in selectedQuery)
                    try
                    {
                        var queryToDelete = Model.ManageMessagesModel.LstQueries.FirstOrDefault(x =>
                            x.Content.QueryValue == currentQuery.QueryValue
                            && x.Content.QueryType == currentQuery.QueryType);


                        if (Model.SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                            Model.SavedQueries.Remove(currentQuery);


                        Model.ManageMessagesModel.LstQueries.Remove(queryToDelete);
                        foreach (var message in Model.LstDisplayManageMessagesModel.ToList())
                        {
                            message.SelectedQuery.Remove(message.SelectedQuery.GetDeletingQuery(queryToDelete));

                            if (message.SelectedQuery.Count == 0)
                                Model.LstDisplayManageMessagesModel.Remove(message);

                            message.LstQueries.Remove(message.LstQueries.GetDeletingQuery(queryToDelete));
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                RemoveQueryAll();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion
    }
}