using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.FriendsModel;
using System;
using System.Linq;
using System.Windows;

namespace FaceDominatorCore.FDViewModel.FriendsViewModel
{

    public class UnfollowFriendViewModel : BindableBase
    {
        public UnfollowFriendViewModel()
        {
            UnfollowModel.UnfriendOptionModel = new DominatorHouseCore.Models.FacebookModels.UnfriendOption()
            {
                SourceDisplayName = Application.Current.FindResource("LangKeyUnfollowFriendSource")?.ToString(),
                BySoftwareDisplayName = Application.Current.FindResource("LangKeyPeopleAddedBySoftware")?.ToString(),
                OutsideSoftwareDisplayName = Application.Current.FindResource("LangKeyPeopleAddedOutsideSoftware")?.ToString()

            };

            UnfollowModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current.FindResource("LangKeyNumberOfUnfollowPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current.FindResource("LangKeyNumberOfUnfollowPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current.FindResource("LangKeyNumberOfUnfollowPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current.FindResource("LangKeyNumberOfUnfollowPerWeek")?.ToString(),
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyMaxUnfollowsPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }

        //Get Global Database
        public UnfollowFriendModel Model => UnfollowModel;

        private UnfollowFriendModel _unfollowModel = new UnfollowFriendModel();

        public UnfollowFriendModel UnfollowModel
        {
            get
            {
                return _unfollowModel;
            }
            set
            {
                if (_unfollowModel == null & _unfollowModel == value)
                    return;
                SetProperty(ref _unfollowModel, value);
            }
        }

    }
}
