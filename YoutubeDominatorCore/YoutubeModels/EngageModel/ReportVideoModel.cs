using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace YoutubeDominatorCore.YoutubeModels.EngageModel
{
    [ProtoContract]
    public class ReportVideoModel : YdModuleSetting
    {
        private bool _isChkAllowMultipleTextsOnSamePost;

        private bool _isChkGroupBlackList;

        private bool _isChkPrivateBlackList;

        private bool _isChkSkipBlackListedUser;

        private bool _isChkTextUnique;

        private bool _isPostUniqueTextFromEachAccount = true;

        private bool _isTextOnceFromEachAccount;

        private ObservableCollection<ChannelDestinationSelectModel> _listSelectDestination =
            new ObservableCollection<ChannelDestinationSelectModel>();

        private int _multipleActionCount = 1;

        public JobConfiguration FastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(66, 100),
            ActivitiesPerHour = new RangeUtilities(6, 10),
            ActivitiesPerWeek = new RangeUtilities(400, 600),
            ActivitiesPerJob = new RangeUtilities(8, 12),
            DelayBetweenJobs = new RangeUtilities(85, 127),
            DelayBetweenActivity = new RangeUtilities(18, 36),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(26, 40),
            ActivitiesPerHour = new RangeUtilities(5, 7),
            ActivitiesPerWeek = new RangeUtilities(160, 240),
            ActivitiesPerJob = new RangeUtilities(2, 5),
            DelayBetweenJobs = new RangeUtilities(86, 130),
            DelayBetweenActivity = new RangeUtilities(30, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration SlowSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(13, 20),
            ActivitiesPerHour = new RangeUtilities(4, 6),
            ActivitiesPerWeek = new RangeUtilities(80, 120),
            ActivitiesPerJob = new RangeUtilities(2, 3),
            DelayBetweenJobs = new RangeUtilities(86, 130),
            DelayBetweenActivity = new RangeUtilities(60, 120),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(200, 300),
            ActivitiesPerHour = new RangeUtilities(20, 30),
            ActivitiesPerWeek = new RangeUtilities(1200, 1800),
            ActivitiesPerJob = new RangeUtilities(25, 37),
            DelayBetweenJobs = new RangeUtilities(83, 125),
            DelayBetweenActivity = new RangeUtilities(7, 15),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        [ProtoMember(1)]
        public override ObservableCollection<QueryInfo> SavedQueries { get; set; } =
            new ObservableCollection<QueryInfo>();

        public List<string> ListQueryType { get; set; } = new List<string>();

        [ProtoMember(2)] public override ChannelFilterModel ChannelFilterModel { get; set; } = new ChannelFilterModel();

        [ProtoMember(3)] public override VideoFilterModel VideoFilterModel { get; set; } = new VideoFilterModel();

        [ProtoMember(4)]
        public bool IsChkSkipBlackListedUser
        {
            get => _isChkSkipBlackListedUser;
            set
            {
                if (_isChkSkipBlackListedUser == value) return;
                SetProperty(ref _isChkSkipBlackListedUser, value);
            }
        }

        [ProtoMember(5)]
        public bool IsChkPrivateBlackList
        {
            get => _isChkPrivateBlackList;
            set => SetProperty(ref _isChkPrivateBlackList, value);
        }

        [ProtoMember(6)]
        public bool IsChkGroupBlackList
        {
            get => _isChkGroupBlackList;
            set => SetProperty(ref _isChkGroupBlackList, value);
        }

        [ProtoMember(7)]
        public bool IsChkTextUnique
        {
            get => _isChkTextUnique;
            set => SetProperty(ref _isChkTextUnique, value);
        }

        [ProtoMember(8)]
        public bool IsChkAllowMultipleTextsOnSamePost
        {
            get => _isChkAllowMultipleTextsOnSamePost;

            set => SetProperty(ref _isChkAllowMultipleTextsOnSamePost, value);
        }

        [ProtoMember(9)]
        public ObservableCollection<ManageReportVideosContentModel> ListReportDetailsModel { get; set; } =
            new ObservableCollection<ManageReportVideosContentModel>();

        public ManageReportVideosContentModel ManageReportDetailsModel { get; set; } =
            new ManageReportVideosContentModel();

        /// <summary>
        ///     To hold all destination list which holds all group,page count both selected and total
        /// </summary>
        [ProtoMember(10)]
        public ObservableCollection<ChannelDestinationSelectModel> ListSelectDestination
        {
            get => _listSelectDestination;
            set => SetProperty(ref _listSelectDestination, value);
        }

        [ProtoMember(11)]
        public bool IsPostUniqueTextFromEachAccount
        {
            get => _isPostUniqueTextFromEachAccount;
            set => SetProperty(ref _isPostUniqueTextFromEachAccount, value);
        }

        [ProtoMember(12)]
        public bool IsTextOnceFromEachAccount
        {
            get => _isTextOnceFromEachAccount;
            set => SetProperty(ref _isTextOnceFromEachAccount, value);
        }

        [ProtoMember(13)]
        public int MultipleActionCount
        {
            get => _multipleActionCount;
            set => SetProperty(ref _multipleActionCount, value);
        }
    }
}