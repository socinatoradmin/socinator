using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IAcceptConnectionRequestViewModel
    {
        bool IsChkAllInvitations { get; set; }
        bool IsChkIgnoreAllInvitations { get; set; }
    }

    public class AcceptConnectionRequestViewModel : StartupBaseViewModel, IAcceptConnectionRequestViewModel
    {
        private bool _isChkIgnoreAllInvitations;
        private bool _IsChkIgnoreAllInvitations;

        public AcceptConnectionRequestViewModel(IRegionManager region) : base(region)
        {
            IsNonQuery = true;
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.AcceptConnectionRequest});
            NextCommand = new DelegateCommand(NavigateNext);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);

            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName =
                    "LangKeyNumberOfConnectionRequestsToAcceptPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName =
                    "LangKeyNumberOfConnectionRequestsToAcceptPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName =
                    "LangKeyNumberOfConnectionRequestsToAcceptPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName =
                    "LangKeyNumberOfConnectionRequestsToAcceptPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxConnectionRequestsToAcceptPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }

        public bool IsChkAllInvitations
        {
            get => _isChkIgnoreAllInvitations;
            set => SetProperty(ref _isChkIgnoreAllInvitations, value);
        }

        public bool IsChkIgnoreAllInvitations
        {
            get => _IsChkIgnoreAllInvitations;
            set => SetProperty(ref _IsChkIgnoreAllInvitations, value);
        }
    }
}