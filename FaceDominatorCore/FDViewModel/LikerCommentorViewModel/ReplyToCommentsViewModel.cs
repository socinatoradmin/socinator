using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDModel.LikerCommentorModel;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace FaceDominatorCore.FDViewModel.LikerCommentorViewModel
{

    public class ReplyToCommentsViewModel : BindableBase
    {
        public ReplyToCommentsViewModel()
        {

            Model.ManageCommentsModel.LstQueries.Add(new QueryContent { Content = new QueryInfo() { QueryType = "All", QueryValue = "All" } });


            ReplyToCommentsModel.ListQueryType.Clear();

            Enum.GetValues(typeof(CommentLikerParameter)).Cast<CommentLikerParameter>().ToList().ForEach(query =>
            {
                ReplyToCommentsModel.ListQueryType.Add(item: Application.Current.FindResource(query.GetDescriptionAttr())?.ToString());
            });

            ReplyToCommentsModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current.FindResource("LangKeyNumberOfReplyPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current.FindResource("LangKeyNumberOfReplyPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current.FindResource("LangKeyNumberOfReplyPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current.FindResource("LangKeyNumberOfReplyPerWeek")?.ToString(),
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyMaxCommentPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            AddQueryCommand = new BaseCommand<object>((sender) => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>((sender) => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>((sender) => true, DeleteQuery);
            AddCommentsCommand = new BaseCommand<object>((sender) => true, AddComments);
            DeleteMulipleCommand = new BaseCommand<object>((sender) => true, DeleteMuliple);
        }

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
                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<ReplyToCommentsViewModel, ReplyToCommentModel>;
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

                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<ReplyToCommentsViewModel, ReplyToCommentModel>;

                if (moduleSettingsUserControl != null)
                {
                    var listQuery = moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue.Split(',').Where(x => !string.IsNullOrEmpty(x.Trim())).Distinct();

                    listQuery.ForEach(z =>
                    {
                        moduleSettingsUserControl?.SetQueryTypeEnumName(Enum.GetNames(typeof(CommentLikerParameter)), moduleSettingsUserControl._queryControl.CurrentQuery);

                        if (!Model.ManageCommentsModel.LstQueries.Any(x =>
                            x.Content.QueryValue == z &&
                            x.Content.QueryType == moduleSettingsUserControl._queryControl.CurrentQuery.QueryType))
                        {

                            Model.ManageCommentsModel.LstQueries.Add(new QueryContent
                            {
                                Content = new QueryInfo
                                {
                                    QueryValue = z,
                                    QueryType = moduleSettingsUserControl._queryControl.CurrentQuery.QueryType,
                                    QueryTypeEnum = moduleSettingsUserControl._queryControl.CurrentQuery.QueryTypeEnum
                                }
                            });
                        }
                    });
                }

                if (moduleSettingsUserControl != null && (string.IsNullOrEmpty(moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue) &&
                                                          moduleSettingsUserControl._queryControl.QueryCollection.Count != 0))
                {
                    moduleSettingsUserControl._queryControl.QueryCollection.ForEach(query =>
                    {
                        var currentQuery = moduleSettingsUserControl._queryControl.CurrentQuery.Clone() as QueryInfo;

                        if (currentQuery == null) return;

                        currentQuery.QueryTypeDisplayName = currentQuery.QueryType;

                        if (!Model.ManageCommentsModel.LstQueries.Any(x =>
                                x.Content.QueryValue == query &&
                                    x.Content.QueryType == currentQuery.QueryTypeDisplayName))
                        {


                            Model.ManageCommentsModel.LstQueries.Add(new QueryContent
                            {
                                Content = new QueryInfo
                                {
                                    QueryValue = query,
                                    QueryType = currentQuery.QueryTypeDisplayName,
                                    QueryTypeEnum = currentQuery.QueryTypeEnum
                                }
                            });
                        }
                    });
                }

                moduleSettingsUserControl?.AddQuery(typeof(CommentLikerParameter));
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

                var queryToDelete = Model.ManageCommentsModel.LstQueries.FirstOrDefault(x =>
                    currentQuery != null && (x.Content.QueryValue == currentQuery.QueryValue
                                             && x.Content.QueryType == currentQuery.QueryType));


                if (Model.SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                    Model.SavedQueries.Remove(currentQuery);


                Model.ManageCommentsModel.LstQueries.Remove(queryToDelete);
                foreach (var message in Model.LstManageCommentModel.ToList())
                {
                    var selectedQuery = message.SelectedQuery.FirstOrDefault(x => queryToDelete != null && x.Content.Id == queryToDelete.Content.Id);
                    message.SelectedQuery.Remove(selectedQuery);
                    if (message.SelectedQuery.Count == 0)
                        Model.LstManageCommentModel.Remove(message);
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        private void AddComments(object sender)
        {
            var commentData = sender as CommentControl;

            if (commentData != null && commentData.Comments.CommentText == null) return;

            if (commentData != null)
            {
                commentData.Comments.SelectedQuery =
                    new ObservableCollection<QueryContent>(
                        commentData.Comments.LstQueries.Where(x => x.IsContentSelected));

                if (commentData.Comments.SelectedQuery.Count == 0)
                {
                    Dialog.ShowDialog(Application.Current.MainWindow, "Warning",
                        "Please select atleast one query!!");
                    return;
                }

                if (string.IsNullOrEmpty(commentData.Comments.CommentText))
                {
                    Dialog.ShowDialog(Application.Current.MainWindow, "Warning",
                        "Please enter message text!!");
                    return;
                }


                commentData.Comments.SelectedQuery.Remove(
                    commentData.Comments.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));

                Model.LstManageCommentModel.Add(commentData.Comments);

                commentData.Comments = new ManageCommentModel
                {
                    LstQueries = Model.ManageCommentsModel.LstQueries
                };
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                commentData.Comments.LstQueries.Select(query =>
                {
                    query.IsContentSelected = false;
                    return query;
                }).ToList();

                Model.ManageCommentsModel = commentData.Comments;

                commentData.ComboBoxQueries.ItemsSource = Model.ManageCommentsModel.LstQueries;
            }
        }

        private void DeleteMuliple(object sender)
        {
            var selectedQuery = Model.SavedQueries.Where(x => x.IsQuerySelected).ToList();
            try
            {
                foreach (var currentQuery in selectedQuery)
                {
                    try
                    {

                        var queryToDelete = Model.ManageCommentsModel.LstQueries.FirstOrDefault(x =>
                            x.Content.QueryValue == currentQuery.QueryValue
                            && x.Content.QueryType == currentQuery.QueryType);


                        if (Model.SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                            Model.SavedQueries.Remove(currentQuery);


                        Model.ManageCommentsModel.LstQueries.Remove(queryToDelete);
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
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion

        public ReplyToCommentModel Model => ReplyToCommentsModel;

        private ReplyToCommentModel _replyToCommentsModel = new ReplyToCommentModel();

        public ReplyToCommentModel ReplyToCommentsModel
        {
            get
            {
                return _replyToCommentsModel;
            }
            set
            {
                if (_replyToCommentsModel == null & _replyToCommentsModel == value)
                    return;
                SetProperty(ref _replyToCommentsModel, value);
            }
        }
    }
}
