using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace PinDominatorCore.PDModel
{
    public interface ITryModel
    {
        bool ChkRetryPost { get; set; }
        bool ChkEnableTryCommentsAfterPostIsTried { get; set; }
        bool ChkRemovePoorQualitySources { get; set; }
        RangeUtilities CommentToBeTryAfterEachTriedPost { get; set; }
    }

    [ProtoContract]
    public class TryModel : ModuleSetting, ITryModel, IGeneralSettings
    {
        private bool _chkTryUsersLatestPinsChecked;
        private bool _chkUploadNotesChecked;
        private RangeUtilities _delayBetweenTriesForAfterActivity = new RangeUtilities(15, 30);
        private RangeUtilities _increaseEachDayTry = new RangeUtilities();

        private bool _isAddedToCampaign;

        private bool _isChkFollowUserAfterTry;

        private bool _isChkGroupBlackList;
        private bool _isChkIncreaseEachDayTry;

        private bool _isChkPrivateBlackList;

        private bool _isChkSkipBlackListedUser;
        private bool _isMakeNoteAsSpinText;
        private List<string> _lstNotes = new List<string>();
        private string _mediaPath = string.Empty;
        private string _message;
        private RangeUtilities _tries = new RangeUtilities(1, 1);
        private RangeUtilities _triesPerUser = new RangeUtilities();

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

        [ProtoMember(11)]
        public bool IsAddedToCampaign
        {
            get => _isAddedToCampaign;
            set
            {
                if (_isAddedToCampaign && _isAddedToCampaign == value)
                    return;
                SetProperty(ref _isAddedToCampaign, value);
            }
        }

        [ProtoMember(12)]
        public bool ChkTryUsersLatestPinsChecked
        {
            get => _chkTryUsersLatestPinsChecked;

            set
            {
                if (value == _chkTryUsersLatestPinsChecked)
                    return;
                SetProperty(ref _chkTryUsersLatestPinsChecked, value);
            }
        }

        [ProtoMember(13)]
        public string MediaPath
        {
            get => _mediaPath;

            set
            {
                if (value == _mediaPath)
                    return;
                SetProperty(ref _mediaPath, value);
            }
        }

        [ProtoMember(14)]
        public RangeUtilities IncreaseEachDayTry
        {
            get => _increaseEachDayTry;

            set
            {
                if (value == _increaseEachDayTry)
                    return;
                SetProperty(ref _increaseEachDayTry, value);
            }
        }

        [ProtoMember(15)]
        public RangeUtilities Tries
        {
            get => _tries;

            set
            {
                if (value == _tries)
                    return;
                SetProperty(ref _tries, value);
            }
        }

        [ProtoMember(16)]
        public bool IsChkIncreaseEachDayTry
        {
            get => _isChkIncreaseEachDayTry;
            set
            {
                if (_isChkIncreaseEachDayTry == value)
                    return;
                SetProperty(ref _isChkIncreaseEachDayTry, value);
            }
        }

        [ProtoMember(17)]
        public bool IsChkFollowUserAfterTry
        {
            get => _isChkFollowUserAfterTry;
            set
            {
                if (_isChkFollowUserAfterTry == value)
                    return;
                SetProperty(ref _isChkFollowUserAfterTry, value);
            }
        }

        [ProtoMember(18)]
        public RangeUtilities TriesPerUser
        {
            get => _triesPerUser;

            set
            {
                if (value == _triesPerUser)
                    return;
                SetProperty(ref _triesPerUser, value);
            }
        }

        [ProtoMember(19)]
        public bool ChkUploadNotesChecked
        {
            get => _chkUploadNotesChecked;

            set
            {
                if (value == _chkUploadNotesChecked)
                    return;
                SetProperty(ref _chkUploadNotesChecked, value);
            }
        }

        [ProtoMember(20)]
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

        [ProtoMember(21)]
        public List<string> LstNotes
        {
            get => _lstNotes;
            set
            {
                if (value == _lstNotes)
                    return;
                SetProperty(ref _lstNotes, value);
            }
        }

        [ProtoMember(22)]
        public ObservableCollection<ManageNoteModel> LstDisplayManageNoteModel { get; set; } =
            new ObservableCollection<ManageNoteModel>();

        [ProtoMember(23)]
        public RangeUtilities DelayBetweenTriesForAfterActivity
        {
            get => _delayBetweenTriesForAfterActivity;

            set
            {
                if (value == _delayBetweenTriesForAfterActivity)
                    return;
                SetProperty(ref _delayBetweenTriesForAfterActivity, value);
            }
        }

        [ProtoMember(24)]
        public bool IsMakeNoteAsSpinText
        {
            get => _isMakeNoteAsSpinText;

            set
            {
                if (value == _isMakeNoteAsSpinText)
                    return;
                SetProperty(ref _isMakeNoteAsSpinText, value);
            }
        }

        public List<string> ListQueryType { get; set; } = new List<string>();

        public ManageNoteModel ManageNoteModel { get; set; } = new ManageNoteModel();

        #region ITryModel

        private bool _chkRetryPost;

        [ProtoMember(5)]
        public bool ChkRetryPost
        {
            get => _chkRetryPost;
            set
            {
                if (value == _chkRetryPost)
                    return;
                SetProperty(ref _chkRetryPost, value);
            }
        }

        private bool _chkEnableTryCommentsAfterPostIsTried;

        [ProtoMember(6)]
        public bool ChkEnableTryCommentsAfterPostIsTried
        {
            get => _chkEnableTryCommentsAfterPostIsTried;
            set
            {
                if (_chkEnableTryCommentsAfterPostIsTried == value)
                    return;
                SetProperty(ref _chkEnableTryCommentsAfterPostIsTried, value);
            }
        }

        private bool _chkRemovePoorQualitySources;

        [ProtoMember(7)]
        public bool ChkRemovePoorQualitySources
        {
            get => _chkRemovePoorQualitySources;
            set
            {
                if (_chkRemovePoorQualitySources == value)
                    return;
                SetProperty(ref _chkRemovePoorQualitySources, value);
            }
        }

        [ProtoMember(9)] public RangeUtilities CommentToBeTryAfterEachTriedPost { get; set; } = new RangeUtilities();

        #endregion
    }
}