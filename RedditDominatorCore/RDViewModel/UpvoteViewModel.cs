using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.RdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using RedditDominatorCore.RDModel;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace RedditDominatorCore.RDViewModel
{
    public class UpvoteViewModel : BindableBase
    {
        public UpvoteViewModel()
        {
            if (UpvoteModel.ListQueryType.Count == 0)
            {
                UpvoteModel.ListQueryType.Add(Application.Current.FindResource(PostQuery.Keywords.GetDescriptionAttr())
                    ?.ToString());
                UpvoteModel.ListQueryType.Add(Application.Current.FindResource(PostQuery.CustomUrl.GetDescriptionAttr())
                    ?.ToString());
                //UpvoteModel.ListQueryType.Add(Application.Current
                //    .FindResource(PostQuery.CommunityUrl.GetDescriptionAttr())?.ToString());
                UpvoteModel.ListQueryType.Add(Application.Current
                    .FindResource(PostQuery.SpecificUserPost.GetDescriptionAttr())?.ToString());
                UpvoteModel.ListQueryType.Add(Application.Current
                    .FindResource(PostQuery.SocinatorPublisherCampaign.GetDescriptionAttr())?.ToString());
            }

            UpvoteModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfUpvotePerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfUpvotePerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfUpvotePerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfUpvotePerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyNumberOfMaxUpvotePerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
            DeleteMulipleCommand = new BaseCommand<object>(sender => true, DeleteMuliple);
        }

        public UpvoteModel Model => UpvoteModel;

        #region Object creation logic

        private UpvoteModel _upvoteModel = new UpvoteModel();

        public UpvoteModel UpvoteModel
        {
            get => _upvoteModel;
            set
            {
                if ((_upvoteModel == null) & (_upvoteModel == value))
                    return;
                SetProperty(ref _upvoteModel, value);
            }
        }

        #endregion

        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand DeleteMulipleCommand { get; set; }

        #endregion

        #region Methods

        private void CustomFilter(object sender)
        {
            try
            {
                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<UpvoteViewModel, UpvoteModel>;
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
                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<UpvoteViewModel, UpvoteModel>;
                moduleSettingsUserControl?.AddQuery(typeof(PostQuery));
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
                if (Model.SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                    Model.SavedQueries.Remove(currentQuery);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DeleteMuliple(object sender)
        {
            var selectedQuery = Model.SavedQueries.Where(x => x.IsQuerySelected).ToList();
            try
            {
                foreach (var currentQuery in selectedQuery)
                    try
                    {
                        if (Model.SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                            Model.SavedQueries.Remove(currentQuery);
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