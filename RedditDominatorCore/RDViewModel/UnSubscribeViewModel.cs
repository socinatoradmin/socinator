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
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace RedditDominatorCore.RDViewModel
{
    public class UnSubscribeViewModel : BindableBase
    {
        public UnSubscribeViewModel()
        {
            if (UnSubscribeModel.ListQueryType.Count == 0)
            {
                UnSubscribeModel.ListQueryType.Add(Application.Current
                    .FindResource(UserQuery.Keywords.GetDescriptionAttr())?.ToString());
                UnSubscribeModel.ListQueryType.Add(Application.Current
                    .FindResource(UserQuery.CustomUsers.GetDescriptionAttr())?.ToString());
            }

            UnSubscribeModel.ListQueryType.Remove("UsersWhoCommentedOnPost");

            UnSubscribeModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfUnSubscribesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfUnSubscribesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfUnSubscribesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfUnSubscribesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxUnSubscribesPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
            DeleteMulipleCommand = new BaseCommand<object>(sender => true, DeleteMuliple);
            SaveCommand = new BaseCommand<object>(sender => true, SaveInput);
        }

        public UnSubscribeModel Model => UnSubscribeModel;

        public bool IsEditCampaignName { get; set; }
        public Visibility CancelEditVisibility { get; set; }
        public string TemplateId { get; set; }
        public string CampaignName { get; set; }
        public string CampaignButtonContent { get; set; }
        public string SelectedAccountCount { get; set; }

        #region Object creation logic

        private UnSubscribeModel _unSubscribeModel = new UnSubscribeModel();

        public UnSubscribeModel UnSubscribeModel
        {
            get => _unSubscribeModel;
            set
            {
                if ((_unSubscribeModel == null) & (_unSubscribeModel == value))
                    return;
                SetProperty(ref _unSubscribeModel, value);
            }
        }

        #endregion


        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand DeleteMulipleCommand { get; set; }

        #endregion

        #region Methods

        private void CustomFilter(object sender)
        {
            try
            {
                var moduleSettingsUserControl =
                    sender as ModuleSettingsUserControl<UnSubscribeViewModel, UnSubscribeModel>;
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
                var moduleSettingsUserControl =
                    sender as ModuleSettingsUserControl<UnSubscribeViewModel, UnSubscribeModel>;
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

        private void SaveInput(object sender)
        {
            try
            {
                var customUsers = UnSubscribeModel.CustomCommunityList;
                UnSubscribeModel.LstCustomCommunity = Regex.Split(customUsers.Trim(), "\r\n").ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion
    }
}