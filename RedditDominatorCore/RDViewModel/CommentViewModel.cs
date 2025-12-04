using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.RdQuery;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using RedditDominatorCore.RDModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace RedditDominatorCore.RDViewModel
{
    public class CommentViewModel : BindableBase
    {
        private CommentModel _commentModel = new CommentModel();

        public CommentViewModel()
        {
            CommentModel.ListQueryType.Clear();

            if (CommentModel.ListQueryType.Count == 0)
            {
                CommentModel.ListQueryType.Add(Application.Current.FindResource(PostQuery.Keywords.GetDescriptionAttr())
                    ?.ToString());
                CommentModel.ListQueryType.Add(Application.Current
                    .FindResource(PostQuery.CustomUrl.GetDescriptionAttr())?.ToString());
                CommentModel.ListQueryType.Add(Application.Current
                    .FindResource(PostQuery.SocinatorPublisherCampaign.GetDescriptionAttr())?.ToString());
            }

            CommentModel.ListQueryType.Remove("LangKeySocinatorPublisherCampaign");

            CommentModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current
                    .FindResource("LangKeyMessagesToNumberOfProfilesPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current
                    .FindResource("LangKeyMessagesToNumberOfProfilesPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current
                    .FindResource("LangKeyMessagesToNumberOfProfilesPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current
                    .FindResource("LangKeyMessageToNumberOfProfilesPerWeek")?.ToString(),
                IncreaseActivityDisplayName =
                    Application.Current.FindResource("LangKeyMessagesToMaxProfilesPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };


            CommentModel.ManageCommentModel.LstQueries.Add(new QueryContent
            {
                Content = new QueryInfo
                {
                    QueryType = "All",
                    QueryValue = "All"
                }
            });
            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
            AddMessagesCommand = new BaseCommand<object>(sender => true, AddMessages);
            DeleteMulipleCommand = new BaseCommand<object>(sender => true, DeleteMuliple);
        }

        public CommentModel Model => CommentModel;

        public Visibility CancelEditVisibility { get; set; }

        public CommentModel CommentModel
        {
            get => _commentModel;
            set
            {
                if ((_commentModel == null) & (_commentModel == value))
                    return;
                SetProperty(ref _commentModel, value);
            }
        }


        #region Commands

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
                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<CommentViewModel, CommentModel>;
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
                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<CommentViewModel, CommentModel>;
                if (moduleSettingsUserControl == null ||
                    string.IsNullOrEmpty(moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue.Trim())
                    && !moduleSettingsUserControl._queryControl.QueryCollection.Any())
                    return;

                var queryControl = moduleSettingsUserControl._queryControl;

                var splittedQuery = queryControl.CurrentQuery.QueryValue.Contains(",")
                    ? queryControl.CurrentQuery.QueryValue.Split(',').Where(x => !string.IsNullOrEmpty(x.Trim()))
                        .ToList()
                    : new List<string> { queryControl.CurrentQuery.QueryValue.Trim() };

                if (string.IsNullOrEmpty(queryControl.CurrentQuery.QueryValue) &&
                    queryControl.QueryCollection.Count != 0)
                {
                    var queryType = queryControl.CurrentQuery.QueryType;
                    AddCommentToList(queryType, queryControl.QueryCollection);
                }
                else
                {
                    AddCommentToList(queryControl.CurrentQuery.QueryType, splittedQuery);
                }

                moduleSettingsUserControl?.AddQuery(typeof(PostQuery));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void AddCommentToList(string queryType, List<string> lstQuery)
        {
            foreach (var queryValue in lstQuery)
                if (!Model.ManageCommentModel.LstQueries.Any(y =>
                    y.Content.QueryValue == queryValue && y.Content.QueryType == queryType))
                    Model.ManageCommentModel.LstQueries.Add(new QueryContent
                    {
                        Content = new QueryInfo
                        {
                            QueryValue = queryValue,
                            QueryType = queryType
                        }
                    });
        }

        private void DeleteQuery(object sender)
        {
            try
            {
                var currentQuery = sender as QueryInfo;

                var queryToDelete = Model.ManageCommentModel.LstQueries.FirstOrDefault(x =>
                    currentQuery != null && x.Content.QueryValue == currentQuery.QueryValue &&
                    x.Content.QueryType == currentQuery.QueryType);

                if (Model.SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                    Model.SavedQueries.Remove(currentQuery);

                Model.ManageCommentModel.LstQueries.Remove(queryToDelete);

                foreach (var message in Model.LstManageCommentModel.ToList())
                {
                    var selectedQueryToDelete = message.SelectedQuery.FirstOrDefault(x =>
                        currentQuery != null && x.Content.QueryValue == currentQuery.QueryValue &&
                        x.Content.QueryType == currentQuery.QueryType);

                    message.SelectedQuery.Remove(selectedQueryToDelete);
                    if (message.SelectedQuery.Count == 0)
                        Model.LstManageCommentModel.Remove(message);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AddMessages(object sender)
        {
            var commentsData = sender as CommentControl;

            if (commentsData != null && string.IsNullOrEmpty(commentsData.Comments.CommentText?.Trim())) return;

            commentsData.Comments.SelectedQuery = new ObservableCollection<QueryContent>
                (commentsData.Comments.LstQueries.Where(x => x.IsContentSelected));

            commentsData.Comments.SelectedQuery.Remove(
                commentsData.Comments.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));

            if (commentsData.Comments.LstQueries.Count == 1 &&
                commentsData.Comments.LstQueries[0].Content.QueryType == "All")
            {
                GlobusLogHelper.log.Info("Please Add Query Type");
                return;
            }

            Model.LstManageCommentModel.Add(commentsData.Comments);

            commentsData.Comments = new ManageCommentModel
            {
                LstQueries = Model.ManageCommentModel.LstQueries
            };
            commentsData.Comments.LstQueries.Select(query =>
            {
                query.IsContentSelected = false;
                return query;
            }).ToList();

            Model.ManageCommentModel = commentsData.Comments;

            commentsData.ComboBoxQueries.ItemsSource = Model.ManageCommentModel.LstQueries;
        }

        private void DeleteMuliple(object sender)
        {
            var selectedQuery = Model.SavedQueries.Where(x => x.IsQuerySelected).ToList();
            try
            {
                foreach (var currentQuery in selectedQuery)
                    try
                    {
                        var queryToDelete = Model.ManageCommentModel.LstQueries.FirstOrDefault(x =>
                            x.Content.QueryValue == currentQuery.QueryValue
                            && x.Content.QueryType == currentQuery.QueryType);

                        if (Model.SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                            Model.SavedQueries.Remove(currentQuery);

                        Model.ManageCommentModel.LstQueries.Remove(queryToDelete);
                        foreach (var message in Model.LstManageCommentModel.ToList())
                        {
                            message.SelectedQuery.Remove(queryToDelete);
                            if (message.SelectedQuery.Count == 0)
                                Model.LstManageCommentModel.Remove(message);
                        }
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