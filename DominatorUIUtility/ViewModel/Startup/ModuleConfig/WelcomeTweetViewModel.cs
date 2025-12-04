using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IWelcomeTweetViewModel
    {
    }

    public class WelcomeTweetViewModel : StartupBaseViewModel, IWelcomeTweetViewModel
    {
        public WelcomeTweetViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.WelcomeTweet});
            NextCommand = new DelegateCommand(ValidateAndNevigate);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            IsNonQuery = true;

            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfMessagesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfMessagesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfMessagesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfMessagesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxMessagePerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }

        public string WelcomeMessageText { get; set; }

        private void ValidateAndNevigate()
        {
            if (string.IsNullOrEmpty(WelcomeMessageText?.Trim()))
            {
                Dialog.ShowDialog("Error", "Please enter atleast one message");
                return;
            }

            NavigateNext();
        }
    }
}