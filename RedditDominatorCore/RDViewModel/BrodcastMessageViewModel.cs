using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.RdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using RedditDominatorCore.RDModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace RedditDominatorCore.RDViewModel
{
    public class BrodcastMessageViewModel : BindableBase
    {
        private BrodcastMessageModel _brodcastMessageModel = new BrodcastMessageModel();

        public BrodcastMessageViewModel()
        {
            BrodcastMessageModel.ListQueryType.Clear();

            if (BrodcastMessageModel.ListQueryType.Count == 0)
            {
                BrodcastMessageModel.ListQueryType.Add(Application.Current
                    .FindResource(UserQuery.Keywords.GetDescriptionAttr())?.ToString());
                BrodcastMessageModel.ListQueryType.Add(Application.Current
                    .FindResource(UserQuery.CustomUsers.GetDescriptionAttr())?.ToString());
            }

            BrodcastMessageModel.ListQueryType.Remove("UsersWhoCommentedOnPost");
            BrodcastMessageModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current
                    .FindResource("LangKeyMessagesToNumberOfProfilesPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current
                    .FindResource("LangKeyMessagesToNumberOfProfilesPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current
                    .FindResource("LangKeyMessagesToNumberOfProfilesPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current
                    .FindResource("LangKeyMessageToNumberOfProfilesPerWeek")?.ToString(),
                IncreaseActivityDisplayName =
                    Application.Current.FindResource("LangKeyMessagesToMaxProfilesPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            BrodcastMessageModel.ManageMessagesModel.LstQueries.Add(new QueryContent
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
            DeleteMulipleCommand = new BaseCommand<object>(sender => true, DeleteMuliple);
        }

        public BrodcastMessageModel Model => BrodcastMessageModel;

        public Visibility CancelEditVisibility { get; set; }

        public BrodcastMessageModel BrodcastMessageModel
        {
            get => _brodcastMessageModel;
            set
            {
                if ((_brodcastMessageModel == null) & (_brodcastMessageModel == value))
                    return;
                SetProperty(ref _brodcastMessageModel, value);
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
                    sender as ModuleSettingsUserControl<BrodcastMessageViewModel, BrodcastMessageModel>;
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
                    sender as ModuleSettingsUserControl<BrodcastMessageViewModel, BrodcastMessageModel>;
                if (!Model.ManageMessagesModel.LstQueries.Any(x =>
                    moduleSettingsUserControl != null &&
                    x.Content.QueryValue == moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue &&
                    x.Content.QueryType == moduleSettingsUserControl._queryControl.CurrentQuery.QueryType))
                    if (moduleSettingsUserControl != null)
                        Model.ManageMessagesModel.LstQueries.Add(new QueryContent
                        {
                            Content = new QueryInfo
                            {
                                QueryValue = moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue,
                                QueryType = moduleSettingsUserControl._queryControl.CurrentQuery.QueryType
                            }
                        });

                moduleSettingsUserControl?.AddQuery(typeof(UserQuery));
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
                foreach (var message in Model.LstManageMessagesModel.ToList())
                {
                    message.SelectedQuery.Remove(queryToDelete);
                    if (message.SelectedQuery.Count == 0)
                        Model.LstManageMessagesModel.Remove(message);
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

            if (messageData != null && messageData.Messages.MessagesText == null) return;

            if (messageData == null) return;
            messageData.Messages.SelectedQuery =
                new ObservableCollection<QueryContent>(
                    messageData.Messages.LstQueries.Where(x => x.IsContentSelected));
            messageData.Messages.SelectedQuery.Remove(
                messageData.Messages.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));

            if (messageData.Messages.SelectedQuery.Count == 0) return;
            Model.LstManageMessagesModel.Add(messageData.Messages);

            messageData.Messages = new ManageMessagesModel
            {
                LstQueries = Model.ManageMessagesModel.LstQueries
            };

            messageData.Messages.LstQueries.Select(query =>
            {
                query.IsContentSelected = false;
                return query;
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
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
                        foreach (var message in Model.LstManageMessagesModel.ToList())
                        {
                            message.SelectedQuery.Remove(queryToDelete);
                            if (message.SelectedQuery.Count == 0)
                                Model.LstManageMessagesModel.Remove(message);
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