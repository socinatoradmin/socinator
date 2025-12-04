using CommonServiceLocator;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.ObjectModel;

namespace TumblrDominatorCore.Models
{
    [ProtoContract]
    [ProtoInclude(100, typeof(FollowerModel))]
    [ProtoInclude(200, typeof(UnfollowerModel))]
    // [ProtoInclude(300, typeof(LikeModel))]
    //[ProtoInclude(400, typeof(CommentModel))]
    public class ModuleSetting : BindableBase, ISearchQueryControl, IGeneralSettings
    {
        private RangeUtilities _delayBetweenPerformingActionOnSamePost = new RangeUtilities(50, 100);
        private bool _enableDelayBetweenPerformingActionOnSamePost;

        private bool _isAccountGrowthActive;

        private bool _ischkUniquePostForCampaign;


        private bool _ischkUniqueUserForAccount;
        private bool _ischkUniqueUserForCampaign;

        private bool _isEnableAdvancedUserMode;

        private bool _isPerformActionFromRandomPercentageOfAccounts;

        private RangeUtilities _performActionFromRandomPercentage = new RangeUtilities(50, 100);

        public ModuleSetting()
        {
            var softwareSettings = InstanceProvider.GetInstance<ISoftwareSettings>();
            _isEnableAdvancedUserMode = softwareSettings.Settings?.IsEnableAdvancedUserMode ?? false;
        }

        [ProtoMember(26)]
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

        [ProtoMember(4)]
        public virtual bool IschkUniqueUserForAccount
        {
            get => _ischkUniqueUserForAccount;
            set
            {
                if (_ischkUniqueUserForAccount == value) return;
                SetProperty(ref _ischkUniqueUserForAccount, value);
            }
        }

        [ProtoMember(5)]
        public virtual bool IschkUniqueUserForCampaign
        {
            get => _ischkUniqueUserForCampaign;
            set
            {
                if (_ischkUniqueUserForCampaign == value) return;
                SetProperty(ref _ischkUniqueUserForCampaign, value);
            }
        }

        [ProtoMember(6)]
        public virtual bool IschkUniquePostForCampaign
        {
            get => _ischkUniquePostForCampaign;
            set
            {
                if (_ischkUniquePostForCampaign == value) return;
                SetProperty(ref _ischkUniquePostForCampaign, value);
            }
        }

        [ProtoMember(7)]
        public virtual bool IsPerformActionFromRandomPercentageOfAccounts
        {
            get => _isPerformActionFromRandomPercentageOfAccounts;
            set
            {
                if (_isPerformActionFromRandomPercentageOfAccounts == value) return;
                SetProperty(ref _isPerformActionFromRandomPercentageOfAccounts, value);
            }
        }

        [ProtoMember(8)]
        public virtual RangeUtilities PerformActionFromRandomPercentage
        {
            get => _performActionFromRandomPercentage;
            set
            {
                if (_performActionFromRandomPercentage == value) return;
                SetProperty(ref _performActionFromRandomPercentage, value);
            }
        }

        [ProtoMember(9)]
        public bool EnableDelayBetweenPerformingActionOnSamePost
        {
            get => _enableDelayBetweenPerformingActionOnSamePost;
            set
            {
                if (_enableDelayBetweenPerformingActionOnSamePost == value) return;
                SetProperty(ref _enableDelayBetweenPerformingActionOnSamePost, value);
            }
        }

        public RangeUtilities DelayBetweenPerformingActionOnSamePost
        {
            get => _delayBetweenPerformingActionOnSamePost;
            set
            {
                if (_delayBetweenPerformingActionOnSamePost == value) return;
                SetProperty(ref _delayBetweenPerformingActionOnSamePost, value);
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
                if (_savedQueries != null && _savedQueries == value)
                    return;
                SetProperty(ref _savedQueries, value);
            }
        }

        #endregion

        #region PostFilter

        [ProtoMember(3)] public virtual PostFilterModel BlogFilterModel { get; set; } = new PostFilterModel();

        [ProtoMember(10)] public virtual UserFilterModel UserFilterModel { get; set; } = new UserFilterModel();

        #endregion

        #region IJobConfigurationFilter

        private JobConfiguration _jobConfiguration;

        [ProtoMember(4)]
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
    }
}