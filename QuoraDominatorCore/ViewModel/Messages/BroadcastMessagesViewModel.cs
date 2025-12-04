using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.QdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using QuoraDominatorCore.Models;

namespace QuoraDominatorCore.ViewModel.Messages
{
    public class BroadcastMessagesViewModel : BindableBase
    {
        private BroadcastMessagesModel _broadcastMessagesModel = new BroadcastMessagesModel();

        public BroadcastMessagesViewModel()
        {
            BroadcastMessagesModel.ListQueryType.Clear();

            Enum.GetValues(typeof(BroadcastMessageQuery)).Cast<BroadcastMessageQuery>().ToList().ForEach(query =>
            {
                BroadcastMessagesModel.ListQueryType.Add(Application.Current
                    .FindResource(query.GetDescriptionAttr())?.ToString());
            });
            BroadcastMessagesModel.ListQueryType.Remove("LangKeyCustomURLS".FromResourceDictionary());
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
            BroadcastMessagesModel.ManageMessagesModel.LstQueries.Add(new QueryContent
            {
                Content = new QueryInfo
                {
                    QueryType = "All",
                    QueryValue = "All"
                }
            });
            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
            AddMessagesCommand = new BaseCommand<object>(sender => true, AddMessages);
            DeleteMultipleCommand = new BaseCommand<object>(sender => true, DeleteMultiple);
        }

        public BroadcastMessagesModel BroadcastMessagesModel
        {
            get => _broadcastMessagesModel;
            set
            {
                if ((_broadcastMessagesModel == null) & (_broadcastMessagesModel == value))
                    return;
                SetProperty(ref _broadcastMessagesModel, value);
            }
        }

        public BroadcastMessagesModel Model => BroadcastMessagesModel;

        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand AddMessagesCommand { get; set; }
        public ICommand DeleteMultipleCommand { get; set; }

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
                if (!Model.ManageMessagesModel.LstQueries.Any(x =>
                    moduleSettingsUserControl != null &&
                    x.Content.QueryValue == moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue &&
                    x.Content.QueryType == moduleSettingsUserControl._queryControl.CurrentQuery.QueryType))
                {
                    if (moduleSettingsUserControl != null &&
                        moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue.Contains(","))
                    {
                        moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue.Split(',')
                            .Where(x => !string.IsNullOrEmpty(x.Trim())).Distinct().ForEach(query =>
                            {
                                var newquery = new QueryContent
                                {
                                    Content = new QueryInfo
                                    {
                                        QueryValue = query,
                                        QueryType = moduleSettingsUserControl._queryControl.CurrentQuery.QueryType
                                    }
                                };
                                Model.ManageMessagesModel.LstQueries.Add(newquery);
                                Model.LstDisplayManageMessageModel.ForEach(x =>
                                {
                                    if (!x.LstQueries.Any(y =>
                                        newquery.Content.QueryType ==
                                        moduleSettingsUserControl._queryControl.CurrentQuery.QueryType &&
                                        y.Content.QueryValue == newquery.Content.QueryValue))
                                        x.LstQueries.Add(newquery);
                                });
                            });
                    }
                    else if (moduleSettingsUserControl != null)
                    {
                        var newquery = new QueryContent
                        {
                            Content = new QueryInfo
                            {
                                QueryValue = moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue,
                                QueryType = moduleSettingsUserControl._queryControl.CurrentQuery.QueryType
                            }
                        };
                        Model.ManageMessagesModel.LstQueries.Add(newquery);
                        Model.LstDisplayManageMessageModel.ForEach(x =>
                        {
                            if (!x.LstQueries.Any(y =>
                                newquery.Content.QueryType ==
                                moduleSettingsUserControl._queryControl.CurrentQuery.QueryType &&
                                y.Content.QueryValue == newquery.Content.QueryValue))
                                x.LstQueries.Add(newquery);
                        });
                    }
                }

                if (Model.ManageMessagesModel.LstQueries[0].IsContentSelected)
                    Model.ManageMessagesModel.LstQueries.Select(x =>
                    {
                        x.IsContentSelected = true;
                        return x;
                    }).ToList();
                moduleSettingsUserControl?.AddQuery(typeof(BroadcastMessageQuery));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
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
                foreach (var message in Model.LstDisplayManageMessageModel.ToList())
                {
                    var queryDelete = message.SelectedQuery.FirstOrDefault(x =>
                        currentQuery != null && x.Content.QueryType == currentQuery.QueryType &&
                        x.Content.QueryValue == currentQuery.QueryValue);
                    message.SelectedQuery.Remove(queryDelete);

                    if (message.SelectedQuery.Count == 0)
                        Model.LstDisplayManageMessageModel.Remove(message);
                }

                if (!Model.ManageMessagesModel.LstQueries.Skip(1).Any())
                    Model.ManageMessagesModel.LstQueries[0].IsContentSelected = false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AddMessages(object sender)
        {
            try
            {
                var messageData = sender as MessagesControl;

                if (messageData == null) return;

                messageData.Messages.SelectedQuery =
                    new ObservableCollection<QueryContent>(
                        messageData.Messages.LstQueries.Where(x => x.IsContentSelected));

                if (messageData.Messages.SelectedQuery.Count == 0 ||
                    string.IsNullOrEmpty(messageData.Messages.MessagesText))
                {
                    Dialog.ShowDialog("Warning", "May be you didn't select any query or message is missing.");
                    return;
                }

                if (messageData.Messages.SelectedQuery.Count == 1 &&
                    messageData.Messages.SelectedQuery[0].Content.QueryType == "All")
                {
                    Dialog.ShowDialog("Warning", "May be you didn't select any query or message is missing.");
                    return;
                }

                messageData.Messages.SelectedQuery.Remove(
                    messageData.Messages.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));

                Model.LstDisplayManageMessageModel.Add(messageData.Messages);

                messageData.Messages = new ManageMessagesModel
                {
                    LstQueries = Model.ManageMessagesModel.LstQueries
                };
                messageData.Messages.LstQueries.Select(x =>
                {
                    x.IsContentSelected = false;
                    return x;
                }).ToList();

                Model.ManageMessagesModel = messageData.Messages;
                messageData.ComboBoxQueries.ItemsSource = Model.ManageMessagesModel.LstQueries;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DeleteMultiple(object sender)
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
                        foreach (var message in Model.LstDisplayManageMessageModel.ToList())
                        {
                            message.SelectedQuery.Remove(queryToDelete);
                            if (message.SelectedQuery.Count == 0)
                                Model.LstDisplayManageMessageModel.Remove(message);
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion
    }
}