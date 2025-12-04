using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace YoutubeDominatorCore.YoutubeModels.EngageModel
{
    [ProtoContract]
    public class DislikeModel : YdModuleSetting
    {
        private string _customPostsList;

        private bool _isChkCustomPostsListChecked;

        private bool _isChkGroupBlackList;

        private bool _isChkPostLikedBySoftwareChecked;

        private bool _isChkPostLikedOutsideSoftwareChecked;

        private bool _isChkPrivateBlackList;

        private bool _isChkSkipBlackListedUser;

        private List<string> _listOfChannels = new List<string>();

        private List<string> _listOfCustomPosts = new List<string>();

        private ObservableCollection<ChannelDestinationSelectModel> _listSelectDestination =
            new ObservableCollection<ChannelDestinationSelectModel>();

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
            ActivitiesPerDay = new RangeUtilities(13, 20),
            ActivitiesPerHour = new RangeUtilities(1, 2),
            ActivitiesPerWeek = new RangeUtilities(80, 120),
            ActivitiesPerJob = new RangeUtilities(1, 2),
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

        [ProtoMember(4)] public override ChannelFilterModel ChannelFilterModel { get; set; } = new ChannelFilterModel();

        [ProtoMember(5)] public override VideoFilterModel VideoFilterModel { get; set; } = new VideoFilterModel();

        [ProtoMember(6)]
        public bool IsChkSkipBlackListedUser
        {
            get => _isChkSkipBlackListedUser;
            set
            {
                if (_isChkSkipBlackListedUser == value) return;
                SetProperty(ref _isChkSkipBlackListedUser, value);
            }
        }

        [ProtoMember(7)]
        public bool IsChkPrivateBlackList
        {
            get => _isChkPrivateBlackList;
            set
            {
                if (_isChkPrivateBlackList == value) return;
                SetProperty(ref _isChkPrivateBlackList, value);
            }
        }

        [ProtoMember(8)]
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
        public bool IsChkPostLikedBySoftwareChecked
        {
            get => _isChkPostLikedBySoftwareChecked;
            set
            {
                if (value == _isChkPostLikedBySoftwareChecked) return;
                SetProperty(ref _isChkPostLikedBySoftwareChecked, value);
            }
        }

        [ProtoMember(11)]
        public bool IsChkPostLikedOutsideSoftwareChecked
        {
            get => _isChkPostLikedOutsideSoftwareChecked;
            set
            {
                if (value == _isChkPostLikedOutsideSoftwareChecked) return;
                SetProperty(ref _isChkPostLikedOutsideSoftwareChecked, value);
            }
        }

        [ProtoMember(12)]
        public bool IsChkCustomPostsListChecked
        {
            get => _isChkCustomPostsListChecked;
            set
            {
                if (_isChkCustomPostsListChecked == value) return;
                SetProperty(ref _isChkCustomPostsListChecked, value);
            }
        }

        [ProtoMember(13)]
        public string CustomPostsList
        {
            get => _customPostsList;
            set
            {
                if (value == _customPostsList)
                    return;
                SetProperty(ref _customPostsList, value);
            }
        }

        [ProtoMember(14)]
        public List<string> ListCustomPosts
        {
            get => _listOfCustomPosts;
            set
            {
                if (value == _listOfCustomPosts)
                    return;
                SetProperty(ref _listOfCustomPosts, value);
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