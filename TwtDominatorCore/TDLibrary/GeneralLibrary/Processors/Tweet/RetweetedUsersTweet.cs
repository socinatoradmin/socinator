using ThreadUtils;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Database;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Processors.Tweet
{
    internal class RetweetedUsersTweet : BaseTwitterTweetsProcessor, IQueryProcessor
    {
        public RetweetedUsersTweet(ITdJobProcess jobProcess, IBlackWhiteListHandler blackWhiteListHandler,
            IDbCampaignService campaignService, ITwitterFunctionFactory twitterFunctionFactory,
            IDelayService threadUtility, IDbInsertionHelper dbInsertionHelper)
            : base(jobProcess, blackWhiteListHandler, campaignService, twitterFunctionFactory.TwitterFunctions,
                threadUtility, dbInsertionHelper)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            if (_jobProcess.checkJobCompleted()) return;
            CheckUserFilterActiveForCurrentQuery(queryInfo);
            jobProcessResult = new JobProcessResult();

            var userInfo =
                TwitterFunction.GetUsersWhoRetweetedTweet(_jobProcess.DominatorAccountModel, queryInfo.QueryValue);

            if (userInfo.Success)
            {
                var lstTwitterUser = userInfo.UserList;

                if (lstTwitterUser == null || lstTwitterUser.Count == 0)
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        _jobProcess.DominatorAccountModel.UserName, ActivityType,
                        string.Format("LangKeyNoMoreDataToActivityForQuery".FromResourceDictionary(), ActivityType,
                            queryInfo.QueryValue));

                #region Skip BlackList or WhiteList

                lstTwitterUser = SkipBlackListOrWhiteList(lstTwitterUser);

                #endregion

                _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                jobProcessResult = StartFinalProcess(queryInfo, jobProcessResult, lstTwitterUser);
            }
        }
    }
}