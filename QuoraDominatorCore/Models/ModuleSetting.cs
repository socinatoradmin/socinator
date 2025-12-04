using System.Collections.ObjectModel;
using CommonServiceLocator;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace QuoraDominatorCore.Models
{
    [ProtoContract]
    [ProtoInclude(100, typeof(FollowerModel))]
    [ProtoInclude(200, typeof(UnfollowerModel))]
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

        #region PostFilter

        [ProtoMember(3)] public virtual PostFilterModel PostFilterModel { get; set; } = new PostFilterModel();

        #endregion

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

        #region

        private QuestionFilterModel _questionFilterModel = new QuestionFilterModel();

        [ProtoMember(3)]
        public virtual QuestionFilterModel QuestionFilterModel
        {
            get => _questionFilterModel;
            set
            {
                if (_questionFilterModel != null && _questionFilterModel == value)
                    return;
                SetProperty(ref _questionFilterModel, value);
            }
        }

        #endregion
        #region Topic Filter model.
        private TopicFilterModel _topicFilterModel = new TopicFilterModel();
        [ProtoMember(10)]
        public virtual TopicFilterModel TopicFilter
        {
            get=> _topicFilterModel;
            set
            {
                if(_topicFilterModel==value) return;
                SetProperty(ref _topicFilterModel, value);
            }
        }
        #endregion
        #region

        private QuestionFilterModel _customQuestionFilterModel = new QuestionFilterModel();

        [ProtoMember(3)]
        public virtual QuestionFilterModel CustomQuestionFilterModel
        {
            get => _customQuestionFilterModel;
            set
            {
                if (_customQuestionFilterModel != null && _customQuestionFilterModel == value)
                    return;
                SetProperty(ref _customQuestionFilterModel, value);
            }
        }

        #endregion

        #region

        private AnswerFilterModel _answerFilterModel = new AnswerFilterModel();

        [ProtoMember(4)]
        public virtual AnswerFilterModel AnswerFilterModel
        {
            get => _answerFilterModel;
            set
            {
                if (_answerFilterModel != null && _answerFilterModel == value)
                    return;
                SetProperty(ref _answerFilterModel, value);
            }
        }

        #endregion

        #region

        private AnswerFilterModel _customanswerFilterModel = new AnswerFilterModel();

        [ProtoMember(7)]
        public virtual AnswerFilterModel CustomAnswerFilterModel
        {
            get => _customanswerFilterModel;
            set
            {
                if (_customanswerFilterModel != null && _customanswerFilterModel == value)
                    return;
                SetProperty(ref _customanswerFilterModel, value);
            }
        }

        #endregion

        #region

        private UnfollowerModel _unfollowFilterModel = new UnfollowerModel();

        [ProtoMember(5)]
        public virtual UnfollowerModel UnFollowFilterModel
        {
            get => _unfollowFilterModel;
            set
            {
                if (_unfollowFilterModel != null && _unfollowFilterModel == value)
                    return;
                SetProperty(ref _unfollowFilterModel, value);
            }
        }

        #endregion

        #region

        private UserFilterModel _customuserfilter = new UserFilterModel();

        [ProtoMember(6)]
        public virtual UserFilterModel CustomUserFilterModel
        {
            get => _customuserfilter;
            set
            {
                if (_questionFilterModel != null && _customuserfilter == value)
                    return;
                SetProperty(ref _customuserfilter, value);
            }
        }

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