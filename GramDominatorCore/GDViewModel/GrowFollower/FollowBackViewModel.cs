using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;

namespace GramDominatorCore.GDViewModel.GrowFollower
{
    public class FollowBackViewModel : BindableBase
    {
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

        private FollowBackModel _followBackModel = new FollowBackModel();

        public FollowBackModel FollowBackModel
        {
            get
            {
                return _followBackModel;
            }
            set
            {
                if (_followBackModel == null & _followBackModel == value)
                    return;
                SetProperty(ref _followBackModel, value);
            }
        }

        // NOTE: Required alias property to make it work at runtime
        public FollowBackModel Model => FollowBackModel;


    }
}
