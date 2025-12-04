using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.NetworkActivitySetting;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup
{
    public interface IJobConfigViewModel
    {
        void AddJobConfiguration(IEnumerable<ActivityChecked> allSelectedActivity);
    }

    public class JobConfigViewModel : StartupBaseViewModel, IJobConfigViewModel
    {
        private bool _isIndivisualSelected;

        private JobConfiguration _jobConfiguration = new JobConfiguration();

        private ObservableCollection<ActivityJobConfig> _lstJobConfiguration =
            new ObservableCollection<ActivityJobConfig>();

        public JobConfigViewModel(IRegionManager region) : base(region)
        {
            NextCommand = new DelegateCommand(NavigateNext);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            JobConfiguration = new JobConfiguration
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

        public bool IsIndivisualSelected
        {
            get => _isIndivisualSelected;
            set => SetProperty(ref _isIndivisualSelected, value);
        }

        public JobConfiguration JobConfiguration
        {
            get => _jobConfiguration;
            set => SetProperty(ref _jobConfiguration, value);
        }

        public ObservableCollection<ActivityJobConfig> LstJobConfiguration
        {
            get => _lstJobConfiguration;
            set => SetProperty(ref _lstJobConfiguration, value);
        }

        public Speed Model => new Speed();


        public void AddJobConfiguration(IEnumerable<ActivityChecked> allSelectedActivity)
        {
            //try
            //{
            //    LstJobConfiguration.Clear();
            //    var selectedNetwork = InstanceProvider.GetInstance<ISelectNetworkViewModel>().SelectedNetwork;
            //    var jobConfig = NetworkReg.RegisterNetworkJobConfig[selectedNetwork];
            //    ((dynamic)jobConfig).RegisterJobConfiguration();
            //    allSelectedActivity.ForEach(activity =>
            //    {
            //        LstJobConfiguration.Add(new ActivityJobConfig
            //        {
            //            ActivityType = activity.ActivityType,
            //            ActivityJobConfiguration = ((dynamic)jobConfig).RegisterJobConfigurations[activity.ActivityType].Model.JobConfiguration
            //        });
            //    });
            //}
            //catch (Exception ex)
            //{

            //}
        }
    }

    public class Speed
    {
        public JobConfiguration FastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(266, 400),
            ActivitiesPerHour = new RangeUtilities(26, 40),
            ActivitiesPerWeek = new RangeUtilities(1600, 2400),
            ActivitiesPerJob = new RangeUtilities(33, 50),
            DelayBetweenJobs = new RangeUtilities(65, 97),
            DelayBetweenActivity = new RangeUtilities(0, 1),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(133, 200),
            ActivitiesPerHour = new RangeUtilities(13, 20),
            ActivitiesPerWeek = new RangeUtilities(800, 1200),
            ActivitiesPerJob = new RangeUtilities(16, 25),
            DelayBetweenJobs = new RangeUtilities(73, 110),
            DelayBetweenActivity = new RangeUtilities(0, 1),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration SlowSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(66, 100),
            ActivitiesPerHour = new RangeUtilities(6, 10),
            ActivitiesPerWeek = new RangeUtilities(400, 600),
            ActivitiesPerJob = new RangeUtilities(8, 12),
            DelayBetweenJobs = new RangeUtilities(73, 110),
            DelayBetweenActivity = new RangeUtilities(1, 2),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration SuperfastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(400, 600),
            ActivitiesPerHour = new RangeUtilities(40, 60),
            ActivitiesPerWeek = new RangeUtilities(2400, 3600),
            ActivitiesPerJob = new RangeUtilities(50, 75),
            DelayBetweenJobs = new RangeUtilities(77, 116),
            DelayBetweenActivity = new RangeUtilities(0, 1),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };
    }

    public class ActivityJobConfig : BindableBase
    {
        private JobConfiguration _activityJobConfiguration = new JobConfiguration();
        private string _activityType;

        public JobConfiguration ActivityJobConfiguration
        {
            get => _activityJobConfiguration;
            set => SetProperty(ref _activityJobConfiguration, value);
        }

        public string ActivityType
        {
            get => _activityType;
            set => SetProperty(ref _activityType, value);
        }
    }
}