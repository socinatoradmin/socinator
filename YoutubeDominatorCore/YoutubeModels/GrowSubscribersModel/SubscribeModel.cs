using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace YoutubeDominatorCore.YoutubeModels.GrowSubscribersModel
{
    [ProtoContract]
    public class SubscribeModel : YdModuleSetting
    {
        private bool _isCheckSkipTheCommentersCommentHaveLessThanNLikes;

        private bool _isCheckSubscribeNCommenters;

        private bool _isChkGroupBlackList;

        private bool _isChkPrivateBlackList;

        private bool _isChkSkipBlackListedUser;

        private List<string> _listOfChannels = new List<string>();

        private ObservableCollection<ChannelDestinationSelectModel> _listSelectDestination =
            new ObservableCollection<ChannelDestinationSelectModel>();

        private int _skipTheCommentersCommentHaveLessThanNLikes = 5;
        private RangeUtilities _subscribeNCommenters = new RangeUtilities(4, 8);

        private bool _uniqueSubscribe;

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

        [ProtoMember(4)] public override ChannelFilterModel ChannelFilterModel { get; set; } = new ChannelFilterModel();

        [ProtoMember(5)] public override VideoFilterModel VideoFilterModel { get; set; } = new VideoFilterModel();

        [ProtoMember(6)]
        public bool UniqueSubscribe
        {
            get => _uniqueSubscribe;
            set
            {
                if (value == _uniqueSubscribe)
                    return;
                SetProperty(ref _uniqueSubscribe, value);
            }
        }

        [ProtoMember(7)]
        public bool IsChkSkipBlackListedUser
        {
            get => _isChkSkipBlackListedUser;
            set
            {
                if (_isChkSkipBlackListedUser == value) return;
                SetProperty(ref _isChkSkipBlackListedUser, value);
            }
        }

        [ProtoMember(8)]
        public bool IsChkPrivateBlackList
        {
            get => _isChkPrivateBlackList;
            set
            {
                if (_isChkPrivateBlackList == value) return;
                SetProperty(ref _isChkPrivateBlackList, value);
            }
        }

        [ProtoMember(9)]
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
        [ProtoMember(10)]
        public ObservableCollection<ChannelDestinationSelectModel> ListSelectDestination
        {
            get => _listSelectDestination;
            set
            {
                if (_listSelectDestination == value) return;
                SetProperty(ref _listSelectDestination, value);
            }
        }

        [ProtoMember(11)]
        public List<string> ListOfChannels
        {
            get => _listOfChannels;
            set
            {
                if (_listOfChannels == value) return;
                SetProperty(ref _listOfChannels, value);
            }
        }

        [ProtoMember(12)]
        public bool IsCheckSubscribeNCommenters
        {
            get => _isCheckSubscribeNCommenters;
            set => SetProperty(ref _isCheckSubscribeNCommenters, value);
        }

        [ProtoMember(13)]
        public RangeUtilities SubscribeNCommenters
        {
            get => _subscribeNCommenters;
            set => SetProperty(ref _subscribeNCommenters, value);
        }

        [ProtoMember(14)]
        public bool IsCheckSkipTheCommentersCommentHaveLessThanNLikes
        {
            get => _isCheckSkipTheCommentersCommentHaveLessThanNLikes;
            set => SetProperty(ref _isCheckSkipTheCommentersCommentHaveLessThanNLikes, value);
        }

        [ProtoMember(15)]
        public int SkipTheCommentersCommentHaveLessThanNLikes
        {
            get => _skipTheCommentersCommentHaveLessThanNLikes;
            set => SetProperty(ref _skipTheCommentersCommentHaveLessThanNLikes, value);
        }

        public RunningTimes RunningTimes { get; set; } = new RunningTimes();
    }
}