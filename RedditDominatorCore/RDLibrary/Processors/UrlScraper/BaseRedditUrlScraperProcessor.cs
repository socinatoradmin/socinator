using DominatorHouseCore;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using RedditDominatorCore.RDLibrary.BrowserManager;
using RedditDominatorCore.RDLibrary.DAL;
using RedditDominatorCore.RDModel;
using System;
using System.Collections.Generic;

namespace RedditDominatorCore.RDLibrary.Processors.UrlScraper
{
    internal abstract class BaseRedditUrlScraperProcessor : BaseRedditProcessor
    {
        private readonly IDbAccountService _dbAccountService;

        protected BaseRedditUrlScraperProcessor(IRdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, 
            IDbGlobalService globalService, IDbCampaignService campaignService, IRedditFunction redditFunction, IRdBrowserManager browserManager) 
            : base(jobProcess, dbAccountService, globalService, campaignService, redditFunction, browserManager)
        {
            _dbAccountService = dbAccountService;
        }

        public void StartProcessForUrlScraper(QueryInfo queryInfo, ref JobProcessResult jobProcessResult, List<RedditPost> newredditPostList)
        {
            var listOfRedditPostAfterFilterCheckOrUncheck = new List<RedditPost>();
            CampaignDetails campaignDetails = null;
            if (JobProcess.ModuleSetting.IsPerformActionFromRandomPercentageOfAccounts ||
                JobProcess.ModuleSetting.EnableDelayBetweenPerformingActionOnSamePost)
            {
                campaignDetails = JobProcess.CampaignDetails;
            }
            try
            {
                var objPost = new ScrapeFilter.Post(JobProcess.ModuleSetting);
                foreach (var post in newredditPostList)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    objPost = new ScrapeFilter.Post(JobProcess.ModuleSetting);

                    var passedFilter = true;

                    if (objPost.IsFilterApplied())
                        passedFilter = objPost.AppplyFilters(post);

                    if (!passedFilter)
                        continue;

                    listOfRedditPostAfterFilterCheckOrUncheck.Add(post);
                }
                if (listOfRedditPostAfterFilterCheckOrUncheck.Count == 0 && objPost.IsFilterApplied())
                    GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType, "Filter Not Matched");
                else
                {
                    if (objPost.IsFilterApplied())
                        GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType, $"{listOfRedditPostAfterFilterCheckOrUncheck.Count} Result Matched With Filter");

                    foreach (var post in listOfRedditPostAfterFilterCheckOrUncheck)
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        if (!CheckPostUniqueNess(jobProcessResult, post, ActivityType)) continue;
                        if (AlreadyInteractedPost(post.Permalink)) continue;
                        if (!ApplyCampaignLevelSettings(queryInfo, post.Permalink, campaignDetails)) continue;

                        SendToPerformActivity(ref jobProcessResult, post, queryInfo);
                    }
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }



        public void SendToPerformActivity(ref JobProcessResult jobProcessResult, RedditPost newRedditPost, QueryInfo queryInfo)
        {
            jobProcessResult = JobProcess.FinalProcess(new ScrapeResultNew
            {
                ResultPost = newRedditPost,
                QueryInfo = queryInfo
            });
        }

    }
}
