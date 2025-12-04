using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.Generic;

namespace RedditDominatorCore.RDModel
{
    internal interface IUnFollower
    {
        bool IsChkEnableAutoFollowUnfollowChecked { get; set; }
        bool IsChkStopFollowToolWhenReachChecked { get; set; }
        RangeUtilities StopFollowToolWhenReachValue { get; set; }
        bool IsChkWhenFollowerFollowingsGreater { get; set; }
        int FollowerFollowingsGreaterThanValue { get; set; }
        bool IsChkWhenNoUsersToUnfollow { get; set; }
        bool IsChkStartFollowBetween { get; set; }
        RangeUtilities StartFollowToolBetween { get; set; }
        bool IsChkStartFollowWithoutStoppingUnfollow { get; set; }
        bool IsChkLikeWhileUnfollowing { get; set; }
        bool IsCheckedStopUnfollowStartFollow { get; set; }
        bool IschkOnlyStopUnfollowTool { get; set; }
        bool IsChkStopUnfollowToolWhenReachedSpecifiedFollowings { get; set; }
        RangeUtilities StopUnfollowToolWhenReachSpecifiedFollowings { get; set; }
        int FollowerFollowingsRatioValue { get; set; }
        bool IsChkPeopleFollowedOutsideSoftwareChecked { get; set; }
        bool IsChkPeopleFollowedBySoftwareChecked { get; set; }
        bool IsChkCustomUsersListChecked { get; set; }
        bool IsUserFollowedBeforeChecked { get; set; }
        int FollowedBeforeDay { get; set; }
        int FollowedBeforeHour { get; set; }
        string CustomUsersList { get; set; }
        List<string> LstCustomUser { get; set; }
        bool IsWhoDoNotFollowBackChecked { get; set; }
        bool IsWhoFollowBackChecked { get; set; }
    }

    [ProtoContract]
    public class UnfollowerModel : ModuleSetting, IUnFollower, IGeneralSettings
    {
        private string _customUsersList;

        private int _followedBeforeDay;

        private int _followedBeforeHour;

        private bool _isAddedToCampaign;

        private bool _isChkCustomUsersListChecked;

        private bool _isChkPeopleFollowedBySoftwareChecked;

        private bool _isChkPeopleFollowedOutsideSoftwareChecked;

        private bool _isUserFollowedBeforeChecked;

        private bool _isWhoDoNotFollowBackChecked;

        private bool _isWhoFollowBackChecked;

        private List<string> _lstCustomUser = new List<string>();

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

        [ProtoMember(1)] public override UserFilterModel UserFilterModel { get; set; } = new UserFilterModel();

        [ProtoMember(20)]
        public bool IsAddedToCampaign
        {
            get => _isAddedToCampaign;
            set
            {
                if (_isAddedToCampaign && _isAddedToCampaign == value)
                    return;
                SetProperty(ref _isAddedToCampaign, value);
            }
        }

        [ProtoMember(21)] public bool UnfollowUsersAfterDays { get; set; }

        [ProtoMember(22)] public int DaysBeforeUnfollow { get; set; }

        [ProtoMember(23)] public int MaxDaysSinceLastPost { get; set; }

        [ProtoMember(24)] public bool IgnoreFollowers { get; set; }

        [ProtoMember(2)] JobConfiguration IGeneralSettings.JobConfiguration { get; set; }

        [ProtoMember(25)]
        public bool IsChkPeopleFollowedOutsideSoftwareChecked
        {
            get => _isChkPeopleFollowedOutsideSoftwareChecked;
            set
            {
                if (_isChkPeopleFollowedOutsideSoftwareChecked == value) return;
                SetProperty(ref _isChkPeopleFollowedOutsideSoftwareChecked, value);
            }
        }

        [ProtoMember(26)]
        public bool IsChkPeopleFollowedBySoftwareChecked
        {
            get => _isChkPeopleFollowedBySoftwareChecked;
            set
            {
                if (value == _isChkPeopleFollowedBySoftwareChecked)
                    return;
                SetProperty(ref _isChkPeopleFollowedBySoftwareChecked, value);
            }
        }

        [ProtoMember(27)]
        public bool IsChkCustomUsersListChecked
        {
            get => _isChkCustomUsersListChecked;
            set
            {
                if (value == _isChkCustomUsersListChecked)
                    return;
                SetProperty(ref _isChkCustomUsersListChecked, value);
            }
        }

        [ProtoMember(28)]
        public bool IsUserFollowedBeforeChecked
        {
            get => _isUserFollowedBeforeChecked;
            set
            {
                if (value == _isUserFollowedBeforeChecked)
                    return;
                SetProperty(ref _isUserFollowedBeforeChecked, value);
            }
        }

        [ProtoMember(29)]
        public int FollowedBeforeDay
        {
            get => _followedBeforeDay;
            set
            {
                if (value == _followedBeforeDay)
                    return;
                SetProperty(ref _followedBeforeDay, value);
            }
        }

        [ProtoMember(30)]
        public int FollowedBeforeHour
        {
            get => _followedBeforeHour;
            set
            {
                if (value == _followedBeforeHour)
                    return;
                SetProperty(ref _followedBeforeHour, value);
            }
        }

        [ProtoMember(31)]
        public string CustomUsersList
        {
            get => _customUsersList;
            set
            {
                if (value == _customUsersList)
                    return;
                SetProperty(ref _customUsersList, value);
            }
        }

        [ProtoMember(32)]
        public List<string> LstCustomUser
        {
            get => _lstCustomUser;
            set
            {
                if (value == _lstCustomUser)
                    return;
                SetProperty(ref _lstCustomUser, value);
            }
        }

        [ProtoMember(33)]
        public bool IsWhoDoNotFollowBackChecked
        {
            get => _isWhoDoNotFollowBackChecked;
            set
            {
                if (_isWhoDoNotFollowBackChecked == value) return;
                SetProperty(ref _isWhoDoNotFollowBackChecked, value);
            }
        }

        [ProtoMember(34)]
        public bool IsWhoFollowBackChecked
        {
            get => _isWhoFollowBackChecked;
            set
            {
                if (_isWhoFollowBackChecked == value) return;
                SetProperty(ref _isWhoFollowBackChecked, value);
            }
        }

        #region IUnfollowerModel

        private bool _isChkAddToBlackList;

        [ProtoMember(3)]
        public bool IsChkAddToBlackListIsCheckedStopFollowStartUnfollow
        {
            get => _isChkAddToBlackList;
            set
            {
                if (_isChkAddToBlackList == value) return;
                SetProperty(ref _isChkAddToBlackList, value);
            }
        }

        private bool _isChkEnableAutoFollowUnfollowChecked;

        [ProtoMember(4)]
        public bool IsChkEnableAutoFollowUnfollowChecked
        {
            get => _isChkEnableAutoFollowUnfollowChecked;
            set
            {
                if (_isChkEnableAutoFollowUnfollowChecked == value) return;
                SetProperty(ref _isChkEnableAutoFollowUnfollowChecked, value);
            }
        }

        private bool _isChkStopFollowToolWhenReachChecked;

        [ProtoMember(5)]
        public bool IsChkStopFollowToolWhenReachChecked
        {
            get => _isChkStopFollowToolWhenReachChecked;
            set
            {
                if (_isChkStopFollowToolWhenReachChecked == value) return;
                SetProperty(ref _isChkStopFollowToolWhenReachChecked, value);
            }
        }

        private RangeUtilities _stopFollowToolWhenReachValue = new RangeUtilities();

        [ProtoMember(6)]
        public RangeUtilities StopFollowToolWhenReachValue
        {
            get => _stopFollowToolWhenReachValue;
            set
            {
                if (_stopFollowToolWhenReachValue == value) return;
                SetProperty(ref _stopFollowToolWhenReachValue, value);
            }
        }

        private bool _isChkWhenFollowerFollowingsGreater;

        [ProtoMember(7)]
        public bool IsChkWhenFollowerFollowingsGreater
        {
            get => _isChkWhenFollowerFollowingsGreater;
            set
            {
                if (_isChkWhenFollowerFollowingsGreater == value) return;
                SetProperty(ref _isChkWhenFollowerFollowingsGreater, value);
            }
        }

        private int _followerFollowingsGreaterThanValue;

        [ProtoMember(8)]
        public int FollowerFollowingsGreaterThanValue
        {
            get => _followerFollowingsGreaterThanValue;
            set
            {
                if (_followerFollowingsGreaterThanValue == value) return;
                SetProperty(ref _followerFollowingsGreaterThanValue, value);
            }
        }

        private bool _isChkWhenNoUsersToUnfollow;

        [ProtoMember(9)]
        public bool IsChkWhenNoUsersToUnfollow
        {
            get => _isChkWhenNoUsersToUnfollow;
            set
            {
                if (_isChkWhenNoUsersToUnfollow == value) return;
                SetProperty(ref _isChkWhenNoUsersToUnfollow, value);
            }
        }

        private bool _isChkStartFollowBetween;

        [ProtoMember(10)]
        public bool IsChkStartFollowBetween
        {
            get => _isChkStartFollowBetween;
            set
            {
                if (_isChkStartFollowBetween == value) return;
                SetProperty(ref _isChkStartFollowBetween, value);
            }
        }

        private RangeUtilities _startFollowToolBetween = new RangeUtilities();

        [ProtoMember(11)]
        public RangeUtilities StartFollowToolBetween
        {
            get => _startFollowToolBetween;
            set
            {
                if (_startFollowToolBetween == value) return;
                SetProperty(ref _startFollowToolBetween, value);
            }
        }

        private bool _isChkStartFollowWithoutStoppingUnfollow;

        [ProtoMember(12)]
        public bool IsChkStartFollowWithoutStoppingUnfollow
        {
            get => _isChkStartFollowWithoutStoppingUnfollow;
            set
            {
                if (_isChkStartFollowWithoutStoppingUnfollow == value) return;
                SetProperty(ref _isChkStartFollowWithoutStoppingUnfollow, value);
            }
        }

        private bool _isChkLikeWhileUnfollowing;

        [ProtoMember(13)]
        public bool IsChkLikeWhileUnfollowing
        {
            get => _isChkLikeWhileUnfollowing;
            set
            {
                if (_isChkLikeWhileUnfollowing == value) return;
                SetProperty(ref _isChkLikeWhileUnfollowing, value);
            }
        }

        private bool _filterInactiveUsers;

        [ProtoMember(14)]
        public bool FilterInactiveUsers
        {
            get => _filterInactiveUsers;
            set
            {
                if (value == _filterInactiveUsers)
                    return;
                SetProperty(ref _filterInactiveUsers, value);
            }
        }

        private bool _isCheckedStopUnfollowStartFollow = true;

        [ProtoMember(15)]
        public bool IsCheckedStopUnfollowStartFollow
        {
            get => _isCheckedStopUnfollowStartFollow;
            set
            {
                if (value == _isCheckedStopUnfollowStartFollow)
                    return;
                SetProperty(ref _isCheckedStopUnfollowStartFollow, value);
            }
        }

        private bool _ischkOnlyStopUnfollowTool;

        [ProtoMember(16)]
        public bool IschkOnlyStopUnfollowTool
        {
            get => _ischkOnlyStopUnfollowTool;
            set
            {
                if (value == _ischkOnlyStopUnfollowTool)
                    return;
                SetProperty(ref _ischkOnlyStopUnfollowTool, value);
            }
        }

        private bool _isChkStopUnfollowToolWhenReachedSpecifiedFollowings;

        [ProtoMember(17)]
        public bool IsChkStopUnfollowToolWhenReachedSpecifiedFollowings
        {
            get => _isChkStopUnfollowToolWhenReachedSpecifiedFollowings;
            set
            {
                if (value == _isChkStopUnfollowToolWhenReachedSpecifiedFollowings)
                    return;
                SetProperty(ref _isChkStopUnfollowToolWhenReachedSpecifiedFollowings, value);
            }
        }

        private RangeUtilities _stopUnfollowToolWhenReachSpecifiedFollowings = new RangeUtilities(1500, 2000);

        [ProtoMember(18)]
        public RangeUtilities StopUnfollowToolWhenReachSpecifiedFollowings
        {
            get => _stopUnfollowToolWhenReachSpecifiedFollowings;
            set
            {
                if (value == _stopUnfollowToolWhenReachSpecifiedFollowings)
                    return;
                SetProperty(ref _stopUnfollowToolWhenReachSpecifiedFollowings, value);
            }
        }

        private int _followerFollowingsRatioValue;

        [ProtoMember(19)]
        public int FollowerFollowingsRatioValue
        {
            get => _followerFollowingsRatioValue;
            set
            {
                if (value == _followerFollowingsRatioValue)
                    return;
                SetProperty(ref _followerFollowingsRatioValue, value);
            }
        }

        #endregion
    }
}