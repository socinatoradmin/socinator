using System.Collections.Generic;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace QuoraDominatorCore.Models
{
    [ProtoContract]
    public class SendMessageToFollowerModel : ModuleSetting, IGeneralSettings
    {
        private bool _ischkSendMessageGroupBlacklist;

        private bool _ischkSendMessagePrivateBlacklist;

        private List<string> _lstMultiMessageForUserHasNotReplied = new List<string>();
        private List<string> _lstMultiMessageForUserHasReplied = new List<string>();

        private string _message;

        public JobConfiguration FastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(333, 500),
            ActivitiesPerHour = new RangeUtilities(33, 50),
            ActivitiesPerWeek = new RangeUtilities(2000, 3000),
            ActivitiesPerJob = new RangeUtilities(10, 10),
            DelayBetweenJobs = new RangeUtilities(40, 70),
            DelayBetweenActivity = new RangeUtilities(30, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(200, 300),
            ActivitiesPerHour = new RangeUtilities(20, 30),
            ActivitiesPerWeek = new RangeUtilities(1200, 1800),
            ActivitiesPerJob = new RangeUtilities(10, 10),
            DelayBetweenJobs = new RangeUtilities(65, 97),
            DelayBetweenActivity = new RangeUtilities(60, 90),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration SlowSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(66, 100),
            ActivitiesPerHour = new RangeUtilities(6, 10),
            ActivitiesPerWeek = new RangeUtilities(400, 600),
            ActivitiesPerJob = new RangeUtilities(2, 2),
            DelayBetweenJobs = new RangeUtilities(81, 122),
            DelayBetweenActivity = new RangeUtilities(90, 120),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(666, 1000),
            ActivitiesPerHour = new RangeUtilities(66, 100),
            ActivitiesPerWeek = new RangeUtilities(4000, 6000),
            ActivitiesPerJob = new RangeUtilities(10, 10),
            DelayBetweenJobs = new RangeUtilities(69, 103),
            DelayBetweenActivity = new RangeUtilities(10, 30),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        [ProtoMember(2)] public override UserFilterModel UserFilterModel { get; set; } = new UserFilterModel();

        [ProtoMember(3)] public OtherConfigModel OtherConfigModel { get; set; } = new OtherConfigModel();

        [ProtoMember(4)]
        public string Message
        {
            get => _message;
            set
            {
                if (_message == value)
                    return;
                SetProperty(ref _message, value);
            }
        }

        [ProtoMember(5)]
        public bool IschkSendMessagePrivateBlacklist
        {
            get => _ischkSendMessagePrivateBlacklist;
            set
            {
                if (_ischkSendMessagePrivateBlacklist != value)
                    SetProperty(ref _ischkSendMessagePrivateBlacklist, value);
            }
        }

        [ProtoMember(6)]
        public bool IschkSendMessageGroupBlacklist
        {
            get => _ischkSendMessageGroupBlacklist;
            set
            {
                if (_ischkSendMessageGroupBlacklist != value)
                    SetProperty(ref _ischkSendMessageGroupBlacklist, value);
            }
        }
        private bool _isChkSkipPrivateBlackList;
        private bool _isChkSkipGroupBlackList;
        public bool IsChkSendMessageSkipPrivateBlacklist
        {
            get => _isChkSkipPrivateBlackList;
            set
            {
                if (_isChkSkipPrivateBlackList != value)
                    SetProperty(ref _isChkSkipPrivateBlackList, value);
            }
        }

        public bool IsChkSendMessageSkipGroupBlacklist
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

        [ProtoMember(1)] JobConfiguration IGeneralSettings.JobConfiguration { get; set; }
    }
}