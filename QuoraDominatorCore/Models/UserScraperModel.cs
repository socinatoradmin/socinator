using System.Collections.Generic;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace QuoraDominatorCore.Models
{
    public class UserScraperModel : ModuleSetting, IGeneralSettings
    {
        private bool _enableFollowUserer;

        private RangeUtilities _followBetweenJobs = new RangeUtilities(10, 40);
        private RangeUtilities _followMaxBetween = new RangeUtilities(400, 500);
        private RangeUtilities _increaseEachDayFollow = new RangeUtilities(10, 20);

        private bool _isblacklist;

        private bool _isGrouplist;

        public JobConfiguration FastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(266, 400),
            ActivitiesPerHour = new RangeUtilities(26, 40),
            ActivitiesPerWeek = new RangeUtilities(1600, 2400),
            ActivitiesPerJob = new RangeUtilities(33, 50),
            DelayBetweenJobs = new RangeUtilities(81, 122),
            DelayBetweenActivity = new RangeUtilities(15, 30),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(133, 200),
            ActivitiesPerHour = new RangeUtilities(13, 20),
            ActivitiesPerWeek = new RangeUtilities(800, 1200),
            ActivitiesPerJob = new RangeUtilities(16, 25),
            DelayBetweenJobs = new RangeUtilities(73, 110),
            DelayBetweenActivity = new RangeUtilities(30, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration SlowSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(66, 100),
            ActivitiesPerHour = new RangeUtilities(6, 10),
            ActivitiesPerWeek = new RangeUtilities(400, 600),
            ActivitiesPerJob = new RangeUtilities(8, 12),
            DelayBetweenJobs = new RangeUtilities(81, 122),
            DelayBetweenActivity = new RangeUtilities(60, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(400, 600),
            ActivitiesPerHour = new RangeUtilities(40, 60),
            ActivitiesPerWeek = new RangeUtilities(2400, 3600),
            ActivitiesPerJob = new RangeUtilities(50, 75),
            DelayBetweenJobs = new RangeUtilities(65, 97),
            DelayBetweenActivity = new RangeUtilities(7, 15),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public List<string> ListQueryType { get; set; } = new List<string>();

        [ProtoMember(1)] public override UserFilterModel UserFilterModel { get; set; } = new UserFilterModel();

        [ProtoMember(3)]
        public RangeUtilities FollowBetweenJobs
        {
            get => _followBetweenJobs;
            set
            {
                if (value == _followBetweenJobs) return;
                SetProperty(ref _followBetweenJobs, value);
            }
        }

        [ProtoMember(4)]
        public bool EnableFollowUserer
        {
            get => _enableFollowUserer;
            set
            {
                if (value == _enableFollowUserer) return;
                SetProperty(ref _enableFollowUserer, value);
            }
        }

        [ProtoMember(5)]
        public RangeUtilities FollowMaxBetween
        {
            get => _followMaxBetween;
            set
            {
                if (value == _followMaxBetween) return;
                SetProperty(ref _followMaxBetween, value);
            }
        }

        [ProtoMember(6)]
        public RangeUtilities IncreaseEachDayFollow
        {
            get => _increaseEachDayFollow;
            set
            {
                if (value == _increaseEachDayFollow) return;
                SetProperty(ref _increaseEachDayFollow, value);
            }
        }

        [ProtoMember(7)]
        public bool IsChkUserScraperPrivateBlacklist
        {
            get => _isblacklist;
            set
            {
                if (_isblacklist != value)
                    SetProperty(ref _isblacklist, value);
            }
        }

        [ProtoMember(8)]
        public bool IsChkUserScraperGroupBlacklist
        {
            get => _isGrouplist;
            set
            {
                if (_isGrouplist != value)
                    SetProperty(ref _isGrouplist, value);
            }
        }
        private bool _isChkSkipPrivateBlackList;
        private bool _isChkSkipGroupBlackList;
        public bool IsChkUserScraperSkipPrivateBlacklist
        {
            get => _isChkSkipPrivateBlackList;
            set
            {
                if (_isChkSkipPrivateBlackList != value)
                    SetProperty(ref _isChkSkipPrivateBlackList, value);
            }
        }

        public bool IsChkUserScraperSkipGroupBlacklist
        {
            get => _isChkSkipGroupBlackList;
            set
            {
                if (_isChkSkipGroupBlackList != value)
                    SetProperty(ref _isChkSkipGroupBlackList, value);
            }
        }
        [ProtoMember(2)] JobConfiguration IGeneralSettings.JobConfiguration { get; set; }
    }
}