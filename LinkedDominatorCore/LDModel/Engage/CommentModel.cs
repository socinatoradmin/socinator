using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Enums.LdQuery;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.LDModel.Filters;
using ProtoBuf;

namespace LinkedDominatorCore.LDModel.Engage
{
    internal interface IComment
    {
        bool IsChkAfterCommentAction { get; set; }
        bool IsChkMentionNonExistingUsers { get; set; }
        bool IsChkMentionRandomUsers { get; set; }
        bool IsChkMultipleCommentsOnSamePost { get; set; }
        bool IsChkCommentsOnOwnPost { get; set; }
        bool IsChkLikePostAfterComment { get; set; }
        bool IsChkSendRequestConnectionRequestAfterComment { get; set; }
        bool IsChkRemovePoorQuality { get; set; }
        bool IsChkRemoveIfConnectionRatioLimit { get; set; }
        bool IsChkSuspendToolWhenGotBlock { get; set; }
        RangeUtilities RandomlyGeneratedUsers { get; set; }
        int RemoveIfMinConnectionRatioValue { get; set; }
        int RemoveIfConnectionRatioAfterCommentValue { get; set; }
        int SuspendToolWhenGotBlockedValue { get; set; }
    }

    [ProtoContract]
    public class CommentModel : ModuleSetting, IComment, IGeneralSettings
    {
        private bool _isAddedToCampaign;

        private bool _IsChkGroupBlackList;

        private bool _IsChkPrivateBlackList;

        private bool _IsChkSkipBlackListedUser;
        private bool _IsChkSpintaxChecked;
        private bool _IsChkMultilineComment;

        public JobConfiguration SlowSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(10, 15),
            ActivitiesPerHour = new RangeUtilities(1, 2),
            ActivitiesPerWeek = new RangeUtilities(60, 90),
            ActivitiesPerJob = new RangeUtilities(1, 2),
            DelayBetweenJobs = new RangeUtilities(88, 133),
            DelayBetweenActivity = new RangeUtilities(30, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)

        };

        public JobConfiguration MediumSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(27, 40),
            ActivitiesPerHour = new RangeUtilities(3, 4),
            ActivitiesPerWeek = new RangeUtilities(160, 240),
            ActivitiesPerJob = new RangeUtilities(3, 5),
            DelayBetweenJobs = new RangeUtilities(87, 131),
            DelayBetweenActivity = new RangeUtilities(23, 45),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration FastSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(53, 80),
            ActivitiesPerHour = new RangeUtilities(5, 8),
            ActivitiesPerWeek = new RangeUtilities(320, 480),
            ActivitiesPerJob = new RangeUtilities(7, 10),
            DelayBetweenJobs = new RangeUtilities(87, 130),
            DelayBetweenActivity = new RangeUtilities(15, 30),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration SuperfastSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(67, 100),
            ActivitiesPerHour = new RangeUtilities(7, 10),
            ActivitiesPerWeek = new RangeUtilities(400, 600),
            ActivitiesPerJob = new RangeUtilities(8, 12),
            DelayBetweenJobs = new RangeUtilities(88, 132),
            DelayBetweenActivity = new RangeUtilities(8, 15),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public List<string> ListQueryType { get; set; } = new List<string>();

        [ProtoMember(1)]
        public new ObservableCollection<QueryInfo> SavedQueries { get; set; } = new ObservableCollection<QueryInfo>();

        [ProtoMember(2)] public override LDUserFilterModel LDUserFilterModel { get; set; } = new LDUserFilterModel();

        [ProtoMember(3)] public override LDPostFilterModel LDPostFilterModel { get; set; } = new LDPostFilterModel();

        [ProtoMember(5)]
        public ObservableCollection<ManageCommentModel> LstDisplayManageCommentModel { get; set; } =
            new ObservableCollection<ManageCommentModel>();

        public ManageCommentModel ManageCommentModel { get; set; } = new ManageCommentModel();

        public ObservableCollection<ManageCommentModel> LstManageCommentModel { get; set; } =
            new ObservableCollection<ManageCommentModel>();

        [ProtoMember(20)]
        public bool IsAddedToCampaign
        {
            get => _isAddedToCampaign;
            set
            {
                if (_isAddedToCampaign && _isAddedToCampaign == value)
                    return;
                SetProperty(ref _isAddedToCampaign, value);
            }
        }

        [ProtoMember(21)]
        public bool IsChkSkipBlackListedUser
        {
            get => _IsChkSkipBlackListedUser;
            set => SetProperty(ref _IsChkSkipBlackListedUser, value);
        }

        [ProtoMember(22)]
        public bool IsChkPrivateBlackList
        {
            get => _IsChkPrivateBlackList;
            set => SetProperty(ref _IsChkPrivateBlackList, value);
        }

        [ProtoMember(23)]
        public bool IsChkGroupBlackList
        {
            get => _IsChkGroupBlackList;
            set => SetProperty(ref _IsChkGroupBlackList, value);
        }


        [ProtoMember(24)]
        public bool IsChkSpintaxChecked
        {
            get => _IsChkSpintaxChecked;
            set => SetProperty(ref _IsChkSpintaxChecked, value);
        }

        [ProtoMember(4)] JobConfiguration IGeneralSettings.JobConfiguration { get; set; }

        public class QueryTypeWithTitle
        {
            public QueryTypeWithTitle(string queryType)
            {
                QueryType = queryType;
            }

            public string QueryType { get; set; }

            public string QueryDisplayName()
            {
                var value = (LDEngageQueryParameters)Enum.Parse(typeof(LDEngageQueryParameters), QueryType);

                var description = value.GetDescriptionAttr().FromResourceDictionary();
                return description;
            }

            public override string ToString()
            {
                return QueryDisplayName();
            }
        }

        #region IComment

        private bool _isChkAfterCommentAction;

        [ProtoMember(6)]
        public bool IsChkAfterCommentAction
        {
            get => _isChkAfterCommentAction;
            set => SetProperty(ref _isChkAfterCommentAction, value);
        }

        private bool _isChkMentionNonExistingUsers;

        [ProtoMember(7)]
        public bool IsChkMentionNonExistingUsers
        {
            get => _isChkMentionNonExistingUsers;
            set => SetProperty(ref _isChkMentionNonExistingUsers, value);
        }

        private bool _isChkMentionRandomUsers;

        [ProtoMember(8)]
        public bool IsChkMentionRandomUsers
        {
            get => _isChkMentionRandomUsers;
            set => SetProperty(ref _isChkMentionRandomUsers, value);
        }

        private bool _isChkMultipleCommentsOnSamePost;

        [ProtoMember(9)]
        public bool IsChkMultipleCommentsOnSamePost
        {
            get => _isChkMultipleCommentsOnSamePost;
            set => SetProperty(ref _isChkMultipleCommentsOnSamePost, value);
        }


        private bool _isChkCommentsOnOwnPost;

        [ProtoMember(10)]
        public bool IsChkCommentsOnOwnPost
        {
            get => _isChkCommentsOnOwnPost;
            set => SetProperty(ref _isChkCommentsOnOwnPost, value);
        }


        private bool _isChkLikePostAfterComment;

        [ProtoMember(11)]
        public bool IsChkLikePostAfterComment
        {
            get => _isChkLikePostAfterComment;
            set => SetProperty(ref _isChkLikePostAfterComment, value);
        }


        private bool _isChkSendRequestConnectionRequestAfterComment;

        [ProtoMember(12)]
        public bool IsChkSendRequestConnectionRequestAfterComment
        {
            get => _isChkSendRequestConnectionRequestAfterComment;
            set => SetProperty(ref _isChkSendRequestConnectionRequestAfterComment, value);
        }

        private bool _isChkRemovePoorQuality;

        [ProtoMember(13)]
        public bool IsChkRemovePoorQuality
        {
            get => _isChkRemovePoorQuality;
            set => SetProperty(ref _isChkRemovePoorQuality, value);
        }


        private bool _isChkRemoveIfConnectionRatioLimit;

        [ProtoMember(14)]
        public bool IsChkRemoveIfConnectionRatioLimit
        {
            get => _isChkRemoveIfConnectionRatioLimit;
            set => SetProperty(ref _isChkRemoveIfConnectionRatioLimit, value);
        }

        private bool _isChkSuspendToolWhenGotBlock;

        [ProtoMember(15)]
        public bool IsChkSuspendToolWhenGotBlock
        {
            get => _isChkSuspendToolWhenGotBlock;
            set => SetProperty(ref _isChkSuspendToolWhenGotBlock, value);
        }

        [ProtoMember(16)] public RangeUtilities RandomlyGeneratedUsers { get; set; } = new RangeUtilities();

        private int _removeIfMinConnectionRatioValue;

        [ProtoMember(17)]
        public int RemoveIfMinConnectionRatioValue
        {
            get => _removeIfMinConnectionRatioValue;
            set => SetProperty(ref _removeIfMinConnectionRatioValue, value);
        }

        private int _removeIfConnectionRatioAfterCommentValue;

        [ProtoMember(18)]
        public int RemoveIfConnectionRatioAfterCommentValue
        {
            get => _removeIfConnectionRatioAfterCommentValue;
            set => SetProperty(ref _removeIfConnectionRatioAfterCommentValue, value);
        }

        private int _suspendToolWhenGotBlockedValue;

        [ProtoMember(19)]
        public int SuspendToolWhenGotBlockedValue
        {
            get => _suspendToolWhenGotBlockedValue;
            set => SetProperty(ref _suspendToolWhenGotBlockedValue, value);
        }

        #endregion

        #region common part for engage

        private bool _isNumberOfPostToComment;

        [ProtoMember(25)]
        public bool IsNumberOfPostToComment
        {
            get => _isNumberOfPostToComment;
            set => SetProperty(ref _isNumberOfPostToComment, value);
        }

        private int _maxNumberOfPostPerUserToComment = 1;

        [ProtoMember(26)]
        public int MaxNumberOfPostPerUserToComment
        {
            get => _maxNumberOfPostPerUserToComment;
            set => SetProperty(ref _maxNumberOfPostPerUserToComment, value);
        }

        private bool _isNumberOfGroupPostToComment;

        [ProtoMember(27)]
        public bool IsNumberOfGroupPostToComment
        {
            get => _isNumberOfGroupPostToComment;
            set => SetProperty(ref _isNumberOfGroupPostToComment, value);
        }

        private int _maxNumberOfPostPerGroupToComment = 1;

        [ProtoMember(28)]
        public int MaxNumberOfPostPerGroupToComment
        {
            get => _maxNumberOfPostPerGroupToComment;
            set => SetProperty(ref _maxNumberOfPostPerGroupToComment, value);
        }

        [ProtoMember(25)]
        public bool IsChkMultilineComment
        {
            get => _IsChkMultilineComment;
            set => SetProperty(ref _IsChkMultilineComment, value);
        }

        #endregion
    }
}