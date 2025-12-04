using System;
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
    public class AnswerQuestionModel : BindableBase
    {
        private ObservableCollection<ManageCommentModel> _lstManageCommentModel =
            new ObservableCollection<ManageCommentModel>();

        private ManageCommentModel _manageCommentModel = new ManageCommentModel();

        public ManageCommentModel ManageCommentModel
        {
            get => _manageCommentModel;
            set => SetProperty(ref _manageCommentModel, value);
        }

        public ObservableCollection<ManageCommentModel> LstManageCommentModel
        {
            get => _lstManageCommentModel;
            set => SetProperty(ref _lstManageCommentModel, value);
        }
    }

    public interface IAnswerOnQuestionsViewModel
    {
    }

    public class AnswerOnQuestionsViewModel : StartupBaseViewModel, IAnswerOnQuestionsViewModel
    {
        private AnswerQuestionModel _answerQuestionModel = new AnswerQuestionModel();

        private ObservableCollection<ManageCommentModel> _lstManageCommentModel =
            new ObservableCollection<ManageCommentModel>();

        public AnswerOnQuestionsViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.AnswerOnQuestions});
            NextCommand = new DelegateCommand(NavigateNext);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            AddAnswerCommand = new DelegateCommand<object>(AddAnswer);
            AddQueryToMsgCommand = new DelegateCommand<object>(AddQuery);
            DeleteQueryCommand = new DelegateCommand<object>(DeleteQuery);
            DeleteMultipleCommand = new DelegateCommand(DeleteMultiple);


            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyAnswerScrapePerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyAnswerScrapePerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyAnswerScrapePerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyAnswerScrapePerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxAnswerScrapePerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }

        public AnswerQuestionModel AnswerQuestionModel
        {
            get => _answerQuestionModel;
            set
            {
                if ((_answerQuestionModel == null) & (_answerQuestionModel == value))
                    return;
                SetProperty(ref _answerQuestionModel, value);
            }
        }

        public ObservableCollection<ManageCommentModel> LstManageCommentModel
        {
            get => _lstManageCommentModel;
            set => SetProperty(ref _lstManageCommentModel, value);
        }

        #region Commands

        public ICommand AddQueryToMsgCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand AddAnswerCommand { get; set; }
        public ICommand DeleteMultipleCommand { get; set; }

        #endregion

        #region Methods

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

                AnswerQuestionModel.LstManageCommentModel.Add(messageData.Comments);

                LstManageCommentModel.Add(messageData.Comments);

                messageData.Comments = new ManageCommentModel
                {
                    LstQueries = AnswerQuestionModel.ManageCommentModel.LstQueries
                };

                messageData.Comments.LstQueries.Select(x =>
                {
                    x.IsContentSelected = false;
                    return x;
                }).ToList();
                AnswerQuestionModel.ManageCommentModel = messageData.Comments;
                messageData.ComboBoxQueries.ItemsSource = AnswerQuestionModel.ManageCommentModel.LstQueries;
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
                var moduleSettingsUserControl = sender as ActivitySettingWithoutButton;
                if (!AnswerQuestionModel.ManageCommentModel.LstQueries.Any(x =>
                    moduleSettingsUserControl != null &&
                    x.Content.QueryValue == moduleSettingsUserControl.QueryControl.CurrentQuery.QueryValue &&
                    x.Content.QueryType == moduleSettingsUserControl.QueryControl.CurrentQuery.QueryType))
                {
                    if (moduleSettingsUserControl != null &&
                        moduleSettingsUserControl.QueryControl.CurrentQuery.QueryValue.Contains(","))
                        moduleSettingsUserControl.QueryControl.CurrentQuery.QueryValue.Split(',')
                            .Where(x => !string.IsNullOrEmpty(x.Trim())).Distinct().ForEach(query =>
                            {
                                AnswerQuestionModel.ManageCommentModel.LstQueries.Add(new QueryContent
                                {
                                    Content = new QueryInfo
                                    {
                                        QueryValue = query,
                                        QueryType = moduleSettingsUserControl.QueryControl.CurrentQuery.QueryType
                                    }
                                });
                            });
                    else if (moduleSettingsUserControl != null)
                        AnswerQuestionModel.ManageCommentModel.LstQueries.Add(new QueryContent
                        {
                            Content = new QueryInfo
                            {
                                QueryValue = moduleSettingsUserControl.QueryControl.CurrentQuery.QueryValue,
                                QueryType = moduleSettingsUserControl.QueryControl.CurrentQuery.QueryType
                            }
                        });
                }

                if (AnswerQuestionModel.ManageCommentModel.LstQueries[0].IsContentSelected)
                    AnswerQuestionModel.ManageCommentModel.LstQueries.Select(x =>
                    {
                        x.IsContentSelected = true;
                        return x;
                    }).ToList();
                AddQueryCommand.Execute(sender);
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

                var queryToDelete = AnswerQuestionModel.ManageCommentModel.LstQueries.FirstOrDefault(x =>
                    currentQuery != null && x.Content.QueryValue == currentQuery.QueryValue &&
                    x.Content.QueryType == currentQuery.QueryType);

                if (SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                    SavedQueries.Remove(currentQuery);

                AnswerQuestionModel.ManageCommentModel.LstQueries.Remove(queryToDelete);
                foreach (var message in AnswerQuestionModel.LstManageCommentModel.ToList())
                {
                    message.SelectedQuery.Remove(queryToDelete);
                    if (message.SelectedQuery.Count == 0)
                        AnswerQuestionModel.LstManageCommentModel.Remove(message);
                }

                if (!AnswerQuestionModel.ManageCommentModel.LstQueries.Skip(1).Any())
                    AnswerQuestionModel.ManageCommentModel.LstQueries[0].IsContentSelected = false;
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
                        var queryToDelete = AnswerQuestionModel.ManageCommentModel.LstQueries.FirstOrDefault(x =>
                            x.Content.QueryValue == currentQuery.QueryValue
                            && x.Content.QueryType == currentQuery.QueryType);

                        if (SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                            SavedQueries.Remove(currentQuery);

                        AnswerQuestionModel.ManageCommentModel.LstQueries.Remove(queryToDelete);
                        foreach (var message in AnswerQuestionModel.LstManageCommentModel.ToList())
                        {
                            message.SelectedQuery.Remove(queryToDelete);
                            if (message.SelectedQuery.Count == 0)
                                AnswerQuestionModel.LstManageCommentModel.Remove(message);
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
    }
}