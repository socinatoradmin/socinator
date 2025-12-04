using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RedditDominatorCore.RDModel
{
    [ProtoContract]
    public class SubscribeModel : ModuleSetting
    {
        private bool _isAccountGrowthActive;

        private bool _isChkEnableAutoSubscribeUnSubscribeChecked;

        private bool _isChkPrivateBlackList;


        private bool _isChkSkipBlackListedUser;
        private bool _isChkStartUnSubscribeToolBetweenChecked;
        private bool _isChkStopSubscribeToolWhenReachChecked;
        private bool _isChkUnSubscribeUsersChecked;

        private RangeUtilities _startSubscribeToolWhenReach = new RangeUtilities();
        private bool _startUnSubscribe;
        private RangeUtilities _stopSubscribeToolWhenReach = new RangeUtilities();

        public JobConfiguration FastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(222, 320),
            ActivitiesPerHour = new RangeUtilities(40, 50),
            ActivitiesPerWeek = new RangeUtilities(1280, 1920),
            ActivitiesPerJob = new RangeUtilities(25, 40),
            DelayBetweenJobs = new RangeUtilities(74, 111),
            DelayBetweenActivity = new RangeUtilities(30, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(120, 180),
            ActivitiesPerHour = new RangeUtilities(12, 18),
            ActivitiesPerWeek = new RangeUtilities(720, 1080),
            ActivitiesPerJob = new RangeUtilities(15, 22),
            DelayBetweenJobs = new RangeUtilities(75, 112),
            DelayBetweenActivity = new RangeUtilities(50, 90),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration SlowSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(48, 70),
            ActivitiesPerHour = new RangeUtilities(10, 15),
            ActivitiesPerWeek = new RangeUtilities(280, 420),
            ActivitiesPerJob = new RangeUtilities(7, 10),
            DelayBetweenJobs = new RangeUtilities(80, 120),
            DelayBetweenActivity = new RangeUtilities(60, 120),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(534, 800),
            ActivitiesPerHour = new RangeUtilities(68, 100),
            ActivitiesPerWeek = new RangeUtilities(3200, 4800),
            ActivitiesPerJob = new RangeUtilities(60, 80),
            DelayBetweenJobs = new RangeUtilities(70, 110),
            DelayBetweenActivity = new RangeUtilities(20, 40),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        [ProtoMember(1)]
        public ObservableCollection<QueryInfo> SavedQueries { get; set; } = new ObservableCollection<QueryInfo>();

        public List<string> ListQueryType { get; set; } = new List<string>();

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

        [ProtoMember(2)]
        public bool IsChkEnableAutoSubscribeUnSubscribeChecked
        {
            get => _isChkEnableAutoSubscribeUnSubscribeChecked;
            set
            {
                if (_isChkEnableAutoSubscribeUnSubscribeChecked == value) return;
                SetProperty(ref _isChkEnableAutoSubscribeUnSubscribeChecked, value);
            }
        }

        [ProtoMember(2)]
        public bool IsChkPrivateBlackList
        {
            get => _isChkPrivateBlackList;
            set
            {
                if (_isChkPrivateBlackList == value) return;
                SetProperty(ref _isChkPrivateBlackList, value);
            }
        }

        [ProtoMember(3)]
        public bool IsChkGroupBlackList
        {
            get => _isChkPrivateBlackList;
            set
            {
                if (_isChkPrivateBlackList == value) return;
                SetProperty(ref _isChkPrivateBlackList, value);
            }
        }


        public RunningTimes RunningTimes { get; set; } = new RunningTimes();

        public bool IsAccountGrowthActive
        {
            get => _isAccountGrowthActive;
            set
            {
                if (_isAccountGrowthActive == value) return;
                SetProperty(ref _isAccountGrowthActive, value);
            }
        }

        public bool IsChkSubscribeUnique { get; set; }
        public bool IsChkUnSubscribeSubscribeedbackChecked { get; set; }

        public bool IsChkStartUnSubscribeWhenReached { get; set; }

        public bool IsChkUnSubscribenotSubscribeedbackChecked { get; set; }

        public int SubscribeerSubscribeingsMaxValue { get; set; }

        public bool IsChkStartUnSubscribe { get; set; }

        public RangeUtilities StopSubscribeToolWhenReach
        {
            get => _stopSubscribeToolWhenReach;

            set
            {
                if (value == _stopSubscribeToolWhenReach)
                    return;
                SetProperty(ref _stopSubscribeToolWhenReach, value);
            }
        }

        public RangeUtilities StartUnSubscribeToolWhenReach
        {
            get => _startSubscribeToolWhenReach;
            set
            {
                if (value == _startSubscribeToolWhenReach)
                    return;
                SetProperty(ref _startSubscribeToolWhenReach, value);
            }
        }

        public bool IsChkSubscribeToolGetsTemporaryBlockedChecked { get; set; }

        public bool IsChkWhenSubscribeerSubscribeingsIsSmallerThanChecked { get; set; }

        public bool IsChkStopSubscribe { get; set; }

        public bool IsChkStopSubscribeToolWhenReachChecked
        {
            get => _isChkStopSubscribeToolWhenReachChecked;
            set
            {
                if (value == _isChkStopSubscribeToolWhenReachChecked)
                    return;
                SetProperty(ref _isChkStopSubscribeToolWhenReachChecked, value);
            }
        }

        public bool IsChkStartUnSubscribeToolBetweenChecked
        {
            get => _isChkStartUnSubscribeToolBetweenChecked;
            set
            {
                if (value == _isChkStartUnSubscribeToolBetweenChecked)
                    return;
                SetProperty(ref _isChkStartUnSubscribeToolBetweenChecked, value);
            }
        }

        public bool IsChkUnSubscribeUsersChecked
        {
            get => _isChkUnSubscribeUsersChecked;
            set
            {
                if (value == _isChkUnSubscribeUsersChecked)
                    return;
                SetProperty(ref _isChkUnSubscribeUsersChecked, value);
            }
        }

        public bool StartUnSubscribe
        {
            get => _startUnSubscribe;
            set
            {
                if (value == _startUnSubscribe)
                    return;
                SetProperty(ref _startUnSubscribe, value);
            }
        }

        public RangeUtilities UnSubscribePrevious { get; set; }
        public override CommunityFiltersModel CommunityFiltersModel { get; set; } = new CommunityFiltersModel();
    }
}