using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IProfileScraperViewModel
    {
    }

    public class ProfileScraperViewModel : StartupBaseViewModel, IProfileScraperViewModel
    {
        public ProfileScraperViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.ProfileScraper});

            NextCommand = new DelegateCommand(NavigateNext);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyScrapNumberOfProfilesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyScrapNumberOfProfilesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyScrapNumberOfProfilesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyScrapNumberOfProfilesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyScrapMaxProfilesPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            ListQueryType.Clear();
        }
    }
}