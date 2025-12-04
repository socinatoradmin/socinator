using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace YoutubeDominatorCore.YoutubeModels.ScraperModel
{
    [ProtoContract]
    public class ChannelScraperModel : YdModuleSetting
    {
        private bool _isCheckScrapeNCommenters;

        private bool _isCheckSkipTheCommentersCommentHaveLessThanNLikes;

        private bool _isChkGroupBlackList;

        private bool _isChkPrivateBlackList;


        private bool _isChkSkipBlackListedUser;

        private List<string> _listOfChannels = new List<string>();

        private ObservableCollection<ChannelDestinationSelectModel> _listSelectDestination =
            new ObservableCollection<ChannelDestinationSelectModel>();

        private RangeUtilities _scrapeNCommenters = new RangeUtilities(4, 8);
        private int _skipTheCommentersCommentHaveLessThanNLikes = 5;

        public JobConfiguration FastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(400, 600),
            ActivitiesPerHour = new RangeUtilities(40, 60),
            ActivitiesPerWeek = new RangeUtilities(2400, 3600),
            ActivitiesPerJob = new RangeUtilities(50, 75),
            DelayBetweenJobs = new RangeUtilities(60, 90),
            DelayBetweenActivity = new RangeUtilities(18, 36),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(266, 400),
            ActivitiesPerHour = new RangeUtilities(26, 40),
            ActivitiesPerWeek = new RangeUtilities(1600, 2400),
            ActivitiesPerJob = new RangeUtilities(33, 50),
            DelayBetweenJobs = new RangeUtilities(56, 85),
            DelayBetweenActivity = new RangeUtilities(30, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };


        public JobConfiguration SlowSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(66, 100),
            ActivitiesPerHour = new RangeUtilities(6, 10),
            ActivitiesPerWeek = new RangeUtilities(400, 600),
            ActivitiesPerJob = new RangeUtilities(8, 12),
            DelayBetweenJobs = new RangeUtilities(73, 110),
            DelayBetweenActivity = new RangeUtilities(60, 120),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(533, 800),
            ActivitiesPerHour = new RangeUtilities(53, 80),
            ActivitiesPerWeek = new RangeUtilities(3200, 4800),
            ActivitiesPerJob = new RangeUtilities(66, 100),
            DelayBetweenJobs = new RangeUtilities(73, 110),
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
            set
            {
                if (_isChkPrivateBlackList == value) return;
                SetProperty(ref _isChkPrivateBlackList, value);
            }
        }

        [ProtoMember(6)]
        public bool IsChkGroupBlackList
        {
            get => _isChkGroupBlackList;
            set
            {
                if (_isChkGroupBlackList == value) return;
                SetProperty(ref _isChkGroupBlackList, value);
            }
        }

        /// <summary>
        ///     To hold all destination list which holds all group,page count both selected and total
        /// </summary>
        [ProtoMember(7)]
        public ObservableCollection<ChannelDestinationSelectModel> ListSelectDestination
        {
            get => _listSelectDestination;
            set
            {
                if (_listSelectDestination == value) return;
                SetProperty(ref _listSelectDestination, value);
            }
        }

        [ProtoMember(8)]
        public List<string> ListOfChannels
        {
            get => _listOfChannels;
            set
            {
                if (_listOfChannels == value) return;
                SetProperty(ref _listOfChannels, value);
            }
        }

        [ProtoMember(9)]
        public bool IsCheckScrapeNCommenters
        {
            get => _isCheckScrapeNCommenters;
            set => SetProperty(ref _isCheckScrapeNCommenters, value);
        }

        [ProtoMember(10)]
        public RangeUtilities ScrapeNCommenters
        {
            get => _scrapeNCommenters;
            set => SetProperty(ref _scrapeNCommenters, value);
        }

        [ProtoMember(11)]
        public bool IsCheckSkipTheCommentersCommentHaveLessThanNLikes
        {
            get => _isCheckSkipTheCommentersCommentHaveLessThanNLikes;
            set => SetProperty(ref _isCheckSkipTheCommentersCommentHaveLessThanNLikes, value);
        }

        [ProtoMember(12)]
        public int SkipTheCommentersCommentHaveLessThanNLikes
        {
            get => _skipTheCommentersCommentHaveLessThanNLikes;
            set => SetProperty(ref _skipTheCommentersCommentHaveLessThanNLikes, value);
        }

        public RunningTimes RunningTimes { get; set; } = new RunningTimes();
    }
}