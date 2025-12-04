using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.YdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeModels.EngageModel;

namespace YoutubeDominatorCore.YoutubeViewModel.EngageViewModel
{
    public class CommentViewModel : BindableBase
    {
        public CommentViewModel()
        {
            CommentModel.ListQueryType.Clear();

            Enum.GetValues(typeof(YdScraperParameters)).Cast<YdScraperParameters>().ToList().ForEach(query =>
            {
                if (query != YdScraperParameters.YTVideoCommenters)
                    CommentModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())
                        ?.ToString());
            });

            CommentModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyCommentsPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyCommentsPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyCommentsPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyCommentsPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaximumCommentsPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            //CommentModel.ManageCommentModel.LstQueries.Add(new QueryContent
            //{
            //    Content = new QueryInfo
            //    {
            //        QueryType = "All",
            //        QueryValue = "All"
            //    }
            //});

            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
            AddCommentsCommand = new BaseCommand<object>(sender => true, AddComments);
            DeleteMulipleCommand = new BaseCommand<object>(sender => true, DeleteMultiple);
        }

        public CommentModel Model => CommentModel;

        #region Object creation logic

        private CommentModel _commentModel = new CommentModel();


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

        #endregion

        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand AddCommentsCommand { get; set; }
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
                    string.IsNullOrEmpty(moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue.Trim()) &&
                    !moduleSettingsUserControl._queryControl.QueryCollection.Any())
                    return;

                var splittedQueries = moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue.Contains(",")
                    ? moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue.Split(',')
                        .Where(x => !string.IsNullOrEmpty(x.Trim())).ToList()
                    : new List<string> { moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue };

                if (string.IsNullOrEmpty(moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue) &&
                    moduleSettingsUserControl._queryControl.QueryCollection.Count != 0)
                    foreach (var queryValue in moduleSettingsUserControl._queryControl.QueryCollection)
                    {
                        var valueToAdd = queryValue;
                        var queryType = string.Empty;
                        if (valueToAdd.StartsWith("Youtube\tComment\t"))
                        {
                            valueToAdd = valueToAdd.Replace("Youtube\tComment\t", "").Trim();
                            queryType = valueToAdd.Split('\t')[0].Trim();
                            valueToAdd = valueToAdd.Replace(queryType, "").Trim('\t');
                        }

                        if (string.IsNullOrEmpty(queryType))
                            queryType = moduleSettingsUserControl._queryControl.CurrentQuery.QueryType;
                        if (string.IsNullOrEmpty(valueToAdd))
                            continue;
                        if (Model.ManageCommentModel.LstQueries.Any(x => x.Content.QueryValue == valueToAdd
                                                                         && x.Content.QueryType == queryType))
                            continue;
                        {
                            var addNew = new QueryContent
                            {
                                Content = new QueryInfo
                                {
                                    QueryValue = valueToAdd,
                                    QueryType = queryType
                                }
                            };
                            Model.ManageCommentModel.LstQueries.Add(addNew);
                            Model.LstDisplayManageCommentModel.ForEach(x =>
                            {
                                if (!x.LstQueries.Any(y =>
                                    addNew.Content.QueryType == queryType &&
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
                            x.Content.QueryValue == queryValue)) continue;
                        {
                            var addNew = new QueryContent
                            {
                                Content = new QueryInfo
                                {
                                    QueryValue = queryValue,
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

                moduleSettingsUserControl.AddQuery(typeof(YdScraperParameters));

                moduleSettingsUserControl.ClearQueryCollection();
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

        private void AddComments(object sender)
        {
            try
            {
                var commentData = sender as CommentControl;
                if (commentData == null) return;
                //commentData.Comments.SerialNo = ObjViewModel.CommentModel.LstDisplayManageCommentModel.Count + 1;

                //Todo : please check below statement
                // commentData.Comments.SelectedQuery.Remove("All");

                commentData.Comments.SelectedQuery =
                    new ObservableCollection<QueryContent>(
                        commentData.Comments.LstQueries.Where(x => x.IsContentSelected));

                if (commentData.Comments.SelectedQuery.Count == 0)
                {
                    Dialog.ShowDialog(Application.Current.MainWindow, "Warning",
                        "Please select atleast one query!!");
                    return;
                }

                if (commentData.Comments.SelectedQuery.Count == 1 &&
                    commentData.Comments.SelectedQuery.FirstOrDefault()?.Content.QueryValue == "All")
                {
                    Dialog.ShowDialog(Application.Current.MainWindow, "Warning",
                        "Please add atleast one query!!");
                    return;
                }

                if (string.IsNullOrEmpty(commentData.Comments.CommentText))
                {
                    Dialog.ShowDialog(Application.Current.MainWindow, "Warning",
                        "Please provide any comment!!");
                    return;
                }

                commentData.Comments.SelectedQuery.Remove(commentData.Comments.SelectedQuery.FirstOrDefault(x =>
                    x.Content.QueryValue == "All" && x.Content.QueryType == "All"));

                //CommentModel.LstDisplayManageCommentModel.Add(commentData.Comments);

                var commentList = _commentModel.IsChkAddMultipleComments
                    ? YdStatic.GetListSplittedWithNextLine(commentData.Comments.CommentText)
                    : new List<string> { commentData.Comments.CommentText };

                commentList.ForEach(comment => { AddToCommentList(commentData.Comments, comment); });

                commentData.Comments = new ManageCommentModel
                {
                    LstQueries = CommentModel.ManageCommentModel.LstQueries
                };

                //commentData.Comments.LstQueries.Select(query => { query.IsContentSelected = false; return query; }).ToList();

                CommentModel.ManageCommentModel = commentData.Comments;
                commentData.ComboBoxQueries.ItemsSource = CommentModel.ManageCommentModel.LstQueries;
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
                    if (lstMessage.CommentText.Equals(commentText, StringComparison.CurrentCultureIgnoreCase))
                        isContain = lstMessage.SelectedQuery.Any(x => commentModel.SelectedQuery.Contains(x));
                });

                if (!isContain)
                    CommentModel.LstDisplayManageCommentModel.Add(new ManageCommentModel
                    {
                        CommentText = commentText,
                        SelectedQuery = new ObservableCollection<QueryContent>(commentModel.LstQueries.Where(x =>
                                x.IsContentSelected && x.Content.QueryType != "All" && x.Content.QueryValue != "All")
                            .ToList()),
                        //CommentId = commentModel.CommentId,
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