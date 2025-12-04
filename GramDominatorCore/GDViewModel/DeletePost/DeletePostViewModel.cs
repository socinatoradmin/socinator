using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;

namespace GramDominatorCore.GDViewModel.DeletePost
{
    public class DeletePostViewModel : BindableBase
    {

        public DeletePostViewModel()
        {
            InitializeJobSetting();
        }


        private void InitializeJobSetting()
        {
            DeletePostModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfPostsDeletePerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfPostsDeletePerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfPostsDeletePerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfPostsDeletePerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxPostsDeletePerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }


        private DeletePostModel _deletePostModel = new DeletePostModel();
        //private DeletePostModel _deletePostModel;
        public DeletePostModel DeletePostModel
        {
            get
            {
                return _deletePostModel;
            }
            set
            {
                if (_deletePostModel == null & _deletePostModel == value)
                    return;
                SetProperty(ref _deletePostModel, value);
            }
        }

        // NOTE: Required alias property to make it work at runtime
        public DeletePostModel Model => DeletePostModel;
    }
}
