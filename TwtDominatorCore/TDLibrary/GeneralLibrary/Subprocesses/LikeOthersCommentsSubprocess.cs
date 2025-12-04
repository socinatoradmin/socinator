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
    public class LikeOthersCommentsSubprocess<T> : TdSubprocess<T>
    {
        private readonly ActivityType _activityType;
        private readonly IDbInsertionHelper _dbInsertionHelper;
        private readonly IDelayService _delayService;
        private readonly DominatorAccountModel _dominatorAccountModel;
        private readonly ITwitterFunctionFactory _twitterFunctionsFactory;

        public LikeOthersCommentsSubprocess(IProcessScopeModel processScopeModel,
            ITwitterFunctionFactory twitterFunctionFactory,
            IDbInsertionHelper dbInsertionHelper, Func<T, int> getAfterActionRange,
            Func<T, int> getAfterActionDelay, IDelayService delayService) : base(processScopeModel, getAfterActionRange,
            getAfterActionDelay)
        {
            _twitterFunctionsFactory = twitterFunctionFactory;
            _dbInsertionHelper = dbInsertionHelper;
            _delayService = delayService;
            _dominatorAccountModel = processScopeModel.Account;
            _activityType = processScopeModel.ActivityType;
        }

        // changement in to assign functions factory beacuse of NewUi error to slove this we use this way.
        private ITwitterFunctions _twitterFunctions => _twitterFunctionsFactory.TwitterFunctions;

        protected override void InternalRun(CancellationTokenSource cancellationTokenSource, TagDetails tagDetails,
            T model, int maxCount)
        {
            LikeOthersComments(cancellationTokenSource, tagDetails, model, maxCount);
        }


        private void LikeOthersComments(CancellationTokenSource cancellationTokenSource, TagDetails tagDetails, T model,
            int maxCount)
        {
            var currentLikeCount = 0;

            var hasNoMorePosts = false;
            string maxPOsition = null;
            _delayService.ThreadSleep(TdConstants.ConsecutiveGetReqInteval);
            while (!hasNoMorePosts)
            {
                var commentHandler =
                    _twitterFunctions.GetUsersWhoCommentedOnTweet(_dominatorAccountModel, tagDetails.Id, maxPOsition);
                if (commentHandler.Success)
                {
                    var listTagDetails = commentHandler.CommentList;
                    if (listTagDetails != null)
                    {
                        _delayService.ThreadSleep(TdConstants.ConsecutiveGetReqInteval);
                        foreach (var singleTweetDetails in listTagDetails)
                        {
                            if (singleTweetDetails.Username.Equals(_dominatorAccountModel.AccountBaseModel.UserName))
                                continue;
                            cancellationTokenSource.Token.ThrowIfCancellationRequested();
                            var likeResponse = _twitterFunctions.Like(_dominatorAccountModel, singleTweetDetails.Id,
                                singleTweetDetails.Username, QueryInfo.NoQuery.QueryType);
                            if (likeResponse.Success)
                            {
                                GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                    _dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    _dominatorAccountModel.UserName, ActivityType.Like, TdUtility.GetTweetUrl(singleTweetDetails.Username, singleTweetDetails.Id));
                                _dbInsertionHelper.AddInteractedTweetDetailInAccountDb(singleTweetDetails,
                                    ActivityType.Like.ToString(), Enums.ProcessType.AfterComment.ToString());
                                currentLikeCount++;
                            }
                            else
                            {
                                GlobusLogHelper.log.Info(Log.ActivityFailed,
                                    _dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    _dominatorAccountModel.UserName, ActivityType.Like, TdUtility.GetTweetUrl(singleTweetDetails.Username, singleTweetDetails.Id),
                                    likeResponse.Issue?.Message);
                            }
                            if (currentLikeCount >= maxCount) return;
                            var randomTime = GetAfterActionDelay(model);
                            GlobusLogHelper.log.Info(Log.DelayBetweenActivity,
                                _dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                _dominatorAccountModel.AccountBaseModel.UserName, ActivityType.Like, randomTime);
                            _delayService.ThreadSleep(TimeSpan.FromSeconds(randomTime));
                        }
                        if (!commentHandler.HasMoreResults) return;
                        maxPOsition = commentHandler.MinPosition;
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.NoMoreDataToPerform,
                            _dominatorAccountModel.AccountBaseModel.AccountNetwork, _dominatorAccountModel.UserName,
                            ActivityType.Like);
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