using ThreadUtils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwtDominatorCore.Database;
using TwtDominatorCore.TDEnums;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Subprocesses
{
    public abstract class CommentOnUserNotificationTweetsSubprocess<T> : TdSubprocess<T>
    {
        private readonly IDbInsertionHelper _dbInsertionHelper;
        private readonly IDelayService _delayService;
        private readonly DominatorAccountModel _dominatorAccountModel;
        private readonly ITwitterFunctionFactory _twitterFunctionsFactory;
        private readonly IDbAccountService _dbAccountService;

        protected CommentOnUserNotificationTweetsSubprocess(IProcessScopeModel processScopeModel,
            ITwitterFunctionFactory twitterFunctionFactory, ITdJobProcess jobProcess,
            IDbInsertionHelper dbInsertionHelper, IDelayService threadUtility, Func<T, int> getAfterActionRange,
            Func<T, int> getAfterActionDelay) : base(processScopeModel, getAfterActionRange, getAfterActionDelay)
        {
            _twitterFunctionsFactory = twitterFunctionFactory;
            _dbInsertionHelper = dbInsertionHelper;
            _delayService = threadUtility;
            _dbAccountService = jobProcess.DbAccountService;
            _dominatorAccountModel = processScopeModel.Account;
        }


        // changement in to assign functions factory beacuse of NewUi error to slove this we use this way.
        private ITwitterFunctions _twitterFunctions => _twitterFunctionsFactory.TwitterFunctions;


        protected override void InternalRun(CancellationTokenSource cancellationTokenSource, TagDetails tagDetails, T model, int maxCount)
        {
            CommentOnUserNotificationTweets(cancellationTokenSource, tagDetails, model, maxCount);
        }

        private void CommentOnUserNotificationTweets(CancellationTokenSource cancellationTokenSource, TagDetails tagDetails, T model, int maxCount)
        {
            var userNotificationTweets = _twitterFunctions.GetTweetListFromNotification(_dominatorAccountModel);

            var comments = GetComments(model);
            if (comments.Count == 0 || userNotificationTweets.Count == 0)
                return;

            var currentCommentCount = 0;

            foreach (var tagDetail in userNotificationTweets)
            {
                if (_dbAccountService.IsActivtyDoneWithThisTweetId(tagDetail.Id, ActivityType.Comment))
                    continue;

                if (maxCount > currentCommentCount)
                {
                    _delayService.ThreadSleep(TdConstants.ConsecutiveGetReqInteval);
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var commentBody = comments.OrderBy(x => Guid.NewGuid()).FirstOrDefault()?.Trim();

                    commentBody = SpintaxParser.SpintaxGenerator(commentBody).FirstOrDefault();

                    var commentResponse = _twitterFunctions.Comment(_dominatorAccountModel, tagDetail.Id,
                        tagDetail.Username,
                        commentBody, QueryInfo.NoQuery.QueryType);

                    if (commentResponse.Success)
                    {
                        ++currentCommentCount;
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            _dominatorAccountModel.AccountBaseModel.AccountNetwork, _dominatorAccountModel.UserName,
                            ActivityType.Comment, TdUtility.GetTweetUrl(tagDetail.Username, tagDetail.Id));
                        _dbInsertionHelper.AddInteractedTweetDetailInAccountDb(tagDetail,
                            ActivityType.Comment.ToString(), Enums.ProcessType.AfterRetweet.ToString(), null,
                            commentBody);
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                            _dominatorAccountModel.AccountBaseModel.AccountNetwork, _dominatorAccountModel.UserName,
                            ActivityType.Comment, TdUtility.GetTweetUrl(tagDetail.Username, tagDetail.Id), commentResponse.Issue?.Message);
                    }

                    if (maxCount > currentCommentCount)
                    {
                        var randomTime = GetAfterActionDelay(model);
                        GlobusLogHelper.log.Info(Log.DelayBetweenActivity,
                            _dominatorAccountModel.AccountBaseModel.AccountNetwork,
                            _dominatorAccountModel.AccountBaseModel.UserName, ActivityType.Comment, randomTime);
                        _delayService.ThreadSleep(TimeSpan.FromSeconds(randomTime));
                    }
                }
                else return;
            }
        }

        protected abstract List<string> GetComments(T model);
    }
}
