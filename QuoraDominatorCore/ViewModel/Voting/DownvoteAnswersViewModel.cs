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
    public class DownvoteAnswersViewModel : BindableBase
    {
        private DownvoteAnswersModel _downvoteAnswersModel = new DownvoteAnswersModel();

        public DownvoteAnswersViewModel()
        {
            DownvoteAnswersModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current.FindResource("LangKeyNumberOfAnswersToDownvotePerJob")
                    ?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current
                    .FindResource("LangKeyNumberOfAnswersToDownvotePerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current.FindResource("LangKeyNumberOfAnswersToDownvotePerDay")
                    ?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current
                    .FindResource("LangKeyNumberOfAnswersToDownvotePerWeek")?.ToString(),
                IncreaseActivityDisplayName =
                    Application.Current.FindResource("LangKeyMaxAnswersToDownvotePerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            DownvoteAnswersModel.ListQueryType.Clear();
            DownvoteAnswersModel.TopicFilter.IsVisibleAnswerFilter = Visibility.Visible;
            Enum.GetValues(typeof(AnswerQueryParameters)).Cast<AnswerQueryParameters>().ToList().ForEach(query =>
            {
                DownvoteAnswersModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())
                    ?.ToString());
            });
            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
        }

        public ICommand AddQueryCommand { get; set; }

        public DownvoteAnswersModel DownvoteAnswersModel
        {
            get => _downvoteAnswersModel;
            set
            {
                if ((_downvoteAnswersModel == null) & (_downvoteAnswersModel == value))
                    return;
                SetProperty(ref _downvoteAnswersModel, value);
            }
        }


        public DownvoteAnswersModel Model => DownvoteAnswersModel;

        private void AddQuery(object sender)
        {
            try
            {
                var moduleSettingsUserControl =
                    sender as ModuleSettingsUserControl<DownvoteAnswersViewModel, DownvoteAnswersModel>;
                moduleSettingsUserControl?.AddQuery(typeof(AnswerQueryParameters));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}