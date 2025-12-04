using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IRemoveVoteCommentViewModel
    {
    }

    public class RemoveVoteCommentViewModel : StartupBaseViewModel, IRemoveVoteCommentViewModel
    {
        public RemoveVoteCommentViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.RemoveVoteComment});
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