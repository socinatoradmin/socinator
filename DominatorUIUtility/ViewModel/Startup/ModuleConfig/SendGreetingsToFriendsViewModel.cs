using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface ISendGreetingsToFriendsViewModel
    {
        ManageMessagesModel ManageMessagesModel { get; set; }
        bool IsMessageAsPreview { get; set; }
        bool IsSpintaxChecked { get; set; }
        bool IsTagChecked { get; set; }
    }

    public class SendGreetingsToFriendsViewModel : StartupBaseViewModel, ISendGreetingsToFriendsViewModel
    {
        private RangeUtilities _daysBefore = new RangeUtilities(1, 2);

        private bool _isFilterByAge = true;


        private bool _isFilterByDays = true;

        private bool _isMessageAsPreview;

        private bool _isSendBirthDayGreetings = true;


        private bool _isSpintaxChecked;

        private bool _isTagChecked;

        public ObservableCollection<ManageMessagesModel> _lstManageMessagesModel =
            new ObservableCollection<ManageMessagesModel>();

        private readonly ManageMessagesModel _manageMessagesModel = new ManageMessagesModel();


        private RangeUtilities _userAge = new RangeUtilities(20, 60);

        public SendGreetingsToFriendsViewModel(IRegionManager region) : base(region)
        {
            IsNonQuery = true;
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.SendGreetingsToFriends});
            NextCommand = new DelegateCommand(SendGreetingsToFriendsValidate);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            AddMessagesCommand = new BaseCommand<object>(sender => true, AddMessages);

            ManageMessagesModel.LstQueries.Add(new QueryContent
                {Content = new QueryInfo {QueryType = "All", QueryValue = "All"}});
            ManageMessagesModel.LstQueries.Add(new QueryContent
            {
                Content = new QueryInfo
                {
                    QueryType = Application.Current.FindResource("LangKeyGreetingOptions")?.ToString(),
                    QueryValue = Application.Current.FindResource("LangKeyTodaysBirthdays")?.ToString()
                }
            });

            ManageMessagesModel.LstQueries.Add(new QueryContent
            {
                Content = new QueryInfo
                {
                    QueryType = Application.Current.FindResource("LangKeyGreetingOptions")?.ToString(),
                    QueryValue = Application.Current.FindResource("LangKeyUpcomingBirthdays")?.ToString()
                }
            });

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

        public ICommand AddMessagesCommand { get; set; }

        public ObservableCollection<ManageMessagesModel> LstManageMessagesModel
        {
            get => _lstManageMessagesModel;
            set
            {
                if ((_lstManageMessagesModel == null) & (_lstManageMessagesModel == value))
                    return;
            }
        }

        public bool IsSendBirthDayGreetings
        {
            get => _isSendBirthDayGreetings;

            set
            {
                if (value == _isSendBirthDayGreetings)
                    return;
                SetProperty(ref _isSendBirthDayGreetings, value);
            }
        }

        public RangeUtilities DaysBefore
        {
            get => _daysBefore;

            set
            {
                if (value == _daysBefore)
                    return;
                SetProperty(ref _daysBefore, value);
            }
        }

        public RangeUtilities UserAge
        {
            get => _userAge;

            set
            {
                if (value == _userAge)
                    return;
                SetProperty(ref _userAge, value);
            }
        }

        public bool IsFilterByAge
        {
            get => _isFilterByAge;

            set
            {
                if (value == _isFilterByAge)
                    return;
                SetProperty(ref _isFilterByAge, value);
            }
        }

        public bool IsFilterByDays
        {
            get => _isFilterByDays;

            set
            {
                if (value == _isFilterByDays)
                    return;
                SetProperty(ref _isFilterByDays, value);
            }
        }

        public ManageMessagesModel ManageMessagesModel
        {
            get => _manageMessagesModel;
            set
            {
                if ((_manageMessagesModel == null) & (_manageMessagesModel == value))
                    return;
            }
        }

        public bool IsMessageAsPreview
        {
            get => _isMessageAsPreview;
            set
            {
                if (value == _isMessageAsPreview)
                    return;
                SetProperty(ref _isMessageAsPreview, value);
            }
        }

        public bool IsSpintaxChecked
        {
            get => _isSpintaxChecked;
            set
            {
                if (value == _isSpintaxChecked)
                    return;
                SetProperty(ref _isSpintaxChecked, value);
            }
        }

        public bool IsTagChecked
        {
            get => _isTagChecked;
            set
            {
                if (value == _isTagChecked)
                    return;
                SetProperty(ref _isTagChecked, value);
            }
        }

        private void SendGreetingsToFriendsValidate()
        {
            if (LstManageMessagesModel.Count == 0)
            {
                Dialog.ShowDialog("Error", "Please add at least one message.");
                return;
            }

            if (IsFilterByDays && DaysBefore.StartValue == 0 && DaysBefore.EndValue == 0)
            {
                Dialog.ShowDialog("Error", "Please select valid days filter.");
                return;
            }

            NavigateNext();
        }

        private void AddMessages(object sender)
        {
            var messageData = sender as MessageMediaControl;
            if (messageData == null) return;
            messageData.Messages.SelectedQuery =
                new ObservableCollection<QueryContent>(messageData.Messages.LstQueries.Where(x => x.IsContentSelected));

            if (messageData.Messages.SelectedQuery.Count == 0)
            {
                Dialog.ShowDialog(Application.Current.MainWindow, "Warning",
                    "Please select atleast one query!!");
                return;
            }

            if (string.IsNullOrEmpty(messageData.Messages.MessagesText))
            {
                Dialog.ShowDialog(Application.Current.MainWindow, "Warning",
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
    }
}