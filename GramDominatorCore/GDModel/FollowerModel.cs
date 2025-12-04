using System.Collections.Generic;
using DominatorHouseCore.Utility;
using ProtoBuf;
using JobConfiguration = DominatorHouseCore.Models.JobConfiguration;
using DominatorHouseCore.Interfaces;
using GramDominatorCore.GDLibrary;
using System.Collections.ObjectModel;
using DominatorHouseCore.Models;

namespace GramDominatorCore.GDModel
{
    public interface IFollowerModel
    {    
        RangeUtilities Comments { get; set; }
    }

    [ProtoContract]
    public class FollowerModel : ModuleSetting, IFollowerModel, IGeneralSettings
    {

        #region Variables

        private RangeUtilities _likeMaxBetween = new RangeUtilities();
        private RangeUtilities _likeBetweenJobs = new RangeUtilities(1, 1);
        private bool _isChkEnableAutoFollowUnfollowChecked;
        private bool _isChkStopFollowToolWhenReachedSpecifiedFollowings;
        private RangeUtilities _stopFollowToolWhenReachSpecifiedFollowings = new RangeUtilities(1500, 2000);
        private bool _isChkWhenFollowerFollowingsIsSmallerThanChecked;
        private int _followerFollowingsMaxValue;
        private bool _chkLikeRandomPostsChecked;
        private bool _chkCommentOnUserLatestPostsChecked;
        private RangeUtilities _comments = new RangeUtilities();
        private int _commentPercentage;
        private bool _chkSendDirectMessageAfterFollowChecked;
        private bool _ChkMuteFollowerAfterFollowChecked;
        private bool _IsCheckedFollowerPostAfterFollow;
        private bool _IsCheckedFollowerStoryAfterFollow;
        private string _mediaPath;
        private bool __IsCheckedFollowerPostAndStoryAfterFollow;
        private RangeUtilities _messageBetween = new RangeUtilities();
        private int _directMessagePercentage;
        private bool _chkFollowUniqueUsersInCampaign;
        private bool _isCheckedStopFollowStartUnfollow = true;
        private RangeUtilities _delayBetweenLikesForAfterActivity = new RangeUtilities(15, 30);
        private RangeUtilities _delayBetweenCommentsForAfterActivity = new RangeUtilities(15, 30);
        private RangeUtilities _delayBetweenMessagesForAfterActivity = new RangeUtilities(15, 30);
        
        #endregion        
        public List<string> ListQueryType { get; set; } = new List<string>();

        [ProtoMember(2)]
        public override UserFilterModel UserFilterModel { get; set; } = new UserFilterModel();

        [ProtoMember(3)]
        public override PostFilterModel PostFilterModel { get; set; } = new PostFilterModel();

        [ProtoMember(4)]
        JobConfiguration IGeneralSettings.JobConfiguration { get; set; }

        [ProtoMember(66)]
        public override MangeBlacklist.SkipBlacklist SkipBlacklist { get; set; } = new MangeBlacklist.SkipBlacklist();

        #region Set Job Configuration speed
        public JobConfiguration SlowSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(66, 100),
            ActivitiesPerHour = new RangeUtilities(6, 10),
            ActivitiesPerWeek = new RangeUtilities(400, 600),
            ActivitiesPerJob = new RangeUtilities(8, 12),
            DelayBetweenJobs = new RangeUtilities(81, 122),
            DelayBetweenActivity = new RangeUtilities(30, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(200, 300),
            ActivitiesPerHour = new RangeUtilities(20, 30),
            ActivitiesPerWeek = new RangeUtilities(1200, 1800),
            ActivitiesPerJob = new RangeUtilities(25, 37),
            DelayBetweenJobs = new RangeUtilities(72, 108),
            DelayBetweenActivity = new RangeUtilities(25, 50),
             DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration FastSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(333, 500),
            ActivitiesPerHour = new RangeUtilities(33, 50),
            ActivitiesPerWeek = new RangeUtilities(2000, 3000),
            ActivitiesPerJob = new RangeUtilities(41, 62),
            DelayBetweenJobs = new RangeUtilities(69, 103),
            DelayBetweenActivity = new RangeUtilities(15, 30),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(533, 800),
            ActivitiesPerHour = new RangeUtilities(53, 80),
            ActivitiesPerWeek = new RangeUtilities(3200, 4800),
            ActivitiesPerJob = new RangeUtilities(66, 100),
            DelayBetweenJobs = new RangeUtilities(73, 110),
            DelayBetweenActivity = new RangeUtilities(8, 15),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };
        #endregion


        #region IFollowerModel
    
        [ProtoMember(7)]
        public RangeUtilities LikeBetweenJobs
        {
            get
            {
                return _likeBetweenJobs;
            }
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
            get { return _likeMaxBetween; }

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
            get
            {
                return _isChkEnableAutoFollowUnfollowChecked;
            }

            set
            {
                if (value == _isChkEnableAutoFollowUnfollowChecked)
                    return;
                SetProperty(ref _isChkEnableAutoFollowUnfollowChecked, value);
            }
        }

        [ProtoMember(10)]
        public bool IsChkStopFollowToolWhenReachedSpecifiedFollowings
        {
            get
            {
                return _isChkStopFollowToolWhenReachedSpecifiedFollowings;
            }

            set
            {
                if (value == _isChkStopFollowToolWhenReachedSpecifiedFollowings)
                    return;
                SetProperty(ref _isChkStopFollowToolWhenReachedSpecifiedFollowings, value);
            }
        }

        [ProtoMember(11)]
        public RangeUtilities StopFollowToolWhenReachSpecifiedFollowings
        {
            get
            {
                return _stopFollowToolWhenReachSpecifiedFollowings;
            }

            set
            {
                if (value == _stopFollowToolWhenReachSpecifiedFollowings)
                    return;
                SetProperty(ref _stopFollowToolWhenReachSpecifiedFollowings, value);
            }
        }

        [ProtoMember(13)]
        public bool IsChkWhenFollowerFollowingsIsSmallerThanChecked
        {
            get
            {
                return _isChkWhenFollowerFollowingsIsSmallerThanChecked;
            }

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
            get
            {
                return _followerFollowingsMaxValue;
            }

            set
            {
                if (value == _followerFollowingsMaxValue)
                    return;
                SetProperty(ref _followerFollowingsMaxValue, value);
            }
        }
       
        [ProtoMember(21)]
        public bool ChkLikeRandomPostsChecked
        {
            get
            {
                return _chkLikeRandomPostsChecked;
            }

            set
            {
                if (value == _chkLikeRandomPostsChecked)
                    return;
                SetProperty(ref _chkLikeRandomPostsChecked, value);
            }
        }

        [ProtoMember(22)]
        public bool ChkCommentOnUserLatestPostsChecked
        {
            get
            {
                return _chkCommentOnUserLatestPostsChecked;
            }

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
            get
            {
                return _comments;
            }

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
            get
            {
                return _commentPercentage;
            }

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
            get { return _chkSendDirectMessageAfterFollowChecked; }

            set
            {
                if (value == _chkSendDirectMessageAfterFollowChecked)
                    return;
                SetProperty(ref _chkSendDirectMessageAfterFollowChecked, value);
            }
        }

        [ProtoMember(29)]
        public RangeUtilities MessageBetween
        {
            get
            {
                return _messageBetween;
            }

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
            get
            {
                return _directMessagePercentage;
            }

            set
            {
                if (value == _directMessagePercentage)
                    return;
                SetProperty(ref _directMessagePercentage, value);
            }
        }    

        private List<string> _lstComments = new List<string>();
        [ProtoMember(38)]
        public List<string> LstComments
        {
            get
            {
                return _lstComments;
            }
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
            get
            {
                return _lstMessages;
            }
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
            get
            {
                return _isChkDirectMessagePercentage;
            }
            set
            {
                if (_isChkDirectMessagePercentage == value)
                    return;
                SetProperty(ref _isChkDirectMessagePercentage, value);
            }
        }

        private bool _isChkMaxMessege;
        [ProtoMember(45)]
        public bool IsChkMaxMessege
        {
            get
            {
                return _isChkMaxMessege;
            }
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
            get
            {
                return _message;
            }
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
            get
            {
                return _uploadComment;
            }
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
            get
            {
                return _isChkCommentPercentage;
            }
            set
            {
                if (_isChkCommentPercentage == value)
                    return;
                SetProperty(ref _isChkCommentPercentage, value);
            }
        }

        private bool _isChkMaxComment;
        [ProtoMember(50)]
        public bool IsChkMaxComment
        {
            get
            {
                return _isChkMaxComment;
            }
            set
            {
                if (_isChkMaxComment == value)
                    return;
                SetProperty(ref _isChkMaxComment, value);
            }
        }
        private bool _isChkMaxLike;
        [ProtoMember(52)]
        public bool IsChkMaxLike
        {
            get
            {
                return _isChkMaxLike;
            }
            set
            {
                if (_isChkMaxLike == value)
                    return;
                SetProperty(ref _isChkMaxLike, value);
            }
        }

        private bool _isChkLikeUsersLatestPost;
        [ProtoMember(53)]
        public bool IsChkLikeUsersLatestPost
        {
            get
            {
                return _isChkLikeUsersLatestPost;
            }
            set
            {
                if (_isChkLikeUsersLatestPost == value)
                    return;
                SetProperty(ref _isChkLikeUsersLatestPost, value);
            }
        }

        private bool _isChkOnlyStopFollowTool;
        [ProtoMember(54)]
        public bool IsChkOnlyStopFollowTool
        {
            get
            {
                return _isChkOnlyStopFollowTool;
            }

            set
            {
                if (value == _isChkOnlyStopFollowTool)
                    return;
                SetProperty(ref _isChkOnlyStopFollowTool, value);
            }
        }
 
        private RangeUtilities _commentsPerUserPerUser = new RangeUtilities();

        [ProtoMember(61)]
        public RangeUtilities CommentsPerUser
        {
            get
            {
                return _commentsPerUserPerUser;
            }

            set
            {
                if (value == _commentsPerUserPerUser)
                    return;
                SetProperty(ref _commentsPerUserPerUser, value);
            }
        }      

        [ProtoMember(64)]
        public bool ChkFollowUniqueUsersInCampaign
        {
            get
            {
                return _chkFollowUniqueUsersInCampaign;
            }

            set
            {
                if (value == _chkFollowUniqueUsersInCampaign)
                    return;
                SetProperty(ref _chkFollowUniqueUsersInCampaign, value);
            }
        }


        [ProtoMember(65)]
        public bool IsCheckedStopFollowStartUnfollow
        {
            get
            {
                return _isCheckedStopFollowStartUnfollow;
            }
            set
            {
                if (value == _isCheckedStopFollowStartUnfollow)
                    return;
                SetProperty(ref _isCheckedStopFollowStartUnfollow, value);
            }
        }

        [ProtoMember(67)]
        public RangeUtilities DelayBetweenLikesForAfterActivity
        {
            get
            {
                return _delayBetweenLikesForAfterActivity;
            }
            set
            {
                if (value == _delayBetweenLikesForAfterActivity)
                    return;
                SetProperty(ref _delayBetweenLikesForAfterActivity, value);
            }
        }
        
        [ProtoMember(68)]
        public RangeUtilities DelayBetweenCommentsForAfterActivity
        {
            get
            {
                return _delayBetweenCommentsForAfterActivity;
            }
            set
            {
                if (value == _delayBetweenCommentsForAfterActivity)
                    return;
                SetProperty(ref _delayBetweenCommentsForAfterActivity, value);
            }
        }
        
        [ProtoMember(69)]
        public RangeUtilities DelayBetweenMessagesForAfterActivity
        {
            get
            {
                return _delayBetweenMessagesForAfterActivity;
            }
            set
            {
                if (value == _delayBetweenMessagesForAfterActivity)
                    return;
                SetProperty(ref _delayBetweenMessagesForAfterActivity, value);
            }
        }
        [ProtoMember(70)]
        public bool ChkMuteFollowerAfterFollowChecked
        {
            get { return _ChkMuteFollowerAfterFollowChecked; }

            set
            {
                if (value == _ChkMuteFollowerAfterFollowChecked)
                    return;
                SetProperty(ref _ChkMuteFollowerAfterFollowChecked, value);
            }
        }

        [ProtoMember(71)]
        public bool IsCheckedFollowerPostAfterFollow
        {
            get { return _IsCheckedFollowerPostAfterFollow; }

            set
            {
                if (value == _IsCheckedFollowerPostAfterFollow)
                    return;
                SetProperty(ref _IsCheckedFollowerPostAfterFollow, value);
            }
        }
        [ProtoMember(72)]
        public bool IsCheckedFollowerStoryAfterFollow
        {
            get { return _IsCheckedFollowerStoryAfterFollow; }

            set
            {
                if (value == _IsCheckedFollowerStoryAfterFollow)
                    return;
                SetProperty(ref _IsCheckedFollowerStoryAfterFollow, value);
            }
        }

        [ProtoMember(73)]
        public bool IsCheckedFollowerPostAndStoryAfterFollow
        {
            get { return __IsCheckedFollowerPostAndStoryAfterFollow; }

            set
            {
                if (value == __IsCheckedFollowerPostAndStoryAfterFollow)
                    return;
                SetProperty(ref __IsCheckedFollowerPostAndStoryAfterFollow, value);
            }
        }

        [ProtoMember(74)]
        public string MediaPath
        {
            get { return _mediaPath; }
            set
            {
                if (value == _mediaPath)
                    return;
                SetProperty(ref _mediaPath, value);
            }
        }
        #endregion

    }

}
