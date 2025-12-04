using System.Collections.ObjectModel;
using CommonServiceLocator;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace PinDominatorCore.PDModel
{
    [ProtoContract]
    [ProtoInclude(100, typeof(FollowerModel))]
    [ProtoInclude(200, typeof(UnfollowerModel))]
    [ProtoInclude(300, typeof(TryModel))]
    [ProtoInclude(400, typeof(CommentModel))]
    public class ModuleSetting : BindableBase, ISearchQueryControl, IGeneralSettings
    {
        private bool _isAccountGrowthActive;

        private bool _isEnableAdvancedUserMode;

        public ModuleSetting()
        {
            var softwareSettings = InstanceProvider.GetInstance<ISoftwareSettings>();
            _isEnableAdvancedUserMode = softwareSettings.Settings.IsEnableAdvancedUserMode;
        }

        #region PostFilter

        [ProtoMember(3)] public virtual PostFilterModel PostFilterModel { get; set; } = new PostFilterModel();

        #endregion
        
        public virtual ObservableCollectionBase<BoardInfo> BoardDetails { get; set; }

        public bool IsEnableAdvancedUserMode
        {
            get => _isEnableAdvancedUserMode;
            set
            {
                if (value == _isEnableAdvancedUserMode)
                    return;
                SetProperty(ref _isEnableAdvancedUserMode, value);
            }
        }

        public bool IsAccountGrowthActive
        {
            get => _isAccountGrowthActive;
            set
            {
                if (_isAccountGrowthActive == value)
                    return;
                SetProperty(ref _isAccountGrowthActive, value);
            }
        }

        #region ISearchQueryControl

        private ObservableCollection<QueryInfo> _savedQueries = new ObservableCollection<QueryInfo>();

        [ProtoMember(1)]
        public virtual ObservableCollection<QueryInfo> SavedQueries
        {
            get => _savedQueries;
            set
            {
                if (_savedQueries != null && _savedQueries == value) return;
                SetProperty(ref _savedQueries, value);
            }
        }

        #endregion

        #region IUserFilter

        [ProtoMember(2)] private UserFilterModel _userFilterModel = new UserFilterModel();

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

        #region IJobConfigurationFilter

        private JobConfiguration _jobConfiguration;

        [ProtoMember(4)]
        public virtual JobConfiguration JobConfiguration
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

        private bool _ischkUniqueUserForAccount;
        [ProtoMember(5)]
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
        [ProtoMember(6)]
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
        [ProtoMember(7)]
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

        private bool _isPerformActionFromRandomPercentageOfAccounts;
        [ProtoMember(8)]
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
        [ProtoMember(9)]
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
        [ProtoMember(10)]
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
        [ProtoMember(11)]
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

        private bool _isChkStopActivityAfterXxFailed;
        [ProtoMember(12)]
        public bool IsChkStopActivityAfterXXFailed
        {
            get { return _isChkStopActivityAfterXxFailed; }
            set
            {
                if (_isChkStopActivityAfterXxFailed == value)
                    return;
                SetProperty(ref _isChkStopActivityAfterXxFailed, value);
            }
        }

        private int _activityFailedCount = 1;
        [ProtoMember(13)]
        public int ActivityFailedCount
        {
            get { return _activityFailedCount; }
            set
            {
                if (_activityFailedCount == value)
                    return;
                SetProperty(ref _activityFailedCount, value);
            }
        }

        private int _failedActivityReschedule = 10;
        [ProtoMember(14)]
        public int FailedActivityReschedule
        {
            get { return _failedActivityReschedule; }
            set
            {
                if (_failedActivityReschedule == value)
                    return;
                SetProperty(ref _failedActivityReschedule, value);
            }
        }

        private bool _skipUserWhoHasEverReceivedMessage;
        [ProtoMember(15)]
        public bool SkipUserWhoHasEverReceivedMessage
        {
            get { return _skipUserWhoHasEverReceivedMessage; }
            set
            {
                if (_skipUserWhoHasEverReceivedMessage == value)
                    return;
                SetProperty(ref _skipUserWhoHasEverReceivedMessage, value);
            }
        }
    }
}