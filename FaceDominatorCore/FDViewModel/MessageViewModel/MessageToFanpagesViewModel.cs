using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDModel.MessageModel;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace FaceDominatorCore.FDViewModel.MessageViewModel
{
    public class MessageToFanpagesViewModel : BindableBase
    {
        public MessageToFanpagesViewModel()
        {
            MessageToFanpagesModel.ListQueryType.Clear();

            Model.ManageMessagesModel.LstQueries.Add(new QueryContent { Content = new QueryInfo() { QueryType = "All", QueryValue = "All" } });

            Enum.GetValues(typeof(FanpageLikerQueryParameters)).Cast<FanpageLikerQueryParameters>().ToList().ForEach(query =>
            {
                MessageToFanpagesModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())?.ToString());
            });



            MessageToFanpagesModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current.FindResource("LangKeyMessagesToNumberOfProfilesPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current.FindResource("LangKeyMessagesToNumberOfProfilesPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current.FindResource("LangKeyMessagesToNumberOfProfilesPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current.FindResource("LangKeyMessageToNumberOfProfilesPerWeek")?.ToString(),
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyMessagesToMaxProfilesPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            AddQueryCommand = new BaseCommand<object>((sender) => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>((sender) => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>((sender) => true, DeleteQuery);
            AddMessagesCommand = new BaseCommand<object>((sender) => true, AddMessages);
            DeleteMulipleCommand = new BaseCommand<object>((sender) => true, DeleteMuliple);
            CheckUncheckCommand = new BaseCommand<object>((sender) => true, CheckUncheckOwnPage);
        }

        private void CheckUncheckOwnPage(object obj)
        {
            if (!MessageToFanpagesModel.IsActionasPageChecked)
            {
                MessageToFanpagesModel.ListOwnPageUrl = new List<string>();
                MessageToFanpagesModel.OwnPageUrl = string.Empty;
            }
        }

        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand AddMessagesCommand { get; set; }
        public ICommand DeleteMulipleCommand { get; set; }
        public ICommand CheckUncheckCommand { get; set; }

        #endregion

        #region Methods

        private void CustomFilter(object sender)
        {
            try
            {
                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<MessageToFanpagesViewModel, MessageToFanpagesModel>;
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
                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<MessageToFanpagesViewModel, MessageToFanpagesModel>;


                if (moduleSettingsUserControl != null)
                {
                    var listQuery = moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue.Split(',').Where(x => !string.IsNullOrEmpty(x.Trim())).Distinct();

                    listQuery.ForEach(z =>
                    {
                        if (!Model.ManageMessagesModel.LstQueries.Any(x =>
                            x.Content.QueryValue == z &&
                            x.Content.QueryType == moduleSettingsUserControl._queryControl.CurrentQuery.QueryType))
                        {

                            moduleSettingsUserControl?.SetQueryTypeEnumName(Enum.GetNames(typeof(FanpageLikerQueryParameters)), moduleSettingsUserControl._queryControl.CurrentQuery);
                            Model.ManageMessagesModel.LstQueries.Add(new QueryContent
                            {
                                Content = new QueryInfo
                                {
                                    QueryValue = z,
                                    QueryType = moduleSettingsUserControl._queryControl.CurrentQuery.QueryType,
                                    QueryTypeEnum = moduleSettingsUserControl._queryControl.CurrentQuery.QueryTypeEnum
                                }
                            });
                        }
                    });
                }

                if (moduleSettingsUserControl != null && (string.IsNullOrEmpty(moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue) &&
                                                          moduleSettingsUserControl._queryControl.QueryCollection.Count != 0))
                {
                    moduleSettingsUserControl._queryControl.QueryCollection.ForEach(query =>
                    {
                        var currentQuery = moduleSettingsUserControl._queryControl.CurrentQuery.Clone() as QueryInfo;

                        if (currentQuery == null) return;

                        currentQuery.QueryTypeDisplayName = currentQuery.QueryType;

                        moduleSettingsUserControl?.SetQueryTypeEnumName(Enum.GetNames(typeof(FanpageLikerQueryParameters)), currentQuery);

                        if (!Model.ManageMessagesModel.LstQueries.Any(x =>
                   x.Content.QueryValue == query &&
                   x.Content.QueryType == currentQuery.QueryTypeDisplayName))
                        {


                            Model.ManageMessagesModel.LstQueries.Add(new QueryContent
                            {
                                Content = new QueryInfo
                                {
                                    QueryValue = query,
                                    QueryType = currentQuery.QueryTypeDisplayName,
                                    QueryTypeEnum = currentQuery.QueryTypeEnum
                                }
                            });
                        }
                    });
                }

                moduleSettingsUserControl?.AddQuery(typeof(FanpageLikerQueryParameters), MessageToFanpagesModel.ListOwnPageUrl);
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
                        currentQuery != null && (x.Content.QueryValue == currentQuery.QueryValue
                                                 && x.Content.QueryType == currentQuery.QueryType));


                if (Model.SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                    Model.SavedQueries.Remove(currentQuery);


                Model.ManageMessagesModel.LstQueries.Remove(queryToDelete);
                foreach (var message in Model.LstDisplayManageMessageModel.ToList())
                {
                    var query = message.SelectedQuery.FirstOrDefault(x => queryToDelete != null && x.Content.Id == queryToDelete.Content.Id);
                    message.SelectedQuery.Remove(query);
                    if (message.SelectedQuery.Count == 0)
                        Model.LstDisplayManageMessageModel.Remove(message);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        private void AddMessages(object sender)
        {
            var messageData = sender as MessageMediaControl;

            if (messageData == null) return;

            messageData.Messages.SelectedQuery = new ObservableCollection<QueryContent>(messageData.Messages.LstQueries.Where(x => x.IsContentSelected));

            if (messageData.Messages.SelectedQuery.Count == 0)
            {
                Dialog.ShowDialog(Application.Current.MainWindow, "Warning",
                     "Please select atleast one query!!");
                return;
            }

            if (string.IsNullOrEmpty(messageData.Messages.MessagesText.Trim()))
            {
                Dialog.ShowDialog(Application.Current.MainWindow, "Warning",
                     "Please enter message text!!");
                return;
            }



            messageData.Messages.SelectedQuery.Remove(messageData.Messages.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));

            Model.LstDisplayManageMessageModel.Add(messageData.Messages);

            messageData.Messages = new ManageMessagesModel
            {
                LstQueries = Model.ManageMessagesModel.LstQueries
            };

            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            messageData.Messages.LstQueries.Select(query => { query.IsContentSelected = false; return query; }).ToList();

            Model.ManageMessagesModel = messageData.Messages;

            messageData.ComboBoxQueries.ItemsSource = Model.ManageMessagesModel.LstQueries;
        }

        private void DeleteMuliple(object sender)
        {
            var selectedQuery = Model.SavedQueries.Where(x => x.IsQuerySelected).ToList();
            try
            {
                foreach (var currentQuery in selectedQuery)
                {
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
                            var query = message.SelectedQuery.FirstOrDefault(x => queryToDelete != null && x.Content.Id == queryToDelete.Content.Id);
                            message.SelectedQuery.Remove(query);
                            if (message.SelectedQuery.Count == 0)
                                Model.LstDisplayManageMessageModel.Remove(message);
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        #endregion

        public MessageToFanpagesModel Model => MessageToFanpagesModel;

        private MessageToFanpagesModel _messageToFanpagesModel = new MessageToFanpagesModel();

        public MessageToFanpagesModel MessageToFanpagesModel
        {
            get
            {
                return _messageToFanpagesModel;
            }
            set
            {
                if (_messageToFanpagesModel == null & _messageToFanpagesModel == value)
                    return;
                SetProperty(ref _messageToFanpagesModel, value);
            }
        }

    }
}
