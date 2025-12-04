/*
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.FriendsModel;
using System;
using System.Linq;
using System.Windows;

namespace FaceDominatorCore.FDViewModel.FriendsViewModel
{
    public class ManageFriendRequestViewModel:BindableBase
    {

        public ManageFriendRequestViewModel()
        {
            
            ManageFriendRequestModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current.FindResource("LangKeyNumberOfRequestPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current.FindResource("LangKeyNumberOfRequestPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current.FindResource("LangKeyNumberOfRequestPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current.FindResource("LangKeyNumberOfRequestPerWeek")?.ToString(),
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyMaxRequestPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }


        public ManageFriendRequestModel Model => ManageFriendRequestModel;

        private ManageFriendRequestModel _manageFriendRequestModel = new ManageFriendRequestModel();

        public ManageFriendRequestModel ManageFriendRequestModel
        {
            get
            {
                return _manageFriendRequestModel;
            }
            set
            {
                if (_manageFriendRequestModel == null & _manageFriendRequestModel == value)
                    return;
                SetProperty(ref _manageFriendRequestModel, value);
            }
        }

    }
}
*/
