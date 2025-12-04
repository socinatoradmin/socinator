using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.LdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.LDModel.Engage;

namespace LinkedDominatorCore.LDViewModel.Engage
{
    public class CommentViewModel : BindableBase
    {
        private CommentModel _commentModel = new CommentModel();

        public CommentViewModel()
        {
            CommentModel.ListQueryType.Clear();
            Enum.GetValues(typeof(LDEngageQueryParameters)).Cast<LDEngageQueryParameters>().ForEach(QueryType =>
            {
                if (QueryType == LDEngageQueryParameters.Keyword ||
                    QueryType == LDEngageQueryParameters.SomeonesPosts ||
                    QueryType == LDEngageQueryParameters.MyConnectionsPosts ||
                    QueryType == LDEngageQueryParameters.GroupsUrlPosts||
                    QueryType == LDEngageQueryParameters.CustomPosts ||
                    QueryType == LDEngageQueryParameters.HashtagUrlPost)
                    CommentModel.ListQueryType.Add(QueryType.GetDescriptionAttr().FromResourceDictionary());
            });

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
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyMaxCommentPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            if (CommentModel.ManageCommentModel.LstQueries.All(x => x.Content.QueryValue != "All"))
                CommentModel.ManageCommentModel.LstQueries.Add(new QueryContent
                {
                    Content = new QueryInfo
                    {
                        QueryValue = "All",
                        QueryType = "All"
                    }
                });
            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
            DeleteMulipleCommand = new BaseCommand<object>(sender => true, DeleteMuliple);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            AddCommentsCommand = new BaseCommand<object>(sender => true, AddComments);
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

        public void AddQuery(object sender)
        {
            try
            {
                var ModuleSettingsUserControl = sender as ModuleSettingsUserControl<CommentViewModel, CommentModel>;
                if (!CommentModel.ManageCommentModel.LstQueries.Any(x =>
                    x.Content.QueryValue == ModuleSettingsUserControl._queryControl.CurrentQuery.QueryValue &&
                    x.Content.QueryType == ModuleSettingsUserControl._queryControl.CurrentQuery.QueryType))
                    CommentModel.ManageCommentModel.LstQueries.Add(new QueryContent
                    {
                        Content = new QueryInfo
                        {
                            QueryValue = ModuleSettingsUserControl._queryControl.CurrentQuery.QueryValue,
                            QueryType = ModuleSettingsUserControl._queryControl.CurrentQuery.QueryType
                        }
                    });
                ModuleSettingsUserControl.AddQuery(typeof(LDGrowConnectionUserQueryParameters));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void DeleteQuery(object sender)
        {
            try
            {
                var currentQuery = sender as QueryInfo;
                if (Model.SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                {
                    Model.SavedQueries.Remove(currentQuery);
                    var removeData = CommentModel.ManageCommentModel.LstQueries.SingleOrDefault(x =>
                        x.Content.QueryType == currentQuery.QueryType &&
                        x.Content.QueryValue == currentQuery.QueryValue);
                    CommentModel.ManageCommentModel.LstQueries.Remove(removeData);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        public void DeleteMuliple(object sender)
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
        private void AddComments(object sender)
        {
            try
            {
                var commentData = sender as CommentControl;

                if (commentData.Comments.LstQueries.Where(x => x.IsContentSelected).ToList().Count == 0)
                {
                    Dialog.ShowDialog("Warning", "Please Select Atleast one Query type!!");
                    return;
                }

                if (Model.IsChkMultilineComment)
                {
                    AddToCommentList(commentData.Comments, commentData.Comments.CommentText);
                }
                else
                {
                    var CommentList = commentData.Comments.CommentText.Split('\n').ToList();
                    CommentList = CommentList.Where(x => !string.IsNullOrEmpty(x.Trim())).Select(y => y.Trim()).ToList();
                    CommentList.ForEach(comment => { AddToCommentList(commentData.Comments, comment); });
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

        private void AddToCommentList(ManageCommentModel commentModel, string CommentText)
        {
            try
            {
                var isContain = false;
                CommentModel.LstDisplayManageCommentModel.ForEach(lstMessage =>
                {
                    if (lstMessage.CommentText.ToLower().Equals(CommentText.ToLower()))
                        isContain = lstMessage.SelectedQuery.Any(x => commentModel.SelectedQuery.Contains(x));
                });

                if (!isContain)
                    CommentModel.LstDisplayManageCommentModel.Add(new ManageCommentModel
                    {
                        CommentText = CommentText,
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

        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand DeleteMulipleCommand { get; set; }
        public ICommand AddCommentsCommand { get; set; }

        #endregion
    }
}