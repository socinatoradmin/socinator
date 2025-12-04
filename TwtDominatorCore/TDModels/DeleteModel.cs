using System;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace TwtDominatorCore.TDModels
{
    [ProtoContract]
    public class DeleteSetting : BindableBase
    {
        private RangeUtilities _deleteCommentRange = new RangeUtilities {StartValue = 1, EndValue = 1};
        private RangeUtilities _deleteTweetRange = new RangeUtilities {StartValue = 1, EndValue = 1};

        private DateTime? _endDateForComment;

        private DateTime? _endDateForRetweet;

        private DateTime? _endDateForTeeet;

        private bool _isChkCommentedDateMustBeInSpecificRange;

        private bool _isChkDeleteComment;
        private bool _isChkDeleteTweet;

        private bool _isChkRetweetedDateMustBeInSpecificRange;

        private bool _isChkTweetedDateMustBeInSpecificRange;


        private bool _isChkUndoRetweet;

        private bool _IsDeleteRandomComments;

        private bool _IsDeleteRandomTweets;

        private bool _IsUndoRandomRetweets;


        private DateTime? _startDateForComment;

        private DateTime? _startDateForRetweet;

        private DateTime? _startDateForTeeet;
        private RangeUtilities _undoRetweetRange = new RangeUtilities {StartValue = 1, EndValue = 1};

        [ProtoMember(1)]
        public bool IsChkDeleteTweet
        {
            get => _isChkDeleteTweet;
            set
            {
                if (_isChkDeleteTweet == value)
                    return;
                SetProperty(ref _isChkDeleteTweet, value);
            }
        }

        [ProtoMember(2)]
        public RangeUtilities DeleteTweetRange
        {
            get => _deleteTweetRange;
            set
            {
                if (_deleteTweetRange == value)
                    return;
                SetProperty(ref _deleteTweetRange, value);
            }
        }

        [ProtoMember(3)]
        public bool IsChkTweetedDateMustBeInSpecificRange
        {
            get => _isChkTweetedDateMustBeInSpecificRange;
            set
            {
                if (_isChkTweetedDateMustBeInSpecificRange == value)
                    return;
                SetProperty(ref _isChkTweetedDateMustBeInSpecificRange, value);
            }
        }

        [ProtoMember(4)]
        public DateTime? StartDateForTweet
        {
            get => _startDateForTeeet;
            set
            {
                if (_startDateForTeeet == value)
                    return;
                SetProperty(ref _startDateForTeeet, value);
            }
        }

        [ProtoMember(5)]
        public DateTime? EndDateForTweet
        {
            get => _endDateForTeeet;
            set
            {
                if (_endDateForTeeet == value)
                    return;
                SetProperty(ref _endDateForTeeet, value);
            }
        }

        [ProtoMember(6)]
        public bool IsChkDeleteComment
        {
            get => _isChkDeleteComment;
            set
            {
                if (_isChkDeleteComment == value)
                    return;
                SetProperty(ref _isChkDeleteComment, value);
            }
        }

        [ProtoMember(7)]
        public RangeUtilities DeleteCommentRange
        {
            get => _deleteCommentRange;
            set
            {
                if (_deleteCommentRange == value)
                    return;
                SetProperty(ref _deleteCommentRange, value);
            }
        }

        [ProtoMember(8)]
        public bool IsChkCommentedDateMustBeInSpecificRange
        {
            get => _isChkCommentedDateMustBeInSpecificRange;
            set
            {
                if (_isChkCommentedDateMustBeInSpecificRange == value)
                    return;
                SetProperty(ref _isChkCommentedDateMustBeInSpecificRange, value);
            }
        }

        [ProtoMember(9)]
        public DateTime? StartDateForComment
        {
            get => _startDateForComment;
            set
            {
                if (_startDateForComment == value)
                    return;
                SetProperty(ref _startDateForComment, value);
            }
        }

        [ProtoMember(10)]
        public DateTime? EndDateForComment
        {
            get => _endDateForComment;
            set
            {
                if (_endDateForComment == value)
                    return;
                SetProperty(ref _endDateForComment, value);
            }
        }

        [ProtoMember(11)]
        public bool IsChkUndoRetweet
        {
            get => _isChkUndoRetweet;
            set
            {
                if (_isChkUndoRetweet == value)
                    return;
                SetProperty(ref _isChkUndoRetweet, value);
            }
        }

        [ProtoMember(12)]
        public RangeUtilities UndoRetweetRange
        {
            get => _undoRetweetRange;
            set
            {
                if (_undoRetweetRange == value)
                    return;
                SetProperty(ref _undoRetweetRange, value);
            }
        }

        [ProtoMember(13)]
        public bool IsChkRetweetedDateMustBeInSpecificRange
        {
            get => _isChkRetweetedDateMustBeInSpecificRange;
            set
            {
                if (_isChkRetweetedDateMustBeInSpecificRange == value)
                    return;
                SetProperty(ref _isChkRetweetedDateMustBeInSpecificRange, value);
            }
        }

        [ProtoMember(14)]
        public DateTime? StartDateForRetweet
        {
            get => _startDateForRetweet;
            set
            {
                if (_startDateForRetweet == value)
                    return;
                SetProperty(ref _startDateForRetweet, value);
            }
        }

        [ProtoMember(15)]
        public DateTime? EndDateForRetweet
        {
            get => _endDateForRetweet;
            set
            {
                if (_endDateForRetweet == value)
                    return;
                SetProperty(ref _endDateForRetweet, value);
            }
        }

        [ProtoMember(16)]
        public bool IsDeleteRandomTweets
        {
            get => _IsDeleteRandomTweets;
            set => SetProperty(ref _IsDeleteRandomTweets, value);
        }

        [ProtoMember(17)]
        public bool IsUndoRandomRetweets
        {
            get => _IsUndoRandomRetweets;
            set => SetProperty(ref _IsUndoRandomRetweets, value);
        }

        [ProtoMember(18)]
        public bool IsDeleteRandomComments
        {
            get => _IsDeleteRandomComments;
            set => SetProperty(ref _IsDeleteRandomComments, value);
        }
    }


    public class DeleteModel : ModuleSetting, IGeneralSettings
    {
        [ProtoMember(1)] public TweetFilterModel TweetFilterModel { get; set; } = new TweetFilterModel();

        [ProtoMember(2)] JobConfiguration IGeneralSettings.JobConfiguration { get; set; }


        #region BlackList and Whitelist

        private bool _IsSkipWhiteListUsers;

        [ProtoMember(4)]
        public bool IsSkipWhiteListUsers
        {
            get => _IsSkipWhiteListUsers;
            set => SetProperty(ref _IsSkipWhiteListUsers, value);
        }

        private bool _IsUsePrivateWhiteList;

        [ProtoMember(5)]
        public bool IsUsePrivateWhiteList
        {
            get => _IsUsePrivateWhiteList;
            set => SetProperty(ref _IsUsePrivateWhiteList, value);
        }

        private bool _IsUseGroupWhiteList;

        [ProtoMember(6)]
        public bool IsUseGroupWhiteList
        {
            get => _IsUseGroupWhiteList;
            set => SetProperty(ref _IsUseGroupWhiteList, value);
        }

        private bool _IsAddToBlackListOnceUnfollowed;

        [ProtoMember(7)]
        public bool IsAddToBlackListOnceUnfollowed
        {
            get => _IsAddToBlackListOnceUnfollowed;
            set => SetProperty(ref _IsAddToBlackListOnceUnfollowed, value);
        }

        private bool _IsAddToPrivateBlackList;

        [ProtoMember(8)]
        public bool IsAddToPrivateBlackList
        {
            get => _IsAddToPrivateBlackList;
            set => SetProperty(ref _IsAddToPrivateBlackList, value);
        }

        private bool _IsAddToPublicBlackList;

        [ProtoMember(9)]
        public bool IsAddToPublicBlackList
        {
            get => _IsAddToPublicBlackList;
            set => SetProperty(ref _IsAddToPublicBlackList, value);
        }

        #endregion

        #region Manage Speed 

        /// <summary>
        ///     Slow week 150
        ///     Medium week 300
        ///     Fast week 450
        ///     SuperFast week 600
        /// </summary>
        public JobConfiguration SlowSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(15, 20),
            ActivitiesPerHour = new RangeUtilities(4, 6),
            ActivitiesPerWeek = new RangeUtilities(120, 150),
            ActivitiesPerJob = new RangeUtilities(2, 3),
            DelayBetweenJobs = new RangeUtilities(20, 30),
            DelayBetweenActivity = new RangeUtilities(40, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(30, 45),
            ActivitiesPerHour = new RangeUtilities(8, 12),
            ActivitiesPerWeek = new RangeUtilities(250, 300),
            ActivitiesPerJob = new RangeUtilities(3, 4),
            DelayBetweenJobs = new RangeUtilities(50, 80),
            DelayBetweenActivity = new RangeUtilities(40, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration FastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(60, 70),
            ActivitiesPerHour = new RangeUtilities(10, 15),
            ActivitiesPerWeek = new RangeUtilities(400, 450),
            ActivitiesPerJob = new RangeUtilities(6, 8),
            DelayBetweenJobs = new RangeUtilities(100, 150),
            DelayBetweenActivity = new RangeUtilities(40, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(80, 90),
            ActivitiesPerHour = new RangeUtilities(18, 25),
            ActivitiesPerWeek = new RangeUtilities(500, 600),
            ActivitiesPerJob = new RangeUtilities(10, 15),
            DelayBetweenJobs = new RangeUtilities(180, 220),
            DelayBetweenActivity = new RangeUtilities(40, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        #endregion
    }
}