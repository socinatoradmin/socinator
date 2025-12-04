using System.Collections.Generic;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace TwtDominatorCore.TDModels
{
    [ProtoContract]
    public class AfterFollowActionModel : BindableBase
    {
        private string _commentOnUsersTweetInput;
        private int _commentPercentage;
        private RangeUtilities _commentsPerDayMaxRange = new RangeUtilities {StartValue = 1, EndValue = 1};
        private RangeUtilities _commentsPerUser = new RangeUtilities {StartValue = 1, EndValue = 1};

        private RangeUtilities _DelayBetweenCommentRange = new RangeUtilities {StartValue = 20, EndValue = 50};
        private RangeUtilities _delayBetweenLikeRange = new RangeUtilities {StartValue = 20, EndValue = 50};

        private RangeUtilities _DelayBetweenRetweetRange = new RangeUtilities {StartValue = 20, EndValue = 50};
        private RangeUtilities _increaseEachDayComment = new RangeUtilities {StartValue = 1, EndValue = 1};
        private RangeUtilities _increaseEachDayLike = new RangeUtilities {StartValue = 1, EndValue = 1};
        private RangeUtilities _increaseEachDayRetweet = new RangeUtilities {StartValue = 1, EndValue = 1};
        private RangeUtilities _IncreaseEachDayTweet = new RangeUtilities {StartValue = 1, EndValue = 1};
        private bool _isCheckLikeRandomTweets;
        private bool _isCheckTurnOnUserNotifications;
        private bool _isCheckMuteUser;
        private bool _isCheckRetweetRandomTweets;
        private bool _isChkCommentButSkipRetweets;


        private bool _isChkCommentOnTweets;
        private bool _isChkCommentPercentage;
        private bool _isChkIncreaseEachDayComment;
        private bool _isChkIncreaseEachDayLike;
        private bool _isChkIncreaseEachDayRetweet;
        private bool _isChkIncreaseEachDayTweet;
        private bool _isChkLikeButSkipRetweets;
        private bool _isChkLikeTweets;

        private bool _isChkMaxComment;
        private bool _isChkMaxLike;

        private bool _isChkMaxRetweet;
        private bool _isChkMaxTweet;
        private bool _isChkRetweetTweetButSkipRetweets;


        private bool _isChkRetweetTweets;
        private bool _isChkTweetPercentage;


        private bool _isChkTweetToUser;
        private bool _IsSpintax;
        private RangeUtilities _likeMaxRange = new RangeUtilities {StartValue = 1, EndValue = 1};
        private RangeUtilities _likeTweetsRange = new RangeUtilities {StartValue = 1, EndValue = 1};
        private List<string> _lstCommentOnUsersTweetInput = new List<string>();
        private List<string> _lstTweetToUserInput = new List<string>();
        private RangeUtilities _RetweetMaxRange = new RangeUtilities {StartValue = 1, EndValue = 1};
        private RangeUtilities _RetweetTweetsRange = new RangeUtilities {StartValue = 1, EndValue = 1};
        private RangeUtilities _tweetMaxRange = new RangeUtilities {StartValue = 1, EndValue = 1};
        private int _TweetPercentage;
        private string _tweetToUserInput;


        [ProtoMember(1)]
        public bool IsChkLikeTweets
        {
            get => _isChkLikeTweets;
            set
            {
                if (value == _isChkLikeTweets)
                    return;
                SetProperty(ref _isChkLikeTweets, value);
            }
        }

        [ProtoMember(2)]
        public RangeUtilities LikeTweetsRange
        {
            get => _likeTweetsRange;
            set
            {
                if (value == _likeTweetsRange)
                    return;
                SetProperty(ref _likeTweetsRange, value);
            }
        }


        [ProtoMember(36)]
        public RangeUtilities DelayBetweenLikeRange
        {
            get => _delayBetweenLikeRange;
            set
            {
                if (value == _delayBetweenLikeRange)
                    return;
                SetProperty(ref _delayBetweenLikeRange, value);
            }
        }


        [ProtoMember(3)]
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

        [ProtoMember(4)]
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

        [ProtoMember(5)]
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

        [ProtoMember(6)]
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

        [ProtoMember(7)]
        public bool IsCheckLikeRandomTweets
        {
            get => _isCheckLikeRandomTweets;
            set
            {
                if (value == _isCheckLikeRandomTweets)
                    return;
                SetProperty(ref _isCheckLikeRandomTweets, value);
            }
        }

        [ProtoMember(33)]
        public bool IsChkLikeButSkipRetweets
        {
            get => _isChkLikeButSkipRetweets;
            set
            {
                if (value == _isChkLikeButSkipRetweets)
                    return;
                SetProperty(ref _isChkLikeButSkipRetweets, value);
            }
        }

        [ProtoMember(8)]
        public bool IsChkRetweetTweets
        {
            get => _isChkRetweetTweets;
            set
            {
                if (value == _isChkRetweetTweets)
                    return;
                SetProperty(ref _isChkRetweetTweets, value);
            }
        }

        [ProtoMember(9)]
        public RangeUtilities RetweetTweetsRange
        {
            get => _RetweetTweetsRange;
            set
            {
                if (value == _RetweetTweetsRange)
                    return;
                SetProperty(ref _RetweetTweetsRange, value);
            }
        }

        [ProtoMember(37)]
        public RangeUtilities DelayBetweenRetweetRange
        {
            get => _DelayBetweenRetweetRange;
            set
            {
                if (value == _DelayBetweenRetweetRange)
                    return;
                SetProperty(ref _DelayBetweenRetweetRange, value);
            }
        }

        [ProtoMember(10)]
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

        [ProtoMember(11)]
        public RangeUtilities RetweetMaxRange
        {
            get => _RetweetMaxRange;
            set
            {
                if (value == _RetweetMaxRange)
                    return;
                SetProperty(ref _RetweetMaxRange, value);
            }
        }

        [ProtoMember(12)]
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

        [ProtoMember(13)]
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

        [ProtoMember(14)]
        public bool IsCheckRetweetRandomTweets
        {
            get => _isCheckRetweetRandomTweets;
            set
            {
                if (value == _isCheckRetweetRandomTweets)
                    return;
                SetProperty(ref _isCheckRetweetRandomTweets, value);
            }
        }

        [ProtoMember(34)]
        public bool IsChkRetweetTweetButSkipRetweets
        {
            get => _isChkRetweetTweetButSkipRetweets;
            set
            {
                if (value == _isChkRetweetTweetButSkipRetweets)
                    return;
                SetProperty(ref _isChkRetweetTweetButSkipRetweets, value);
            }
        }

        [ProtoMember(15)]
        public bool IsChkCommentOnTweets
        {
            get => _isChkCommentOnTweets;
            set
            {
                if (value == _isChkCommentOnTweets)
                    return;
                SetProperty(ref _isChkCommentOnTweets, value);
            }
        }

        [ProtoMember(16)]
        public RangeUtilities CommentsPerUser
        {
            get => _commentsPerUser;
            set
            {
                if (value == _commentsPerUser)
                    return;
                SetProperty(ref _commentsPerUser, value);
            }
        }

        [ProtoMember(16)]
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

        [ProtoMember(17)]
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

        [ProtoMember(18)]
        public RangeUtilities CommentsPerDayMaxRange
        {
            get => _commentsPerDayMaxRange;
            set
            {
                if (value == _commentsPerDayMaxRange)
                    return;
                SetProperty(ref _commentsPerDayMaxRange, value);
            }
        }

        [ProtoMember(19)]
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

        [ProtoMember(20)]
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

        [ProtoMember(21)]
        public bool IsChkCommentPercentage
        {
            get => _isChkCommentPercentage;
            set
            {
                if (value == _isChkCommentPercentage)
                    return;
                SetProperty(ref _isChkCommentPercentage, value);
            }
        }

        [ProtoMember(22)]
        public int CommentPercentage
        {
            get => _commentPercentage;
            set
            {
                if (value == _commentPercentage)
                    return;
                SetProperty(ref _commentPercentage, value);
            }
        }

        [ProtoMember(35)]
        public bool IsChkCommentButSkipRetweets
        {
            get => _isChkCommentButSkipRetweets;
            set
            {
                if (value == _isChkCommentButSkipRetweets)
                    return;
                SetProperty(ref _isChkCommentButSkipRetweets, value);
            }
        }

        [ProtoMember(23)]
        public string CommentOnUsersTweetInput
        {
            get => _commentOnUsersTweetInput;
            set
            {
                if (value == _commentOnUsersTweetInput)
                    return;
                SetProperty(ref _commentOnUsersTweetInput, value);
            }
        }

        [ProtoMember(24)]
        public List<string> LstCommentOnUsersTweetInput
        {
            get => _lstCommentOnUsersTweetInput;
            set
            {
                if (value == _lstCommentOnUsersTweetInput)
                    return;
                SetProperty(ref _lstCommentOnUsersTweetInput, value);
            }
        }

        [ProtoMember(25)]
        public bool IsChkTweetToUser
        {
            get => _isChkTweetToUser;
            set
            {
                if (value == _isChkTweetToUser)
                    return;
                SetProperty(ref _isChkTweetToUser, value);
            }
        }

        [ProtoMember(26)]
        public bool IsChkMaxTweet
        {
            get => _isChkMaxTweet;
            set
            {
                if (value == _isChkMaxTweet)
                    return;
                SetProperty(ref _isChkMaxTweet, value);
            }
        }

        [ProtoMember(27)]
        public RangeUtilities TweetMaxRange
        {
            get => _tweetMaxRange;
            set
            {
                if (value == _tweetMaxRange)
                    return;
                SetProperty(ref _tweetMaxRange, value);
            }
        }

        [ProtoMember(28)]
        public bool IsChkIncreaseEachDayTweet
        {
            get => _isChkIncreaseEachDayTweet;
            set
            {
                if (value == _isChkIncreaseEachDayTweet)
                    return;
                SetProperty(ref _isChkIncreaseEachDayTweet, value);
            }
        }

        [ProtoMember(29)]
        public RangeUtilities IncreaseEachDayTweet
        {
            get => _IncreaseEachDayTweet;
            set
            {
                if (value == _IncreaseEachDayTweet)
                    return;
                SetProperty(ref _IncreaseEachDayTweet, value);
            }
        }

        [ProtoMember(29)]
        public bool IsChkTweetPercentage
        {
            get => _isChkTweetPercentage;
            set
            {
                if (value == _isChkTweetPercentage)
                    return;
                SetProperty(ref _isChkTweetPercentage, value);
            }
        }

        [ProtoMember(30)]
        public int TweetPercentage
        {
            get => _TweetPercentage;
            set
            {
                if (value == _TweetPercentage)
                    return;
                SetProperty(ref _TweetPercentage, value);
            }
        }

        [ProtoMember(31)]
        public string TweetToUserInput
        {
            get => _tweetToUserInput;
            set
            {
                if (value == _tweetToUserInput)
                    return;
                SetProperty(ref _tweetToUserInput, value);
            }
        }

        [ProtoMember(32)]
        public List<string> LstTweetToUserInput
        {
            get => _lstTweetToUserInput;
            set
            {
                if (value == _lstTweetToUserInput)
                    return;
                SetProperty(ref _lstTweetToUserInput, value);
            }
        }

        [ProtoMember(33)]
        public bool IsChkMuteUser
        {
            get => _isCheckMuteUser;
            set
            {
                if (value == _isCheckMuteUser)
                    return;
                SetProperty(ref _isCheckMuteUser, value);
            }
        }

        [ProtoMember(33)]
        public bool IsChkTurnOnUserNotifications
        {
            get => _isCheckTurnOnUserNotifications;
            set
            {
                if (value == _isCheckTurnOnUserNotifications)
                    return;
                SetProperty(ref _isCheckTurnOnUserNotifications, value);
            }
        }

        [ProtoMember(38)]
        public bool IsSpintax
        {
            get => _IsSpintax;
            set => SetProperty(ref _IsSpintax, value);
        }

        //IsSpintax
    }
}