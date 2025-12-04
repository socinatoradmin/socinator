using System.Collections.Generic;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace TwtDominatorCore.TDModels
{
    [ProtoContract]
    public class RetweetModel : ModuleSetting, IGeneralSettings
    {
        private RangeUtilities _increaseEachDayFollow = new RangeUtilities {StartValue = 1, EndValue = 1};

        private bool _isChkEnableLikeComments;
        private bool _IsChkLikeTweet;
        private bool _IsSpintax;
        private RangeUtilities _likeCommentRange = new RangeUtilities {StartValue = 1, EndValue = 1};

        public List<string> ListQueryType { get; set; } = new List<string>();


        [ProtoMember(1)] public override UserFilterModel UserFilterModel { get; set; } = new UserFilterModel();


        [ProtoMember(2)] public override TweetFilterModel TweetFilterModel { get; set; } = new TweetFilterModel();


        [ProtoMember(3)] JobConfiguration IGeneralSettings.JobConfiguration { get; set; }


        #region After Retweet Action

        [ProtoMember(4)]
        public bool IsChkEnableLikeComments
        {
            get => _isChkEnableLikeComments;
            set
            {
                if (_isChkEnableLikeComments == value)
                    return;
                SetProperty(ref _isChkEnableLikeComments, value);
            }
        }


        [ProtoMember(5)]
        public RangeUtilities LikeCommentRange
        {
            get => _likeCommentRange;
            set
            {
                if (_likeCommentRange == value)
                    return;
                SetProperty(ref _likeCommentRange, value);
            }
        }

        private RangeUtilities _DelayBetweenLikeTweetRange = new RangeUtilities {StartValue = 20, EndValue = 50};

        [ProtoMember(25)]
        public RangeUtilities DelayBetweenLikeTweetRange
        {
            get => _DelayBetweenLikeTweetRange;
            set
            {
                if (_DelayBetweenLikeTweetRange == value)
                    return;
                SetProperty(ref _DelayBetweenLikeTweetRange, value);
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

        private bool _isChkLikeRandomComment;

        [ProtoMember(10)]
        public bool IsChkLikeRandomComment
        {
            get => _isChkLikeRandomComment;
            set
            {
                if (value == _isChkLikeRandomComment)
                    return;
                SetProperty(ref _isChkLikeRandomComment, value);
            }
        }


        private bool _isChkEnableRetweetComments;

        [ProtoMember(11)]
        public bool IsChkEnableRetweetComments
        {
            get => _isChkEnableRetweetComments;
            set
            {
                if (_isChkEnableRetweetComments == value)
                    return;
                SetProperty(ref _isChkEnableRetweetComments, value);
            }
        }

        private RangeUtilities _RetweetCommentRange = new RangeUtilities {StartValue = 1, EndValue = 1};

        [ProtoMember(12)]
        public RangeUtilities RetweetCommentRange
        {
            get => _RetweetCommentRange;
            set
            {
                if (_RetweetCommentRange == value)
                    return;
                SetProperty(ref _RetweetCommentRange, value);
            }
        }

        private RangeUtilities _DelayBetweenRetweetRange = new RangeUtilities {StartValue = 20, EndValue = 50};

        [ProtoMember(26)]
        public RangeUtilities DelayBetweenRetweetRange
        {
            get => _DelayBetweenRetweetRange;
            set
            {
                if (_DelayBetweenRetweetRange == value)
                    return;
                SetProperty(ref _DelayBetweenRetweetRange, value);
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

        private bool _isChkRetweetRandomComment;

        [ProtoMember(17)]
        public bool IsChkRetweetRandomComment
        {
            get => _isChkRetweetRandomComment;
            set
            {
                if (value == _isChkRetweetRandomComment)
                    return;
                SetProperty(ref _isChkRetweetRandomComment, value);
            }
        }

        private bool _isChkUploadComments;

        [ProtoMember(18)]
        public bool IsChkUploadComments
        {
            get => _isChkUploadComments;
            set
            {
                if (_isChkUploadComments == value)
                    return;
                SetProperty(ref _isChkUploadComments, value);
            }
        }

        private string _uploadedCommentInput;

        [ProtoMember(19)]
        public string UploadedCommentInput
        {
            get => _uploadedCommentInput;
            set
            {
                if (_uploadedCommentInput == value)
                    return;
                SetProperty(ref _uploadedCommentInput, value);
            }
        }

        private List<string> _lstUploadedComment = new List<string>();

        [ProtoMember(20)]
        public List<string> LstUploadedComment
        {
            get => _lstUploadedComment;
            set
            {
                if (_lstUploadedComment == value)
                    return;
                SetProperty(ref _lstUploadedComment, value);
            }
        }

        private bool _isChkMaxComment;

        [ProtoMember(21)]
        public bool IsChkMaxComment
        {
            get => _isChkMaxComment;
            set
            {
                if (value == _isChkMaxComment)
                    return;
                SetProperty(ref _isChkMaxComment, value);
            }
        }

        private RangeUtilities _commentMaxRange = new RangeUtilities {StartValue = 1, EndValue = 1};

        [ProtoMember(22)]
        public RangeUtilities CommentMaxRange
        {
            get => _commentMaxRange;
            set
            {
                if (value == _commentMaxRange)
                    return;
                SetProperty(ref _commentMaxRange, value);
            }
        }

        private RangeUtilities _DelayBetweenCommentRange = new RangeUtilities {StartValue = 20, EndValue = 50};

        [ProtoMember(27)]
        public RangeUtilities DelayBetweenCommentRange
        {
            get => _DelayBetweenCommentRange;
            set
            {
                if (value == _DelayBetweenCommentRange)
                    return;
                SetProperty(ref _DelayBetweenCommentRange, value);
            }
        }

        private bool _isChkIncreaseEachDayComment;

        [ProtoMember(23)]
        public bool IsChkIncreaseEachDayComment
        {
            get => _isChkIncreaseEachDayComment;
            set
            {
                if (value == _isChkIncreaseEachDayComment)
                    return;
                SetProperty(ref _isChkIncreaseEachDayComment, value);
            }
        }

        private RangeUtilities _increaseEachDayComment = new RangeUtilities {StartValue = 1, EndValue = 1};

        [ProtoMember(24)]
        public RangeUtilities IncreaseEachDayComment
        {
            get => _increaseEachDayComment;
            set
            {
                if (value == _increaseEachDayComment)
                    return;
                SetProperty(ref _increaseEachDayComment, value);
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


        [ProtoMember(28)]
        public bool IsChkLikeTweet
        {
            get => _IsChkLikeTweet;
            set => SetProperty(ref _IsChkLikeTweet, value);
        }

        [ProtoMember(29)]
        public bool IsSpintax
        {
            get => _IsSpintax;
            set => SetProperty(ref _IsSpintax, value);
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

        private bool _isChkQuoteTweet;

        [ProtoMember(30)]
        public bool IsChkQuoteTweet
        {
            get => _isChkQuoteTweet;
            set
            {
                if (value == _isChkQuoteTweet)
                    return;
                SetProperty(ref _isChkQuoteTweet, value);
            }
        }
        private string _UploadQuotesTweets;

        [ProtoMember(31)]
        public string UploadQuotesTweets
        {
            get => _UploadQuotesTweets;
            set
            {
                if (value == _UploadQuotesTweets)
                    return;
                SetProperty(ref _UploadQuotesTweets, value);
            }
        }
        private bool _isBookmarkTweet = false;
        [ProtoMember(32)]
        public bool IsBookmarkTweet
        {
            get => _isBookmarkTweet;
            set
            {
                if (_isBookmarkTweet == value)
                    return;
                SetProperty(ref _isBookmarkTweet, value);
            }
        }

    }
}