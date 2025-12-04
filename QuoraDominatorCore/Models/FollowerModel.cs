using System.Collections.Generic;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace QuoraDominatorCore.Models
{
    [ProtoContract]
    public class FollowerModel : ModuleSetting, IGeneralSettings
    {
        private RangeUtilities _commentMax = new RangeUtilities();
        private bool _ischkGroupblacklist;

        private bool _isChkIncreaseEachDayMessage;
        private bool _isChkMaxMessage;
        private bool _isChkMaxPerDayMessage;

        private bool _ischkprivateblacklist;
        private bool _ischkUpvotePerday;
        private bool _isFollowUniqueUsersInCampaignChecked;

        private bool _isPerDayComment;
        private RangeUtilities _messageMax = new RangeUtilities();

        private List<string> _privateBlackedlist = new List<string>();

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

        [ProtoMember(3)] public override PostFilterModel PostFilterModel { get; set; } = new PostFilterModel();

        [ProtoMember(63)]
        public bool IsChkPrivateblacklist
        {
            get => _ischkprivateblacklist;
            set
            {
                if (_ischkprivateblacklist == value)
                    return;
                SetProperty(ref _ischkprivateblacklist, value);
            }
        }

        public List<string> PrivateBlackListUser
        {
            get => _privateBlackedlist;
            set
            {
                if (_privateBlackedlist == value) return;
                SetProperty(ref _privateBlackedlist, value);
            }
        }

        [ProtoMember(64)]
        public bool IsChkUpvotePerDay
        {
            get => _ischkUpvotePerday;
            set
            {
                if (_ischkUpvotePerday == value)
                    return;
                SetProperty(ref _ischkUpvotePerday, value);
            }
        }

        [ProtoMember(65)]
        public bool IsChkIncreaseEachDayMessage
        {
            get => _isChkIncreaseEachDayMessage;

            set
            {
                if (value == _isChkIncreaseEachDayMessage)
                    return;
                SetProperty(ref _isChkIncreaseEachDayMessage, value);
            }
        }

        [ProtoMember(66)]
        public bool IsChkMaxPerDayMessage
        {
            get => _isChkMaxPerDayMessage;
            set
            {
                if (value == _isChkMaxPerDayMessage)
                    return;
                SetProperty(ref _isChkMaxPerDayMessage, value);
            }
        }

        [ProtoMember(67)]
        public RangeUtilities CommentMax
        {
            get => _commentMax;
            set
            {
                if (_commentMax != value)
                    SetProperty(ref _commentMax, value);
            }
        }

        [ProtoMember(68)]
        public bool IsChkPerDayComment
        {
            get => _isPerDayComment;
            set
            {
                if (_isPerDayComment == value)
                    return;
                SetProperty(ref _isPerDayComment, value);
            }
        }

        [ProtoMember(69)]
        public bool IsChkMaxMessage
        {
            get => _isChkMaxMessage;
            set
            {
                if (_isChkMaxMessage == value)
                    return;
                SetProperty(ref _isChkMaxMessage, value);
            }
        }

        [ProtoMember(70)]
        public RangeUtilities MessageMax
        {
            get => _messageMax;
            set
            {
                if (_messageMax != value)
                    SetProperty(ref _messageMax, value);
            }
        }

        [ProtoMember(71)]
        public bool IsChkGroupblacklist
        {
            get => _ischkGroupblacklist;
            set
            {
                if (_ischkGroupblacklist == value)
                    return;
                SetProperty(ref _ischkGroupblacklist, value);
            }
        }

        [ProtoMember(71)]
        public bool IsFollowUniqueUsersInCampaignChecked
        {
            get => _isFollowUniqueUsersInCampaignChecked;
            set
            {
                if (_isFollowUniqueUsersInCampaignChecked == value)
                    return;
                SetProperty(ref _isFollowUniqueUsersInCampaignChecked, value);
            }
        }

        [ProtoMember(4)] JobConfiguration IGeneralSettings.JobConfiguration { get; set; }

        #region Variables

        private RangeUtilities _likeMaxBetween = new RangeUtilities();
        private int _followCountUntil;
        private RangeUtilities _likeBetweenJobs = new RangeUtilities();
        private bool _isChkEnableAutoFollowUnfollowChecked;
        private bool _isChkStopFollowToolWhenReachChecked;
        private RangeUtilities _stopFollowToolWhenReach = new RangeUtilities();
        private RangeUtilities _startUnfollow = new RangeUtilities();
        private bool _isChkWhenFollowerFollowingsIsSmallerThanChecked;
        private int _followerFollowingsMaxValue;
        private bool _isChkFollowToolGetsTemporaryBlockedChecked;
        private RangeUtilities _unfollowPrevious = new RangeUtilities();
        private bool _isChkUnfollowUsersChecked;
        private bool _isChkUnfollowfollowedbackChecked;
        private bool _isChkUnfollownotfollowedbackChecked;
        private bool _chkCommentOnUserLatestPostsChecked;
        private RangeUtilities _comments = new RangeUtilities();
        private int _commentPercentage;
        private bool _chkSendDirectMessageAfterFollowChecked;
        private RangeUtilities _messageBetween = new RangeUtilities();
        private RangeUtilities _increaseEachDayMessage = new RangeUtilities();
        private int _directMessagePercentage;
        private bool _chkRemovePoorQualitySourcesChecked;
        private RangeUtilities _followBackRatio = new RangeUtilities();
        private bool _chkIgnoreAddingSourcesPreviouslyRemovedChecked;
        private int _increaseFollowCount;

        #endregion

        #region IFollowerModel

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
        public RangeUtilities UpvoteMax
        {
            get => _likeBetweenJobs;
            set
            {
                if (value == _likeBetweenJobs)
                    return;
                SetProperty(ref _likeBetweenJobs, value);
            }
        }

        [ProtoMember(8)]
        public RangeUtilities UpvotePerDay
        {
            get => _likeMaxBetween;

            set
            {
                if (value == _likeMaxBetween)
                    return;
                SetProperty(ref _likeMaxBetween, value);
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
        public bool IsChkWhenFollowerFollowingsIsSmallerThanChecked
        {
            get => _isChkWhenFollowerFollowingsIsSmallerThanChecked;

            set
            {
                if (value == _isChkWhenFollowerFollowingsIsSmallerThanChecked)
                    return;
                SetProperty(ref _isChkWhenFollowerFollowingsIsSmallerThanChecked, value);
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

        [ProtoMember(27)]
        public bool ChkSendDirectMessageAfterFollowChecked
        {
            get => _chkSendDirectMessageAfterFollowChecked;

            set
            {
                if (value == _chkSendDirectMessageAfterFollowChecked)
                    return;
                SetProperty(ref _chkSendDirectMessageAfterFollowChecked, value);
            }
        }

        [ProtoMember(28)]
        public bool ChkAddMessageChecked
        {
            get => _chkAddMessageChecked;

            set
            {
                if (value == _chkAddMessageChecked)
                    return;
                SetProperty(ref _chkAddMessageChecked, value);
            }
        }


        [ProtoMember(29)]
        public RangeUtilities MessageBetween
        {
            get => _messageBetween;

            set
            {
                if (value == _messageBetween)
                    return;
                SetProperty(ref _messageBetween, value);
            }
        }

        [ProtoMember(30)]
        public RangeUtilities IncreaseEachDayMessage
        {
            get => _increaseEachDayMessage;

            set
            {
                if (value == _increaseEachDayMessage)
                    return;
                SetProperty(ref _increaseEachDayMessage, value);
            }
        }

        [ProtoMember(31)]
        public int DirectMessagePercentage
        {
            get => _directMessagePercentage;

            set
            {
                if (value == _directMessagePercentage)
                    return;
                SetProperty(ref _directMessagePercentage, value);
            }
        }

        [ProtoMember(34)]
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

        [ProtoMember(35)]
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

        [ProtoMember(36)]
        public bool ChkIgnoreAddingSourcesPreviouslyRemovedChecked
        {
            get => _chkIgnoreAddingSourcesPreviouslyRemovedChecked;

            set
            {
                if (value == _chkIgnoreAddingSourcesPreviouslyRemovedChecked)
                    return;
                SetProperty(ref _chkIgnoreAddingSourcesPreviouslyRemovedChecked, value);
            }
        }

        private List<string> _lstMessages = new List<string>();

        [ProtoMember(39)]
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

        [ProtoMember(40)]
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

        [ProtoMember(41)]
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

        private bool _isChkAcceptBetween;

        private bool _isChkDirectMessagePercentage;

        [ProtoMember(43)]
        public bool IsChkDirectMessagePercentage
        {
            get => _isChkDirectMessagePercentage;
            set
            {
                if (_isChkDirectMessagePercentage == value)
                    return;
                SetProperty(ref _isChkDirectMessagePercentage, value);
            }
        }

        private bool _isChkIncreaseEachDay;


        private bool _isChkMaxMessege;

        [ProtoMember(45)]
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

        [ProtoMember(46)]
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

        private string _uploadComment;

        [ProtoMember(47)]
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

        [ProtoMember(48)]
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


        private bool _isChkMaxComment;

        private bool _isChkIncreaseEachDayUpvote;

        [ProtoMember(51)]
        public bool IsChkIncreaseEachDayUpvote
        {
            get => _isChkIncreaseEachDayUpvote;
            set
            {
                if (_isChkIncreaseEachDayUpvote == value)
                    return;
                SetProperty(ref _isChkIncreaseEachDayUpvote, value);
            }
        }

        private bool _isChkMaxUpvote;

        [ProtoMember(52)]
        public bool IsChkMaxUpvote
        {
            get => _isChkMaxUpvote;
            set
            {
                if (_isChkMaxUpvote == value)
                    return;
                SetProperty(ref _isChkMaxUpvote, value);
            }
        }

        private bool _isChkLikeUsersLatestPost;

        [ProtoMember(53)]
        public bool IsChkLikeUsersLatestPost
        {
            get => _isChkLikeUsersLatestPost;
            set
            {
                if (_isChkLikeUsersLatestPost == value)
                    return;
                SetProperty(ref _isChkLikeUsersLatestPost, value);
            }
        }

        private bool _isChkStopFollow;
        private bool _isChkStartUnFollow;

        private bool _isChkStartUnFollowWhenReached;

        [ProtoMember(56)]
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

        [ProtoMember(57)]
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
        private int _unFollowerFollowingsMaxValue;

        [ProtoMember(59)]
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

        private bool _ischkWhenTheUnFollowToolGetsTemporaryBlocked;

        private bool _chkUploadMessageChecked;
        private bool _chkAddMessageChecked;

        #endregion

        private bool _isChkSkipPrivateBlackList;
        private bool _isChkSkipGroupBlackList;
        public bool IsChkFollowerSkipPrivateBlacklist
        {
            get => _isChkSkipPrivateBlackList;
            set
            {
                if (_isChkSkipPrivateBlackList != value)
                    SetProperty(ref _isChkSkipPrivateBlackList, value);
            }
        }

        public bool IsChkFollowerSkipGroupBlacklist
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