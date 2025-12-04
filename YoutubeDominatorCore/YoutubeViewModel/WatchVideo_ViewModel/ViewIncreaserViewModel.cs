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
using YoutubeDominatorCore.YoutubeModels.WatchVideoModel;

namespace YoutubeDominatorCore.YoutubeViewModel.WatchVideo_ViewModel
{
    public class ViewIncreaserViewModel : BindableBase
    {
        public ViewIncreaserViewModel()
        {
            ViewIncreaserModel.ListQueryType.Clear();
            Enum.GetValues(typeof(YdScraperParameters)).Cast<YdScraperParameters>().ToList().ForEach(query =>
            {
                if (query != YdScraperParameters.YTVideoCommenters)
                    ViewIncreaserModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())
                        .ToString());
            });
            ViewIncreaserModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyViewIncreasePerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyViewIncreasePerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyViewIncreasePerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyViewIncreasePerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaximumViewIncreasePerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
        }

        public ViewIncreaserModel Model => ViewIncreaserModel;

        #region Object creation logic

        private ViewIncreaserModel _viewIncreaserModel = new ViewIncreaserModel();


        public ViewIncreaserModel ViewIncreaserModel
        {
            get => _viewIncreaserModel;
            set
            {
                if ((_viewIncreaserModel == null) & (_viewIncreaserModel == value))
                    return;
                SetProperty(ref _viewIncreaserModel, value);
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
                    sender as ModuleSettingsUserControl<ViewIncreaserViewModel, ViewIncreaserModel>;
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
                    sender as ModuleSettingsUserControl<ViewIncreaserViewModel, ViewIncreaserModel>;

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