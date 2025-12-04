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
    public class GroupInviterViewModel : BindableBase
    {
        private GroupInviterModel _GroupInviterModel = new GroupInviterModel();

        public GroupInviterViewModel()
        {
            if (GroupInviterModel.ListQueryType.Count == 0)
            {
                GroupInviterModel.ListQueryType.Clear();
                Enum.GetValues(typeof(LDScraperUserQueryParameters)).Cast<LDScraperUserQueryParameters>().ForEach(
                    QueryType =>
                    {
                        if (QueryType != LDScraperUserQueryParameters.Input &&
                            QueryType != LDScraperUserQueryParameters.JoinedGroupUrl &&
                            QueryType != LDScraperUserQueryParameters.Keyword &&
                            QueryType != LDScraperUserQueryParameters.SearchUrl)
                            GroupInviterModel.ListQueryType.Add(QueryType.GetDescriptionAttr()
                                .FromResourceDictionary());
                    });
            }

            GroupInviterModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfGroupInvitesPerJob")?.ToString(),
                ActivitiesPerHourDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfGroupInvitesPerHour")?.ToString(),
                ActivitiesPerDayDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfGroupInvitesPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfGroupInvitesPerWeek")?.ToString(),
                IncreaseActivityDisplayName =
                    Application.Current.FindResource("LangKeyMaxGroupInvitesPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
            DeleteMulipleCommand = new BaseCommand<object>(sender => true, DeleteMuliple);
        }

        public GroupInviterModel GroupInviterModel
        {
            get => _GroupInviterModel;
            set
            {
                if ((_GroupInviterModel == null) & (_GroupInviterModel == value))
                    return;
                SetProperty(ref _GroupInviterModel, value);
            }
        }

        public ICommand StoreModuleSettingsCommand { get; set; }
        public ICommand CreateCampaignCommand { get; set; }
        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand DeleteMulipleCommand { get; set; }


        public GroupInviterModel Model => GroupInviterModel;
        public void AddQuery(object sender)
        {
            try
            {
                var ModuleSettingsUserControl =
                    sender as ModuleSettingsUserControl<GroupInviterViewModel, GroupInviterModel>;
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
    }
}