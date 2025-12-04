using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDEnums;
using GramDominatorCore.GDLibrary;
using ProtoBuf;

namespace GramDominatorCore.GDModel
{

    //public interface IBlockFollowerModel
    //{
    //    #region IBlockFollowerModel
    //    int IncreaseFollowCount { get; set; }

    //    int FollowCountUntil { get; set; }

    //    RangeUtilities LikeBetweenJobs { get; set; }

    //    RangeUtilities LikeMaxBetween { get; set; }

    //    bool IsChkEnableAutoFollowUnfollowChecked { get; set; }

    //    bool IsChkStopFollowToolWhenReachChecked { get; set; }

    //    RangeUtilities StopFollowToolWhenReach { get; set; }

    //    RangeUtilities StartUnfollow { get; set; }

    //    bool IsChkWhenFollowerFollowingsIsSmallerThanChecked { get; set; }

    //    int FollowerFollowingsMaxValue { get; set; }

    //    bool IsChkFollowToolGetsTemporaryBlockedChecked { get; set; }

    //    RangeUtilities UnfollowPrevious { get; set; }

    //    bool IsChkUnfollowUsersChecked { get; set; }

    //    bool IsChkUnfollowfollowedbackChecked { get; set; }

    //    bool IsChkUnfollownotfollowedbackChecked { get; set; }

    //    RangeUtilities IncreaseEachDayLike { get; set; }

    //    bool ChkLikeRandomPostsChecked { get; set; }

    //    bool ChkCommentOnUserLatestPostsChecked { get; set; }

    //    RangeUtilities Comments { get; set; }

    //    RangeUtilities IncreaseEachDayComment { get; set; }

    //    int CommentPercentage { get; set; }

    //    bool ChkUploadCommentsChecked { get; set; }

    //    bool ChkSendDirectMessageAfterFollowChecked { get; set; }

    //    RangeUtilities MessageBetween { get; set; }

    //    RangeUtilities IncreaseEachDayMessage { get; set; }

    //    int DirectMessagePercentage { get; set; }

    //    bool ChkAcceptPendingFriendRequestChecked { get; set; }

    //    RangeUtilities AcceptBetween { get; set; }

    //    bool ChkRemovePoorQualitySourcesChecked { get; set; }

    //    RangeUtilities FollowBackRatio { get; set; }

    //    bool ChkIgnoreAddingSourcesPrevioslyRemovedChecked { get; set; }

    //    bool IsChkStartUnfollowToolBetweenChecked { get; set; }

    //    #endregion
    //}

    [ProtoContract]
    public class BlockFollowerModel : ModuleSetting, IFollowerModel, IGeneralSettings
    {


        #region Variables

       // private RangeUtilities _likeMaxBetween = new RangeUtilities();
      //  private int _followCountUntil;
       // private RangeUtilities _likeBetweenJobs = new RangeUtilities();
       // private bool _isChkEnableAutoFollowUnfollowChecked;
       // private bool _isChkStopFollowToolWhenReachedSpecifiedFollowings;
       // private RangeUtilities _stopFollowToolWhenReachSpecifiedFollowings = new RangeUtilities();
       // private RangeUtilities _startUnfollow = new RangeUtilities();
       // private bool _isChkWhenFollowerFollowingsIsSmallerThanChecked;
       // private int _followerFollowingsMaxValue;
       // private bool _isChkFollowToolGetsTemporaryBlockedChecked;
       // private RangeUtilities _unfollowPrevious = new RangeUtilities();
       // private bool _isChkUnfollowUsersChecked;
       // private bool _isChkUnfollowfollowedbackChecked;
       // private bool _isChkUnfollownotfollowedbackChecked;
      //  private RangeUtilities _increaseEachDayLike = new RangeUtilities();
      //  private bool _chkLikeRandomPostsChecked;
       // private bool _chkCommentOnUserLatestPostsChecked;
        private RangeUtilities _comments = new RangeUtilities();
       // private RangeUtilities _increaseEachDayComment = new RangeUtilities();
       // private int _commentPercentage;
       // private bool _chkUploadCommentsChecked;
       // private bool _chkSendDirectMessageAfterFollowChecked;
      //  private RangeUtilities _messageBetween = new RangeUtilities();
//private RangeUtilities _increaseEachDayMessage = new RangeUtilities();
       // private int _directMessagePercentage;
       // private bool _chkAcceptPendingFriendRequestChecked;
       // private RangeUtilities _acceptBetween = new RangeUtilities();
       // private bool _chkRemovePoorQualitySourcesChecked;
//private RangeUtilities _followBackRatio = new RangeUtilities();
//private bool _chkIgnoreAddingSourcesPrevioslyRemovedChecked;
       // private int _increaseFollowCount;
       // private bool _isChkStartUnfollowToolBetweenChecked;

       // bool _chkFollowUniqueUsersGlobally, _chkFollowUniqueUsersInCampaign;

      //  private bool _isCheckedStopFollowStartUnfollow = true;

        #endregion

        public List<string> ListQueryType { get; set; }


        public BlockFollowerModel()
        {
            ListQueryType = Enum.GetNames(typeof(Enums.UserQueryParameters)).ToList();
            ListQueryType.Remove("HashtagPost");
            ListQueryType.Remove("LocationPosts");
            ListQueryType.Remove("CustomPhotos");

        }

        [ProtoMember(2)]
        public override UserFilterModel UserFilterModel { get; set; } = new UserFilterModel();

        [ProtoMember(3)]
        public override PostFilterModel PostFilterModel { get; set; } = new PostFilterModel();

        [ProtoMember(4)]
        JobConfiguration IGeneralSettings.JobConfiguration { get; set; }

        [ProtoMember(66)]
        public override MangeBlacklist.ManageBlackWhiteListModel ManageBlackWhiteListModel { get; set; } = new MangeBlacklist.ManageBlackWhiteListModel();


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


        #region IBlockFollowerModel


        //[ProtoMember(5)]
        //public int IncreaseFollowCount
        //{
        //    get
        //    {
        //        return _increaseFollowCount;
        //    }
        //    set
        //    {
        //        if (value == _increaseFollowCount)
        //            return;
        //        SetProperty(ref _increaseFollowCount, value);
        //    }
        //}

        //[ProtoMember(6)]
        //public int FollowCountUntil
        //{
        //    get
        //    {
        //        return _followCountUntil;
        //    }
        //    _followCountUntil
        //    set
        //    {
        //        if (value == _followCountUntil)
        //            return;
        //        SetProperty(ref _followCountUntil, value);
        //    }
        //}

        //[ProtoMember(7)]
        //public RangeUtilities LikeBetweenJobs
        //{
        //    get
        //    {
        //        return _likeBetweenJobs;
        //    }
        //    set
        //    {
        //        if (value == _likeBetweenJobs)
        //            return;
        //        SetProperty(ref _likeBetweenJobs, value);
        //    }
        //}

        //[ProtoMember(8)]
        //public RangeUtilities LikeMaxBetween
        //{
        //    get { return _likeMaxBetween; }

        //    set
        //    {
        //        if (value == _likeMaxBetween)
        //            return;
        //        SetProperty(ref _likeMaxBetween, value);
        //    }
        //}

        //[ProtoMember(9)]
        //public bool IsChkEnableAutoFollowUnfollowChecked
        //{
        //    get
        //    {
        //        return _isChkEnableAutoFollowUnfollowChecked;
        //    }

        //    set
        //    {
        //        if (value == _isChkEnableAutoFollowUnfollowChecked)
        //            return;
        //        SetProperty(ref _isChkEnableAutoFollowUnfollowChecked, value);
        //    }
        //}

        //[ProtoMember(10)]
        //public bool IsChkStopFollowToolWhenReachedSpecifiedFollowings
        //{
        //    get
        //    {
        //        return _isChkStopFollowToolWhenReachedSpecifiedFollowings;
        //    }

        //    set
        //    {
        //        if (value == _isChkStopFollowToolWhenReachedSpecifiedFollowings)
        //            return;
        //        SetProperty(ref _isChkStopFollowToolWhenReachedSpecifiedFollowings, value);
        //    }
        //}

        //[ProtoMember(11)]
        //public RangeUtilities StopFollowToolWhenReachSpecifiedFollowings
        //{
        //    get
        //    {
        //        return _stopFollowToolWhenReachSpecifiedFollowings;
        //    }

        //    set
        //    {
        //        if (value == _stopFollowToolWhenReachSpecifiedFollowings)
        //            return;
        //        SetProperty(ref _stopFollowToolWhenReachSpecifiedFollowings, value);
        //    }
        //}

        //[ProtoMember(12)]
        //public RangeUtilities StartUnfollow
        //{
        //    get
        //    {
        //        return _startUnfollow;
        //    }

        //    set
        //    {
        //        if (value == _startUnfollow)
        //            return;
        //        SetProperty(ref _startUnfollow, value);
        //    }
        //}

        //[ProtoMember(13)]
        //public bool IsChkWhenFollowerFollowingsIsSmallerThanChecked
        //{
        //    get
        //    {
        //        return _isChkWhenFollowerFollowingsIsSmallerThanChecked;
        //    }

        //    set
        //    {
        //        if (value == _isChkWhenFollowerFollowingsIsSmallerThanChecked)
        //            return;
        //        SetProperty(ref _isChkWhenFollowerFollowingsIsSmallerThanChecked, value);
        //    }
        //}

        //[ProtoMember(14)]
        //public int FollowerFollowingsMaxValue
        //{
        //    get
        //    {
        //        return _followerFollowingsMaxValue;
        //    }

        //    set
        //    {
        //        if (value == _followerFollowingsMaxValue)
        //            return;
        //        SetProperty(ref _followerFollowingsMaxValue, value);
        //    }
        //}

        //[ProtoMember(15)]
        //public bool IsChkFollowToolGetsTemporaryBlockedChecked
        //{
        //    get
        //    {
        //        return _isChkFollowToolGetsTemporaryBlockedChecked;
        //    }

        //    set
        //    {
        //        if (value == _isChkFollowToolGetsTemporaryBlockedChecked)
        //            return;
        //        SetProperty(ref _isChkFollowToolGetsTemporaryBlockedChecked, value);
        //    }
        //}

        //[ProtoMember(16)]
        //public RangeUtilities UnfollowPrevious
        //{
        //    get
        //    {
        //        return _unfollowPrevious;
        //    }

        //    set
        //    {
        //        if (value == _unfollowPrevious)
        //            return;
        //        SetProperty(ref _unfollowPrevious, value);
        //    }
        //}

        //[ProtoMember(17)]
        //public bool IsChkUnfollowUsersChecked
        //{
        //    get
        //    {
        //        return _isChkUnfollowUsersChecked;
        //    }

        //    set
        //    {
        //        if (value == _isChkUnfollowUsersChecked)
        //            return;
        //        SetProperty(ref _isChkUnfollowUsersChecked, value);
        //    }
        //}

        //[ProtoMember(18)]
        //public bool IsChkUnfollowfollowedbackChecked
        //{
        //    get
        //    {
        //        return _isChkUnfollowfollowedbackChecked;
        //    }

        //    set
        //    {
        //        if (value == _isChkUnfollowfollowedbackChecked)
        //            return;
        //        SetProperty(ref _isChkUnfollowfollowedbackChecked, value);
        //    }
        //}

        //[ProtoMember(19)]
        //public bool IsChkUnfollownotfollowedbackChecked
        //{
        //    get
        //    {
        //        return _isChkUnfollownotfollowedbackChecked;
        //    }

        //    set
        //    {
        //        if (value == _isChkUnfollownotfollowedbackChecked)
        //            return;
        //        SetProperty(ref _isChkUnfollownotfollowedbackChecked, value);
        //    }
        //}

        //[ProtoMember(20)]
        //public RangeUtilities IncreaseEachDayLike
        //{
        //    get
        //    {
        //        return _increaseEachDayLike;
        //    }

        //    set
        //    {
        //        if (value == _increaseEachDayLike)
        //            return;
        //        SetProperty(ref _increaseEachDayLike, value);
        //    }
        //}

        //[ProtoMember(21)]
        //public bool ChkLikeRandomPostsChecked
        //{
        //    get
        //    {
        //        return _chkLikeRandomPostsChecked;
        //    }

        //    set
        //    {
        //        if (value == _chkLikeRandomPostsChecked)
        //            return;
        //        SetProperty(ref _chkLikeRandomPostsChecked, value);
        //    }
        //}

        //[ProtoMember(22)]
        //public bool ChkCommentOnUserLatestPostsChecked
        //{
        //    get
        //    {
        //        return _chkCommentOnUserLatestPostsChecked;
        //    }

        //    set
        //    {
        //        if (value == _chkCommentOnUserLatestPostsChecked)
        //            return;
        //        SetProperty(ref _chkCommentOnUserLatestPostsChecked, value);
        //    }
        //}

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

        //[ProtoMember(24)]
        //public RangeUtilities IncreaseEachDayComment
        //{
        //    get
        //    {
        //        return _increaseEachDayComment;
        //    }

        //    set
        //    {
        //        if (value == _increaseEachDayComment)
        //            return;
        //        SetProperty(ref _increaseEachDayComment, value);
        //    }
        //}

        //[ProtoMember(25)]
        //public int CommentPercentage
        //{
        //    get
        //    {
        //        return _commentPercentage;
        //    }

        //    set
        //    {
        //        if (value == _commentPercentage)
        //            return;
        //        SetProperty(ref _commentPercentage, value);
        //    }
        //}

        //[ProtoMember(26)]
        //public bool ChkUploadCommentsChecked
        //{
        //    get
        //    {
        //        return _chkUploadCommentsChecked;
        //    }

        //    set
        //    {
        //        if (value == _chkUploadCommentsChecked)
        //            return;
        //        SetProperty(ref _chkUploadCommentsChecked, value);
        //    }
        //}

        //[ProtoMember(27)]
        //public bool ChkSendDirectMessageAfterFollowChecked
        //{
        //    get { return _chkSendDirectMessageAfterFollowChecked; }

        //    set
        //    {
        //        if (value == _chkSendDirectMessageAfterFollowChecked)
        //            return;
        //        SetProperty(ref _chkSendDirectMessageAfterFollowChecked, value);
        //    }
        //}

        //[ProtoMember(28)]
        //public bool ChkAddMessageChecked
        //{
        //    get { return _chkAddMessageChecked; }

        //    set
        //    {
        //        if (value == _chkAddMessageChecked)
        //            return;
        //        SetProperty(ref _chkAddMessageChecked, value);
        //    }
        //}

        //[ProtoMember(29)]
        //public RangeUtilities MessageBetween
        //{
        //    get
        //    {
        //        return _messageBetween;
        //    }

        //    set
        //    {
        //        if (value == _messageBetween)
        //            return;
        //        SetProperty(ref _messageBetween, value);
        //    }
        //}

        //[ProtoMember(30)]
        //public RangeUtilities IncreaseEachDayMessage
        //{
        //    get
        //    {
        //        return _increaseEachDayMessage;
        //    }

        //    set
        //    {
        //        if (value == _increaseEachDayMessage)
        //            return;
        //        SetProperty(ref _increaseEachDayMessage, value);
        //    }
        //}

        //[ProtoMember(31)]
        //public int DirectMessagePercentage
        //{
        //    get
        //    {
        //        return _directMessagePercentage;
        //    }

        //    set
        //    {
        //        if (value == _directMessagePercentage)
        //            return;
        //        SetProperty(ref _directMessagePercentage, value);
        //    }
        //}

        //[ProtoMember(32)]
        //public bool ChkAcceptPendingFriendRequestChecked
        //{
        //    get
        //    {
        //        return _chkAcceptPendingFriendRequestChecked;
        //    }

        //    set
        //    {
        //        if (value == _chkAcceptPendingFriendRequestChecked)
        //            return;
        //        SetProperty(ref _chkAcceptPendingFriendRequestChecked, value);
        //    }
        //}

        //[ProtoMember(33)]
        //public RangeUtilities AcceptBetween
        //{
        //    get
        //    {
        //        return _acceptBetween;
        //    }

        //    set
        //    {
        //        if (value == _acceptBetween)
        //            return;
        //        SetProperty(ref _acceptBetween, value);
        //    }
        //}

        //[ProtoMember(34)]
        //public bool ChkRemovePoorQualitySourcesChecked
        //{
        //    get
        //    {
        //        return _chkRemovePoorQualitySourcesChecked;
        //    }

        //    set
        //    {
        //        if (value == _chkRemovePoorQualitySourcesChecked)
        //            return;
        //        SetProperty(ref _chkRemovePoorQualitySourcesChecked, value);
        //    }
        //}

        //[ProtoMember(35)]
        //public RangeUtilities FollowBackRatio
        //{
        //    get { return _followBackRatio; }

        //    set
        //    {
        //        if (value == _followBackRatio)
        //            return;
        //        SetProperty(ref _followBackRatio, value);
        //    }
        //}

        //[ProtoMember(36)]
        //public bool ChkIgnoreAddingSourcesPrevioslyRemovedChecked
        //{
        //    get { return _chkIgnoreAddingSourcesPrevioslyRemovedChecked; }

        //    set
        //    {
        //        if (value == _chkIgnoreAddingSourcesPrevioslyRemovedChecked)
        //            return;
        //        SetProperty(ref _chkIgnoreAddingSourcesPrevioslyRemovedChecked, value);
        //    }
        //}
        //[ProtoMember(37)]
        //public bool IsChkStartUnfollowToolBetweenChecked
        //{
        //    get { return _isChkStartUnfollowToolBetweenChecked; }
        //    set
        //    {
        //        if (value == _isChkStartUnfollowToolBetweenChecked)
        //            return;
        //        SetProperty(ref _isChkStartUnfollowToolBetweenChecked, value);
        //    }
        //}

        //private List<string> _lstComments = new List<string>();
        //[ProtoMember(38)]
        //public List<string> LstComments
        //{
        //    get
        //    {
        //        return _lstComments;
        //    }
        //    set
        //    {
        //        if (value == _lstComments)
        //            return;
        //        SetProperty(ref _lstComments, value);
        //    }
        //}
        //private List<string> _lstMessages = new List<string>();
        //[ProtoMember(39)]
        //public List<string> LstMessages
        //{
        //    get
        //    {
        //        return _lstMessages;
        //    }
        //    set
        //    {
        //        if (value == _lstMessages)
        //            return;
        //        SetProperty(ref _lstMessages, value);
        //    }
        //}

        //private bool _isAddedToCampaign;
        //[ProtoMember(40)]
        //public bool IsAddedToCampaign
        //{
        //    get
        //    {
        //        return _isAddedToCampaign;
        //    }
        //    set
        //    {
        //        if (_isAddedToCampaign && _isAddedToCampaign == value)
        //            return;
        //        SetProperty(ref _isAddedToCampaign, value);
        //    }
        //}


        //private bool _isChkFollowBackRatio;
        //[ProtoMember(41)]
        //public bool IsChkFollowBackRatio
        //{
        //    get
        //    {
        //        return _isChkFollowBackRatio;
        //    }
        //    set
        //    {
        //        if (_isChkFollowBackRatio == value)
        //            return;
        //        SetProperty(ref _isChkFollowBackRatio, value);
        //    }
        //}

        //private bool _isChkAcceptbetween;
        //[ProtoMember(42)]
        //public bool IsChkAcceptbetween
        //{
        //    get
        //    {
        //        return _isChkAcceptbetween;
        //    }
        //    set
        //    {
        //        if (_isChkAcceptbetween == value)
        //            return;
        //        SetProperty(ref _isChkAcceptbetween, value);
        //    }
        //}

        //private bool _isChkDirectMessagePercentage;
        //[ProtoMember(43)]
        //public bool IsChkDirectMessagePercentage
        //{
        //    get
        //    {
        //        return _isChkDirectMessagePercentage;
        //    }
        //    set
        //    {
        //        if (_isChkDirectMessagePercentage == value)
        //            return;
        //        SetProperty(ref _isChkDirectMessagePercentage, value);
        //    }
        //}
        //private bool _isChkIncreaseEachDay;
        //[ProtoMember(44)]
        //public bool IsChkIncreaseEachDay
        //{
        //    get
        //    {
        //        return _isChkIncreaseEachDay;
        //    }
        //    set
        //    {
        //        if (_isChkIncreaseEachDay == value)
        //            return;
        //        SetProperty(ref _isChkIncreaseEachDay, value);
        //    }
        //}


        //private bool _isChkMaxMessege;
        //[ProtoMember(45)]
        //public bool IsChkMaxMessege
        //{
        //    get
        //    {
        //        return _isChkMaxMessege;
        //    }
        //    set
        //    {
        //        if (_isChkMaxMessege == value)
        //            return;
        //        SetProperty(ref _isChkMaxMessege, value);
        //    }
        //}



        //private string _message;
        //[ProtoMember(46)]
        //public string Message
        //{
        //    get
        //    {
        //        return _message;
        //    }
        //    set
        //    {
        //        if (_message == value)
        //            return;
        //        SetProperty(ref _message, value);
        //    }
        //}
        //private string _uploadComment;
        //[ProtoMember(47)]
        //public string UploadComment
        //{
        //    get
        //    {
        //        return _uploadComment;
        //    }
        //    set
        //    {
        //        if (_uploadComment == value)
        //            return;
        //        SetProperty(ref _uploadComment, value);
        //    }
        //}



        //private bool _isChkCommentPercentage;
        //[ProtoMember(48)]
        //public bool IsChkCommentPercentage
        //{
        //    get
        //    {
        //        return _isChkCommentPercentage;
        //    }
        //    set
        //    {
        //        if (_isChkCommentPercentage == value)
        //            return;
        //        SetProperty(ref _isChkCommentPercentage, value);
        //    }
        //}

        //private bool _isChkIncreaseEachDayComment;
        //[ProtoMember(49)]
        //public bool IsChkIncreaseEachDayComment
        //{
        //    get
        //    {
        //        return _isChkIncreaseEachDayComment;
        //    }
        //    set
        //    {
        //        if (_isChkIncreaseEachDayComment == value)
        //            return;
        //        SetProperty(ref _isChkIncreaseEachDayComment, value);
        //    }
        //}

        //private bool _isChkMaxComment;
        //[ProtoMember(50)]
        //public bool IsChkMaxComment
        //{
        //    get
        //    {
        //        return _isChkMaxComment;
        //    }
        //    set
        //    {
        //        if (_isChkMaxComment == value)
        //            return;
        //        SetProperty(ref _isChkMaxComment, value);
        //    }
        //}

        //private bool _isChkIncreaseEachDayLike;
        //[ProtoMember(51)]
        //public bool IsChkIncreaseEachDayLike
        //{
        //    get
        //    {
        //        return _isChkIncreaseEachDayLike;
        //    }
        //    set
        //    {
        //        if (_isChkIncreaseEachDayLike == value)
        //            return;
        //        SetProperty(ref _isChkIncreaseEachDayLike, value);
        //    }
        //}

        //private bool _isChkMaxLike;
        //[ProtoMember(52)]
        //public bool IsChkMaxLike
        //{
        //    get
        //    {
        //        return _isChkMaxLike;
        //    }
        //    set
        //    {
        //        if (_isChkMaxLike == value)
        //            return;
        //        SetProperty(ref _isChkMaxLike, value);
        //    }
        //}

        //private bool _isChkLikeUsersLatestPost;
        //[ProtoMember(53)]
        //public bool IsChkLikeUsersLatestPost
        //{
        //    get
        //    {
        //        return _isChkLikeUsersLatestPost;
        //    }
        //    set
        //    {
        //        if (_isChkLikeUsersLatestPost == value)
        //            return;
        //        SetProperty(ref _isChkLikeUsersLatestPost, value);
        //    }
        //}

        //private bool _isChkStopFollow;
        //[ProtoMember(54)]
        //public bool IsChkStopFollow
        //{
        //    get
        //    {
        //        return _isChkStopFollow;
        //    }

        //    set
        //    {
        //        if (value == _isChkStopFollow)
        //            return;
        //        SetProperty(ref _isChkStopFollow, value);
        //    }
        //}
        //private bool _isChkStartUnFollow;
        //[ProtoMember(55)]
        //public bool IsChkStartUnFollow
        //{
        //    get
        //    {
        //        return _isChkStartUnFollow;
        //    }

        //    set
        //    {
        //        if (value == _isChkStartUnFollow)
        //            return;
        //        SetProperty(ref _isChkStartUnFollow, value);
        //    }
        //}

        //private bool _isChkStartUnFollowWhenReached;
        //[ProtoMember(56)]
        //public bool IsChkStartUnFollowWhenReached
        //{
        //    get
        //    {
        //        return _isChkStartUnFollowWhenReached;
        //    }

        //    set
        //    {
        //        if (value == _isChkStartUnFollowWhenReached)
        //            return;
        //        SetProperty(ref _isChkStartUnFollowWhenReached, value);
        //    }
        //}
        //private RangeUtilities _startUnFollowToolWhenReach = new RangeUtilities();
        //[ProtoMember(57)]
        //public RangeUtilities StartUnFollowToolWhenReach
        //{
        //    get
        //    {
        //        return _startUnFollowToolWhenReach;
        //    }

        //    set
        //    {
        //        if (value == _startUnFollowToolWhenReach)
        //            return;
        //        SetProperty(ref _startUnFollowToolWhenReach, value);
        //    }
        //}
        //private bool _isChkWhenFollowerFollowingsIsSmallerThan;
        //[ProtoMember(58)]
        //public bool IsChkWhenFollowerFollowingsIsSmallerThan
        //{
        //    get
        //    {
        //        return _isChkWhenFollowerFollowingsIsSmallerThan;
        //    }

        //    set
        //    {
        //        if (value == _isChkWhenFollowerFollowingsIsSmallerThan)
        //            return;
        //        SetProperty(ref _isChkWhenFollowerFollowingsIsSmallerThan, value);
        //    }
        //}
        //private int _unFollowerFollowingsMaxValue;
        //[ProtoMember(59)]
        //public int UnFollowerFollowingsMaxValue
        //{
        //    get
        //    {
        //        return _unFollowerFollowingsMaxValue;
        //    }

        //    set
        //    {
        //        if (value == _unFollowerFollowingsMaxValue)
        //            return;
        //        SetProperty(ref _unFollowerFollowingsMaxValue, value);
        //    }
        //}

        //private bool _ischkwhenTheUnFollowToolGetsTemporaryBlocked;
        //[ProtoMember(60)]
        //public bool IschkwhenTheUnFollowToolGetsTemporaryBlocked
        //{
        //    get
        //    {
        //        return _ischkwhenTheUnFollowToolGetsTemporaryBlocked;
        //    }

        //    set
        //    {
        //        if (value == _ischkwhenTheUnFollowToolGetsTemporaryBlocked)
        //            return;
        //        SetProperty(ref _ischkwhenTheUnFollowToolGetsTemporaryBlocked, value);
        //    }
        //}

      //  private RangeUtilities _commentsPerUserPerUser = new RangeUtilities();

        //[ProtoMember(61)]
        //public RangeUtilities CommentsPerUser
        //{
        //    get
        //    {
        //        return _commentsPerUserPerUser;
        //    }

        //    set
        //    {
        //        if (value == _commentsPerUserPerUser)
        //            return;
        //        SetProperty(ref _commentsPerUserPerUser, value);
        //    }
        //}

        //private bool _chkUploadMessageChecked;
        //private bool _chkAddMessageChecked;

        //[ProtoMember(62)]
        //public bool ChkUploadMessageChecked
        //{
        //    get
        //    {
        //        return _chkUploadMessageChecked;
        //    }

        //    set
        //    {
        //        if (value == _chkUploadMessageChecked)
        //            return;
        //        SetProperty(ref _chkUploadMessageChecked, value);
        //    }
        //}


        //[ProtoMember(63)]
        //public bool ChkFollowUniqueUsersGlobally
        //{
        //    get
        //    {
        //        return _chkFollowUniqueUsersGlobally;
        //    }

        //    set
        //    {
        //        if (value == _chkFollowUniqueUsersGlobally)
        //            return;
        //        SetProperty(ref _chkFollowUniqueUsersGlobally, value);
        //    }
        //}

        //[ProtoMember(64)]
        //public bool ChkFollowUniqueUsersInCampaign
        //{
        //    get
        //    {
        //        return _chkFollowUniqueUsersInCampaign;
        //    }

        //    set
        //    {
        //        if (value == _chkFollowUniqueUsersInCampaign)
        //            return;
        //        SetProperty(ref _chkFollowUniqueUsersInCampaign, value);
        //    }
        //}



        //[ProtoMember(65)]
        //public bool IsCheckedStopFollowStartUnfollow
        //{
        //    get
        //    {
        //        return _isCheckedStopFollowStartUnfollow;
        //    }
        //    set
        //    {
        //        if (value == _isCheckedStopFollowStartUnfollow)
        //            return;
        //        SetProperty(ref _isCheckedStopFollowStartUnfollow, value);
        //    }
        //}


        //private RangeUtilities _delayBetweenLikesForAfterActivity = new RangeUtilities(15, 30);
        //[ProtoMember(67)]
        //public RangeUtilities DelayBetweenLikesForAfterActivity
        //{
        //    get
        //    {
        //        return _delayBetweenLikesForAfterActivity;
        //    }
        //    set
        //    {
        //        if (value == _delayBetweenLikesForAfterActivity)
        //            return;
        //        SetProperty(ref _delayBetweenLikesForAfterActivity, value);
        //    }
        //}
        
    //    private RangeUtilities _delayBetweenCommentsForAfterActivity = new RangeUtilities(15, 30);
    //    [ProtoMember(68)]
    //    public RangeUtilities DelayBetweenCommentsForAfterActivity
    //    {
    //        get
    //        {
    //            return _delayBetweenCommentsForAfterActivity;
    //        }
    //        set
    //        {
    //            if (value == _delayBetweenCommentsForAfterActivity)
    //                return;
    //            SetProperty(ref _delayBetweenCommentsForAfterActivity, value);
    //        }
    //    }

    //    private RangeUtilities _delayBetweenMessagesForAfterActivity = new RangeUtilities(15, 30);
    //    [ProtoMember(69)]
    //    public RangeUtilities DelayBetweenMessagesForAfterActivity
    //{
    //        get
    //        {
    //            return _delayBetweenMessagesForAfterActivity;
    //        }
    //        set
    //        {
    //            if (value == _delayBetweenMessagesForAfterActivity)
    //                return;
    //            SetProperty(ref _delayBetweenMessagesForAfterActivity, value);
    //        }
    //    }

    //    [ProtoMember(70)]
    //    private bool _ChkMuteFollowerAfterFollowChecked;
    //    public bool ChkMuteFollowerAfterFollowChecked
    //    {
    //        get { return _ChkMuteFollowerAfterFollowChecked; }

    //        set
    //        {
    //            if (value == _ChkMuteFollowerAfterFollowChecked)
    //                return;
    //            SetProperty(ref _ChkMuteFollowerAfterFollowChecked, value);
    //        }
    //    }       
       #endregion

    }
}
