using System;
using System.Collections.Generic;
using System.Linq;
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
    public abstract class CommentOntweetSubprocess<T> : TdSubprocess<T>
    {
        private readonly IDbInsertionHelper _dbInsertionHelper;
        private readonly IDelayService _delayService;
        private readonly DominatorAccountModel _dominatorAccountModel;
        private readonly ITwitterFunctionFactory _twitterFunctionsFactory;

        protected CommentOntweetSubprocess(IProcessScopeModel processScopeModel,
            ITwitterFunctionFactory twitterFunctionFactory,
            IDbInsertionHelper dbInsertionHelper, IDelayService threadUtility, Func<T, int> getAfterActionRange,
            Func<T, int> getAfterActionDelay) : base(processScopeModel, getAfterActionRange, getAfterActionDelay)
        {
            _twitterFunctionsFactory = twitterFunctionFactory;
            _dbInsertionHelper = dbInsertionHelper;
            _delayService = threadUtility;
            _dominatorAccountModel = processScopeModel.Account;
        }

        // changement in to assign functions factory beacuse of NewUi error to slove this we use this way.
        private ITwitterFunctions _twitterFunctions => _twitterFunctionsFactory.TwitterFunctions;

        protected override void InternalRun(CancellationTokenSource cancellationTokenSource, TagDetails tagDetails,
            T model, int maxCount)
        {
            CommentOntweet(cancellationTokenSource, tagDetails, model, maxCount);
        }

        private void CommentOntweet(CancellationTokenSource cancellationTokenSource, TagDetails tagDetails, T model,
            int maxCount)
        {
            bool isSpintax = IsSpintax(model);
            var comments = new List<string>();
            if (isSpintax)
                comments = SpintaxParser.SpintaxGenerator(GetComments(model).FirstOrDefault()?.Trim());
            else
                comments = GetComments(model);
            if (comments.Count == 0)
                return;
            var currentCommentCount = 0;
            var FailedCount = 0;
            var tweetId = tagDetails.IsRetweet ? tagDetails.OriginalTweetId : tagDetails.Id;
            while (maxCount > currentCommentCount)
            {
                _delayService.ThreadSleep(TdConstants.ConsecutiveGetReqInteval);
                cancellationTokenSource.Token.ThrowIfCancellationRequested();
                var commentBody = comments.FirstOrDefault();
                var commentResponse = _twitterFunctions.Comment(_dominatorAccountModel, tagDetails.Id,
                    tagDetails.Username,
                    commentBody, QueryInfo.NoQuery.QueryType);
                if (commentResponse.Success)
                {
                    ++currentCommentCount;
                    if(FailedCount > 0)
                        FailedCount--;
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        _dominatorAccountModel.AccountBaseModel.AccountNetwork, _dominatorAccountModel.UserName,
                        ActivityType.Comment,TdUtility.GetTweetUrl(tagDetails.Username, tagDetails.Id));
                    _dbInsertionHelper.AddInteractedTweetDetailInAccountDb(tagDetails,
                        ActivityType.Comment.ToString(), Enums.ProcessType.AfterRetweet.ToString(), null,
                        commentBody);
                    comments.Remove(commentBody);
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed,
                        _dominatorAccountModel.AccountBaseModel.AccountNetwork, _dominatorAccountModel.UserName,
                        ActivityType.Comment, TdUtility.GetTweetUrl(tagDetails.Username, tagDetails.Id) + " ==> ", commentResponse.Issue?.Message);
                    FailedCount++;
                }
                if (FailedCount >= 3 || comments.Count == 0)
                    break;
                var randomTime = GetAfterActionDelay(model);
                GlobusLogHelper.log.Info(Log.DelayBetweenActivity,
                    _dominatorAccountModel.AccountBaseModel.AccountNetwork,
                    _dominatorAccountModel.AccountBaseModel.UserName, ActivityType.Comment, randomTime);
                _delayService.ThreadSleep(TimeSpan.FromSeconds(randomTime));
            }
        }

        protected abstract List<string> GetComments(T model);

        protected abstract bool IsSpintax(T model);
    }
}