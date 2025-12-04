using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using RedditDominatorCore.RDModel;
using System;
using System.Linq;
using System.Windows;

namespace RedditDominatorCore.RDViewModel
{
    public class PostAutoActivityViewModel : BindableBase
    {
        public PostAutoActivityModel PostAutoActivity = new PostAutoActivityModel();
        public PostAutoActivityViewModel()
        {
            AutoActivityModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfAutoActivityPerJob")?.ToString(),
                ActivitiesPerHourDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfAutoActivityPerHour")?.ToString(),
                ActivitiesPerDayDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfAutoActivityPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfAutoActivityPerWeek")?.ToString(),
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyMaxAutoActivityPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }
        public PostAutoActivityModel AutoActivityModel
        {
            get => PostAutoActivity;
            set
            {
                if ((PostAutoActivity == null) & (PostAutoActivity == value))
                    return;
                SetProperty(ref PostAutoActivity, value);
            }
        }
        public PostAutoActivityModel Model => AutoActivityModel;
    }
}
