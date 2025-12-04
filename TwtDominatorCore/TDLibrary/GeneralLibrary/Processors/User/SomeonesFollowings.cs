using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.TdTables.Accounts;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using System;
using System.Threading;
using TwtDominatorCore.Database;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Processors.User
{
    internal class SomeonesFollowings : BaseTwitterUserProcessor, IQueryProcessor
    {
        private readonly ITDAccountUpdateFactory _accountUpdateFactory;
        private readonly IDelayService _delayService;

        public SomeonesFollowings(ITdJobProcess jobProcess,
            IBlackWhiteListHandler blackWhiteListHandler, IDbCampaignService campaignService,
            ITwitterFunctionFactory twitterFunctionFactory, ITDAccountUpdateFactory accountUpdateFactory,
            IDbInsertionHelper dbInsertionHelper, IDelayService delayService)
            : base(jobProcess, blackWhiteListHandler, campaignService, twitterFunctionFactory.TwitterFunctions,
                dbInsertionHelper)
        {
            _accountUpdateFactory = accountUpdateFactory;
            _delayService = delayService;
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
                    var isNotFirst = false;

                    if (_jobProcess.ModuleSetting.IsSavePagination) jobProcessResult.maxId = GetPaginationId(queryInfo);
                    _delayService.ThreadSleep(15000);
                    var userInfo = TwitterFunction.GetUserDetails(_jobProcess.DominatorAccountModel,
                        queryInfo.QueryValue.Trim(), queryInfo.QueryType);


                    if (userInfo.Success && userInfo.UserDetail.FollowingsCount > 0 &&
                        (!userInfo.UserDetail.IsPrivate ||
                         queryInfo.QueryValue.Contains(_jobProcess.DominatorAccountModel.AccountBaseModel.ProfileId)))
                        while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                        {
                            // We are not saving pagination first time we hit the url we will 
                            // save next time when we paginate it because pagination id gives us next page users
                            // here we save last page id
                            if (_jobProcess.ModuleSetting.IsSavePagination)
                                AddOrUpdatePaginationId(queryInfo, jobProcessResult.maxId, ref isNotFirst);

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
                                //schedule job next auto activity
                                ++TwitterFunction.UsedQueryCount;
                                TwitterFunction.IsScheduleNext = true;

                                jobProcessResult.maxId = null;
                                jobProcessResult.HasNoResult = true;
                            }
                        }

                    if (!jobProcessResult.IsProcessCompleted)
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            _jobProcess.DominatorAccountModel.UserName,ActivityType,
                            $"No more data available to perform {ActivityType} for query value {queryInfo.QueryValue}");
                }
            }

            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
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