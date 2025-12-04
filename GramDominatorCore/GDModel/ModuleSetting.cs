using System.Collections.ObjectModel;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary;
using ProtoBuf;
using DominatorHouseCore.Settings;
using CommonServiceLocator;

namespace GramDominatorCore.GDModel
{
    [ProtoContract]
    [ProtoInclude(100, typeof(FollowerModel))]
    [ProtoInclude(200, typeof(UnfollowerModel))]
    [ProtoInclude(300, typeof(LikeModel))]
    [ProtoInclude(400, typeof(CommentModel))]
    public class ModuleSetting : BindableBase, ISearchQueryControl, IGeneralSettings
    {
        public ModuleSetting()
        {
            var softwareSettings = InstanceProvider.GetInstance<ISoftwareSettings>();
            _isEnableAdvancedUserMode = softwareSettings.Settings?.IsEnableAdvancedUserMode?? false;
        }

        #region ISearchQueryControl
        private ObservableCollection<QueryInfo> _savedQueries = new ObservableCollection<QueryInfo>();
        [ProtoMember(1)]
        public virtual ObservableCollection<QueryInfo> SavedQueries
        {
            get
            {
                return _savedQueries;
            }
            set
            {
                if (_savedQueries != null && _savedQueries == value)
                    return;
                SetProperty(ref _savedQueries, value);
            }
        }

        #endregion

        #region IUserFilter

        private UserFilterModel _userFilterModel = new UserFilterModel();
        [ProtoMember(2)]
        public virtual UserFilterModel UserFilterModel
        {
            get
            {
                return _userFilterModel;
            }
            set
            {
                if (_userFilterModel != null && _userFilterModel == value)
                    return;
                SetProperty(ref _userFilterModel, value);
            }
        }

        public virtual ObservableCollectionCustom<BlacklistedUser> UserBlacklist { get;  }

        #endregion

        #region PostFilter

        [ProtoMember(3)]
        public virtual PostFilterModel PostFilterModel { get; set; } = new PostFilterModel();

        #endregion

        #region IJobConfigurationFilter

        private JobConfiguration _jobConfiguration;
        [ProtoMember(4)]
        public JobConfiguration JobConfiguration
        {
            get
            {
                return _jobConfiguration;
            }
            set
            {
                if (value == _jobConfiguration)
                    return;
                SetProperty(ref _jobConfiguration, value);
            }
        }

        #endregion


        private bool _isAccountGrowthActive;
        [ProtoMember(5)]
        public bool IsAccountGrowthActive
        {
            get
            {
                return _isAccountGrowthActive;
            }
            set
            {
                if (_isAccountGrowthActive == value)
                    return;
                SetProperty(ref _isAccountGrowthActive, value);
            }
        }

        private bool _isEnableAdvancedUserMode;
        [ProtoMember(26)]
        public bool IsEnableAdvancedUserMode
        {
            get
            {
                return _isEnableAdvancedUserMode;
            }
            set
            {
                if (value == _isEnableAdvancedUserMode)
                    return;
                SetProperty(ref _isEnableAdvancedUserMode, value);
            }
        }


        private MangeBlacklist.ManageBlackWhiteListModel _manageBlackWhiteListModel = new MangeBlacklist.ManageBlackWhiteListModel();
        [ProtoMember(6)]
        public virtual MangeBlacklist.ManageBlackWhiteListModel ManageBlackWhiteListModel
        {
            get
            {
                return _manageBlackWhiteListModel;
            }
            set
            {
                if (_manageBlackWhiteListModel == value)
                    return;
                SetProperty(ref _manageBlackWhiteListModel, value);
            }
        }


        
        private MangeBlacklist.SkipBlacklist _skipBlacklist = new MangeBlacklist.SkipBlacklist();
        [ProtoMember(7)]
        public virtual MangeBlacklist.SkipBlacklist SkipBlacklist
        {
            get
            {
                return _skipBlacklist;
            }
            set
            {
                SetProperty(ref _skipBlacklist, value);
            }
        }


        #region Unique feature properties for Like, Comment and Reposter module

        private bool _ischkUniqueUserForAccount;
        [ProtoMember(6)]
        public virtual bool IschkUniqueUserForAccount
        {
            get
            {
                return _ischkUniqueUserForAccount;
            }
            set
            {
                if (_ischkUniqueUserForAccount == value)
                {
                    return;
                }
                SetProperty(ref _ischkUniqueUserForAccount, value);
            }
        }

        private bool _ischkUniqueUserForCampaign;
        [ProtoMember(7)]
        public virtual bool IschkUniqueUserForCampaign
        {
            get
            {
                return _ischkUniqueUserForCampaign;
            }
            set
            {
                if (_ischkUniqueUserForCampaign == value)
                {
                    return;
                }
                SetProperty(ref _ischkUniqueUserForCampaign, value);
            }
        }

        private bool _ischkUniquePostForCampaign;
        [ProtoMember(8)]
        public virtual bool IschkUniquePostForCampaign
        {
            get
            {
                return _ischkUniquePostForCampaign;
            }
            set
            {
                if (_ischkUniquePostForCampaign == value)
                {
                    return;
                }
                SetProperty(ref _ischkUniquePostForCampaign, value);
            }
        }
       
        private bool _IschkUniqueMentionPerPostFromEachAccount;
        [ProtoMember(13)]
        public virtual bool IschkUniqueMentionPerPostFromEachAccount
        {
            get
            {
                return _IschkUniqueMentionPerPostFromEachAccount;
            }
            set
            {
                if (_IschkUniqueMentionPerPostFromEachAccount == value)
                {
                    return;
                }
                SetProperty(ref _IschkUniqueMentionPerPostFromEachAccount, value);
            }
        }

        private bool _IschkUniqueCommentPerPostFromEachAccount;
        [ProtoMember(14)]
        public virtual bool IschkUniqueCommentPerPostFromEachAccount
        {
            get
            {
                return _IschkUniqueCommentPerPostFromEachAccount;
            }
            set
            {
                if (_IschkUniqueCommentPerPostFromEachAccount == value)
                {
                    return;
                }
                SetProperty(ref _IschkUniqueCommentPerPostFromEachAccount, value);
            }
        }

        private bool _isPerformActionFromRandomPercentageOfAccounts;
        [ProtoMember(10)]
        public bool IsPerformActionFromRandomPercentageOfAccounts
        {
            get
            {
                return _isPerformActionFromRandomPercentageOfAccounts;
            }
            set
            {
                if (_isPerformActionFromRandomPercentageOfAccounts == value)
                {
                    return;
                }
                SetProperty(ref _isPerformActionFromRandomPercentageOfAccounts, value);
            }
        }

        private RangeUtilities _performActionFromRandomPercentage = new RangeUtilities(50, 100);
        [ProtoMember(11)]
        public RangeUtilities PerformActionFromRandomPercentage
        {
            get
            {
                return _performActionFromRandomPercentage;
            }
            set
            {
                if (_performActionFromRandomPercentage == value)
                {
                    return;
                }
                SetProperty(ref _performActionFromRandomPercentage, value);
            }
        }

        private bool _enableDelayBetweenPerformingActionOnSamePost;
        [ProtoMember(12)]
        public bool EnableDelayBetweenPerformingActionOnSamePost
        {
            get
            {
                return _enableDelayBetweenPerformingActionOnSamePost;
            }
            set
            {
                if (_enableDelayBetweenPerformingActionOnSamePost == value)
                {
                    return;
                }
                SetProperty(ref _enableDelayBetweenPerformingActionOnSamePost, value);
            }
        }

        private RangeUtilities _delayBetweenPerformingActionOnSamePost = new RangeUtilities(50, 100);

        public RangeUtilities DelayBetweenPerformingActionOnSamePost
        {
            get
            {
                return _delayBetweenPerformingActionOnSamePost;
            }
            set
            {
                if (_delayBetweenPerformingActionOnSamePost == value)
                {
                    return;
                }
                SetProperty(ref _delayBetweenPerformingActionOnSamePost, value);
            }
        }



        private bool _IsChkMakeCaptionAsSpinText;
        [ProtoMember(15)]
        public bool IsChkMakeCaptionAsSpinText
        {
            get
            {
                return _IsChkMakeCaptionAsSpinText;
            }
            set
            {
                if (_IsChkMakeCaptionAsSpinText == value)
                    return;
                SetProperty(ref _IsChkMakeCaptionAsSpinText, value);
            }
        }

        private bool _shouldNotBeAlreadyFollowingThisAccount;
        [ProtoMember(16)]
        public bool shouldNotBeAlreadyFollowingThisAccount
        {
            get
            {
                return _shouldNotBeAlreadyFollowingThisAccount;
            }
            set
            {
                if (_shouldNotBeAlreadyFollowingThisAccount == value)
                    return;
                SetProperty(ref _shouldNotBeAlreadyFollowingThisAccount, value);
            }
        }

        private bool _IsSkipUserWhoReceivedMessage;
        [ProtoMember(17)]
        public bool IsSkipUserWhoReceivedMessage
        {
            get
            {
                return _IsSkipUserWhoReceivedMessage;
            }
            set
            {
                if (_IsSkipUserWhoReceivedMessage == value)
                    return;
                SetProperty(ref _IsSkipUserWhoReceivedMessage, value);
            }
        }


        private RangeUtilities _DelayBetweenEachActionBlock= new RangeUtilities(1, 5);
        [ProtoMember(18)]
        public RangeUtilities DelayBetweenEachActionBlock
        {
            get
            {
                return _DelayBetweenEachActionBlock;
            }
            set
            {
                SetProperty(ref _DelayBetweenEachActionBlock, value);
            }
        }

        private bool _ChkDeletePostWhichIsPostedBySoftware;
        [ProtoMember(19)]
        public bool ChkDeletePostWhichIsPostedBySoftware
        {
            get
            {
                return _ChkDeletePostWhichIsPostedBySoftware;
            }
            set
            {
                if (_ChkDeletePostWhichIsPostedBySoftware == value)
                    return;
                SetProperty(ref _ChkDeletePostWhichIsPostedBySoftware, value);
            }
        }

        private bool _ChkDeletePostWhichIsPostedByOutsideSoftware ;
        [ProtoMember(20)]
        public bool ChkDeletePostWhichIsPostedByOutsideSoftware
        {
            get
            {
                return _ChkDeletePostWhichIsPostedByOutsideSoftware;
            }
            set
            {
                if (_ChkDeletePostWhichIsPostedByOutsideSoftware == value)
                    return;
                SetProperty(ref _ChkDeletePostWhichIsPostedByOutsideSoftware, value);
            }
        }

        private bool _IsChkRequiredData;
        [ProtoMember(21)]
        public bool IsChkRequiredData
        {
            get { return _IsChkRequiredData; }
            set { SetProperty(ref _IsChkRequiredData, value); }
        }

        private bool _IsChkPostCaption;

        [ProtoMember(22)]
        public bool IsChkPostCaption
        {
            get
            {
                return _IsChkPostCaption;
            }
            set
            {
                if (_IsChkPostCaption == value)
                    return;
                SetProperty(ref _IsChkPostCaption, value);
                if(!value)
                    IsChkMakeCaptionAsSpinText = value;
            }
        }

        private bool _IsChkUserTag;

        [ProtoMember(23)]
        public bool IsChkUserTag
        {
            get
            {
                return _IsChkUserTag;
            }
            set
            {
                if (_IsChkUserTag == value)
                    return;
                SetProperty(ref _IsChkUserTag, value);
            }
        }


        private bool _isChkLikeOnSpecificCommnetOfPost;
        [ProtoMember(24)]
        public bool isChkLikeOnSpecificCommnetOfPost
        {
            get
            {
                return _isChkLikeOnSpecificCommnetOfPost;
            }
            set
            {
                if (_isChkLikeOnSpecificCommnetOfPost && _isChkLikeOnSpecificCommnetOfPost == value)
                    return;
                SetProperty(ref _isChkLikeOnSpecificCommnetOfPost, value);
            }

        }
        private string _SpecificCommentText = string.Empty;
        [ProtoMember(25)]
        public string SpecificCommentText
        {
            get
            {
                return _SpecificCommentText;
            }
            set
            {
                if (value == _SpecificCommentText)
                    return;
                SetProperty(ref _SpecificCommentText, value);
            }
        }

        private bool _IsChkCancelPrivateRequest;
        [ProtoMember(26)]
        public bool IsChkCancelPrivateRequest
        {
            get
            {
                return _IsChkCancelPrivateRequest;
            }
            set
            {
                if (value == _IsChkCancelPrivateRequest)
                    return;
                SetProperty(ref _IsChkCancelPrivateRequest, value);
            }
        }

        private bool _IsEngagementRate;
        [ProtoMember(27)]
        public bool IsEnagementRate
        {
            get { return _IsEngagementRate; }
            set
            {
                if (value == _IsEngagementRate)
                    return;
                SetProperty(ref _IsEngagementRate, value);
            }
        }
        [ProtoMember(28)]
        private int _maxPost;
        public int MaxPost
        {
            get
            {
                return _maxPost;
            }
            set
            {
                if (value == _maxPost)
                    return;
                SetProperty(ref _maxPost, value);
            }
        }
      
        [ProtoMember (29)]
        private bool _IsUniqueComment;
        public bool IsUniqueComment
        {
            get
            {
                return _IsUniqueComment;
            }
            set
            {
                if (value == _IsUniqueComment)
                    return;
                SetProperty(ref _IsUniqueComment, value);
            }
        }

        [ProtoMember(30)]
        private bool _IsPostUniqueCommentFromEachAccount;
        public bool IsPostUniqueCommentFromEachAccount
        {
            get
            {
                return _IsPostUniqueCommentFromEachAccount;
            }
            set
            {
                if (value == _IsPostUniqueCommentFromEachAccount)
                    return;
                SetProperty(ref _IsPostUniqueCommentFromEachAccount, value);
            }
        }
   
        [ProtoMember(31)]
        private bool _IsCommentOnceFromEachAccount;
        public bool IsCommentOnceFromEachAccount
        {
            get
            {
                return _IsCommentOnceFromEachAccount;
            }
            set
            {
                if (value == _IsCommentOnceFromEachAccount)
                    return;
                SetProperty(ref _IsCommentOnceFromEachAccount, value);
            }
        }

        private bool _IsAcceptFollowRequest;
        [ProtoMember(32)]
        public bool IsAcceptFollowRequest
        {
            get
            {
                return _IsAcceptFollowRequest;
            }
            set
            {
                if (value == _IsAcceptFollowRequest)
                {
                    return;
                }
                SetProperty(ref _IsAcceptFollowRequest, value);
            }
        }
        private bool _IsFollowBack=true;
        [ProtoMember(33)]
        public bool IsFollowBack
        {
            get
            {
                return _IsFollowBack;
            }
            set
            {
                if (value == _IsFollowBack)
                {
                    return;
                }
                SetProperty(ref _IsFollowBack, value);
            }
        }

        private bool _IsPostCommentAndLikeCounte;
        [ProtoMember(34)]
        public bool IsPostCommentAndLikeCount
        {
            get { return _IsPostCommentAndLikeCounte; }
            set
            {
                if (value == _IsPostCommentAndLikeCounte)
                    return;
                SetProperty(ref _IsPostCommentAndLikeCounte, value);
            }
        }

        [ProtoMember(35)]
        private int _maxPostForCommentAndLIkeCount;
        public int maxPostForCommentAndLIkeCount
        {
            get
            {
                return _maxPostForCommentAndLIkeCount;
            }
            set
            {
                if (value == _maxPostForCommentAndLIkeCount)
                    return;
                SetProperty(ref _maxPostForCommentAndLIkeCount, value);
            }
        }
        [ProtoMember(36)]
        private bool _IsScrpeUniqueUserForThisCampaign;
        public bool IsScrpeUniqueUserForThisCampaign
        {
            get
            {
                return _IsScrpeUniqueUserForThisCampaign;
            }
            set
            {
                if (value == _IsScrpeUniqueUserForThisCampaign)
                    return;
                SetProperty(ref _IsScrpeUniqueUserForThisCampaign, value);
            }
        }

        [ProtoMember(37)]
        private bool _isScrapeTaggedUser;
        public bool IsScrapeTaggedUser
        {
            get
            {
                return _isScrapeTaggedUser;
            }
            set
            {
                if (value == _isScrapeTaggedUser)
                    return;
                SetProperty(ref _isScrapeTaggedUser, value);
            }
        }

        [ProtoMember(38)]
        private bool _isTaggedPostUser;
        public bool IsTaggedPostUser
        {
            get
            {
                return _isTaggedPostUser;
            }
            set
            {
                if (value == _isTaggedPostUser)
                    return;
                SetProperty(ref _isTaggedPostUser, value);
            }
        }

        [ProtoMember(39)]
        private bool _isTaggedUser;
        public bool IsTaggedUser
        {
            get
            {
                return _isTaggedUser;
            }
            set
            {
                if (value == _isTaggedUser)
                    return;
                SetProperty(ref _isTaggedUser, value);
            }
        }

        [ProtoMember(40)]
        private bool _IsUniqueCommentAndMention;
        public bool IsUniqueCommentAndMention
        {
            get
            {
                return _IsUniqueCommentAndMention;
            }
            set
            {
                if (value == _IsUniqueCommentAndMention)
                    return;
                SetProperty(ref _IsUniqueCommentAndMention, value);
            }
        }
        [ProtoMember(41)]
        private bool _IsUniqueMention;
        public bool IsUniqueMention
        {
            get
            {
                return _IsUniqueMention;
            }
            set
            {
                if (value == _IsUniqueMention)
                    return;
                SetProperty(ref _IsUniqueMention, value);
            }
        }

        [ProtoMember(42)]
        private bool _IsUniqueUserFromAllAccount = false;
        public bool IsSendMessageToUniqueUserFromAllAccount
        {
            get
            {
                return _IsUniqueUserFromAllAccount;
            }
            set
            {
                if (value == _IsUniqueUserFromAllAccount)
                    return;
                SetProperty(ref _IsUniqueUserFromAllAccount, value);
            }
        }

        private bool _IsChkAddMultipleComments;
        [ProtoMember(43)]
        public bool IsChkAddMultipleComments
        {
            get
            {
                return _IsChkAddMultipleComments;
            }
            set
            {
                if (_IsChkAddMultipleComments == value)
                    return;
                SetProperty(ref _IsChkAddMultipleComments, value);
            }
        }

        private bool _IsChangeHashOfImage;
        [ProtoMember(44)]
        public bool IsChangeHashOfImage
        {
            get
            {
                return _IsChangeHashOfImage;
            }
            set
            {
                if (_IsChangeHashOfImage == value)
                    return;
                SetProperty(ref _IsChangeHashOfImage, value);
            }
        }


        //IsRecentHashTagPost

        private bool _IsRecentHashTagPost = false;
        [ProtoMember(45)]
        public bool IsRecentHashTagPost
        {
            get
            {
                return _IsRecentHashTagPost;
            }
            set
            {
                if (_IsRecentHashTagPost == value)
                    return;
                SetProperty(ref _IsRecentHashTagPost, value);
            }
        }

        //IsTopRecentHashTagPost
        private bool _IsTopRecentHashTagPost = false;
        [ProtoMember(46)]
        public bool IsTopRecentHashTagPost
        {
            get
            {
                return _IsTopRecentHashTagPost;
            }
            set
            {
                if (_IsTopRecentHashTagPost == value)
                    return;
                SetProperty(ref _IsTopRecentHashTagPost, value);
            }
        }

        //IsTopHashTagPost

        private bool _IsTopHashTagPost = false;
        [ProtoMember(47)]
        public bool IsTopHashTagPost
        {
            get
            {
                return _IsTopHashTagPost;
            }
            set
            {
                if (_IsTopHashTagPost == value)
                    return;
                SetProperty(ref _IsTopHashTagPost, value);
            }
        }

        //IsTopAndRecentHashTagPost

//        private bool _IsTopAndRecentHashTagPost = false;
//        [ProtoMember(47)]
//        public bool IsTopAndRecentHashTagPost
//        {
//            get
//            {
//                return _IsTopAndRecentHashTagPost;
//            }
//            set
//            {
//                if (_IsTopAndRecentHashTagPost == value)
//                    return;
//                SetProperty(ref _IsTopAndRecentHashTagPost, value);
//            }
//        }

//<<<<<<< HEAD
//=======

        private bool _NoOfUniqueUserTag = false;
        [ProtoMember(48)]
        public bool NoOfUniqueUserTag
        {
            get { return _NoOfUniqueUserTag; }
            set
            {
                if (_NoOfUniqueUserTag == value)
                    return;
                SetProperty(ref _NoOfUniqueUserTag, value);
            }
        }

        private RangeUtilities _NoOfUserTagPerPost = new RangeUtilities();
        [ProtoMember(49)]
        public RangeUtilities NoOfUserTagPerPost
        {
            get
            {
                return _NoOfUserTagPerPost;
            }
            set
            {
                if (_NoOfUserTagPerPost == value)
                {
                    return;
                }
                SetProperty(ref _NoOfUserTagPerPost, value);
            }
        }


        private bool _unfollowFollowers;
        [ProtoMember(50)]
        public bool IsUnfollowFollowers
        {
            get
            {
                return _unfollowFollowers;
            }
            set
            {
                if (value == _unfollowFollowers)
                    return;
                SetProperty(ref _unfollowFollowers, value);
            }
        }

        private bool _unfollowFollowings;
        [ProtoMember(51)]
        public bool IsUnfollowFollowings
        {
            get
            {
                return _unfollowFollowings;
            }
            set
            {
                if (value == _unfollowFollowings)
                    return;
                SetProperty(ref _unfollowFollowings, value);
            }
        }

        private bool _followOnlyBusinessAccounts;
        [ProtoMember(52)]
        public bool FollowOnlyBusinessAccounts
        {
            get
            {
                return _followOnlyBusinessAccounts;
            }
            set
            {
                if (value == _followOnlyBusinessAccounts)
                    return;
                SetProperty(ref _followOnlyBusinessAccounts, value);
            }
        }
        #endregion
    }
}
