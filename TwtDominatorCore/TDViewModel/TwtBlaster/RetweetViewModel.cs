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
    public class RetweetViewModel : BindableBase
    {
        private RetweetModel _objRetweetModel = new RetweetModel();

        public RetweetViewModel()
        {
            RetweetModel.ListQueryType.Clear();

            Enum.GetValues(typeof(TdTweetInteractionQueryEnum)).Cast<TdTweetInteractionQueryEnum>().ToList().ForEach(
                query =>
                {
                    RetweetModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())
                        .ToString());
                });

            //RetweetModel.ListQueryType = Enum.GetNames(typeof(DominatorHouseCore.Enums.TdQuery.TdTweetInteractionQueryEnum)).ToList();


            // Load job configuration values
            RetweetModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfRetweetsPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfRetweetsPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfRetweetsPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfRetweetsPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxRetweetPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            #region  commands

            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
            DeleteMulipleCommand = new BaseCommand<object>(sender => true, DeleteMuliple);
            SplitCommentInputToListCommand = new BaseCommand<object>(sender => true, SplitCommentInputToListExecute);
            UploadQuoteTweets = new BaseCommand<object>(sender => true, UploadQuoteTweetsList);


            #endregion
        }

        public RetweetModel RetweetModel
        {
            get => _objRetweetModel;
            set
            {
                if ((_objRetweetModel == null) & (_objRetweetModel == value))
                    return;
                SetProperty(ref _objRetweetModel, value);
            }
        }

        public RetweetModel Model => RetweetModel;

        private void SplitCommentInputToListExecute(object obj)
        {
            try
            {
                var CustomUsers = RetweetModel.UploadedCommentInput;
                RetweetModel.Unfollower.ListCustomUsers = Regex.Split(CustomUsers, "\r\n").ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #region  ICommands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand AddMessagesCommand { get; set; }
        public ICommand DeleteMulipleCommand { get; set; }
        public ICommand SplitCommentInputToListCommand { get; set; }
        public ICommand UploadQuoteTweets { get; set; }

        #endregion
        private void UploadQuoteTweetsList(object obj)
        {
            try
            {
                var CustomUsers = RetweetModel.UploadQuotesTweets;
                RetweetModel.Unfollower.ListCustomUsers = Regex.Split(CustomUsers, "\r\n").ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
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
                var ModuleSettingsUserControl = sender as ModuleSettingsUserControl<RetweetViewModel, RetweetModel>;
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

        #endregion
    }
}