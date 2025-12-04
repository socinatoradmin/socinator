using System.Collections.Generic;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace TwtDominatorCore.TDModels
{
    [ProtoContract]
    public class UnFollower : BindableBase
    {
        private string _customUsers;
        private int _followedBeforeDay = 1;
        private int _followedBeforeHour = 1;
        private bool _isChkCustomUsersListChecked;

        private bool _isChkPeopleFollowedBySoftwareCheecked;
        private bool _isChkPeopleFollowedOutsideSoftwareChecked;
        private bool _isUserFollowedBeforeChecked;
        private bool _isWhoDoNotFollowBackChecked;
        private bool _isWhoFollowBackChecked;
        private List<string> _listOfCustomUsers = new List<string>();

        [ProtoMember(3)]
        public bool IsChkPeopleFollowedBySoftwareCheecked
        {
            get => _isChkPeopleFollowedBySoftwareCheecked;
            set
            {
                if (value == _isChkPeopleFollowedBySoftwareCheecked) return;
                SetProperty(ref _isChkPeopleFollowedBySoftwareCheecked, value);
            }
        }


        [ProtoMember(4)]
        public bool IsChkPeopleFollowedOutsideSoftwareChecked
        {
            get => _isChkPeopleFollowedOutsideSoftwareChecked;
            set
            {
                if (value == _isChkPeopleFollowedOutsideSoftwareChecked) return;
                SetProperty(ref _isChkPeopleFollowedOutsideSoftwareChecked, value);
            }
        }


        [ProtoMember(5)]
        public bool IsChkCustomUsersListChecked
        {
            get => _isChkCustomUsersListChecked;
            set
            {
                if (_isChkCustomUsersListChecked == value) return;

                SetProperty(ref _isChkCustomUsersListChecked, value);
            }
        }


        [ProtoMember(6)]
        public bool IsWhoDoNotFollowBackChecked
        {
            get => _isWhoDoNotFollowBackChecked;
            set
            {
                if (value)
                    IsWhoFollowBackChecked = false;

                SetProperty(ref _isWhoDoNotFollowBackChecked, value);
            }
        }


        [ProtoMember(7)]
        public bool IsWhoFollowBackChecked
        {
            get => _isWhoFollowBackChecked;
            set
            {
                if (value)
                    IsWhoDoNotFollowBackChecked = false;

                SetProperty(ref _isWhoFollowBackChecked, value);
            }
        }

        [ProtoMember(8)]
        public bool IsUserFollowedBeforeChecked
        {
            get => _isUserFollowedBeforeChecked;
            set
            {
                if (_isUserFollowedBeforeChecked == value) return;
                SetProperty(ref _isUserFollowedBeforeChecked, value);
            }
        }


        [ProtoMember(9)]
        public int FollowedBeforeDay
        {
            get => _followedBeforeDay;
            set
            {
                if (_followedBeforeDay == value) return;
                SetProperty(ref _followedBeforeDay, value);
            }
        }


        [ProtoMember(10)]
        public int FollowedBeforeHour
        {
            get => _followedBeforeHour;
            set
            {
                if (_followedBeforeHour == value) return;
                SetProperty(ref _followedBeforeHour, value);
            }
        }


        [ProtoMember(11)]
        public string CustomUsers
        {
            get => _customUsers;
            set => SetProperty(ref _customUsers, value);
        }


        [ProtoMember(12)]
        public List<string> ListCustomUsers
        {
            get => _listOfCustomUsers;
            set
            {
                if (value == _listOfCustomUsers)
                    return;
                SetProperty(ref _listOfCustomUsers, value);
            }
        }
    }

    public class UnfollowerModel : ModuleSetting, IGeneralSettings
    {
        private int _followerFollowingsGreaterThanValue;

        private bool _isChkAddToBlackList;

        private bool _isChkEnableAutoFollowUnfollowChecked;

        private bool _isChkLikeWhileUnfollowing;

        private bool _isChkStartFollowBetween;

        private bool _isChkStartFollowTool;

        private bool _isChkStartFollowWithoutStoppingUnfollow;

        private bool _isChkStopFollowToolWhenReachChecked;

        private bool _isChkStopUnFollowTool;


        private bool _isChkWhenFollowerFollowingsGreater;

        private bool _isChkWhenNoUsersToUnfollow;

        private RangeUtilities _startFollowToolBetwen = new RangeUtilities {StartValue = 1, EndValue = 1};

        private RangeUtilities _stopFollowToolWhenReachValue = new RangeUtilities {StartValue = 1, EndValue = 1};

        [ProtoMember(1)] public UserFilterModel UserFilterModel { get; set; } = new UserFilterModel();

        [ProtoMember(3)]
        public bool IsChkAddToBlackList
        {
            get => _isChkAddToBlackList;
            set
            {
                if (_isChkAddToBlackList == value) return;
                SetProperty(ref _isChkAddToBlackList, value);
            }
        }

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

        [ProtoMember(11)]
        public RangeUtilities StartFollowToolBetwen
        {
            get => _startFollowToolBetwen;
            set
            {
                if (_startFollowToolBetwen == value) return;
                SetProperty(ref _startFollowToolBetwen, value);
            }
        }

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

        [ProtoMember(14)]
        public bool IsChkStartFollowTool
        {
            get => _isChkStartFollowTool;
            set => SetProperty(ref _isChkStartFollowTool, value);
        }

        [ProtoMember(15)]
        public bool IsChkStopUnFollowTool
        {
            get => _isChkStopUnFollowTool;
            set => SetProperty(ref _isChkStopUnFollowTool, value);
        }

        [ProtoMember(2)] JobConfiguration IGeneralSettings.JobConfiguration { get; set; }

        #region Manage Speed 

        /// <summary>
        ///     Slow week 150
        ///     Medium week 300
        ///     Fast week 450
        ///     SuperFast week 600
        /// </summary>
        public JobConfiguration SlowSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(15, 20),
            ActivitiesPerHour = new RangeUtilities(4, 6),
            ActivitiesPerWeek = new RangeUtilities(120, 150),
            ActivitiesPerJob = new RangeUtilities(2, 3),
            DelayBetweenJobs = new RangeUtilities(20, 30),
            DelayBetweenActivity = new RangeUtilities(40, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(30, 45),
            ActivitiesPerHour = new RangeUtilities(8, 12),
            ActivitiesPerWeek = new RangeUtilities(250, 300),
            ActivitiesPerJob = new RangeUtilities(3, 4),
            DelayBetweenJobs = new RangeUtilities(50, 80),
            DelayBetweenActivity = new RangeUtilities(40, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration FastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(60, 70),
            ActivitiesPerHour = new RangeUtilities(10, 15),
            ActivitiesPerWeek = new RangeUtilities(400, 450),
            ActivitiesPerJob = new RangeUtilities(6, 8),
            DelayBetweenJobs = new RangeUtilities(100, 150),
            DelayBetweenActivity = new RangeUtilities(40, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(80, 90),
            ActivitiesPerHour = new RangeUtilities(18, 25),
            ActivitiesPerWeek = new RangeUtilities(500, 600),
            ActivitiesPerJob = new RangeUtilities(10, 15),
            DelayBetweenJobs = new RangeUtilities(180, 220),
            DelayBetweenActivity = new RangeUtilities(40, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        #endregion
    }
}