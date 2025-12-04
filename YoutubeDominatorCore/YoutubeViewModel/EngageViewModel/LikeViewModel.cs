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
using YoutubeDominatorCore.YoutubeModels.EngageModel;

namespace YoutubeDominatorCore.YoutubeViewModel.EngageViewModel
{
    public class LikeViewModel : BindableBase
    {
        public LikeViewModel()
        {
            LikeModel.ListQueryType.Clear();
            Enum.GetValues(typeof(YdScraperParameters)).Cast<YdScraperParameters>().ToList().ForEach(query =>
            {
                if (query != YdScraperParameters.YTVideoCommenters)
                    LikeModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())
                        .ToString());
            });

            LikeModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyLikesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyLikesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyLikesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyLikesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaximumLikesPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
        }

        public LikeModel Model => LikeModel;

        #region Object creation logic

        private LikeModel _likeModel = new LikeModel();


        public LikeModel LikeModel
        {
            get => _likeModel;
            set
            {
                if ((_likeModel == null) & (_likeModel == value))
                    return;
                SetProperty(ref _likeModel, value);
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
                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<LikeViewModel, LikeModel>;
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
                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<LikeViewModel, LikeModel>;
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