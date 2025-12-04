using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;

namespace GramDominatorCore.GDViewModel.InstaLikerCommenter
{
    public class MediaUnlikerViewModel : BindableBase
    {
        public MediaUnlikerViewModel()
        {
            InitializeJobSetting();
        }

        private void InitializeJobSetting()
        {
            MediaUnlikerModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfMediaUnlikePerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfMediaUnlikePerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfMediaUnlikePerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfMediaUnlikePerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxMediaUnlikePerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }


        private MediaUnlikerModel _mediaUnlikerModel = new MediaUnlikerModel();
       // private MediaUnlikerModel _mediaUnlikerModel;
        public MediaUnlikerModel MediaUnlikerModel
        {
            get
            {
                return _mediaUnlikerModel;
            }
            set
            {
                if (_mediaUnlikerModel == null & _mediaUnlikerModel == value)
                    return;
                SetProperty(ref _mediaUnlikerModel, value);
            }
        }

        // NOTE: Required alias property to make it work at runtime
        public MediaUnlikerModel Model => MediaUnlikerModel;

    }
}
