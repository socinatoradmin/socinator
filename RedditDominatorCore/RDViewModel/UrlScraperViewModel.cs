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
    public class UrlScraperViewModel : BindableBase
    {
        public UrlScraperViewModel()
        {
            if (UrlScraperModel.ListQueryType.Count == 0)
            {
                UrlScraperModel.ListQueryType.Add(Application.Current
                    .FindResource(PostQuery.Keywords.GetDescriptionAttr())?.ToString());
                UrlScraperModel.ListQueryType.Add(Application.Current
                    .FindResource(PostQuery.CustomUrl.GetDescriptionAttr())?.ToString());
                //UrlScraperModel.ListQueryType.Add(Application.Current.FindResource(PostQuery.SocinatorPublisherCampaign.GetDescriptionAttr())?.ToString());
            }

            UrlScraperModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfUrlscraperPerJob")?.ToString(),
                ActivitiesPerHourDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfUrlscraperPerHour")?.ToString(),
                ActivitiesPerDayDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfUrlscraperPerday")?.ToString(),
                ActivitiesPerWeekDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfUrlscraperPerweek")?.ToString(),
                IncreaseActivityDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfMaxurlscraPerday")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
            DeleteMulipleCommand = new BaseCommand<object>(sender => true, DeleteMuliple);
        }

        public UrlScraperModel Model => UrlScraperModel;

        #region Object creation logic

        private UrlScraperModel _urlScraperModel = new UrlScraperModel();

        public UrlScraperModel UrlScraperModel
        {
            get => _urlScraperModel;
            set
            {
                if ((_urlScraperModel == null) & (_urlScraperModel == value))
                    return;
                SetProperty(ref _urlScraperModel, value);
            }
        }

        #endregion


        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand DeleteMulipleCommand { get; set; }

        #endregion

        #region Methods

        private void CustomFilter(object sender)
        {
            try
            {
                var moduleSettingsUserControl =
                    sender as ModuleSettingsUserControl<UrlScraperViewModel, UrlScraperModel>;
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
                    sender as ModuleSettingsUserControl<UrlScraperViewModel, UrlScraperModel>;
                moduleSettingsUserControl?.AddQuery(typeof(PostQuery));
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