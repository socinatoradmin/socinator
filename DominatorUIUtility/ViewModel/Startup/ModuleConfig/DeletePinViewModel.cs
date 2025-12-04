using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IDeletePinViewModel
    {
    }

    public class DeletePinViewModel : StartupBaseViewModel, IDeletePinViewModel
    {
        public DeletePinViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.DeletePin});
            NextCommand = new DelegateCommand(NavigateNext);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);

            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfDeletePinsPerJob".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfDeletePinsPerDay".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfDeletePinsPerHour".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfDeletePinsPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxDeletePinsPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }
    }
}