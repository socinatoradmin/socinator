using System.Collections.Generic;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace QuoraDominatorCore.Models
{
    public class QuestionsScraperModel : ModuleSetting, IGeneralSettings
    {
        private bool _enableFollowUserer;

        private RangeUtilities _followBetweenJobs = new RangeUtilities(10, 40);
        private RangeUtilities _followMaxBetween = new RangeUtilities(400, 500);
        private RangeUtilities _increaseEachDayFollow = new RangeUtilities(10, 20);
        private bool _ischkGroupblacklist;
        private bool _ischkprivateblacklist;

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

        [ProtoMember(1)]
        public override QuestionFilterModel QuestionFilterModel { get; set; } = new QuestionFilterModel();
        [ProtoMember(9)]
        public override TopicFilterModel TopicFilter { get; set; }=new TopicFilterModel();
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
        public bool IsChkPrivateblacklist
        {
            get => _ischkprivateblacklist;
            set
            {
                if (_ischkprivateblacklist == value)
                    return;
                SetProperty(ref _ischkprivateblacklist, value);
            }
        }

        [ProtoMember(8)]
        public bool IsChkGroupblacklist
        {
            get => _ischkGroupblacklist;
            set
            {
                if (_ischkGroupblacklist == value)
                    return;
                SetProperty(ref _ischkGroupblacklist, value);
            }
        }
        private bool _isChkSkipPrivateBlackList;
        private bool _isChkSkipGroupBlackList;
        public bool IsChkQuestionScraperSkipPrivateBlacklist
        {
            get => _isChkSkipPrivateBlackList;
            set
            {
                if (_isChkSkipPrivateBlackList != value)
                    SetProperty(ref _isChkSkipPrivateBlackList, value);
            }
        }

        public bool IsChkQuestionScraperSkipGroupBlacklist
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