using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace LinkedDominatorCore.LDModel
{
    [ProtoContract]
    public class GroupJoinerModel : ModuleSetting
    {
        private bool _IsCheckedEnableAutoGroupUnJoiner;

        private bool _IsCheckedGroupJoinerToolGetsTemporaryBlocked;

        private bool _IsCheckedStartGroupUnJoinerBetween;


        private bool _IsCheckedStartGroupUnJoinerWhenLimitReach;

        private bool _IsStopSendGroupJoinRequestOnFailed;

        private RangeUtilities _StartGroupUnJoinerAfter = new RangeUtilities(30, 60);

        private RangeUtilities _StartGroupUnJoinerWhenLimitReach = new RangeUtilities(8, 10);

        private int _StopSendGroupJoinRequestOnCount = 3;

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

        [ProtoMember(1)]
        public override ObservableCollection<QueryInfo> SavedQueries { get; set; } =
            new ObservableCollection<QueryInfo>();

        public List<string> ListQueryType { get; set; } = new List<string>();

        [ProtoMember(2)] public override JobConfiguration JobConfiguration { get; set; } = new JobConfiguration();

        [ProtoMember(3)]
        public bool IsCheckedEnableAutoGroupUnJoiner
        {
            get => _IsCheckedEnableAutoGroupUnJoiner;
            set
            {
                if (value == _IsCheckedEnableAutoGroupUnJoiner)
                    return;
                SetProperty(ref _IsCheckedEnableAutoGroupUnJoiner, value);
            }
        }

        [ProtoMember(4)]
        public bool IsCheckedStartGroupUnJoinerWhenLimitReach
        {
            get => _IsCheckedStartGroupUnJoinerWhenLimitReach;
            set
            {
                if (value == _IsCheckedStartGroupUnJoinerWhenLimitReach)
                    return;
                SetProperty(ref _IsCheckedStartGroupUnJoinerWhenLimitReach, value);
            }
        }

        [ProtoMember(5)]
        public RangeUtilities StartGroupUnJoinerWhenLimitReach
        {
            get => _StartGroupUnJoinerWhenLimitReach;
            set
            {
                if (value == _StartGroupUnJoinerWhenLimitReach)
                    return;
                SetProperty(ref _StartGroupUnJoinerWhenLimitReach, value);
            }
        }

        [ProtoMember(6)]
        public bool IsCheckedGroupJoinerToolGetsTemporaryBlocked
        {
            get => _IsCheckedGroupJoinerToolGetsTemporaryBlocked;
            set
            {
                if (value == _IsCheckedGroupJoinerToolGetsTemporaryBlocked)
                    return;
                SetProperty(ref _IsCheckedGroupJoinerToolGetsTemporaryBlocked, value);
            }
        }

        [ProtoMember(7)]
        public bool IsCheckedStartGroupUnJoinerBetween
        {
            get => _IsCheckedStartGroupUnJoinerBetween;
            set
            {
                if (value == _IsCheckedStartGroupUnJoinerBetween)
                    return;
                SetProperty(ref _IsCheckedStartGroupUnJoinerBetween, value);
            }
        }

        [ProtoMember(8)]
        public RangeUtilities StartGroupUnJoinerAfter
        {
            get => _StartGroupUnJoinerAfter;
            set
            {
                if (value == _StartGroupUnJoinerAfter)
                    return;
                SetProperty(ref _StartGroupUnJoinerAfter, value);
            }
        }

        [ProtoMember(9)]
        public bool IsStopSendGroupJoinRequestOnFailed
        {
            get => _IsStopSendGroupJoinRequestOnFailed;
            set => SetProperty(ref _IsStopSendGroupJoinRequestOnFailed, value);
        }

        [ProtoMember(10)]
        public int StopSendGroupJoinRequestOnCount
        {
            get => _StopSendGroupJoinRequestOnCount;
            set => SetProperty(ref _StopSendGroupJoinRequestOnCount, value);
        }
    }
}