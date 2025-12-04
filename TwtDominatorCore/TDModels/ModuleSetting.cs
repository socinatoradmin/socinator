using System.Collections.ObjectModel;
using CommonServiceLocator;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace TwtDominatorCore.TDModels
{
    [ProtoContract]
    [ProtoInclude(100, typeof(FollowerModel))]
    [ProtoInclude(200, typeof(UnfollowerModel))]
    [ProtoInclude(300, typeof(LikeModel))]
    public class ModuleSetting : BindableBase, ISearchQueryControl, IModuleSetting, IGeneralSettings
    {
        private RangeUtilities _delayBetweenPerformingActionOnSamePost = new RangeUtilities(50, 100);


        private bool _enableDelayBetweenPerformingActionOnSamePost;
        private bool _IsAccountGrowthActive;
        private bool _isChkPostCaption;
        private bool _ischkUniquePostForCampaign;
        private bool _ischkUniqueUserForAccount;

        private bool _ischkUniqueUserForCampaign;

        private bool _isEnableAdvancedUserMode;


        /// <summary>
        ///     AutoActivity
        /// </summary>
        private bool _IsNeedToStart;

        private bool _isPerformActionFromRandomPercentageOfAccount;

        private bool _isPerformActionFromRandomPercentageOfAccounts;
        private bool _isSpintax;


        [ProtoMember(12)]
        private ManageBlackWhiteListModel _ManageBlackWhiteListModel = new ManageBlackWhiteListModel();

        private string _originalPostCaptionInputText;

        private RangeUtilities _performActionFromRandomPercentage = new RangeUtilities(50, 100);


        [ProtoMember(13)] private SkipBlacklist _SkipBlacklist = new SkipBlacklist();


        public ModuleSetting()
        {
            var softwareSettings = InstanceProvider.GetInstance<ISoftwareSettings>();
            _isEnableAdvancedUserMode = softwareSettings.Settings.IsEnableAdvancedUserMode;
        }

        #region TweetFilter

        [ProtoMember(3)] public virtual TweetFilterModel TweetFilterModel { get; set; } = new TweetFilterModel();

        #endregion

        public ManageBlackWhiteListModel ManageBlackWhiteListModel
        {
            get => _ManageBlackWhiteListModel;
            set => SetProperty(ref _ManageBlackWhiteListModel, value);
        }

        public SkipBlacklist SkipBlacklist
        {
            get => _SkipBlacklist;
            set => SetProperty(ref _SkipBlacklist, value);
        }

        public bool IsEnableAdvancedUserMode
        {
            get => _isEnableAdvancedUserMode;
            set => SetProperty(ref _isEnableAdvancedUserMode, value);
        }

        public bool IsChkPostCaption
        {
            get => _isChkPostCaption;
            set
            {
                if (value == _isChkPostCaption)
                    return;
                SetProperty(ref _isChkPostCaption, value);
            }
        }

        public bool IsSpintax
        {
            get => _isSpintax;
            set
            {
                if (value == _isSpintax)
                    return;
                SetProperty(ref _isSpintax, value);
            }
        }

        public string OriginalPostCaptionInputText
        {
            get => _originalPostCaptionInputText;
            set => SetProperty(ref _originalPostCaptionInputText, value);
        }

        [ProtoMember(17)]
        public bool IsNeedToStart
        {
            get => _IsNeedToStart;
            set => SetProperty(ref _IsNeedToStart, value);
        }

        [ProtoMember(18)]
        public bool IschkUniqueUserForAccount
        {
            get => _ischkUniqueUserForAccount;
            set => SetProperty(ref _ischkUniqueUserForAccount, value);
        }

        [ProtoMember(19)]
        public bool IschkUniqueUserForCampaign
        {
            get => _ischkUniqueUserForCampaign;
            set => SetProperty(ref _ischkUniqueUserForCampaign, value);
        }

        [ProtoMember(20)]
        public bool IschkUniquePostForCampaign
        {
            get => _ischkUniquePostForCampaign;
            set => SetProperty(ref _ischkUniquePostForCampaign, value);
        }

        [ProtoMember(21)]
        public bool IsPerformActionFromRandomPercentageOfAccount
        {
            get => _isPerformActionFromRandomPercentageOfAccount;
            set => SetProperty(ref _isPerformActionFromRandomPercentageOfAccount, value);
        }

        [ProtoMember(22)]
        public bool IsPerformActionFromRandomPercentageOfAccounts
        {
            get => _isPerformActionFromRandomPercentageOfAccounts;
            set => SetProperty(ref _isPerformActionFromRandomPercentageOfAccounts, value);
        }

        [ProtoMember(23)]
        public RangeUtilities PerformActionFromRandomPercentage
        {
            get => _performActionFromRandomPercentage;
            set => SetProperty(ref _performActionFromRandomPercentage, value);
        }

        [ProtoMember(24)]
        public bool EnableDelayBetweenPerformingActionOnSamePost
        {
            get => _enableDelayBetweenPerformingActionOnSamePost;
            set => SetProperty(ref _enableDelayBetweenPerformingActionOnSamePost, value);
        }

        [ProtoMember(25)]
        public RangeUtilities DelayBetweenPerformingActionOnSamePost
        {
            get => _delayBetweenPerformingActionOnSamePost;
            set => SetProperty(ref _delayBetweenPerformingActionOnSamePost, value);
        }

        public bool IsAccountGrowthActive
        {
            get => _IsAccountGrowthActive;
            set => SetProperty(ref _IsAccountGrowthActive, value);
        }

        #region ISearchQueryControl

        private ObservableCollection<QueryInfo> _savedQueries = new ObservableCollection<QueryInfo>();

        [ProtoMember(1)]
        public virtual ObservableCollection<QueryInfo> SavedQueries
        {
            get => _savedQueries;
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
            get => _userFilterModel;
            set
            {
                if (_userFilterModel != null && _userFilterModel == value)
                    return;
                SetProperty(ref _userFilterModel, value);
            }
        }

        #endregion

        #region Configuration for per user Action(applicable  for only like,retweet,repost,comment,Scrape Tweet Module )

        private bool _isChkActionTweetPerUser;

        [ProtoMember(4)]
        public bool IsChkActionTweetPerUser
        {
            get => _isChkActionTweetPerUser;
            set
            {
                if (_isChkActionTweetPerUser == value)
                    return;
                SetProperty(ref _isChkActionTweetPerUser, value);
            }
        }

        private RangeUtilities _NoOfActionTweetPerUser = new RangeUtilities(2, 5);

        [ProtoMember(5)]
        public RangeUtilities NoOfActionTweetPerUser
        {
            get => _NoOfActionTweetPerUser;
            set
            {
                if (_NoOfActionTweetPerUser == value)
                    return;
                SetProperty(ref _NoOfActionTweetPerUser, value);
            }
        }

        #endregion

        #region Unfollower Setting

        private UnFollower _unfollowerModel = new UnFollower();

        [ProtoMember(6)]
        public UnFollower Unfollower
        {
            get => _unfollowerModel;
            set
            {
                if (_unfollowerModel != null && _unfollowerModel == value)
                    return;
                SetProperty(ref _unfollowerModel, value);
            }
        }

        #endregion

        #region Delete Setting

        private DeleteSetting _deleteSetting = new DeleteSetting();

        [ProtoMember(7)]
        public DeleteSetting DeleteSetting
        {
            get => _deleteSetting;
            set
            {
                if (_deleteSetting != null && _deleteSetting == value)
                    return;
                SetProperty(ref _deleteSetting, value);
            }
        }

        #endregion

        #region Download Setting

        private DownloadSettings _downloadSetting = new DownloadSettings();

        [ProtoMember(10)]
        public DownloadSettings DownloadSetting
        {
            get => _downloadSetting;
            set
            {
                if (_downloadSetting != null && _downloadSetting == value)
                    return;
                SetProperty(ref _downloadSetting, value);
            }
        }

        #endregion

        #region Message Setting

        private MessageSetting _messageSetting = new MessageSetting();

        [ProtoMember(8)]
        public MessageSetting MessageSetting
        {
            get => _messageSetting;
            set
            {
                if (_deleteSetting != null && _messageSetting == value)
                    return;
                SetProperty(ref _messageSetting, value);
            }
        }

        #endregion

        #region IJobConfigurationFilter

        private JobConfiguration _jobConfiguration;

        [ProtoMember(9)]
        public JobConfiguration JobConfiguration
        {
            get => _jobConfiguration;
            set
            {
                if (value == _jobConfiguration)
                    return;
                SetProperty(ref _jobConfiguration, value);
            }
        }

        #endregion

        #region Uniqueness

        private bool _isCampaignWiseUniqueChecked;

        [ProtoMember(11)]
        public bool IsCampaignWiseUniqueChecked
        {
            get => _isCampaignWiseUniqueChecked;
            set
            {
                if (_isCampaignWiseUniqueChecked == value)
                    return;
                SetProperty(ref _isCampaignWiseUniqueChecked, value);
            }
        }

        #endregion

        #region UnLike 

        private UnLike _UnLike = new UnLike();

        [ProtoMember(15)]
        public UnLike UnLike
        {
            get => _UnLike;
            set => SetProperty(ref _UnLike, value);
        }

        #endregion

        #region  save pagination

        private bool _isSavePagination;

        [ProtoMember(16)]
        public bool IsSavePagination
        {
            get => _isSavePagination;
            set => SetProperty(ref _isSavePagination, value);
        }

        #endregion
    }
}