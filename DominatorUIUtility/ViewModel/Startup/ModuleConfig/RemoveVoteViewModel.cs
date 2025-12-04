using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IRemoveVoteViewModel
    {
    }

    public class RemoveVoteViewModel : StartupBaseViewModel, IRemoveVoteViewModel
    {
        public RemoveVoteViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.RemoveVote});
            NextCommand = new DelegateCommand(NavigateNext);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);

            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyRemoveVotePerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyRemoveVotePerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyRemoveVotePerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyRemoveVotePerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxRemoveVoteDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }
    }
}