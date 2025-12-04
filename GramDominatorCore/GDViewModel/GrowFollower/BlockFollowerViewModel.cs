using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;

namespace GramDominatorCore.GDViewModel.GrowFollower
{
    public class BlockFollowerViewModel : BindableBase
    {
        public BlockFollowerViewModel()
        {
            BlockFollowerModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfBlockFollowersPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfBlockFollowersPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfBlockFollowersPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfBlockFollowersPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxBlockFollowersPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

        }

        private BlockFollowerModel _blockFollowerModel = new BlockFollowerModel();

        public BlockFollowerModel BlockFollowerModel
        {
            get
            {
                return _blockFollowerModel;
            }
            set
            {
                if (_blockFollowerModel == null & _blockFollowerModel == value)
                    return;
                SetProperty(ref _blockFollowerModel, value);
            }
        }

        // NOTE: Required alias property to make it work at runtime
        public BlockFollowerModel Model => BlockFollowerModel;

     
    }
}
