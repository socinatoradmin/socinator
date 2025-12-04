using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.YdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.CustomControl.YoutubeCutomControl;
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
    public class ReportVideoViewModel : BindableBase
    {
        public ReportVideoViewModel()
        {
            ReportVideoModel.ListQueryType.Clear();
            Enum.GetValues(typeof(YdScraperParameters)).Cast<YdScraperParameters>().ToList().ForEach(query =>
            {
                if (query != YdScraperParameters.YTVideoCommenters)
                    ReportVideoModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())
                        .ToString());
            });

            ReportVideoModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyReportToVideosPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyReportToVideosPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyReportToVideosPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyReportToVideosPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaximumLikesPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            AddReportDetailsCommand = new BaseCommand<object>(sender => true, AddReportDetails);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
            DeleteMulipleCommand = new BaseCommand<object>(sender => true, DeleteMultiple);
        }

        public ReportVideoModel Model => ReportVideoModel;

        #region Object creation logic

        private ReportVideoModel _reportVideoModel = new ReportVideoModel();


        public ReportVideoModel ReportVideoModel
        {
            get => _reportVideoModel;
            set
            {
                if (_reportVideoModel == null)
                    return;
                SetProperty(ref _reportVideoModel, value);
            }
        }

        #endregion

        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand AddReportDetailsCommand { get; set; }
        public ICommand DeleteMulipleCommand { get; set; }

        #endregion

        #region Methods

        private void AddQuery(object sender)
        {
            try
            {
                var moduleSettingsUserControl =
                    sender as ModuleSettingsUserControl<ReportVideoViewModel, ReportVideoModel>;

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
                        if (Model.ManageReportDetailsModel.LstQueries.Any(x => x.Content.QueryValue == valueToAdd
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
                            Model.ManageReportDetailsModel.LstQueries.Add(addNew);
                            Model.ListReportDetailsModel.ForEach(x =>
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
                        if (Model.ManageReportDetailsModel.LstQueries.Any(x =>
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
                            Model.ManageReportDetailsModel.LstQueries.Add(addNew);

                            Model.ListReportDetailsModel.ForEach(x =>
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
            if (Model.ManageReportDetailsModel.LstQueries.Count > 1 &&
                !Model.ManageReportDetailsModel.LstQueries.Any(x =>
                    x.Content.QueryValue == "All" && x.Content.QueryType == "All"))
                Model.ManageReportDetailsModel.LstQueries.Insert(0, new QueryContent
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

                var queryToDelete = Model.ManageReportDetailsModel.LstQueries.FirstOrDefault(x =>
                    currentQuery != null && x.Content.QueryValue == currentQuery.QueryValue &&
                    x.Content.QueryType == currentQuery.QueryType);


                if (Model.SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                    Model.SavedQueries.Remove(currentQuery);


                Model.ManageReportDetailsModel.LstQueries.Remove(queryToDelete);
                foreach (var message in Model.ListReportDetailsModel.ToList())
                {
                    message.SelectedQuery.Remove(message.SelectedQuery.GetDeletingQuery(queryToDelete));

                    if (message.SelectedQuery.Count == 0)
                        Model.ListReportDetailsModel.Remove(message);

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
                        var queryToDelete = Model.ManageReportDetailsModel.LstQueries.FirstOrDefault(x =>
                            x.Content.QueryValue == currentQuery.QueryValue
                            && x.Content.QueryType == currentQuery.QueryType);


                        if (Model.SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                            Model.SavedQueries.Remove(currentQuery);


                        Model.ManageReportDetailsModel.LstQueries.Remove(queryToDelete);
                        foreach (var message in Model.ListReportDetailsModel.ToList())
                        {
                            message.SelectedQuery.Remove(message.SelectedQuery.GetDeletingQuery(queryToDelete));

                            if (message.SelectedQuery.Count == 0)
                                Model.ListReportDetailsModel.Remove(message);

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
            if (Model.ManageReportDetailsModel.LstQueries.Count < 3 &&
                Model.ManageReportDetailsModel.LstQueries.Any(x =>
                    x.Content.QueryValue == "All" && x.Content.QueryType == "All"))
                Model.ManageReportDetailsModel.LstQueries.Remove(Model.ManageReportDetailsModel.LstQueries.First(y =>
                    y.Content.QueryValue == "All" && y.Content.QueryType == "All"));
        }

        private void AddReportDetails(object sender)
        {
            try
            {
                var commentData = sender as VideoReportContentControl;
                if (commentData == null) return;
                //commentData.Comments.SerialNo = ObjViewModel.CommentModel.LstDisplayManageCommentModel.Count + 1;

                //Todo : please check below statement
                // commentData.Comments.SelectedQuery.Remove("All");

                commentData.ReportDetails.SelectedQuery =
                    new ObservableCollection<QueryContent>(
                        commentData.ReportDetails.LstQueries.Where(x => x.IsContentSelected));

                if (commentData.ReportDetails.SelectedQuery.Count == 0)
                {
                    Dialog.ShowDialog(Application.Current.MainWindow,
                        "LangKeyWarning".FromResourceDictionary(),
                        "Please select atleast one query!!");
                    return;
                }

                if (commentData.ReportDetails.SelectedQuery.Count == 1 &&
                    commentData.ReportDetails.SelectedQuery.FirstOrDefault()?.Content.QueryValue == "All")
                {
                    Dialog.ShowDialog(Application.Current.MainWindow,
                        "LangKeyWarning".FromResourceDictionary(),
                        "Please add atleast one query!!");
                    return;
                }

                //if (string.IsNullOrEmpty(commentData.ReportDetails.CommentText))
                //{
                //    Dialog.ShowDialog(Application.Current.MainWindow, "LangKeyWarning".FromResourceDictionary(), "Please provide any comment!!");
                //    return;
                //}
                commentData.ReportDetails.SelectedQuery.Remove(
                    commentData.ReportDetails.SelectedQuery.FirstOrDefault(x =>
                        x.Content.QueryValue == "All" && x.Content.QueryType == "All"));

                //CommentModel.LstDisplayManageCommentModel.Add(commentData.Comments);

                var commentList = commentData.SplitTextByNextLine
                    ? YdStatic.GetListSplittedWithNextLine(commentData.ReportDetails.CommentText)
                    : new List<string> { commentData.ReportDetails.CommentText };

                commentList.ForEach(comment => { AddToCommentList(commentData, comment); });

                commentData.ReportDetails = new ManageReportVideosContentModel
                {
                    LstQueries = Model.ManageReportDetailsModel.LstQueries
                };

                //commentData.Comments.LstQueries.Select(query => { query.IsContentSelected = false; return query; }).ToList();

                Model.ManageReportDetailsModel = commentData.ReportDetails;
                commentData.ComboBoxQueries.ItemsSource = Model.ManageReportDetailsModel.LstQueries;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AddToCommentList(VideoReportContentControl commentData, string commentText)
        {
            try
            {
                var commentModel = commentData.ReportDetails;
                commentData.ReportDetails.ReportOption = commentData.CmbReportOption.SelectedIndex;
                commentData.ReportDetails.ReportSubOption = commentData.CmbReportSubOption.SelectedIndex;
                commentData.ReportDetails.ReportText = commentData.CmbReportOption.SelectedItem as string;
                var isContain = false;
                Model.ListReportDetailsModel.ForEach(lstMessage =>
                {
                    if (!string.IsNullOrEmpty(lstMessage.CommentText) &&
                        lstMessage.CommentText.Equals(commentText, StringComparison.CurrentCultureIgnoreCase))
                        isContain = lstMessage.SelectedQuery.Any(x => commentModel.SelectedQuery.Contains(x));
                });

                if (!isContain)
                    Model.ListReportDetailsModel.Add(new ManageReportVideosContentModel
                    {
                        CommentText = commentText,
                        SelectedQuery = new ObservableCollection<QueryContent>(commentModel.LstQueries.Where(x =>
                                x.IsContentSelected && x.Content.QueryType != "All" && x.Content.QueryValue != "All")
                            .ToList()),
                        //CommentId = commentModel.CommentId,
                        LstQueries = commentModel.LstQueries,
                        ReportOption = commentData.CmbReportOption.SelectedIndex,
                        ReportSubOption = commentData.CmbReportSubOption.SelectedIndex,
                        ReportText = commentData.CmbReportOption.SelectedItem as string,
                        IsSpinTax = commentData.ReportDetails.IsSpinTax,
                        VideoTimestampPercentage = commentModel.VideoTimestampPercentage
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