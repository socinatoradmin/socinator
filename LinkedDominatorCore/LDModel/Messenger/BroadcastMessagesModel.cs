using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DominatorHouseCore.Enums.LdQuery;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.LDModel.Filters;
using ProtoBuf;

namespace LinkedDominatorCore.LDModel.Messenger
{
    [ProtoContract]
    public class BroadcastMessagesModel : ModuleSetting, IGeneralSettings
    {
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

        public BroadcastMessagesModel()
        {
            ListQueryType = Enum.GetNames(typeof(LDMessengerQueryParameters)).ToList();
            ListQueryType.Remove("Notification");
            ListQueryType.Remove("JoinedGroupUrl");
        }

        public List<string> ListQueryType { get; set; } = new List<string>();

        [ProtoMember(2)] public override LDUserFilterModel LDUserFilterModel { get; set; } = new LDUserFilterModel();
        [ProtoMember(3)] public OtherConfigModel OtherConfigModel { get; set; } = new OtherConfigModel();

        [ProtoMember(4)]
        public bool IsChkSpintaxChecked
        {
            get => _IsChkSpintaxChecked;
            set => SetProperty(ref _IsChkSpintaxChecked, value);
        }

        [ProtoMember(5)]
        public bool IsChkTagChecked
        {
            get => _IsChkTagChecked;

            set => SetProperty(ref _IsChkTagChecked, value);
        }

        [ProtoMember(6)]
        public ObservableCollection<ManageMessagesModel> LstDisplayManageMessagesModel { get; set; } =
            new ObservableCollection<ManageMessagesModel>();

        public ManageMessagesModel ManageMessagesModel { get; set; } = new ManageMessagesModel();

        public ObservableCollection<ManageMessagesModel> LstManageMessagesModel { get; set; } =
            new ObservableCollection<ManageMessagesModel>();

        [ProtoMember(7)]
        public bool IsCheckedBySoftware
        {
            get => _IsCheckedBySoftware;
            set => SetProperty(ref _IsCheckedBySoftware, value);
        }

        [ProtoMember(8)]
        public bool IsCheckedOutSideSoftware
        {
            get => _IsCheckedOutSideSoftware;
            set => SetProperty(ref _IsCheckedOutSideSoftware, value);
        }

        [ProtoMember(9)]
        public bool IsCheckedLangKeyCustomUserList
        {
            get => _IsCheckedLangKeyCustomUserList;
            set => SetProperty(ref _IsCheckedLangKeyCustomUserList, value);
        }

        [ProtoMember(10)]
        public string UrlInput
        {
            get => _UrlInput;
            set => SetProperty(ref _UrlInput, value);
        }

        public List<string> UrlList
        {
            get => _UrlList;
            set => SetProperty(ref _UrlList, value);
        }

        public List<string> GroupUrlList
        {
            get => _GroupUrlList;
            set => SetProperty(ref _GroupUrlList, value);
        }


        [ProtoMember(11)]
        public bool IsChkSkipBlackListedUser
        {
            get => _IsChkSkipBlackListedUser;
            set => SetProperty(ref _IsChkSkipBlackListedUser, value);
        }


        [ProtoMember(12)]
        public bool IsChkPrivateBlackList
        {
            get => _IsChkPrivateBlackList;
            set => SetProperty(ref _IsChkPrivateBlackList, value);
        }


        [ProtoMember(13)]
        public bool IsChkGroupBlackList
        {
            get => _IsChkGroupBlackList;
            set => SetProperty(ref _IsChkGroupBlackList, value);
        }

        [ProtoMember(14)]
        public bool IsConnections
        {
            get => _IsConnections;
            set => SetProperty(ref _IsConnections, value);
        }

        [ProtoMember(15)]
        public bool IsGroup
        {
            get => _IsGroup;
            set => SetProperty(ref _IsGroup, value);
        }

        [ProtoMember(16)]
        public string GroupUrlInput
        {
            get => _GroupUrlInput;
            set => SetProperty(ref _GroupUrlInput, value);
        }

        [ProtoMember(17)]
        public bool IsSkipUserAlreadyRecievedMessageFromSoftware
        {
            get => _isSkipUserAlreadyRecievedMessageFromSoftware;
            set => SetProperty(ref _isSkipUserAlreadyRecievedMessageFromSoftware, value);
        }

        [ProtoMember(18)]
        public bool IsSaveFailedActivity
        {
            get => _isSaveFailedUser;
            set => SetProperty(ref _isSaveFailedUser, value);
        }
        

        [ProtoMember(19)]
        public bool IsStopSendMessageOnFailed
        {
            get => _IsStopSendMessageOnFailed;
            set => SetProperty(ref _IsStopSendMessageOnFailed, value);
        }        

        [ProtoMember(20)]
        public int StopSendMessageOnCount
        {
            get => _StopSendMessageOnCount;
            set => SetProperty(ref _StopSendMessageOnCount, value);
        }

        [ProtoMember(21)]
        public bool IsCheckedConnectedBefore
        {
            get => _IsCheckedConnectedBefore;
            set
            {
                if (value == _IsCheckedConnectedBefore) return;
                SetProperty(ref _IsCheckedConnectedBefore, value);
            }
        }

        [ProtoMember(22)]
        public int Days
        {
            get => _Days;
            set
            {
                if (value == _Days) return;
                SetProperty(ref _Days, value);
            }
        }

        [ProtoMember(23)]
        public int Hours
        {
            get => _Hours;
            set
            {
                if (value == _Hours) return;
                SetProperty(ref _Hours, value);
            }
        }
        [ProtoMember(24)]
        public bool IsSkipUserAlreadyRecievedMessageFromOutSideSoftware
        {
            get => _isSkipUserAlreadyRecievedMessageFromOutSideSoftware;
            set => SetProperty(ref _isSkipUserAlreadyRecievedMessageFromOutSideSoftware, value);
        }
        [ProtoMember(25)]
        public bool IsFollower
        {
            get => _IsFollower;
            set => SetProperty(ref _IsFollower, value);
        }
        [ProtoMember(1)] JobConfiguration IGeneralSettings.JobConfiguration { get; set; } = new JobConfiguration();

        #region MyRegion

        private bool _IsChkSpintaxChecked;
        private bool _IsChkTagChecked;
        private bool _IsCheckedBySoftware;
        private bool _IsCheckedOutSideSoftware;
        private bool _IsCheckedLangKeyCustomUserList;
        private bool _IsChkSkipBlackListedUser;
        private string _UrlInput;
        private bool _IsChkPrivateBlackList;
        private bool _IsChkGroupBlackList;
        private bool _IsCheckedConnectedBefore;
        private int _Days;
        private int _Hours;


        private List<string> _UrlList = new List<string>();
        private List<string> _GroupUrlList = new List<string>();
        private string _GroupUrlInput;
        private bool _IsConnections = true;
        private bool _IsGroup;
        private bool _IsFollower;
        private bool _isSkipUserAlreadyRecievedMessageFromSoftware;
        private bool _isSkipUserAlreadyRecievedMessageFromOutSideSoftware;
        private bool _isSaveFailedUser;
        private bool _IsStopSendMessageOnFailed;
        private int _StopSendMessageOnCount = 3;

        #endregion
    }
}