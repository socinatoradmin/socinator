using DominatorHouseCore.Models;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.Generic;

namespace TumblrDominatorCore.Models
{
    internal interface IUnFollower
    {
        bool IsChkPeopleFollowedBySoftwareChecked { get; set; }
        bool IsChkPeopleFollowedOutsideSoftwareChecked { get; set; }
        bool IsChkCustomUsersListChecked { get; set; }
        bool IsChkAddToBlackList { get; set; }

        bool IsChkSkipWhiteListedUser { get; set; }

        bool IsChkPrivateWhiteListed { get; set; }

        bool IsChkToGroupWhitelist { get; set; }
        bool IsChkAddToGroupBlackList { get; set; }

        bool IsChkAddToPrivateBlackList { get; set; }

        bool IsChkEnableAutoFollowUnfollowChecked { get; set; }

        bool IsChkStopFollowToolWhenReachChecked { get; set; }

        // bool IsChkStopUnFollowWhenReachChecked { get; set; }
        // RangeUtilities StopFollowToolWhenReachValue { get; set; }
        bool IsChkWhenFollowerFollowingsGreater { get; set; }
        int FollowerFollowingsGreaterThanValue { get; set; }
        bool IsChkWhenNoUsersToUnfollow { get; set; }
        bool IsChkStartFollowBetween { get; set; }
        RangeUtilities StartFollowToolBetwen { get; set; }
        bool IsChkStartFollowWithoutStoppingUnfollow { get; set; }
        bool IsChkLikeWhileUnfollowing { get; set; }
        bool IsWhoDoNotFollowBackChecked { get; set; }
        bool IsWhoFollowBackChecked { get; set; }
        bool IsUserFollowedBeforeChecked { get; set; }
        int FollowedBeforeDay { get; set; }
        int FollowedBeforeHour { get; set; }
        bool IsChkStopUnFollowTool { get; set; }
        bool IsChkStartFollowToolStopUnFollow { get; set; }
    }

    [ProtoContract]
    public class UnfollowerModel : ModuleSetting, IUnFollower
    {
        private bool _IsChkStopFollowToolWhenReachChecked;

        public JobConfiguration FastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(133, 200),
            ActivitiesPerHour = new RangeUtilities(13, 20),
            ActivitiesPerWeek = new RangeUtilities(800, 1200),
            ActivitiesPerJob = new RangeUtilities(16, 25),
            DelayBetweenJobs = new RangeUtilities(75, 100),
            DelayBetweenActivity = new RangeUtilities(35, 50)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(66, 100),
            ActivitiesPerHour = new RangeUtilities(6, 10),
            ActivitiesPerWeek = new RangeUtilities(400, 600),
            ActivitiesPerJob = new RangeUtilities(8, 12),
            DelayBetweenJobs = new RangeUtilities(81, 122),
            DelayBetweenActivity = new RangeUtilities(30, 40)
        };

        //public RangeUtilities StopFollowToolWhenReachValue
        //{
        //    get
        //    {
        //        //throw new NotImplementedException();
        //    }

        //    set
        //    {
        //       // throw new NotImplementedException();
        //    }
        //}

        public JobConfiguration SlowSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(16, 25),
            ActivitiesPerHour = new RangeUtilities(2, 3),
            ActivitiesPerWeek = new RangeUtilities(100, 150),
            ActivitiesPerJob = new RangeUtilities(2, 3),
            DelayBetweenJobs = new RangeUtilities(45, 60),
            DelayBetweenActivity = new RangeUtilities(30, 60)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(266, 400),
            ActivitiesPerHour = new RangeUtilities(26, 40),
            ActivitiesPerWeek = new RangeUtilities(1600, 2400),
            ActivitiesPerJob = new RangeUtilities(33, 50),
            DelayBetweenJobs = new RangeUtilities(60, 95),
            DelayBetweenActivity = new RangeUtilities(40, 50)
        };

        [ProtoMember(1)] public new JobConfiguration JobConfiguration { get; set; } = new JobConfiguration();


        [ProtoMember(23)] public BlacklistSettings Whitelist { get; set; } = new BlacklistSettings();

        [ProtoMember(24)] public int MaxDaysSinceLastPost { get; set; }

        [ProtoMember(25)] public bool IgnoreFollowers { get; set; }

        public bool IsWhoNotFollowBackChecked { get; set; }

        public bool IsChkStartFollowToolStopUnFollow
        {
            get => _isChkStartFollowToolStopUnFollow;

            set
            {
                if (value == _isChkStartFollowToolStopUnFollow)
                    return;
                SetProperty(ref _isChkStartFollowToolStopUnFollow, value);
            }
        }

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

        public bool IsChkStopFollowToolWhenReachChecked
        {
            get => _IsChkStopFollowToolWhenReachChecked;

            set
            {
                if (value == _IsChkStopFollowToolWhenReachChecked)
                    return;
                SetProperty(ref _IsChkStopFollowToolWhenReachChecked, value);
            }
        }


        #region IUnfollowerModel

        private bool _isChkPeopleFollowedBySoftwareCheecked;

        [ProtoMember(2)]
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

        [ProtoMember(3)]
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

        [ProtoMember(4)]
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

        [ProtoMember(5)]
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

        [ProtoMember(6)]
        public bool IsChkEnableAutoFollowUnfollowChecked
        {
            get => _isChkEnableAutoFollowUnfollowChecked;
            set
            {
                if (_isChkEnableAutoFollowUnfollowChecked == value) return;
                SetProperty(ref _isChkEnableAutoFollowUnfollowChecked, value);
            }
        } //IsChkStopUnFollowToolWhenReachChecked

        private bool _isChkStopUnFollowToolWhenReachChecked;

        [ProtoMember(7)]
        public bool IsChkStopUnFollowToolWhenReachChecked
        {
            get => _isChkStopUnFollowToolWhenReachChecked;
            set
            {
                if (_isChkStopUnFollowToolWhenReachChecked == value) return;
                SetProperty(ref _isChkStopUnFollowToolWhenReachChecked, value);
            }
        }

        [ProtoMember(8)]
        public bool IsChkAddToPrivateBlackList
        {
            get => _isChkAddToPrivateBlackList;
            set
            {
                if (value == _isChkAddToPrivateBlackList) return;
                SetProperty(ref _isChkAddToPrivateBlackList, value);
            }
        }


        private RangeUtilities _stopUnFollowToolWhenReachValue = new RangeUtilities();

        [ProtoMember(8)]
        public RangeUtilities StopUnFollowToolWhenReachValue
        {
            get => _stopUnFollowToolWhenReachValue;
            set
            {
                if (_stopUnFollowToolWhenReachValue == value) return;
                SetProperty(ref _stopUnFollowToolWhenReachValue, value);
            }
        }


        private bool _isChkWhenFollowerFollowingsGreater;

        [ProtoMember(9)]
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

        [ProtoMember(10)]
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

        [ProtoMember(11)]
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

        [ProtoMember(12)]
        public bool IsChkStartFollowBetween
        {
            get => _isChkStartFollowBetween;
            set
            {
                if (_isChkStartFollowBetween == value) return;
                SetProperty(ref _isChkStartFollowBetween, value);
            }
        }

        private RangeUtilities _startFollowToolBetwen = new RangeUtilities();

        [ProtoMember(13)]
        public RangeUtilities StartFollowToolBetwen
        {
            get => _startFollowToolBetwen;
            set
            {
                if (_startFollowToolBetwen == value) return;
                SetProperty(ref _startFollowToolBetwen, value);
            }
        }

        private bool _isChkStartFollowWithoutStoppingUnfollow;

        [ProtoMember(14)]
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

        [ProtoMember(15)]
        public bool IsChkLikeWhileUnfollowing
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

        [ProtoMember(27)] public bool FilterInactiveUser;

        public bool FilterInactiveUsers
        {
            get => FilterInactiveUser;
            set
            {
                if (value == FilterInactiveUser)
                    return;
                SetProperty(ref FilterInactiveUser, value);
            }
        }


        private List<string> _lstcustomusers = new List<string>();

        [ProtoMember(28)]
        public List<string> LstCustomusers
        {
            get => _lstcustomusers;
            set
            {
                if (value == _lstcustomusers)
                    return;
                SetProperty(ref _lstcustomusers, value);
            }
        }

        [ProtoMember(29)]
        public bool IsChkAddToGroupBlackList
        {
            get => _isChkAddToGroupBlackList;

            set
            {
                if (value == _isChkAddToGroupBlackList)
                    return;
                SetProperty(ref _isChkAddToGroupBlackList, value);
            }
        }


        private bool _isChkSkipWhiteListedUser;
        private bool _isChkPrivateWhiteListed;
        private bool _isChkToGroupWhitelist;
        private bool _isChkAddToPrivateBlackList; //IsChkStartFollowToolStopUnFollow
        private bool _isChkAddToGroupBlackList;

        private bool _isChkStartFollowToolStopUnFollow;
        private bool _isChkStopUnFollowTool;


        public bool IsChkSkipWhiteListedUser
        {
            get => _isChkSkipWhiteListedUser;

            set
            {
                if (value == _isChkSkipWhiteListedUser)
                    return;
                SetProperty(ref _isChkSkipWhiteListedUser, value);
            }
        }

        public bool IsChkPrivateWhiteListed
        {
            get => _isChkPrivateWhiteListed;

            set
            {
                if (value == _isChkPrivateWhiteListed)
                    return;
                SetProperty(ref _isChkPrivateWhiteListed, value);
            }
        }

        public bool IsChkToGroupWhitelist
        {
            get => _isChkToGroupWhitelist;

            set
            {
                if (value == _isChkToGroupWhitelist)
                    return;
                SetProperty(ref _isChkToGroupWhitelist, value);
            }
        }

        //public bool IsChkAddToPrivateBlackList
        //{
        //    get
        //    {
        //        return _isChkAddToPrivateBlackList;
        //    }

        //    set
        //    {
        //        if (value == _isChkAddToPrivateBlackList)
        //            return;
        //        SetProperty(ref _isChkAddToPrivateBlackList, value);
        //    }
        //}

        #endregion
    }
}