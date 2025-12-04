using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IWatchPartyInviterViewModel
    {
    }

    public class WatchPartyInviterViewModel : StartupBaseViewModel, IWatchPartyInviterViewModel
    {
        private InviterDetails _inviterDetails = new InviterDetails();

        private InviterOptions _inviterOptions = new InviterOptions();

        public WatchPartyInviterViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.WatchPartyInviter});
            NextCommand = new DelegateCommand(NavigateNext);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            IsNonQuery = true;

            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyInviteNumberOfProfilesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyInviteNumberOfProfilesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyInviteNumberOfProfilesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyInviteToNumberOfProfilesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyInviteMaxProfilesPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }

        public InviterDetails InviterDetailsModel
        {
            get => _inviterDetails;
            set
            {
                if ((_inviterDetails == null) & (_inviterDetails == value))
                    return;
                SetProperty(ref _inviterDetails, value);
            }
        }

        public InviterOptions InviterOptionsModel
        {
            get => _inviterOptions;
            set
            {
                if ((_inviterOptions == value) & (_inviterOptions == null))
                    return;
                SetProperty(ref _inviterOptions, value);
            }
        }
    }
}