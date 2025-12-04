using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IDownvoteAnswersViewModel
    {
    }

    public class DownvoteAnswersViewModel : StartupBaseViewModel, IDownvoteAnswersViewModel
    {
        public DownvoteAnswersViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.DownvoteAnswers});

            NextCommand = new DelegateCommand(NavigateNext);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfAnswersToDownvotePerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfAnswersToDownvotePerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfAnswersToDownvotePerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfAnswersToDownvotePerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxAnswersToDownvotePerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            ListQueryType.Clear();
        }
    }
}