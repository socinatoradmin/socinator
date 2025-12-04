using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace TwtDominatorCore.TDModels
{
    [ProtoContract]
    public class CommentModel : ModuleSetting, IGeneralSettings
    {
        private bool _IsSpintax;
        private bool _multilineComment;
        [ProtoMember(37)] public bool IsMultilineComment
        {
            get => _multilineComment;
            set=>SetProperty(ref _multilineComment, value);
        }
        public List<string> ListQueryType { get; set; } = new List<string>();


        [ProtoMember(1)] public override UserFilterModel UserFilterModel { get; set; } = new UserFilterModel();


        [ProtoMember(2)] public override TweetFilterModel TweetFilterModel { get; set; } = new TweetFilterModel();

        [ProtoMember(4)]
        public ObservableCollection<ManageCommentModel> LstDisplayManageCommentModel { get; set; } =
            new ObservableCollection<ManageCommentModel>();

        public ManageCommentModel ManageCommentModel { get; set; } = new ManageCommentModel();


        [ProtoMember(27)]
        public ObservableCollection<ManageMessagesModel> LstDisplayManageMessagesModel { get; set; } =
            new ObservableCollection<ManageMessagesModel>();

        public ManageMessagesModel ManageMessagesModel { get; set; } = new ManageMessagesModel();

        [ProtoMember(31)]
        public bool IsSpintax
        {
            get => _IsSpintax;
            set => SetProperty(ref _IsSpintax, value);
        }


        [ProtoMember(3)] JobConfiguration IGeneralSettings.JobConfiguration { get; set; }

        #region After Comment Action

        private bool _isChkEnableLikeComment;

        [ProtoMember(4)]
        public bool IsChkEnableLikeComment
        {
            get => _isChkEnableLikeComment;
            set
            {
                if (_isChkEnableLikeComment == value)
                    return;
                SetProperty(ref _isChkEnableLikeComment, value);
            }
        }

        private bool _isChkMaxLike;

        [ProtoMember(6)]
        public bool IsChkMaxLike
        {
            get => _isChkMaxLike;
            set
            {
                if (value == _isChkMaxLike)
                    return;
                SetProperty(ref _isChkMaxLike, value);
            }
        }

        private RangeUtilities _likeMaxRange = new RangeUtilities {StartValue = 1, EndValue = 1};

        [ProtoMember(7)]
        public RangeUtilities LikeMaxRange
        {
            get => _likeMaxRange;
            set
            {
                if (value == _likeMaxRange)
                    return;
                SetProperty(ref _likeMaxRange, value);
            }
        }

        private RangeUtilities _DelayBetweenLikeTweetRange = new RangeUtilities {StartValue = 20, EndValue = 50};

        [ProtoMember(25)]
        public RangeUtilities DelayBetweenLikeTweetRange
        {
            get => _DelayBetweenLikeTweetRange;
            set => SetProperty(ref _DelayBetweenLikeTweetRange, value);
        }

        private bool _isChkIncreaseEachDayLike;

        [ProtoMember(8)]
        public bool IsChkIncreaseEachDayLike
        {
            get => _isChkIncreaseEachDayLike;
            set
            {
                if (value == _isChkIncreaseEachDayLike)
                    return;
                SetProperty(ref _isChkIncreaseEachDayLike, value);
            }
        }

        private RangeUtilities _increaseEachDayLike = new RangeUtilities {StartValue = 1, EndValue = 1};

        [ProtoMember(9)]
        public RangeUtilities IncreaseEachDayLike
        {
            get => _increaseEachDayLike;
            set
            {
                if (value == _increaseEachDayLike)
                    return;
                SetProperty(ref _increaseEachDayLike, value);
            }
        }

        private bool _isChkEnableRetweetComment;

        [ProtoMember(11)]
        public bool IsChkEnableRetweetComment
        {
            get => _isChkEnableRetweetComment;
            set
            {
                if (_isChkEnableRetweetComment == value)
                    return;
                SetProperty(ref _isChkEnableRetweetComment, value);
            }
        }

        private bool _isChkCommentOnTweetsFromUserNotifications;

        [ProtoMember(12)]
        public bool IsChkCommentOnTweetsOnUserNotifications
        {
            get => _isChkCommentOnTweetsFromUserNotifications;
            set
            {
                if (_isChkCommentOnTweetsFromUserNotifications == value)
                    return;
                SetProperty(ref _isChkCommentOnTweetsFromUserNotifications, value);
            }
        }

        private string _commentTextToCommentOnTweetsFromUserNotifications;

        [ProtoMember(12)]
        public string CommentTextToCommentOnTweetsFromUserNotifications
        {
            get => _commentTextToCommentOnTweetsFromUserNotifications;
            set
            {
                if (_commentTextToCommentOnTweetsFromUserNotifications == value)
                    return;
                SetProperty(ref _commentTextToCommentOnTweetsFromUserNotifications, value);
            }
        }

        private bool _isChkMaxRetweet;

        [ProtoMember(13)]
        public bool IsChkMaxRetweet
        {
            get => _isChkMaxRetweet;
            set
            {
                if (value == _isChkMaxRetweet)
                    return;
                SetProperty(ref _isChkMaxRetweet, value);
            }
        }

        private RangeUtilities _retweetMaxRange = new RangeUtilities {StartValue = 1, EndValue = 1};

        [ProtoMember(14)]
        public RangeUtilities RetweetMaxRange
        {
            get => _retweetMaxRange;
            set
            {
                if (value == _retweetMaxRange)
                    return;
                SetProperty(ref _retweetMaxRange, value);
            }
        }


        private RangeUtilities _DelayBetweenRetweetRange = new RangeUtilities {StartValue = 20, EndValue = 50};

        [ProtoMember(26)]
        public RangeUtilities DelayBetweenRetweetRange
        {
            get => _DelayBetweenRetweetRange;
            set => SetProperty(ref _DelayBetweenRetweetRange, value);
        }

        private bool _isChkIncreaseEachDayRetweet;

        [ProtoMember(15)]
        public bool IsChkIncreaseEachDayRetweet
        {
            get => _isChkIncreaseEachDayRetweet;
            set
            {
                if (value == _isChkIncreaseEachDayRetweet)
                    return;
                SetProperty(ref _isChkIncreaseEachDayRetweet, value);
            }
        }

        private RangeUtilities _increaseEachDayRetweet = new RangeUtilities {StartValue = 1, EndValue = 1};

        [ProtoMember(16)]
        public RangeUtilities IncreaseEachDayRetweet
        {
            get => _increaseEachDayRetweet;
            set
            {
                if (value == _increaseEachDayRetweet)
                    return;
                SetProperty(ref _increaseEachDayRetweet, value);
            }
        }

        private bool _isChkEnableLikeOthersComment;

        [ProtoMember(4)]
        public bool IsChkEnableLikeOthersComment
        {
            get => _isChkEnableLikeOthersComment;
            set
            {
                if (_isChkEnableLikeOthersComment == value)
                    return;
                SetProperty(ref _isChkEnableLikeOthersComment, value);
            }
        }

        private RangeUtilities _likeOthersCommentRange = new RangeUtilities {StartValue = 1, EndValue = 1};

        [ProtoMember(4)]
        public RangeUtilities LikeOthersCommentRange
        {
            get => _likeOthersCommentRange;
            set
            {
                if (value == _likeOthersCommentRange)
                    return;
                SetProperty(ref _likeOthersCommentRange, value);
            }
        }

        private RangeUtilities _DelayBetweenLikeOthersCommentRange =
            new RangeUtilities {StartValue = 20, EndValue = 50};

        [ProtoMember(27)]
        public RangeUtilities DelayBetweenLikeOthersCommentRange
        {
            get => _DelayBetweenLikeOthersCommentRange;
            set
            {
                if (value == _DelayBetweenLikeOthersCommentRange)
                    return;
                SetProperty(ref _DelayBetweenLikeOthersCommentRange, value);
            }
        }

        private bool _isChkMaxOthersCommentLike;

        [ProtoMember(6)]
        public bool IsChkMaxOthersCommentLike
        {
            get => _isChkMaxOthersCommentLike;
            set
            {
                if (value == _isChkMaxOthersCommentLike)
                    return;
                SetProperty(ref _isChkMaxOthersCommentLike, value);
            }
        }

        private RangeUtilities _likeMaxOthersCommentRange = new RangeUtilities {StartValue = 1, EndValue = 1};

        [ProtoMember(7)]
        public RangeUtilities LikeMaxOthersCommentRange
        {
            get => _likeMaxOthersCommentRange;
            set
            {
                if (value == _likeMaxOthersCommentRange)
                    return;
                SetProperty(ref _likeMaxOthersCommentRange, value);
            }
        }

        private bool _isChkIncreaseEachDayOthersCommentLike;

        [ProtoMember(8)]
        public bool IsChkIncreaseEachDayOthersCommentLike
        {
            get => _isChkIncreaseEachDayOthersCommentLike;
            set
            {
                if (value == _isChkIncreaseEachDayOthersCommentLike)
                    return;
                SetProperty(ref _isChkIncreaseEachDayOthersCommentLike, value);
            }
        }

        private RangeUtilities _increaseEachDayOthersCommentLike = new RangeUtilities {StartValue = 1, EndValue = 1};

        [ProtoMember(9)]
        public RangeUtilities IncreaseEachDayOthersCommentLike
        {
            get => _increaseEachDayOthersCommentLike;
            set
            {
                if (value == _increaseEachDayOthersCommentLike)
                    return;
                SetProperty(ref _increaseEachDayOthersCommentLike, value);
            }
        }


        private bool _isChkFollowTweetOwner;

        [ProtoMember(18)]
        public bool IsChkFollowTWeetOwner
        {
            get => _isChkFollowTweetOwner;
            set
            {
                if (_isChkFollowTweetOwner == value)
                    return;
                SetProperty(ref _isChkFollowTweetOwner, value);
            }
        }

        private bool _isChkOnlyFollowFollowers;

        public bool IsChkOnlyFollowFollowers
        {
            get => _isChkOnlyFollowFollowers;
            set
            {
                if (_isChkOnlyFollowFollowers == value)
                    return;
                SetProperty(ref _isChkOnlyFollowFollowers, value);
            }
        }

        private bool _isChkMaxFollow;

        [ProtoMember(21)]
        public bool IsChkMaxFollow
        {
            get => _isChkMaxFollow;
            set
            {
                if (value == _isChkMaxFollow)
                    return;
                SetProperty(ref _isChkMaxFollow, value);
            }
        }

        private RangeUtilities _followtMaxRange = new RangeUtilities {StartValue = 1, EndValue = 1};

        [ProtoMember(22)]
        public RangeUtilities FollowMaxRange
        {
            get => _followtMaxRange;
            set
            {
                if (value == _followtMaxRange)
                    return;
                SetProperty(ref _followtMaxRange, value);
            }
        }

        private bool _isChkMaxCommentOnTweetsFromUserNotifications;

        [ProtoMember(34)]
        public bool IsChkMaxCommentOnTweetsFromUserNotifications
        {
            get => _isChkMaxCommentOnTweetsFromUserNotifications;
            set
            {
                if (value == _isChkMaxCommentOnTweetsFromUserNotifications)
                    return;
                SetProperty(ref _isChkMaxCommentOnTweetsFromUserNotifications, value);
            }
        }

        private RangeUtilities _commentOnTweetsFromUserNotificationsMaxRange = new RangeUtilities { StartValue = 1, EndValue = 1 };

        [ProtoMember(35)]
        public RangeUtilities CommentOnTweetsFromUserNotificationsMaxRange
        {
            get => _commentOnTweetsFromUserNotificationsMaxRange;
            set
            {
                if (value == _commentOnTweetsFromUserNotificationsMaxRange)
                    return;
                SetProperty(ref _commentOnTweetsFromUserNotificationsMaxRange, value);
            }
        }

        private RangeUtilities _delayBetweenCommentOnTweetsFromUserNotifications = new RangeUtilities { StartValue = 1, EndValue = 1 };

        [ProtoMember(36)]
        public RangeUtilities DelayBetweenCommentOnTweetsFromUserNotifications
        {
            get => _delayBetweenCommentOnTweetsFromUserNotifications;
            set
            {
                if (value == _delayBetweenCommentOnTweetsFromUserNotifications)
                    return;
                SetProperty(ref _delayBetweenCommentOnTweetsFromUserNotifications, value);
            }
        }

        private bool _isChkIncreaseEachDayFollow;

        [ProtoMember(23)]
        public bool IsChkIncreaseEachDayFollow
        {
            get => _isChkIncreaseEachDayFollow;
            set
            {
                if (value == _isChkIncreaseEachDayFollow)
                    return;
                SetProperty(ref _isChkIncreaseEachDayFollow, value);
            }
        }

        private RangeUtilities _increaseEachDayFollow = new RangeUtilities {StartValue = 1, EndValue = 1};

        [ProtoMember(24)]
        public RangeUtilities IncreaseEachDayFollow
        {
            get => _increaseEachDayFollow;
            set
            {
                if (value == _increaseEachDayFollow)
                    return;
                SetProperty(ref _increaseEachDayFollow, value);
            }
        }


        //DelayBetweenLikeRange

        #endregion


        #region Other Configuration

        //IsUniqueComment

        private bool _IsUniqueComment;

        [ProtoMember(28)]
        public bool IsUniqueComment
        {
            get => _IsUniqueComment;
            set => SetProperty(ref _IsUniqueComment, value);
        }

        private bool _IsChkPostUniqueCommentOnPostFromEachAccount = true;

        [ProtoMember(29)]
        public bool IsChkPostUniqueCommentOnPostFromEachAccount
        {
            get => _IsChkPostUniqueCommentOnPostFromEachAccount;
            set => SetProperty(ref _IsChkPostUniqueCommentOnPostFromEachAccount, value);
        }

        private bool _IsChkCommentOnceFromEachAccount;

        [ProtoMember(30)]
        public bool IsChkCommentOnceFromEachAccount
        {
            get => _IsChkCommentOnceFromEachAccount;
            set => SetProperty(ref _IsChkCommentOnceFromEachAccount, value);
        }

        private bool _IsAllowMultipleCommentOnSamePost;

        [ProtoMember(32)]
        public bool IsAllowMultipleCommentOnSamePost
        {
            get => _IsAllowMultipleCommentOnSamePost;
            set => SetProperty(ref _IsAllowMultipleCommentOnSamePost, value);
        }

        private RangeUtilities _NoOfMultipleCommentOnSamePost = new RangeUtilities(2, 5);

        [ProtoMember(33)]
        public RangeUtilities NoOfMultipleCommentOnSamePost
        {
            get => _NoOfMultipleCommentOnSamePost;
            set => SetProperty(ref _NoOfMultipleCommentOnSamePost, value);
        }

        #endregion


        #region Manage Speed 

        /// <summary>
        ///     Slow week 200
        ///     Medium week 400
        ///     Fast week 600
        ///     SuperFast week 800
        /// </summary>
        public JobConfiguration SlowSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(20, 25),
            ActivitiesPerHour = new RangeUtilities(4, 6),
            ActivitiesPerWeek = new RangeUtilities(150, 200),
            ActivitiesPerJob = new RangeUtilities(2, 3),
            DelayBetweenJobs = new RangeUtilities(20, 30),
            DelayBetweenActivity = new RangeUtilities(40, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(40, 60),
            ActivitiesPerHour = new RangeUtilities(8, 12),
            ActivitiesPerWeek = new RangeUtilities(300, 400),
            ActivitiesPerJob = new RangeUtilities(4, 6),
            DelayBetweenJobs = new RangeUtilities(50, 80),
            DelayBetweenActivity = new RangeUtilities(40, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration FastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(60, 90),
            ActivitiesPerHour = new RangeUtilities(10, 15),
            ActivitiesPerWeek = new RangeUtilities(500, 600),
            ActivitiesPerJob = new RangeUtilities(6, 8),
            DelayBetweenJobs = new RangeUtilities(100, 150),
            DelayBetweenActivity = new RangeUtilities(40, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(95, 120),
            ActivitiesPerHour = new RangeUtilities(18, 25),
            ActivitiesPerWeek = new RangeUtilities(600, 800),
            ActivitiesPerJob = new RangeUtilities(10, 15),
            DelayBetweenJobs = new RangeUtilities(180, 220),
            DelayBetweenActivity = new RangeUtilities(40, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        #endregion
    }
}