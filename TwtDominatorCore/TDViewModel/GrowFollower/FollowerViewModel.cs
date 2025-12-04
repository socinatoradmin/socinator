using System;
using System.Linq;
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

namespace TwtDominatorCore.TDViewModel.GrowFollower
{
    public class FollowerViewModel : BindableBase
    {
        private FollowerModel _followerModel = new FollowerModel();

        public FollowerViewModel()
        {
            FollowerModel.ListQueryType.Clear();

            Enum.GetValues(typeof(TdUserInteractionQueryEnum)).Cast<TdUserInteractionQueryEnum>().ToList().ForEach(
                query =>
                {
                    FollowerModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())
                        .ToString());
                });

            //FollowerModel.ListQueryType = Enum.GetNames(typeof(TdUserInteractionQueryEnum)).ToList();
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

            #region  commands

            StoreModuleSettingsCommand = new BaseCommand<object>(sender => true, StoreCampaignExecute);
            CreateCampaignCommand = new BaseCommand<object>(sender => true, CreateCampaignExecute);
            AddSearchQueryCommand = new BaseCommand<object>(sender => true, AddSearchQueryExecute);

            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
            DeleteMulipleCommand = new BaseCommand<object>(sender => true, DeleteMuliple);

            #endregion
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

        public FollowerModel Model => FollowerModel;


        private void CreateCampaignExecute(object obj)
        {
        }

        private void AddSearchQueryExecute(object obj)
        {
            var followerSearchControl = obj as SearchQueryControl;

            if (followerSearchControl != null && string.IsNullOrEmpty(followerSearchControl.CurrentQuery.QueryValue))
            {
                followerSearchControl.QueryCollection.ForEach(query =>
                {
                    var currentQuery = followerSearchControl.CurrentQuery.Clone() as QueryInfo;

                    if (currentQuery == null) return;

                    currentQuery.QueryValue = query;

                    currentQuery.QueryTypeDisplayName = currentQuery.QueryType;
                    // = (currentQuery.QueryType).ToString();

                    currentQuery.QueryPriority
                        = FollowerModel.SavedQueries.Count + 1;

                    FollowerModel.SavedQueries.Add(currentQuery);
                });
            }
            else
            {
                if (followerSearchControl == null) return;
                followerSearchControl.CurrentQuery.QueryTypeDisplayName = followerSearchControl.CurrentQuery.QueryType;
                //= ((UserQueryParameters)followerSearchControl.CurrentQuery.QueryType).ToString();
                var currentQuery = followerSearchControl.CurrentQuery.Clone() as QueryInfo;
                if (currentQuery == null) return;
                currentQuery.QueryPriority = FollowerModel.SavedQueries.Count + 1;
                FollowerModel.SavedQueries.Add(currentQuery);
                followerSearchControl.CurrentQuery = new QueryInfo();
            }
        }

        private void StoreCampaignExecute(object sender)
        {
        }

        #region  ICommands

        public ICommand StoreModuleSettingsCommand { get; set; }
        public ICommand CreateCampaignCommand { get; set; }
        public ICommand AddSearchQueryCommand { get; set; }
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
                var ModuleSettingsUserControl = sender as ModuleSettingsUserControl<FollowerViewModel, FollowerModel>;
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
                var ModuleSettingsUserControl = sender as ModuleSettingsUserControl<FollowerViewModel, FollowerModel>;
                ModuleSettingsUserControl.AddQuery(typeof(TdUserInteractionQueryEnum));
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