using CommonServiceLocator;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.FilterModel;
using ProtoBuf;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using static FaceDominatorCore.FDLibrary.FdClassLibrary.MangeBlacklist;
// ReSharper disable UnusedMember.Global

namespace FaceDominatorCore.FDModel.CommonSettings
{
    [ProtoContract]
    public class ModuleSetting : BindableBase, ISearchQueryControl, IGeneralSettings
    {
        public ModuleSetting()
        {
            var softwareSettings = InstanceProvider.GetInstance<ISoftwareSettings>();
            _isEnableAdvancedUserMode = softwareSettings.Settings.IsEnableAdvancedUserMode;
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
                SetProperty(ref _userFilterModel, value);
            }
        }



        #endregion


        #region IGenderFilter

        /*private FdGenderFilterModel _genderFilterModel = new FdGenderFilterModel();
        [ProtoMember(3)]
        public virtual FdGenderFilterModel GenderFilterModel
        {
            get
            {
                return _genderFilterModel;
            }
            set
            {
                if (_genderFilterModel != null && _genderFilterModel == value)
                    return;
                SetProperty(ref _genderFilterModel, value);
            }
        }*/


        #endregion



        #region PostFilter

        [ProtoMember(4)]
        public virtual PostFilterModel PostFilterModel { get; set; } = new PostFilterModel();

        #endregion


        #region GroupFilter

        private FdGroupFilterModel _groupFilterModel = new FdGroupFilterModel();


        [ProtoMember(5)]
        public virtual FdGroupFilterModel GroupFilterModel
        {
            get
            {
                return _groupFilterModel;
            }
            set
            {
                SetProperty(ref _groupFilterModel, value);
            }
        }

        #endregion



        #region GroupFilter

        private FdFanpageFilterModel _fanpageFilterModel = new FdFanpageFilterModel();


        [ProtoMember(6)]
        public virtual FdFanpageFilterModel FanpageFilterModel
        {
            get
            {
                return _fanpageFilterModel;
            }
            set
            {
                SetProperty(ref _fanpageFilterModel, value);
            }
        }

        #endregion



        private LikerCommentorConfigModel _likerCommentorConfigModel = new LikerCommentorConfigModel();

        [ProtoMember(7)]
        public virtual LikerCommentorConfigModel LikerCommentorConfigModel
        {
            get
            {
                return _likerCommentorConfigModel;
            }
            set
            {
                SetProperty(ref _likerCommentorConfigModel, value);
            }
        }


        private PostLikeCommentorModel _postLikerCommentor = new PostLikeCommentorModel();

        [ProtoMember(8)]
        public virtual PostLikeCommentorModel PostLikeCommentorModel
        {
            get
            {
                return _postLikerCommentor;
            }
            set
            {
                SetProperty(ref _postLikerCommentor, value);
            }
        }




        #region IJobConfigurationFilter

        private JobConfiguration _jobConfiguration;
        [ProtoMember(9)]
        public virtual JobConfiguration JobConfiguration
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

        private bool _isAccountGrowthActive;

        [ProtoMember(19)]
        public bool IsAccountGrowthActive //IsAccountGrowthActive
        {
            get
            {
                return _isAccountGrowthActive;
            }

            set
            {
                if (value == _isAccountGrowthActive)
                    return;
                SetProperty(ref _isAccountGrowthActive, value);
            }
        }


        private List<string> _listKeywordsNonQuery = new List<string>();


        [ProtoMember(10)]
        public virtual List<string> ListKeywordsNonQuery
        {
            get { return _listKeywordsNonQuery; }
            set
            {
                if (value == _listKeywordsNonQuery)
                    return;
                SetProperty(ref _listKeywordsNonQuery, value);
            }
        }



        private FdGenderAndLocationFilterModel _genderAndLocationFilterModel = new FdGenderAndLocationFilterModel();

        [ProtoMember(11)]
        public virtual FdGenderAndLocationFilterModel GenderAndLocationFilter
        {
            get { return _genderAndLocationFilterModel; }
            set
            {
                if (value == _genderAndLocationFilterModel)
                    return;
                SetProperty(ref _genderAndLocationFilterModel, value);
            }
        }

        #endregion


        private ObservableCollection<ManageMessagesModel> _lstManageMessagesModel = new ObservableCollection<ManageMessagesModel>();


        [ProtoMember(12)]
        public virtual ObservableCollection<ManageMessagesModel> LstDisplayManageMessageModel
        {
            get { return _lstManageMessagesModel; }
            set
            {
                SetProperty(ref _lstManageMessagesModel, value);
            }
        }


        private List<string> _postLikeCommentOptions = new List<string>();


        [ProtoMember(13)]
        public virtual List<string> PostLikeCommentOptions
        {
            get { return _postLikeCommentOptions; }
            set
            {
                SetProperty(ref _postLikeCommentOptions, value);
            }
        }

        private FdCommentFilterModel _commentFilterModel = new FdCommentFilterModel();


        [ProtoMember(13)]
        public virtual FdCommentFilterModel CommentFilterModel
        {
            get { return _commentFilterModel; }
            set
            {
                SetProperty(ref _commentFilterModel, value);
            }
        }



        private OtherConfigModel _otherConfigModel = new OtherConfigModel();

        [ProtoMember(14)]
        public virtual OtherConfigModel OtherConfigModel
        {
            get { return _otherConfigModel; }
            set
            {
                SetProperty(ref _otherConfigModel, value);
            }
        }

        private ManageFriends _manageFriendsModel = new ManageFriends();

        [ProtoMember(15)]
        public virtual ManageFriends ManageFriendsModel
        {
            get { return _manageFriendsModel; }
            set
            {
                SetProperty(ref _manageFriendsModel, value);
            }
        }


        private UnfriendOption _unfriendOptionModel = new UnfriendOption();

        [ProtoMember(16)]
        public virtual UnfriendOption UnfriendOptionModel
        {
            get { return _unfriendOptionModel; }
            set
            {
                SetProperty(ref _unfriendOptionModel, value);
            }
        }


        private InviterOptions _inviterOptionsModel = new InviterOptions();

        [ProtoMember(17)]
        public virtual InviterOptions InviterOptionsModel
        {
            get { return _inviterOptionsModel; }
            set
            {
                SetProperty(ref _inviterOptionsModel, value);
            }
        }

        private InviterDetails _inviterDetails = new InviterDetails();

        [ProtoMember(18)]
        public virtual InviterDetails InviterDetailsModel
        {
            get { return _inviterDetails; }
            set
            {
                SetProperty(ref _inviterDetails, value);
            }
        }

        private ManageBlackWhiteListModel _manageBlackWhiteListModel = new ManageBlackWhiteListModel();

        [ProtoMember(19)]
        public virtual ManageBlackWhiteListModel ManageBlackWhiteListModel
        {
            get { return _manageBlackWhiteListModel; }
            set
            {
                SetProperty(ref _manageBlackWhiteListModel, value);
            }
        }

        private SkipBlacklist _skipBlacklist = new SkipBlacklist();

        [ProtoMember(20)]
        public virtual SkipBlacklist SkipBlacklist
        {
            get { return _skipBlacklist; }
            set
            {
                SetProperty(ref _skipBlacklist, value);
            }
        }


        private AutoReplyOptionModel _autoReplyOptionModel = new AutoReplyOptionModel();

        [ProtoMember(21)]
        public virtual AutoReplyOptionModel AutoReplyOptionModel
        {
            get { return _autoReplyOptionModel; }
            set
            {
                SetProperty(ref _autoReplyOptionModel, value);
            }
        }

        private bool _isEnableAdvancedUserMode;

        public bool IsEnableAdvancedUserMode
        {
            get
            {
                return _isEnableAdvancedUserMode;
            }
            set
            {
                SetProperty(ref _isEnableAdvancedUserMode, value);
            }
        }

        private List<string> _messageRequestFilterText = new List<string>();


        [ProtoMember(22)]
        public virtual List<string> LstMessage
        {
            get
            {
                return _messageRequestFilterText;
            }
            set
            {
                SetProperty(ref _messageRequestFilterText, value);
            }
        }


        public SelectAccountDetailsModel SelctAccountDetailsModel;

        [ProtoMember(23)]
        public virtual SelectAccountDetailsModel SelectAccountDetailsModel
        {
            get
            {
                return SelctAccountDetailsModel;
            }
            set
            {
                SetProperty(ref SelctAccountDetailsModel, value);
            }
        }



        private FdGenderAndLocationFilterModel _genderAndLocationCancelFilter = new FdGenderAndLocationFilterModel();

        [ProtoMember(24)]
        public virtual FdGenderAndLocationFilterModel GenderAndLocationCancelFilter
        {
            get { return _genderAndLocationCancelFilter; }
            set
            {
                SetProperty(ref _genderAndLocationCancelFilter, value);
            }
        }


        private ObservableCollection<ManageCommentModel> _lstManageCommentModel = new ObservableCollection<ManageCommentModel>();

        [ProtoMember(25)]
        public virtual ObservableCollection<ManageCommentModel> LstManageCommentModel
        {
            get { return _lstManageCommentModel; }
            set
            {
                SetProperty(ref _lstManageCommentModel, value);
            }
        }


        private bool _ischkUniqueRequestChecked;

        [ProtoMember(26)]
        public virtual bool IschkUniqueRequest
        {
            get { return _ischkUniqueRequestChecked; }
            set
            {
                SetProperty(ref _ischkUniqueRequestChecked, value);
            }
        }

        private MarketplaceFilterModel _marketplaceFilterModel;

        [ProtoMember(27)]
        public virtual MarketplaceFilterModel MarketplaceFilterModel
        {
            get { return _marketplaceFilterModel; }
            set
            {
                SetProperty(ref _marketplaceFilterModel, value);
            }
        }

        private FdPlaceFilterModel _fdPlaceFilterModel;

        [ProtoMember(28)]
        public virtual FdPlaceFilterModel FdPlaceFilterModel
        {
            get { return _fdPlaceFilterModel; }
            set
            {
                SetProperty(ref _fdPlaceFilterModel, value);
            }
        }

        private EventCreaterManagerModel _eventCreaterManagerModel = new EventCreaterManagerModel();
        [ProtoMember(29)]
        public virtual EventCreaterManagerModel EventCreaterManagerModel
        {
            get { return _eventCreaterManagerModel; }
            set
            {
                SetProperty(ref _eventCreaterManagerModel, value);

            }
        }

        private bool _ischkUniqueUserForAccount;

        [ProtoMember(30)]
        public virtual bool IschkUniqueUserForAccount
        {
            get { return _ischkUniqueUserForAccount; }
            set
            {
                SetProperty(ref _ischkUniqueUserForAccount, value);
            }
        }


        private ObservableCollection<EventCreaterManagerModel> _lstManageEventModel;


        [ProtoMember(31)]
        public virtual ObservableCollection<EventCreaterManagerModel> LstManageEventModel
        {
            get { return _lstManageEventModel; }
            set
            {
                SetProperty(ref _lstManageEventModel, value);

            }
        }


        private bool _ischkUniqueUserForCampaign;

        [ProtoMember(32)]
        public virtual bool IschkUniqueUserForCampaign
        {
            get { return _ischkUniqueUserForCampaign; }
            set
            {
                SetProperty(ref _ischkUniqueUserForCampaign, value);
            }
        }

        private bool _ischkUniquePostForCampaign;

        [ProtoMember(33)]
        public virtual bool IschkUniquePostForCampaign
        {
            get { return _ischkUniquePostForCampaign; }
            set
            {
                SetProperty(ref _ischkUniquePostForCampaign, value);
            }
        }

        private bool _isPerformActionFromRandomPercentageOfAccounts;

        [ProtoMember(34)]
        public virtual bool IsPerformActionFromRandomPercentageOfAccounts
        {
            get { return _isPerformActionFromRandomPercentageOfAccounts; }
            set
            {
                SetProperty(ref _isPerformActionFromRandomPercentageOfAccounts, value);
            }
        }

        private RangeUtilities _performActionFromRandomPercentage = new RangeUtilities(5, 10);

        [ProtoMember(35)]
        public virtual RangeUtilities PerformActionFromRandomPercentage
        {
            get { return _performActionFromRandomPercentage; }
            set
            {
                SetProperty(ref _performActionFromRandomPercentage, value);
            }
        }

        private bool _enableDelayBetweenPerformingActionOnSamePost;

        [ProtoMember(36)]
        public virtual bool EnableDelayBetweenPerformingActionOnSamePost
        {
            get { return _enableDelayBetweenPerformingActionOnSamePost; }
            set
            {
                SetProperty(ref _enableDelayBetweenPerformingActionOnSamePost, value);
            }
        }


        private RangeUtilities _delayBetweenPerformingActionOnSamePost = new RangeUtilities(5, 10);

        [ProtoMember(37)]
        public virtual RangeUtilities DelayBetweenPerformingActionOnSamePost
        {
            get { return _delayBetweenPerformingActionOnSamePost; }
            set
            {
                SetProperty(ref _delayBetweenPerformingActionOnSamePost, value);
            }
        }


    }
}
