using System;
using System.Collections.ObjectModel;
using CommonServiceLocator;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.LDModel.Filters;
using ProtoBuf;

namespace LinkedDominatorCore.LDModel
{
    [ProtoContract]
    public class ModuleSetting : BindableBase, ISearchQueryControl
    {
        private RangeUtilities _delayBetweenPerformingActionOnSamePost = new RangeUtilities(50, 100);


        private bool _enableDelayBetweenPerformingActionOnSamePost;

        private bool _isAccountGrowthActive;

        private bool _isCampaignWiseUniqueChecked;

        private bool _ischkUniquePostForCampaign;

        private bool _ischkUniqueUserForAccount;

        private bool _ischkUniqueUserForCampaign;

        private bool _isEnableAdvancedUserMode;

        private bool _isPerformActionFromRandomPercentageOfAccount;

        private bool _isPerformActionFromRandomPercentageOfAccounts;

        private bool _isSavePagination;

        private bool _isViewProfileUsingEmbeddedBrowser;

        private RangeUtilities _performActionFromRandomPercentage = new RangeUtilities(50, 100);

        private ManageBlacklist.ManageBlackWhiteListModel _manageBlackWhiteListModel =
            new ManageBlacklist.ManageBlackWhiteListModel();

        public ModuleSetting()
        {
            try
            {
                var softwareSettings = InstanceProvider.GetInstance<ISoftwareSettings>();
                _isEnableAdvancedUserMode = softwareSettings.Settings.IsEnableAdvancedUserMode;
            }
            catch (Exception exception)
            {
                GlobusLogHelper.log.Debug(exception.ToString());
            }
        }

        #region PostFilter

        [ProtoMember(4)] public virtual LDPostFilterModel LDPostFilterModel { get; set; } = new LDPostFilterModel();

        #endregion

        [ProtoMember(5)]
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

        [ProtoMember(6)]
        public bool IsSavePagination
        {
            get => _isSavePagination;
            set => SetProperty(ref _isSavePagination, value);
        }

        [ProtoMember(7)]
        public bool IsCampaignWiseUniqueChecked
        {
            get => _isCampaignWiseUniqueChecked;
            set => SetProperty(ref _isCampaignWiseUniqueChecked, value);
        }

        [ProtoMember(8)]
        public bool IsViewProfileUsingEmbeddedBrowser
        {
            get => _isViewProfileUsingEmbeddedBrowser;
            set => SetProperty(ref _isViewProfileUsingEmbeddedBrowser, value);
        }

        [ProtoMember(9)]
        public bool IschkUniqueUserForAccount
        {
            get => _ischkUniqueUserForAccount;
            set => SetProperty(ref _ischkUniqueUserForAccount, value);
        }

        [ProtoMember(10)]
        public bool IschkUniqueUserForCampaign
        {
            get => _ischkUniqueUserForCampaign;
            set => SetProperty(ref _ischkUniqueUserForCampaign, value);
        }

        [ProtoMember(11)]
        public bool IschkUniquePostForCampaign
        {
            get => _ischkUniquePostForCampaign;
            set => SetProperty(ref _ischkUniquePostForCampaign, value);
        }

        [ProtoMember(12)]
        public bool IsPerformActionFromRandomPercentageOfAccount
        {
            get => _isPerformActionFromRandomPercentageOfAccount;
            set => SetProperty(ref _isPerformActionFromRandomPercentageOfAccount, value);
        }

        [ProtoMember(13)]
        public bool IsPerformActionFromRandomPercentageOfAccounts
        {
            get => _isPerformActionFromRandomPercentageOfAccounts;
            set => SetProperty(ref _isPerformActionFromRandomPercentageOfAccounts, value);
        }

        [ProtoMember(14)]
        public RangeUtilities PerformActionFromRandomPercentage
        {
            get => _performActionFromRandomPercentage;
            set => SetProperty(ref _performActionFromRandomPercentage, value);
        }

        [ProtoMember(15)]
        public bool EnableDelayBetweenPerformingActionOnSamePost
        {
            get => _enableDelayBetweenPerformingActionOnSamePost;
            set => SetProperty(ref _enableDelayBetweenPerformingActionOnSamePost, value);
        }

        [ProtoMember(14)]
        public RangeUtilities DelayBetweenPerformingActionOnSamePost
        {
            get => _delayBetweenPerformingActionOnSamePost;
            set => SetProperty(ref _delayBetweenPerformingActionOnSamePost, value);
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

        #region IJobConfigurationFilter

        private JobConfiguration _jobConfiguration;

        [ProtoMember(2)]
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

        #region IUserFilter

        private LDUserFilterModel _LDUserFilterModel = new LDUserFilterModel();

        [ProtoMember(3)]
        public virtual LDUserFilterModel LDUserFilterModel
        {
            get => _LDUserFilterModel;
            set
            {
                if (_LDUserFilterModel != null && _LDUserFilterModel == value)
                    return;
                SetProperty(ref _LDUserFilterModel, value);
            }
        }
        #endregion

        #region
        [ProtoMember(16)]
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
        #endregion
    }
}