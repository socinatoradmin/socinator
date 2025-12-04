using ThreadUtils;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using TwtDominatorCore.Database;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Processors.Tweet
{
    internal class CommentedTweets : BaseTwitterTweetsProcessor, IQueryProcessor
    {
        public CommentedTweets(ITdJobProcess jobProcess, IBlackWhiteListHandler blackWhiteListHandler,
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

            while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
            {
                _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var userInfo =
                    TwitterFunction.GetUsersWhoCommentedOnTweet(_jobProcess.DominatorAccountModel, queryInfo.QueryValue,
                        jobProcessResult.maxId);

                if (userInfo.Success)
                {
                    #region Skip BlackList or WhiteList

                    var lstTagDetails = SkipBlackListOrWhiteList(userInfo.CommentList);

                    #endregion

                    jobProcessResult = StartFinalProcess(queryInfo, jobProcessResult, lstTagDetails);
                    jobProcessResult.maxId = userInfo.MinPosition;

                    if (!userInfo.HasMoreResults) jobProcessResult.HasNoResult = true;
                }
                else
                {
                    jobProcessResult.maxId = null;
                    jobProcessResult.HasNoResult = true;
                }
            }
        }
    }
}