using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IUpvoteAnswersViewModel
    {
    }

    public class UpvoteAnswersViewModel : StartupBaseViewModel, IUpvoteAnswersViewModel
    {
        public UpvoteAnswersViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.UpvoteAnswers});

            NextCommand = new DelegateCommand(NavigateNext);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfAnswersToUpvotePerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfAnswersToUpvotePerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfAnswersToUpvotePerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfAnswersToUpvotePerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxAnswersToUpvotePerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            ListQueryType.Clear();
        }
    }
}