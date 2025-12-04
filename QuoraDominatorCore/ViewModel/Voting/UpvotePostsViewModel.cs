using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.QdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using QuoraDominatorCore.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QuoraDominatorCore.ViewModel.Voting
{
    public class UpvotePostsViewModel : BindableBase
    {
        private UpvotePostsModel _upvotePostsModel = new UpvotePostsModel();
        public UpvotePostsModel UpvotePostsModel
        {
            get => _upvotePostsModel;
            set
            {
                if ((_upvotePostsModel == null) & (_upvotePostsModel == value))
                    return;
                SetProperty(ref _upvotePostsModel, value);
            }
        }
        public UpvotePostsModel Model => UpvotePostsModel;
        public UpvotePostsViewModel()
        {
            UpvotePostsModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName =
                    Application.Current.FindResource("LangKeyUpvoteNumberOfPostsPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current.FindResource("LangKeyUpvoteNumberOfPostsPerHour")
                    ?.ToString(),
                ActivitiesPerDayDisplayName =
                    Application.Current.FindResource("LangKeyUpvoteNumberOfPostsPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current.FindResource("LangKeyUpvoteNumberOfPostsPerWeek")
                    ?.ToString(),
                IncreaseActivityDisplayName =
                    Application.Current.FindResource("LangKeyUpvoteNumberOfPostsPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            UpvotePostsModel.ListQueryType.Clear();
            Enum.GetValues(typeof(PostQueryParameters)).Cast<PostQueryParameters>().ToList().ForEach(query =>
            {
                UpvotePostsModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())
                    ?.ToString());
            });
            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
        }
        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        private void AddQuery(object sender)
        {
            try
            {
                var moduleSettingsUserControl =
                    sender as ModuleSettingsUserControl<UpvotePostsViewModel, UpvotePostsModel>;
                moduleSettingsUserControl?.AddQuery(typeof(PostQueryParameters));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void CustomFilter(object sender)
        {
            
        }
    }
}
