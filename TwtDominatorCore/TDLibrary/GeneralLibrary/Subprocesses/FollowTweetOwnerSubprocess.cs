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
    public class FollowTweetOwnerSubprocess<T> : TdSubprocess<T>
    {
        private readonly IDbInsertionHelper _dbInsertionHelper;
        private readonly IDelayService _delayService;
        private readonly DominatorAccountModel _dominatorAccountModel;
        private readonly ITwitterFunctionFactory _twitterFunctionsFactory;


        public FollowTweetOwnerSubprocess(IProcessScopeModel processScopeModel,
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

        // changement in to assign functions factory beacuse of NewUi error to slove this we use this way.
        private ITwitterFunctions _twitterFunctions => _twitterFunctionsFactory.TwitterFunctions;

        protected override void InternalRun(CancellationTokenSource cancellationTokenSource, TagDetails tagDetails,
            T model, int maxCount)
        {
            FollowTweetOwner(cancellationTokenSource, tagDetails, model);
        }

        private void FollowTweetOwner(CancellationTokenSource cancellationTokenSource, TagDetails tagDetails, T model)
        {
            // _twitterFunctions = _twitterFunctionsFactory.TwitterFunctions;
            if (tagDetails.FollowStatus || GetAfterActionRange(model) == 0) //!tagDetails.FollowBackStatus ||
                return;


            _delayService.ThreadSleep(TdConstants.ConsecutiveGetReqInteval);

            cancellationTokenSource.Token.ThrowIfCancellationRequested();
            var followResponse =
                _twitterFunctions.Follow(_dominatorAccountModel, tagDetails.UserId, tagDetails.Username, null);

            if (followResponse.Success)
            {
                GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                    _dominatorAccountModel.AccountBaseModel.AccountNetwork, _dominatorAccountModel.UserName,
                    ActivityType.Follow, tagDetails.Username);
                var twitterUser = new TwitterUser
                {
                    UserId = tagDetails.UserId,
                    Username = tagDetails.Username,
                    FollowBackStatus = tagDetails.FollowBackStatus
                };
                _dbInsertionHelper.AddInteractedUserDetailsInAccountDb(twitterUser,
                    ActivityType.Follow.ToString(), Enums.ProcessType.AfterRetweet.ToString());
            }
            else
            {
                GlobusLogHelper.log.Info(Log.ActivityFailed, _dominatorAccountModel.AccountBaseModel.AccountNetwork,
                    _dominatorAccountModel.UserName, ActivityType.Follow, tagDetails.Username,
                    followResponse.Issue?.Message);
            }
        }
    }
}