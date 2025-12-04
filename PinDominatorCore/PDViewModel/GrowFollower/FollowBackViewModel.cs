using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using PinDominatorCore.PDModel;

namespace PinDominatorCore.PDViewModel.GrowFollower
{
    public class FollowBackViewModel : BindableBase
    {
        private FollowBackModel _followBackModel = new FollowBackModel();

        public FollowBackViewModel()
        {
            FollowBackModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfFollowbacksPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfFollowbacksPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfFollowbacksPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfFollowbacksPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxFollowbackPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }

        public FollowBackModel Model => FollowBackModel;

        public FollowBackModel FollowBackModel
        {
            get => _followBackModel;
            set
            {
                if ((_followBackModel == null) & (_followBackModel == value))
                    return;
                SetProperty(ref _followBackModel, value);
            }
        }
    }
}