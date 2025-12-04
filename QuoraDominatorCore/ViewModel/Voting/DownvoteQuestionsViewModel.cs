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

namespace QuoraDominatorCore.ViewModel.Voting
{
    public class DownvoteQuestionsViewModel : BindableBase
    {
        private DownvoteQuestionsModel _downvoteQuestionsModel = new DownvoteQuestionsModel();

        public DownvoteQuestionsViewModel()
        {
            DownvoteQuestionsModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current
                    .FindResource("LangKeyNumberOfQuestionsToDownvotePerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current
                    .FindResource("LangKeyNumberOfQuestionsToDownvotePerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current
                    .FindResource("LangKeyNumberOfQuestionsToDownvotePerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current
                    .FindResource("LangKeyNumberOfQuestionsToDownvotePerWeek")?.ToString(),
                IncreaseActivityDisplayName =
                    Application.Current.FindResource("LangKeyMaxQuestionsToDownvotePerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            DownvoteQuestionsModel.ListQueryType.Clear();
            DownvoteQuestionsModel.TopicFilter.IsVisibleAnswerFilter = Visibility.Collapsed;
            Enum.GetValues(typeof(QuestionQueryParameters)).Cast<QuestionQueryParameters>().ToList().ForEach(query =>
            {
                DownvoteQuestionsModel.ListQueryType.Add(Application.Current
                    .FindResource(query.GetDescriptionAttr())?.ToString());
            });


            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
        }

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }

        public DownvoteQuestionsModel DownvoteQuestionsModel
        {
            get => _downvoteQuestionsModel;
            set
            {
                if ((_downvoteQuestionsModel == null) & (_downvoteQuestionsModel == value))
                    return;
                SetProperty(ref _downvoteQuestionsModel, value);
            }
        }

        public DownvoteQuestionsModel Model => DownvoteQuestionsModel;

        private void AddQuery(object sender)
        {
            try
            {
                var moduleSettingsUserControl =
                    sender as ModuleSettingsUserControl<DownvoteQuestionsViewModel, DownvoteQuestionsModel>;
                moduleSettingsUserControl?.AddQuery(typeof(QuestionQueryParameters));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void CustomFilter(object sender)
        {
            // GlobalMethods.ShowQuestionFilterControl(DownvoteQuestionsSearchControl);
        }
    }
}