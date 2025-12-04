using System.Collections.Generic;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace LinkedDominatorCore.LDModel.GrowConnection
{
    [ProtoContract]
    public class ExportConnectionModel : ModuleSetting
    {
        private string _FilenameFormat;

        private bool _IsCheckedDownloadPdf;

        private bool _isChkGroupBlackList;

        private bool _isChkPrivateBlackList;

        private bool _isChkSkipBlackListedUser;


        private bool _IsFilenameFormatChecked;

        [ProtoMember(5)] private string _urlInput;

        private RangeUtilities _WaitOnEachProfileBetween = new RangeUtilities(10, 12);

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
            get => _isCheckedBySoftware;
            set => SetProperty(ref _isCheckedBySoftware, value);
        }

        [ProtoMember(3)]
        public bool IsCheckedOutSideSoftware
        {
            get => _isCheckedOutSideSoftware;
            set => SetProperty(ref _isCheckedOutSideSoftware, value);
        }

        [ProtoMember(4)]
        public bool IsCheckedLangKeyCustomUserList
        {
            get => _isCheckedLangKeyCustomUserList;
            set => SetProperty(ref _isCheckedLangKeyCustomUserList, value);
        }

        public string UrlInput
        {
            get => _urlInput;
            set => SetProperty(ref _urlInput, value);
        }

        [ProtoMember(6)]
        public List<string> UrlList
        {
            get => _urlList;
            set => SetProperty(ref _urlList, value);
        }

        [ProtoMember(7)]
        public bool IsCheckedConnectedBefore
        {
            get => _isCheckedConnectedBefore;
            set => SetProperty(ref _isCheckedConnectedBefore, value);
        }

        [ProtoMember(8)]
        public int Days
        {
            get => _days;
            set => SetProperty(ref _days, value);
        }

        [ProtoMember(9)]
        public int Hours
        {
            get => _hours;
            set => SetProperty(ref _hours, value);
        }

        [ProtoMember(10)]
        public bool IsCheckedOnlyContactInfo
        {
            get => _IsCheckedOnlyContactInfo;
            set => SetProperty(ref _IsCheckedOnlyContactInfo, value);
        }

        [ProtoMember(11)]
        public bool IsChkSkipBlackListedUser
        {
            get => _isChkSkipBlackListedUser;
            set => SetProperty(ref _isChkSkipBlackListedUser, value);
        }

        [ProtoMember(12)]
        public bool IsChkPrivateBlackList
        {
            get => _isChkPrivateBlackList;
            set => SetProperty(ref _isChkPrivateBlackList, value);
        }

        [ProtoMember(13)]
        public bool IsChkGroupBlackList
        {
            get => _isChkGroupBlackList;
            set => SetProperty(ref _isChkGroupBlackList, value);
        }

        [ProtoMember(14)]
        public bool IsFilenameFormatChecked
        {
            get => _IsFilenameFormatChecked;
            set => SetProperty(ref _IsFilenameFormatChecked, value);
        }

        [ProtoMember(15)]
        public string FilenameFormat
        {
            get => _FilenameFormat;
            set => SetProperty(ref _FilenameFormat, value);
        }

        [ProtoMember(16)]
        public bool IsCheckedDownloadPdf
        {
            get => _IsCheckedDownloadPdf;
            set => SetProperty(ref _IsCheckedDownloadPdf, value);
        }

        [ProtoMember(17)]
        public RangeUtilities WaitOnEachProfileBetween
        {
            get => _WaitOnEachProfileBetween;
            set => SetProperty(ref _WaitOnEachProfileBetween, value);
        }

        [ProtoMember(18)]
        public bool IsCheckedRecentConnections
        {
            get => _IsCheckedRecentConnections;
            set => SetProperty(ref _IsCheckedRecentConnections, value);
        }

        [ProtoMember(19)]
        public bool IsStopScheduling
        {
            get => _isStopScheduling;
            set => SetProperty(ref _isStopScheduling, value);
        }

        #region private properties

        private bool _isCheckedBySoftware;
        private bool _isCheckedOutSideSoftware;
        private bool _isCheckedLangKeyCustomUserList;
        private List<string> _urlList;
        private bool _isCheckedConnectedBefore;
        private int _days;
        private int _hours;
        private bool _IsCheckedOnlyContactInfo;
        private bool _IsCheckedRecentConnections;
        private bool _isStopScheduling = true;

        #endregion
    }
}