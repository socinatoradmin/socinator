using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IChannelScraperViewModel
    {
    }

    public class ChannelScraperViewModel : StartupBaseViewModel, IChannelScraperViewModel
    {
        public ChannelScraperViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.ChannelScraper});
            NextCommand = new DelegateCommand(NavigateNext);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);

            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfChannelScrapePerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfChannelScrapePerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfChannelScrapePerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfChannelScrapePerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyNumberOfMaxChannelScrapePerday".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }
    }
}