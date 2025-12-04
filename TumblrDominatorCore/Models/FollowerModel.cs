using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.Generic;

namespace TumblrDominatorCore.Models
{
    public interface IFollowerModel
    {
        #region IFollowerModel

        int IncreaseFollowCount { get; set; }

        RangeUtilities LikeBetweenJobs { get; set; }

        RangeUtilities LikeMaxBetween { get; set; }

        bool IsChkEnableAutoFollowUnfollowChecked { get; set; }

        bool IsChkStopFollowToolWhenReachChecked { get; set; }

        RangeUtilities StopFollowToolWhenReach { get; set; }

        bool IsChkWhenFollowerFollowingsIsSmallerThanChecked { get; set; }

        int FollowerFollowingsMaxValue { get; set; }

        RangeUtilities UnfollowPrevious { get; set; }

        bool IsChkUnfollowUsersChecked { get; set; }

        bool IsChkUnfollowfollowedbackChecked { get; set; }

        bool IsChkUnfollownotfollowedbackChecked { get; set; }


        bool ChkLikeRandomPostsChecked { get; set; }


        RangeUtilities Comments { get; set; }

        RangeUtilities IncreaseEachDayComment { get; set; }

        int CommentPercentage { get; set; }

        bool ChkUploadCommentsChecked { get; set; }

        bool ChkSendDirectMessageAfterFollowChecked { get; set; }

        bool IsChkMaxMessege { get; set; }

        RangeUtilities MessageBetween { get; set; }


        int DirectMessagePercentage { get; set; }

        bool ChkRemovePoorQualitySourcesChecked { get; set; }


        bool IsChkStartUnfollowToolBetweenChecked { get; set; }
        bool ChkAddMessageChecked { get; set; }

        bool IsSkipBlacklistsUser { get; set; }

        bool IsPrivateBlacklists { get; set; }

        bool IsGroupBlacklists { get; set; }
        UserFilterModel UserFilterModel { get; set; }

        #endregion
    }

    [ProtoContract]
    public class FollowerModel : ModuleSetting, IFollowerModel, IGeneralSettings
    {
        public JobConfiguration FastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(66, 100),
            ActivitiesPerHour = new RangeUtilities(6, 10),
            ActivitiesPerWeek = new RangeUtilities(300, 500),
            ActivitiesPerJob = new RangeUtilities(9, 14),
            DelayBetweenJobs = new RangeUtilities(55, 80),
            DelayBetweenActivity = new RangeUtilities(25, 35)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(33, 50),
            ActivitiesPerHour = new RangeUtilities(5, 6),
            ActivitiesPerWeek = new RangeUtilities(200, 300),
            ActivitiesPerJob = new RangeUtilities(4, 6),
            DelayBetweenJobs = new RangeUtilities(60, 100),
            DelayBetweenActivity = new RangeUtilities(30, 40)
        };

        public JobConfiguration SlowSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(16, 25),
            ActivitiesPerHour = new RangeUtilities(3, 4),
            ActivitiesPerWeek = new RangeUtilities(100, 150),
            ActivitiesPerJob = new RangeUtilities(2, 3),
            DelayBetweenJobs = new RangeUtilities(75, 120),
            DelayBetweenActivity = new RangeUtilities(30, 60)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(133, 200),
            ActivitiesPerHour = new RangeUtilities(15, 22),
            ActivitiesPerWeek = new RangeUtilities(800, 1200),
            ActivitiesPerJob = new RangeUtilities(16, 28),
            DelayBetweenJobs = new RangeUtilities(45, 60),
            DelayBetweenActivity = new RangeUtilities(35, 60)
        };

        public List<string> ListQueryType { get; set; } = new List<string>();


        [ProtoMember(2)] public override PostFilterModel BlogFilterModel { get; set; } = new PostFilterModel();

        [ProtoMember(3)] JobConfiguration IGeneralSettings.JobConfiguration { get; set; }

        #region Variables

        private RangeUtilities _likeMaxBetween = new RangeUtilities();
        private RangeUtilities _likeBetweenJobs = new RangeUtilities();
        private bool _isChkEnableAutoFollowUnfollowChecked;
        private bool _isChkStopFollowToolWhenReachChecked;
        private RangeUtilities _stopFollowToolWhenReach = new RangeUtilities();
        private bool _isChkWhenFollowerFollowingsIsSmallerThanChecked;
        private int _followerFollowingsMaxValue;
        private RangeUtilities _unfollowPrevious = new RangeUtilities();
        private bool _isChkUnfollowUsersChecked;
        private bool _isChkUnfollowfollowedbackChecked;

        private bool _isChkUnfollownotfollowedbackChecked;

        //private RangeUtilities _increaseEachDayLike = new RangeUtilities();
        private bool _chkLikeRandomPostsChecked;

        private RangeUtilities _comments = new RangeUtilities();
        private RangeUtilities _increaseEachDayComment = new RangeUtilities();
        private int _commentPercentage;
        private bool _chkUploadCommentsChecked;
        private bool _chkSendDirectMessageAfterFollowChecked;
        private bool _isChkMaxMessege;
        private RangeUtilities _messageBetween = new RangeUtilities();
        private int _directMessagePercentage;
        private bool _chkRemovePoorQualitySourcesChecked;
        private int _increaseFollowCount;
        private bool _isChkStartUnfollowToolBetweenChecked;
        private bool _chkFollowUniqueUsersInCampaign;
        private bool _ischkSkipWhiteListedUser;

        private bool _isSkipBlacklistsUser;
        private bool _isPrivateBlacklists;
        private bool _isGroupBlacklists;
        private UserFilterModel _userFilterModel = new UserFilterModel();

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


        [ProtoMember(7)]
        public RangeUtilities LikeBetweenJobs
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
        public RangeUtilities LikeMaxBetween
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


        [ProtoMember(21)]
        public bool ChkLikeRandomPostsChecked
        {
            get => _chkLikeRandomPostsChecked;

            set
            {
                if (value == _chkLikeRandomPostsChecked)
                    return;
                SetProperty(ref _chkLikeRandomPostsChecked, value);
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

        [ProtoMember(68)]
        public bool IsChkMaxMessege
        {
            get => _isChkMaxMessege;
            set
            {
                if (value == _isChkMaxMessege)
                    return;
                SetProperty(ref _isChkMaxMessege, value);
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


        [ProtoMember(37)]
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

        [ProtoMember(38)]
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

        [ProtoMember(44)]
        public bool IsChkIncreaseEachDay
        {
            get => _isChkIncreaseEachDay;
            set
            {
                if (_isChkIncreaseEachDay == value)
                    return;
                SetProperty(ref _isChkIncreaseEachDay, value);
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

        private bool _isChkMaxComment;

        [ProtoMember(49)]
        public bool IsChkMaxComment
        {
            get => _isChkMaxComment;
            set
            {
                if (_isChkMaxComment == value)
                    return;
                SetProperty(ref _isChkMaxComment, value);
            }
        }

        private bool _isChkMaxLike;

        [ProtoMember(50)]
        public bool IsChkMaxLike
        {
            get => _isChkMaxLike;
            set
            {
                if (_isChkMaxLike == value)
                    return;
                SetProperty(ref _isChkMaxLike, value);
            }
        }

        private bool _isChkLikeUsersLatestPost;

        [ProtoMember(51)]
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

        [ProtoMember(52)]
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

        [ProtoMember(53)]
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


        private bool _chkAddMessageChecked;


        [ProtoMember(63)]
        public bool ChkFollowUniqueUsersInCampaign
        {
            get => _chkFollowUniqueUsersInCampaign;

            set
            {
                if (value == _chkFollowUniqueUsersInCampaign)
                    return;
                SetProperty(ref _chkFollowUniqueUsersInCampaign, value);
            }
        }

        public bool IsChkSkipWhiteListedUser
        {
            get => _ischkSkipWhiteListedUser;

            set
            {
                if (value == _ischkSkipWhiteListedUser)
                    return;
                SetProperty(ref _ischkSkipWhiteListedUser, value);
            }
        }


        [ProtoMember(64)]
        public bool IsSkipBlacklistsUser
        {
            get => _isSkipBlacklistsUser;

            set
            {
                if (value == _isSkipBlacklistsUser)
                    return;
                SetProperty(ref _isSkipBlacklistsUser, value);
            }
        }

        [ProtoMember(65)]
        public bool IsPrivateBlacklists
        {
            get => _isPrivateBlacklists;

            set
            {
                if (value == _isPrivateBlacklists)
                    return;
                SetProperty(ref _isPrivateBlacklists, value);
            }
        }

        [ProtoMember(66)]
        public bool IsGroupBlacklists
        {
            get => _isGroupBlacklists;

            set
            {
                if (value == _isGroupBlacklists)
                    return;
                SetProperty(ref _isGroupBlacklists, value);
            }
        }

        //UserFilterModel
        [ProtoMember(67)]
        public UserFilterModel UserFilterModel
        {
            get => _userFilterModel;

            set
            {
                if (value == _userFilterModel)
                    return;
                SetProperty(ref _userFilterModel, value);
            }
        }

        #endregion
    }
}