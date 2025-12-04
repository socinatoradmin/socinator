using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.TdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using TwtDominatorCore.TDModels;

namespace TwtDominatorCore.TDViewModel.TwtBlaster
{
    public class ReposterViewModel : BindableBase
    {
        private ReposterModel _ReposterModel = new ReposterModel();

        public ReposterViewModel()
        {
            ReposterModel.ListQueryType.Clear();

            Enum.GetValues(typeof(TdTweetInteractionQueryEnum)).Cast<TdTweetInteractionQueryEnum>().ToList().ForEach(
                query =>
                {
                    ReposterModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())
                        .ToString());
                });

            //ReposterModel.ListQueryType = Enum.GetNames(typeof(DominatorHouseCore.Enums.TdQuery.TdTweetInteractionQueryEnum)).ToList();

            // Load job configuration values
            ReposterModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfRepostsPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfRepostsPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfRepostsPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfRepostsPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxRepostPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            #region  commands

            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
            DeleteMulipleCommand = new BaseCommand<object>(sender => true, DeleteMuliple);
            UpdateMediaDownloadPathCommand = new BaseCommand<object>(sender => true, UpdateMediaDownloadPathExecute);
            UploadQuoteTweets = new BaseCommand<object>(sender => true, UploadQuoteTweetsList);
            #endregion
        }
        private void UploadQuoteTweetsList(object obj)
        {
            try
            {
                var CustomUsers = ReposterModel.UploadQuotesTweets;
                ReposterModel.Unfollower.ListCustomUsers = Regex.Split(CustomUsers, "\r\n").ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        public ReposterModel ReposterModel
        {
            get => _ReposterModel;
            set => SetProperty(ref _ReposterModel, value);
        }

        public ReposterModel Model => ReposterModel;


        #region  ICommands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand DeleteMulipleCommand { get; set; }

        public ICommand UpdateMediaDownloadPathCommand { get; set; }
        public ICommand UploadQuoteTweets { get; set; }

        #endregion

        #region Methods

        private void CustomFilter(object sender)
        {
            try
            {
                var ModuleSettingsUserControl = sender as ModuleSettingsUserControl<ReposterViewModel, ReposterModel>;
                ModuleSettingsUserControl.CustomFilter();
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
                var ModuleSettingsUserControl = sender as ModuleSettingsUserControl<ReposterViewModel, ReposterModel>;
                ModuleSettingsUserControl.AddQuery(typeof(TdTweetInteractionQueryEnum));
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

        private void UpdateMediaDownloadPathExecute(object sender)
        {
            try
            {
                var downloadedFolderPath = FileUtilities.GetExportPath();

                if (string.IsNullOrEmpty(downloadedFolderPath))
                    downloadedFolderPath = ConstantVariable.GetDownloadedMediaFolderPath;

                ReposterModel.DownloadFolderPath = downloadedFolderPath;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion
    }
}