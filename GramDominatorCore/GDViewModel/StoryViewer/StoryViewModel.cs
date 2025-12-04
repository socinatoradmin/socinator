using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.GdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GramDominatorCore.GDViewModel.StoryViewer
{
    public class StoryViewModel: BindableBase
    {
        public StoryViewModel()
        {
            Enum.GetValues(typeof(GdUserQuery)).Cast<GdUserQuery>().ToList().ForEach(query =>
            {
                StoryModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())?.ToString());
            });
            StoryModel.ListQueryType.Remove("Scrape The User Who Messaged Us");
            StoryModel.ListQueryType.Remove("Own Followers");
            StoryModel.ListQueryType.Remove("Own Followings");
            StoryModel.ListQueryType.Remove("Scrape users whom we messaged");
            InitializeJobSetting();
        }

        private void InitializeJobSetting()
        {
            StoryModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfviewerPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfviewerPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfviewerPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfviewerPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxviewerPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            AddQueryCommand = new BaseCommand<object>((sender) => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>((sender) => true, CustomFilter);
        }


        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }


        private void CustomFilter(object sender)
        {
            try
            {
                var ModuleSettingsUserControl = sender as DominatorUIUtility.CustomControl.ModuleSettingsUserControl<StoryViewModel,StoryModel>;
                if (ModuleSettingsUserControl != null) ModuleSettingsUserControl.CustomFilter();
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
                var ModuleSettingsUserControl = /*((FrameworkElement)*/sender/*).DataContext*/ as /*StoryViewModel; //as*/ DominatorUIUtility.CustomControl.ModuleSettingsUserControl<StoryViewModel, StoryModel>;
                if (ModuleSettingsUserControl != null) ModuleSettingsUserControl.AddQuery(typeof(GdUserQuery), Model.ListQueryType);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private StoryModel _storyModel = new StoryModel();

        public StoryModel StoryModel
        {
            get
            {
                return _storyModel;
            }
            set
            {
                if (_storyModel == null & _storyModel == value)
                    return;
                SetProperty(ref _storyModel, value);
            }
        }

        // NOTE: Required alias property to make it work at runtime
        public StoryModel Model => StoryModel;
    }
}
