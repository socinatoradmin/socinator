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
    public class UserScraperViewModel : BindableBase
    {
        private UserScraperModel _userScraperModel = new UserScraperModel();

        public UserScraperViewModel()
        {
            UserScraperModel.ListQueryType.Clear();

            Enum.GetValues(typeof(TumblrPostQuery)).Cast<TumblrPostQuery>().ForEach(query =>
            {
                UserScraperModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())
                    ?.ToString());
            });

            UserScraperModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfUserScrapPerJob")?.ToString(),
                ActivitiesPerHourDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfUserScrapPerHour")?.ToString(),
                ActivitiesPerDayDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfUserScrapPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfUserScrapPerWeek")?.ToString(),
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyMaxScrapingPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
            DeleteMulipleCommand = new BaseCommand<object>(sender => true, DeleteMuliple);
        }

        public UserScraperModel UserScraperModel
        {
            get => _userScraperModel;
            set
            {
                if ((_userScraperModel == null) & (_userScraperModel == value))
                    return;
                SetProperty(ref _userScraperModel, value);
            }
        }

        public UserScraperModel Model => UserScraperModel;


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
                    sender as ModuleSettingsUserControl<UserScraperViewModel, UserScraperModel>;
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