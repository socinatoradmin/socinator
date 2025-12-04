using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface ISendGreetingsToConnectionsViewModel
    {
        bool IsCheckedBirthdayGreeting { get; set; }
        bool IsCheckedNewJobGreeting { get; set; }
        bool IsCheckedWorkAnniversaryGreeting { get; set; }
        bool IsChkSpintaxChecked { get; set; }
        bool IsChkTagChecked { get; set; }

        ICommand AddMessagesCommand { get; set; }
        ManageMessagesModel ManageMessagesModel { get; set; }
        ObservableCollection<ManageMessagesModel> LstDisplayManageMessagesModel { get; set; }
        ObservableCollection<ManageMessagesModel> LstManageMessagesModel { get; set; }
    }

    public class SendGreetingsToConnectionsViewModel : StartupBaseViewModel, ISendGreetingsToConnectionsViewModel
    {
        private bool _IsCheckedBirthdayGreeting;
        private bool _IsCheckedNewJobGreeting;
        private bool _IsCheckedWorkAnniversaryGreeting;
        private bool _IsChkSpintaxChecked;
        private bool _IsChkTagChecked;

        public SendGreetingsToConnectionsViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig
                {Model = this, ActivityType = ActivityType.SendGreetingsToConnections});
            NextCommand = new DelegateCommand(ValidateAndNevigate);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            IsNonQuery = true;

            AddMessagesCommand = new BaseCommand<object>(sender => true, AddMessages);
            AddQueries();
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfConnectionsToGreetPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfConnectionsToGreetPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfConnectionsToGreetPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfConnectionsToGreetPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyNumberOfConnectionsToGreetPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }


        public ICommand AddMessagesCommand { get; set; }

        public bool IsCheckedBirthdayGreeting
        {
            get => _IsCheckedBirthdayGreeting;
            set => SetProperty(ref _IsCheckedBirthdayGreeting, value);
        }

        public bool IsCheckedNewJobGreeting
        {
            get => _IsCheckedNewJobGreeting;
            set => SetProperty(ref _IsCheckedNewJobGreeting, value);
        }

        public bool IsCheckedWorkAnniversaryGreeting
        {
            get => _IsCheckedWorkAnniversaryGreeting;
            set => SetProperty(ref _IsCheckedWorkAnniversaryGreeting, value);
        }

        public bool IsChkSpintaxChecked
        {
            get => _IsChkSpintaxChecked;
            set => SetProperty(ref _IsChkSpintaxChecked, value);
        }


        public bool IsChkTagChecked
        {
            get => _IsChkTagChecked;
            set => SetProperty(ref _IsChkTagChecked, value);
        }

        public ObservableCollection<ManageMessagesModel> LstDisplayManageMessagesModel { get; set; } =
            new ObservableCollection<ManageMessagesModel>();

        public ManageMessagesModel ManageMessagesModel { get; set; } = new ManageMessagesModel();

        public ObservableCollection<ManageMessagesModel> LstManageMessagesModel { get; set; } =
            new ObservableCollection<ManageMessagesModel>();

        private void ValidateAndNevigate()
        {
            if (!IsCheckedBirthdayGreeting && !IsCheckedNewJobGreeting && !IsCheckedWorkAnniversaryGreeting)
            {
                Dialog.ShowDialog("Error", "select at least once of the greeting options");
                return;
            }

            if (ManageMessagesModel.LstQueries.Count > 0 && LstDisplayManageMessagesModel.Count == 0)
            {
                Dialog.ShowDialog("Error", "please add at least one message to the list of messages.");
                return;
            }

            NavigateNext();
        }

        public void AddMessages(object sender)
        {
            try
            {
                var messageData = sender as MessagesControl;

                if (messageData == null) return;


                #region Validations Before Adding Message to the list

                //MessageData.Messages.MessagesText
                if (string.IsNullOrEmpty(messageData.Messages.MessagesText?.Trim()) &&
                    string.IsNullOrEmpty(messageData.Messages.MediaPath))
                    return;

                messageData.Messages.SelectedQuery =
                    new ObservableCollection<QueryContent>(
                        messageData.Messages.LstQueries.Where(x => x.IsContentSelected));

                var lstQuery = messageData.Messages.SelectedQuery.Select(x => x.Content.QueryValue).ToList();

                if (lstQuery.Contains("All") &&
                    (!IsCheckedBirthdayGreeting ||
                     !IsCheckedNewJobGreeting ||
                     !IsCheckedWorkAnniversaryGreeting))
                {
                    Dialog.ShowDialog("Error",
                        "Please make sure you have selected all available connection source before adding to message list");
                    return;
                }

                if (!IsCheckedBirthdayGreeting &&
                    lstQuery.Contains(Application.Current.FindResource("LangKeyBirthdayGreeting")))
                {
                    Dialog.ShowDialog("Error",
                        "Please make sure you have selected " +
                        Application.Current.FindResource("LangKeyBirthdayGreeting") +
                        " as connection source before adding to message list");
                    return;
                }

                if (!IsCheckedNewJobGreeting &&
                    lstQuery.Contains(Application.Current.FindResource("LangKeyNewJobGreeting")))
                {
                    Dialog.ShowDialog("Error",
                        "Please make sure you have selected " +
                        Application.Current.FindResource("LangKeyNewJobGreeting") +
                        " as connection source before adding to message list");
                    return;
                }

                if (!IsCheckedWorkAnniversaryGreeting &&
                    lstQuery.Contains(Application.Current.FindResource("LangKeyWorkAnniversaryGreeting")))
                {
                    Dialog.ShowDialog("Error",
                        "Please make sure you have selected " +
                        Application.Current.FindResource("LangKeyWorkAnniversaryGreeting") +
                        " as connection source before adding to message list");
                    return;
                }

                #endregion

                messageData.Messages.SelectedQuery =
                    new ObservableCollection<QueryContent>(
                        messageData.Messages.LstQueries.Where(x => x.IsContentSelected));

                messageData.Messages.SelectedQuery.Remove(
                    messageData.Messages.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));

                LstDisplayManageMessagesModel.Add(messageData.Messages);

                messageData.Messages = new ManageMessagesModel
                {
                    LstQueries = ManageMessagesModel.LstQueries
                };
                AddQueries();
                messageData.Messages.LstQueries.Select(query =>
                {
                    query.IsContentSelected = false;
                    return query;
                }).ToList();

                ManageMessagesModel = messageData.Messages;
                messageData.ComboBoxQueries.ItemsSource = ManageMessagesModel.LstQueries;
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
                // string queryValue = Application.Current.FindResource(keyResource).ToString();
                if (ManageMessagesModel.LstQueries.All(x => x.Content.QueryValue != queryValue))
                    ManageMessagesModel.LstQueries.Add(new QueryContent
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
            AddQuery("LangKeyBirthdayGreeting");
            AddQuery("LangKeyNewJobGreeting");
            AddQuery("LangKeyWorkAnniversaryGreeting");
        }
    }
}