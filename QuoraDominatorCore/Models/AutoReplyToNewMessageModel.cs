using System.Collections.Generic;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace QuoraDominatorCore.Models
{
    [ProtoContract]
    public class AutoReplyToNewMessageModel : ModuleSetting, IGeneralSettings
    {
        private bool _isChkAutoReplayGroupBlacklist;

        private bool _isChkAutoReplayPrivateBlacklist;
        private bool _isReplyToAllMessages﻿﻿Checked = true;

        private bool _isReplyToMessagesThatContainSpecificWord﻿Checked;
        private bool _isReplyToPendingMessages﻿﻿Checked;

        private List<string> _lstMultiMessageForUserHasNotReplied = new List<string>();
        private List<string> _lstMultiMessageForUserHasReplied = new List<string>();

        private string _message;
        private string _specificWord;

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

        [ProtoMember(2)] public override UserFilterModel UserFilterModel { get; set; } = new UserFilterModel();

        [ProtoMember(3)] public OtherConfigModel OtherConfigModel { get; set; } = new OtherConfigModel();

        [ProtoMember(4)]
        public bool IsReplyToMessagesThatContainSpecificWordChecked
        {
            get => _isReplyToMessagesThatContainSpecificWord﻿Checked;
            set
            {
                if (_isReplyToMessagesThatContainSpecificWord﻿Checked == value)
                    return;
                SetProperty(ref _isReplyToMessagesThatContainSpecificWord﻿Checked, value);
            }
        }

        [ProtoMember(5)]
        public bool IsReplyToPendingMessagesChecked
        {
            get => _isReplyToPendingMessages﻿﻿Checked;
            set
            {
                if (_isReplyToPendingMessages﻿﻿Checked == value)
                    return;
                SetProperty(ref _isReplyToPendingMessages﻿﻿Checked, value);
            }
        }

        [ProtoMember(6)]
        public bool IsReplyToAllMessagesChecked
        {
            get => _isReplyToAllMessages﻿﻿Checked;
            set
            {
                if (_isReplyToAllMessages﻿﻿Checked == value)
                    return;
                SetProperty(ref _isReplyToAllMessages﻿﻿Checked, value);
            }
        }

        [ProtoMember(7)]
        public string SpecificWord
        {
            get => _specificWord;
            set
            {
                if (_specificWord == value)
                    return;
                SetProperty(ref _specificWord, value);
            }
        }

        [ProtoMember(8)]
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

        [ProtoMember(9)]
        public bool IsChkAutoReplyPrivateBlacklist
        {
            get => _isChkAutoReplayPrivateBlacklist;
            set
            {
                if (_isChkAutoReplayPrivateBlacklist == value)
                    return;
                SetProperty(ref _isChkAutoReplayPrivateBlacklist, value);
            }
        }

        [ProtoMember(10)]
        public bool IsChkAutoReplyGroupBlacklist
        {
            get => _isChkAutoReplayGroupBlacklist;
            set
            {
                if (_isChkAutoReplayGroupBlacklist == value)
                    return;
                SetProperty(ref _isChkAutoReplayGroupBlacklist, value);
            }
        }

        [ProtoMember(11)]
        public List<string> LstMultiMessageForUserHasNotReplied
        {
            get => _lstMultiMessageForUserHasNotReplied;

            set
            {
                if (_lstMultiMessageForUserHasNotReplied != value)
                    SetProperty(ref _lstMultiMessageForUserHasNotReplied, value);
            }
        }

        [ProtoMember(12)]
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

        private bool _isChkSkipPrivateBlackList;
        private bool _isChkSkipGroupBlackList;
        public bool IsChkAutoReplySkipPrivateBlacklist
        {
            get => _isChkSkipPrivateBlackList;
            set
            {
                if (_isChkSkipPrivateBlackList != value)
                    SetProperty(ref _isChkSkipPrivateBlackList, value);
            }
        }

        public bool IsChkAutoReplySkipGroupBlacklist
        {
            get => _isChkSkipGroupBlackList;
            set
            {
                if (_isChkSkipGroupBlackList != value)
                    SetProperty(ref _isChkSkipGroupBlackList, value);
            }
        }
    }
}