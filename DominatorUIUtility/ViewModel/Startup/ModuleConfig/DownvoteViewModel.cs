using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IDownvoteViewModel
    {
    }

    public class DownvoteViewModel : StartupBaseViewModel, IDownvoteViewModel
    {
        public DownvoteViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.Downvote});
            NextCommand = new DelegateCommand(NavigateNext);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);

            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfDownvotePerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfDownvotePerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfDownvotePerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfDownvotePerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyNumberOfDownvotePerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }
    }
}