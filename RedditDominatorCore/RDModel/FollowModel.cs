using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RedditDominatorCore.RDModel
{
    public class FollowModel : ModuleSetting
    {
        private bool _isChkEnableAutoFollowUnfollowChecked;

        private bool _isChkPrivateBlackList;
        private bool _isChkGroupBlackList;

        private bool _isChkSkipBlackListedUser;
        private bool _isChkStartUnfollowToolBetweenChecked;
        private bool _isChkStopFollowToolWhenReachChecked;
        private bool _isChkUnfollowUsersChecked;

        private RangeUtilities _startFollowToolWhenReach = new RangeUtilities();
        private bool _startUnfollow;
        private RangeUtilities _stopFollowToolWhenReach = new RangeUtilities();

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
        public override ObservableCollection<QueryInfo> SavedQueries { get; set; } =
            new ObservableCollection<QueryInfo>();

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

        [ProtoMember(3)]
        public bool IsChkEnableAutoFollowUnfollowChecked
        {
            get => _isChkEnableAutoFollowUnfollowChecked;
            set
            {
                if (_isChkEnableAutoFollowUnfollowChecked == value) return;
                SetProperty(ref _isChkEnableAutoFollowUnfollowChecked, value);
            }
        }

        [ProtoMember(4)]
        public bool IsChkPrivateBlackList
        {
            get => _isChkPrivateBlackList;
            set
            {
                if (_isChkPrivateBlackList == value) return;
                SetProperty(ref _isChkPrivateBlackList, value);
            }
        }

        [ProtoMember(5)]
        public bool IsChkGroupBlackList
        {
            get => _isChkGroupBlackList;
            set
            {
                if (_isChkGroupBlackList == value) return;
                SetProperty(ref _isChkGroupBlackList, value);
            }
        }

        [ProtoMember(6)] public UserFilterModel UserFilterModel { get; set; } = new UserFilterModel();

        [ProtoMember(7)] public PostFilterModel PostFilterModel { get; set; } = new PostFilterModel();

        public override JobConfiguration JobConfiguration { get; set; }

        public RunningTimes RunningTimes { get; set; } = new RunningTimes();

        public bool IsChkFollowUnique { get; set; }
        public bool IsChkUnfollowfollowedbackChecked { get; set; }

        public bool IsChkStartUnFollowWhenReached { get; set; }

        public bool IsChkUnfollownotfollowedbackChecked { get; set; }

        public int FollowerFollowingsMaxValue { get; set; }

        public bool IsChkStartUnFollow { get; set; }

        public RangeUtilities StopFollowToolWhenReach
        {
            get => _stopFollowToolWhenReach;

            set
            {
                if (value == _stopFollowToolWhenReach)
                    return;
                SetProperty(ref _stopFollowToolWhenReach, value);
            }
        }

        public RangeUtilities StartUnFollowToolWhenReach
        {
            get => _startFollowToolWhenReach;
            set
            {
                if (value == _startFollowToolWhenReach)
                    return;
                SetProperty(ref _startFollowToolWhenReach, value);
            }
        }

        public bool IsChkFollowToolGetsTemporaryBlockedChecked { get; set; }

        public bool IsChkWhenFollowerFollowingsIsSmallerThanChecked { get; set; }

        public bool IsChkStopFollow { get; set; }

        public bool IsChkStopFollowToolWhenReachChecked
        {
            get => _isChkStopFollowToolWhenReachChecked;
            set
            {
                if (value == _isChkStopFollowToolWhenReachChecked)
                    return;
                SetProperty(ref _isChkStopFollowToolWhenReachChecked, value);
            }
        }

        public bool IsChkStartUnfollowToolBetweenChecked
        {
            get => _isChkStartUnfollowToolBetweenChecked;
            set
            {
                if (value == _isChkStartUnfollowToolBetweenChecked)
                    return;
                SetProperty(ref _isChkStartUnfollowToolBetweenChecked, value);
            }
        }

        public bool IsChkUnfollowUsersChecked
        {
            get => _isChkUnfollowUsersChecked;
            set
            {
                if (value == _isChkUnfollowUsersChecked)
                    return;
                SetProperty(ref _isChkUnfollowUsersChecked, value);
            }
        }

        public bool StartUnfollow
        {
            get => _startUnfollow;
            set
            {
                if (value == _startUnfollow)
                    return;
                SetProperty(ref _startUnfollow, value);
            }
        }

        public RangeUtilities UnfollowPrevious { get; set; }
    }
}