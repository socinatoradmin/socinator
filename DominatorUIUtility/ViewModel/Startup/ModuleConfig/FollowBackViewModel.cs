using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IFollowBackViewModel
    {
    }

    public class FollowBackViewModel : StartupBaseViewModel, IFollowBackViewModel
    {
        public FollowBackViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.FollowBack});
            IsNonQuery = true;
            NextCommand = new DelegateCommand(ValidateAndNavigate);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfFollowbacksPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfFollowbacksPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfFollowbacksPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfFollowbacksPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxFollowbackPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            ListQueryType.Clear();
        }

        public bool IsFollowBack { get; set; }

        public bool IsAcceptFollowRequest { get; set; }

        private void ValidateAndNavigate()
        {
            if (!IsFollowBack && !IsAcceptFollowRequest)
            {
                Dialog.ShowDialog("Input Error",
                    "Please select atleast one checkbox option either follow back or Accept follow request");
                return;
            }

            NavigateNext();
        }
    }
}