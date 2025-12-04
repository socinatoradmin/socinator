using System;
using System.Linq;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.TdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using TwtDominatorCore.TDModels;

namespace TwtDominatorCore.TDViewModel.Scraper
{
    public class DownloadViewModel : BindableBase
    {
        private ScrapeTweetModel _scrapeTweetModel = new ScrapeTweetModel();

        public DownloadViewModel()
        {
            ScrapeTweetModel.ListQueryType.Clear();

            Enum.GetValues(typeof(TdTweetInteractionQueryEnum)).Cast<TdTweetInteractionQueryEnum>().ToList().ForEach(
                query =>
                {
                    ScrapeTweetModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())
                        .ToString());
                });

            // ScrapeTweetModel.ListQueryType = Enum.GetNames(typeof(DominatorHouseCore.Enums.TdQuery.TdTweetInteractionQueryEnum)).ToList();
            // Load job configuration values
            ScrapeTweetModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfDownloadPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfDownloadPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfDownloadPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfDownloadPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxDownloadPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }

        public ScrapeTweetModel ScrapeTweetModel
        {
            get => _scrapeTweetModel;
            set
            {
                if ((_scrapeTweetModel == null) & (_scrapeTweetModel == value))
                    return;
                SetProperty(ref _scrapeTweetModel, value);
            }
        }

        public ScrapeTweetModel Model => ScrapeTweetModel;
    }
}