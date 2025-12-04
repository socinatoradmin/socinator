using System;
using System.Collections.Generic;
using System.Linq;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.TdTables.Campaign;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using TwtDominatorCore.Database;
using TwtDominatorCore.QueryHelper;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using DominatorHouseCore.Utility;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Processors.Tweet
{
    internal class ScrapedTweetCampaign : BaseTwitterTweetsProcessor, IQueryProcessor
    {
        public ScrapedTweetCampaign(ITdJobProcess jobProcess, IBlackWhiteListHandler blackWhiteListHandler,
            IDbCampaignService campaignService,
            ITwitterFunctionFactory twitterFunctionFactory, IDelayService threadUtility,
            IDbInsertionHelper dbInsertionHelper)
            : base(jobProcess, blackWhiteListHandler, campaignService, twitterFunctionFactory.TwitterFunctions,
                threadUtility, dbInsertionHelper)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                var classMapper = new ClassMapper(_jobProcess.AccountId, _jobProcess.AccountName);

                if (_jobProcess.checkJobCompleted()) return;

                // getting campaign details from given campaingn id in queryvalue 
                var scrapedTweetCampaignDb =
                    InstanceProvider.ResolveCampaignDbOperations(queryInfo.QueryValue, SocialNetworks.Twitter);

                // get all scraped tweets from  given campaignid
                var scrapedTweets = scrapedTweetCampaignDb.Get<InteractedPosts>();

                List<string> activityDoneWithUserList;
                if (_campaignService != null)
                    activityDoneWithUserList = _campaignService.GetAllInteractedPosts().Select(x => x.TweetId).ToList();
                else
                    activityDoneWithUserList = _dbAccountService.GetInteractedPosts(ActivityType).Select(x => x.TweetId)
                        .ToList();

                scrapedTweets.RemoveAll(user => activityDoneWithUserList.Contains(user.TweetId));

                foreach (var tweet in scrapedTweets)
                {
                    jobProcessResult = new JobProcessResult();
                    var tweetDetails = classMapper.InteractedTweetToTweetDetailsMapper(tweet);
                    _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    FinalProcessForEachTag(queryInfo, out jobProcessResult, tweetDetails);
                }
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