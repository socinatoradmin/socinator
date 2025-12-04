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
    public class UpvoteAnswersViewModel : BindableBase
    {
        private UpvoteAnswersModel _upvoteAnswersModel = new UpvoteAnswersModel();

        public UpvoteAnswersViewModel()
        {
            UpvoteAnswersModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfAnswersToUpvotePerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current.FindResource("LangKeyNumberOfAnswersToUpvotePerHour")
                    ?.ToString(),
                ActivitiesPerDayDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfAnswersToUpvotePerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current.FindResource("LangKeyNumberOfAnswersToUpvotePerWeek")
                    ?.ToString(),
                IncreaseActivityDisplayName =
                    Application.Current.FindResource("LangKeyMaxAnswersToUpvotePerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            UpvoteAnswersModel.ListQueryType.Clear();
            UpvoteAnswersModel.TopicFilter.IsVisibleAnswerFilter = Visibility.Visible;
            Enum.GetValues(typeof(AnswerQueryParameters)).Cast<AnswerQueryParameters>().ToList().ForEach(query =>
            {
                UpvoteAnswersModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())
                    ?.ToString());
            });
            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
        }

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }

        public UpvoteAnswersModel UpvoteAnswersModel
        {
            get => _upvoteAnswersModel;
            set
            {
                if ((_upvoteAnswersModel == null) & (_upvoteAnswersModel == value))
                    return;
                SetProperty(ref _upvoteAnswersModel, value);
            }
        }

        public UpvoteAnswersModel Model => UpvoteAnswersModel;

        private void AddQuery(object sender)
        {
            try
            {
                var moduleSettingsUserControl =
                    sender as ModuleSettingsUserControl<UpvoteAnswersViewModel, UpvoteAnswersModel>;
                moduleSettingsUserControl?.AddQuery(typeof(AnswerQueryParameters));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void CustomFilter(object sender)
        {
            // GlobalMethods.ShowAnswerFilterControl(UpvoteAnswersSearchControl);
        }
    }
}