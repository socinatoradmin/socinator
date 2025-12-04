using System;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using TwtDominatorCore.Database;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Processors.Tweet
{
    internal class SocinatorPublishedPosts : BaseTwitterTweetsProcessor, IQueryProcessor
    {
        public SocinatorPublishedPosts(ITdJobProcess jobProcess, IBlackWhiteListHandler blackWhiteListHandler,
            IDbCampaignService campaignService, ITwitterFunctionFactory twitterFunctionFactory,
            IDelayService threadUtility, IDbInsertionHelper dbInsertionHelper)
            : base(jobProcess, blackWhiteListHandler, campaignService, twitterFunctionFactory.TwitterFunctions,
                threadUtility, dbInsertionHelper)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            jobProcessResult = new JobProcessResult();

            var publishedPosts =
                PublisherInitialize.GetNetworksPublishedPost(queryInfo.QueryValue, SocialNetworks.Twitter);

            foreach (var post in publishedPosts)
            {
                if (_jobProcess.checkJobCompleted()) return;

                _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                CheckUserFilterActiveForCurrentQuery(queryInfo);

                try
                {
                    var tweetInfo = TwitterFunction.GetSingleTweetDetails(_jobProcess.DominatorAccountModel, post.Link);

                    if (tweetInfo.Success)
                        FinalProcessForEachTag(queryInfo, out jobProcessResult, tweetInfo.TweetDetails);
                }
                catch (OperationCanceledException)
                {
                    throw new OperationCanceledException();
                }
                catch (Exception ex)
                {
                    ex.DebugLog(
                        $"TwtDominator : [Account: {_jobProcess.DominatorAccountModel?.AccountBaseModel?.UserName}]   (Module => {ActivityType.ToString()})");
                }
            }
        }
    }
}