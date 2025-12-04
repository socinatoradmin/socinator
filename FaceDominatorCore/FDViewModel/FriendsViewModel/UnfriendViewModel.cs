using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.FriendsModel;
using System;
using System.Linq;
using System.Windows;

namespace FaceDominatorCore.FDViewModel.FriendsViewModel
{
    public class UnfriendViewModel : BindableBase
    {
        public UnfriendViewModel()
        {
            UnfriendModel.UnfriendOptionModel = new DominatorHouseCore.Models.FacebookModels.UnfriendOption()
            {
                SourceDisplayName = Application.Current.FindResource("LangKeyUnfriendSource")?.ToString(),
                BySoftwareDisplayName = Application.Current.FindResource("LangKeyPeopleAddedBySoftware")?.ToString(),
                OutsideSoftwareDisplayName = Application.Current.FindResource("LangKeyPeopleAddedOutsideSoftware")?.ToString()

            };

            UnfriendModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current.FindResource("LangKeyNumberOfUnfriendPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current.FindResource("LangKeyNumberOfUnfriendPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current.FindResource("LangKeyNumberOfUnfriendPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current.FindResource("LangKeyNumberOfUnfriendPerWeek")?.ToString(),
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyMaxUnfriendPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }

        //Get Global Database
        public UnfriendModel Model => UnfriendModel;

        private UnfriendModel _unfriendModel = new UnfriendModel();

        public UnfriendModel UnfriendModel
        {
            get
            {
                return _unfriendModel;
            }
            set
            {
                if (_unfriendModel == null & _unfriendModel == value)
                    return;
                SetProperty(ref _unfriendModel, value);
            }
        }

    }
}
