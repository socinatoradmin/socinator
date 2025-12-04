using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.CommonSettings;
using FaceDominatorCore.FDModel.FilterModel;
using ProtoBuf;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using static FaceDominatorCore.FDLibrary.FdClassLibrary.MangeBlacklist;

namespace FaceDominatorCore.FDModel.FriendsModel
{
    public interface ISendFriendRequestModel
    {

        /*bool IschkUniqueRequestChecked { get; set; }      

        int CancelRequestAfterDays { get; set; }
        
        bool IsChkStopFriendToolWhenReachChecked { get; set; }

        bool IsChkEnableAutoSendRequestWithdrawChecked { get; set; }
        
        RangeUtilities StopFollowToolWhenReach { get; set; }

        bool IsChkLikeUsersLatestPost { get; set; }

        RangeUtilities LikeBetweenJobs { get; set; }

        RangeUtilities DelayBetweenLikesForAfterActivity { get; set; }

        bool ChkLikeRandomPostsChecked { get; set; }

        bool ChkCommentOnUserLatestPostsChecked { get; set; }

        RangeUtilities CommentsPerUser { get; set; }

        RangeUtilities DelayBetweenCommentsForAfterActivity { get; set; }

        bool IsChkCommentPercentage { get; set; }

        int CommentPercentage { get; set; }

        string UploadComment { get; set; }

        bool ChkSendDirectMessageAfterFollowChecked { get; set; }

        RangeUtilities DelayBetweenMessagesForAfterActivity { get; set; }

        RangeUtilities MessagePerUser { get; set; }

        bool IsChkDirectMessagePercentage { get; set; }

        int DirectMessagePercentage { get; set; }*/

        string Message { get; set; }

    }

    [ProtoContract]
    public class SendFriendRequestModel : ModuleSetting, ISendFriendRequestModel
    {

        public List<string> ListQueryType { get; set; } = new List<string>();

        [ProtoMember(1)]
        public override JobConfiguration JobConfiguration { get; set; }

        [ProtoMember(2)]
        public override ObservableCollection<QueryInfo> SavedQueries { get; set; } = new ObservableCollection<QueryInfo>();

        private bool _ischkUniqueRequestChecked;
        [ProtoMember(3)]
        public bool IschkUniqueRequestChecked
        {
            get { return _ischkUniqueRequestChecked; }

            set
            {
                SetProperty(ref _ischkUniqueRequestChecked, value);
            }
        }

        private bool _isChkStopFriendToolWhenReachChecked;

        [ProtoMember(4)]
        public bool IsChkStopFriendToolWhenReachChecked
        {
            get
            {
                return _isChkStopFriendToolWhenReachChecked;
            }

            set
            {
                SetProperty(ref _isChkStopFriendToolWhenReachChecked, value);
            }
        }

        private bool _isChkEnableAutoSendRequestWithdrawChecked;

        [ProtoMember(5)]
        public bool IsChkEnableAutoSendRequestWithdrawChecked
        {
            get
            {
                return _isChkEnableAutoSendRequestWithdrawChecked;
            }

            set
            {
                SetProperty(ref _isChkEnableAutoSendRequestWithdrawChecked, value);
            }
        }


        private RangeUtilities _stopFollowToolWhenReach = new RangeUtilities();

        [ProtoMember(6)]

        public RangeUtilities StopFollowToolWhenReach
        {
            get
            {
                return _stopFollowToolWhenReach;
            }

            set
            {
                SetProperty(ref _stopFollowToolWhenReach, value);
            }
        }

        [ProtoMember(7)]

        public override SkipBlacklist SkipBlacklist { get; set; } = new SkipBlacklist();


        private bool _isChkLikeUsersLatestPost;

        [ProtoMember(8)]

        public bool IsChkLikeUsersLatestPost
        {
            get
            {
                return _isChkLikeUsersLatestPost;
            }

            set
            {
                SetProperty(ref _isChkLikeUsersLatestPost, value);
            }
        }

        private RangeUtilities _likeBetweenJobs = new RangeUtilities(1, 2);

        [ProtoMember(9)]

        public RangeUtilities LikeBetweenJobs
        {
            get
            {
                return _likeBetweenJobs;
            }

            set
            {
                SetProperty(ref _likeBetweenJobs, value);
            }
        }

        private RangeUtilities _delayBetweenLikesForAfterActivity = new RangeUtilities(1, 2);

        [ProtoMember(10)]

        public RangeUtilities DelayBetweenLikesForAfterActivity
        {
            get
            {
                return _delayBetweenLikesForAfterActivity;
            }

            set
            {
                SetProperty(ref _delayBetweenLikesForAfterActivity, value);
            }
        }

        private bool _chkCommentOnUserLatestPostsChecked;

        [ProtoMember(11)]

        public bool ChkCommentOnUserLatestPostsChecked
        {
            get
            {
                return _chkCommentOnUserLatestPostsChecked;
            }

            set
            {
                SetProperty(ref _chkCommentOnUserLatestPostsChecked, value);
            }
        }

        private RangeUtilities _commentsPerUser = new RangeUtilities(1, 2);

        [ProtoMember(12)]

        public RangeUtilities CommentsPerUser
        {
            get
            {
                return _commentsPerUser;
            }

            set
            {
                SetProperty(ref _commentsPerUser, value);
            }
        }

        private RangeUtilities _delayBetweenCommentsForAfterActivity = new RangeUtilities(1, 2);

        [ProtoMember(13)]

        public RangeUtilities DelayBetweenCommentsForAfterActivity
        {
            get
            {
                return _delayBetweenCommentsForAfterActivity;
            }

            set
            {
                SetProperty(ref _delayBetweenCommentsForAfterActivity, value);
            }
        }

        private string _uploadComment = string.Empty;

        [ProtoMember(14)]
        public string UploadComment
        {
            get
            {
                return _uploadComment;
            }

            set
            {
                SetProperty(ref _uploadComment, value);
            }
        }

        private bool _chkSendDirectMessageAfterFollowChecked;

        [ProtoMember(15)]
        public bool ChkSendDirectMessageAfterFollowChecked
        {
            get
            {
                return _chkSendDirectMessageAfterFollowChecked;
            }

            set
            {
                SetProperty(ref _chkSendDirectMessageAfterFollowChecked, value);
            }
        }

        private RangeUtilities _delayBetweenMessagesForAfterActivity = new RangeUtilities(1, 2);

        [ProtoMember(16)]
        public RangeUtilities DelayBetweenMessagesForAfterActivity
        {
            get
            {
                return _delayBetweenMessagesForAfterActivity;
            }

            set
            {
                SetProperty(ref _delayBetweenMessagesForAfterActivity, value);
            }
        }

        private string _message = string.Empty;

        [ProtoMember(17)]
        public string Message
        {
            get
            {
                return _message;
            }

            set
            {
                SetProperty(ref _message, value);
            }
        }

        private bool _isTagChecked;
        [ProtoMember(18)]
        public bool IsTagChecked
        {
            get
            {
                return _isTagChecked;
            }

            set
            {
                SetProperty(ref _isTagChecked, value);
            }
        }

        [ProtoMember(19)]
        public override FdGenderAndLocationFilterModel GenderAndLocationFilter { get; set; } = new FdGenderAndLocationFilterModel();



        // ReSharper disable once UnusedMember.Global
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

        // ReSharper disable once UnusedMember.Global
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

        // ReSharper disable once UnusedMember.Global
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


        // ReSharper disable once IdentifierTypo
        // ReSharper disable once UnusedMember.Global
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
    }
}