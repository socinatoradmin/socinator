using System;
using System.Collections.ObjectModel;
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
    public class CommentViewModel : BindableBase
    {
        private CommentModel _commentModel = new CommentModel();

        public CommentViewModel()
        {
            CommentModel.ListQueryType.Clear();

            Enum.GetValues(typeof(TdTweetInteractionQueryEnum)).Cast<TdTweetInteractionQueryEnum>().ToList().ForEach(
                query =>
                {
                    CommentModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())
                        .ToString());
                });

            // CommentModel.ListQueryType = Enum.GetNames(typeof(DominatorHouseCore.Enums.TdQuery.TdTweetInteractionQueryEnum)).ToList();
            // Load job configuration values
            CommentModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfCommentsPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfCommentsPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfCommentsPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfCommentsPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxCommentPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            #region  commands

            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
            DeleteMulipleCommand = new BaseCommand<object>(sender => true, DeleteMuliple);
            SplitInputToListCommand = new BaseCommand<object>(sender => true, SplitInputToListExecute);
            AddCommentsCommand = new BaseCommand<object>(sender => true, AddComments);

            AddToQueryList("Default", "Default");

            #endregion
        }

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

        public CommentModel Model => CommentModel;


        private void AddToCommentList(ManageCommentModel commentModel, string commentText)
        {
            try
            {
                var isContain = false;
                CommentModel.LstDisplayManageCommentModel.ForEach(lstMessage =>
                {
                    if (lstMessage.CommentText.ToLower().Equals(commentText.ToLower()))
                        isContain = lstMessage.SelectedQuery.Any(x => commentModel.SelectedQuery.Contains(x));
                });
                if (!isContain)
                    CommentModel.LstDisplayManageCommentModel.Add(new ManageCommentModel
                    {
                        CommentText = commentText,
                        SelectedQuery =
                            new ObservableCollection<QueryContent>(commentModel.LstQueries
                                .Where(x => x.IsContentSelected).ToList()),
                        CommentId = commentModel.CommentId,
                        FilterText = commentModel.FilterText,
                        LstQueries = commentModel.LstQueries,
                        MediaPath = commentModel.MediaPath,
                        MediaList = commentModel.MediaList
                    });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AddToQueryList(string keyword, string queryType)
        {
            try
            {
                if (!CommentModel.ManageCommentModel.LstQueries.Any(x =>
                    x.Content.QueryValue == keyword && x.Content.QueryType == queryType))
                    CommentModel.ManageCommentModel.LstQueries.Add(new QueryContent
                    {
                        Content = new QueryInfo
                        {
                            QueryValue = keyword,
                            QueryType =
                                queryType /*QueryType=FindResource("TdLangReplyToContainSpecificWord").ToString()*/
                        }
                    });
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
        public ICommand SplitInputToListCommand { get; set; }
        public ICommand AddCommentsCommand { get; set; }

        #endregion

        #region Methods

        private void CustomFilter(object sender)
        {
            try
            {
                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<CommentViewModel, CommentModel>;
                moduleSettingsUserControl.CustomFilter();
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
                if (string.IsNullOrEmpty(moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue) &&
                    !moduleSettingsUserControl._queryControl.QueryCollection.Any()) return;
                if (string.IsNullOrEmpty(moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue) &&
                    moduleSettingsUserControl._queryControl.QueryCollection.Any())
                {
                    foreach (var queryValue in moduleSettingsUserControl._queryControl.QueryCollection)
                        if (!Model.ManageCommentModel.LstQueries.Any(x =>
                            x.Content.QueryValue == queryValue &&
                            x.Content.QueryType == moduleSettingsUserControl._queryControl.CurrentQuery.QueryType))
                            Model.ManageCommentModel.LstQueries.Add(new QueryContent
                            {
                                Content = new QueryInfo
                                {
                                    QueryValue = queryValue,
                                    QueryType = moduleSettingsUserControl._queryControl.CurrentQuery.QueryType
                                }
                            });
                }
                else
                {
                    if (!Model.ManageCommentModel.LstQueries.Any(x =>
                        x.Content.QueryValue == moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue &&
                        x.Content.QueryType == moduleSettingsUserControl._queryControl.CurrentQuery.QueryType))
                        Model.ManageCommentModel.LstQueries.Add(new QueryContent
                        {
                            Content = new QueryInfo
                            {
                                QueryValue = moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue,
                                QueryType = moduleSettingsUserControl._queryControl.CurrentQuery.QueryType
                            }
                        });
                }

                moduleSettingsUserControl.AddQuery(typeof(TdUserInteractionQueryEnum));
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
                ManageCommentModel currentManageCommentModel = null;
                if(queryToDelete == null)
                {
                    currentManageCommentModel = Model.LstDisplayManageCommentModel?.ToList()?.FirstOrDefault(x => x.SelectedQuery.Any(y => currentQuery != null && y.Content.QueryType == currentQuery.QueryType && y.Content.QueryValue == currentQuery.QueryValue));
                    if(currentManageCommentModel != null )
                        queryToDelete = currentManageCommentModel?.LstQueries?.FirstOrDefault(x => x.Content.QueryValue != "Default" && x.Content.QueryType==currentQuery.QueryType && x.Content.QueryValue==currentQuery.QueryValue);
                }
                if (Model.SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                    Model.SavedQueries.Remove(currentQuery);

                Model.ManageCommentModel.LstQueries.Remove(queryToDelete);
                foreach (var message in Model.LstDisplayManageCommentModel.ToList())
                {
                    message.SelectedQuery.Remove(GetDeletingQuery(message.SelectedQuery, queryToDelete));

                    if (message.SelectedQuery.Count == 1 && message.SelectedQuery.FirstOrDefault(x=>x.Content.QueryValue=="Default")!=null)
                        Model.LstDisplayManageCommentModel.Remove(message);
                    else if(currentManageCommentModel !=null)
                        Model.LstDisplayManageCommentModel.Remove(currentManageCommentModel);
                    message.LstQueries.Remove(GetDeletingQuery(message.LstQueries, queryToDelete));
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        public QueryContent GetDeletingQuery(ObservableCollection<QueryContent> queryList, QueryContent queryToDelete)
        {
            return queryList.FirstOrDefault(x =>
                x.Content.QueryType == queryToDelete.Content.QueryType &&
                x.Content.QueryValue == queryToDelete.Content.QueryValue);
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
                        var removeData = CommentModel.ManageCommentModel.LstQueries.SingleOrDefault(x =>
                            x.Content.QueryType == currentQuery.QueryType &&
                            x.Content.QueryValue == currentQuery.QueryValue);
                        CommentModel.ManageCommentModel.LstQueries.Remove(removeData);
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

        private void SplitInputToListExecute(object sender)
        {
            try
            {
                var customUsers = CommentModel.Unfollower.CustomUsers;
                CommentModel.Unfollower.ListCustomUsers = Regex.Split(customUsers, "\r\n").ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AddComments(object sender)
        {
            try
            {
                var commentData = sender as CommentMediaControl;

                if (ValidateQuery(commentData))
                    return;
                if (CommentModel.IsMultilineComment)
                {
                    var commentList = commentData.Comments.CommentText.Split('\n').ToList();
                    commentList = commentList.Where(x => !string.IsNullOrEmpty(x.Trim())).Select(y => y.Trim()).ToList();

                    commentList.ForEach(comment => { AddToCommentList(commentData.Comments, comment); });
                }
                else
                {
                    AddToCommentList(commentData.Comments, commentData.Comments.CommentText);
                }
                var lastData = CommentModel.ManageCommentModel.LstQueries;
                try
                {
                    commentData.Comments = new ManageCommentModel();
                }
                catch
                {
                }
                commentData.Comments.LstQueries = CommentModel.ManageCommentModel.LstQueries = lastData;
                commentData.Comments.LstQueries.Select(query =>
                {
                    query.IsContentSelected = false;
                    return query;
                }).ToList();
                CommentModel.ManageCommentModel = commentData.Comments;
                commentData.ComboBoxQueries.ItemsSource = CommentModel.ManageCommentModel.LstQueries;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        public bool ValidateQuery(CommentMediaControl commentData)
        {
            var IsInValid = false;
            try
            {
                IsInValid = commentData == null ||
                            string.IsNullOrEmpty(commentData.Comments?.CommentText) &&
                            string.IsNullOrEmpty(commentData.Comments?.MediaPath) || commentData.Comments
                                .LstQueries.Where(x => x.IsContentSelected).ToList().Count == 0;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return IsInValid;
        }
        #endregion
    }
}