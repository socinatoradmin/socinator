using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.RdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using RedditDominatorCore.RDModel;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace RedditDominatorCore.RDViewModel
{
    public class UserScraperViewModel : BindableBase
    {
        public UserScraperViewModel()
        {
            if (UserScraperModel.ListQueryType.Count == 0)
            {
                UserScraperModel.ListQueryType.Add(Application.Current
                    .FindResource(UserQuery.Keywords.GetDescriptionAttr())?.ToString());
                UserScraperModel.ListQueryType.Add(Application.Current
                    .FindResource(UserQuery.CustomUsers.GetDescriptionAttr())?.ToString());
            }

            UserScraperModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfUserscraperPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfUserscraperPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfUserscraperPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfUserscraperPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyNumberOfUsersToScrapePerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
            DeleteMultipleCommand = new BaseCommand<object>(sender => true, DeleteMultiple);
        }

        public UserScraperModel Model => UserScraperModel;

        #region Object creation logic

        private UserScraperModel _userScraperModel = new UserScraperModel();

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

        #endregion


        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand DeleteMultipleCommand { get; set; }

        #endregion

        #region Methods

        private void CustomFilter(object sender)
        {
            try
            {
                var moduleSettingsUserControl =
                    sender as ModuleSettingsUserControl<UserScraperViewModel, UserScraperModel>;
                moduleSettingsUserControl?.CustomFilter();
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
                    sender as ModuleSettingsUserControl<UserScraperViewModel, UserScraperModel>;
                moduleSettingsUserControl?.AddQuery(typeof(UserQuery));
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

        private void DeleteMultiple(object sender)
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