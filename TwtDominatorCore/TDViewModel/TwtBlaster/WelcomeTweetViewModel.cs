using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using TwtDominatorCore.TDModels;

namespace TwtDominatorCore.TDViewModel.TwtBlaster
{
    public class WelcomeTweetViewModel : BindableBase
    {
        private WelcomeTweetModel _welcomeTweetModel = new WelcomeTweetModel();

        public WelcomeTweetViewModel()
        {
            WelcomeTweetModel.JobConfiguration = new JobConfiguration
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

        public WelcomeTweetModel WelcomeTweetModel
        {
            get => _welcomeTweetModel;
            set
            {
                if ((_welcomeTweetModel == null) & (_welcomeTweetModel == value))
                    return;
                SetProperty(ref _welcomeTweetModel, value);
            }
        }

        public WelcomeTweetModel Model => WelcomeTweetModel;
    }
}