using System.Collections.ObjectModel;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.LDModel.Filters;
using ProtoBuf;

namespace LinkedDominatorCore.LDModel.Messenger
{
    [ProtoContract]
    public class AutoReplyToNewMessageModel : ModuleSetting, IGeneralSettings
    {
        private bool _IsChkGroupBlackList;

        private bool _IsChkPrivateBlackList;

        private bool _IsChkSkipBlackListedUser;

        private bool _isReplyToAllMessages﻿﻿Checked;

        private bool _isReplyToAllUserMessagesWhodidnotReply;

        private bool _isReplyToMessagesThatContainSpecificWord﻿Checked;

        private bool _isReplyToPendingMessages﻿﻿Checked;

        private string _specificWord;

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

        [ProtoMember(2)] public LDUserFilterModel UserFilterModel { get; set; } = new LDUserFilterModel();

        [ProtoMember(3)] public OtherConfigModel OtherConfigModel { get; set; } = new OtherConfigModel();

        [ProtoMember(4)]
        public bool IsReplyToMessagesThatContainSpecificWord﻿Checked
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
        public bool IsReplyToPendingMessages﻿﻿Checked
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
        public bool IsReplyToAllUserMessagesWhodidnotReply
        {
            get => _isReplyToAllUserMessagesWhodidnotReply;
            set
            {
                if (_isReplyToAllUserMessagesWhodidnotReply == value)
                    return;
                SetProperty(ref _isReplyToAllUserMessagesWhodidnotReply, value);
            }
        }

        [ProtoMember(8)]
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

        [ProtoMember(9)]
        public bool IsChkSpintaxChecked
        {
            get => _IsChkSpintaxChecked;

            set
            {
                if (value == _IsChkSpintaxChecked)
                    return;
                SetProperty(ref _IsChkSpintaxChecked, value);
            }
        }

        [ProtoMember(10)]
        public bool IsChkTagChecked
        {
            get => _IsChkTagChecked;

            set
            {
                if (value == _IsChkTagChecked)
                    return;
                SetProperty(ref _IsChkTagChecked, value);
            }
        }

        [ProtoMember(11)]
        public ObservableCollection<ManageMessagesModel> LstDisplayManageMessagesModel { get; set; } =
            new ObservableCollection<ManageMessagesModel>();

        public ManageMessagesModel ManageMessagesModel { get; set; } = new ManageMessagesModel();

        public ObservableCollection<ManageMessagesModel> LstManageMessagesModel { get; set; } =
            new ObservableCollection<ManageMessagesModel>();

        [ProtoMember(12)]
        public bool IsChkSkipBlackListedUser
        {
            get => _IsChkSkipBlackListedUser;
            set
            {
                if (value == _IsChkSkipBlackListedUser)
                    return;
                SetProperty(ref _IsChkSkipBlackListedUser, value);
            }
        }

        [ProtoMember(13)]
        public bool IsChkPrivateBlackList
        {
            get => _IsChkPrivateBlackList;
            set
            {
                if (value == _IsChkPrivateBlackList)
                    return;
                SetProperty(ref _IsChkPrivateBlackList, value);
            }
        }

        [ProtoMember(14)]
        public bool IsChkGroupBlackList
        {
            get => _IsChkGroupBlackList;
            set
            {
                if (value == _IsChkGroupBlackList)
                    return;
                SetProperty(ref _IsChkGroupBlackList, value);
            }
        }

        [ProtoMember(15)]
        public bool IsCheckedLastSendMessageFrom
        {
            get => _IsCheckedLastSendMessageFrom;
            set
            {
                if (value == _IsCheckedLastSendMessageFrom) return;
                SetProperty(ref _IsCheckedLastSendMessageFrom, value);
            }
        }

        [ProtoMember(16)]
        public int Days
        {
            get => _Days;
            set
            {
                if (value == _Days) return;
                SetProperty(ref _Days, value);
            }
        }

        [ProtoMember(17)]
        public int Hours
        {
            get => _Hours;
            set
            {
                if (value == _Hours) return;
                SetProperty(ref _Hours, value);
            }
        }
        [ProtoMember(18)]
        public bool IsCheckedBySoftware
        {
            get => _IsCheckedBySoftware;
            set => SetProperty(ref _IsCheckedBySoftware, value);
        }

        [ProtoMember(19)]
        public bool IsCheckedIgnoreAlreadySendMessage
        {
            get => _IsCheckedIgnoreAlreadySendMessage;
            set => SetProperty(ref _IsCheckedIgnoreAlreadySendMessage, value);
        }

        [ProtoMember(1)] JobConfiguration IGeneralSettings.JobConfiguration { get; set; } = new JobConfiguration();

        #region MyRegion

        private bool _IsChkSpintaxChecked;
        private bool _IsChkTagChecked;
        private bool _IsCheckedLastSendMessageFrom;
        private int _Days;
        private int _Hours;
        private bool _IsCheckedBySoftware;
        private bool _IsCheckedIgnoreAlreadySendMessage;

        #endregion
    }
}