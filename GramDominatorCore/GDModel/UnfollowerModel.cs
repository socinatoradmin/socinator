using System.Collections.Generic;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary;
using ProtoBuf;

namespace GramDominatorCore.GDModel
{
    interface IUnFollower
    {
    }

    [ProtoContract]
    public class UnfollowerModel : ModuleSetting, IUnFollower, IGeneralSettings
    {

        [ProtoMember(1)]
        public override UserFilterModel UserFilterModel { get; set; } = new UserFilterModel();

        [ProtoMember(3)]
        JobConfiguration IGeneralSettings.JobConfiguration { get; set; }

        [ProtoMember(34)]
        public override MangeBlacklist.ManageBlackWhiteListModel ManageBlackWhiteListModel { get; set; } = new MangeBlacklist.ManageBlackWhiteListModel();


        #region Set Job Configuration speed
        public JobConfiguration SlowSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(66, 100),
            ActivitiesPerHour = new RangeUtilities(6, 10),
            ActivitiesPerWeek = new RangeUtilities(400, 600),
            ActivitiesPerJob = new RangeUtilities(8, 12),
            DelayBetweenJobs = new RangeUtilities(81, 122),
            DelayBetweenActivity = new RangeUtilities(30, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(200, 300),
            ActivitiesPerHour = new RangeUtilities(20, 30),
            ActivitiesPerWeek = new RangeUtilities(1200, 1800),
            ActivitiesPerJob = new RangeUtilities(25, 37),
            DelayBetweenJobs = new RangeUtilities(72, 108),
            DelayBetweenActivity = new RangeUtilities(25, 50),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration FastSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(333, 500),
            ActivitiesPerHour = new RangeUtilities(33, 50),
            ActivitiesPerWeek = new RangeUtilities(2000, 3000),
            ActivitiesPerJob = new RangeUtilities(41, 62),
            DelayBetweenJobs = new RangeUtilities(69, 103),
            DelayBetweenActivity = new RangeUtilities(15, 30),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(533, 800),
            ActivitiesPerHour = new RangeUtilities(53, 80),
            ActivitiesPerWeek = new RangeUtilities(3200, 4800),
            ActivitiesPerJob = new RangeUtilities(66, 100),
            DelayBetweenJobs = new RangeUtilities(73, 110),
            DelayBetweenActivity = new RangeUtilities(8, 15),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };
        #endregion


        #region IUnfollowerModel

        private bool _isChkPeopleFollowedBySoftwareChecked;
        [ProtoMember(5)]
        public bool IsChkPeopleFollowedBySoftwareChecked
        {
            get
            {
                return _isChkPeopleFollowedBySoftwareChecked;
            }
            set
            {
                if (value == _isChkPeopleFollowedBySoftwareChecked)
                {
                    return;
                }
                SetProperty(ref _isChkPeopleFollowedBySoftwareChecked, value);
            }

        }


        private bool _isChkPeopleFollowedOutsideSoftwareChecked;
        [ProtoMember(6)]
        public bool IsChkPeopleFollowedOutsideSoftwareChecked
        {
            get
            {
                return _isChkPeopleFollowedOutsideSoftwareChecked;
            }
            set
            {
                if (value == _isChkPeopleFollowedOutsideSoftwareChecked)
                {
                    return;
                }
                SetProperty(ref _isChkPeopleFollowedOutsideSoftwareChecked, value);
            }
        }

        private bool _isChkCustomUsersListChecked;
        [ProtoMember(7)]
        public bool IsChkCustomUsersListChecked
        {
            get
            {
                return _isChkCustomUsersListChecked;
            }
            set
            {
                if (_isChkCustomUsersListChecked == value)
                {
                    return;
                }

                SetProperty(ref _isChkCustomUsersListChecked, value);
            }
        }


        private bool _isChkEnableAutoFollowUnfollowChecked;
        [ProtoMember(9)]
        public bool IsChkEnableAutoFollowUnfollowChecked
        {
            get
            {
                return _isChkEnableAutoFollowUnfollowChecked;
            }
            set
            {
                if (_isChkEnableAutoFollowUnfollowChecked == value)
                {
                    return;
                }
                SetProperty(ref _isChkEnableAutoFollowUnfollowChecked, value);
            }
        }


        private bool _isChkWhenFollowerFollowingsGreater;
        [ProtoMember(12)]
        public bool IsChkWhenFollowerFollowingsGreater
        {
            get
            {
                return _isChkWhenFollowerFollowingsGreater;
            }
            set
            {
                if (_isChkWhenFollowerFollowingsGreater == value)
                {
                    return;
                }
                SetProperty(ref _isChkWhenFollowerFollowingsGreater, value);
            }
        }


        private bool _isWhoDoNotFollowBackChecked = true;
        [ProtoMember(21)]
        public bool IsWhoDoNotFollowBackChecked
        {
            get
            {
                return _isWhoDoNotFollowBackChecked;
            }
            set
            {
                if (_isWhoDoNotFollowBackChecked == value)
                {
                    return;
                }
                SetProperty(ref _isWhoDoNotFollowBackChecked, value);
            }
        }

        private bool _isWhoFollowBackChecked;

        [ProtoMember(22)]
        public bool IsWhoFollowBackChecked
        {
            get
            {
                return _isWhoFollowBackChecked;
            }
            set
            {
                if (_isWhoFollowBackChecked == value)
                {
                    return;
                }
                SetProperty(ref _isWhoFollowBackChecked, value);
            }
        }

        private bool _isUserFollowedBeforeChecked;

        [ProtoMember(23)]
        public bool IsUserFollowedBeforeChecked
        {
            get
            {
                return _isUserFollowedBeforeChecked;
            }
            set
            {
                if (_isUserFollowedBeforeChecked == value)
                {
                    return;
                }
                SetProperty(ref _isUserFollowedBeforeChecked, value);
            }
        }

        private int _followedBeforeDay = 1;

        [ProtoMember(24)]
        public int FollowedBeforeDay
        {
            get
            {
                return _followedBeforeDay;
            }
            set
            {
                if (_followedBeforeDay == value)
                {
                    return;
                }
                SetProperty(ref _followedBeforeDay, value);
            }
        }

        private int _followedBeforeHour;

        [ProtoMember(25)]
        public int FollowedBeforeHour
        {
            get
            {
                return _followedBeforeHour;
            }
            set
            {
                if (_followedBeforeHour == value)
                {
                    return;
                }
                SetProperty(ref _followedBeforeHour, value);
            }
        }
        private string _customUsersList;

        [ProtoMember(26)]
        public string CustomUsersList
        {
            get
            {
                return _customUsersList;
            }
            set
            {
                if (value == _customUsersList)
                    return;
                SetProperty(ref _customUsersList, value);
            }
        }

      

        [ProtoMember(28)]
        private List<string> _lstCustomUser;

        public List<string> LstCustomUser
        {
            get
            {
                return _lstCustomUser;
            }
            set
            {
                if (value == _lstCustomUser)
                    return;
                SetProperty(ref _lstCustomUser, value);
            }
        }


        private bool _isCheckedStopUnfollowStartFollow = true;
        [ProtoMember(29)]
        public bool IsCheckedStopUnfollowStartFollow
        {
            get
            {
                return _isCheckedStopUnfollowStartFollow;
            }
            set
            {
                if (value == _isCheckedStopUnfollowStartFollow)
                    return;
                SetProperty(ref _isCheckedStopUnfollowStartFollow, value);
            }
        }


        private bool _ischkOnlyStopUnfollowTool;
        [ProtoMember(30)]
        public bool IschkOnlyStopUnfollowTool
        {
            get
            {
                return _ischkOnlyStopUnfollowTool;
            }
            set
            {
                if (value == _ischkOnlyStopUnfollowTool)
                    return;
                SetProperty(ref _ischkOnlyStopUnfollowTool, value);
            }
        }


        private bool _isChkStopUnfollowToolWhenReachedSpecifiedFollowings;
        [ProtoMember(31)]
        public bool IsChkStopUnfollowToolWhenReachedSpecifiedFollowings
        {
            get
            {
                return _isChkStopUnfollowToolWhenReachedSpecifiedFollowings;
            }
            set
            {
                if (value == _isChkStopUnfollowToolWhenReachedSpecifiedFollowings)
                    return;
                SetProperty(ref _isChkStopUnfollowToolWhenReachedSpecifiedFollowings, value);
            }
        }


        private RangeUtilities _stopUnfollowToolWhenReachSpecifiedFollowings = new RangeUtilities(1500, 2000);
        [ProtoMember(32)]
        public RangeUtilities StopUnfollowToolWhenReachSpecifiedFollowings
        {
            get
            {
                return _stopUnfollowToolWhenReachSpecifiedFollowings;
            }
            set
            {
                if (value == _stopUnfollowToolWhenReachSpecifiedFollowings)
                    return;
                SetProperty(ref _stopUnfollowToolWhenReachSpecifiedFollowings, value);
            }
        }


        private int _followerFollowingsRatioValue;
        [ProtoMember(33)]
        public int FollowerFollowingsRatioValue
        {
            get
            {
                return _followerFollowingsRatioValue;
            }
            set
            {
                if (value == _followerFollowingsRatioValue)
                    return;
                SetProperty(ref _followerFollowingsRatioValue, value);
            }
        }


        private bool _isUseFilterBySourceType;
        [ProtoMember(35)]
        public bool IsUseFilterBySourceType
        {
            get
            {
                return _isUseFilterBySourceType;
            }
            set
            {
                if (value == _isUseFilterBySourceType)
                    return;
                SetProperty(ref _isUseFilterBySourceType, value);
            }
        }

        private bool _blockUnblockFollower;
        [ProtoMember(36)]
        public bool IsBlockUnBlockUnfollow
        {
            get
            {
                return _blockUnblockFollower;
            }
            set
            {
                if (value == _blockUnblockFollower)
                    return;
                SetProperty(ref _blockUnblockFollower, value);
            }
        }

      

        private bool _isChkCustomFollowUsersListChecked;
        [ProtoMember(37)]
        public bool IsChkCustomFollowUsersListChecked
        {
            get
            {
                return _isChkCustomFollowUsersListChecked;
            }
            set
            {
                if (value == _isChkCustomFollowUsersListChecked)
                    return;
                SetProperty(ref _isChkCustomFollowUsersListChecked, value);
            }
        }

        private string _customFollowUsersList;

        [ProtoMember(38)]
        public string CustomFollowUsersList
        {
            get
            {
                return _customFollowUsersList;
            }
            set
            {
                if (value == _customFollowUsersList)
                    return;
                SetProperty(ref _customFollowUsersList, value);
            }
        }

        private bool _removeAllUsers;

        [ProtoMember(39)]
        public bool RemoveAllFollowUsers
        {
            get
            {
                return _removeAllUsers;
            }
            set
            {
                if (value == _removeAllUsers)
                    return;
                SetProperty(ref _removeAllUsers, value);
            }
        }

        [ProtoMember(40)]
        private List<string> _lstFollowCustomUser;

        public List<string> LstFollowCustomUser
        {
            get
            {
                return _lstFollowCustomUser;
            }
            set
            {
                if (value == _lstFollowCustomUser)
                    return;
                SetProperty(ref _lstFollowCustomUser, value);
            }
        }
        #endregion
    }
}
