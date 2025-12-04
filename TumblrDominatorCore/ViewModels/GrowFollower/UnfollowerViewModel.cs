using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using TumblrDominatorCore.Models;

namespace TumblrDominatorCore.ViewModels.GrowFollower
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

            UploadNotesCommand = new BaseCommand<object>(sender => true, CustomUser);
        }

        public ICommand UploadNotesCommand { get; set; }

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

        private void CustomUser(object sender)
        {
            try
            {
                UnfollowerModel.LstCustomusers = Regex.Split(UnfollowerModel.CustomUsersList, "\r\n").ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}