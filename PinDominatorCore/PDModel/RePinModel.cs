using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using DominatorHouseCore.Models.SocioPublisher;
namespace PinDominatorCore.PDModel
{
    public interface IRePin
    {
        bool ChkEnableTryCommentsAfterPostIsTried { get; set; }
        bool ChkRemovePoorQualitySources { get; set; }
        bool ChkRemoveSourceIfFollowRatioLower { get; set; }
        RangeUtilities TryRange { get; set; }
        RangeUtilities FollowBackRatio { get; set; }
        bool IsChkReTryPost { get; set; }
        bool ChkTryOnPinAfterRepinChecked { get; set; }

        bool ChkCommentOnPinAfterRepinChecked { get; set; }

        RangeUtilities Comments { get; set; }

        RangeUtilities Tries { get; set; }

        bool ChkUploadCommentsChecked { get; set; }

        bool ChkUploadNotesChecked { get; set; }
    }

    [ProtoContract]
    public class RePinModel : ModuleSetting, IRePin, IGeneralSettings
    {
        #region IRePoster

        private bool _chkEnableTryCommentsAfterPostIsTried;
        private bool _chkRemovePoorQualitySources;
        private bool _chkRemoveSourceIfFollowRatioLower;
        private RangeUtilities _tryRange = new RangeUtilities();
        private RangeUtilities _followBackRatio = new RangeUtilities();
        private bool _chkTryOnPinAfterRepinChecked;
        private bool _chkCommentOnPinAfterRepinChecked;
        private RangeUtilities _tries = new RangeUtilities(1, 1);
        private RangeUtilities _comments = new RangeUtilities(1, 1);
        private bool _chkUploadCommentsChecked;
        private bool _chkUploadNotesChecked;
        private List<RepinSelectDestination> _accountPagesBoardsPair;
        private bool _isChkRepinToNumberOfBoards;
        private bool _isChkRepinOnTheSameBoards;
        private int _numberOfBoardsToRepin = 1;
        private int _numberOfTimesToRepinOnSameBoard = 1;

        private ObservableCollection<RepinCreateDestinationSelectModel> _listSelectDestination =
            new ObservableCollection<RepinCreateDestinationSelectModel>();

        private List<string> _listOfBoardUrl = new List<string>();

        public List<string> ListOfBoardUrl
        {
            get => _listOfBoardUrl;
            set
            {
                if (value == _listOfBoardUrl)
                    return;
                SetProperty(ref _listOfBoardUrl, value);
            }
        }

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


        [ProtoMember(5)]
        public bool ChkEnableTryCommentsAfterPostIsTried
        {
            get => _chkEnableTryCommentsAfterPostIsTried;
            set
            {
                if (value == _chkEnableTryCommentsAfterPostIsTried)
                    return;
                SetProperty(ref _chkEnableTryCommentsAfterPostIsTried, value);
            }
        }

        [ProtoMember(6)]
        public bool ChkRemovePoorQualitySources
        {
            get => _chkRemovePoorQualitySources;
            set
            {
                if (value == _chkRemovePoorQualitySources)
                    return;
                SetProperty(ref _chkRemovePoorQualitySources, value);
            }
        }

        [ProtoMember(7)]
        public bool ChkRemoveSourceIfFollowRatioLower
        {
            get => _chkRemoveSourceIfFollowRatioLower;
            set
            {
                if (value == _chkRemoveSourceIfFollowRatioLower)
                    return;
                SetProperty(ref _chkRemoveSourceIfFollowRatioLower, value);
            }
        }

        [ProtoMember(8)]
        public RangeUtilities TryRange
        {
            get => _tryRange;
            set
            {
                if (value == _tryRange)
                    return;
                SetProperty(ref _tryRange, value);
            }
        }

        [ProtoMember(9)]
        public RangeUtilities FollowBackRatio
        {
            get => _followBackRatio;
            set
            {
                if (value == _followBackRatio)
                    return;
                SetProperty(ref _followBackRatio, value);
            }
        }

        private bool _isChkRetryPost;

        [ProtoMember(10)]
        public bool IsChkReTryPost
        {
            get => _isChkRetryPost;
            set
            {
                if (value == _isChkRetryPost)
                    return;
                SetProperty(ref _isChkRetryPost, value);
            }
        }

        public List<string> ListQueryType { get; set; } = new List<string>();

        [ProtoMember(11)]
        public bool ChkCommentOnPinAfterRepinChecked
        {
            get => _chkCommentOnPinAfterRepinChecked;

            set
            {
                if (value == _chkCommentOnPinAfterRepinChecked)
                    return;
                SetProperty(ref _chkCommentOnPinAfterRepinChecked, value);
            }
        }

        [ProtoMember(12)]
        public RangeUtilities Comments
        {
            get => _comments;

            set
            {
                if (value == _comments)
                    return;
                SetProperty(ref _comments, value);
            }
        }

        [ProtoMember(13)]
        public int NumberOfBoardsToRepin
        {
            get => _numberOfBoardsToRepin;
            set
            {
                if (_numberOfBoardsToRepin == value) return;
                SetProperty(ref _numberOfBoardsToRepin, value);
            }
        }

        private RangeUtilities _delayBetweenTriesForAfterActivity = new RangeUtilities(15, 30);

        [ProtoMember(14)]
        public RangeUtilities DelayBetweenTriesForAfterActivity
        {
            get => _delayBetweenTriesForAfterActivity;
            set
            {
                if (_delayBetweenTriesForAfterActivity == value)
                    return;
                SetProperty(ref _delayBetweenTriesForAfterActivity, value);
            }
        }

        [ProtoMember(15)]
        public bool ChkUploadCommentsChecked
        {
            get => _chkUploadCommentsChecked;

            set
            {
                if (value == _chkUploadCommentsChecked)
                    return;
                SetProperty(ref _chkUploadCommentsChecked, value);
            }
        }

        [ProtoMember(16)]
        public bool ChkTryOnPinAfterRepinChecked
        {
            get => _chkTryOnPinAfterRepinChecked;

            set
            {
                if (value == _chkTryOnPinAfterRepinChecked)
                    return;
                SetProperty(ref _chkTryOnPinAfterRepinChecked, value);
            }
        }

        [ProtoMember(17)]
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

        [ProtoMember(18)]
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

        private string _note;

        [ProtoMember(19)]
        public string Note
        {
            get => _note;
            set
            {
                if (_note == value)
                    return;
                SetProperty(ref _note, value);
            }
        }

        private List<string> _lstNotes = new List<string>();

        [ProtoMember(20)]
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

        private string _message;

        [ProtoMember(21)]
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

        private List<string> _lstComments = new List<string>();

        [ProtoMember(22)]
        public List<string> LstComments
        {
            get => _lstComments;
            set
            {
                if (value == _lstComments)
                    return;
                SetProperty(ref _lstComments, value);
            }
        }

        private bool _isChkSkipBlackListedUser;

        [ProtoMember(23)]
        public bool IsChkSkipBlackListedUser
        {
            get => _isChkSkipBlackListedUser;
            set
            {
                if (_isChkSkipBlackListedUser == value) return;
                SetProperty(ref _isChkSkipBlackListedUser, value);
            }
        }

        private bool _isChkPrivateBlackList;

        [ProtoMember(24)]
        public bool IsChkPrivateBlackList
        {
            get => _isChkPrivateBlackList;
            set
            {
                if (_isChkPrivateBlackList == value) return;
                SetProperty(ref _isChkPrivateBlackList, value);
            }
        }

        private bool _isChkGroupBlackList;

        [ProtoMember(25)]
        public bool IsChkGroupBlackList
        {
            get => _isChkGroupBlackList;
            set
            {
                if (_isChkGroupBlackList == value) return;
                SetProperty(ref _isChkGroupBlackList, value);
            }
        }

        [ProtoMember(26)]
        public List<RepinSelectDestination> AccountPagesBoardsPair
        {
            get => _accountPagesBoardsPair;
            set
            {
                if (_accountPagesBoardsPair == value) return;
                SetProperty(ref _accountPagesBoardsPair, value);
            }
        }

        [ProtoMember(27)]
        public bool IsChkRepinToNumberOfBoards
        {
            get => _isChkRepinToNumberOfBoards;
            set
            {
                if (_isChkRepinToNumberOfBoards == value) return;
                SetProperty(ref _isChkRepinToNumberOfBoards, value);
            }
        }

        [ProtoMember(28)]
        public bool IsChkRepinOnTheSameBoard
        {
            get => _isChkRepinOnTheSameBoards;
            set
            {
                if (_isChkRepinOnTheSameBoards == value) return;
                SetProperty(ref _isChkRepinOnTheSameBoards, value);
            }
        }

        [ProtoMember(13)]
        public int NumberOfTimesToRepinOnSameBoard
        {
            get => _numberOfTimesToRepinOnSameBoard;
            set
            {
                if (_numberOfTimesToRepinOnSameBoard == value) return;
                SetProperty(ref _numberOfTimesToRepinOnSameBoard, value);
            }
        }

        public ObservableCollection<RepinCreateDestinationSelectModel> ListSelectDestination
        {
            get => _listSelectDestination;
            set
            {
                if (_listSelectDestination == value)
                    return;
                _listSelectDestination = value;
                OnPropertyChanged(nameof(ListSelectDestination));
            }
        }

        private bool _isSelectPinsFromBoard;

        [ProtoMember(1)]
        public bool IsSelectPinsFromBoard
        {
            get => _isSelectPinsFromBoard;
            set
            {
                if (_isSelectPinsFromBoard == value)
                    return;
                SetProperty(ref _isSelectPinsFromBoard, value);
            }
        }

        private string _comment;

        [ProtoMember(2)]
        public string Comment
        {
            get => _comment;
            set
            {
                if (_comment == value)
                    return;
                SetProperty(ref _comment, value);
            }
        }

        private RangeUtilities _delayBetweenCommentsForAfterActivity = new RangeUtilities(15, 30);

        [ProtoMember(3)]
        public RangeUtilities DelayBetweenCommentsForAfterActivity
        {
            get => _delayBetweenCommentsForAfterActivity;
            set
            {
                if (_delayBetweenCommentsForAfterActivity == value)
                    return;
                SetProperty(ref _delayBetweenCommentsForAfterActivity, value);
            }
        }

        private string _mediaPath = string.Empty;

        [ProtoMember(4)]
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

        private ObservableCollection<Boards> _lstBoardsDetails = new ObservableCollection<Boards>();

        public ObservableCollection<Boards> LstBoardsDetails
        {
            get => _lstBoardsDetails;
            set
            {
                if (_lstBoardsDetails == value)
                    return;
                SetProperty(ref _lstBoardsDetails, value);
            }
        }

        private bool _repinByQueryWithLimit;

        public bool RepinByQueryWithLimit
        {
            get { return _repinByQueryWithLimit; }
            set { SetProperty(ref _repinByQueryWithLimit, value); }
        }

        private int _repinByQueryValue;

        public int RepinByQueryValue
        {
            get { return _repinByQueryValue; }
            set { SetProperty(ref _repinByQueryValue, value); }
        }

        #endregion
    }

    public class RepinSelectDestination
    {
        public string AccountId { get; set; }

        public KeyValuePair<string, List<RepinQueryContent>> LstofPinsToRepin { get; set; }
        public KeyValuePair<string,List<SectionDetails>> LstSection { get; set; }
        public string Label { get; set; }

        public bool IsSelected { get; set; }
    }
}