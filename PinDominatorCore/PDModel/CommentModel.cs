using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace PinDominatorCore.PDModel
{
    internal interface IComment
    {
        bool IsChkAfterCommentAction { get; set; }
        bool IsChkMentionNonExistingUsers { get; set; }
        bool IsChkMentionRandomUsers { get; set; }
        bool IsChkMultipleCommentsOnSamePost { get; set; }
        bool IsChkCommentsOnOwnPost { get; set; }
        bool IsChkTryPostAfterComment { get; set; }
        bool IsChkFollowUserAfterComment { get; set; }
        bool IsChkRemovePoorQuality { get; set; }
        bool IsChkRemoveIfFollowBackRatioLimit { get; set; }
        bool IsChkSuspendToolWhenGotBlock { get; set; }
        RangeUtilities RandomlyGeneratedUsers { get; set; }
        int RemoveIfMinFollowBackRatioValue { get; set; }
        int RemoveIfFollowBackRatioAfterCommentValue { get; set; }
        int SuspendToolWhenGotBlockedValue { get; set; }
        bool ChkCommentOnUserLatestPostsChecked { get; set; }
        bool ChkLikeOthersCommentChecked { get; set; }
        RangeUtilities Comments { get; set; }
        RangeUtilities IncreaseEachDayComment { get; set; }
        bool ChkUploadCommentsChecked { get; set; }
    }

    [ProtoContract]
    public class CommentModel : ModuleSetting, IComment, IGeneralSettings
    {
        private bool _chkCommentOnUserLatestPostsChecked;
        private bool _chkLikeOthersCommentChecked;
        private bool _chkUploadCommentsChecked;
        private int _commentPercentage;
        private RangeUtilities _comments = new RangeUtilities(1, 1);
        private RangeUtilities _commentsPerUser = new RangeUtilities();
        private RangeUtilities _delayBetweenCommentsForAfterActivity = new RangeUtilities(15, 30);
        private RangeUtilities _delayBetweenLikeCommentsForAfterActivity = new RangeUtilities(15, 30);
        private RangeUtilities _increaseEachDayComment = new RangeUtilities();
        private RangeUtilities _increaseEachDayLike = new RangeUtilities();

        private bool _isAddedToCampaign;

        private bool _isChkAddMultipleComments;

        private bool _isChkAllowMultipleCommentsOnSamePost;
        private bool _isChkCommentPercentage;

        private bool _isChkGroupBlackList;
        private bool _isChkIncreaseEachDayComment;
        private bool _isChkIncreaseEachDayLike;

        private bool _isChkPrivateBlackList;

        private bool _isChkSkipBlackListedUser;
        private bool _isMakeCommentAsSpinText;
        private RangeUtilities _likes = new RangeUtilities(1, 1);
        private List<string> _lstComments = new List<string>();
        private string _message;

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

        private bool _isUniqueComment;

        [ProtoMember(1)]
        public bool IsUniqueComment
        {
            get => _isUniqueComment;
            set
            {
                if (_isUniqueComment == value)
                    return;
                SetProperty(ref _isUniqueComment, value);
            }
        }

        private bool _isPostUniqueCommentFromEachAccount;

        [ProtoMember(2)]
        public bool IsPostUniqueCommentFromEachAccount
        {
            get => _isPostUniqueCommentFromEachAccount;
            set
            {
                if (_isPostUniqueCommentFromEachAccount == value)
                    return;
                SetProperty(ref _isPostUniqueCommentFromEachAccount, value);
            }
        }

        private bool _isPostUniqueCommentOnceFromAccount;

        [ProtoMember(3)]
        public bool IsPostUniqueCommentOnceFromAccount
        {
            get => _isPostUniqueCommentOnceFromAccount;
            set
            {
                if (_isPostUniqueCommentOnceFromAccount == value)
                    return;
                SetProperty(ref _isPostUniqueCommentOnceFromAccount, value);
            }
        }

        private bool _isChkAfterCommentAction;

        [ProtoMember(5)]
        public bool IsChkAfterCommentAction
        {
            get => _isChkAfterCommentAction;
            set
            {
                if (_isChkAfterCommentAction == value)
                    return;
                SetProperty(ref _isChkAfterCommentAction, value);
            }
        }

        private bool _isChkMentionNonExistingUsers;

        [ProtoMember(6)]
        public bool IsChkMentionNonExistingUsers
        {
            get => _isChkMentionNonExistingUsers;
            set
            {
                if (_isChkMentionNonExistingUsers == value)
                    return;
                SetProperty(ref _isChkMentionNonExistingUsers, value);
            }
        }

        private bool _isChkMentionRandomUsers;

        [ProtoMember(7)]
        public bool IsChkMentionRandomUsers
        {
            get => _isChkMentionRandomUsers;
            set
            {
                if (_isChkMentionRandomUsers == value)
                    return;
                SetProperty(ref _isChkMentionRandomUsers, value);
            }
        }

        private bool _isChkMultipleCommentsOnSamePost;

        [ProtoMember(8)]
        public bool IsChkMultipleCommentsOnSamePost
        {
            get => _isChkMultipleCommentsOnSamePost;
            set
            {
                if (_isChkMultipleCommentsOnSamePost == value)
                    return;
                SetProperty(ref _isChkMultipleCommentsOnSamePost, value);
            }
        }


        private bool _isChkCommentsOnOwnPost;

        [ProtoMember(9)]
        public bool IsChkCommentsOnOwnPost
        {
            get => _isChkCommentsOnOwnPost;
            set
            {
                if (_isChkCommentsOnOwnPost == value)
                    return;
                SetProperty(ref _isChkCommentsOnOwnPost, value);
            }
        }


        private bool _isChkTryPostAfterComment;

        [ProtoMember(10)]
        public bool IsChkTryPostAfterComment
        {
            get => _isChkTryPostAfterComment;
            set
            {
                if (_isChkTryPostAfterComment == value)
                    return;
                SetProperty(ref _isChkTryPostAfterComment, value);
            }
        }


        private bool _isChkFollowUserAfterComment;

        [ProtoMember(11)]
        public bool IsChkFollowUserAfterComment
        {
            get => _isChkFollowUserAfterComment;
            set
            {
                if (_isChkFollowUserAfterComment == value)
                    return;
                SetProperty(ref _isChkFollowUserAfterComment, value);
            }
        }

        private bool _isChkRemovePoorQuality;

        [ProtoMember(12)]
        public bool IsChkRemovePoorQuality
        {
            get => _isChkRemovePoorQuality;
            set
            {
                if (_isChkRemovePoorQuality == value)
                    return;
                SetProperty(ref _isChkRemovePoorQuality, value);
            }
        }


        private bool _isChkRemoveIfFollowBackRatioLimit;

        [ProtoMember(13)]
        public bool IsChkRemoveIfFollowBackRatioLimit
        {
            get => _isChkRemoveIfFollowBackRatioLimit;
            set
            {
                if (_isChkRemoveIfFollowBackRatioLimit == value)
                    return;
                SetProperty(ref _isChkRemoveIfFollowBackRatioLimit, value);
            }
        }

        private bool _isChkSuspendToolWhenGotBlock;

        [ProtoMember(14)]
        public bool IsChkSuspendToolWhenGotBlock
        {
            get => _isChkSuspendToolWhenGotBlock;
            set
            {
                if (_isChkSuspendToolWhenGotBlock == value)
                    return;
                SetProperty(ref _isChkSuspendToolWhenGotBlock, value);
            }
        }

        [ProtoMember(15)] public RangeUtilities RandomlyGeneratedUsers { get; set; } = new RangeUtilities();

        private int _removeIfMinFollowBackRatioValue;

        [ProtoMember(16)]
        public int RemoveIfMinFollowBackRatioValue
        {
            get => _removeIfMinFollowBackRatioValue;
            set
            {
                if (_removeIfMinFollowBackRatioValue == value)
                    return;
                SetProperty(ref _removeIfMinFollowBackRatioValue, value);
            }
        }

        private int _removeIfFollowBackRatioAfterCommentValue;

        [ProtoMember(17)]
        public int RemoveIfFollowBackRatioAfterCommentValue
        {
            get => _removeIfFollowBackRatioAfterCommentValue;
            set
            {
                if (_removeIfFollowBackRatioAfterCommentValue == value)
                    return;
                SetProperty(ref _removeIfFollowBackRatioAfterCommentValue, value);
            }
        }

        private int _suspendToolWhenGotBlockedValue;

        [ProtoMember(18)]
        public int SuspendToolWhenGotBlockedValue
        {
            get => _suspendToolWhenGotBlockedValue;
            set
            {
                if (_suspendToolWhenGotBlockedValue == value)
                    return;
                SetProperty(ref _suspendToolWhenGotBlockedValue, value);
            }
        }
        
        [ProtoMember(19)]
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

        [ProtoMember(21)]
        public ObservableCollection<ManageCommentModel> LstDisplayManageCommentModel { get; set; } =
            new ObservableCollection<ManageCommentModel>();

        [ProtoMember(23)]
        public bool IsChkAllowMultipleCommentsOnSamePost
        {
            get => _isChkAllowMultipleCommentsOnSamePost;

            set
            {
                if (value == _isChkAllowMultipleCommentsOnSamePost)
                    return;
                SetProperty(ref _isChkAllowMultipleCommentsOnSamePost, value);
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


        [ProtoMember(28)]
        public RangeUtilities CommentsPerUser
        {
            get => _commentsPerUser;

            set
            {
                if (value == _commentsPerUser)
                    return;
                SetProperty(ref _commentsPerUser, value);
            }
        }

        [ProtoMember(29)]
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

        [ProtoMember(30)]
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

        [ProtoMember(31)]
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

        [ProtoMember(33)]
        public bool IsChkAddMultipleComments
        {
            get => _isChkAddMultipleComments;

            set
            {
                if (value == _isChkAddMultipleComments)
                    return;
                SetProperty(ref _isChkAddMultipleComments, value);
            }
        }

        [ProtoMember(34)]
        public RangeUtilities Likes
        {
            get => _likes;

            set
            {
                if (value == _likes)
                    return;
                SetProperty(ref _likes, value);
            }
        }

        [ProtoMember(35)]
        public bool IsChkIncreaseEachDayLike
        {
            get => _isChkIncreaseEachDayLike;
            set
            {
                if (_isChkIncreaseEachDayLike == value)
                    return;
                SetProperty(ref _isChkIncreaseEachDayLike, value);
            }
        }

        [ProtoMember(36)]
        public RangeUtilities IncreaseEachDayLike
        {
            get => _increaseEachDayLike;

            set
            {
                if (value == _increaseEachDayLike)
                    return;
                SetProperty(ref _increaseEachDayLike, value);
            }
        }

        [ProtoMember(37)]
        public bool IsChkSkipBlackListedUser
        {
            get => _isChkSkipBlackListedUser;
            set
            {
                if (_isChkSkipBlackListedUser == value) return;
                SetProperty(ref _isChkSkipBlackListedUser, value);
            }
        }

        [ProtoMember(38)]
        public bool IsChkPrivateBlackList
        {
            get => _isChkPrivateBlackList;
            set
            {
                if (_isChkPrivateBlackList == value) return;
                SetProperty(ref _isChkPrivateBlackList, value);
            }
        }

        [ProtoMember(39)]
        public bool IsChkGroupBlackList
        {
            get => _isChkGroupBlackList;
            set
            {
                if (_isChkGroupBlackList == value) return;
                SetProperty(ref _isChkGroupBlackList, value);
            }
        }

        [ProtoMember(40)]
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

        [ProtoMember(41)]
        public RangeUtilities DelayBetweenLikeCommentsForAfterActivity
        {
            get => _delayBetweenLikeCommentsForAfterActivity;

            set
            {
                if (value == _delayBetweenLikeCommentsForAfterActivity)
                    return;
                SetProperty(ref _delayBetweenLikeCommentsForAfterActivity, value);
            }
        }

        [ProtoMember(42)]
        public bool IsMakeCommentAsSpinText
        {
            get => _isMakeCommentAsSpinText;

            set
            {
                if (value == _isMakeCommentAsSpinText)
                    return;
                SetProperty(ref _isMakeCommentAsSpinText, value);
            }
        }

        public ManageCommentModel ManageCommentModel { get; set; } = new ManageCommentModel();

        public List<string> ListQueryType { get; set; } = new List<string>();

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

        [ProtoMember(26)]
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

        [ProtoMember(32)]
        public bool ChkLikeOthersCommentChecked
        {
            get => _chkLikeOthersCommentChecked;

            set
            {
                if (value == _chkLikeOthersCommentChecked)
                    return;
                SetProperty(ref _chkLikeOthersCommentChecked, value);
            }
        }

       
    }
}