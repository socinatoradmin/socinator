using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.TumblrQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;

namespace TumblrDominatorCore.ViewModels.Message
{
    public class BroadcastMessagesViewModel : BindableBase
    {
        private BroadcastMessagesModel _broadcastMessagesModel = new BroadcastMessagesModel();


        public BroadcastMessagesViewModel()
        {
            BroadcastMessagesModel.ListQueryType.Clear();

            Enum.GetValues(typeof(TumblrBroadcastMessageQuery)).Cast<TumblrBroadcastMessageQuery>().ToList().ForEach(
                query =>
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

            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
            AddMessagesCommand = new BaseCommand<object>(sender => true, AddMessages);
            DeleteMulipleCommand = new BaseCommand<object>(sender => true, DeleteMuliple);
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

                if (string.IsNullOrEmpty(moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue) &&
                    moduleSettingsUserControl._queryControl.QueryCollection.Count != 0)
                    foreach (var queryValue in moduleSettingsUserControl._queryControl.QueryCollection)
                    {
                        if (Model.ManageMessagesModel.LstQueries.Any(x =>
                            x.Content.QueryValue == queryValue &&
                            x.Content.QueryType == moduleSettingsUserControl._queryControl.CurrentQuery.QueryType))
                            continue;
                        {
                            var addNew = new QueryContent
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
                            Model.ManageMessagesModel.LstQueries.Add(addNew);
                            Model.LstDisplayManageMessageModel.ForEach(x =>
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
                            Model.LstDisplayManageMessageModel.ForEach(x =>
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
                moduleSettingsUserControl.AddQuery(typeof(TumblrBroadcastMessageQuery));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

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
                foreach (var message in Model.LstDisplayManageMessageModel.ToList())
                {
                    message.SelectedQuery.Remove(message.SelectedQuery.GetDeletingQuery(queryToDelete));

                    if (message.SelectedQuery.Count == 0)
                        Model.LstDisplayManageMessageModel.Remove(message);

                    message.LstQueries.Remove(message.LstQueries.GetDeletingQuery(queryToDelete));
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AddMessages(object sender)
        {
            var messageData = sender as MessagesControl;

            if (messageData == null) return;

            messageData.Messages.SelectedQuery =
                new ObservableCollection<QueryContent>(messageData.Messages.LstQueries.Where(x => x.IsContentSelected));
            messageData.Messages.SelectedQuery.Remove(
                messageData.Messages.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));
            if (messageData.Messages.SelectedQuery.Any() && !string.IsNullOrEmpty(messageData.Messages.MessagesText))
                Model.LstDisplayManageMessageModel.Add(messageData.Messages);
            messageData.Messages = new ManageMessagesModel
            {
                LstQueries = Model.ManageMessagesModel.LstQueries
            };

            messageData.Messages.LstQueries.Select(query =>
            {
                query.IsContentSelected = false;
                return query;
            }).ToList();

            Model.ManageMessagesModel = messageData.Messages;
            messageData.ComboBoxQueries.ItemsSource = Model.ManageMessagesModel.LstQueries;
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