using CommonServiceLocator;
using DominatorHouseCore.Models;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace QuoraDominatorCore.Models
{
    public interface IUnFollower
    {
        bool IsChkPeopleFollowedBySoftwareChecked { get; set; }
        bool IsChkPeopleFollowedOutsideSoftwareChecked { get; set; }
        bool IsChkAddToBlackList { get; set; }
        bool IsChkEnableAutoFollowUnfollowChecked { get; set; }
        bool IsChkStopUnFollowToolWhenReachChecked { get; set; }
        RangeUtilities StopFollowToolWhenReachValue { get; set; }
        bool IsChkWhenFollowerFollowingsGreater { get; set; }
        int FollowerFollowingsGreaterThanValue { get; set; }
        bool IsChkWhenNoUsersToUnfollow { get; set; }
        bool IsChkStartFollowBetween { get; set; }
        RangeUtilities StartFollowToolBetween { get; set; }
        bool IsChkStartFollowWithoutStoppingUnfollow { get; set; }
        bool IsChkLikeWhileUnFollowing { get; set; }
        bool IsWhoDoNotFollowBackChecked { get; set; }
        bool IsWhoFollowBackChecked { get; set; }
        bool IsUserFollowedBeforeChecked { get; set; }
        int FollowedBeforeDay { get; set; }
        int FollowedBeforeHour { get; set; }
    }

    [ProtoContract]
    public class UnfollowerModel : BindableBase, IUnFollower
    {
        private bool _isAddedToCampaign;

        private bool _isEnableAdvancedUserMode;

        private bool _isGroupList;

        private bool _isPrivateList;

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

        public UnfollowerModel()
        {
            var softwareSettings = InstanceProvider.GetInstance<ISoftwareSettings>();
            _isEnableAdvancedUserMode = softwareSettings.Settings?.IsEnableAdvancedUserMode ?? false;
        }

        [ProtoMember(1)] public UserFilterModel UserFilterModel { get; set; } = new UserFilterModel();

        [ProtoMember(3)] public JobConfiguration JobConfiguration { get; set; } = new JobConfiguration();


        [ProtoMember(23)] public BlacklistSettings Whitelist { get; set; } = new BlacklistSettings();

        public bool IsAccountGrowthActive { get; set; }

        [ProtoMember(26)]
        public bool IsChkPrivateList
        {
            get => _isPrivateList;
            set
            {
                if (_isPrivateList && _isPrivateList == value)
                    return;
                SetProperty(ref _isPrivateList, value);
            }
        }

        [ProtoMember(27)]
        public bool IsChkGroupList
        {
            get => _isGroupList;
            set
            {
                if (_isGroupList && _isGroupList == value)
                    return;
                SetProperty(ref _isGroupList, value);
            }
        }

        [ProtoMember(28)]
        public bool IsEnableAdvancedUserMode
        {
            get => _isEnableAdvancedUserMode;
            set
            {
                if (value == _isEnableAdvancedUserMode)
                    return;
                SetProperty(ref _isEnableAdvancedUserMode, value);
            }
        }


        #region IUnfollowerModel

        private bool _isChkPeopleFollowedBySoftwareCheecked;

        [ProtoMember(5)]
        public bool IsChkPeopleFollowedBySoftwareChecked
        {
            get => _isChkPeopleFollowedBySoftwareCheecked;
            set
            {
                if (value == _isChkPeopleFollowedBySoftwareCheecked) return;
                SetProperty(ref _isChkPeopleFollowedBySoftwareCheecked, value);
            }
        }


        private bool _isChkPeopleFollowedOutsideSoftwareChecked;

        [ProtoMember(6)]
        public bool IsChkPeopleFollowedOutsideSoftwareChecked
        {
            get => _isChkPeopleFollowedOutsideSoftwareChecked;
            set
            {
                if (value == _isChkPeopleFollowedOutsideSoftwareChecked) return;
                SetProperty(ref _isChkPeopleFollowedOutsideSoftwareChecked, value);
            }
        }

        private bool _isChkCustomUsersListChecked;

        [ProtoMember(7)]
        public bool IsChkCustomUsersListChecked
        {
            get => _isChkCustomUsersListChecked;
            set
            {
                if (_isChkCustomUsersListChecked == value) return;

                SetProperty(ref _isChkCustomUsersListChecked, value);
            }
        }


        private bool _isChkAddToBlackList;

        [ProtoMember(8)]
        public bool IsChkAddToBlackList
        {
            get => _isChkAddToBlackList;
            set
            {
                if (_isChkAddToBlackList == value) return;
                SetProperty(ref _isChkAddToBlackList, value);
            }
        }

        private bool _isChkEnableAutoFollowUnfollowChecked;

        [ProtoMember(9)]
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

        [ProtoMember(10)]
        public bool IsChkStopUnFollowToolWhenReachChecked
        {
            get => _isChkStopFollowToolWhenReachChecked;
            set
            {
                if (_isChkStopFollowToolWhenReachChecked == value) return;
                SetProperty(ref _isChkStopFollowToolWhenReachChecked, value);
            }
        }

        private RangeUtilities _stopFollowToolWhenReachValue = new RangeUtilities();

        [ProtoMember(11)]
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

        [ProtoMember(12)]
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

        [ProtoMember(13)]
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

        [ProtoMember(14)]
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

        [ProtoMember(15)]
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

        [ProtoMember(16)]
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

        [ProtoMember(17)]
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

        [ProtoMember(18)]
        public bool IsChkLikeWhileUnFollowing
        {
            get => _isChkLikeWhileUnfollowing;
            set
            {
                if (_isChkLikeWhileUnfollowing == value) return;
                SetProperty(ref _isChkLikeWhileUnfollowing, value);
            }
        }

        private bool _isWhoDoNotFollowBackChecked;

        [ProtoMember(21)]
        public bool IsWhoDoNotFollowBackChecked
        {
            get => _isWhoDoNotFollowBackChecked;
            set
            {
                if (_isWhoDoNotFollowBackChecked == value) return;
                SetProperty(ref _isWhoDoNotFollowBackChecked, value);
            }
        }

        private bool _isWhoFollowBackChecked;

        [ProtoMember(22)]
        public bool IsWhoFollowBackChecked
        {
            get => _isWhoFollowBackChecked;
            set
            {
                if (_isWhoFollowBackChecked == value) return;
                SetProperty(ref _isWhoFollowBackChecked, value);
            }
        }

        private bool _isUserFollowedBeforeChecked;

        [ProtoMember(23)]
        public bool IsUserFollowedBeforeChecked
        {
            get => _isUserFollowedBeforeChecked;
            set
            {
                if (_isUserFollowedBeforeChecked == value) return;
                SetProperty(ref _isUserFollowedBeforeChecked, value);
            }
        }

        private int _followedBeforeDay;

        [ProtoMember(24)]
        public int FollowedBeforeDay
        {
            get => _followedBeforeDay;
            set
            {
                if (_followedBeforeDay == value) return;
                SetProperty(ref _followedBeforeDay, value);
            }
        }

        private int _followedBeforeHour;

        [ProtoMember(25)]
        public int FollowedBeforeHour
        {
            get => _followedBeforeHour;
            set
            {
                if (_followedBeforeHour == value) return;
                SetProperty(ref _followedBeforeHour, value);
            }
        }

        private string _customUsersList;

        [ProtoMember(26)]
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


        private bool _filterInactiveUsers;

        [ProtoMember(27)]
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

        private bool _isChkSkipPrivateBlackList;
        private bool _isChkSkipGroupBlackList;
        public bool IsChkUnFollowerSkipPrivateBlacklist
        {
            get => _isChkSkipPrivateBlackList;
            set
            {
                if (_isChkSkipPrivateBlackList != value)
                    SetProperty(ref _isChkSkipPrivateBlackList, value);
            }
        }

        public bool IsChkUnFollowerSkipGroupBlacklist
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