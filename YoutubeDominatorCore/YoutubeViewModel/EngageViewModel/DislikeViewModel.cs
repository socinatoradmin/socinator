using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.YdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using YoutubeDominatorCore.YoutubeModels.EngageModel;

namespace YoutubeDominatorCore.YoutubeViewModel.EngageViewModel
{
    public class DislikeViewModel : BindableBase
    {
        public DislikeViewModel()
        {
            DislikeModel.ListQueryType.Clear();

            Enum.GetValues(typeof(YdScraperParameters)).Cast<YdScraperParameters>().ToList().ForEach(query =>
            {
                if (query != YdScraperParameters.YTVideoCommenters)
                    DislikeModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())
                        .ToString());
            });

            DislikeModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyDislikesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyDislikesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyDislikesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyDislikesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaximumDislikesPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
            UploadPostsListCommand = new BaseCommand<object>(sender => true, CustomPost);
        }

        public DislikeModel Model => DislikeModel;

        #region Object creation logic

        private DislikeModel _dislikeModel = new DislikeModel();


        public DislikeModel DislikeModel
        {
            get => _dislikeModel;
            set
            {
                if ((_dislikeModel == null) & (_dislikeModel == value))
                    return;
                SetProperty(ref _dislikeModel, value);
            }
        }

        #endregion

        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand UploadPostsListCommand { get; set; }

        #endregion

        #region Methods

        private void CustomFilter(object sender)
        {
            try
            {
                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<DislikeViewModel, DislikeModel>;
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
                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<DislikeViewModel, DislikeModel>;

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

        private void CustomPost(object sender)
        {
            try
            {
                DislikeModel.ListCustomPosts = Regex.Split(DislikeModel.CustomPostsList, "\r\n").ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion
    }
}