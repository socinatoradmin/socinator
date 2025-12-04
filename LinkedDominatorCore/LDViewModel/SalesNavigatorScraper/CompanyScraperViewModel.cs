using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.LdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.LDModel.SalesNavigatorScraper;
using LinkedDominatorCore.LDViewModel.Scraper;

namespace LinkedDominatorCore.LDViewModel.SalesNavigatorScraper
{
    public class CompanyScraperViewModel : BindableBase
    {
        private CompanyScraperModel _CompanyScraperModel = new CompanyScraperModel();

        public CompanyScraperViewModel()
        {
            CompanyScraperModel.ListQueryType.Clear();
            Enum.GetValues(typeof(LDScraperUserQueryParameters)).Cast<LDScraperUserQueryParameters>().ForEach(
                QueryType =>
                {
                    if (QueryType == LDScraperUserQueryParameters.SearchUrl)
                        CompanyScraperModel.ListQueryType.Add(QueryType.GetDescriptionAttr()
                            .FromResourceDictionary());
                });
            CompanyScraperModel.JobConfiguration = new JobConfiguration
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
            DeleteMulipleCommand = new BaseCommand<object>(sender => true, DeleteMultiple);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
        }

        public CompanyScraperModel CompanyScraperModel
        {
            get => _CompanyScraperModel;
            set
            {
                if ((_CompanyScraperModel == null) & (_CompanyScraperModel == value))
                    return;
                SetProperty(ref _CompanyScraperModel, value);
            }
        }

        public CompanyScraperModel Model => CompanyScraperModel;

        public void AddQuery(object sender)
        {
            try
            {
                var moduleSettingsUserControl =
                    sender as ModuleSettingsUserControl<CompanyScraperViewModel, CompanyScraperModel>;

                if (moduleSettingsUserControl == null)
                    return;

                if (moduleSettingsUserControl._queryControl.CurrentQuery.QueryType == "Search Url")
                    new SearchUrlQueryHelper().AddQuery(moduleSettingsUserControl._queryControl,
                        moduleSettingsUserControl.CampaignName, ActivityType.SalesNavigatorCompanyScraper,
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

        public void DeleteMultiple(object sender)
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
                var moduleSettingsUserControl =
                    sender as ModuleSettingsUserControl<CompanyScraperViewModel, CompanyScraperModel>;
                moduleSettingsUserControl?.CustomFilter();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand DeleteMulipleCommand { get; set; }

        #endregion
    }
}