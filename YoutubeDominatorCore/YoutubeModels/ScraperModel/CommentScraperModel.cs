using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace YoutubeDominatorCore.YoutubeModels.ScraperModel
{
    [ProtoContract]
    public class CommentScraperModel : YdModuleSetting
    {
        private bool _isCheckScrapeNComments;

        private bool _isChkGroupBlackList;

        private bool _isChkPrivateBlackList;

        private bool _isChkSkipBlackListedUser;

        private List<string> _listOfChannels = new List<string>();

        private ObservableCollection<ChannelDestinationSelectModel> _listSelectDestination =
            new ObservableCollection<ChannelDestinationSelectModel>();

        private RangeUtilities _scrapeNComments = new RangeUtilities(4, 8);

        public JobConfiguration FastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(133, 200),
            ActivitiesPerHour = new RangeUtilities(13, 20),
            ActivitiesPerWeek = new RangeUtilities(800, 1200),
            ActivitiesPerJob = new RangeUtilities(16, 25),
            DelayBetweenJobs = new RangeUtilities(80, 120),
            DelayBetweenActivity = new RangeUtilities(18, 36),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(66, 100),
            ActivitiesPerHour = new RangeUtilities(6, 10),
            ActivitiesPerWeek = new RangeUtilities(400, 600),
            ActivitiesPerJob = new RangeUtilities(8, 12),
            DelayBetweenJobs = new RangeUtilities(81, 122),
            DelayBetweenActivity = new RangeUtilities(30, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration SlowSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(33, 50),
            ActivitiesPerHour = new RangeUtilities(3, 5),
            ActivitiesPerWeek = new RangeUtilities(200, 300),
            ActivitiesPerJob = new RangeUtilities(4, 6),
            DelayBetweenJobs = new RangeUtilities(81, 122),
            DelayBetweenActivity = new RangeUtilities(60, 120),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(266, 400),
            ActivitiesPerHour = new RangeUtilities(26, 40),
            ActivitiesPerWeek = new RangeUtilities(1600, 2400),
            ActivitiesPerJob = new RangeUtilities(33, 50),
            DelayBetweenJobs = new RangeUtilities(81, 122),
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
        public ObservableCollection<ManageCommentModel> LstDisplayManageCommentModel { get; set; } =
            new ObservableCollection<ManageCommentModel>();

        [ProtoMember(5)]
        public bool IsChkSkipBlackListedUser
        {
            get => _isChkSkipBlackListedUser;
            set
            {
                if (_isChkSkipBlackListedUser == value) return;
                SetProperty(ref _isChkSkipBlackListedUser, value);
            }
        }

        [ProtoMember(6)]
        public bool IsChkPrivateBlackList
        {
            get => _isChkPrivateBlackList;
            set
            {
                if (_isChkPrivateBlackList == value) return;
                SetProperty(ref _isChkPrivateBlackList, value);
            }
        }

        [ProtoMember(7)]
        public bool IsChkGroupBlackList
        {
            get => _isChkGroupBlackList;
            set => SetProperty(ref _isChkGroupBlackList, value);
        }

        /// <summary>
        ///     To hold all destination list which holds all group,page count both selected and total
        /// </summary>
        [ProtoMember(8)]
        public ObservableCollection<ChannelDestinationSelectModel> ListSelectDestination
        {
            get => _listSelectDestination;
            set => SetProperty(ref _listSelectDestination, value);
        }

        [ProtoMember(9)]
        public List<string> ListOfChannels
        {
            get => _listOfChannels;
            set => SetProperty(ref _listOfChannels, value);
        }

        [ProtoMember(10)]
        public bool IsCheckScrapeNComments
        {
            get => _isCheckScrapeNComments;
            set => SetProperty(ref _isCheckScrapeNComments, value);
        }

        [ProtoMember(11)]
        public RangeUtilities ScrapeNComments
        {
            get => _scrapeNComments;
            set => SetProperty(ref _scrapeNComments, value);
        }
    }
}