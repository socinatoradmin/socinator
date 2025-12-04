using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.YdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using YoutubeDominatorCore.YoutubeModels.ScraperModel;

namespace YoutubeDominatorCore.YoutubeViewModel.Scraper_ViewModel
{
    public class PostScraperViewModel : BindableBase
    {
        public PostScraperViewModel()
        {
            Enum.GetValues(typeof(YdScraperParameters)).Cast<YdScraperParameters>().ToList().ForEach(query =>
            {
                if (query != YdScraperParameters.YTVideoCommenters)
                    PostScraperModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())
                        .ToString());
            });

            PostScraperModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyPostsScrapPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyPostsScrapPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyPostsScrapPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyPostsScrapPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaximumPostsScrapPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
        }

        public PostScraperModel Model => PostScraperModel;

        #region Object creation logic

        private PostScraperModel _postScraperModel = new PostScraperModel();


        public PostScraperModel PostScraperModel
        {
            get => _postScraperModel;
            set
            {
                if ((_postScraperModel == null) & (_postScraperModel == value))
                    return;
                SetProperty(ref _postScraperModel, value);
            }
        }

        #endregion

        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }

        #endregion

        #region Methods

        private void CustomFilter(object sender)
        {
            try
            {
                var moduleSettingsUserControl =
                    sender as ModuleSettingsUserControl<PostScraperViewModel, PostScraperModel>;
                moduleSettingsUserControl.CustomFilter();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AddQuery(object sender)
        {
            try
            {
                var moduleSettingsUserControl =
                    sender as ModuleSettingsUserControl<PostScraperViewModel, PostScraperModel>;

                moduleSettingsUserControl.AddQuery(typeof(YdScraperParameters), Model.ListQueryType);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DeleteQuery(object sender)
        {
            try
            {
                var currentQuery = sender as QueryInfo;

                if (Model.SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                    Model.SavedQueries.Remove(currentQuery);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion
    }
}