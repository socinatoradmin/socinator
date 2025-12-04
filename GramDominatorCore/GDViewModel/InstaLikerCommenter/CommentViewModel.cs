using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.GdQuery;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDModel;
using GramDominatorCore.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace GramDominatorCore.GDViewModel.InstaLikerCommenter
{
    public class CommentViewModel : BindableBase
    {

        public CommentViewModel()
        {
            Enum.GetValues(typeof(GdPostQuery)).Cast<GdPostQuery>().ToList().ForEach(query =>
            {
                CommentModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())?.ToString());
            });
            
            CommentModel.ManageCommentModel = new ManageCommentModel()
            {
                LstQueries = new ObservableCollection<QueryContent>()
                {
                    new QueryContent()
                    {
                        Content = new QueryInfo
                        {
                            QueryType = "All",
                            QueryValue = "All"
                        }
                    }
                }
            };

            CommentModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current.FindResource("LangKeyNumberOfCommentPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current.FindResource("LangKeyNumberOfCommentPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current.FindResource("LangKeyNumberOfCommentPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current.FindResource("LangKeyNumberOfCommentPerWeek")?.ToString(),
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyMaxCommentPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            AddQueryCommand = new BaseCommand<object>((sender) => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>((sender) => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>((sender) => true, DeleteQuery);
            AddCommentsCommand = new BaseCommand<object>((sender) => true, AddComments);
            DeleteMultipleCommand = new BaseCommand<object>((sender) => true, DeleteMultiple);
        }

        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand AddCommentsCommand { get; set; }
        public ICommand DeleteMultipleCommand { get; set; }

        #endregion

        #region Command Implemented Methods

        private void AddQuery(object sender)
        {
            try
            {
                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<CommentViewModel, CommentModel>;

                if (moduleSettingsUserControl == null || string.IsNullOrEmpty(moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue.Trim()) && !moduleSettingsUserControl._queryControl.QueryCollection.Any())
                    return;

                var splittedQueries = moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue.Contains(",")
                    ? moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue.Split(',').Where(x => !string.IsNullOrEmpty(x.Trim())).ToList()
                    : new List<string> { moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue };

                if (string.IsNullOrEmpty(moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue) && moduleSettingsUserControl._queryControl.QueryCollection.Count != 0)
                {
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
                                    QueryValue = queryValue,
                                    QueryType = moduleSettingsUserControl._queryControl.CurrentQuery.QueryType
                                }
                            };
                            Model.ManageCommentModel.LstQueries.Add(addNew);
                            Model.LstDisplayManageCommentModel.ForEach(x =>
                            {
                                if (!x.LstQueries.Any(y => addNew.Content.QueryType == moduleSettingsUserControl._queryControl.CurrentQuery.QueryType &&
                                                               y.Content.QueryValue == addNew.Content.QueryValue))
                                    x.LstQueries.Add(addNew);
                            });
                        }
                    }
                }
                else
                {
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
                                if (!x.LstQueries.Any(y => addNew.Content.QueryType == moduleSettingsUserControl._queryControl.CurrentQuery.QueryType &&
                                                               y.Content.QueryValue == addNew.Content.QueryValue))
                                    x.LstQueries.Add(addNew);
                            });
                        }
                    }

                }

                moduleSettingsUserControl.AddQuery(typeof(GdPostQuery));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void CustomFilter(object sender)
        {
            try
            {
                var ModuleSettingsUserControl = sender as ModuleSettingsUserControl<CommentViewModel, CommentModel>;
                if (ModuleSettingsUserControl != null) ModuleSettingsUserControl.CustomFilter();
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
                        currentQuery != null && (x.Content.QueryValue == currentQuery.QueryValue
                                                 && x.Content.QueryType == currentQuery.QueryType));


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
                {
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
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }
              
        private void AddComments(object sender)
        {
            var messageData = sender as CommentControl;

            if (messageData == null) return;

            messageData.Comments.SelectedQuery = new ObservableCollection<QueryContent>(messageData.Comments.LstQueries.Where(x => x.IsContentSelected));

            if (messageData.Comments.SelectedQuery.Count == 0)
            {
                GlobusLogHelper.log.Info("Please add query type with message(s)");
                return;
            }
            if (messageData.Comments.LstQueries.Count == 1 && messageData.Comments.LstQueries[0].Content.QueryType == "All")
            {
                GlobusLogHelper.log.Info("Please add query type");
                return;
            }
            messageData.Comments.SelectedQuery.Remove(messageData.Comments.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));
            if (messageData.Comments.CommentText != null)
            {
                List<string> listMessages = new List<string>();
                if (CommentModel.IsChkAddMultipleComments)
                {
                    listMessages = messageData.Comments.CommentText.Split('\n').ToList();
                    listMessages = listMessages.Where(x => !string.IsNullOrEmpty(x.Trim())).Select(y => y.Trim()).ToList();
                    listMessages.ForEach(message =>
                    {
                        AddToList(messageData.Comments, message);
                    });
                }
                else
                    Model.LstDisplayManageCommentModel.Add(messageData.Comments);
            }
            else
            {
                Model.LstDisplayManageCommentModel.Add(messageData.Comments);
            }

            messageData.Comments = new ManageCommentModel
            {
                LstQueries = Model.ManageCommentModel.LstQueries
            };
            messageData.Comments.LstQueries.Select(query => { query.IsContentSelected = false; return query; }).ToList();

            Model.ManageCommentModel = messageData.Comments;
            messageData.ComboBoxQueries.ItemsSource = Model.ManageCommentModel.LstQueries;
        }

        private void AddToList(ManageCommentModel commentModel, string CommentText)
        {
            try
            {
                CommentModel.LstDisplayManageCommentModel.Add(new ManageCommentModel()
                {
                    CommentText = CommentText, SelectedQuery = commentModel.SelectedQuery,
                    CommentId = commentModel.CommentId, FilterText = commentModel.FilterText,
                    LstQueries = commentModel.LstQueries
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    
        #endregion
        private CommentModel _commentModel = new CommentModel();

        public CommentModel CommentModel
        {
            get
            {
                return _commentModel;
            }
            set
            {
                if (_commentModel == null & _commentModel == value)
                    return;
                SetProperty(ref _commentModel, value);
            }
        }

        public CommentModel Model => CommentModel;

    }
}
