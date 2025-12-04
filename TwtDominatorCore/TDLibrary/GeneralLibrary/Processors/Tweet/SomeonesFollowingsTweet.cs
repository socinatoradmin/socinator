using System;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.TdTables.Accounts;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Database;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Processors.Tweet
{
    internal class SomeonesFollowingsTweet : BaseTwitterTweetsProcessor, IQueryProcessor
    {
        private readonly ITDAccountUpdateFactory _accountUpdateFactory;

        public SomeonesFollowingsTweet(ITdJobProcess jobProcess, IBlackWhiteListHandler blackWhiteListHandler,
            IDbCampaignService campaignService, ITwitterFunctionFactory twitterFunctionFactory,
            ITDAccountUpdateFactory accountUpdateFactory, IDelayService threadUtility,
            IDbInsertionHelper dbInsertionHelper)
            : base(jobProcess, blackWhiteListHandler, campaignService, twitterFunctionFactory.TwitterFunctions,
                threadUtility, dbInsertionHelper)
        {
            _accountUpdateFactory = accountUpdateFactory;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            if (_jobProcess.checkJobCompleted()) return;

            CheckUserFilterActiveForCurrentQuery(queryInfo);

            jobProcessResult = new JobProcessResult();
            try
            {
                if (string.Equals(queryInfo.QueryValue, "[Own]", StringComparison.CurrentCultureIgnoreCase))
                {
                    StartProcessForOwnFollowings();
                }
                else
                {
                    var userInfo = TwitterFunction.GetUserDetails(_jobProcess.DominatorAccountModel,
                        queryInfo.QueryValue.Trim(), queryInfo.QueryType);

                    if (userInfo.Success && userInfo.UserDetail.FollowingsCount > 0 &&
                        (!userInfo.UserDetail.IsPrivate ||
                         queryInfo.QueryValue.Contains(_jobProcess.DominatorAccountModel.AccountBaseModel.ProfileId)))
                        while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                        {
                            var followingsHandler = TwitterFunction
                                .GetUserFollowingsAsync(_jobProcess.DominatorAccountModel, queryInfo.QueryValue,
                                    _jobProcess.JobCancellationTokenSource.Token, jobProcessResult.maxId).Result;

                            _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                            if (followingsHandler.Success)
                            {
                                var listOfTwitterUser = followingsHandler.ListOfTwitterUser;

                                #region Skip BlackList or WhiteList

                                listOfTwitterUser = SkipBlackListOrWhiteList(listOfTwitterUser);

                                #endregion

                                jobProcessResult = StartFinalProcess(queryInfo, jobProcessResult, listOfTwitterUser);
                                jobProcessResult.maxId = followingsHandler.MinPosition;

                                if (!followingsHandler.HasMoreResults) jobProcessResult.HasNoResult = true;
                            }
                            else
                            {
                                jobProcessResult.maxId = null;
                                jobProcessResult.HasNoResult = true;
                            }
                        }

                    if (!jobProcessResult.IsProcessCompleted)
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            _jobProcess.DominatorAccountModel.UserName,
                            string.Format("LangKeyNoMoreDataToActivityForQuery".FromResourceDictionary(), ActivityType,
                                queryInfo.QueryValue));
                }
            }

            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(_jobProcess.DominatorAccountModel?.AccountBaseModel?.UserName))
                    ex.ErrorLog();
            }
        }

        protected void StartProcessForOwnFollowings()
        {
            if (_jobProcess.checkJobCompleted()) return;
            _accountUpdateFactory.CheckIsFollowingsUpdated(_jobProcess.DominatorAccountModel,
                AccountModel.LastFollowersUpdatedTime, _jobProcess.JobCancellationTokenSource.Token);
            var followingsList = GetFriendshipsFromDb(FollowType.Following);
            followingsList.Shuffle();
            _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
        }
    }
}