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

namespace TumblrDominatorCore.ViewModels.Blog
{
    public class ReblogViewModel : BindableBase
    {
        private ReblogModel _reblogModel = new ReblogModel();

        public ReblogViewModel()
        {
            ReblogModel.ListQueryType.Clear();
            Enum.GetValues(typeof(TumblrPostQuery)).Cast<TumblrPostQuery>().ForEach(query =>
            {
                ReblogModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())
                    ?.ToString());
            });

            ReblogModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfRepostPerJob")?.ToString(),
                ActivitiesPerHourDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfRepostPerHour")?.ToString(),
                ActivitiesPerDayDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfRepostPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfRepostPerWeek")?.ToString(),
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyMaxLikesPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
            DeleteMulipleCommand = new BaseCommand<object>(sender => true, DeleteMuliple);
        }

        public ReblogModel ReblogModel
        {
            get => _reblogModel;
            set
            {
                if ((_reblogModel == null) & (_reblogModel == value))
                    return;
                SetProperty(ref _reblogModel, value);
            }
        }

        public ReblogModel Model => ReblogModel;


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
                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<ReblogViewModel, ReblogModel>;
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
                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<ReblogViewModel, ReblogModel>;
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