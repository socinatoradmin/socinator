using System;
using System.Threading;
using ThreadUtils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Database;
using TwtDominatorCore.TDEnums;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Subprocesses
{
    public class RetweetCommentsSubprocess<T> : TdSubprocess<T>
    {
        private readonly IDbInsertionHelper _dbInsertionHelper;
        private readonly IDelayService _delayService;
        private readonly DominatorAccountModel _dominatorAccountModel;
        private readonly ITwitterFunctionFactory _twitterFunctionsFactory;

        public RetweetCommentsSubprocess(IProcessScopeModel processScopeModel,
            ITwitterFunctionFactory twitterFunctionFactory,
            IDbInsertionHelper dbInsertionHelper, Func<T, int> getAfterActionRange,
            Func<T, int> getAfterActionDelay, IDelayService delayService) : base(processScopeModel, getAfterActionRange,
            getAfterActionDelay)
        {
            _twitterFunctionsFactory = twitterFunctionFactory;
            _dbInsertionHelper = dbInsertionHelper;
            _delayService = delayService;
            _dominatorAccountModel = processScopeModel.Account;
        }

        // changement in to assign functions factory beacuse of NewUi error to slove this we use this way
        private ITwitterFunctions _twitterFunctions => _twitterFunctionsFactory.TwitterFunctions;

        protected override void InternalRun(CancellationTokenSource cancellationTokenSource, TagDetails tagDetails,
            T model, int maxCount)
        {
            RetweetComments(cancellationTokenSource, tagDetails, model, maxCount);
        }


        private void RetweetComments(CancellationTokenSource cancellationTokenSource, TagDetails tagDetails, T model,
            int maxCount)

        {
            var currentRetweetCount = 0;
            var hasNoMorePosts = false;
            string maxPOsition = null;
            _delayService.ThreadSleep(TdConstants.ConsecutiveGetReqInteval);
            var TweetId = tagDetails.IsRetweet && !string.IsNullOrEmpty(tagDetails?.OriginalTweetId) ? tagDetails.OriginalTweetId : tagDetails.Id;
            var tweetUrl = $"{TdConstants.MainUrl}{tagDetails.Username}/status/{TweetId}";
            while (!hasNoMorePosts)
            {
                var commentHandler =
                    _twitterFunctions.GetUsersWhoCommentedOnTweet(_dominatorAccountModel, tweetUrl, maxPOsition);
                if (commentHandler.Success)
                {
                    var listTagDetails = commentHandler.CommentList;
                    if (listTagDetails != null && listTagDetails.Count > 0)
                    {
                        _delayService.ThreadSleep(TdConstants.ConsecutiveGetReqInteval);
                        var SkippedTweet = 0;
                        if ((SkippedTweet = listTagDetails.RemoveAll(x => x.IsAlreadyRetweeted)) > 0)
                            GlobusLogHelper.log.Info(Log.CustomMessage, _dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                _dominatorAccountModel.UserName, ActivityType.Retweet, $"Skipped {SkippedTweet} Already ReTweeted Tweets.");
                        foreach (var singleTweetDetails in listTagDetails)
                        {
                            cancellationTokenSource.Token.ThrowIfCancellationRequested();
                            GlobusLogHelper.log.Info(Log.StartedActivity,
                                _dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                _dominatorAccountModel.UserName, ActivityType.Retweet+"Comment", TdUtility.GetTweetUrl(singleTweetDetails.Username, singleTweetDetails.Id));
                            var retweetResponse = _twitterFunctions.Retweet(_dominatorAccountModel,
                                singleTweetDetails.Id,
                                singleTweetDetails.Username);
                            if (retweetResponse.Success)
                            {
                                var tweetUser = !string.IsNullOrEmpty(_dominatorAccountModel.AccountBaseModel.UserName) ? _dominatorAccountModel.AccountBaseModel.UserName : singleTweetDetails?.Username;
                                var id = retweetResponse != null && !string.IsNullOrEmpty(retweetResponse?.TweetId) ? retweetResponse?.TweetId : singleTweetDetails.Id;
                                GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                    _dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    _dominatorAccountModel.UserName, ActivityType.Retweet+"Comment", TdUtility.GetTweetUrl(tweetUser, id));
                                _dbInsertionHelper.AddInteractedTweetDetailInAccountDb(tagDetails,
                                    ActivityType.Retweet.ToString(), Enums.ProcessType.AfterRetweet.ToString());
                                _dbInsertionHelper.AddFeedsInfo(singleTweetDetails, true);
                                currentRetweetCount++;
                            }
                            else
                            {
                                GlobusLogHelper.log.Info(Log.ActivityFailed,
                                    _dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    _dominatorAccountModel.UserName, ActivityType.Retweet + "Comment", TdUtility.GetTweetUrl(singleTweetDetails.Username, singleTweetDetails.Id),
                                    retweetResponse.Issue?.Message);
                            }

                            var randomTime =
                                GetAfterActionDelay(model);
                            GlobusLogHelper.log.Info(Log.DelayBetweenActivity,
                                _dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                _dominatorAccountModel.AccountBaseModel.UserName, ActivityType.Retweet + "Comment", randomTime);
                            _delayService.ThreadSleep(TimeSpan.FromSeconds(randomTime));
                            if (maxCount <= currentRetweetCount) return;
                        }

                        if (!commentHandler.HasMoreResults) return;

                        maxPOsition = commentHandler.MinPosition;
                    }
                    else if (string.IsNullOrEmpty(maxPOsition))
                    {
                        break;
                    }
                }
                else
                {
                    hasNoMorePosts = true;
                }
            }
        }
    }
}