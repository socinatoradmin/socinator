using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IFanpageScraperViewModel
    {
    }

    public class FanpageScraperViewModel : StartupBaseViewModel, IFanpageScraperViewModel
    {
        public FanpageScraperViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.FanpageScraper});

            NextCommand = new DelegateCommand(NavigateNext);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyScrapNumberOfFanpagesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyScrapNumberOfFanpagesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyScrapNumberOfFanpagesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyScrapNumberOfFanpagesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyScrapMaxFanpagesPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            ListQueryType.Clear();
        }
    }
}