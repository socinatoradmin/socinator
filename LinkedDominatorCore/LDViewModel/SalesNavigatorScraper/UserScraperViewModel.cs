using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.LdQuery;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.LDModel.SalesNavigatorScraper;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.LDViewModel.Scraper;
using MahApps.Metro.Controls.Dialogs;

namespace LinkedDominatorCore.LDViewModel.SalesNavigatorScraper
{
    public class UserScraperViewModel : BindableBase
    {
        private UserScraperModel _UserScraperModel = new UserScraperModel();

        public UserScraperViewModel()
        {
            UserScraperModel.ListQueryType.Clear();
            Enum.GetValues(typeof(LDScraperUserQueryParameters)).Cast<LDScraperUserQueryParameters>().ForEach(
                QueryType =>
                {
                    if (QueryType != LDScraperUserQueryParameters.JoinedGroupUrl &&
                        QueryType != LDScraperUserQueryParameters.Input &&
                        QueryType != LDScraperUserQueryParameters.Only1stConnection)
                        UserScraperModel.ListQueryType.Add(QueryType.GetDescriptionAttr()
                            .FromResourceDictionary());
                });


            UserScraperModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfScrapingPerJob")?.ToString(),
                ActivitiesPerHourDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfScrapingPerHour")?.ToString(),
                ActivitiesPerDayDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfScrapingPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfScrapingPerWeek")?.ToString(),
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyMaxScrape")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };


            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
            DeleteMulipleCommand = new BaseCommand<object>(sender => true, DeleteMuliple);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
        }

        public UserScraperModel UserScraperModel
        {
            get => _UserScraperModel;
            set
            {
                if ((_UserScraperModel == null) & (_UserScraperModel == value))
                    return;
                SetProperty(ref _UserScraperModel, value);
            }
        }

        public UserScraperModel Model => UserScraperModel;

        public void AddQuery(object sender)
        {
            try
            {
                var moduleSettingsUserControl =
                    sender as ModuleSettingsUserControl<UserScraperViewModel, UserScraperModel>;
                if (moduleSettingsUserControl == null)
                    return;

                if (Utils.IsMultiContains(moduleSettingsUserControl._queryControl.CurrentQuery.QueryType,
                    "Search Url", "Profile Url"))
                    new SearchUrlQueryHelper().AddQuery(moduleSettingsUserControl._queryControl,
                        moduleSettingsUserControl.CampaignName, ActivityType.SalesNavigatorUserScraper,
                        Model.SavedQueries);
                else
                    moduleSettingsUserControl.AddQuery(typeof(LDGrowConnectionUserQueryParameters));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void DeleteQuery(object sender)
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

        public void DeleteMuliple(object sender)
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

        private void CustomFilter(object sender)
        {
            try
            {
                var ModuleSettingsUserControl =
                    sender as ModuleSettingsUserControl<UserScraperViewModel, UserScraperModel>;
                ModuleSettingsUserControl.CustomFilter();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void AddQuery(SearchQueryControl queryControl, string campaignName, ActivityType activityType)
        {
            try
            {
                if (string.IsNullOrEmpty(queryControl.CurrentQuery.QueryValue.Trim()) &&
                    queryControl.QueryCollection.Count == 0)
                    return;

                var queryValueIndex = new List<int>();
                if (string.IsNullOrEmpty(queryControl.CurrentQuery.QueryValue) &&
                    queryControl.QueryCollection.Count != 0)
                {
                    queryControl.QueryCollection.ForEach(query =>
                    {
                        var currentQuery = queryControl.CurrentQuery.Clone() as QueryInfo;

                        if (currentQuery == null)
                            return;

                        currentQuery.QueryValue = query;
                        currentQuery.QueryTypeDisplayName = currentQuery.QueryType;
                        currentQuery.QueryPriority = Model.SavedQueries.Count + 1;


                        Model.SavedQueries.Add(currentQuery);
                        currentQuery.Index = Model.SavedQueries.IndexOf(currentQuery) + 1;
                    });
                    if (queryValueIndex.Count > 0)
                    {
                        if (queryValueIndex.Count <= 10)
                            GlobusLogHelper.log.Info(Log.AlreadyExistQuery, SocinatorInitialize.ActiveSocialNetwork,
                                campaignName, activityType,
                                "{ " + string.Join(" },{ ", queryValueIndex.ToArray()) + " }");
                        else
                            GlobusLogHelper.log.Info(Log.AlreadyExistQueryCount,
                                SocinatorInitialize.ActiveSocialNetwork, campaignName, activityType,
                                queryValueIndex.Count);
                    }
                }
                else
                {
                    queryControl.CurrentQuery.QueryTypeDisplayName = queryControl.CurrentQuery.QueryType;

                    

                    var currentQuery = queryControl.CurrentQuery.Clone() as QueryInfo;

                    if (currentQuery == null) return;

                    currentQuery.QueryValue = currentQuery.QueryValue.Trim();

                    currentQuery.QueryPriority = Model.SavedQueries.Count + 1;

                    if (IsQueryExist(currentQuery, Model.SavedQueries)) return;

                    Model.SavedQueries.Add(currentQuery);
                    currentQuery.Index = Model.SavedQueries.IndexOf(currentQuery) + 1;
                    queryControl.CurrentQuery.QueryValue = string.Empty;
                }

                queryControl.IsEnabled = true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private bool IsQueryExist(QueryInfo currentQuery, ObservableCollection<QueryInfo> queryToSave)
        {
            try
            {
                if (queryToSave.Any(x =>
                    x.QueryType == currentQuery.QueryType && x.QueryValue == currentQuery.QueryValue))
                {
                    Dialog.ShowDialog(Application.Current.MainWindow, "Alert",
                        "Query already Exist !!");
                    return true;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand DeleteMulipleCommand { get; set; }

        #endregion
    }
}