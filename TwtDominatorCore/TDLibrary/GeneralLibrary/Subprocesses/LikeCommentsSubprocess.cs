using System;
using System.Threading;
using ThreadUtils;
using DominatorHouseCore;
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
    public class LikeCommentsSubprocess<T> : TdSubprocess<T>
    {
        private readonly IDbInsertionHelper _dbInsertionHelper;
        private readonly IDelayService _delayService;
        private readonly DominatorAccountModel _dominatorAccountModel;
        private readonly ITwitterFunctionFactory _twitterFunctionFactory;

        public LikeCommentsSubprocess(IProcessScopeModel processScopeModel,
            ITwitterFunctionFactory twitterFunctionFactory,
            IDbInsertionHelper dbInsertionHelper, Func<T, int> getAfterActionRange,
            Func<T, int> getAfterActionDelay, IDelayService delayService) : base(processScopeModel, getAfterActionRange,
            getAfterActionDelay)
        {
            _twitterFunctionFactory = twitterFunctionFactory;
            _dbInsertionHelper = dbInsertionHelper;
            _delayService = delayService;
            _dominatorAccountModel = processScopeModel.Account;
            CommentModel = processScopeModel.GetActivitySettingsAs<CommentModel>();
        }

        public CommentModel CommentModel { get; set; }

        // changement in to assign functions factory beacuse of NewUi error to slove this we use this way.
        private ITwitterFunctions _twitterFunctions => _twitterFunctionFactory.TwitterFunctions;

        protected override void InternalRun(CancellationTokenSource cancellationTokenSource, TagDetails tagDetails,
            T model, int maxCount)
        {
            LikeComments(cancellationTokenSource, tagDetails, model, maxCount);
        }


        private void LikeComments(CancellationTokenSource cancellationTokenSource, TagDetails tagDetails, T model,
            int maxCount)
        {
            var currentLikeCount = 0;

            var hasNoMorePosts = false;
            string maxPOsition = null;

            _delayService.ThreadSleep(TdConstants.ConsecutiveGetReqInteval);
            var TweetId=tagDetails.IsRetweet && !string.IsNullOrEmpty(tagDetails?.OriginalTweetId) ? tagDetails.OriginalTweetId:tagDetails.Id;
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
                        var commentedText = string.Empty;
                        var SkippedTweet = 0;
                        if ((SkippedTweet = listTagDetails.RemoveAll(x=>x.IsAlreadyLiked)) > 0)
                            GlobusLogHelper.log.Info(Log.CustomMessage, _dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                _dominatorAccountModel.UserName, ActivityType.Like+"Comment", $"Skipped {SkippedTweet} Already Liked Tweets.");
                        foreach (var singleTweetDetails in listTagDetails)
                        {
                            GlobusLogHelper.log.Info(Log.StartedActivity,
                                _dominatorAccountModel.AccountBaseModel.AccountNetwork, 
                                _dominatorAccountModel.UserName,ActivityType.Like+"Comment", TdUtility.GetTweetUrl(singleTweetDetails.Username, singleTweetDetails.Id));
                            cancellationTokenSource.Token.ThrowIfCancellationRequested();
                            var likeResponse = _twitterFunctions.Like(_dominatorAccountModel, singleTweetDetails.Id,
                                singleTweetDetails.Username, QueryInfo.NoQuery.QueryType);
                            if (likeResponse.Success)
                                try
                                {
                                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                        _dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                        _dominatorAccountModel.UserName, ActivityType.Like + "Comment", TdUtility.GetTweetUrl(singleTweetDetails.Username, singleTweetDetails.Id));
                                    _dbInsertionHelper.AddInteractedTweetDetailInAccountDb(singleTweetDetails,
                                        ActivityType.Like.ToString(), Enums.ProcessType.AfterRetweet.ToString());
                                    currentLikeCount++;
                                }
                                catch (OperationCanceledException)
                                {
                                    throw new OperationCanceledException();
                                }
                                catch (Exception ex)
                                {
                                    ex.ErrorLog();
                                }
                            else
                                GlobusLogHelper.log.Info(Log.ActivityFailed,
                                    _dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    _dominatorAccountModel.UserName, ActivityType.Like + "Comment", TdUtility.GetTweetUrl(singleTweetDetails.Username, singleTweetDetails.Id),
                                    likeResponse.Issue?.Message);
                            var randomTime = GetAfterActionDelay(model);
                            if (maxCount <= currentLikeCount) return;
                            GlobusLogHelper.log.Info(Log.DelayBetweenActivity,_dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                _dominatorAccountModel.AccountBaseModel.UserName, ActivityType.Like + "Comment", randomTime);
                            _delayService.ThreadSleep(TimeSpan.FromSeconds(randomTime));
                            
                        }

                        if (!commentHandler.HasMoreResults) return;

                        maxPOsition = commentHandler.MinPosition;
                    }

                    if (string.IsNullOrEmpty(maxPOsition))
                        break;
                }
                else
                {
                    hasNoMorePosts = true;
                }
            }
        }
    }
}