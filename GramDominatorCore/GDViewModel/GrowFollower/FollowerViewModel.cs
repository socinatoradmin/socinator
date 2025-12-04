using System;
using System.Linq;
using System.Windows;
using DominatorHouseCore.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.GdQuery;
using DominatorHouseCore.Models;
using GramDominatorCore.GDModel;
using DominatorHouseCore.Command;
using DominatorHouseCore;
using System.Windows.Input;

namespace GramDominatorCore.GDViewModel.GrowFollower
{

    public class FollowerViewModel : BindableBase
    {
        public FollowerViewModel()
        {
            Enum.GetValues(typeof(GdUserQuery)).Cast<GdUserQuery>().ToList().ForEach(query =>
          {
              FollowerModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())?.ToString());
          });
            FollowerModel.ListQueryType.Remove("Scrape The User Who Messaged Us");
            FollowerModel.ListQueryType.Remove("Own Followers");
            FollowerModel.ListQueryType.Remove("Own Followings");
            FollowerModel.ListQueryType.Remove("Scrape users whom we messaged");
            InitializeJobSetting();
        }

        private void InitializeJobSetting()
        {
            FollowerModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfFollowsPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfFollowsPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfFollowsPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfFollowsPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxFollowsPerDay".FromResourceDictionary(),
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
                var ModuleSettingsUserControl = sender as DominatorUIUtility.CustomControl.ModuleSettingsUserControl<FollowerViewModel, FollowerModel>;
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
                var ModuleSettingsUserControl = sender as DominatorUIUtility.CustomControl.ModuleSettingsUserControl<FollowerViewModel, FollowerModel>;
                if (ModuleSettingsUserControl != null) ModuleSettingsUserControl.AddQuery(typeof(GdUserQuery), Model.ListQueryType);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private FollowerModel _followerModel = new FollowerModel();

        public FollowerModel FollowerModel
        {
            get
            {
                return _followerModel;
            }
            set
            {
                if (_followerModel == null & _followerModel == value)
                    return;
                SetProperty(ref _followerModel, value);
            }
        }

        // NOTE: Required alias property to make it work at runtime
        public FollowerModel Model => FollowerModel;
    }
}
