using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.TumblrQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;

namespace TumblrDominatorCore.ViewModels.Engage
{
    public class CommentViewModel : BindableBase
    {
        private CommentModel _commentModel = new CommentModel();

        public CommentViewModel()
        {
            try
            {
                CommentModel.JobConfiguration = new JobConfiguration
                {
                    ActivitiesPerJobDisplayName =
                        Application.Current.FindResource("LangKeyNumberOfCommentPerJob")?.ToString(),
                    ActivitiesPerHourDisplayName =
                        Application.Current.FindResource("LangKeyNumberOfCommentPerHour")?.ToString(),
                    ActivitiesPerDayDisplayName =
                        Application.Current.FindResource("LangKeyNumberOfCommentPerDay")?.ToString(),
                    ActivitiesPerWeekDisplayName =
                        Application.Current.FindResource("LangKeyNumberOfCommentPerWeek")?.ToString(),
                    IncreaseActivityDisplayName =
                        Application.Current.FindResource("LangKeyNumberOfCommentsPerDay")?.ToString(),
                    RunningTime = RunningTimes.DayWiseRunningTimes,
                    Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
                };

                CommentModel.ListQueryType.Clear();

                Enum.GetValues(typeof(TumblrPostQuery)).Cast<TumblrPostQuery>().ToList().ForEach(query =>
                {
                    CommentModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())
                        ?.ToString());
                });
                CommentModel.ListQueryType.Remove("Custom URL");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
            AddCommentsCommand = new BaseCommand<object>(sender => true, AddComments);
            DeleteMulipleCommand = new BaseCommand<object>(sender => true, DeleteMultiple);
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

        //private void AddQuery(object sender)
        //{
        //    try
        //    {
        //        var moduleSettingsUserControl = sender as ModuleSettingsUserControl<CommentViewModel, CommentModel>;
        //        if (!Model.ManageCommentModel.LstQueries.Any(x =>
        //            moduleSettingsUserControl != null && (x.Content.QueryValue == moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue &&
        //                                                  x.Content.QueryType == moduleSettingsUserControl._queryControl.CurrentQuery.QueryType)))
        //        {
        //            if (moduleSettingsUserControl != null)
        //                Model.ManageCommentModel.LstQueries.Add(new QueryContent
        //                {
        //                    Content = new QueryInfo
        //                    {
        //                        QueryValue = moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue,
        //                        QueryType = moduleSettingsUserControl._queryControl.CurrentQuery.QueryType
        //                    }
        //                });
        //        }
        //        moduleSettingsUserControl?.AddQuery(typeof(TumblrPostQuery));
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //    }
        //}

        private void AddComments(object sender)
        {
            var messageData = sender as CommentControl;

            if (messageData == null) return;

            messageData.Comments.SelectedQuery =
                new ObservableCollection<QueryContent>(messageData.Comments.LstQueries.Where(x => x.IsContentSelected));
            // messageData.Messages.MessageId = ObjViewModel.BroadcastMessagesModel.LstDisplayManageMessageModel.Count + 1;
            messageData.Comments.SelectedQuery.Remove(
                messageData.Comments.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));

            if (!string.IsNullOrEmpty(messageData.Comments.CommentText) && messageData.Comments.SelectedQuery.Any())
                Model.LstDisplayManageCommentModel.Add(messageData.Comments);

            messageData.Comments = new ManageCommentModel
            {
                LstQueries = Model.ManageCommentModel.LstQueries
            };

            messageData.Comments.LstQueries.Select(query =>
            {
                query.IsContentSelected = false;
                return query;
            }).ToList();

            Model.ManageCommentModel = messageData.Comments;
            messageData.ComboBoxQueries.ItemsSource = Model.ManageCommentModel.LstQueries;
        }

        //public void AddQuery(string keyResource)
        //{
        //    try
        //    {
        //        string queryValue = keyResource.Equals("All") ? "All" : Application.Current.FindResource(keyResource).ToString();
        //        if (BroadcastMessagesModel.ManageMessagesModel.LstQueries.All(x => x.Content.QueryValue != queryValue))
        //            BroadcastMessagesModel.ManageMessagesModel.LstQueries.Add(new QueryContent
        //            {
        //                Content = new QueryInfo
        //                {
        //                    QueryValue = queryValue
        //                }
        //            });
        //    }
        //    catch (Exception exception)
        //    {
        //        exception.DebugLog();
        //    }
        //}


        //private void DeleteQuery(object sender)
        //{
        //    try
        //    {
        //        var currentQuery = sender as QueryInfo;

        //        if (Model.SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
        //            Model.SavedQueries.Remove(currentQuery);
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //    }

        //}

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
                moduleSettingsUserControl.AddQuery(typeof(TumblrPostQuery));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

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
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        //private void DeleteMuliple(object sender)
        //{
        //    var selectedQuery = Model.SavedQueries.Where(x => x.IsQuerySelected).ToList();
        //    try
        //    {
        //        foreach (var currentQuery in selectedQuery)
        //        {
        //            try
        //            {
        //                if (Model.SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
        //                    Model.SavedQueries.Remove(currentQuery);
        //            }
        //            catch (Exception ex)
        //            {
        //                ex.DebugLog();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //    }

        //}

        #endregion
    }
}