using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IDeleteTweetViewModel
    {
    }

    public class DeleteTweetViewModel : StartupBaseViewModel, IDeleteTweetViewModel
    {
        public DeleteTweetViewModel(IRegionManager region) : base(region)
        {
            //ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.DeleteTweet });

            NextCommand = new DelegateCommand(NavigateNext);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfDeletesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfDeletesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfDeletesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfDeletesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxDeletePerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            ListQueryType.Clear();
        }
    }
}