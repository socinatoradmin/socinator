using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace PinDominatorCore.PDModel
{

    public class AutoReplyToNewMessageModel : ModuleSetting
    {
        private bool _isReplyToAllMessagesChecked;
        private bool _isCheckedReplyToMessagesThatContainsSpecificWord;

        private bool _isChkGroupBlackList;

        private bool _isChkPrivateBlackList;

        private bool _isChkSkipBlackListedUser;

        private bool _isChkAddMultipleMessages;

        private bool _isMakeMessageAsSpinText;

        private bool _isScrapFullInformation;

        private bool _isSendPinAsAMessage;

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

        public ManageMessagesModel ManageMessagesModel { get; set; } = new ManageMessagesModel();

        [ProtoMember(4)]
        public ObservableCollection<ManageMessagesModel> LstDisplayManageMessagesModel { get; set; } =
            new ObservableCollection<ManageMessagesModel>();

        [ProtoMember(1)]
        public bool IsChkSkipBlackListedUser
        {
            get => _isChkSkipBlackListedUser;
            set
            {
                if (_isChkSkipBlackListedUser == value) return;
                SetProperty(ref _isChkSkipBlackListedUser, value);
            }
        }

        [ProtoMember(2)]
        public bool IsChkPrivateBlackList
        {
            get => _isChkPrivateBlackList;
            set
            {
                if (_isChkPrivateBlackList == value) return;
                SetProperty(ref _isChkPrivateBlackList, value);
            }
        }

        [ProtoMember(3)]
        public bool IsChkGroupBlackList
        {
            get => _isChkGroupBlackList;
            set
            {
                if (_isChkGroupBlackList == value) return;
                SetProperty(ref _isChkGroupBlackList, value);
            }
        }

        [ProtoMember(4)]
        public bool IsScrapFullInformation
        {
            get => _isScrapFullInformation;
            set
            {
                if (_isScrapFullInformation == value) return;
                SetProperty(ref _isScrapFullInformation, value);
            }
        }

        [ProtoMember(5)]
        public bool IsSendPinAsAMessage
        {
            get => _isSendPinAsAMessage;
            set
            {
                if (_isSendPinAsAMessage == value) return;
                SetProperty(ref _isSendPinAsAMessage, value);
            }
        }

        [ProtoMember(6)]
        public bool IsMakeMessageAsSpinText
        {
            get => _isMakeMessageAsSpinText;
            set
            {
                if (_isMakeMessageAsSpinText == value) return;
                SetProperty(ref _isMakeMessageAsSpinText, value);
            }
        }

        private bool _isReplyToPendingMessagesChecked = true;


        [ProtoMember(7)]
        public bool IsReplyToPendingMessagesChecked
        {
            get => _isReplyToPendingMessagesChecked;
            set
            {
                if (value == _isReplyToPendingMessagesChecked)
                    return;
                SetProperty(ref _isReplyToPendingMessagesChecked, value);
            }
        }

        [ProtoMember(8)]
        public bool IsReplyToAllMessagesChecked
        {
            get => _isReplyToAllMessagesChecked;
            set
            {
                if (value == _isReplyToAllMessagesChecked)
                    return;
                SetProperty(ref _isReplyToAllMessagesChecked, value);
            }
        }

        [ProtoMember(9)]
        public bool IsCheckedReplyToMessagesThatContainsSpecificWord
        {
            get => _isCheckedReplyToMessagesThatContainsSpecificWord;
            set
            {
                if (value == _isCheckedReplyToMessagesThatContainsSpecificWord)
                    return;
                SetProperty(ref _isCheckedReplyToMessagesThatContainsSpecificWord, value);
            }
        }

        [ProtoMember(10)]
        public bool IsChkAddMultipleMessages
        {
            get => _isChkAddMultipleMessages;
            set
            {
                if (_isChkAddMultipleMessages == value) return;
                SetProperty(ref _isChkAddMultipleMessages, value);
            }
        }

        public string ReplyToMessagesThatContainsSpecificWordText { get; set; }

        public List<string> LstMessagesContainsSpecificWords { get; set; }

    }
}