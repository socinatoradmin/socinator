using System;
using System.Linq;
using DominatorHouseCore.Models;
using GramDominatorCore.GDModel;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;

namespace GramDominatorCore.GDViewModel.InstaScraper
{
    public class HashtagsScraperViewModel : BindableBase
    {
        public HashtagsScraperViewModel()
        {
            HashtagsScraperModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current.FindResource("LangKeyNumberOfHashtagsPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current.FindResource("LangKeyNumberOfHashtagsPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current.FindResource("LangKeyNumberOfHashtagsPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current.FindResource("LangKeyNumberOfHashtagsPerWeek")?.ToString(),
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyMaxHashtagsPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };         
        }      
        private HashtagsScraperModel _hashtagsScraperModel = new HashtagsScraperModel();       
        public HashtagsScraperModel HashtagsScraperModel
        {
            get
            {
                return _hashtagsScraperModel;
            }
            set
            {
                if (_hashtagsScraperModel == null & _hashtagsScraperModel == value)
                    return;
                SetProperty(ref _hashtagsScraperModel, value);
            }
        }

     
     
        public HashtagsScraperModel Model => HashtagsScraperModel;
    }
}
