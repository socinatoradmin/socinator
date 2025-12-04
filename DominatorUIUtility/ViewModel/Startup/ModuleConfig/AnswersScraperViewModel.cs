using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IAnswersScraperViewModel
    {
    }

    public class AnswersScraperViewModel : StartupBaseViewModel, IAnswersScraperViewModel
    {
        public AnswersScraperViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.AnswersScraper});

            NextCommand = new DelegateCommand(NavigateNext);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyAnswerScrapePerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyAnswerScrapePerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyAnswerScrapePerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyAnswerScrapePerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxAnswerScrapePerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            ListQueryType.Clear();
        }
    }
}