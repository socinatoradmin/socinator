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
    public class CommentScraperViewModel : BindableBase
    {
        public CommentScraperViewModel()
        {
            CommentScraperModel.ListQueryType.Clear();
            Enum.GetValues(typeof(YdScraperParameters)).Cast<YdScraperParameters>().ToList().ForEach(query =>
            {
                if (query != YdScraperParameters.YTVideoCommenters)
                    CommentScraperModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())
                        .ToString());
            });

            CommentScraperModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyCommentsScrapPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyCommentsScrapPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyCommentsScrapPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyCommentsScrapPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaximumCommentssScrapPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
        }

        public CommentScraperModel Model => CommentScraperModel;

        #region Object creation logic

        private CommentScraperModel _likeCommentsModel = new CommentScraperModel();

        public CommentScraperModel CommentScraperModel
        {
            get => _likeCommentsModel;
            set
            {
                if ((_likeCommentsModel == null) & (_likeCommentsModel == value))
                    return;
                SetProperty(ref _likeCommentsModel, value);
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
                    sender as ModuleSettingsUserControl<CommentScraperViewModel, CommentScraperModel>;
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
                    sender as ModuleSettingsUserControl<CommentScraperViewModel, CommentScraperModel>;

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