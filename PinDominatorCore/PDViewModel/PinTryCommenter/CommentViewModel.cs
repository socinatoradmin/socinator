using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.PdQuery;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;

namespace PinDominatorCore.PDViewModel.PinTryCommenter
{
    public class CommentViewModel : BindableBase
    {
        private CommentModel _commentModel = new CommentModel();

        public CommentViewModel()
        {
            CommentModel.ListQueryType.Clear();

            Enum.GetValues(typeof(PDPinQueries)).Cast<PDPinQueries>().ToList().ForEach(query =>
            {
                CommentModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())
                    ?.ToString());
            });

            // Load job configuration values
            CommentModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfCommentPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfCommentPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfCommentPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfCommentPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxLikesPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
            AddCommentsCommand = new BaseCommand<object>(sender => true, AddComments);
            DeleteMulipleCommand = new BaseCommand<object>(sender => true, DeleteMultiple);
            UploadCommentsCommand = new BaseCommand<object>(sender => true, UploadComments);
        }

        public CommentModel Model => CommentModel;

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
        public ICommand AddCommentsCommand { get; set; }
        public ICommand DeleteMulipleCommand { get; set; }
        public ICommand UploadCommentsCommand { get; set; }

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

        private void AddComments(object sender)
        {
            var commentData = sender as CommentControl;

            if (commentData != null)
            {
                var commentText=commentData.Comments.CommentText = commentData.Comments.CommentText.Trim();
                if (string.IsNullOrEmpty(commentText))
                {
                    Dialog.ShowDialog(Application.Current.MainWindow, "Warning",
                        "Please type some comment !!");
                    return;
                }
                if(!string.IsNullOrEmpty(commentText) && commentText.Length > 500)
                {
                    var IsConfirmed = Dialog.ShowCustomDialog("LangKeyWarning".FromResourceDictionary(), "Comment Text is More than 500 Character,Do you Want to Short It?", "LangKeyYes".FromResourceDictionary(), "LangKeyNo".FromResourceDictionary())==MessageDialogResult.Affirmative;
                    if (IsConfirmed)
                        commentData.Comments.CommentText = commentText.Substring(0, 500);
                    else
                    {
                        commentData.Comments.CommentText = string.Empty;
                        return;
                    }
                }
                if (commentData == null) return;

                commentData.Comments.SelectedQuery =
                    new ObservableCollection<QueryContent>(
                        commentData.Comments.LstQueries.Where(x => x.IsContentSelected));
                commentData.Comments.SelectedQuery.Remove(
                    commentData.Comments.SelectedQuery.FirstOrDefault(x =>
                        x.Content.QueryValue == "All" && x.Content.QueryType == "All"));

                if (commentData.Comments.SelectedQuery.Count == 0 ||
                    string.IsNullOrEmpty(commentData.Comments.CommentText))
                {
                    Dialog.ShowDialog("Manage Comments Input Error", "Please select query type with comment(s)");
                    return;
                }

                commentData.Comments.SelectedQuery.Remove(
                    commentData.Comments.SelectedQuery.FirstOrDefault(x =>
                        x.Content.QueryValue == "All" && x.Content.QueryType == "All"));

                var commentList = commentData.Comments.CommentText.Split('\n')
                    .Where(x => !string.IsNullOrEmpty(x.Trim()))
                    .Select(y => y.Trim()).Distinct().ToList();

                if (CommentModel.IsChkAddMultipleComments)
                    commentList.ForEach(comment =>
                    {
                        commentData.Comments.CommentId = Utilities.GetGuid();
                        AddToCommentList(commentData.Comments, comment);
                    });
                else
                    Model.LstDisplayManageCommentModel.Add(commentData.Comments);

                commentData.Comments = new ManageCommentModel
                {
                    LstQueries = Model.ManageCommentModel.LstQueries
                };

                commentData.Comments.LstQueries.ForEach(query => { query.IsContentSelected = false; });

                Model.ManageCommentModel = commentData.Comments;
                commentData.ComboBoxQueries.ItemsSource = Model.ManageCommentModel.LstQueries;
            }
        }

        private void AddQuery(object sender)
        {
            try
            {
                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<CommentViewModel, CommentModel>;

                if (moduleSettingsUserControl == null ||
                    string.IsNullOrEmpty(moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue.Trim()) &&
                    !moduleSettingsUserControl._queryControl.QueryCollection.Any())
                    return;

                var splittedQueries = moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue.Contains(",")
                    ? moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue.Split(',')
                        .Where(x => !string.IsNullOrEmpty(x.Trim())).ToList()
                    : new List<string> {moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue};

                if (string.IsNullOrEmpty(moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue) &&
                    moduleSettingsUserControl._queryControl.QueryCollection.Count != 0)
                    foreach (var queryValue in moduleSettingsUserControl._queryControl.QueryCollection)
                    {
                        if (Model.ManageCommentModel.LstQueries.Any(x =>
                            x.Content.QueryValue == queryValue &&
                            x.Content.QueryType == moduleSettingsUserControl._queryControl.CurrentQuery.QueryType))
                            continue;
                        {
                            var addNew = new QueryContent
                            {
                                Content = new QueryInfo
                                {
                                    AddedDateTime = moduleSettingsUserControl._queryControl.CurrentQuery.AddedDateTime,
                                    Id = moduleSettingsUserControl._queryControl.CurrentQuery.Id,
                                    QueryPriority = moduleSettingsUserControl._queryControl.CurrentQuery.QueryPriority,
                                    QueryType = moduleSettingsUserControl._queryControl.CurrentQuery.QueryType,
                                    QueryTypeDisplayName = moduleSettingsUserControl._queryControl.CurrentQuery
                                        .QueryTypeDisplayName,
                                    QueryValue = queryValue
                                }
                            };
                            Model.ManageCommentModel.LstQueries.Add(addNew);
                            Model.LstDisplayManageCommentModel.ForEach(x =>
                            {
                                if (!x.LstQueries.Any(y =>
                                    addNew.Content.QueryType ==
                                    moduleSettingsUserControl._queryControl.CurrentQuery.QueryType &&
                                    y.Content.QueryValue == addNew.Content.QueryValue))
                                    x.LstQueries.Add(addNew);
                            });
                        }
                    }
                else
                    foreach (var queryValue in splittedQueries)
                    {
                        if (Model.ManageCommentModel.LstQueries.Any(x =>
                            x.Content.QueryType == moduleSettingsUserControl._queryControl.CurrentQuery.QueryType &&
                            x.Content.QueryValue == queryValue.Trim())) continue;
                        {
                            var addNew = new QueryContent
                            {
                                Content = new QueryInfo
                                {
                                    QueryValue = queryValue.Trim(),
                                    QueryType = moduleSettingsUserControl._queryControl.CurrentQuery.QueryType
                                }
                            };
                            Model.ManageCommentModel.LstQueries.Add(addNew);

                            Model.LstDisplayManageCommentModel.ForEach(x =>
                            {
                                if (!x.LstQueries.Any(y =>
                                    addNew.Content.QueryType ==
                                    moduleSettingsUserControl._queryControl.CurrentQuery.QueryType &&
                                    y.Content.QueryValue == addNew.Content.QueryValue))
                                    x.LstQueries.Add(addNew);
                            });
                        }
                    }

                AddQueryAll();
                moduleSettingsUserControl.AddQuery(typeof(PDPinQueries));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        /// <summary>
        ///     Add query "All" if ListOfQueries has more than 1 query (add if query "All" already doesn't exist)
        /// </summary>
        private void AddQueryAll()
        {
            if (Model.ManageCommentModel.LstQueries.Count > 1 &&
                !Model.ManageCommentModel.LstQueries.Any(x =>
                    x.Content.QueryValue == "All" && x.Content.QueryType == "All"))
                Model.ManageCommentModel.LstQueries.Insert(0, new QueryContent
                {
                    Content = new QueryInfo
                    {
                        QueryType = "All",
                        QueryValue = "All"
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
                foreach (var message in Model.LstDisplayManageCommentModel.ToList())
                {
                    message.SelectedQuery.Remove(message.SelectedQuery.GetDeletingQuery(queryToDelete));

                    if (message.SelectedQuery.Count == 0)
                        Model.LstDisplayManageCommentModel.Remove(message);

                    message.LstQueries.Remove(message.LstQueries.GetDeletingQuery(queryToDelete));
                }

                RemoveQueryAll();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DeleteMultiple(object sender)
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
                        foreach (var message in Model.LstDisplayManageCommentModel.ToList())
                        {
                            message.SelectedQuery.Remove(message.SelectedQuery.GetDeletingQuery(queryToDelete));

                            if (message.SelectedQuery.Count == 0)
                                Model.LstDisplayManageCommentModel.Remove(message);

                            message.LstQueries.Remove(message.LstQueries.GetDeletingQuery(queryToDelete));
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                RemoveQueryAll();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        /// <summary>
        ///     Remove query "All" if only 1 or 0 query left in ListOfQueries
        /// </summary>
        private void RemoveQueryAll()
        {
            if (Model.ManageCommentModel.LstQueries.Count < 3 &&
                Model.ManageCommentModel.LstQueries.Any(x =>
                    x.Content.QueryValue == "All" && x.Content.QueryType == "All"))
                Model.ManageCommentModel.LstQueries.Remove(Model.ManageCommentModel.LstQueries.First(y =>
                    y.Content.QueryValue == "All" && y.Content.QueryType == "All"));
        }

        private void UploadComments(object sender)
        {
            try
            {
                CommentModel.Message = CommentModel.Message.Trim();
                if (string.IsNullOrEmpty(CommentModel.Message))
                    return;
                CommentModel.LstComments = Regex.Split(CommentModel.Message, "\r\n").ToList();
                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest, "", ActivityType.Comment,
                    "Comments Saved Successfully");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

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
                        LstQueries = commentModel.LstQueries
                    });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion
    }
}