using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.TumblrQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TumblrDominatorCore.Models;

namespace TumblrDominatorCore.ViewModels.Scraper
{
    public class PostScraperViewModel : BindableBase
    {
        private PostScraperModel _postScraperModel = new PostScraperModel();

        public PostScraperViewModel()
        {
            PostScraperModel.ListQueryType.Clear();

            Enum.GetValues(typeof(TumblrPostQuery)).Cast<TumblrPostQuery>().ForEach(query =>
            {
                PostScraperModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())
                    ?.ToString());
            });

            PostScraperModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfPostsScrapingPerJob")?.ToString(),
                ActivitiesPerHourDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfPostsScrapingPerHour")?.ToString(),
                ActivitiesPerDayDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfPostsScrapingPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfPostsScrapingPerWeek")?.ToString(),
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyMaxScrapingPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
            DeleteMulipleCommand = new BaseCommand<object>(sender => true, DeleteMuliple);
        }

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

        public PostScraperModel Model => PostScraperModel;


        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand AddMessagesCommand { get; set; }
        public ICommand DeleteMulipleCommand { get; set; }

        #endregion

        #region Methods

        private void CustomFilter(object sender)
        {
            try
            {
                var moduleSettingsUserControl =
                    sender as ModuleSettingsUserControl<PostScraperViewModel, PostScraperModel>;
                if (moduleSettingsUserControl == null)
                    throw new ArgumentNullException(nameof(moduleSettingsUserControl));
                moduleSettingsUserControl.CustomFilter();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        private static void AddQuery(object sender)
        {
            try
            {
                var moduleSettingsUserControl =
                    sender as ModuleSettingsUserControl<PostScraperViewModel, PostScraperModel>;
                moduleSettingsUserControl?.AddQuery(typeof(TumblrQuery));
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

        private void DeleteMuliple(object sender)
        {
            var selectedQuery = Model.SavedQueries.Where(x => x.IsQuerySelected).ToList();
            try
            {
                foreach (var currentQuery in selectedQuery)
                    try
                    {
                        if (Model.SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                            Model.SavedQueries.Remove(currentQuery);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion
    }
}