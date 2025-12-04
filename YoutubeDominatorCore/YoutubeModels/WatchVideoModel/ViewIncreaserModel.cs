using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace YoutubeDominatorCore.YoutubeModels.WatchVideoModel
{
    [ProtoContract]
    public class ViewIncreaserModel : YdModuleSetting
    {
        private bool _cefSharpSelected;

        private bool _isBrowserVisibilityAvailable;

        private bool _isBrowserHidden = true;

        private bool _isBrowserVisible;

        private bool _isChkGroupBlackList;

        private bool _isChkPrivateBlackList;

        private bool _isChkRepeatVideo;

        private bool _isChkSkipBlackListedUser;

        private bool _isChkWatchVideoBetweenPercentage;


        private bool _isChkWatchVideoBetweentSeconds;

        private List<string> _listOfChannels = new List<string>();

        private ObservableCollection<ChannelDestinationSelectModel> _listSelectDestination =
            new ObservableCollection<ChannelDestinationSelectModel>();

        private bool _mozillaSelected = true;

        private RangeUtilities _repeatWatchAfterMinutes = new RangeUtilities(2, 5);

        private bool _runEveryday;

        private bool _skipAd;

        private RangeUtilities _stopWatchVideoBetweenPercentage = new RangeUtilities(30, 60);


        private RangeUtilities _stopWatchVideoBetweentSeconds = new RangeUtilities(30, 60);

        private int _viewCountForRepeatVideo;

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
            ActivitiesPerDay = new RangeUtilities(133, 200),
            ActivitiesPerHour = new RangeUtilities(13, 20),
            ActivitiesPerWeek = new RangeUtilities(800, 1200),
            ActivitiesPerJob = new RangeUtilities(16, 25),
            DelayBetweenJobs = new RangeUtilities(56, 85),
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

        [ProtoMember(7)]
        public bool IsChkWatchVideoBetweentSeconds
        {
            get => _isChkWatchVideoBetweentSeconds;
            set
            {
                if (value == _isChkWatchVideoBetweentSeconds) return;

                if (value && IsChkWatchVideoBetweenPercentage)
                    IsChkWatchVideoBetweenPercentage = false;

                SetProperty(ref _isChkWatchVideoBetweentSeconds, value);
            }
        }

        [ProtoMember(8)]
        public bool IsChkWatchVideoBetweenPercentage
        {
            get => _isChkWatchVideoBetweenPercentage;
            set
            {
                if (value == _isChkWatchVideoBetweenPercentage) return;

                if (value && IsChkWatchVideoBetweentSeconds)
                    IsChkWatchVideoBetweentSeconds = false;

                SetProperty(ref _isChkWatchVideoBetweenPercentage, value);
            }
        }

        [ProtoMember(9)]
        public bool IsChkRepeatVideo
        {
            get => _isChkRepeatVideo;
            set
            {
                SetProperty(ref _isChkRepeatVideo, value);
                if (!IsChkRepeatVideo && RunEveryday)
                    RunEveryday = false;
            }
        }

        [ProtoMember(10)]
        public RangeUtilities StopWatchVideoBetweentSeconds
        {
            get => _stopWatchVideoBetweentSeconds;
            set
            {
                if (_stopWatchVideoBetweentSeconds == value) return;
                SetProperty(ref _stopWatchVideoBetweentSeconds, value);
            }
        }

        [ProtoMember(11)]
        public RangeUtilities StopWatchVideoBetweenPercentage
        {
            get => _stopWatchVideoBetweenPercentage;
            set
            {
                if (_stopWatchVideoBetweenPercentage == value) return;
                SetProperty(ref _stopWatchVideoBetweenPercentage, value);
            }
        }

        [ProtoMember(12)]
        public int ViewCountForRepeatVideo
        {
            get => _viewCountForRepeatVideo;
            set
            {
                if (_viewCountForRepeatVideo == value) return;
                SetProperty(ref _viewCountForRepeatVideo, value);
            }
        }

        /// <summary>
        ///     To hold all destination list which holds all group,page count both selected and total
        /// </summary>
        [ProtoMember(13)]
        public ObservableCollection<ChannelDestinationSelectModel> ListSelectDestination
        {
            get => _listSelectDestination;
            set
            {
                if (_listSelectDestination == value) return;
                SetProperty(ref _listSelectDestination, value);
            }
        }

        [ProtoMember(14)]
        public List<string> ListOfChannels
        {
            get => _listOfChannels;
            set
            {
                if (_listOfChannels == value) return;
                SetProperty(ref _listOfChannels, value);
            }
        }

        [ProtoMember(15)]
        public bool MozillaSelected
        {
            get
            {
                var hasFirefox = false;
                try
                {
                    if (_mozillaSelected)
                        hasFirefox = File.Exists(@"C:\Program Files (x86)\Mozilla Firefox\firefox.exe") ||
                                     File.Exists(@"C:\Program Files\Mozilla Firefox\firefox.exe");
                }
                catch
                {
                    /*ignored*/
                }

                if (_mozillaSelected && !hasFirefox)
                {
                    CefSharpSelected = true;
                    IsBrowserVisibilityAvailable = false;
                    return false;
                }
                if (_mozillaSelected)
                    IsBrowserVisibilityAvailable = true;
                else
                    IsBrowserVisibilityAvailable = false;

                return _mozillaSelected;
            }
            set => SetProperty(ref _mozillaSelected, value);
        }

        [ProtoMember(22)]

        public bool IsBrowserVisibilityAvailable
        {
            get => _isBrowserVisibilityAvailable;
            set => SetProperty(ref _isBrowserVisibilityAvailable, value);
        }

        [ProtoMember(16)]
        public bool CefSharpSelected
        {
            get => _cefSharpSelected;
            set => SetProperty(ref _cefSharpSelected, value);
        }

        [ProtoMember(17)]
        public RangeUtilities RepeatWatchAfterMinutes
        {
            get => _repeatWatchAfterMinutes;
            set
            {
                if (_repeatWatchAfterMinutes == value) return;
                SetProperty(ref _repeatWatchAfterMinutes, value);
            }
        }

        [ProtoMember(18)]
        public bool IsBrowserHidden
        {
            get => _isBrowserHidden;
            set => SetProperty(ref _isBrowserHidden, value);
        }

        [ProtoMember(19)]
        public bool IsBrowserVisible
        {
            get => _isBrowserVisible;
            set => SetProperty(ref _isBrowserVisible, value);
        }

        [ProtoMember(20)]
        public bool RunEveryday
        {
            get => _runEveryday;
            set => SetProperty(ref _runEveryday, value);
        }

        [ProtoMember(21)]
        public bool SkipAd
        {
            get => _skipAd;
            set => SetProperty(ref _skipAd, value);
        }

        public RunningTimes RunningTimes { get; set; } = new RunningTimes();
    }
}