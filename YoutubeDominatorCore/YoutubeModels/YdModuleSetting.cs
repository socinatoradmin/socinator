using CommonServiceLocator;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.ObjectModel;

namespace YoutubeDominatorCore.YoutubeModels
{
    [ProtoContract]
    public class YdModuleSetting : BindableBase, ISearchQueryControl, IModuleSetting, IGeneralSettings
    {
        private ChannelFilterModel _channelFilterModel = new ChannelFilterModel();
        private RangeUtilities _delayBetweenPerformingActionOnSamePost = new RangeUtilities(50, 100);

        private bool _enableDelayBetweenPerformingActionOnSamePost;

        private bool _isAccountGrowthActive;

        private bool _ischkUniquePostForCampaign;

        private bool _ischkUniqueUserForAccount;

        private bool _ischkUniqueUserForCampaign;

        private bool _isEnableAdvancedUserMode;

        private bool _isPerformActionFromRandomPercentageOfAccounts;


        private ManageBlackWhiteListModel _ManageBlackWhiteListModel = new ManageBlackWhiteListModel();

        private RangeUtilities _performActionFromRandomPercentage = new RangeUtilities(50, 100);

        private ObservableCollection<QueryInfo> _savedQueries = new ObservableCollection<QueryInfo>();


        private SkipBlacklist _SkipBlacklist = new SkipBlacklist();

        private VideoFilterModel _videoFilterModel = new VideoFilterModel();

        public YdModuleSetting()
        {
            var softwareSettings = InstanceProvider.GetInstance<ISoftwareSettings>();
            _isEnableAdvancedUserMode = softwareSettings.Settings.IsEnableAdvancedUserMode;
        }

        [ProtoMember(4)]
        public virtual VideoFilterModel VideoFilterModel
        {
            get => _videoFilterModel;
            set => SetProperty(ref _videoFilterModel, value);
        }

        [ProtoMember(5)]
        public virtual ChannelFilterModel ChannelFilterModel
        {
            get => _channelFilterModel;
            set => SetProperty(ref _channelFilterModel, value);
        }

        [ProtoMember(7)]
        public bool EnableDelayBetweenPerformingActionOnSamePost
        {
            get => _enableDelayBetweenPerformingActionOnSamePost;
            set => SetProperty(ref _enableDelayBetweenPerformingActionOnSamePost, value);
        }

        [ProtoMember(8)]
        public RangeUtilities DelayBetweenPerformingActionOnSamePost
        {
            get => _delayBetweenPerformingActionOnSamePost;
            set => SetProperty(ref _delayBetweenPerformingActionOnSamePost, value);
        }

        [ProtoMember(9)]
        public ManageBlackWhiteListModel ManageBlackWhiteListModel
        {
            get => _ManageBlackWhiteListModel;
            set => SetProperty(ref _ManageBlackWhiteListModel, value);
        }

        [ProtoMember(10)]
        public SkipBlacklist SkipBlacklist
        {
            get => _SkipBlacklist;
            set => SetProperty(ref _SkipBlacklist, value);
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

        [ProtoMember(11)]
        public bool IsPerformActionFromRandomPercentageOfAccounts
        {
            get => _isPerformActionFromRandomPercentageOfAccounts;
            set
            {
                if (_isPerformActionFromRandomPercentageOfAccounts == value) return;
                SetProperty(ref _isPerformActionFromRandomPercentageOfAccounts, value);
            }
        }

        [ProtoMember(12)]
        public RangeUtilities PerformActionFromRandomPercentage
        {
            get => _performActionFromRandomPercentage;
            set
            {
                if (_performActionFromRandomPercentage == value) return;
                SetProperty(ref _performActionFromRandomPercentage, value);
            }
        }

        [ProtoMember(13)]
        public virtual bool IschkUniquePostForCampaign
        {
            get => _ischkUniquePostForCampaign;
            set
            {
                if (_ischkUniquePostForCampaign == value) return;
                SetProperty(ref _ischkUniquePostForCampaign, value);
            }
        }

        [ProtoMember(14)]
        public virtual bool IschkUniqueUserForCampaign
        {
            get => _ischkUniqueUserForCampaign;
            set
            {
                if (_ischkUniqueUserForCampaign == value) return;
                SetProperty(ref _ischkUniqueUserForCampaign, value);
            }
        }

        [ProtoMember(15)]
        public virtual bool IschkUniqueUserForAccount
        {
            get => _ischkUniqueUserForAccount;
            set
            {
                if (_ischkUniqueUserForAccount == value) return;
                SetProperty(ref _ischkUniqueUserForAccount, value);
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

        [ProtoMember(1)]
        public virtual ObservableCollection<QueryInfo> SavedQueries
        {
            get => _savedQueries;
            set => SetProperty(ref _savedQueries, value);
        }

        #region IJobConfigurationFilter

        private JobConfiguration _jobConfiguration;

        [ProtoMember(6)]
        public JobConfiguration JobConfiguration
        {
            get => _jobConfiguration;
            set => SetProperty(ref _jobConfiguration, value);
        }

        #endregion
    }
}