using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IDislikeViewModel
    {
    }

    public class DislikeViewModel : StartupBaseViewModel, IDislikeViewModel
    {
        public DislikeViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.Dislike});
            NextCommand = new DelegateCommand(NavigateNext);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);

            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyDislikesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyDislikesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyDislikesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyDislikesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaximumDislikesPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }
    }
}