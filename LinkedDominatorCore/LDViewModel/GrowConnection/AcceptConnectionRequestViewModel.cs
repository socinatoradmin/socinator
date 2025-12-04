using System;
using System.Linq;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.LDModel.GrowConnection;

namespace LinkedDominatorCore.LDViewModel.GrowConnection
{
    public class AcceptConnectionRequestViewModel : BindableBase
    {
        private AcceptConnectionRequestModel _AcceptConnectionRequestModel = new AcceptConnectionRequestModel();

        public AcceptConnectionRequestViewModel()
        {
            AcceptConnectionRequestModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current
                    .FindResource("LangKeyNumberOfConnectionRequestsToAcceptPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current
                    .FindResource("LangKeyNumberOfConnectionRequestsToAcceptPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current
                    .FindResource("LangKeyNumberOfConnectionRequestsToAcceptPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current
                    .FindResource("LangKeyNumberOfConnectionRequestsToAcceptPerWeek")?.ToString(),
                IncreaseActivityDisplayName = Application.Current
                    .FindResource("LangKeyMaxConnectionRequestsToAcceptPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }

        public AcceptConnectionRequestModel AcceptConnectionRequestModel
        {
            get => _AcceptConnectionRequestModel;
            set
            {
                if ((_AcceptConnectionRequestModel == null) & (_AcceptConnectionRequestModel == value))
                    return;
                SetProperty(ref _AcceptConnectionRequestModel, value);
            }
        }

        public AcceptConnectionRequestModel Model => AcceptConnectionRequestModel;
    }
}