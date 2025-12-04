using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using TwtDominatorCore.TDModels;

namespace TwtDominatorCore.TDViewModel.GrowFollower
{
    public class UnfollowerViewModel : BindableBase
    {
        private UnfollowerModel _unfollowerModel = new UnfollowerModel();

        public UnfollowerViewModel()
        {
            UnfollowerModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfUnfollowsPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfUnfollowsPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfUnfollowsPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfUnfollowsPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxUnfollowsPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            SplitInputToListCommand = new BaseCommand<object>(sender => true, SplitInputToListExecute);
        }

        public UnfollowerModel Model => UnfollowerModel;

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

        public ICommand SplitInputToListCommand { get; set; }


        private void SplitInputToListExecute(object sender)
        {
            try
            {
                var CustomUsers = UnfollowerModel.Unfollower.CustomUsers;
                var tempList = Regex.Split(CustomUsers, "\r\n").ToList();
                UnfollowerModel.Unfollower.ListCustomUsers =
                    tempList.Where(x => !string.IsNullOrEmpty(x.Trim())).ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}