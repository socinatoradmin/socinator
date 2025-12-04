using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.RdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using RedditDominatorCore.RDModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace RedditDominatorCore.RDViewModel
{
    public class ReplyViewModel : BindableBase
    {
        private ReplyModel _replyModel = new ReplyModel();

        public ReplyViewModel()
        {
            ReplyModel.ListQueryType.Clear();

            if (ReplyModel.ListQueryType.Count == 0)
                ReplyModel.ListQueryType.Add(Application.Current.FindResource(PostQuery.CustomUrl.GetDescriptionAttr())
                    ?.ToString());
            ReplyModel.JobConfiguration = new JobConfiguration
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


            ReplyModel.ManageCommentModel.LstQueries.Add(new QueryContent
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

        public ReplyModel Model => ReplyModel;

        public Visibility CancelEditVisibility { get; set; }

        public ReplyModel ReplyModel
        {
            get => _replyModel;
            set
            {
                if ((_replyModel == null) & (_replyModel == value))
                    return;
                SetProperty(ref _replyModel, value);
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
                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<ReplyViewModel, ReplyModel>;
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
                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<ReplyViewModel, ReplyModel>;
                if (!Model.ManageCommentModel.LstQueries.Any(x =>
                    moduleSettingsUserControl != null &&
                    x.Content.QueryValue == moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue &&
                    x.Content.QueryType == moduleSettingsUserControl._queryControl.CurrentQuery.QueryType))
                    if (moduleSettingsUserControl != null)
                        Model.ManageCommentModel.LstQueries.Add(new QueryContent
                        {
                            Content = new QueryInfo
                            {
                                QueryValue = moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue,
                                QueryType = moduleSettingsUserControl._queryControl.CurrentQuery.QueryType
                            }
                        });

                moduleSettingsUserControl?.AddQuery(typeof(PostQuery));
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

            if (commentsData == null) return;
            commentsData.Comments.SelectedQuery =
                new ObservableCollection<QueryContent>(
                    commentsData.Comments.LstQueries.Where(x => x.IsContentSelected));

            if (commentsData.Comments.SelectedQuery.Count == 0) return;
            commentsData.Comments.SelectedQuery.Remove(
                commentsData.Comments.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));

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