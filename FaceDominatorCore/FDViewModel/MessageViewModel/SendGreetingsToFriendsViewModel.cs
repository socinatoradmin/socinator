using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
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
    public class SendGreetingsToFriendsViewModel : BindableBase
    {

        public SendGreetingsToFriendsViewModel()
        {
            SendGreetingsToFriendsModel.ManageMessagesModel.LstQueries.Add(new QueryContent { Content = new QueryInfo() { QueryType = "All", QueryValue = "All" } });

            SendGreetingsToFriendsModel.GenderAndLocationFilter.IsFriendsDropdownVisible = true;

            SendGreetingsToFriendsModel.ManageMessagesModel.LstQueries.Add(new QueryContent
            {
                Content = new QueryInfo()
                {
                    QueryType = Application.Current.FindResource("LangKeyGreetingOptions")?.ToString(),
                    QueryValue = Application.Current.FindResource("LangKeyTodaysBirthdays")?.ToString(),
                    QueryTypeEnum = "GreetingOptions"
                }
            });

            SendGreetingsToFriendsModel.ManageMessagesModel.LstQueries.Add(new QueryContent
            {
                Content = new QueryInfo()
                {
                    QueryType = Application.Current.FindResource("LangKeyGreetingOptions")?.ToString(),
                    QueryValue = Application.Current.FindResource("LangKeyUpcomingBirthdays")?.ToString(),
                    QueryTypeEnum = "GreetingOptions"
                }
            });

            SendGreetingsToFriendsModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current.FindResource("LangKeyMessagesToNumberOfProfilesPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current.FindResource("LangKeyMessagesToNumberOfProfilesPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current.FindResource("LangKeyMessagesToNumberOfProfilesPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current.FindResource("LangKeyMessageToNumberOfProfilesPerWeek")?.ToString(),
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyMessagesToMaxProfilesPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };


            CustomFilterCommand = new BaseCommand<object>((sender) => true, CustomFilter);
            AddMessagesCommand = new BaseCommand<object>((sender) => true, AddMessages);
            SavePostDetailsCommand = new BaseCommand<object>((sender) => true, SavePostDetails);
        }

        private void SavePostDetails(object sender)
        {
            try
            {
                SendGreetingsToFriendsModel.ListPostDetails = Regex.Split(SendGreetingsToFriendsModel.PostDetailsText,
                        "\r\n").ToList();
                DominatorHouseCore.LogHelper.GlobusLogHelper.log.Info(DominatorHouseCore.Utility.Log.CustomMessage
                    , DominatorHouseCore.Enums.SocialNetworks.Facebook, "N/A", "N/A", "LangKeyDataSaved".FromResourceDictionary());
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #region Commands


        public ICommand CustomFilterCommand { get; set; }

        public ICommand AddMessagesCommand { get; set; }

        public ICommand SavePostDetailsCommand { get; set; }

        /*
                public ICommand CheckedChangedCommand { get; set; }
        */

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

        public SendGreetingsToFriendsModel Model => SendGreetingsToFriendsModel;

        private SendGreetingsToFriendsModel _sendGreetingsToFriendsModel = new SendGreetingsToFriendsModel();

        public SendGreetingsToFriendsModel SendGreetingsToFriendsModel
        {
            get { return _sendGreetingsToFriendsModel; }
            set
            {
                if ((_sendGreetingsToFriendsModel == null) & (_sendGreetingsToFriendsModel == value))
                    return;
                SetProperty(ref _sendGreetingsToFriendsModel, value);
            }
        }
    }
}
