using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.GdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace GramDominatorCore.GDViewModel.StoryViewer
{
    public class AddInstaStoryViewModel: BindableBase
    {
        public AddInstaStoryViewModel()
        {
            Enum.GetValues(typeof(GdPostQuery)).Cast<GdPostQuery>().ToList().ForEach(query =>
            {
                var res = Application.Current.FindResource(query.GetDescriptionAttr())?.ToString();
                if(res == "Hashtag Post(S)" || res == "Location Posts"|| res == "Custom Photos"
                || res == "Specific User's Posts")
                    AddStory.ListQueryType.Add(res);
            });
            InitializeJobSetting();
        }
        private void InitializeJobSetting()
        {
            AddStory.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfAddStoryPerDay".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfAddStoryPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfAddStoryPerJob".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfAddStoryPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxAddStoryPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            AddQueryCommand = new BaseCommand<object>((sender) => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>((sender) => true, CustomFilter);
        }
        private void CustomFilter(object sender)
        {
            try
            {
                var ModuleSettingsUserControl = sender as DominatorUIUtility.CustomControl.ModuleSettingsUserControl<AddInstaStoryViewModel, AddInstaStoryModel>;
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
                var ModuleSettingsUserControl = /*((FrameworkElement)*/sender/*).DataContext*/ as /*StoryViewModel; //as*/ DominatorUIUtility.CustomControl.ModuleSettingsUserControl<AddInstaStoryViewModel, AddInstaStoryModel>;
                if (ModuleSettingsUserControl != null) 
                    ModuleSettingsUserControl.AddQuery(typeof(GdPostQuery), Model.ListQueryType);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        private AddInstaStoryModel _AddStory = new AddInstaStoryModel();
        public AddInstaStoryModel AddStory
        {
            get => _AddStory;
            set=>SetProperty(ref _AddStory, value);
        }
        public AddInstaStoryModel Model => AddStory;
    }
}
