using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace YoutubeDominatorCore.YoutubeModels.GrowSubscribersModel
{
    [ProtoContract]
    public class UnsubscribeModel : YdModuleSetting
    {
        private string _customChannelsList;

        private bool _isChkChannelSubscribedBySoftwareChecked;

        private bool _isChkChannelSubscribedOutsideSoftwareChecked;

        private bool _isChkCustomChannelsListChecked;

        private bool _isChkGroupBlackList;

        private bool _isChkPrivateBlackList;

        private bool _isChkSkipBlackListedUser;

        private List<string> _listOfChannels = new List<string>();

        private List<string> _listOfCustomChannels = new List<string>();

        private ObservableCollection<ChannelDestinationSelectModel> _listSelectDestination =
            new ObservableCollection<ChannelDestinationSelectModel>();

        private bool _uniqueSubscribe;

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
            ActivitiesPerDay = new RangeUtilities(33, 50),
            ActivitiesPerHour = new RangeUtilities(3, 5),
            ActivitiesPerWeek = new RangeUtilities(200, 300),
            ActivitiesPerJob = new RangeUtilities(4, 6),
            DelayBetweenJobs = new RangeUtilities(85, 128),
            DelayBetweenActivity = new RangeUtilities(30, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration SlowSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(16, 25),
            ActivitiesPerHour = new RangeUtilities(1, 2),
            ActivitiesPerWeek = new RangeUtilities(100, 150),
            ActivitiesPerJob = new RangeUtilities(2, 3),
            DelayBetweenJobs = new RangeUtilities(85, 128),
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

        [ProtoMember(10)]
        public bool IsChkChannelSubscribedBySoftwareChecked
        {
            get => _isChkChannelSubscribedBySoftwareChecked;
            set
            {
                if (value == _isChkChannelSubscribedBySoftwareChecked) return;
                SetProperty(ref _isChkChannelSubscribedBySoftwareChecked, value);
            }
        }

        [ProtoMember(11)]
        public bool IsChkChannelSubscribedOutsideSoftwareChecked
        {
            get => _isChkChannelSubscribedOutsideSoftwareChecked;
            set
            {
                if (value == _isChkChannelSubscribedOutsideSoftwareChecked) return;
                SetProperty(ref _isChkChannelSubscribedOutsideSoftwareChecked, value);
            }
        }

        [ProtoMember(12)]
        public bool IsChkCustomChannelsListChecked
        {
            get => _isChkCustomChannelsListChecked;
            set
            {
                if (_isChkCustomChannelsListChecked == value) return;
                SetProperty(ref _isChkCustomChannelsListChecked, value);
            }
        }

        [ProtoMember(13)]
        public string CustomChannelsList
        {
            get => _customChannelsList;
            set
            {
                if (value == _customChannelsList)
                    return;
                SetProperty(ref _customChannelsList, value);
            }
        }

        [ProtoMember(14)]
        public List<string> ListCustomChannels
        {
            get => _listOfCustomChannels;
            set
            {
                if (value == _listOfCustomChannels)
                    return;
                SetProperty(ref _listOfCustomChannels, value);
            }
        }

        /// <summary>
        ///     To hold all destination list which holds all group,page count both selected and total
        /// </summary>
        [ProtoMember(15)]
        public ObservableCollection<ChannelDestinationSelectModel> ListSelectDestination
        {
            get => _listSelectDestination;
            set
            {
                if (_listSelectDestination == value) return;
                SetProperty(ref _listSelectDestination, value);
            }
        }

        [ProtoMember(16)]
        public List<string> ListOfChannels
        {
            get => _listOfChannels;
            set
            {
                if (_listOfChannels == value) return;
                SetProperty(ref _listOfChannels, value);
            }
        }

        public RunningTimes RunningTimes { get; set; } = new RunningTimes();
    }
}