using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using MahApps.Metro.Controls.Dialogs;
using DominatorHouseCore.LogHelper;

namespace GramDominatorCore.GDViewModel.GrowFollower
{
    public class UnfollowerViewModel : BindableBase
    {

        public UnfollowerViewModel()
        {
            UnfollowerModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current.FindResource("LangKeyNumberOfUnfollowPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current.FindResource("LangKeyNumberOfUnfollowPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current.FindResource("LangKeyNumberOfUnfollowPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current.FindResource("LangKeyNumberOfUnfollowPerWeek")?.ToString(),
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyMaxUnfollowsPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
         
            StoreModuleSettingsCommand = new BaseCommand<object>(StoreCampaignCanExecute, StoreCampaignExecute);
            SaveCommand = new BaseCommand<object>(saveCommandCanExecute, SaveCommandExecute);
            SaveFollowUserCommand=new BaseCommand<object>(saveFollowUserCommandCanExecute, SaveFollowUserCommandExecute);
        }

        private UnfollowerModel _unfollowerModel = new UnfollowerModel();

        public UnfollowerModel UnfollowerModel
        {
            get
            {
                return _unfollowerModel;
            }
            set
            {
                if (_unfollowerModel == null & _unfollowerModel == value)
                    return;
                SetProperty(ref _unfollowerModel, value);
            }
        }

        public UnfollowerModel Model => UnfollowerModel;

        public ICommand StoreModuleSettingsCommand { get; set; }

        public ICommand SaveCommand { get; set; }
        public ICommand SaveFollowUserCommand { get; set; }
        private static bool StoreCampaignCanExecute(object sender)
        {
            return true;
        }

        private void StoreCampaignExecute(object sender)
        {
            // TODO: ???
        }
        private static bool saveCommandCanExecute(object sender)
        {
            return true;
        }
        private static bool saveFollowUserCommandCanExecute(object sender)
        {
            return true;
        }
        private void SaveCommandExecute(object sender)
        {
            if(string.IsNullOrWhiteSpace(UnfollowerModel.CustomUsersList))
            {
                UnfollowerModel.CustomUsersList = string.Empty;
                GlobusLogHelper.log.Info("Please add custom user list");
            }

        }

        private void SaveFollowUserCommandExecute(object sender)
        {
            if (string.IsNullOrWhiteSpace(UnfollowerModel.CustomFollowUsersList))
            {
                UnfollowerModel.CustomFollowUsersList = string.Empty;
                GlobusLogHelper.log.Info("Please add custom user list");
            }

        }
    }
}
