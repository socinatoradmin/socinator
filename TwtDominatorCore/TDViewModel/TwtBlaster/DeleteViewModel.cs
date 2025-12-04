using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using TwtDominatorCore.TDModels;

namespace TwtDominatorCore.TDViewModel.TwtBlaster
{
    public class DeleteViewModel : BindableBase
    {
        private DeleteModel _deleteModel = new DeleteModel();

        public DeleteViewModel()
        {
            DeleteModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfDeletesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfDeletesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfDeletesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfDeletesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxDeletePerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }

        public DeleteModel DeleteModel
        {
            get => _deleteModel;
            set
            {
                if ((_deleteModel == null) & (_deleteModel == value))
                    return;
                SetProperty(ref _deleteModel, value);
            }
        }

        public DeleteModel Model => DeleteModel;
    }
}