using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface ISendMessageToNewFriendsViewModel
    {
    }

    public class SendMessageToNewFriendsViewModel : StartupBaseViewModel, ISendMessageToNewFriendsViewModel
    {
        private RangeUtilities _daysBefore = new RangeUtilities(1, 2);

        private bool _isMessageAsPreview;


        private bool _isSpintaxChecked;

        private bool _isTagChecked;

        private ObservableCollection<ManageMessagesModel> _lstManageMessagesModel =
            new ObservableCollection<ManageMessagesModel>();

        private ManageMessagesModel _manageMessagesModel = new ManageMessagesModel();

        public SendMessageToNewFriendsViewModel(IRegionManager region) : base(region)
        {
            ManageMessagesModel.LstQueries.Add(new QueryContent
                {Content = new QueryInfo {QueryType = "All", QueryValue = "All"}});
            ManageMessagesModel.LstQueries.Add(new QueryContent
            {
                Content = new QueryInfo {QueryType = "Message To New Friends", QueryValue = "Message To New Friends"}
            });

            IsNonQuery = true;
            AddMessagesCommand = new DelegateCommand<object>(AddMessages);
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.SendMessageToNewFriends});
            NextCommand = new DelegateCommand(SendMessageToNewFriendsValidate);
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

        public ICommand AddMessagesCommand { get; set; }

        public ObservableCollection<ManageMessagesModel> LstManageMessagesModel
        {
            get => _lstManageMessagesModel;
            set
            {
                if ((_lstManageMessagesModel == value) & (_lstManageMessagesModel == null))
                    return;
                SetProperty(ref _lstManageMessagesModel, value);
            }
        }

        public ManageMessagesModel ManageMessagesModel
        {
            get => _manageMessagesModel;
            set
            {
                if ((_manageMessagesModel == value) & (_manageMessagesModel == null))
                    return;
                SetProperty(ref _manageMessagesModel, value);
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

        private void SendMessageToNewFriendsValidate()
        {
            if (LstManageMessagesModel.Count == 0)
            {
                Dialog.ShowDialog("Error", "Please add at least one message.");
                return;
            }

            if (DaysBefore.StartValue == 0 && DaysBefore.EndValue == 0)
            {
                Dialog.ShowDialog("Error", "Please select valid source filter.");
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