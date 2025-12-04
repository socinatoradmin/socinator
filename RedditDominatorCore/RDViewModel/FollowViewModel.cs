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
    public class FollowViewModel : BindableBase
    {
        public FollowViewModel()
        {
            if (FollowModel.ListQueryType.Count == 0)
            {
                FollowModel.ListQueryType.Add(Application.Current.FindResource(UserQuery.Keywords.GetDescriptionAttr())
                    ?.ToString());
                FollowModel.ListQueryType.Add(Application.Current
                    .FindResource(UserQuery.CustomUsers.GetDescriptionAttr())?.ToString());
                FollowModel.ListQueryType.Add(Application.Current
                    .FindResource(UserQuery.UsersWhoCommentedOnPost.GetDescriptionAttr())?.ToString());
            }

            FollowModel.ListQueryType.Remove("UsersWhoCommentedOnPost");

            FollowModel.JobConfiguration = new JobConfiguration
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
        }

        public FollowModel Model => FollowModel;

        public bool IsEditCampaignName { get; set; }
        public Visibility CancelEditVisibility { get; set; }
        public string TemplateId { get; set; }
        public string CampaignName { get; set; }
        public string CampaignButtonContent { get; set; }
        public string SelectedAccountCount { get; set; }

        #region Object creation logic

        private FollowModel _followModel = new FollowModel();

        public FollowModel FollowModel
        {
            get => _followModel;
            set
            {
                if ((_followModel == null) & (_followModel == value))
                    return;
                SetProperty(ref _followModel, value);
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
                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<FollowViewModel, FollowModel>;
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
                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<FollowViewModel, FollowModel>;
                moduleSettingsUserControl?.AddQuery(typeof(UserQuery));
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