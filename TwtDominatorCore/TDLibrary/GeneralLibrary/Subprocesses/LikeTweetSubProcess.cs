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
    public class LikeTweetSubProcess<T> : TdSubprocess<T>
    {
        private readonly ActivityType _activityType;
        private readonly IDbInsertionHelper _dbInsertionHelper;
        private readonly IDelayService _delayService;
        private readonly DominatorAccountModel _dominatorAccountModel;
        private readonly ITwitterFunctionFactory _twitterFunctionsFactory;

        public LikeTweetSubProcess(IProcessScopeModel processScopeModel, ITwitterFunctionFactory twitterFunctionFactory,
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

        // changement in to assign functions factory beacuse of NewUi error to slove this we use this way
        private ITwitterFunctions _twitterFunctions => _twitterFunctionsFactory.TwitterFunctions;

        protected override void InternalRun(CancellationTokenSource cancellationTokenSource, TagDetails tagDetails,
            T model, int maxCount)
        {
            LikeTweet(cancellationTokenSource, tagDetails);
        }


        private void LikeTweet(CancellationTokenSource cancellationTokenSource, TagDetails tagDetails)
        {
            _delayService.ThreadSleep(TdConstants.ConsecutiveGetReqInteval);
            try
            {
                var likeResponse = _twitterFunctions.Like(_dominatorAccountModel, tagDetails.Id,
                    tagDetails.Username, QueryInfo.NoQuery.QueryType);
                if (likeResponse.Success)
                {
                    _dbInsertionHelper.AddInteractedTweetDetailInAccountDb(tagDetails,
                        ActivityType.Like.ToString(), Enums.ProcessType.AfterComment.ToString());
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        _dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        _dominatorAccountModel.UserName, ActivityType.Like, TdUtility.GetTweetUrl(tagDetails.Username, tagDetails.Id));
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed,
                        _dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        _dominatorAccountModel.UserName, ActivityType.Like, TdUtility.GetTweetUrl(tagDetails.Username, tagDetails.Id),
                        likeResponse.Issue?.Message);
                }

                cancellationTokenSource.Token.ThrowIfCancellationRequested();
                _delayService.ThreadSleep(TdConstants.ConsecutiveGetReqInteval);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
        }
    }
}