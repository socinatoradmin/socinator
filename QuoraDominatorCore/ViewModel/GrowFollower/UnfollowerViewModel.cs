using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.Models;

namespace QuoraDominatorCore.ViewModel.GrowFollower
{
    public class UnfollowerViewModel : BindableBase
    {
        private UnfollowerModel _unfollowerModel = new UnfollowerModel();

        public UnfollowerViewModel()
        {
            UnfollowerModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfUnfollowPerJob")?.ToString(),
                ActivitiesPerHourDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfUnfollowPerHour")?.ToString(),
                ActivitiesPerDayDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfUnfollowPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfUnfollowPerWeek")?.ToString(),
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyMaxUnfollowsPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            SaveInputCommand = new BaseCommand<object>(sender => true, SaveInput);
        }

        #region Commands

        public ICommand SaveInputCommand { get; set; }

        #endregion

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

        public UnfollowerModel Model => UnfollowerModel;

        private void SaveInput(object sender)
        {
            UnfollowerModel.CustomUsersList = sender as string;
        }
    }
}