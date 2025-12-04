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

namespace TwtDominatorCore.TDViewModel.TwtEngage
{
    public class LikeViewModel : BindableBase
    {
        private LikeModel _likeModel = new LikeModel();

        public LikeViewModel()
        {
            LikeModel.ListQueryType.Clear();

            Enum.GetValues(typeof(TdTweetInteractionQueryEnum)).Cast<TdTweetInteractionQueryEnum>().ToList().ForEach(
                query =>
                {
                    LikeModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())
                        .ToString());
                });


            //LikeModel.ListQueryType = Enum.GetNames(typeof(DominatorHouseCore.Enums.TdQuery.TdTweetInteractionQueryEnum)).ToList();
            // Load job configuration values
            LikeModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfLikesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfLikesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfLikesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfLikesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxLikesPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            #region  commands

            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
            DeleteMulipleCommand = new BaseCommand<object>(sender => true, DeleteMuliple);
            SplitInputToListCommand = new BaseCommand<object>(sender => true, SplitInputToListExecute);

            #endregion
        }

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

        public LikeModel Model => LikeModel;

        public void AddQueryOnSearchControl(object sender)
        {
            var followerSearchControl = sender as SearchQueryControl;

            if (followerSearchControl != null && string.IsNullOrEmpty(followerSearchControl.CurrentQuery.QueryValue))
            {
                followerSearchControl.QueryCollection.ForEach(query =>
                {
                    var currentQuery = followerSearchControl.CurrentQuery.Clone() as QueryInfo;

                    if (currentQuery == null) return;

                    currentQuery.QueryValue = query;

                    currentQuery.QueryTypeDisplayName
                        = currentQuery.QueryType.ToString();

                    currentQuery.QueryPriority
                        = LikeModel.SavedQueries.Count + 1;

                    LikeModel.SavedQueries.Add(currentQuery);
                });
            }
            else
            {
                if (followerSearchControl == null) return;

                followerSearchControl.CurrentQuery.QueryTypeDisplayName
                    = followerSearchControl.CurrentQuery.QueryType;

                var currentQuery = followerSearchControl.CurrentQuery.Clone() as QueryInfo;

                if (currentQuery == null) return;

                currentQuery.QueryPriority = LikeModel.SavedQueries.Count + 1;

                LikeModel.SavedQueries.Add(currentQuery);
            }
        }

        #region  ICommands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand DeleteMulipleCommand { get; set; }
        public ICommand SplitInputToListCommand { get; set; }

        #endregion

        #region Methods

        private void CustomFilter(object sender)
        {
            try
            {
                var ModuleSettingsUserControl = sender as ModuleSettingsUserControl<LikeViewModel, LikeModel>;
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
                var ModuleSettingsUserControl = sender as ModuleSettingsUserControl<LikeViewModel, LikeModel>;
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

        private void SplitInputToListExecute(object sender)
        {
            try
            {
                var Comments = LikeModel.UploadedCommentInput; // UnfollowerModel.Unfollower.CustomUsers;
                LikeModel.LstUploadedComment = Regex.Split(Comments, "\r\n").ToList();
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