using System.Collections.Generic;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace PinDominatorCore.PDModel
{
    [ProtoContract]
    public class UnfollowerModel : ModuleSetting, IGeneralSettings
    {
        private string _customUsers;

        private string _customUsersList;

        private int _followedBeforeDay;

        private int _followedBeforeHour;

        private int _followerFollowingsGreaterThanValue = 1;      

        private bool _isAddedToCampaign;

        private bool _isChkAddToBlackList;

        private bool _isChkAddToGroupBlackList;

        private bool _isChkAddToPrivateBlackList;

        private bool _isChkCustomUsersListChecked;

        private bool _isChkEnableAutoFollowUnfollowChecked;

        private bool _isChkPeopleFollowedBySoftwareChecked;


        private bool _isChkPeopleFollowedOutsideSoftwareChecked;

        private bool _isChkSkipWhiteListedUser;

        private bool _isChkStartFollowBetween;

        private bool _isChkStartFollowStopUnfollowTool;

        private bool _isChkStartFollowWithoutStoppingUnfollow;

        private bool _isChkStopUnFollowTool;

        private bool _isChkStopUnFollowToolWhenReachChecked;

        private string _isChkTryWhileUnfollowing;

        private bool _isChkUseGroupWhiteList;

        private bool _isChkUsePrivateWhiteList;


        private bool _isChkWhenFollowerFollowingsGreater;

        private bool _isChkWhenNoUsersToUnfollow;

        private bool _isUserFollowedBeforeChecked;

        private bool _isWhoDoNotFollowBackChecked;

        private bool _isWhoFollowBackChecked;

        private List<string> _listOfCustomUsers = new List<string>();

        private RangeUtilities _startFollowToolBetwen = new RangeUtilities();

        private RangeUtilities _stopUnFollowToolWhenReachValue = new RangeUtilities(1, 1);

        private string _tryWhileUnfollowingText;

        public JobConfiguration SlowSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(10, 15),
            ActivitiesPerHour = new RangeUtilities(1, 2),
            ActivitiesPerWeek = new RangeUtilities(60, 90),
            ActivitiesPerJob = new RangeUtilities(1, 2),
            DelayBetweenJobs = new RangeUtilities(88, 133),
            DelayBetweenActivity = new RangeUtilities(30, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(27, 40),
            ActivitiesPerHour = new RangeUtilities(3, 4),
            ActivitiesPerWeek = new RangeUtilities(160, 240),
            ActivitiesPerJob = new RangeUtilities(3, 5),
            DelayBetweenJobs = new RangeUtilities(87, 131),
            DelayBetweenActivity = new RangeUtilities(23, 45),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration FastSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(53, 80),
            ActivitiesPerHour = new RangeUtilities(5, 8),
            ActivitiesPerWeek = new RangeUtilities(320, 480),
            ActivitiesPerJob = new RangeUtilities(7, 10),
            DelayBetweenJobs = new RangeUtilities(87, 130),
            DelayBetweenActivity = new RangeUtilities(15, 30),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration SuperfastSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(67, 100),
            ActivitiesPerHour = new RangeUtilities(7, 10),
            ActivitiesPerWeek = new RangeUtilities(400, 600),
            ActivitiesPerJob = new RangeUtilities(8, 12),
            DelayBetweenJobs = new RangeUtilities(88, 132),
            DelayBetweenActivity = new RangeUtilities(8, 15),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

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

        [ProtoMember(10)]
        public bool IsChkStopUnFollowToolWhenReachChecked
        {
            get => _isChkStopUnFollowToolWhenReachChecked;
            set
            {
                if (_isChkStopUnFollowToolWhenReachChecked == value) return;
                SetProperty(ref _isChkStopUnFollowToolWhenReachChecked, value);
            }
        }

        [ProtoMember(11)]
        public RangeUtilities StopUnFollowToolWhenReachValue
        {
            get => _stopUnFollowToolWhenReachValue;
            set
            {
                if (_stopUnFollowToolWhenReachValue == value) return;
                SetProperty(ref _stopUnFollowToolWhenReachValue, value);
            }
        }

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

        [ProtoMember(16)]
        public RangeUtilities StartFollowToolBetwen
        {
            get => _startFollowToolBetwen;
            set
            {
                if (_startFollowToolBetwen == value) return;
                SetProperty(ref _startFollowToolBetwen, value);
            }
        }

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

        [ProtoMember(19)]
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

        [ProtoMember(3)]
        public bool IsChkPeopleFollowedBySoftwareChecked
        {
            get => _isChkPeopleFollowedBySoftwareChecked;
            set
            {
                if (value == _isChkPeopleFollowedBySoftwareChecked) return;
                SetProperty(ref _isChkPeopleFollowedBySoftwareChecked, value);
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
                if (_isWhoDoNotFollowBackChecked == value) return;
                SetProperty(ref _isWhoDoNotFollowBackChecked, value);
            }
        }

        [ProtoMember(7)]
        public bool IsWhoFollowBackChecked
        {
            get => _isWhoFollowBackChecked;
            set
            {
                if (_isWhoFollowBackChecked == value) return;
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
            set
            {
                if (value == _customUsers)
                    return;
                SetProperty(ref _customUsers, value);
            }
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

        [ProtoMember(13)]
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

        [ProtoMember(14)]
        public string IsChkTryWhileUnfollowing
        {
            get => _isChkTryWhileUnfollowing;
            set
            {
                if (value == _isChkTryWhileUnfollowing)
                    return;
                SetProperty(ref _isChkTryWhileUnfollowing, value);
            }
        }

        [ProtoMember(15)]
        public string TryWhileUnfollowingText
        {
            get => _tryWhileUnfollowingText;
            set
            {
                if (value == _tryWhileUnfollowingText)
                    return;
                SetProperty(ref _tryWhileUnfollowingText, value);
            }
        }

        [ProtoMember(16)]
        public bool IsChkAddToPrivateBlackList
        {
            get => _isChkAddToPrivateBlackList;
            set
            {
                if (_isChkAddToPrivateBlackList == value) return;
                SetProperty(ref _isChkAddToPrivateBlackList, value);
            }
        }

        [ProtoMember(17)]
        public bool IsChkAddToGroupBlackList
        {
            get => _isChkAddToGroupBlackList;
            set
            {
                if (_isChkAddToGroupBlackList == value) return;
                SetProperty(ref _isChkAddToGroupBlackList, value);
            }
        }

        [ProtoMember(18)]
        public bool IsChkSkipWhiteListedUser
        {
            get => _isChkSkipWhiteListedUser;
            set
            {
                if (_isChkSkipWhiteListedUser == value) return;
                SetProperty(ref _isChkSkipWhiteListedUser, value);
            }
        }

        [ProtoMember(19)]
        public bool IsChkUsePrivateWhiteList
        {
            get => _isChkUsePrivateWhiteList;
            set
            {
                if (_isChkUsePrivateWhiteList == value) return;
                SetProperty(ref _isChkUsePrivateWhiteList, value);
            }
        }

        [ProtoMember(20)]
        public bool IsChkUseGroupWhiteList
        {
            get => _isChkUseGroupWhiteList;
            set
            {
                if (_isChkUseGroupWhiteList == value) return;
                SetProperty(ref _isChkUseGroupWhiteList, value);
            }
        }

        [ProtoMember(21)]
        public bool IsChkStartFollowToolStopUnFollow
        {
            get => _isChkStartFollowStopUnfollowTool;

            set
            {
                if (value == _isChkStartFollowStopUnfollowTool)
                    return;
                SetProperty(ref _isChkStartFollowStopUnfollowTool, value);
            }
        }

        [ProtoMember(58)]
        public bool IsChkStopUnFollowTool
        {
            get => _isChkStopUnFollowTool;

            set
            {
                if (value == _isChkStopUnFollowTool)
                    return;
                SetProperty(ref _isChkStopUnFollowTool, value);
            }
        }
      
    }
}