using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.Views.AccountSetting.CustomControl;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IMessageToFanpagesViewModel
    {
    }

    public class MessageToFanpagesViewModel : StartupBaseViewModel, IMessageToFanpagesViewModel
    {
        public MessageToFanpagesViewModel(IRegionManager region) : base(region)
        {
            AddQueryMessageCommand = new DelegateCommand<object>(AddQueryMessage);
            DeleteQueryCommand = new DelegateCommand<object>(DeleteQuery);
            AddMessagesCommand = new DelegateCommand<object>(AddMessages);
            DeleteMulipleCommand = new DelegateCommand<object>(DeleteMuliple);

            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.MessageToFanpages});
            NextCommand = new DelegateCommand(MessageToFanPageValidation);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);

            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyMessagesToNumberOfProfilesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyMessagesToNumberOfProfilesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyMessagesToNumberOfProfilesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyMessageToNumberOfProfilesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMessagesToMaxProfilesPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }

        public ICommand DeleteQueryCommand { get; set; }
        public ICommand AddMessagesCommand { get; set; }
        public ICommand DeleteMulipleCommand { get; set; }
        public ICommand AddQueryMessageCommand { get; set; }

        public bool IsMessageAsPreview { get; set; }
        public bool IsSpintaxChecked { get; set; }
        public bool IsTagChecked { get; set; }

        public ObservableCollection<ManageMessagesModel> LstManageMessagesModel { get; set; } =
            new ObservableCollection<ManageMessagesModel>();

        public ManageMessagesModel ManageMessagesModel { get; set; } = new ManageMessagesModel();

        private void MessageToFanPageValidation()
        {
            if (LstManageMessagesModel.Count == 0)
            {
                Dialog.ShowDialog("Warning", "Please add atleast one message");
                return;
            }

            NavigateNext();
        }

        private void AddQueryMessage(object sender)
        {
            try
            {
                var activitySetting = sender as ActivitySettingWithoutButton;

                if (activitySetting == null ||
                    string.IsNullOrEmpty(activitySetting.QueryControl.CurrentQuery.QueryValue.Trim()) &&
                    !activitySetting.QueryControl.QueryCollection.Any())
                    return;

                var splittedQueries = activitySetting.QueryControl.CurrentQuery.QueryValue.Contains(",")
                    ? activitySetting.QueryControl.CurrentQuery.QueryValue.Split(',')
                        .Where(x => !string.IsNullOrEmpty(x.Trim())).ToList()
                    : new List<string> {activitySetting.QueryControl.CurrentQuery.QueryValue};

                if (string.IsNullOrEmpty(activitySetting.QueryControl.CurrentQuery.QueryValue) &&
                    activitySetting.QueryControl.QueryCollection.Count != 0)
                    foreach (var queryValue in activitySetting.QueryControl.QueryCollection)
                    {
                        if (ManageMessagesModel.LstQueries.Any(x =>
                            x.Content.QueryValue == queryValue &&
                            x.Content.QueryType == activitySetting.QueryControl.CurrentQuery.QueryType))
                            continue;
                        {
                            var addNew = new QueryContent
                            {
                                Content = new QueryInfo
                                {
                                    QueryValue = queryValue,
                                    QueryType = activitySetting.QueryControl.CurrentQuery.QueryType
                                }
                            };
                            ManageMessagesModel.LstQueries.Add(addNew);
                            LstManageMessagesModel.ForEach(x =>
                            {
                                if (!x.LstQueries.Any(y =>
                                    addNew.Content.QueryType == activitySetting.QueryControl.CurrentQuery.QueryType &&
                                    y.Content.QueryValue == addNew.Content.QueryValue))
                                    x.LstQueries.Add(addNew);
                            });
                        }
                    }
                else
                    foreach (var queryValue in splittedQueries)
                    {
                        if (ManageMessagesModel.LstQueries.Any(x =>
                            x.Content.QueryType == activitySetting.QueryControl.CurrentQuery.QueryType &&
                            x.Content.QueryValue == queryValue)) continue;
                        {
                            var addNew = new QueryContent
                            {
                                Content = new QueryInfo
                                {
                                    QueryValue = queryValue,
                                    QueryType = activitySetting.QueryControl.CurrentQuery.QueryType
                                }
                            };
                            ManageMessagesModel.LstQueries.Add(addNew);

                            LstManageMessagesModel.ForEach(x =>
                            {
                                if (!x.LstQueries.Any(y =>
                                    addNew.Content.QueryType == activitySetting.QueryControl.CurrentQuery.QueryType &&
                                    y.Content.QueryValue == addNew.Content.QueryValue))
                                    x.LstQueries.Add(addNew);
                            });
                        }
                    }

                AddQueryAll();

                AddQueryCommand.Execute(sender);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AddQueryAll()
        {
            if (ManageMessagesModel.LstQueries.Count > 1 &&
                !ManageMessagesModel.LstQueries.Any(x =>
                    x.Content.QueryValue == "All" && x.Content.QueryType == "All"))
                ManageMessagesModel.LstQueries.Insert(0, new QueryContent
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

                var queryToDelete = ManageMessagesModel.LstQueries.FirstOrDefault(x =>
                    currentQuery != null && x.Content.QueryValue == currentQuery.QueryValue &&
                    x.Content.QueryType == currentQuery.QueryType);


                if (SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                    SavedQueries.Remove(currentQuery);


                ManageMessagesModel.LstQueries.Remove(queryToDelete);
                foreach (var message in LstManageMessagesModel.ToList())
                {
                    var query = message.SelectedQuery.FirstOrDefault(x =>
                        queryToDelete != null && x.Content.Id == queryToDelete.Content.Id);
                    message.SelectedQuery.Remove(query);
                    if (message.SelectedQuery.Count == 0)
                        LstManageMessagesModel.Remove(message);
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

            messageData.Messages.SelectedQuery =
                new ObservableCollection<QueryContent>(messageData.Messages.LstQueries.Where(x => x.IsContentSelected));

            if (messageData.Messages.SelectedQuery.Count == 0)
            {
                Dialog.ShowDialog("Warning",
                    "Please select atleast one query!!");
                return;
            }

            if (string.IsNullOrEmpty(messageData.Messages.MessagesText))
            {
                Dialog.ShowDialog("Warning",
                    "Please enter message text!!");
                return;
            }


            messageData.Messages.SelectedQuery.Remove(
                messageData.Messages.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));

            LstManageMessagesModel.Add(messageData.Messages);

            messageData.Messages = new ManageMessagesModel
            {
                LstQueries = ManageMessagesModel.LstQueries
            };

            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            messageData.Messages.LstQueries.Select(query =>
            {
                query.IsContentSelected = false;
                return query;
            }).ToList();

            ManageMessagesModel = messageData.Messages;

            messageData.ComboBoxQueries.ItemsSource = ManageMessagesModel.LstQueries;
        }

        private void DeleteMuliple(object sender)
        {
            var selectedQuery = SavedQueries.Where(x => x.IsQuerySelected).ToList();
            try
            {
                foreach (var currentQuery in selectedQuery)
                    try
                    {
                        var queryToDelete = ManageMessagesModel.LstQueries.FirstOrDefault(x =>
                            x.Content.QueryValue == currentQuery.QueryValue
                            && x.Content.QueryType == currentQuery.QueryType);


                        if (SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                            SavedQueries.Remove(currentQuery);


                        ManageMessagesModel.LstQueries.Remove(queryToDelete);
                        foreach (var message in LstManageMessagesModel.ToList())
                        {
                            var query = message.SelectedQuery.FirstOrDefault(x =>
                                queryToDelete != null && x.Content.Id == queryToDelete.Content.Id);
                            message.SelectedQuery.Remove(query);
                            if (message.SelectedQuery.Count == 0)
                                LstManageMessagesModel.Remove(message);
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
    }
}