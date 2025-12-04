using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using PinDominatorCore.PDModel;

namespace PinDominatorCore.PDViewModel.DeletePins
{
    public class DeletePinsViewModel : BindableBase
    {
        private DeletePinModel _deletePostModel = new DeletePinModel();

        public DeletePinsViewModel()
        {
            DeletePinModel.JobConfiguration = new JobConfiguration
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

        public DeletePinModel Model => DeletePinModel;

        public DeletePinModel DeletePinModel
        {
            get => _deletePostModel;
            set
            {
                if ((_deletePostModel == null) & (_deletePostModel == value))
                    return;
                SetProperty(ref _deletePostModel, value);
            }
        }
    }
}