using System.Collections.Generic;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace PinDominatorCore.PDModel
{
    public interface IFollowerModel
    {
        #region IFollowerModel

        int IncreaseFollowCount { get; set; }

        int FollowCountUntil { get; set; }

        RangeUtilities TryBetweenJobs { get; set; }

        RangeUtilities TryMaxBetween { get; set; }

        bool IsChkEnableAutoFollowUnfollowChecked { get; set; }

        bool IsChkStopFollowToolWhenReachChecked { get; set; }

        RangeUtilities StopFollowToolWhenReach { get; set; }

        RangeUtilities StartUnfollow { get; set; }

        int FollowerFollowingsMaxValue { get; set; }

        bool IsChkFollowToolGetsTemporaryBlockedChecked { get; set; }

        RangeUtilities UnfollowPrevious { get; set; }

        bool IsChkUnfollowUsersChecked { get; set; }

        bool IsChkUnfollowfollowedbackChecked { get; set; }

        bool IsChkUnfollownotfollowedbackChecked { get; set; }

        RangeUtilities IncreaseEachDayTry { get; set; }

        bool ChkTryUserLatestPostsChecked { get; set; }

        bool ChkCommentOnUserLatestPostsChecked { get; set; }

        RangeUtilities Comments { get; set; }

        RangeUtilities Tries { get; set; }

        RangeUtilities IncreaseEachDayComment { get; set; }

        int CommentPercentage { get; set; }

        int TryPercentage { get; set; }

        bool ChkUploadCommentsChecked { get; set; }

        bool ChkUploadNotesChecked { get; set; }

        bool ChkRemovePoorQualitySourcesChecked { get; set; }

        RangeUtilities FollowBackRatio { get; set; }

        bool ChkIgnoreAddingSourcesPrevioslyRemovedChecked { get; set; }

        bool IsChkStartUnfollowToolBetweenChecked { get; set; }

        #endregion
    }

    [ProtoContract]
    public class FollowerModel : ModuleSetting, IFollowerModel, IGeneralSettings
    {
        private bool _isChkFollowUnique;


        public List<string> ListQueryType { get; set; } = new List<string>();

        [ProtoMember(61)]
        public RangeUtilities StopUnFollowToolWhenReach
        {
            get => _stopUnFollowToolWhenReach;

            set
            {
                if (value == _stopUnFollowToolWhenReach)
                    return;
                SetProperty(ref _stopUnFollowToolWhenReach, value);
            }
        }

        [ProtoMember(62)]
        public bool IsChkFollowUnique
        {
            get => _isChkFollowUnique;

            set
            {
                if (value == _isChkFollowUnique)
                    return;
                SetProperty(ref _isChkFollowUnique, value);
            }
        }

        [ProtoMember(63)]
        public RangeUtilities DelayBetweenCommentsForAfterActivity
        {
            get => _delayBetweenCommentsForAfterActivity;

            set
            {
                if (value == _delayBetweenCommentsForAfterActivity)
                    return;
                SetProperty(ref _delayBetweenCommentsForAfterActivity, value);
            }
        }

        #region Variables

        private RangeUtilities _tryMaxBetween = new RangeUtilities();
        private int _followCountUntil;
        private RangeUtilities _tryBetweenJobs = new RangeUtilities();
        private bool _isChkEnableAutoFollowUnfollowChecked;
        private bool _isChkStopFollowTool;
        private bool _isChkStartUnFollowTool;
        private bool _isChkStopFollowToolWhenReachChecked;
        private bool _isChkStopUnFollowToolWhenReachChecked;
        private RangeUtilities _stopFollowToolWhenReach = new RangeUtilities(1, 1);
        private RangeUtilities _stopUnFollowToolWhenReach = new RangeUtilities();
        private RangeUtilities _startUnfollow = new RangeUtilities();
        private bool _isChkWhenFollowerFollowingsGreater;
        private int _followerFollowingsMaxValue = 1;
        private bool _isChkFollowToolGetsTemporaryBlockedChecked;
        private RangeUtilities _unfollowPrevious = new RangeUtilities();
        private bool _isChkUnfollowUsersChecked;
        private bool _isChkUnfollowfollowedbackChecked;
        private bool _isChkUnfollownotfollowedbackChecked;
        private RangeUtilities _increaseEachDayTry = new RangeUtilities();
        private bool _chkTryUserLatestPostsChecked;
        private bool _chkCommentOnUserLatestPostsChecked;
        private RangeUtilities _tries = new RangeUtilities(1, 1);
        private RangeUtilities _comments = new RangeUtilities(1, 1);
        private RangeUtilities _increaseEachDayComment = new RangeUtilities();
        private int _commentPercentage;
        private int _tryPercentage;
        private bool _chkUploadCommentsChecked;
        private bool _chkUploadNotesChecked;

        private RangeUtilities _messageBetween = new RangeUtilities();
        private RangeUtilities _increaseEachDayMessage = new RangeUtilities();

        private RangeUtilities _acceptBetween = new RangeUtilities();
        private bool _chkRemovePoorQualitySourcesChecked;

        private RangeUtilities _followBackRatio = new RangeUtilities();
        private bool _chkIgnoreAddingSourcesPrevioslyRemovedChecked;
        private int _increaseFollowCount;

        private bool _isChkStartUnfollowToolBetweenChecked;
        private RangeUtilities _delayBetweenCommentsForAfterActivity = new RangeUtilities(15, 30);
        private RangeUtilities _delayBetweenTriesForAfterActivity = new RangeUtilities(15, 30);
        private string _mediaPath = string.Empty;

        #endregion


        #region IFollowerModel

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

        private bool _isChkSkipBlackListedUser;

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

        private bool _isChkPrivateBlackList;

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

        private bool _isChkGroupBlackList;

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

        [ProtoMember(5)]
        public int IncreaseFollowCount
        {
            get => _increaseFollowCount;
            set
            {
                if (value == _increaseFollowCount)
                    return;
                SetProperty(ref _increaseFollowCount, value);
            }
        }

        [ProtoMember(6)]
        public int FollowCountUntil
        {
            get => _followCountUntil;
            set
            {
                if (value == _followCountUntil)
                    return;
                SetProperty(ref _followCountUntil, value);
            }
        }

        [ProtoMember(7)]
        public RangeUtilities TryBetweenJobs
        {
            get => _tryBetweenJobs;
            set
            {
                if (value == _tryBetweenJobs)
                    return;
                SetProperty(ref _tryBetweenJobs, value);
            }
        }

        [ProtoMember(8)]
        public RangeUtilities TryMaxBetween
        {
            get => _tryMaxBetween;

            set
            {
                if (value == _tryMaxBetween)
                    return;
                SetProperty(ref _tryMaxBetween, value);
            }
        }

        [ProtoMember(9)]
        public bool IsChkEnableAutoFollowUnfollowChecked
        {
            get => _isChkEnableAutoFollowUnfollowChecked;

            set
            {
                if (value == _isChkEnableAutoFollowUnfollowChecked)
                    return;
                SetProperty(ref _isChkEnableAutoFollowUnfollowChecked, value);
            }
        }

        [ProtoMember(10)]
        public bool IsChkStopFollowToolWhenReachChecked
        {
            get => _isChkStopFollowToolWhenReachChecked;

            set
            {
                if (value == _isChkStopFollowToolWhenReachChecked)
                    return;
                SetProperty(ref _isChkStopFollowToolWhenReachChecked, value);
            }
        }

        [ProtoMember(11)]
        public RangeUtilities StopFollowToolWhenReach
        {
            get => _stopFollowToolWhenReach;

            set
            {
                if (value == _stopFollowToolWhenReach)
                    return;
                SetProperty(ref _stopFollowToolWhenReach, value);
            }
        }

        [ProtoMember(12)]
        public RangeUtilities StartUnfollow
        {
            get => _startUnfollow;

            set
            {
                if (value == _startUnfollow)
                    return;
                SetProperty(ref _startUnfollow, value);
            }
        }

        [ProtoMember(13)]
        public bool IsChkWhenFollowerFollowingsGreater
        {
            get => _isChkWhenFollowerFollowingsGreater;

            set
            {
                if (value == _isChkWhenFollowerFollowingsGreater)
                    return;
                SetProperty(ref _isChkWhenFollowerFollowingsGreater, value);
            }
        }

        [ProtoMember(14)]
        public int FollowerFollowingsMaxValue
        {
            get => _followerFollowingsMaxValue;

            set
            {
                if (value == _followerFollowingsMaxValue)
                    return;
                SetProperty(ref _followerFollowingsMaxValue, value);
            }
        }

        [ProtoMember(15)]
        public bool IsChkFollowToolGetsTemporaryBlockedChecked
        {
            get => _isChkFollowToolGetsTemporaryBlockedChecked;

            set
            {
                if (value == _isChkFollowToolGetsTemporaryBlockedChecked)
                    return;
                SetProperty(ref _isChkFollowToolGetsTemporaryBlockedChecked, value);
            }
        }

        [ProtoMember(16)]
        public RangeUtilities UnfollowPrevious
        {
            get => _unfollowPrevious;

            set
            {
                if (value == _unfollowPrevious)
                    return;
                SetProperty(ref _unfollowPrevious, value);
            }
        }

        [ProtoMember(17)]
        public bool IsChkUnfollowUsersChecked
        {
            get => _isChkUnfollowUsersChecked;

            set
            {
                if (value == _isChkUnfollowUsersChecked)
                    return;
                SetProperty(ref _isChkUnfollowUsersChecked, value);
            }
        }

        [ProtoMember(18)]
        public bool IsChkUnfollowfollowedbackChecked
        {
            get => _isChkUnfollowfollowedbackChecked;

            set
            {
                if (value == _isChkUnfollowfollowedbackChecked)
                    return;
                SetProperty(ref _isChkUnfollowfollowedbackChecked, value);
            }
        }

        [ProtoMember(19)]
        public bool IsChkUnfollownotfollowedbackChecked
        {
            get => _isChkUnfollownotfollowedbackChecked;

            set
            {
                if (value == _isChkUnfollownotfollowedbackChecked)
                    return;
                SetProperty(ref _isChkUnfollownotfollowedbackChecked, value);
            }
        }

        [ProtoMember(20)]
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

        [ProtoMember(21)]
        public bool ChkTryUserLatestPostsChecked
        {
            get => _chkTryUserLatestPostsChecked;

            set
            {
                if (value == _chkTryUserLatestPostsChecked)
                    return;
                SetProperty(ref _chkTryUserLatestPostsChecked, value);
            }
        }

        [ProtoMember(22)]
        public bool ChkCommentOnUserLatestPostsChecked
        {
            get => _chkCommentOnUserLatestPostsChecked;

            set
            {
                if (value == _chkCommentOnUserLatestPostsChecked)
                    return;
                SetProperty(ref _chkCommentOnUserLatestPostsChecked, value);
            }
        }

        [ProtoMember(23)]
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

        [ProtoMember(24)]
        public RangeUtilities IncreaseEachDayComment
        {
            get => _increaseEachDayComment;

            set
            {
                if (value == _increaseEachDayComment)
                    return;
                SetProperty(ref _increaseEachDayComment, value);
            }
        }

        [ProtoMember(25)]
        public int CommentPercentage
        {
            get => _commentPercentage;

            set
            {
                if (value == _commentPercentage)
                    return;
                SetProperty(ref _commentPercentage, value);
            }
        }

        [ProtoMember(26)]
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


        [ProtoMember(27)]
        public bool ChkRemovePoorQualitySourcesChecked
        {
            get => _chkRemovePoorQualitySourcesChecked;

            set
            {
                if (value == _chkRemovePoorQualitySourcesChecked)
                    return;
                SetProperty(ref _chkRemovePoorQualitySourcesChecked, value);
            }
        }

        [ProtoMember(28)]
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

        [ProtoMember(29)]
        public bool ChkIgnoreAddingSourcesPrevioslyRemovedChecked
        {
            get => _chkIgnoreAddingSourcesPrevioslyRemovedChecked;

            set
            {
                if (value == _chkIgnoreAddingSourcesPrevioslyRemovedChecked)
                    return;
                SetProperty(ref _chkIgnoreAddingSourcesPrevioslyRemovedChecked, value);
            }
        }

        [ProtoMember(30)]
        public bool IsChkStartUnfollowToolBetweenChecked
        {
            get => _isChkStartUnfollowToolBetweenChecked;
            set
            {
                if (value == _isChkStartUnfollowToolBetweenChecked)
                    return;
                SetProperty(ref _isChkStartUnfollowToolBetweenChecked, value);
            }
        }

        private List<string> _lstComments = new List<string>();

        [ProtoMember(31)]
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

        private List<string> _lstMessages = new List<string>();

        [ProtoMember(32)]
        public List<string> LstMessages
        {
            get => _lstMessages;
            set
            {
                if (value == _lstMessages)
                    return;
                SetProperty(ref _lstMessages, value);
            }
        }

        private bool _isAddedToCampaign;

        [ProtoMember(33)]
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

        private bool _isChkFollowBackRatio;

        [ProtoMember(34)]
        public bool IsChkFollowBackRatio
        {
            get => _isChkFollowBackRatio;
            set
            {
                if (_isChkFollowBackRatio == value)
                    return;
                SetProperty(ref _isChkFollowBackRatio, value);
            }
        }

        private string _uploadComment;

        private bool _isChkMaxMessege;

        [ProtoMember(51)]
        public bool IsChkMaxMessege
        {
            get => _isChkMaxMessege;
            set
            {
                if (_isChkMaxMessege == value)
                    return;
                SetProperty(ref _isChkMaxMessege, value);
            }
        }


        private string _message;

        [ProtoMember(52)]
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

        [ProtoMember(35)]
        public string UploadComment
        {
            get => _uploadComment;
            set
            {
                if (_uploadComment == value)
                    return;
                SetProperty(ref _uploadComment, value);
            }
        }


        private bool _isChkCommentPercentage;

        [ProtoMember(36)]
        public bool IsChkCommentPercentage
        {
            get => _isChkCommentPercentage;
            set
            {
                if (_isChkCommentPercentage == value)
                    return;
                SetProperty(ref _isChkCommentPercentage, value);
            }
        }

        private bool _isChkIncreaseEachDayComment;

        [ProtoMember(37)]
        public bool IsChkIncreaseEachDayComment
        {
            get => _isChkIncreaseEachDayComment;
            set
            {
                if (_isChkIncreaseEachDayComment == value)
                    return;
                SetProperty(ref _isChkIncreaseEachDayComment, value);
            }
        }

        [ProtoMember(38)]
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

        private bool _isChkIncreaseEachDayTry;

        [ProtoMember(39)]
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

        [ProtoMember(40)]
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

        private bool _isChkTryUsersLatestPost;

        [ProtoMember(41)]
        public bool IsChkTryUsersLatestPost
        {
            get => _isChkTryUsersLatestPost;
            set
            {
                if (_isChkTryUsersLatestPost == value)
                    return;
                SetProperty(ref _isChkTryUsersLatestPost, value);
            }
        }

        private bool _isChkStopFollow;

        [ProtoMember(42)]
        public bool IsChkStopFollow
        {
            get => _isChkStopFollow;

            set
            {
                if (value == _isChkStopFollow)
                    return;
                SetProperty(ref _isChkStopFollow, value);
            }
        }

        private bool _isChkStartUnFollow;

        [ProtoMember(43)]
        public bool IsChkStartUnFollow
        {
            get => _isChkStartUnFollow;

            set
            {
                if (value == _isChkStartUnFollow)
                    return;
                SetProperty(ref _isChkStartUnFollow, value);
            }
        }

        private bool _isChkStartUnFollowWhenReached;

        [ProtoMember(44)]
        public bool IsChkStartUnFollowWhenReached
        {
            get => _isChkStartUnFollowWhenReached;

            set
            {
                if (value == _isChkStartUnFollowWhenReached)
                    return;
                SetProperty(ref _isChkStartUnFollowWhenReached, value);
            }
        }

        private RangeUtilities _startUnFollowToolWhenReach = new RangeUtilities();

        [ProtoMember(45)]
        public RangeUtilities StartUnFollowToolWhenReach
        {
            get => _startUnFollowToolWhenReach;

            set
            {
                if (value == _startUnFollowToolWhenReach)
                    return;
                SetProperty(ref _startUnFollowToolWhenReach, value);
            }
        }

        private bool _isChkWhenFollowerFollowingsIsSmallerThan;

        [ProtoMember(46)]
        public bool IsChkWhenFollowerFollowingsIsSmallerThan
        {
            get => _isChkWhenFollowerFollowingsIsSmallerThan;

            set
            {
                if (value == _isChkWhenFollowerFollowingsIsSmallerThan)
                    return;
                SetProperty(ref _isChkWhenFollowerFollowingsIsSmallerThan, value);
            }
        }

        private int _unFollowerFollowingsMaxValue;

        [ProtoMember(47)]
        public int UnFollowerFollowingsMaxValue
        {
            get => _unFollowerFollowingsMaxValue;

            set
            {
                if (value == _unFollowerFollowingsMaxValue)
                    return;
                SetProperty(ref _unFollowerFollowingsMaxValue, value);
            }
        }

        private bool _ischkwhenTheUnFollowToolGetsTemporaryBlocked;

        [ProtoMember(48)]
        public bool IschkwhenTheUnFollowToolGetsTemporaryBlocked
        {
            get => _ischkwhenTheUnFollowToolGetsTemporaryBlocked;

            set
            {
                if (value == _ischkwhenTheUnFollowToolGetsTemporaryBlocked)
                    return;
                SetProperty(ref _ischkwhenTheUnFollowToolGetsTemporaryBlocked, value);
            }
        }

        private RangeUtilities _commentsPerUserPerUser = new RangeUtilities();

        [ProtoMember(49)]
        public RangeUtilities CommentsPerUser
        {
            get => _commentsPerUserPerUser;

            set
            {
                if (value == _commentsPerUserPerUser)
                    return;
                SetProperty(ref _commentsPerUserPerUser, value);
            }
        }

        private bool _chkUploadMessageChecked;

        [ProtoMember(50)]
        public bool ChkUploadMessageChecked
        {
            get => _chkUploadMessageChecked;

            set
            {
                if (value == _chkUploadMessageChecked)
                    return;
                SetProperty(ref _chkUploadMessageChecked, value);
            }
        }

        [ProtoMember(51)]
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

        private bool _isChkTryPercentage;

        [ProtoMember(52)]
        public bool IsChkTryPercentage
        {
            get => _isChkTryPercentage;
            set
            {
                if (_isChkTryPercentage == value)
                    return;
                SetProperty(ref _isChkTryPercentage, value);
            }
        }

        [ProtoMember(53)]
        public int TryPercentage
        {
            get => _tryPercentage;

            set
            {
                if (value == _tryPercentage)
                    return;
                SetProperty(ref _tryPercentage, value);
            }
        }

        private RangeUtilities _triesPerUser = new RangeUtilities();

        [ProtoMember(54)]
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

        [ProtoMember(55)]
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

        [ProtoMember(56)]
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

        [ProtoMember(57)]
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

        [ProtoMember(58)]
        public bool IsChkStopFollowTool
        {
            get => _isChkStopFollowTool;

            set
            {
                if (value == _isChkStopFollowTool)
                    return;
                SetProperty(ref _isChkStopFollowTool, value);
            }
        }

        [ProtoMember(59)]
        public bool IsChkStartUnFollowToolStopFollow
        {
            get => _isChkStartUnFollowTool;

            set
            {
                if (value == _isChkStartUnFollowTool)
                    return;
                SetProperty(ref _isChkStartUnFollowTool, value);
            }
        }

        [ProtoMember(60)]
        public bool IsChkStopUnFollowToolWhenReachChecked
        {
            get => _isChkStopUnFollowToolWhenReachChecked;

            set
            {
                if (value == _isChkStopUnFollowToolWhenReachChecked)
                    return;
                SetProperty(ref _isChkStopUnFollowToolWhenReachChecked, value);
            }
        }

        #endregion
    }
}