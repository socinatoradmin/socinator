using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.Generic;

namespace RedditDominatorCore.RDModel
{
    internal interface IUnSubscribeModel
    {
        CommunityFiltersModel CommunityFiltersModel { get; set; }
        string CustomCommunityList { get; set; }
        bool IsChkCommunitySubscribedBySoftwareChecked { get; set; }
        bool IsChkCommunitySubscribedOutsideSoftwareChecked { get; set; }
        bool IsChkCustomCommunityListChecked { get; set; }
        bool IsChkEnableAutoSubscribeUnSubscribeChecked { get; set; }
        bool IsChkGroupBlackList { get; set; }
        bool IsChkPrivateBlackList { get; set; }
        bool IsChkSkipBlackListedUser { get; set; }
        bool IsChkStopUnSubscribeToolWhenReachChecked { get; set; }
        bool IsChkUnSubscribeUnique { get; set; }
        bool IsCommunitySubscribedBeforeChecked { get; set; }
        List<string> ListQueryType { get; set; }
        List<string> LstCustomCommunity { get; set; }
        RunningTimes RunningTimes { get; set; }
        RangeUtilities StartUnSubscribeToolWhenReach { get; set; }
        RangeUtilities StopUnSubscribeToolWhenReach { get; set; }
        int SubscribedBeforeDay { get; set; }
        int SubscribedBeforeHour { get; set; }
    }

    [ProtoContract]
    public class UnSubscribeModel : ModuleSetting, IUnSubscribeModel
    {
        private string _customCommunityList;

        private bool _isChkCommunitySubscribedBySoftwareChecked;

        private bool _isChkCommunitySubscribedOutsideSoftwareChecked;

        private bool _isChkCustomCommunityListChecked;

        private bool _isChkEnableAutoUnSubscribeUnUnSubscribeChecked;

        private bool _isChkPrivateBlackList;

        private bool _isChkSkipBlackListedUser;

        private bool _isChkStopUnSubscribeToolWhenReachChecked;

        private bool _isCommunitySubscribedBeforeChecked;

        private List<string> _lstCustomCommunity;

        private RangeUtilities _startUnSubscribeToolWhenReach;

        private RangeUtilities _stopUnSubscribeToolWhenReach = new RangeUtilities();

        private int _subscribedBeforeDay;

        private int _subscribedBeforeHour;

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

        public List<string> ListQueryType { get; set; } = new List<string>();

        [ProtoMember(1)]
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
            get => _isChkEnableAutoUnSubscribeUnUnSubscribeChecked;
            set
            {
                if (_isChkEnableAutoUnSubscribeUnUnSubscribeChecked == value) return;
                SetProperty(ref _isChkEnableAutoUnSubscribeUnUnSubscribeChecked, value);
            }
        }

        [ProtoMember(3)]
        public bool IsChkPrivateBlackList
        {
            get => _isChkPrivateBlackList;
            set
            {
                if (_isChkPrivateBlackList == value) return;
                SetProperty(ref _isChkPrivateBlackList, value);
            }
        }

        [ProtoMember(4)]
        public bool IsChkGroupBlackList
        {
            get => _isChkPrivateBlackList;
            set
            {
                if (_isChkPrivateBlackList == value) return;
                SetProperty(ref _isChkPrivateBlackList, value);
            }
        }

        [ProtoMember(5)]
        public override CommunityFiltersModel CommunityFiltersModel { get; set; } = new CommunityFiltersModel();

        [ProtoMember(6)] public RunningTimes RunningTimes { get; set; } = new RunningTimes();

        [ProtoMember(7)] public bool IsChkUnSubscribeUnique { get; set; }

        [ProtoMember(8)]
        public RangeUtilities StopUnSubscribeToolWhenReach
        {
            get => _stopUnSubscribeToolWhenReach;

            set
            {
                if (value == _stopUnSubscribeToolWhenReach)
                    return;
                SetProperty(ref _stopUnSubscribeToolWhenReach, value);
            }
        }

        [ProtoMember(9)]
        public RangeUtilities StartUnSubscribeToolWhenReach
        {
            get => _startUnSubscribeToolWhenReach;
            set
            {
                if (value == _startUnSubscribeToolWhenReach)
                    return;
                SetProperty(ref _startUnSubscribeToolWhenReach, value);
            }
        }

        [ProtoMember(10)]
        public bool IsChkStopUnSubscribeToolWhenReachChecked
        {
            get => _isChkStopUnSubscribeToolWhenReachChecked;
            set
            {
                if (value == _isChkStopUnSubscribeToolWhenReachChecked)
                    return;
                SetProperty(ref _isChkStopUnSubscribeToolWhenReachChecked, value);
            }
        }

        [ProtoMember(11)]
        public bool IsChkCommunitySubscribedBySoftwareChecked
        {
            get => _isChkCommunitySubscribedBySoftwareChecked;
            set => SetProperty(ref _isChkCommunitySubscribedBySoftwareChecked, value);
        }

        [ProtoMember(12)]
        public bool IsChkCommunitySubscribedOutsideSoftwareChecked
        {
            get => _isChkCommunitySubscribedOutsideSoftwareChecked;
            set => SetProperty(ref _isChkCommunitySubscribedOutsideSoftwareChecked, value);
        }

        [ProtoMember(13)]
        public bool IsChkCustomCommunityListChecked
        {
            get => _isChkCustomCommunityListChecked;
            set => SetProperty(ref _isChkCustomCommunityListChecked, value);
        }

        [ProtoMember(14)]
        public string CustomCommunityList
        {
            get => _customCommunityList;
            set => SetProperty(ref _customCommunityList, value);
        }

        [ProtoMember(15)]
        public List<string> LstCustomCommunity
        {
            get => _lstCustomCommunity;
            set
            {
                if (value == _lstCustomCommunity)
                    return;
                SetProperty(ref _lstCustomCommunity, value);
            }
        }

        [ProtoMember(16)]
        public int SubscribedBeforeDay
        {
            get => _subscribedBeforeDay;
            set => SetProperty(ref _subscribedBeforeDay, value);
        }

        [ProtoMember(17)]
        public int SubscribedBeforeHour
        {
            get => _subscribedBeforeHour;
            set => SetProperty(ref _subscribedBeforeHour, value);
        }

        [ProtoMember(18)]
        public bool IsCommunitySubscribedBeforeChecked
        {
            get => _isCommunitySubscribedBeforeChecked;
            set => SetProperty(ref _isCommunitySubscribedBeforeChecked, value);
        }
    }
}