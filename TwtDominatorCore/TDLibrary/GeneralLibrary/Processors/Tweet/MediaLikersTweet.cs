using ThreadUtils;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using TwtDominatorCore.Database;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Processors.Tweet
{
    internal class MediaLikersTweet : BaseTwitterTweetsProcessor, IQueryProcessor
    {
        public MediaLikersTweet(ITdJobProcess jobProcess, IBlackWhiteListHandler blackWhiteListHandler,
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
                TwitterFunction.GetUsersWhoLikedTweet(_jobProcess.DominatorAccountModel, queryInfo.QueryValue);

            if (userInfo.Success && userInfo.UserList != null)
            {
                var lstTwitterUser = userInfo.UserList;

                #region Skip BlackList or WhiteList

                lstTwitterUser = SkipBlackListOrWhiteList(lstTwitterUser);

                #endregion

                jobProcessResult = StartFinalProcess(queryInfo, jobProcessResult, lstTwitterUser);
            }
        }
    }
}