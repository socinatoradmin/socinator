using System;
using System.Linq;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Database;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Processors.Tweet
{
    internal class UnlikeTweet : BaseTwitterTweetsProcessor, IQueryProcessor
    {
        private readonly IDbInsertionHelper _dbInsertionHelper;
        private readonly IDelayService _delayService;
        private readonly ITwitterFunctionFactory _twitterFunctionFactory;
        private IQueryProcessor _queryProcessor;

        public UnlikeTweet(ITdJobProcess jobProcess, IBlackWhiteListHandler blackWhiteListHandler,
            IDbCampaignService campaignService, ITwitterFunctionFactory twitterFunctionFactory,
            IDelayService threadUtility, IDbInsertionHelper dbInsertionHelper)
            : base(jobProcess, blackWhiteListHandler, campaignService, twitterFunctionFactory.TwitterFunctions,
                threadUtility, dbInsertionHelper)
        {
            _delayService = threadUtility;
            _twitterFunctionFactory = twitterFunctionFactory;
            _dbInsertionHelper = dbInsertionHelper;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (_jobProcess.checkJobCompleted()) return;

                jobProcessResult = new JobProcessResult();

                _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (_jobProcess.ModuleSetting.UnLike.IsCustomTweets)
                {
                    var customTweets = _jobProcess.ModuleSetting.UnLike.CustomTweets.Split('\n')
                        .Where(x => !string.IsNullOrEmpty(x.Trim())).ToList();
                    customTweets = customTweets.Select(x => x.Trim()).ToList();

                    var getInteractedTweet = _dbAccountService.GetInteractedPosts(ActivityType.Unlike)
                        .Select(x => x.TweetId).ToList();

                    foreach (var tweet in customTweets)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            _jobProcess.DominatorAccountModel.UserName, _jobProcess.ActivityType,$"Searching for Custom Tweet ==> {tweet}");
                        var tweetId = Utilities.GetBetween(tweet + "$$", "/status/", "$$").Replace("/", "");
                        if (getInteractedTweet.Contains(tweetId)) continue;

                        queryInfo = new QueryInfo {QueryType = "LangKeyUnlike".FromResourceDictionary(),QueryValue = tweet};
                        var eachUserTag = TwitterFunction
                            .GetSingleTweetDetails(_jobProcess.DominatorAccountModel, tweetId)
                            .TweetDetails;
                        FinalProcessForEachTag(queryInfo, out jobProcessResult, eachUserTag);
                    }
                }

                if (_jobProcess.ModuleSetting.UnLike.IsLikedTweets)
                {
                    queryInfo = new QueryInfo {QueryType = "LangKeyUnlike".FromResourceDictionary()};
                    _queryProcessor = new SpecificUserLikedTweets(_jobProcess, BlackWhiteListHandler,
                        _campaignService, _twitterFunctionFactory, _delayService, _dbInsertionHelper);

                    _queryProcessor.Start(queryInfo);
                }

                if (!jobProcessResult.IsProcessCompleted && !_jobProcess.ModuleSetting.UnLike.IsLikedTweets)
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        _jobProcess.DominatorAccountModel.UserName, ActivityType,
                        $"{"LangKeyNoMoreDataAvailableToPerform".FromResourceDictionary()} {ActivityType} ");
            }

            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}