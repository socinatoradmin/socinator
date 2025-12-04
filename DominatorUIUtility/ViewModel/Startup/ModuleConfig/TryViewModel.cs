using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface ITryViewModel
    {
    }

    public class TryViewModel : StartupBaseViewModel, ITryViewModel
    {
        public TryViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.Try});
            NextCommand = new DelegateCommand(NavigateNext);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);

            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfTriesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfTriesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfTriesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfTriesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxFollowsPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }
    }
}