using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RedditDominatorCore.RDModel
{
    public class DownvoteModel : ModuleSetting
    {
        private bool _downvotePostOnQueryWithLimit;

        private bool _isChkGroupBlackList;

        private bool _isChkPrivateBlackList;

        private bool _isChkSkipBlackListedUser;

        private RangeUtilities _numberOfDownvotePostOnQuery = new RangeUtilities(1, 5);

        public JobConfiguration FastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(213, 320),
            ActivitiesPerHour = new RangeUtilities(21, 32),
            ActivitiesPerWeek = new RangeUtilities(1280, 1920),
            ActivitiesPerJob = new RangeUtilities(27, 40),
            DelayBetweenJobs = new RangeUtilities(74, 111),
            DelayBetweenActivity = new RangeUtilities(40, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(120, 180),
            ActivitiesPerHour = new RangeUtilities(12, 18),
            ActivitiesPerWeek = new RangeUtilities(720, 1080),
            ActivitiesPerJob = new RangeUtilities(15, 23),
            DelayBetweenJobs = new RangeUtilities(75, 113),
            DelayBetweenActivity = new RangeUtilities(50, 90),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration SlowSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(53, 80),
            ActivitiesPerHour = new RangeUtilities(5, 8),
            ActivitiesPerWeek = new RangeUtilities(320, 480),
            ActivitiesPerJob = new RangeUtilities(7, 10),
            DelayBetweenJobs = new RangeUtilities(77, 115),
            DelayBetweenActivity = new RangeUtilities(60, 120),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration SuperfastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(533, 800),
            ActivitiesPerHour = new RangeUtilities(53, 80),
            ActivitiesPerWeek = new RangeUtilities(3200, 4800),
            ActivitiesPerJob = new RangeUtilities(67, 100),
            DelayBetweenJobs = new RangeUtilities(73, 110),
            DelayBetweenActivity = new RangeUtilities(20, 30),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        [ProtoMember(1)] public override JobConfiguration JobConfiguration { get; set; }

        [ProtoMember(2)]
        public override ObservableCollection<QueryInfo> SavedQueries { get; set; } =
            new ObservableCollection<QueryInfo>();

        public List<string> ListQueryType { get; set; } = new List<string>();

        [ProtoMember(3)]
        public bool IsChkSkipBlackListedUser
        {
            get => _isChkSkipBlackListedUser;
            set
            {
                if (_isChkSkipBlackListedUser == value) return;
                SetProperty(ref _isChkSkipBlackListedUser, value);
            }
        }

        [ProtoMember(4)]
        public bool IsChkPrivateBlackList
        {
            get => _isChkPrivateBlackList;
            set
            {
                if (_isChkPrivateBlackList == value) return;
                SetProperty(ref _isChkPrivateBlackList, value);
            }
        }

        [ProtoMember(5)]
        public bool IsChkGroupBlackList
        {
            get => _isChkGroupBlackList;
            set
            {
                if (_isChkGroupBlackList == value) return;
                SetProperty(ref _isChkGroupBlackList, value);
            }
        }

        [ProtoMember(6)] public override bool IschkUniqueUserForAccount { get; set; } = false;

        [ProtoMember(7)] public override bool IschkUniquePostForCampaign { get; set; } = false;

        [ProtoMember(8)] public override bool IschkUniqueUserForCampaign { get; set; } = false;

        [ProtoMember(9)] public override PostFilterModel PostFilterModel { get; set; } = new PostFilterModel();

        [ProtoMember(10)]
        public bool DownvotePostOnQueryWithLimit
        {
            get => _downvotePostOnQueryWithLimit;
            set
            {
                if (_downvotePostOnQueryWithLimit == value)
                    return;
                SetProperty(ref _downvotePostOnQueryWithLimit, value);
            }
        }

        [ProtoMember(11)]
        public RangeUtilities NumberOfDownvotePostOnQuery
        {
            get => _numberOfDownvotePostOnQuery;
            set
            {
                if (_numberOfDownvotePostOnQuery == value)
                    return;
                SetProperty(ref _numberOfDownvotePostOnQuery, value);
            }
        }
    }
}