using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.QdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using Microsoft.Win32;
using QuoraDominatorCore.Models;

namespace QuoraDominatorCore.ViewModel.Scrape
{
    public class AnswerQuestionViewModel : BindableBase
    {
        public AnswerQuestionViewModel()
        {
            try
            {
                AnswerQuestionModel.JobConfiguration = new JobConfiguration
                {
                    ActivitiesPerJobDisplayName =
                        Application.Current.FindResource("LangKeyAnswerScrapePerJob")?.ToString(),
                    ActivitiesPerHourDisplayName =
                        Application.Current.FindResource("LangKeyAnswerScrapePerHour")?.ToString(),
                    ActivitiesPerDayDisplayName =
                        Application.Current.FindResource("LangKeyAnswerScrapePerDay")?.ToString(),
                    ActivitiesPerWeekDisplayName =
                        Application.Current.FindResource("LangKeyAnswerScrapePerWeek")?.ToString(),
                    IncreaseActivityDisplayName =
                        Application.Current.FindResource("LangKeyMaxAnswerScrapePerDay")?.ToString(),
                    RunningTime = RunningTimes.DayWiseRunningTimes,
                    Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
                };

                AnswerQuestionModel.ListQueryType.Clear();
                AnswerQuestionModel.TopicFilter.IsVisibleAnswerFilter = Visibility.Collapsed;
                Enum.GetValues(typeof(QuestionQueryParameters)).Cast<QuestionQueryParameters>().ToList().ForEach(
                    query =>
                    {
                        AnswerQuestionModel.ListQueryType.Add(Application.Current
                            .FindResource(query.GetDescriptionAttr())?.ToString());
                    });
                AnswerQuestionModel.ListQueryType.Remove("Custom URL");
                AnswerQuestionModel.ManageCommentModel.LstQueries.Add(new QueryContent
                {
                    Content = new QueryInfo
                    {
                        QueryType = "All",
                        QueryValue = "All"
                    }
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            AddAnswerCommand = new BaseCommand<object>(sender => true, AddAnswer);
            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
            DeleteMultipleCommand = new BaseCommand<object>(sender => true, DeleteMultiple);
            AddMediaCommand = new BaseCommand<object>(sender => true, AddMediaExecute);
            DeleteMediaCommand = new BaseCommand<object>(sender => true, DeleteMediaExecute);
        }
        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand AddAnswerCommand { get; set; }
        public ICommand DeleteMultipleCommand { get; set; }
        public ICommand AddMediaCommand { get; set; }
        public ICommand DeleteMediaCommand { get; set; }

        #endregion

        #region Methods


        private void AddMediaExecute(object sender)
        {
            try
            {
                var messageData = sender as AnswerQuestionViewModel;
                if (messageData == null) return;
                var opf = new OpenFileDialog();
                opf.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF";
                if (opf.ShowDialog().Value) messageData.Model.MediaPath = opf.FileName;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        private void DeleteMediaExecute(object sender)
        {
            try
            {
                var messageData = sender as AnswerQuestionViewModel;
                if (messageData == null) return;
                messageData.Model.MediaPath = string.Empty;
            }
            catch (Exception ex) { ex.DebugLog(); }
        }
        private void AddAnswer(object sender)
        {
            try
            {
                var messageData = sender as CommentControl;

                if (messageData == null) return;

                messageData.Comments.SelectedQuery =
                    new ObservableCollection<QueryContent>(
                        messageData.Comments.LstQueries.Where(x => x.IsContentSelected));
                if (messageData.Comments.SelectedQuery.Count == 0 ||
                    string.IsNullOrEmpty(messageData.Comments.CommentText))
                {
                    Dialog.ShowDialog("Warning", "May be you didn't select any query or answer is missing.");
                    return;
                }

                if (messageData.Comments.SelectedQuery.Count == 1 &&
                    messageData.Comments.SelectedQuery[0].Content.QueryType == "All")
                {
                    Dialog.ShowDialog("Warning", "May be you didn't select any query or answer is missing.");
                    return;
                }

                messageData.Comments.SelectedQuery.Remove(
                    messageData.Comments.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));
                messageData.Comments.MediaPath = AnswerQuestionModel.MediaPath;
                AnswerQuestionModel.LstManageCommentModel.Add(messageData.Comments);

                messageData.Comments = new ManageCommentModel
                {
                    LstQueries = AnswerQuestionModel.ManageCommentModel.LstQueries,
                    MediaPath = AnswerQuestionModel.MediaPath
                };

                messageData.Comments.LstQueries.Select(x =>
                {
                    x.IsContentSelected = false;
                    return x;
                }).ToList();
                AnswerQuestionModel.ManageCommentModel = messageData.Comments;
                messageData.ComboBoxQueries.ItemsSource = AnswerQuestionModel.ManageCommentModel.LstQueries;
                AnswerQuestionModel.MediaPath = string.Empty;
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
                var moduleSettingsUserControl =
                    sender as ModuleSettingsUserControl<AnswerQuestionViewModel, AnswerQuestionModel>;
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
                var moduleSettingsUserControl =
                    sender as ModuleSettingsUserControl<AnswerQuestionViewModel, AnswerQuestionModel>;
                if (!Model.ManageCommentModel.LstQueries.Any(x =>
                    moduleSettingsUserControl != null &&
                    x.Content.QueryValue == moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue &&
                    x.Content.QueryType == moduleSettingsUserControl._queryControl.CurrentQuery.QueryType))
                {
                    if (moduleSettingsUserControl != null &&
                        moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue.Contains(","))
                        moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue.Split(',')
                            .Where(x => !string.IsNullOrEmpty(x.Trim())).Distinct().ForEach(query =>
                            {
                                Model.ManageCommentModel.LstQueries.Add(new QueryContent
                                {
                                    Content = new QueryInfo
                                    {
                                        QueryValue = query,
                                        QueryType = moduleSettingsUserControl._queryControl.CurrentQuery.QueryType
                                    }
                                });
                            });
                    else if (moduleSettingsUserControl != null)
                        Model.ManageCommentModel.LstQueries.Add(new QueryContent
                        {
                            Content = new QueryInfo
                            {
                                QueryValue = moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue,
                                QueryType = moduleSettingsUserControl._queryControl.CurrentQuery.QueryType
                            }
                        });
                }

                if (Model.ManageCommentModel.LstQueries[0].IsContentSelected)
                    Model.ManageCommentModel.LstQueries.Select(x =>
                    {
                        x.IsContentSelected = true;
                        return x;
                    }).ToList();
                moduleSettingsUserControl?.AddQuery(typeof(QuestionQueryParameters));
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

                if (Model.SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                    Model.SavedQueries.Remove(currentQuery);

                Model.ManageCommentModel.LstQueries.Remove(queryToDelete);
                foreach (var message in Model.LstManageCommentModel.ToList())
                {
                    var queryDelete = message.SelectedQuery.FirstOrDefault(x =>
                        currentQuery != null && x.Content.QueryType == currentQuery.QueryType &&
                        x.Content.QueryValue == currentQuery.QueryValue);
                    message.SelectedQuery.Remove(queryDelete);
                    if (message.SelectedQuery.Count == 0)
                        Model.LstManageCommentModel.Remove(message);
                }

                if (!Model.ManageCommentModel.LstQueries.Skip(1).Any())
                    Model.ManageCommentModel.LstQueries[0].IsContentSelected = false;
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
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion

        #region Properties

        private AnswerQuestionModel _AnswerOnQuestionModel = new AnswerQuestionModel();

        public AnswerQuestionModel AnswerQuestionModel
        {
            get => _AnswerOnQuestionModel;
            set
            {
                if ((_AnswerOnQuestionModel == null) & (_AnswerOnQuestionModel == value))
                    return;
                SetProperty(ref _AnswerOnQuestionModel, value);
            }
        }

        public AnswerQuestionModel Model => AnswerQuestionModel;

        #endregion
    }
}