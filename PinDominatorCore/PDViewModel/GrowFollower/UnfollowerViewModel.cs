using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using PinDominatorCore.PDModel;

namespace PinDominatorCore.PDViewModel.GrowFollower
{
    public class UnfollowerViewModel : BindableBase
    {
        private UnfollowerModel _unfollowerModel = new UnfollowerModel();

        public UnfollowerViewModel()
        {
            UnfollowerModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfUnfollowPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfUnfollowPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfUnfollowPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfUnfollowPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxUnfollowsPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            UploadUsersListCommand = new BaseCommand<object>(sender => true, CustomUser);
            CustomUserCheckedCommand = new BaseCommand<object>(sender => true, CustomUserChecked);
            PeopleFollowedOutsideSoftwareCheckedCommand =
                new BaseCommand<object>(sender => true, PeopleFollowedOutsideSoftwareChecked);
            PeopleFollowedBySoftwareCheckedCommand =
                new BaseCommand<object>(sender => true, PeopleFollowedBySoftwareCheecked);
        }

        public UnfollowerModel Model => UnfollowerModel;

        public ICommand UploadUsersListCommand { get; set; }
        public ICommand CustomUserCheckedCommand { get; set; }
        public ICommand PeopleFollowedOutsideSoftwareCheckedCommand { get; set; }
        public ICommand PeopleFollowedBySoftwareCheckedCommand { get; set; }

        public UnfollowerModel UnfollowerModel
        {
            get => _unfollowerModel;
            set
            {
                if ((_unfollowerModel == null) & (_unfollowerModel == value))
                    return;
                SetProperty(ref _unfollowerModel, value);
            }
        }

        private void CustomUser(object sender)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(UnfollowerModel.CustomUsersList))
                {
                    UnfollowerModel.CustomUsersList = string.Empty;
                    return;
                }

                UnfollowerModel.ListCustomUsers = Regex.Split(UnfollowerModel.CustomUsersList, "\r\n").ToList();
                GlobusLogHelper.log.Info("LangKeyDataSavedSuccessfully".FromResourceDictionary());
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void CustomUserChecked(object sender)
        {
            try
            {
                if (UnfollowerModel.IsChkPeopleFollowedBySoftwareChecked &&
                    UnfollowerModel.IsChkPeopleFollowedOutsideSoftwareChecked)
                    UnfollowerModel.IsChkCustomUsersListChecked = false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void PeopleFollowedOutsideSoftwareChecked(object sender)
        {
            try
            {
                if (UnfollowerModel.IsChkPeopleFollowedBySoftwareChecked &&
                    UnfollowerModel.IsChkPeopleFollowedOutsideSoftwareChecked)
                    UnfollowerModel.IsChkCustomUsersListChecked = false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void PeopleFollowedBySoftwareCheecked(object sender)
        {
            try
            {
                if (UnfollowerModel.IsChkPeopleFollowedBySoftwareChecked &&
                    UnfollowerModel.IsChkPeopleFollowedOutsideSoftwareChecked)
                    UnfollowerModel.IsChkCustomUsersListChecked = false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}