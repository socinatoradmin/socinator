using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IQuestionsScraperViewModel
    {
    }

    public class QuestionsScraperViewModel : StartupBaseViewModel, IQuestionsScraperViewModel
    {
        public QuestionsScraperViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.QuestionsScraper});

            NextCommand = new DelegateCommand(NavigateNext);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyQuestionScrapePerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyQuestionScrapePerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyQuestionScrapePerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyQuestionScrapePerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxQuestionScrapePerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            ListQueryType.Clear();
        }
    }
}