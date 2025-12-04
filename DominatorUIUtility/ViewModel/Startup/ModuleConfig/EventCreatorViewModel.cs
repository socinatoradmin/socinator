using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Utility;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IEventCreatorViewModel
    {
    }

    public class EventCreatorViewModel : StartupBaseViewModel, IEventCreatorViewModel
    {
        private EventCreaterManagerModel _eventCreaterManagerModel
            = new EventCreaterManagerModel();

        private ObservableCollection<EventCreaterManagerModel> _lstManageEventModel =
            new ObservableCollection<EventCreaterManagerModel>();

        public EventCreatorViewModel(IRegionManager region) : base(region)
        {
            EventCreaterManagerModel.FbMultiMediaModel.IsMultiselect = false;
            EventCreaterManagerModel.IsPrivatePostingVisibile = true;

            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.EventCreator});
            IsNonQuery = true;
            NextCommand = new DelegateCommand(EventCreaterValidate);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);

            AddEventCommand = new DelegateCommand<object>(AddEventExecute);

            TypeSelectionChangedCommand = new DelegateCommand<object>(TypeSelectionChangedCommandExecute);

            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyCreateNumberOfEventsPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyCreateNumberOfEventsPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyCreateNumberOfEventsPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyCreateNumberOfMaximumEventsPerDay".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyCreateNumberOfEventsPerWeek".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            ListQueryType.Clear();
        }

        public ICommand AddEventCommand { get; set; }
        public ICommand TypeSelectionChangedCommand { get; set; }

        public EventCreaterManagerModel EventCreaterManagerModel
        {
            get => _eventCreaterManagerModel;
            set
            {
                if (_eventCreaterManagerModel == value)
                    return;
                SetProperty(ref _eventCreaterManagerModel, value);
            }
        }

        public ObservableCollection<EventCreaterManagerModel> LstManageEventModel
        {
            get => _lstManageEventModel;
            set
            {
                if ((_lstManageEventModel == null) & (_lstManageEventModel == value))
                    return;
                SetProperty(ref _lstManageEventModel, value);
            }
        }


        private void AddEventExecute(object sender)
        {
            if (string.IsNullOrEmpty(EventCreaterManagerModel.EventName?.Trim()))
            {
                Dialog.ShowDialog(Application.Current.MainWindow, "Warning",
                    "Please Give A Proper Event Name !!");
                return;
            }

            if (EventCreaterManagerModel.EventStartDate.AddDays(13) < EventCreaterManagerModel.EventEndDate)
            {
                Dialog.ShowDialog(Application.Current.MainWindow, "Warning",
                    "The Event Will Be Of Within 14 Days !!");
                return;
            }

            if (EventCreaterManagerModel.EventStartDate < DateTime.Now
                || EventCreaterManagerModel.EventEndDate < DateTime.Now)
            {
                Dialog.ShowDialog(Application.Current.MainWindow, "Warning",
                    "Please Give A Proper Event Start And End Date !!");
                return;
            }

            if (EventCreaterManagerModel.EventStartDate > EventCreaterManagerModel.EventEndDate)
            {
                Dialog.ShowDialog(Application.Current.MainWindow, "Warning",
                    "End Date Should not be less then Start Date!!");
                return;
            }

            EventCreaterManagerModel.MediaPath = EventCreaterManagerModel.FbMultiMediaModel.MediaPaths
                .FirstOrDefault().MediaPath;
            LstManageEventModel.Add(EventCreaterManagerModel);
            EventCreaterManagerModel = new EventCreaterManagerModel
            {
                FbMultiMediaModel = new FbMultiMediaModel
                {
                    IsMultiselect = false,
                    IsAddImageVisibile = true,
                    MediaPaths = new ObservableCollection<MultiMediaValueModel>()
                },
                EventType = "LangKeyCreatePrivateEvent".FromResourceDictionary()
            };
        }

        public void TypeSelectionChangedCommandExecute(object sender)
        {
            if (EventCreaterManagerModel.EventType == "LangKeyCreatePrivateEvent".FromResourceDictionary())
            {
                EventCreaterManagerModel.IsPublicPostingVisibile = false;
                EventCreaterManagerModel.IsPrivatePostingVisibile = true;
            }
            else
            {
                EventCreaterManagerModel.IsPrivatePostingVisibile = false;
                EventCreaterManagerModel.IsPublicPostingVisibile = true;
            }
        }

        private void EventCreaterValidate()
        {
            if (LstManageEventModel.Count <= 0)
            {
                Dialog.ShowDialog("Error", "Please Add Events To List.");
                return;
            }

            NavigateNext();
        }
    }
}