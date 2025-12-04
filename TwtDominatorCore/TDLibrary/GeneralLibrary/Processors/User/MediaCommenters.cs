using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using TwtDominatorCore.Database;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDUtility;
using DominatorHouseCore.Utility;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Processors.User
{
    internal class MediaCommenters : BaseTwitterUserProcessor, IQueryProcessor
    {
        public MediaCommenters(ITdJobProcess jobProcess,
            IBlackWhiteListHandler blackWhiteListHandler, IDbCampaignService campaignService,
            ITwitterFunctionFactory twitterFunctionFactory, IDbInsertionHelper dbInsertionHelper)
            : base(jobProcess, blackWhiteListHandler, campaignService, twitterFunctionFactory.TwitterFunctions,
                dbInsertionHelper)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            if (_jobProcess.checkJobCompleted())
                return;

            var delayService = InstanceProvider.GetInstance<IDelayService>();

            CheckUserFilterActiveForCurrentQuery(queryInfo);
            jobProcessResult = new JobProcessResult();
            while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
            {
                _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var userInfo =
                    TwitterFunction.GetUsersWhoCommentedOnTweet(_jobProcess.DominatorAccountModel, queryInfo.QueryValue,
                        jobProcessResult.maxId);
                if (userInfo.Success)
                {
                    var lstTagDetails = userInfo.CommentList;
                    TweetFilterApply(lstTagDetails);

                    #region Skip BlackList or WhiteList

                    lstTagDetails = SkipBlackListOrWhiteList(lstTagDetails);

                    #endregion

                    jobProcessResult = StartFinalProcess(queryInfo, jobProcessResult, lstTagDetails);
                    if (!userInfo.HasMoreResults)
                        jobProcessResult.HasNoResult = true;
                    else
                        jobProcessResult.maxId = userInfo.MinPosition;
                }
                else
                {
                    //GlobusLogHelper.log.Info(Log.NoMoreDataToPerform,
                    //    _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    //    _jobProcess.DominatorAccountModel.UserName,
                    //    ActivityType);
                    jobProcessResult.maxId = null;
                    jobProcessResult.HasNoResult = true;
                }

                delayService.ThreadSleep(TdConstants.ConsecutiveGetReqInteval);
            }
        }
    }
}