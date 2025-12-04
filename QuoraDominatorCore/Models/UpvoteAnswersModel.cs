using System.Collections.Generic;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace QuoraDominatorCore.Models
{
    public interface IUpvoteAnswersModel
    {
        #region IUpvoteAnswersModel

        bool EnableFollowUserer { get; set; }


        RangeUtilities FollowMaxBetween { get; set; }

        RangeUtilities IncreaseEachDayFollow { get; set; }

        #endregion
    }

    [ProtoContract]
    public class UpvoteAnswersModel : ModuleSetting, IUpvoteAnswersModel, IGeneralSettings
    {
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

        [ProtoMember(2)] public override UserFilterModel UserFilterModel { get; set; } = new UserFilterModel();

        [ProtoMember(3)] public override PostFilterModel PostFilterModel { get; set; } = new PostFilterModel();

        [ProtoMember(5)] public override AnswerFilterModel AnswerFilterModel { get; set; } = new AnswerFilterModel();
        [ProtoMember(15)]public override TopicFilterModel TopicFilter { get; set; }= new TopicFilterModel();
        [ProtoMember(4)] JobConfiguration IGeneralSettings.JobConfiguration { get; set; }

        #region IUpvoteAnswersModel

        private int _increaseUpvoteAnswersByCount = 10;

        [ProtoMember(5)]
        public int IncreaseUpvoteAnswersByCount
        {
            get => _increaseUpvoteAnswersByCount;
            set
            {
                if (value == _increaseUpvoteAnswersByCount) return;
                SetProperty(ref _increaseUpvoteAnswersByCount, value);
            }
        }

        private int _increaseUpvoteAnswersCountUntil = 500;

        [ProtoMember(6)]
        public int IncreaseUpvoteAnswersCountUntil
        {
            get => _increaseUpvoteAnswersCountUntil;
            set
            {
                if (value == _increaseUpvoteAnswersByCount) return;
                SetProperty(ref _increaseUpvoteAnswersCountUntil, value);
            }
        }


        private RangeUtilities _followBetweenJobs = new RangeUtilities(10, 40);

        [ProtoMember(7)]
        public RangeUtilities FollowBetweenJobs
        {
            get => _followBetweenJobs;
            set
            {
                if (value == _followBetweenJobs) return;
                SetProperty(ref _followBetweenJobs, value);
            }
        }

        private bool _enableFollowAnswerer;

        [ProtoMember(8)]
        public bool EnableFollowAnswerer
        {
            get => _enableFollowAnswerer;
            set
            {
                if (value == _enableFollowAnswerer) return;
                SetProperty(ref _enableFollowAnswerer, value);
            }
        }

        private RangeUtilities _followMaxBetween = new RangeUtilities(40, 50);

        [ProtoMember(9)]
        public RangeUtilities FollowMaxBetween
        {
            get => _followMaxBetween;
            set
            {
                if (value == _followMaxBetween) return;
                SetProperty(ref _followMaxBetween, value);
            }
        }


        private RangeUtilities _increaseEachDayFollow = new RangeUtilities(10, 100);

        [ProtoMember(10)]
        public RangeUtilities IncreaseEachDayFollow
        {
            get => _increaseEachDayFollow;
            set
            {
                if (value == _increaseEachDayFollow) return;
                SetProperty(ref _increaseEachDayFollow, value);
            }
        }

        private bool _enableFollowUserer;

        [ProtoMember(11)]
        public bool EnableFollowUserer
        {
            get => _enableFollowUserer;
            set
            {
                if (value == _enableFollowUserer) return;
                SetProperty(ref _enableFollowUserer, value);
            }
        }

        private bool _isChkIncreaseFollower;

        private bool _isChkGroupBlacklist;

        [ProtoMember(13)]
        public bool IsChkUpvoteAnswerGroupBlackList
        {
            get => _isChkGroupBlacklist;
            set
            {
                if (value == _isChkGroupBlacklist) return;
                SetProperty(ref _isChkGroupBlacklist, value);
            }
        }


        private bool _isChkPrivateBlacklist;

        [ProtoMember(14)]
        public bool IsChkUpvoteAnswerPrivateBlacklist
        {
            get => _isChkPrivateBlacklist;
            set
            {
                if (value == _isChkPrivateBlacklist) return;
                SetProperty(ref _isChkPrivateBlacklist, value);
            }
        }

        private bool _isChkFollowMax;

        [ProtoMember(13)]
        public bool IsChkFollowMax
        {
            get => _isChkFollowMax;
            set
            {
                if (value == _isChkFollowMax) return;
                SetProperty(ref _isChkFollowMax, value);
            }
        }
        private bool _isChkSkipPrivateBlackList;
        private bool _isChkSkipGroupBlackList;
        public bool IsChkUpvoteAnswerSkipPrivateBlacklist
        {
            get => _isChkSkipPrivateBlackList;
            set
            {
                if (_isChkSkipPrivateBlackList != value)
                    SetProperty(ref _isChkSkipPrivateBlackList, value);
            }
        }

        public bool IsChkUpvoteAnswerSkipGroupBlacklist
        {
            get => _isChkSkipGroupBlackList;
            set
            {
                if (_isChkSkipGroupBlackList != value)
                    SetProperty(ref _isChkSkipGroupBlackList, value);
            }
        }
        #endregion
    }
}