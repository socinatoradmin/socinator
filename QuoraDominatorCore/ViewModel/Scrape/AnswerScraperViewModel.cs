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
    public class AnswerScraperViewModel : BindableBase
    {
        private AnswersScraperModel _answersScraperModel = new AnswersScraperModel();

        public AnswerScraperViewModel()
        {
            try
            {
                AnswersScraperModel.JobConfiguration = new JobConfiguration
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

                AnswersScraperModel.ListQueryType.Clear();
                AnswersScraperModel.TopicFilter.IsVisibleAnswerFilter=Visibility.Visible;
                Enum.GetValues(typeof(AnswerQueryParameters)).Cast<AnswerQueryParameters>().ToList().ForEach(query =>
                {
                    AnswersScraperModel.ListQueryType.Add(Application.Current
                        .FindResource(query.GetDescriptionAttr())?.ToString());
                });
                AnswersScraperModel.ListQueryType.Remove("Custom URL");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
        }

        public AnswersScraperModel AnswersScraperModel
        {
            get => _answersScraperModel;
            set
            {
                if ((_answersScraperModel == null) & (_answersScraperModel == value))
                    return;
                SetProperty(ref _answersScraperModel, value);
            }
        }

        public AnswersScraperModel Model => AnswersScraperModel;

        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }

        #endregion

        #region Methods

        private void CustomFilter(object sender)
        {
            try
            {
                var moduleSettingsUserControl =
                    sender as ModuleSettingsUserControl<AnswerScraperViewModel, AnswersScraperModel>;
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
                    sender as ModuleSettingsUserControl<AnswerScraperViewModel, AnswersScraperModel>;
                moduleSettingsUserControl?.AddQuery(typeof(AnswerQueryParameters));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion
    }
}