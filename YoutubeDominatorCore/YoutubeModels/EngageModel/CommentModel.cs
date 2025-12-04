using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace YoutubeDominatorCore.YoutubeModels.EngageModel
{
    [ProtoContract]
    public class CommentModel : YdModuleSetting
    {
        private bool _commentAsReplyOnlyChecked;

        private string _commentIdForReply;

        private bool _isCheckAfterCommentAction;

        private bool _isCheckFixCommentCountForPost;

        private bool _isCheckLikePostAfterComment;

        private bool _isCheckReplyNCommentsOnly;


        private bool _IsCheckSkipLimitation;

        private bool _isChkAddMultipleComments;

        private bool _isChkAllowMultipleCommentsOnSamePost;

        private bool _isChkCommentUnique;

        private bool _isChkGroupBlackList;

        private bool _isChkPrivateBlackList;

        private bool _isChkSkipBlackListedUser;

        private bool _isCommentOnceFromEachAccount;


        private bool _isPostUniqueCommentFromEachAccount = true;
        private bool _isSpintax;

        private List<string> _listOfChannels = new List<string>();

        private ObservableCollection<ChannelDestinationSelectModel> _listSelectDestination =
            new ObservableCollection<ChannelDestinationSelectModel>();

        private int _multipleActionCount = 1;
        private RangeUtilities _numberOfCommentPostPerQuery = new RangeUtilities(1, 5);

        private RangeUtilities _numberOfSkippingUrl = new RangeUtilities(1, 100);
        private RangeUtilities _replyNComments = new RangeUtilities(4, 8);

        private bool _replyToComments;

        public JobConfiguration FastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(66, 100),
            ActivitiesPerHour = new RangeUtilities(6, 10),
            ActivitiesPerWeek = new RangeUtilities(400, 600),
            ActivitiesPerJob = new RangeUtilities(8, 12),
            DelayBetweenJobs = new RangeUtilities(85, 127),
            DelayBetweenActivity = new RangeUtilities(18, 36),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(26, 40),
            ActivitiesPerHour = new RangeUtilities(2, 4),
            ActivitiesPerWeek = new RangeUtilities(160, 240),
            ActivitiesPerJob = new RangeUtilities(3, 5),
            DelayBetweenJobs = new RangeUtilities(86, 130),
            DelayBetweenActivity = new RangeUtilities(30, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };


        public JobConfiguration SlowSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(13, 20),
            ActivitiesPerHour = new RangeUtilities(1, 2),
            ActivitiesPerWeek = new RangeUtilities(80, 120),
            ActivitiesPerJob = new RangeUtilities(1, 2),
            DelayBetweenJobs = new RangeUtilities(86, 130),
            DelayBetweenActivity = new RangeUtilities(60, 120),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(200, 300),
            ActivitiesPerHour = new RangeUtilities(20, 30),
            ActivitiesPerWeek = new RangeUtilities(1200, 1800),
            ActivitiesPerJob = new RangeUtilities(25, 37),
            DelayBetweenJobs = new RangeUtilities(83, 125),
            DelayBetweenActivity = new RangeUtilities(7, 15),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        [ProtoMember(1)]
        public override ObservableCollection<QueryInfo> SavedQueries { get; set; } =
            new ObservableCollection<QueryInfo>();

        public List<string> ListQueryType { get; set; } = new List<string>();

        [ProtoMember(4)] public override ChannelFilterModel ChannelFilterModel { get; set; } = new ChannelFilterModel();

        [ProtoMember(5)] public override VideoFilterModel VideoFilterModel { get; set; } = new VideoFilterModel();

        [ProtoMember(7)]
        public bool IsChkSkipBlackListedUser
        {
            get => _isChkSkipBlackListedUser;
            set
            {
                if (_isChkSkipBlackListedUser == value) return;
                SetProperty(ref _isChkSkipBlackListedUser, value);
            }
        }

        [ProtoMember(8)]
        public bool IsChkPrivateBlackList
        {
            get => _isChkPrivateBlackList;
            set
            {
                if (_isChkPrivateBlackList == value) return;
                SetProperty(ref _isChkPrivateBlackList, value);
            }
        }

        [ProtoMember(9)]
        public bool IsChkGroupBlackList
        {
            get => _isChkGroupBlackList;
            set
            {
                if (_isChkGroupBlackList == value) return;
                SetProperty(ref _isChkGroupBlackList, value);
            }
        }

        [ProtoMember(10)]
        public bool IsChkCommentUnique
        {
            get => _isChkCommentUnique;

            set
            {
                if (value == _isChkCommentUnique)
                    return;
                SetProperty(ref _isChkCommentUnique, value);
                if (value && IsCheckReplyNCommentsOnly)
                    IsCheckReplyNCommentsOnly = false;
            }
        }

        [ProtoMember(11)]
        public bool IsChkAddMultipleComments
        {
            get => _isChkAddMultipleComments;

            set
            {
                if (value == _isChkAddMultipleComments)
                    return;
                SetProperty(ref _isChkAddMultipleComments, value);
            }
        }

        [ProtoMember(12)]
        public bool IsChkAllowMultipleCommentsOnSamePost
        {
            get => _isChkAllowMultipleCommentsOnSamePost;

            set => SetProperty(ref _isChkAllowMultipleCommentsOnSamePost, value);
        }

        [ProtoMember(6)]
        public ObservableCollection<ManageCommentModel> LstDisplayManageCommentModel { get; set; } =
            new ObservableCollection<ManageCommentModel>();

        public ManageCommentModel ManageCommentModel { get; set; } = new ManageCommentModel();

        /// <summary>
        ///     To hold all destination list which holds all group,page count both selected and total
        /// </summary>
        [ProtoMember(13)]
        public ObservableCollection<ChannelDestinationSelectModel> ListSelectDestination
        {
            get => _listSelectDestination;
            set
            {
                if (_listSelectDestination == value) return;
                SetProperty(ref _listSelectDestination, value);
            }
        }

        [ProtoMember(14)]
        public List<string> ListOfChannels
        {
            get => _listOfChannels;
            set
            {
                if (_listOfChannels == value) return;
                SetProperty(ref _listOfChannels, value);
            }
        }

        [ProtoMember(15)]
        public bool IsPostUniqueCommentFromEachAccount
        {
            get => _isPostUniqueCommentFromEachAccount;

            set
            {
                if (value == _isPostUniqueCommentFromEachAccount)
                    return;
                SetProperty(ref _isPostUniqueCommentFromEachAccount, value);
            }
        }

        [ProtoMember(16)]
        public bool IsCommentOnceFromEachAccount
        {
            get => _isCommentOnceFromEachAccount;

            set
            {
                if (value == _isCommentOnceFromEachAccount)
                    return;
                SetProperty(ref _isCommentOnceFromEachAccount, value);
            }
        }

        [ProtoMember(17)]
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

        [ProtoMember(18)]
        public bool CommentAsReplyOnlyChecked
        {
            get => _commentAsReplyOnlyChecked;
            set
            {
                SetProperty(ref _commentAsReplyOnlyChecked, value);
                if (value && ReplyToComments)
                    ReplyToComments = false;
            }
        }

        /// <summary>
        ///     Use this as the string text/word to reply the comments containing
        /// </summary>
        [ProtoMember(19)]
        public string CommentIdForReply
        {
            get => _commentIdForReply;
            set => SetProperty(ref _commentIdForReply, value);
        }

        /// <summary>
        ///     Use this as a condition to reply the comments containing any specific given string
        /// </summary>
        [ProtoMember(20)]
        public bool IsCheckFixCommentCountForPost
        {
            get => _isCheckFixCommentCountForPost;
            set => SetProperty(ref _isCheckFixCommentCountForPost, value);
        }

        [ProtoMember(21)]
        public RangeUtilities NumberOfCommentPostPerQuery
        {
            get => _numberOfCommentPostPerQuery;
            set => SetProperty(ref _numberOfCommentPostPerQuery, value);
        }

        [ProtoMember(22)]
        public bool IsCheckSkipLimitation
        {
            get => _IsCheckSkipLimitation;
            set => SetProperty(ref _IsCheckSkipLimitation, value);
        }

        [ProtoMember(23)]
        public RangeUtilities NumberOfSkippingUrl
        {
            get => _numberOfSkippingUrl;
            set => SetProperty(ref _numberOfSkippingUrl, value);
        }

        [ProtoMember(24)]
        public bool IsCheckAfterCommentAction
        {
            get => _isCheckAfterCommentAction;
            set => SetProperty(ref _isCheckAfterCommentAction, value);
        }

        [ProtoMember(25)]
        public bool IsCheckLikePostAfterComment
        {
            get => _isCheckLikePostAfterComment;
            set => SetProperty(ref _isCheckLikePostAfterComment, value);
        }

        [ProtoMember(26)]
        public int MultipleActionCount
        {
            get => _multipleActionCount;

            set => SetProperty(ref _multipleActionCount, value);
        }

        [ProtoMember(27)]
        public bool ReplyToComments
        {
            get => _replyToComments;
            set
            {
                SetProperty(ref _replyToComments, value);
                if (value)
                {
                    if (CommentAsReplyOnlyChecked)
                        CommentAsReplyOnlyChecked = false;
                }
                else
                {
                    if (IsCheckReplyNCommentsOnly)
                        IsCheckReplyNCommentsOnly = false;
                    if (IsCheckFixCommentCountForPost)
                        IsCheckFixCommentCountForPost = false;
                }
            }
        }

        [ProtoMember(28)]
        public bool IsCheckReplyNCommentsOnly
        {
            get => _isCheckReplyNCommentsOnly;
            set
            {
                SetProperty(ref _isCheckReplyNCommentsOnly, value);
                if (value && IsChkCommentUnique)
                    IsChkCommentUnique = false;
            }
        }

        [ProtoMember(29)]
        public RangeUtilities ReplyNComments
        {
            get => _replyNComments;
            set => SetProperty(ref _replyNComments, value);
        }
    }
}