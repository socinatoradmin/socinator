using System;
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
using QuoraDominatorCore.Models;

namespace QuoraDominatorCore.ViewModel.Scrape
{
    public class QuestionScraperViewModel : BindableBase
    {
        private QuestionsScraperModel _questionsScraperModel = new QuestionsScraperModel();

        public QuestionScraperViewModel()
        {
            QuestionsScraperModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName =
                    Application.Current.FindResource("LangKeyQuestionScrapePerJob")?.ToString(),
                ActivitiesPerHourDisplayName =
                    Application.Current.FindResource("LangKeyQuestionScrapePerHour")?.ToString(),
                ActivitiesPerDayDisplayName =
                    Application.Current.FindResource("LangKeyQuestionScrapePerDay")?.ToString(),
                ActivitiesPerWeekDisplayName =
                    Application.Current.FindResource("LangKeyQuestionScrapePerWeek")?.ToString(),
                IncreaseActivityDisplayName =
                    Application.Current.FindResource("LangKeyMaxQuestionScrapePerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            QuestionsScraperModel.TopicFilter.IsVisibleAnswerFilter = Visibility.Collapsed;
            QuestionsScraperModel.ListQueryType.Clear();

            Enum.GetValues(typeof(QuestionQueryParameters)).Cast<QuestionQueryParameters>().ToList().ForEach(query =>
            {
                QuestionsScraperModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())
                    ?.ToString());
            });
            QuestionsScraperModel.ListQueryType.Remove("Custom URL");

            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
        }

        public QuestionsScraperModel QuestionsScraperModel
        {
            get => _questionsScraperModel;
            set
            {
                if ((_questionsScraperModel == null) & (_questionsScraperModel == value))
                    return;
                SetProperty(ref _questionsScraperModel, value);
            }
        }

        public QuestionsScraperModel Model => QuestionsScraperModel;

        #region Command

        public ICommand CustomFilterCommand { get; set; }
        public ICommand AddQueryCommand { get; set; }

        #endregion

        #region Methods

        private void AddQuery(object sender)
        {
            try
            {
                var moduleSettingsUserControl =
                    sender as ModuleSettingsUserControl<QuestionScraperViewModel, QuestionsScraperModel>;
                moduleSettingsUserControl?.AddQuery(typeof(QuestionQueryParameters));
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
                    sender as ModuleSettingsUserControl<QuestionScraperViewModel, QuestionsScraperModel>;
                moduleSettingsUserControl?.CustomFilter();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion
    }
}