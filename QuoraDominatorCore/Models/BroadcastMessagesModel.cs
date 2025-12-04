using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace QuoraDominatorCore.Models
{
    [ProtoContract]
    public class BroadcastMessagesModel : ModuleSetting, IGeneralSettings
    {
        private bool _isChkGroupBlackList;

        private bool _isChkPrivateBlackList;


        private bool _isSpintaxChecked;

        private ObservableCollection<ManageMessagesModel> _lstDisplayManageMessageModel =
            new ObservableCollection<ManageMessagesModel>();

        private List<string> _lstMultiMessageForUserHasNotReplied = new List<string>();
        private List<string> _lstMultiMessageForUserHasReplied = new List<string>();
        private ManageMessagesModel _manageMessagesModel = new ManageMessagesModel();

        public JobConfiguration FastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(266, 400),
            ActivitiesPerHour = new RangeUtilities(26, 40),
            ActivitiesPerWeek = new RangeUtilities(1600, 2400),
            ActivitiesPerJob = new RangeUtilities(33, 50),
            DelayBetweenJobs = new RangeUtilities(81, 122),
            DelayBetweenActivity = new RangeUtilities(15, 30),
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
            ActivitiesPerDay = new RangeUtilities(66, 100),
            ActivitiesPerHour = new RangeUtilities(6, 10),
            ActivitiesPerWeek = new RangeUtilities(400, 600),
            ActivitiesPerJob = new RangeUtilities(8, 12),
            DelayBetweenJobs = new RangeUtilities(81, 122),
            DelayBetweenActivity = new RangeUtilities(60, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(400, 600),
            ActivitiesPerHour = new RangeUtilities(40, 60),
            ActivitiesPerWeek = new RangeUtilities(2400, 3600),
            ActivitiesPerJob = new RangeUtilities(50, 75),
            DelayBetweenJobs = new RangeUtilities(65, 97),
            DelayBetweenActivity = new RangeUtilities(7, 15),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public List<string> ListQueryType { get; set; } = new List<string>();

        [ProtoMember(2)] public override UserFilterModel UserFilterModel { get; set; } = new UserFilterModel();

        [ProtoMember(3)] public OtherConfigModel OtherConfigModel { get; set; } = new OtherConfigModel();

        [ProtoMember(4)]
        public ObservableCollection<ManageMessagesModel> LstDisplayManageMessageModel
        {
            get => _lstDisplayManageMessageModel;
            set
            {
                if (_lstDisplayManageMessageModel != value)
                    SetProperty(ref _lstDisplayManageMessageModel, value);
            }
        }

        [ProtoMember(7)]
        public ManageMessagesModel ManageMessagesModel
        {
            get => _manageMessagesModel;
            set
            {
                if (_manageMessagesModel != value)
                    SetProperty(ref _manageMessagesModel, value);
            }
        }

        [ProtoMember(5)]
        public bool IsChkBroadCastPrivateBlacklist
        {
            get => _isChkPrivateBlackList;
            set
            {
                if (_isChkPrivateBlackList != value)
                    SetProperty(ref _isChkPrivateBlackList, value);
            }
        }

        [ProtoMember(6)]
        public bool IsChkBroadCastGroupBlacklist
        {
            get => _isChkGroupBlackList;
            set
            {
                if (_isChkGroupBlackList != value)
                    SetProperty(ref _isChkGroupBlackList, value);
            }
        }
        private bool _isChkSkipPrivateBlackList;
        private bool _isChkSkipGroupBlackList;
        public bool IsChkBroadCastSkipPrivateBlacklist
        {
            get => _isChkSkipPrivateBlackList;
            set
            {
                if (_isChkSkipPrivateBlackList != value)
                    SetProperty(ref _isChkSkipPrivateBlackList, value);
            }
        }

        public bool IsChkBroadCastSkipGroupBlacklist
        {
            get => _isChkSkipGroupBlackList;
            set
            {
                if (_isChkSkipGroupBlackList != value)
                    SetProperty(ref _isChkSkipGroupBlackList, value);
            }
        }

        [ProtoMember(7)]
        public List<string> LstMultiMessageForUserHasNotReplied
        {
            get => _lstMultiMessageForUserHasNotReplied;

            set
            {
                if (_lstMultiMessageForUserHasNotReplied != value)
                    SetProperty(ref _lstMultiMessageForUserHasNotReplied, value);
            }
        }

        [ProtoMember(8)]
        public List<string> LstMultiMessageForUserHasReplied
        {
            get => _lstMultiMessageForUserHasReplied;

            set
            {
                if (_lstMultiMessageForUserHasReplied != value)
                    SetProperty(ref _lstMultiMessageForUserHasReplied, value);
            }
        }

        [ProtoMember(9)]
        public bool IsSpintaxChecked
        {
            get => _isSpintaxChecked;
            set
            {
                if (_isSpintaxChecked != value)
                    SetProperty(ref _isSpintaxChecked, value);
            }
        }

        [ProtoMember(1)] JobConfiguration IGeneralSettings.JobConfiguration { get; set; }
    }
}