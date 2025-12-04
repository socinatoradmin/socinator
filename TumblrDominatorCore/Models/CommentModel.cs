using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TumblrDominatorCore.Models
{
    internal interface IComment
    {
        bool IsChkAfterCommentAction { get; set; }
        bool IsChkMentionNonExistingUsers { get; set; }
        bool IsChkMentionRandomUsers { get; set; }
        bool IsChkMultipleCommentsOnSamePost { get; set; }
        bool IsChkCommentsOnOwnPost { get; set; }
        bool IsChkLikePostAfterComment { get; set; }
        bool IsChkFollowUserAfterComment { get; set; }
        bool IsChkRemovePoorQuality { get; set; }
        bool IsChkRemoveIfFollowBackRatioLimit { get; set; }
        bool IsChkSuspendToolWhenGotBlock { get; set; }
        RangeUtilities RandomlyGeneratedUsers { get; set; }
        int RemoveIfMinFollowBackRatioValue { get; set; }
        int RemoveIfFollowBackRatioAfterCommentValue { get; set; }
        int SuspendToolWhenGotBlockedValue { get; set; }
    }

    [ProtoContract]
    public class CommentModel : ModuleSetting, IComment, IGeneralSettings
    {
        private ObservableCollection<ManageCommentModel> _lstDisplayManageCommentModel =
            new ObservableCollection<ManageCommentModel>();

        //[ProtoMember(3)]
        //public ObservableCollection<ManageCommentModel> LstDisplayManageCommentModel { get; set; } = new ObservableCollection<ManageCommentModel>();
        //[ProtoMember(4)]
        //public ManageCommentModel ManageCommentModel { get; set; } = new ManageCommentModel();

        //public ObservableCollection<ManageCommentModel> LstManageCommentModel { get; set; } = new ObservableCollection<ManageCommentModel>();


        private ManageCommentModel _manageCommentModel = new ManageCommentModel();

        public JobConfiguration FastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(230, 400),
            ActivitiesPerHour = new RangeUtilities(26, 40),
            ActivitiesPerWeek = new RangeUtilities(1600, 2400),
            ActivitiesPerJob = new RangeUtilities(33, 50),
            DelayBetweenJobs = new RangeUtilities(60, 105),
            DelayBetweenActivity = new RangeUtilities(30, 50)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(120, 200),
            ActivitiesPerHour = new RangeUtilities(13, 20),
            ActivitiesPerWeek = new RangeUtilities(800, 1200),
            ActivitiesPerJob = new RangeUtilities(16, 25),
            DelayBetweenJobs = new RangeUtilities(70, 100),
            DelayBetweenActivity = new RangeUtilities(30, 40)
        };


        public JobConfiguration SlowSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(66, 100),
            ActivitiesPerHour = new RangeUtilities(6, 8),
            ActivitiesPerWeek = new RangeUtilities(400, 600),
            ActivitiesPerJob = new RangeUtilities(8, 12),
            DelayBetweenJobs = new RangeUtilities(75, 110),
            DelayBetweenActivity = new RangeUtilities(30, 60)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(466, 700),
            ActivitiesPerHour = new RangeUtilities(46, 70),
            ActivitiesPerWeek = new RangeUtilities(2800, 4200),
            ActivitiesPerJob = new RangeUtilities(58, 87),
            DelayBetweenJobs = new RangeUtilities(45, 70),
            DelayBetweenActivity = new RangeUtilities(30, 60)
        };

        public List<string> ListQueryType { get; set; } = new List<string>();

        [ProtoMember(1)]
        public override ObservableCollection<QueryInfo> SavedQueries { get; set; } =
            new ObservableCollection<QueryInfo>();

        [ProtoMember(5)]
        public ManageCommentModel ManageCommentModel
        {
            get => _manageCommentModel;
            set
            {
                if (_manageCommentModel == value)
                    return;
                SetProperty(ref _manageCommentModel, value);
            }
        }


        [ProtoMember(6)]
        public ObservableCollection<ManageCommentModel> LstDisplayManageCommentModel
        {
            get => _lstDisplayManageCommentModel;
            set
            {
                if (_lstDisplayManageCommentModel == value)
                    return;
                SetProperty(ref _lstDisplayManageCommentModel, value);
            }
        }

        [ProtoMember(2)] JobConfiguration IGeneralSettings.JobConfiguration { get; set; }


        #region IComment

        private bool _isChkAfterCommentAction;

        [ProtoMember(5)]
        public bool IsChkAfterCommentAction
        {
            get => _isChkAfterCommentAction;
            set
            {
                if (_isChkAfterCommentAction == value)
                    return;
                SetProperty(ref _isChkAfterCommentAction, value);
            }
        }

        private bool _isChkMentionNonExistingUsers;

        [ProtoMember(6)]
        public bool IsChkMentionNonExistingUsers
        {
            get => _isChkMentionNonExistingUsers;
            set
            {
                if (_isChkMentionNonExistingUsers == value)
                    return;
                SetProperty(ref _isChkMentionNonExistingUsers, value);
            }
        }

        private bool _isChkMentionRandomUsers;

        [ProtoMember(7)]
        public bool IsChkMentionRandomUsers
        {
            get => _isChkMentionRandomUsers;
            set
            {
                if (_isChkMentionRandomUsers == value)
                    return;
                SetProperty(ref _isChkMentionRandomUsers, value);
            }
        }

        private bool _isChkMultipleCommentsOnSamePost;

        [ProtoMember(8)]
        public bool IsChkMultipleCommentsOnSamePost
        {
            get => _isChkMultipleCommentsOnSamePost;
            set
            {
                if (_isChkMultipleCommentsOnSamePost == value)
                    return;
                SetProperty(ref _isChkMultipleCommentsOnSamePost, value);
            }
        }


        private bool _isChkCommentsOnOwnPost;

        [ProtoMember(9)]
        public bool IsChkCommentsOnOwnPost
        {
            get => _isChkCommentsOnOwnPost;
            set
            {
                if (_isChkCommentsOnOwnPost == value)
                    return;
                SetProperty(ref _isChkCommentsOnOwnPost, value);
            }
        }


        private bool _isChkLikePostAfterComment;

        [ProtoMember(10)]
        public bool IsChkLikePostAfterComment
        {
            get => _isChkLikePostAfterComment;
            set
            {
                if (_isChkLikePostAfterComment == value)
                    return;
                SetProperty(ref _isChkLikePostAfterComment, value);
            }
        }


        private bool _isChkFollowUserAfterComment;

        [ProtoMember(11)]
        public bool IsChkFollowUserAfterComment
        {
            get => _isChkFollowUserAfterComment;
            set
            {
                if (_isChkFollowUserAfterComment == value)
                    return;
                SetProperty(ref _isChkFollowUserAfterComment, value);
            }
        }

        private bool _isChkRemovePoorQuality;

        [ProtoMember(12)]
        public bool IsChkRemovePoorQuality
        {
            get => _isChkRemovePoorQuality;
            set
            {
                if (_isChkRemovePoorQuality == value)
                    return;
                SetProperty(ref _isChkRemovePoorQuality, value);
            }
        }


        private bool _isChkRemoveIfFollowBackRatioLimit;

        [ProtoMember(13)]
        public bool IsChkRemoveIfFollowBackRatioLimit
        {
            get => _isChkRemoveIfFollowBackRatioLimit;
            set
            {
                if (_isChkRemoveIfFollowBackRatioLimit == value)
                    return;
                SetProperty(ref _isChkRemoveIfFollowBackRatioLimit, value);
            }
        }

        private bool _isChkSuspendToolWhenGotBlock;

        [ProtoMember(14)]
        public bool IsChkSuspendToolWhenGotBlock
        {
            get => _isChkSuspendToolWhenGotBlock;
            set
            {
                if (_isChkSuspendToolWhenGotBlock == value)
                    return;
                SetProperty(ref _isChkSuspendToolWhenGotBlock, value);
            }
        }

        [ProtoMember(15)] public RangeUtilities RandomlyGeneratedUsers { get; set; } = new RangeUtilities();

        private int _removeIfMinFollowBackRatioValue;

        [ProtoMember(16)]
        public int RemoveIfMinFollowBackRatioValue
        {
            get => _removeIfMinFollowBackRatioValue;
            set
            {
                if (_removeIfMinFollowBackRatioValue == value)
                    return;
                SetProperty(ref _removeIfMinFollowBackRatioValue, value);
            }
        }

        private int _removeIfFollowBackRatioAfterCommentValue;

        [ProtoMember(17)]
        public int RemoveIfFollowBackRatioAfterCommentValue
        {
            get => _removeIfFollowBackRatioAfterCommentValue;
            set
            {
                if (_removeIfFollowBackRatioAfterCommentValue == value)
                    return;
                SetProperty(ref _removeIfFollowBackRatioAfterCommentValue, value);
            }
        }

        private int _suspendToolWhenGotBlockedValue;

        [ProtoMember(18)]
        public int SuspendToolWhenGotBlockedValue
        {
            get => _suspendToolWhenGotBlockedValue;
            set
            {
                if (_suspendToolWhenGotBlockedValue == value)
                    return;
                SetProperty(ref _suspendToolWhenGotBlockedValue, value);
            }
        }

        [ProtoMember(19)]
        public override PostFilterModel BlogFilterModel { get; set; } = new PostFilterModel();
        [ProtoMember(20)]
        public SearchFilterModel SearchFilter { get; set; } = new SearchFilterModel();
        #endregion
    }
}