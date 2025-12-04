using System.Collections.Generic;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace TwtDominatorCore.TDModels
{
    internal interface IFollowerModel
    {
        bool IsChkEnableAutoFollowUnfollow { get; set; }
        bool IsChkStopFollow { get; set; }
        bool IsChkStopFollowToolWhenReach { get; set; }
        RangeUtilities StopFollowToolWhenReach { get; set; }
        bool IsChkStopFollowWhenFollowerFollowingsIsSmallerThan { get; set; }
        int FollowerFollowingsMaxValue { get; set; }
        bool IsChkFollowToolGetsTemporaryBlocked { get; set; }
        bool IsChkStartUnFollow { get; set; }
        bool IsChkStartUnFollowWhenReached { get; set; }
        RangeUtilities StartUnFollowToolWhenReach { get; set; }
        bool IsChkStartUnfollowWhenFollowerFollowingsIsSmallerThan { get; set; }
        int UnFollowerFollowingsMaxValue { get; set; }
        bool IschkwhenTheUnFollowToolGetsTemporaryBlocked { get; set; }
        bool IsChkUnfollowUsers { get; set; }
        RangeUtilities UnfollowPrevious { get; set; }

        bool IsChkUnfollowfollowedback { get; set; }

        bool IsChkUnfollownotfollowedback { get; set; }

        bool IsChkAcceptPendingFollowRequest { get; set; }
        bool IsChkAcceptbetween { get; set; }
        RangeUtilities AcceptBetween { get; set; }
    }

    [ProtoContract]
    public class FollowerModel : ModuleSetting, IFollowerModel, IGeneralSettings
    {
        public List<string> ListQueryType { get; set; } = new List<string>();


        [ProtoMember(2)] public override UserFilterModel UserFilterModel { get; set; } = new UserFilterModel();


        [ProtoMember(3)] public override TweetFilterModel TweetFilterModel { get; set; } = new TweetFilterModel();

        #region After Follow Action

        [ProtoMember(24)] public AfterFollowActionModel AfterFollowAction { get; set; } = new AfterFollowActionModel();

        #endregion


        [ProtoMember(4)] JobConfiguration IGeneralSettings.JobConfiguration { get; set; }


        #region IFollowerModel

        private bool _isChkEnableAutoFollowUnfollow;

        [ProtoMember(5)]
        public bool IsChkEnableAutoFollowUnfollow
        {
            get => _isChkEnableAutoFollowUnfollow;

            set
            {
                if (value == _isChkEnableAutoFollowUnfollow)
                    return;
                SetProperty(ref _isChkEnableAutoFollowUnfollow, value);
            }
        }

        private bool _isChkStopFollow;

        [ProtoMember(6)]
        public bool IsChkStopFollow
        {
            get => _isChkStopFollow;

            set
            {
                if (value == _isChkStopFollow)
                    return;
                SetProperty(ref _isChkStopFollow, value);
            }
        }


        private bool _isChkStopFollowToolWhenReach;

        [ProtoMember(7)]
        public bool IsChkStopFollowToolWhenReach
        {
            get => _isChkStopFollowToolWhenReach;

            set
            {
                if (value == _isChkStopFollowToolWhenReach)
                    return;
                SetProperty(ref _isChkStopFollowToolWhenReach, value);
            }
        }


        private RangeUtilities _stopFollowToolWhenReach = new RangeUtilities {StartValue = 1, EndValue = 1};

        [ProtoMember(8)]
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

        private bool _isChkStopFollowWhenFollowerFollowingsIsSmallerThan;

        [ProtoMember(9)]
        public bool IsChkStopFollowWhenFollowerFollowingsIsSmallerThan
        {
            get => _isChkStopFollowWhenFollowerFollowingsIsSmallerThan;

            set
            {
                if (value == _isChkStopFollowWhenFollowerFollowingsIsSmallerThan)
                    return;
                SetProperty(ref _isChkStopFollowWhenFollowerFollowingsIsSmallerThan, value);
            }
        }

        private int _followerFollowingsMaxValue;

        [ProtoMember(10)]
        public int FollowerFollowingsMaxValue
        {
            get => _followerFollowingsMaxValue;

            set
            {
                if (value == _followerFollowingsMaxValue)
                    return;
                SetProperty(ref _followerFollowingsMaxValue, value);
            }
        }

        private bool _isChkFollowToolGetsTemporaryBlocked;

        [ProtoMember(11)]
        public bool IsChkFollowToolGetsTemporaryBlocked
        {
            get => _isChkFollowToolGetsTemporaryBlocked;

            set
            {
                if (value == _isChkFollowToolGetsTemporaryBlocked)
                    return;
                SetProperty(ref _isChkFollowToolGetsTemporaryBlocked, value);
            }
        }

        private bool _isChkStartUnFollow;

        [ProtoMember(12)]
        public bool IsChkStartUnFollow
        {
            get => _isChkStartUnFollow;

            set
            {
                if (value == _isChkStartUnFollow)
                    return;
                SetProperty(ref _isChkStartUnFollow, value);
            }
        }

        private bool _isChkStartUnFollowWhenReached;

        [ProtoMember(13)]
        public bool IsChkStartUnFollowWhenReached
        {
            get => _isChkStartUnFollowWhenReached;

            set
            {
                if (value == _isChkStartUnFollowWhenReached)
                    return;
                SetProperty(ref _isChkStartUnFollowWhenReached, value);
            }
        }

        private RangeUtilities _startUnFollowToolWhenReach = new RangeUtilities {StartValue = 1, EndValue = 1};

        [ProtoMember(14)]
        public RangeUtilities StartUnFollowToolWhenReach
        {
            get => _startUnFollowToolWhenReach;

            set
            {
                if (value == _startUnFollowToolWhenReach)
                    return;
                SetProperty(ref _startUnFollowToolWhenReach, value);
            }
        }

        private bool _isChkStartUnfollowWhenFollowerFollowingsIsSmallerThan;

        [ProtoMember(15)]
        public bool IsChkStartUnfollowWhenFollowerFollowingsIsSmallerThan
        {
            get => _isChkStartUnfollowWhenFollowerFollowingsIsSmallerThan;

            set
            {
                if (value == _isChkStartUnfollowWhenFollowerFollowingsIsSmallerThan)
                    return;
                SetProperty(ref _isChkStartUnfollowWhenFollowerFollowingsIsSmallerThan, value);
            }
        }

        private int _unFollowerFollowingsMaxValue;

        [ProtoMember(16)]
        public int UnFollowerFollowingsMaxValue
        {
            get => _unFollowerFollowingsMaxValue;

            set
            {
                if (value == _unFollowerFollowingsMaxValue)
                    return;
                SetProperty(ref _unFollowerFollowingsMaxValue, value);
            }
        }

        private bool _ischkwhenTheUnFollowToolGetsTemporaryBlocked;

        [ProtoMember(17)]
        public bool IschkwhenTheUnFollowToolGetsTemporaryBlocked
        {
            get => _ischkwhenTheUnFollowToolGetsTemporaryBlocked;

            set
            {
                if (value == _ischkwhenTheUnFollowToolGetsTemporaryBlocked)
                    return;
                SetProperty(ref _ischkwhenTheUnFollowToolGetsTemporaryBlocked, value);
            }
        }


        private bool _isChkUnfollowUsers;

        [ProtoMember(18)]
        public bool IsChkUnfollowUsers
        {
            get => _isChkUnfollowUsers;

            set
            {
                if (value == _isChkUnfollowUsers)
                    return;
                SetProperty(ref _isChkUnfollowUsers, value);
            }
        }

        private RangeUtilities _unfollowPrevious = new RangeUtilities {StartValue = 1, EndValue = 1};

        [ProtoMember(19)]
        public RangeUtilities UnfollowPrevious
        {
            get => _unfollowPrevious;

            set
            {
                if (value == _unfollowPrevious)
                    return;
                SetProperty(ref _unfollowPrevious, value);
            }
        }

        private bool _isChkUnfollowfollowedback;

        [ProtoMember(20)]
        public bool IsChkUnfollowfollowedback
        {
            get => _isChkUnfollowfollowedback;

            set
            {
                if (value == _isChkUnfollowfollowedback)
                    return;
                SetProperty(ref _isChkUnfollowfollowedback, value);
            }
        }

        private bool _isChkUnfollownotfollowedback;

        [ProtoMember(21)]
        public bool IsChkUnfollownotfollowedback
        {
            get => _isChkUnfollownotfollowedback;

            set
            {
                if (value == _isChkUnfollownotfollowedback)
                    return;
                SetProperty(ref _isChkUnfollownotfollowedback, value);
            }
        }

        private bool _isChkAcceptPendingFollowRequest;

        [ProtoMember(22)]
        public bool IsChkAcceptPendingFollowRequest
        {
            get => _isChkAcceptPendingFollowRequest;

            set
            {
                if (value == _isChkAcceptPendingFollowRequest)
                    return;
                SetProperty(ref _isChkAcceptPendingFollowRequest, value);
            }
        }

        private bool _isChkAcceptbetween;

        [ProtoMember(23)]
        public bool IsChkAcceptbetween
        {
            get => _isChkAcceptbetween;

            set
            {
                if (value == _isChkAcceptbetween)
                    return;
                SetProperty(ref _isChkAcceptbetween, value);
            }
        }

        private RangeUtilities _acceptBetween = new RangeUtilities {StartValue = 1, EndValue = 1};

        [ProtoMember(24)]
        public RangeUtilities AcceptBetween
        {
            get => _acceptBetween;

            set
            {
                if (value == _acceptBetween)
                    return;
                SetProperty(ref _acceptBetween, value);
            }
        }

        private double _startValue;

        [ProtoMember(25)]
        public double startValue
        {
            get => _startValue;
            set => SetProperty(ref _startValue, value);
        }

        private bool _IsChkWhenFollowerFollowingsIsSmallerThan;

        [ProtoMember(26)]
        public bool IsChkWhenFollowerFollowingsIsSmallerThan
        {
            get => _IsChkWhenFollowerFollowingsIsSmallerThan;
            set => SetProperty(ref _IsChkWhenFollowerFollowingsIsSmallerThan, value);
        }

        //  IsChkWhenFollowerFollowingsIsSmallerThan

        #endregion

        #region Manage Speed 

        /// <summary>
        ///     Slow week 200
        ///     Medium week 400
        ///     Fast week 600
        ///     SuperFast week 800
        /// </summary>
        public JobConfiguration SlowSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(20, 25),
            ActivitiesPerHour = new RangeUtilities(4, 6),
            ActivitiesPerWeek = new RangeUtilities(150, 200),
            ActivitiesPerJob = new RangeUtilities(2, 3),
            DelayBetweenJobs = new RangeUtilities(20, 30),
            DelayBetweenActivity = new RangeUtilities(40, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(40, 60),
            ActivitiesPerHour = new RangeUtilities(8, 12),
            ActivitiesPerWeek = new RangeUtilities(300, 400),
            ActivitiesPerJob = new RangeUtilities(4, 6),
            DelayBetweenJobs = new RangeUtilities(50, 80),
            DelayBetweenActivity = new RangeUtilities(40, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration FastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(60, 90),
            ActivitiesPerHour = new RangeUtilities(10, 15),
            ActivitiesPerWeek = new RangeUtilities(500, 600),
            ActivitiesPerJob = new RangeUtilities(6, 8),
            DelayBetweenJobs = new RangeUtilities(100, 150),
            DelayBetweenActivity = new RangeUtilities(40, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(95, 120),
            ActivitiesPerHour = new RangeUtilities(18, 25),
            ActivitiesPerWeek = new RangeUtilities(600, 800),
            ActivitiesPerJob = new RangeUtilities(10, 15),
            DelayBetweenJobs = new RangeUtilities(180, 220),
            DelayBetweenActivity = new RangeUtilities(40, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        #endregion
    }
}