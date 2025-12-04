using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDModel.MessageModel;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;


namespace FaceDominatorCore.FDViewModel.MessageViewModel
{
    public class AutoReplyMessageViewModel : BindableBase
    {
        public AutoReplyMessageViewModel()
        {
            Model.ManageMessagesModel.LstQueries.Add(new QueryContent { Content = new QueryInfo() { QueryType = "All", QueryValue = "All" } });

            Model.AutoReplyOptionModel = new AutoReplyOptionModel
            {
                BySoftwareDisplayName = "LangKeyReplyToNewPendingMessagesReplyToMessageRequests".FromResourceDictionary(),
                OutsideSoftwareDisplayName = "LangKeyReplyToConnectedPeopleReplyToThePeopleWhoAreConnectedInMessanger".FromResourceDictionary()
            };

            AutoReplyMessageModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current.FindResource("LangKeyMessagesToNumberOfProfilesPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current.FindResource("LangKeyMessagesToNumberOfProfilesPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current.FindResource("LangKeyMessagesToNumberOfProfilesPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current.FindResource("LangKeyMessageToNumberOfProfilesPerWeek")?.ToString(),
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyMessagesToMaxProfilesPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            SaveCommand = new BaseCommand<object>((sender) => true, SaveCommandExecute);
            CustomFilterCommand = new BaseCommand<object>((sender) => true, CustomFilter);
            AddMessagesCommand = new BaseCommand<object>((sender) => true, AddMessages);
            CheckedChangedCommand = new BaseCommand<object>((sender) => true, CheckedChangedExecute);
            CheckUncheckCommand = new BaseCommand<object>((sender) => true, CheckUncheckCommentFilters);
        }


        private void SaveCommandExecute(object sender)
        {
            AutoReplyMessageModel.LstMessage =
                Regex.Split(AutoReplyMessageModel.IncommingFilterText, "\r\n").Where(x => !string.IsNullOrWhiteSpace(x.Trim())).Select(y => y.Trim()).Distinct().ToList();
        }

        private void CheckUncheckCommentFilters(object obj)
        {
            if (!AutoReplyMessageModel.AutoReplyOptionModel.IsFilterByIncommingMessageText)
                AutoReplyMessageModel.IncommingFilterText = string.Empty;
        }

        //private AutoReplyOptionModel _AutoReplyOptionModel = new AutoReplyOptionModel();

        //public AutoReplyOptionModel AutoReplyOptionModel
        //{
        //    get
        //    {
        //        return _AutoReplyOptionModel;
        //    }
        //    set
        //    {
        //        if (_AutoReplyOptionModel == null & _AutoReplyOptionModel == value)
        //            return;
        //        SetProperty(ref _AutoReplyOptionModel, value);
        //    }
        //}


        private void CheckedChangedExecute(object obj)
        {
            if (Model.AutoReplyOptionModel.IsMessageRequestChecked && Model.ManageMessagesModel.LstQueries.All(x => x.Content.QueryType != "Reply to Message Requests"))
            {
                AddMessagesToModel("Reply to Message Requests", string.Empty);
            }
            else if (!Model.AutoReplyOptionModel.IsMessageRequestChecked)
            {
                RemoveMessagesToModel("Reply to Message Requests", string.Empty);
            }

            if (Model.AutoReplyOptionModel.IsFriendsMessageChecked && Model.ManageMessagesModel.LstQueries.All(x => x.Content.QueryType != "Reply to Connected Friends"))
            {
                AddMessagesToModel("Reply to Connected Friends", string.Empty);
            }
            else if (!Model.AutoReplyOptionModel.IsFriendsMessageChecked)
            {
                RemoveMessagesToModel("Reply to Connected Friends", string.Empty);
            }

            if (Model.AutoReplyOptionModel.IsReplyToPageMessagesChecked && Model.ManageMessagesModel.LstQueries.All(x => x.Content.QueryType != "Reply To Page Messages"))
            {
                AddMessagesToModel("Reply To Page Messages", string.Empty);
            }
            else if (!Model.AutoReplyOptionModel.IsReplyToPageMessagesChecked)
            {
                RemoveMessagesToModel("Reply To Page Messages", string.Empty);
            }

            if (!Model.AutoReplyOptionModel.IsReplyToPageMessagesChecked)
                Model.AutoReplyOptionModel.OwnPages = string.Empty;
        }

        public void AddMessagesToModel(string queryType, string queryValue)
        {
            Model.ManageMessagesModel.LstQueries.Add(new QueryContent { Content = new QueryInfo() { QueryType = queryType } });
        }

        public void RemoveMessagesToModel(string queryType, string queryValue)
        {
            var queryToDelete = Model.ManageMessagesModel.LstQueries.FirstOrDefault(x => x.Content.QueryType == queryType);
            Model.ManageMessagesModel.LstQueries.Remove(queryToDelete);
            Model.ManageMessagesModel.LstQueries.Remove(queryToDelete);
            foreach (var message in Model.LstDisplayManageMessageModel.ToList())
            {
                var query = message.SelectedQuery.FirstOrDefault(x => queryToDelete != null && x.Content.Id == queryToDelete.Content.Id);
                message.SelectedQuery.Remove(query);
                if (message.SelectedQuery.Count == 0)
                    Model.LstDisplayManageMessageModel.Remove(message);
            }
        }

        #region Commands


        public ICommand CustomFilterCommand { get; set; }

        public ICommand AddMessagesCommand { get; set; }

        public ICommand CheckedChangedCommand { get; set; }

        public ICommand CheckUncheckCommand { get; set; }

        public ICommand SaveCommand { get; set; }

        #endregion

        #region Methods

        private void CustomFilter(object sender)
        {
            try
            {
                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<AutoReplyMessageViewModel, AutoReplyMessageModel>;
                moduleSettingsUserControl?.CustomFilter();
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




        #endregion

        public AutoReplyMessageModel Model => AutoReplyMessageModel;

        private AutoReplyMessageModel _autoReplyMessageModel = new AutoReplyMessageModel();

        public AutoReplyMessageModel AutoReplyMessageModel
        {
            get { return _autoReplyMessageModel; }
            set
            {
                if ((_autoReplyMessageModel == null) & (_autoReplyMessageModel == value))
                    return;
                SetProperty(ref _autoReplyMessageModel, value);
            }
        }
    }
}
