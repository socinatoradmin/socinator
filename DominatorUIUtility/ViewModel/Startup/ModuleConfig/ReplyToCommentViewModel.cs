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
    public interface IReplyToCommentViewModel
    {
    }

    public class ReplyToCommentViewModel : StartupBaseViewModel, IReplyToCommentViewModel
    {
        public bool _isActionasOwnAccountChecked = true;

        public bool _isActionasPageChecked;

        public ReplyToCommentViewModel(IRegionManager region) : base(region)
        {
            AddQueryCommentCommand = new DelegateCommand<object>(AddQueryComments);
            DeleteQueryCommand = new DelegateCommand<object>(DeleteQuery);
            AddCommentsCommand = new DelegateCommand<object>(AddComments);
            DeleteMulipleCommand = new DelegateCommand<object>(DeleteMuliple);

            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.ReplyToComment});
            NextCommand = new DelegateCommand(ReplyToCommentValidation);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);

            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfReplyPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfReplyPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfReplyPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfReplyPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxReplyPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }

        public ICommand DeleteQueryCommand { get; set; }
        public ICommand AddCommentsCommand { get; set; }
        public ICommand DeleteMulipleCommand { get; set; }
        public ICommand AddQueryCommentCommand { get; set; }

        public bool IsActionasPageChecked
        {
            get => _isActionasPageChecked;
            set
            {
                if (value == _isActionasPageChecked)
                    return;
                SetProperty(ref _isActionasPageChecked, value);
            }
        }

        public bool IsActionasOwnAccountChecked
        {
            get => _isActionasOwnAccountChecked;
            set
            {
                if (value == _isActionasOwnAccountChecked)
                    return;
                SetProperty(ref _isActionasOwnAccountChecked, value);
            }
        }

        public string OwnPageUrl { get; set; }

        public List<string> ListOwnPageUrl { get; set; } = new List<string>();

        public ObservableCollection<ManageCommentModel> LstManageCommentModel { get; set; } =
            new ObservableCollection<ManageCommentModel>();

        public ManageCommentModel ManageCommentsModel { get; set; } = new ManageCommentModel();

        private void ReplyToCommentValidation()
        {
            if (!IsActionasPageChecked && !IsActionasOwnAccountChecked)
            {
                Dialog.ShowDialog("Warning", "Please Select Reaction Type");
                return;
            }

            if (IsActionasPageChecked && ListOwnPageUrl.Count == 0)
            {
                Dialog.ShowDialog("Warning", "Please Select PageUrls");
                return;
            }

            if (LstManageCommentModel.Count == 0)
            {
                Dialog.ShowDialog("Warning", "Please Add Atleast one comment");
                return;
            }

            NavigateNext();
        }

        private void DeleteQuery(object sender)
        {
            try
            {
                var currentQuery = sender as QueryInfo;

                var queryToDelete = ManageCommentsModel.LstQueries.FirstOrDefault(x =>
                    currentQuery != null && x.Content.QueryValue == currentQuery.QueryValue &&
                    x.Content.QueryType == currentQuery.QueryType);


                if (SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                    SavedQueries.Remove(currentQuery);


                ManageCommentsModel.LstQueries.Remove(queryToDelete);
                foreach (var message in LstManageCommentModel.ToList())
                {
                    var selectedQuery = message.SelectedQuery.FirstOrDefault(x =>
                        queryToDelete != null && x.Content.Id == queryToDelete.Content.Id);
                    message.SelectedQuery.Remove(selectedQuery);
                    if (message.SelectedQuery.Count == 0)
                        LstManageCommentModel.Remove(message);
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
                    Dialog.ShowDialog("Warning",
                        "Please select atleast one query!!");
                    return;
                }

                if (string.IsNullOrEmpty(commentData.Comments.CommentText))
                {
                    Dialog.ShowDialog("Warning",
                        "Please enter message text!!");
                    return;
                }


                commentData.Comments.SelectedQuery.Remove(
                    commentData.Comments.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));

                LstManageCommentModel.Add(commentData.Comments);

                commentData.Comments = new ManageCommentModel
                {
                    LstQueries = ManageCommentsModel.LstQueries
                };
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                commentData.Comments.LstQueries.Select(query =>
                {
                    query.IsContentSelected = false;
                    return query;
                }).ToList();

                ManageCommentsModel = commentData.Comments;

                commentData.ComboBoxQueries.ItemsSource = ManageCommentsModel.LstQueries;
            }
        }

        private void DeleteMuliple(object sender)
        {
            var selectedQuery = SavedQueries.Where(x => x.IsQuerySelected).ToList();
            try
            {
                foreach (var currentQuery in selectedQuery)
                    try
                    {
                        var queryToDelete = ManageCommentsModel.LstQueries.FirstOrDefault(x =>
                            x.Content.QueryValue == currentQuery.QueryValue
                            && x.Content.QueryType == currentQuery.QueryType);


                        if (SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                            SavedQueries.Remove(currentQuery);


                        ManageCommentsModel.LstQueries.Remove(queryToDelete);
                        foreach (var message in LstManageCommentModel.ToList())
                        {
                            message.SelectedQuery.Remove(queryToDelete);
                            if (message.SelectedQuery.Count == 0)
                                LstManageCommentModel.Remove(message);
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

        private void AddQueryComments(object sender)
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
                        if (ManageCommentsModel.LstQueries.Any(x =>
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
                            ManageCommentsModel.LstQueries.Add(addNew);
                            LstManageCommentModel.ForEach(x =>
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
                        if (ManageCommentsModel.LstQueries.Any(x =>
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
                            ManageCommentsModel.LstQueries.Add(addNew);

                            LstManageCommentModel.ForEach(x =>
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
            if (ManageCommentsModel.LstQueries.Count > 1 &&
                !ManageCommentsModel.LstQueries.Any(x =>
                    x.Content.QueryValue == "All" && x.Content.QueryType == "All"))
                ManageCommentsModel.LstQueries.Insert(0, new QueryContent
                {
                    Content = new QueryInfo
                    {
                        QueryType = "All",
                        QueryValue = "All"
                    }
                });
        }
    }
}