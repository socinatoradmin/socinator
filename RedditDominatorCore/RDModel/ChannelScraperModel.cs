using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RedditDominatorCore.RDModel
{
    public class ChannelScraperModel : ModuleSetting, IGeneralSettings
    {
        //public new bool IsAccountGrowthActive { get; set; }
        private bool _isAccountGrowthActive;

        private bool _isChkGroupBlackList;

        private bool _isChkPrivateBlackList;

        private bool _isChkSkipBlackListedUser;

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
        public override ObservableCollection<QueryInfo> SavedQueries { get; set; } =
            new ObservableCollection<QueryInfo>();

        public List<string> ListQueryType { get; set; } = new List<string>();
        public RunningTimes RunningTimes { get; set; } = new RunningTimes();

        [ProtoMember(2)]
        public bool IsChkSkipBlackListedUser
        {
            get => _isChkSkipBlackListedUser;
            set
            {
                if (_isChkSkipBlackListedUser == value) return;
                SetProperty(ref _isChkSkipBlackListedUser, value);
            }
        }

        [ProtoMember(3)]
        public bool IsChkPrivateBlackList
        {
            get => _isChkPrivateBlackList;
            set
            {
                if (_isChkPrivateBlackList == value) return;
                SetProperty(ref _isChkPrivateBlackList, value);
            }
        }

        [ProtoMember(4)]
        public bool IsChkGroupBlackList
        {
            get => _isChkGroupBlackList;
            set
            {
                if (_isChkGroupBlackList == value) return;
                SetProperty(ref _isChkGroupBlackList, value);
            }
        }

        [ProtoMember(5)]
        public override CommunityFiltersModel CommunityFiltersModel { get; set; } = new CommunityFiltersModel();

        public override JobConfiguration JobConfiguration { get; set; } = new JobConfiguration();

        public new bool IsAccountGrowthActive
        {
            get => _isAccountGrowthActive;
            set
            {
                if (_isAccountGrowthActive == value) return;
                SetProperty(ref _isAccountGrowthActive, value);
            }
        }
    }
}