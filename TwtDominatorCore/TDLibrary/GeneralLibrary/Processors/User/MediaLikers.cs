using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using TwtDominatorCore.Database;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Processors.User
{
    internal class MediaLikers : BaseTwitterUserProcessor, IQueryProcessor
    {
        public MediaLikers(ITdJobProcess jobProcess, IBlackWhiteListHandler blackWhiteListHandler,
            IDbCampaignService campaignService, ITwitterFunctionFactory twitterFunctionFactory,
            IDbInsertionHelper dbInsertionHelper)
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