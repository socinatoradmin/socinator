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

namespace QuoraDominatorCore.ViewModel.GrowFollower
{
    public class FollowerViewModel : BindableBase
    {
        private FollowerModel _followerModel = new FollowerModel();

        public FollowerViewModel()
        {
            InitializeJobSetting();
            FollowerModel.ListQueryType.Clear();

            Enum.GetValues(typeof(FollowerQuery)).Cast<FollowerQuery>().ToList().ForEach(query =>
            {
                FollowerModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())
                    ?.ToString());
            });
            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
        }

        public FollowerModel FollowerModel
        {
            get => _followerModel;
            set
            {
                if ((_followerModel == null) & (_followerModel == value))
                    return;
                SetProperty(ref _followerModel, value);
            }
        }

        // NOTE: Required alias property to make it work at runtime
        public FollowerModel Model => FollowerModel;

        public void InitializeJobSetting()
        {
            try
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
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }

        #endregion

        #region Methods

        private void CustomFilter(object sender)
        {
            try
            {
                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<FollowerViewModel, FollowerModel>;
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
                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<FollowerViewModel, FollowerModel>;
                moduleSettingsUserControl?.AddQuery(typeof(FollowerQuery));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion
    }
}