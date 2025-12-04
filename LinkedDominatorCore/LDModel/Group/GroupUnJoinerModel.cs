using System.Collections.Generic;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace LinkedDominatorCore.LDModel
{
    [ProtoContract]
    public class GroupUnJoinerModel : ModuleSetting
    {
        private bool _IsCheckedEnableAutoGroupJoiner;

        private bool _IsCheckedGroupUnJoinerToolGetsTemporaryBlocked;

        private bool _IsCheckedStartGroupJoinerBetween;


        private bool _IsCheckedStartGroupJoinerWhenLimitReach;

        private RangeUtilities _StartGroupJoinerAfter = new RangeUtilities(30, 60);

        private RangeUtilities _StartGroupJoinerWhenLimitReach = new RangeUtilities(8, 10);

        [ProtoMember(5)] private string _UrlInput;

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

        [ProtoMember(1)] public override JobConfiguration JobConfiguration { get; set; } = new JobConfiguration();

        [ProtoMember(2)]
        public bool IsCheckedBySoftware
        {
            get => _IsCheckedBySoftware;
            set
            {
                if (value == _IsCheckedBySoftware) return;
                SetProperty(ref _IsCheckedBySoftware, value);
            }
        }

        [ProtoMember(3)]
        public bool IsCheckedOutSideSoftware
        {
            get => _IsCheckedOutSideSoftware;
            set
            {
                if (value == _IsCheckedOutSideSoftware) return;
                SetProperty(ref _IsCheckedOutSideSoftware, value);
            }
        }

        [ProtoMember(4)]
        public bool IsCheckedCustomGroupList
        {
            get => _IsCheckedCustomGroupList;
            set
            {
                if (value == _IsCheckedCustomGroupList) return;
                SetProperty(ref _IsCheckedCustomGroupList, value);
            }
        }

        public string UrlInput
        {
            get => _UrlInput;
            set
            {
                if (_UrlInput == value)
                    return;
                SetProperty(ref _UrlInput, value);
            }
        }

        [ProtoMember(6)]
        public List<string> UrlList
        {
            get => _UrlList;
            set
            {
                if (value == _UrlList) return;
                SetProperty(ref _UrlList, value);
            }
        }

        [ProtoMember(7)]
        public bool IsCheckedEnableAutoGroupJoiner
        {
            get => _IsCheckedEnableAutoGroupJoiner;
            set
            {
                if (value == _IsCheckedEnableAutoGroupJoiner)
                    return;
                SetProperty(ref _IsCheckedEnableAutoGroupJoiner, value);
            }
        }

        [ProtoMember(8)]
        public bool IsCheckedStartGroupJoinerWhenLimitReach
        {
            get => _IsCheckedStartGroupJoinerWhenLimitReach;
            set
            {
                if (value == _IsCheckedStartGroupJoinerWhenLimitReach)
                    return;
                SetProperty(ref _IsCheckedStartGroupJoinerWhenLimitReach, value);
            }
        }

        [ProtoMember(9)]
        public RangeUtilities StartGroupJoinerWhenLimitReach
        {
            get => _StartGroupJoinerWhenLimitReach;
            set
            {
                if (value == _StartGroupJoinerWhenLimitReach)
                    return;
                SetProperty(ref _StartGroupJoinerWhenLimitReach, value);
            }
        }

        [ProtoMember(10)]
        public bool IsCheckedGroupUnJoinerToolGetsTemporaryBlocked
        {
            get => _IsCheckedGroupUnJoinerToolGetsTemporaryBlocked;
            set
            {
                if (value == _IsCheckedGroupUnJoinerToolGetsTemporaryBlocked)
                    return;
                SetProperty(ref _IsCheckedGroupUnJoinerToolGetsTemporaryBlocked, value);
            }
        }

        [ProtoMember(11)]
        public bool IsCheckedStartGroupJoinerBetween
        {
            get => _IsCheckedStartGroupJoinerBetween;
            set
            {
                if (value == _IsCheckedStartGroupJoinerBetween)
                    return;
                SetProperty(ref _IsCheckedStartGroupJoinerBetween, value);
            }
        }

        [ProtoMember(12)]
        public RangeUtilities StartGroupJoinerAfter
        {
            get => _StartGroupJoinerAfter;
            set
            {
                if (value == _StartGroupJoinerAfter)
                    return;
                SetProperty(ref _StartGroupJoinerAfter, value);
            }
        }

        #region MyRegion

        private bool _IsCheckedBySoftware;
        private bool _IsCheckedOutSideSoftware;
        private bool _IsCheckedCustomGroupList;
        private List<string> _UrlList;

        #endregion
    }
}