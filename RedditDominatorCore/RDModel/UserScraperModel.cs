using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RedditDominatorCore.RDModel
{
    public class UserScraperModel : ModuleSetting, IGeneralSettings
    {
        public JobConfiguration FastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(334, 500),
            ActivitiesPerHour = new RangeUtilities(40, 60),
            ActivitiesPerWeek = new RangeUtilities(2000, 3000),
            ActivitiesPerJob = new RangeUtilities(35, 50),
            DelayBetweenJobs = new RangeUtilities(65, 90),
            DelayBetweenActivity = new RangeUtilities(30, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(200, 300),
            ActivitiesPerHour = new RangeUtilities(30, 40),
            ActivitiesPerWeek = new RangeUtilities(1200, 1800),
            ActivitiesPerJob = new RangeUtilities(25, 35),
            DelayBetweenJobs = new RangeUtilities(65, 98),
            DelayBetweenActivity = new RangeUtilities(50, 90),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };
        //public bool IsAccountGrowthActive { get; set; }


        public JobConfiguration SlowSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(68, 100),
            ActivitiesPerHour = new RangeUtilities(10, 15),
            ActivitiesPerWeek = new RangeUtilities(400, 600),
            ActivitiesPerJob = new RangeUtilities(8, 12),
            DelayBetweenJobs = new RangeUtilities(74, 110),
            DelayBetweenActivity = new RangeUtilities(60, 120),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration SuperfastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(668, 1000),
            ActivitiesPerHour = new RangeUtilities(80, 100),
            ActivitiesPerWeek = new RangeUtilities(4000, 6000),
            ActivitiesPerJob = new RangeUtilities(60, 80),
            DelayBetweenJobs = new RangeUtilities(70, 104),
            DelayBetweenActivity = new RangeUtilities(15, 30),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        [ProtoMember(1)]
        public ObservableCollection<QueryInfo> SavedQueries { get; set; } = new ObservableCollection<QueryInfo>();

        public List<string> ListQueryType { get; set; } = new List<string>();

        [ProtoMember(2)] public UserFilterModel UserFilterModel { get; set; } = new UserFilterModel();

        [ProtoMember(3)] public PostFilterModel PostFilterModel { get; set; } = new PostFilterModel();

        public RunningTimes RunningTimes { get; set; } = new RunningTimes();
        public JobConfiguration JobConfiguration { get; set; } = new JobConfiguration();
    }
}