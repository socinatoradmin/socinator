using System.Collections.Generic;
using ThreadUtils;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using TwtDominatorCore.Database;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDModels;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Processors.Tweet
{
    internal class CommentedUsers : BaseTwitterTweetsProcessor, IQueryProcessor
    {
        public CommentedUsers(ITdJobProcess jobProcess, IBlackWhiteListHandler blackWhiteListHandler,
            IDbCampaignService campaignService, ITwitterFunctionFactory twitterFunctionFactory,
            IDelayService threadUtility, IDbInsertionHelper dbInsertionHelper)
            : base(jobProcess, blackWhiteListHandler, campaignService, twitterFunctionFactory.TwitterFunctions,
                threadUtility, dbInsertionHelper)
        {
        }


        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            if (_jobProcess.checkJobCompleted())
                return;
            CheckUserFilterActiveForCurrentQuery(queryInfo);
            var checkPostHaveUsers = true;

            jobProcessResult = new JobProcessResult();
            while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult && checkPostHaveUsers)
            {
                _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var userInfo =
                    TwitterFunction.GetUsersWhoCommentedOnTweet(_jobProcess.DominatorAccountModel, queryInfo.QueryValue,
                        jobProcessResult.maxId);
                if (userInfo.Success)
                {
                    #region Skip BlackList or WhiteList

                    var commentedUserTagDetails = SkipBlackListOrWhiteList(userInfo.CommentList);

                    #endregion

                    if (userInfo.CommentList.Count == 0)
                        checkPostHaveUsers = false;

                    #region Getting tweets of users commented on post 

                    foreach (var tagUserDetails in commentedUserTagDetails)
                    {
                        var commentedUsersTweets = new List<TagDetails>();
                        // Scraping User Tweets
                        var ResponseHandler = TwitterFunction.GetUserDetails(_jobProcess.DominatorAccountModel,
                            tagUserDetails.Username, queryInfo.QueryType, true);
                        commentedUsersTweets = ResponseHandler.UserDetail.ListTag;

                        // Removing already liked Tweets
                        UniqueListTwtUser(ref commentedUsersTweets);

                        jobProcessResult = StartFinalProcess(queryInfo, jobProcessResult, commentedUsersTweets);

                        jobProcessResult.maxId = userInfo.MinPosition;
                        if (!userInfo.HasMoreResults)
                            jobProcessResult.HasNoResult = true;

                        if (jobProcessResult.IsProcessCompleted)
                            break;
                    }

                    #endregion
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