using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.LDModel.Messenger;
using DominatorUIUtility.CustomControl;
using System.Collections.ObjectModel;
using System.Windows;

namespace LinkedDominatorCore.LDViewModel.Messenger
{
    public class SendMessageToNewConnectionViewModel : BindableBase
    {
        private SendMessageToNewConnectionModel
            _SendMessageToNewConnectionModel = new SendMessageToNewConnectionModel();

        public SendMessageToNewConnectionViewModel()
        {
            SendMessageToNewConnectionModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfMessagesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfMessagesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfMessagesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfMessagesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxConnectionsToMessagePerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            if (SendMessageToNewConnectionModel.ManageMessagesModel.LstQueries.All(x => x.Content.QueryValue != "All"))
                SendMessageToNewConnectionModel.ManageMessagesModel.LstQueries.Add(new QueryContent
                {
                    Content = new QueryInfo
                    {
                        QueryValue = "All",
                        QueryType = "All"
                    }
                });
            AddQueries();
            AddMessagesCommand = new BaseCommand<object>(sender => true, AddMessages);
        }

        public SendMessageToNewConnectionModel SendMessageToNewConnectionModel
        {
            get => _SendMessageToNewConnectionModel;
            set
            {
                if ((_SendMessageToNewConnectionModel == null) & (_SendMessageToNewConnectionModel == value))
                    return;
                SetProperty(ref _SendMessageToNewConnectionModel, value);
            }
        }

        public SendMessageToNewConnectionModel Model => SendMessageToNewConnectionModel;
        public ICommand AddMessagesCommand { get; set; }
        
        
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
                

                #endregion

                SendMessageToNewConnectionModel.LstDisplayManageMessagesModel.Add(MessageData.Messages);

                var lastQueries = MessageData.Messages.LstQueries;
                MessageData.Messages = new ManageMessagesModel();

                MessageData.Messages.LstQueries = lastQueries;
                MessageData.Messages.LstQueries.Select(query =>
                {
                    query.IsContentSelected = false;
                    return query;
                }).ToList();

                SendMessageToNewConnectionModel.ManageMessagesModel = MessageData.Messages;
                MessageData.ComboBoxQueries.ItemsSource = SendMessageToNewConnectionModel.ManageMessagesModel.LstQueries;

                AddQueries();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void AddQueries()
        {
            AddQuery("All");
        }
        public void AddQuery(string keyResource)
        {
            try
            {
                var queryValue = keyResource.Equals("All")
                    ? "All"
                    : Application.Current.FindResource(keyResource).ToString();
                if (SendMessageToNewConnectionModel.ManageMessagesModel.LstQueries.All(x =>
                    x.Content.QueryValue != queryValue))
                    SendMessageToNewConnectionModel.ManageMessagesModel.LstQueries.Add(new QueryContent
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
    }
}