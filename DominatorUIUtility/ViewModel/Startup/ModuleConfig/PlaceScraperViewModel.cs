using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IPlaceScraperViewModel
    {
    }

    public class PlaceScraperViewModel : StartupBaseViewModel, IPlaceScraperViewModel
    {
        public PlaceScraperViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.PlaceScraper});
            NextCommand = new DelegateCommand(NavigateNext);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);

            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfLikesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfLikesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfLikesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfLikesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxLikesPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }
    }
}