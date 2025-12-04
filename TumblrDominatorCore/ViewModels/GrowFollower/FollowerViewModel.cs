using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.TumblrQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using TumblrDominatorCore.Models;

namespace TumblrDominatorCore.ViewModels.GrowFollower
{
    public class FollowerViewModel : BindableBase
    {
        private FollowerModel _followerModel = new FollowerModel();

        public FollowerViewModel()
        {
            FollowerModel.ListQueryType.Clear();

            Enum.GetValues(typeof(TumblrQuery)).Cast<TumblrQuery>().ForEach(query =>
            {
                FollowerModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())
                    ?.ToString());
            });
            try
            {
                FollowerModel.JobConfiguration = new JobConfiguration
                {
                    ActivitiesPerJobDisplayName = "LangKeyNumberOfFollowsPerJob".FromResourceDictionary(),
                    ActivitiesPerHourDisplayName = "LangKeyNumberOfFollowsPerHour".FromResourceDictionary(),
                    ActivitiesPerDayDisplayName = "LangKeyNumberOfFollowsPerDay".FromResourceDictionary(),
                    ActivitiesPerWeekDisplayName = "LangKeyNumberOfFollowsPerWeek".FromResourceDictionary(),
                    IncreaseActivityDisplayName = "LangKeyMaxFollowsPerDay".FromResourceDictionary(),
                    RunningTime = RunningTimes.DayWiseRunningTimes,
                    Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
                };

                AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
                CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
                DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
                DeleteMulipleCommand = new BaseCommand<object>(sender => true, DeleteMuliple);
                UploadMessagesCommand = new BaseCommand<object>(sender => true, UploadMessage);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public FollowerModel FollowerModel
        {
            get => _followerModel;
            set
            {
                if ((_followerModel == null) & (_followerModel == value))
                    return;
                SetProperty(ref _followerModel, value);
            }
        }

        // NOTE: Required alias property to make it work at runtime
        public FollowerModel Model => FollowerModel;

        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand UploadMessagesCommand { get; set; }
        public ICommand DeleteMulipleCommand { get; set; }

        #endregion


        #region Methods

        private void CustomFilter(object sender)
        {
            try
            {
                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<FollowerViewModel, FollowerModel>;
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
                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<FollowerViewModel, FollowerModel>;
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

        private void UploadMessage(object sender)
        {
            try
            {
                FollowerModel.LstMessages = Regex.Split(FollowerModel.Message, "\r\n").ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion
    }
}