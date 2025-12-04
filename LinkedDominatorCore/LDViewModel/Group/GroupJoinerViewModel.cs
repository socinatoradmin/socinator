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
using LinkedDominatorCore.LDModel;

namespace LinkedDominatorCore.LDViewModel.Group
{
    public class GroupJoinerViewModel : BindableBase
    {
        private GroupJoinerModel _GroupJoinerModel = new GroupJoinerModel();

        public GroupJoinerViewModel()
        {
            if (GroupJoinerModel.ListQueryType.Count == 0)
            {
                GroupJoinerModel.ListQueryType.Clear();
                Enum.GetValues(typeof(LDGroupQueryParameters)).Cast<LDGroupQueryParameters>().ForEach(QueryType =>
                {
                    if (QueryType != LDGroupQueryParameters.JoinedGroupUrl)
                        GroupJoinerModel.ListQueryType.Add(QueryType.GetDescriptionAttr()
                            .FromResourceDictionary());
                });
            }

            GroupJoinerModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfGroupJoinsPerJob")?.ToString(),
                ActivitiesPerHourDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfGroupJoinsPerHour")?.ToString(),
                ActivitiesPerDayDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfGroupJoinsPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfGroupJoinsPerWeek")?.ToString(),
                IncreaseActivityDisplayName =
                    Application.Current.FindResource("LangKeyMaxGroupJoinsPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
            DeleteMulipleCommand = new BaseCommand<object>(sender => true, DeleteMuliple);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
        }

        public GroupJoinerModel GroupJoinerModel
        {
            get => _GroupJoinerModel;
            set
            {
                if ((_GroupJoinerModel == null) & (_GroupJoinerModel == value))
                    return;
                SetProperty(ref _GroupJoinerModel, value);
            }
        }

        public ICommand StoreModuleSettingsCommand { get; set; }
        public ICommand CreateCampaignCommand { get; set; }
        public ICommand AddSearchQueryCommand { get; set; }
        public GroupJoinerModel Model => GroupJoinerModel;

        public void AddQuery(object sender)
        {
            try
            {
                var ModuleSettingsUserControl =
                    sender as ModuleSettingsUserControl<GroupJoinerViewModel, GroupJoinerModel>;
                ModuleSettingsUserControl.AddQuery(typeof(LDGrowConnectionUserQueryParameters));
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
                    sender as ModuleSettingsUserControl<GroupJoinerViewModel, GroupJoinerModel>;
                ModuleSettingsUserControl.CustomFilter();
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