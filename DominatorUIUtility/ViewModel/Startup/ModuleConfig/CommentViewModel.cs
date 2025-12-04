using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.Views.AccountSetting.CustomControl;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface ICommentViewModel
    {
    }

    public class CommentViewModel : StartupBaseViewModel, ICommentViewModel
    {
        public CommentViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.Comment});
            NextCommand = new DelegateCommand(ValidateAndNavigate);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            AddQueryToCommentCommand = new DelegateCommand<object>(AddQueryToComment);
            AddCommentCommand = new DelegateCommand<object>(AddComment);
            DeleteQueryCommand = new DelegateCommand<object>(DeleteQuery);
            DeleteMultipleCommand = new DelegateCommand(DeleteMultiple);

            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfCommentsPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfCommentsPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfCommentsPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfCommentsPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxCommentPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }

        #region Command

        public ICommand AddQueryToCommentCommand { get; set; }
        public ICommand AddCommentCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand DeleteMultipleCommand { get; set; }

        #endregion

        #region Property

        private int _isSpintax;

        public int IsSpintax
        {
            get => _isSpintax;
            set => SetProperty(ref _isSpintax, value);
        }

        public ObservableCollection<ManageCommentModel> LstDisplayManageCommentModel { get; set; } =
            new ObservableCollection<ManageCommentModel>();

        public ManageCommentModel ManageCommentModel { get; set; } = new ManageCommentModel();

        #endregion

        #region Methods

        private void ValidateAndNavigate()
        {
            if (LstDisplayManageCommentModel.Count == 0)
            {
                Dialog.ShowDialog("Error", "Please add at least one Comment.");
                return;
            }

            NavigateNext();
        }

        private void AddQueryToComment(object sender)
        {
            try
            {
                var activitySetting = sender as ActivitySettingWithoutButton;

                if (activitySetting == null ||
                    string.IsNullOrEmpty(activitySetting.QueryControl.CurrentQuery.QueryValue.Trim()) &&
                    !activitySetting.QueryControl.QueryCollection.Any())
                    return;

                var splittedQueries = activitySetting.QueryControl.CurrentQuery.QueryValue.Contains(",")
                    ? activitySetting.QueryControl.CurrentQuery.QueryValue.Split(',')
                        .Where(x => !string.IsNullOrEmpty(x.Trim())).ToList()
                    : new List<string> {activitySetting.QueryControl.CurrentQuery.QueryValue};

                if (string.IsNullOrEmpty(activitySetting.QueryControl.CurrentQuery.QueryValue) &&
                    activitySetting.QueryControl.QueryCollection.Count != 0)
                    foreach (var queryValue in activitySetting.QueryControl.QueryCollection)
                    {
                        if (ManageCommentModel.LstQueries.Any(x =>
                            x.Content.QueryValue == queryValue &&
                            x.Content.QueryType == activitySetting.QueryControl.CurrentQuery.QueryType))
                            continue;
                        {
                            var addNew = new QueryContent
                            {
                                Content = new QueryInfo
                                {
                                    QueryValue = queryValue,
                                    QueryType = activitySetting.QueryControl.CurrentQuery.QueryType
                                }
                            };
                            ManageCommentModel.LstQueries.Add(addNew);
                            LstDisplayManageCommentModel.ForEach(x =>
                            {
                                if (!x.LstQueries.Any(y =>
                                    addNew.Content.QueryType == activitySetting.QueryControl.CurrentQuery.QueryType &&
                                    y.Content.QueryValue == addNew.Content.QueryValue))
                                    x.LstQueries.Add(addNew);
                            });
                        }
                    }
                else
                    foreach (var queryValue in splittedQueries)
                    {
                        if (ManageCommentModel.LstQueries.Any(x =>
                            x.Content.QueryType == activitySetting.QueryControl.CurrentQuery.QueryType &&
                            x.Content.QueryValue == queryValue)) continue;
                        {
                            var addNew = new QueryContent
                            {
                                Content = new QueryInfo
                                {
                                    QueryValue = queryValue,
                                    QueryType = activitySetting.QueryControl.CurrentQuery.QueryType
                                }
                            };
                            ManageCommentModel.LstQueries.Add(addNew);

                            LstDisplayManageCommentModel.ForEach(x =>
                            {
                                if (!x.LstQueries.Any(y =>
                                    addNew.Content.QueryType == activitySetting.QueryControl.CurrentQuery.QueryType &&
                                    y.Content.QueryValue == addNew.Content.QueryValue))
                                    x.LstQueries.Add(addNew);
                            });
                        }
                    }

                AddQueryAll();

                AddQueryCommand.Execute(sender);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AddQueryAll()
        {
            if (ManageCommentModel.LstQueries.Count > 1 &&
                !ManageCommentModel.LstQueries.Any(x =>
                    x.Content.QueryValue == "All" && x.Content.QueryType == "All"))
                ManageCommentModel.LstQueries.Insert(0, new QueryContent
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

                var queryToDelete = ManageCommentModel.LstQueries.FirstOrDefault(x =>
                    currentQuery != null && x.Content.QueryValue == currentQuery.QueryValue &&
                    x.Content.QueryType == currentQuery.QueryType);

                if (SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                    SavedQueries.Remove(currentQuery);

                ManageCommentModel.LstQueries.Remove(queryToDelete);
                foreach (var message in LstDisplayManageCommentModel.ToList())
                {
                    var queryDelete = message.SelectedQuery.FirstOrDefault(x =>
                        currentQuery != null && x.Content.QueryType == currentQuery.QueryType &&
                        x.Content.QueryValue == currentQuery.QueryValue);
                    message.SelectedQuery.Remove(queryDelete);

                    if (message.SelectedQuery.Count == 0)
                        LstDisplayManageCommentModel.Remove(message);
                }

                if (!ManageCommentModel.LstQueries.Skip(1).Any())
                    ManageCommentModel.LstQueries[0].IsContentSelected = false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DeleteMultiple()
        {
            var selectedQuery = SavedQueries.Where(x => x.IsQuerySelected).ToList();
            try
            {
                foreach (var currentQuery in selectedQuery)
                    try
                    {
                        var queryToDelete = ManageCommentModel.LstQueries.FirstOrDefault(x =>
                            x.Content.QueryValue == currentQuery.QueryValue
                            && x.Content.QueryType == currentQuery.QueryType);

                        if (SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                            SavedQueries.Remove(currentQuery);

                        ManageCommentModel.LstQueries.Remove(queryToDelete);
                        foreach (var message in LstDisplayManageCommentModel.ToList())
                        {
                            message.SelectedQuery.Remove(queryToDelete);
                            if (message.SelectedQuery.Count == 0)
                                LstDisplayManageCommentModel.Remove(message);
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

        private void AddComment(object sender)
        {
            try
            {
                var commentData = sender as CommentControl;
                if (commentData == null) return;

                commentData.Comments.SelectedQuery =
                    new ObservableCollection<QueryContent>(
                        commentData.Comments.LstQueries.Where(x => x.IsContentSelected));

                if (commentData.Comments.SelectedQuery.Count == 0)
                {
                    Dialog.ShowDialog("Warning", "Please select atleast one query!!");
                    return;
                }

                if (commentData.Comments.SelectedQuery.Count == 1 &&
                    commentData.Comments.SelectedQuery.FirstOrDefault()?.Content.QueryValue == "All")
                {
                    Dialog.ShowDialog("Warning", "Please add atleast one query!!");
                    return;
                }

                if (string.IsNullOrEmpty(commentData.Comments.CommentText))
                {
                    Dialog.ShowDialog("Warning", "Please provide any comment!!");
                    return;
                }

                commentData.Comments.SelectedQuery.Remove(commentData.Comments.SelectedQuery.FirstOrDefault(x =>
                    x.Content.QueryValue == "All" && x.Content.QueryType == "All"));

                AddToCommentList(commentData.Comments, commentData.Comments.CommentText);
                commentData.Comments = new ManageCommentModel
                {
                    LstQueries = ManageCommentModel.LstQueries
                };

                ManageCommentModel = commentData.Comments;
                commentData.ComboBoxQueries.ItemsSource = ManageCommentModel.LstQueries;
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
                LstDisplayManageCommentModel.ForEach(lstMessage =>
                {
                    if (lstMessage.CommentText.Equals(commentText, StringComparison.CurrentCultureIgnoreCase))
                        isContain = lstMessage.SelectedQuery.Any(x => commentModel.SelectedQuery.Contains(x));
                });

                if (!isContain)
                    LstDisplayManageCommentModel.Add(new ManageCommentModel
                    {
                        CommentText = commentText,
                        SelectedQuery = new ObservableCollection<QueryContent>(commentModel.LstQueries.Where(x =>
                                x.IsContentSelected && x.Content.QueryType != "All" && x.Content.QueryValue != "All")
                            .ToList()),
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