using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Database;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Processors.User
{
    internal class RetweetedUsers : BaseTwitterUserProcessor, IQueryProcessor
    {
        public RetweetedUsers(ITdJobProcess jobProcess, IBlackWhiteListHandler blackWhiteListHandler,
            IDbCampaignService campaignService,
            ITwitterFunctionFactory twitterFunctionFactory, IDbInsertionHelper dbInsertionHelper)
            : base(jobProcess, blackWhiteListHandler, campaignService, twitterFunctionFactory.TwitterFunctions,
                dbInsertionHelper)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            if (_jobProcess.checkJobCompleted())
                return;
            CheckUserFilterActiveForCurrentQuery(queryInfo);
            jobProcessResult = new JobProcessResult();
            var UserInfo =
                TwitterFunction.GetUsersWhoRetweetedTweet(_jobProcess.DominatorAccountModel, queryInfo.QueryValue);
            if (UserInfo.Success)
            {
                var lstTwitterUser = UserInfo.UserList;

                if (lstTwitterUser == null || lstTwitterUser.Count == 0)
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        _jobProcess.DominatorAccountModel.UserName, ActivityType,
                        $"No more data available to perform {ActivityType} for query value {queryInfo.QueryValue}");

                #region Skip BlackList or WhiteList

                lstTwitterUser = SkipBlackListOrWhiteList(lstTwitterUser);

                #endregion


                _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                jobProcessResult = StartFinalProcess(queryInfo, jobProcessResult, lstTwitterUser);
            }
        }
    }
}