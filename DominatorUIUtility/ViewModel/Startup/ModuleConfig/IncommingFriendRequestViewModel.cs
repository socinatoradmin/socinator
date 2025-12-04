using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IIncommingFriendRequestViewModel
    {
        int Count { get; set; }
        bool IsCancelReceivedRequest { get; set; }
        bool IsAcceptRequest { get; set; }
    }

    public class IncommingFriendRequestViewModel : StartupBaseViewModel, IIncommingFriendRequestViewModel
    {
        private int _count;
        private bool _isAcceptRequest;

        private bool _isCancelReceivedRequest;

        public IncommingFriendRequestViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.IncommingFriendRequest});
            NextCommand = new DelegateCommand(NavigateNext);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);

            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfRequestPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfRequestPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfRequestPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfRequestPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxRequestPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }

        public int Count
        {
            get => _count;
            set
            {
                if (value == _count)
                    return;
                SetProperty(ref _count, value);
            }
        }

        public bool IsAcceptRequest
        {
            get => _isAcceptRequest;
            set
            {
                if (value == _isAcceptRequest)
                    return;
                SetProperty(ref _isAcceptRequest, value);
            }
        }

        public bool IsCancelReceivedRequest
        {
            get => _isCancelReceivedRequest;
            set
            {
                if (value == _isCancelReceivedRequest)
                    return;
                SetProperty(ref _isCancelReceivedRequest, value);
            }
        }
    }
}