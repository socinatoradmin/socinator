using CommonServiceLocator;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.ObjectModel;

namespace RedditDominatorCore.RDModel
{
    [ProtoContract]
    public class ModuleSetting : BindableBase, ISearchQueryControl, IModuleSetting, IGeneralSettings
    {
        private int _activityFailedCount = 1;

        private RangeUtilities _delayBetweenPerformingActionOnSamePost = new RangeUtilities(50, 100);

        private bool _enableDelayBetweenPerformingActionOnSamePost;

        private int _failedActivityReschedule = 10;

        private bool _isAccountGrowthActive;

        private bool _isChkStopActivityAfterXXFailed;

        private bool _ischkUniquePostForCampaign;

        private bool _ischkUniqueUserForAccount;

        private bool _ischkUniqueUserForCampaign;

        private bool _isEnableAdvancedUserMode;
        //private bool _isTemplateMadeByCampaignMode;

        //public virtual bool IsTemplateMadeByCampaignMode
        //{
        //    get
        //    {
        //        return _isTemplateMadeByCampaignMode;
        //    }
        //    set
        //    {
        //        if (_isTemplateMadeByCampaignMode == value)
        //        {
        //            return;
        //        }
        //        SetProperty(ref _isTemplateMadeByCampaignMode, value);
        //    }
        //}

        private bool _isPerformActionFromRandomPercentageOfAccounts;

        private ManageBlacklist.ManageBlackWhiteListModel _manageBlackWhiteListModel =
            new ManageBlacklist.ManageBlackWhiteListModel();

        private RangeUtilities _performActionFromRandomPercentage = new RangeUtilities(50, 100);

        public ModuleSetting()
        {
            var softwareSettings = InstanceProvider.GetInstance<ISoftwareSettings>();
            _isEnableAdvancedUserMode = softwareSettings.Settings.IsEnableAdvancedUserMode;
        }

        #region PostFilter

        [ProtoMember(3)] public virtual PostFilterModel PostFilterModel { get; set; } = new PostFilterModel();

        #endregion

        [ProtoMember(5)]
        public virtual bool IschkUniqueUserForAccount
        {
            get => _ischkUniqueUserForAccount;
            set
            {
                if (_ischkUniqueUserForAccount == value) return;
                SetProperty(ref _ischkUniqueUserForAccount, value);
            }
        }

        [ProtoMember(6)]
        public virtual bool IschkUniqueUserForCampaign
        {
            get => _ischkUniqueUserForCampaign;
            set
            {
                if (_ischkUniqueUserForCampaign == value) return;
                SetProperty(ref _ischkUniqueUserForCampaign, value);
            }
        }

        [ProtoMember(7)]
        public virtual bool IschkUniquePostForCampaign
        {
            get => _ischkUniquePostForCampaign;
            set
            {
                if (_ischkUniquePostForCampaign == value) return;
                SetProperty(ref _ischkUniquePostForCampaign, value);
            }
        }

        [ProtoMember(8)]
        public bool IsPerformActionFromRandomPercentageOfAccounts
        {
            get => _isPerformActionFromRandomPercentageOfAccounts;
            set
            {
                if (_isPerformActionFromRandomPercentageOfAccounts == value) return;
                SetProperty(ref _isPerformActionFromRandomPercentageOfAccounts, value);
            }
        }

        [ProtoMember(9)]
        public RangeUtilities PerformActionFromRandomPercentage
        {
            get => _performActionFromRandomPercentage;
            set
            {
                if (_performActionFromRandomPercentage == value) return;
                SetProperty(ref _performActionFromRandomPercentage, value);
            }
        }

        [ProtoMember(10)]
        public bool EnableDelayBetweenPerformingActionOnSamePost
        {
            get => _enableDelayBetweenPerformingActionOnSamePost;
            set
            {
                if (_enableDelayBetweenPerformingActionOnSamePost == value) return;
                SetProperty(ref _enableDelayBetweenPerformingActionOnSamePost, value);
            }
        }

        [ProtoMember(11)]
        public RangeUtilities DelayBetweenPerformingActionOnSamePost
        {
            get => _delayBetweenPerformingActionOnSamePost;
            set
            {
                if (_delayBetweenPerformingActionOnSamePost == value) return;
                SetProperty(ref _delayBetweenPerformingActionOnSamePost, value);
            }
        }

        [ProtoMember(13)]
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

        [ProtoMember(15)]
        public virtual ManageBlacklist.ManageBlackWhiteListModel ManageBlackWhiteListModel
        {
            get => _manageBlackWhiteListModel;
            set
            {
                if (_manageBlackWhiteListModel == value)
                    return;
                SetProperty(ref _manageBlackWhiteListModel, value);
            }
        }

        [ProtoMember(16)]
        public bool IsChkStopActivityAfterXXFailed
        {
            get => _isChkStopActivityAfterXXFailed;
            set
            {
                if (_isChkStopActivityAfterXXFailed == value)
                    return;
                SetProperty(ref _isChkStopActivityAfterXXFailed, value);
            }
        }

        [ProtoMember(17)]
        public int ActivityFailedCount
        {
            get => _activityFailedCount;
            set
            {
                if (_activityFailedCount == value)
                    return;
                SetProperty(ref _activityFailedCount, value);
            }
        }

        [ProtoMember(18)]
        public int FailedActivityReschedule
        {
            get => _failedActivityReschedule;
            set
            {
                if (_failedActivityReschedule == value)
                    return;
                SetProperty(ref _failedActivityReschedule, value);
            }
        }

        [ProtoMember(12)]
        public bool IsAccountGrowthActive
        {
            get => _isAccountGrowthActive;
            set
            {
                if (value == _isAccountGrowthActive)
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

        #region CommunityFilters

        [ProtoMember(14)] private CommunityFiltersModel _communityFiltersModel = new CommunityFiltersModel();

        public virtual CommunityFiltersModel CommunityFiltersModel
        {
            get => _communityFiltersModel;
            set
            {
                if (_communityFiltersModel != null && _communityFiltersModel == value)
                    return;
                SetProperty(ref _communityFiltersModel, value);
            }
        }

        #endregion
    }
}