using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace GramDominatorCore.GDViewModel.GrowFollower
{
    public class CloseFriendViewModel: BindableBase
    {

        private CloseFriendModel _closefriendModel = new CloseFriendModel();
        public CloseFriendViewModel()
        {
            CloseFriend.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current.FindResource("LangKeyNumberOfFollowerPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current.FindResource("LangKeyNumberOfFollowerPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current.FindResource("LangKeyNumberOfFollowerPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current.FindResource("LangKeyNumberOfFollowerPerWeek")?.ToString(),
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyMaxAddCloseFriendPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            SaveFollowUserCommandExecute = new BaseCommand<object>(sender => true, SaveExecute);
        }

        private void SaveExecute(object obj)
        {
            try
            {
                if (Model.IsCheckedCustomFollowerList && (string.IsNullOrEmpty(Model.CustomFollowerList) || string.IsNullOrWhiteSpace(Model.CustomFollowerList)))
                {
                    Dialog.ShowDialog(this, "Input Error",
                        "Please Provide At Least One Custom Follower To Make Close Friends");
                    return;
                }
                ToasterNotification.ShowSuccess("User List Saved Successfully");
            }
            catch { }
        }

        public ICommand SaveFollowUserCommandExecute {  get; set; }
        public CloseFriendModel CloseFriend
        {
            get
            {
                return _closefriendModel;
            }
            set
            {
                if (_closefriendModel == null & _closefriendModel == value)
                    return;
                SetProperty(ref _closefriendModel, value);
            }
        }
        public CloseFriendModel Model => CloseFriend;
    }
}
