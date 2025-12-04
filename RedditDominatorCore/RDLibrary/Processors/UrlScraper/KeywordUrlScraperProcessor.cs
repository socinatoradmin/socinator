using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using RedditDominatorCore.RDLibrary.BrowserManager;
using RedditDominatorCore.RDLibrary.DAL;
using RedditDominatorCore.Response;

namespace RedditDominatorCore.RDLibrary.Processors.UrlScraper
{
    internal class KeywordUrlScraperProcessor : BaseRedditUrlScraperProcessor
    {
        private RedditPostResponseHandler _redditPostResponseHandler;
        public KeywordUrlScraperProcessor(IRdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService, 
            IDbCampaignService campaignService, IRedditFunction redditFunction, IRdBrowserManager browserManager) 
            : base(jobProcess, dbAccountService, globalService, campaignService, redditFunction, browserManager)
        {
            _redditPostResponseHandler = null;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            _redditPostResponseHandler = RedditFunction.ScrapePostsByKeywords(JobProcess.DominatorAccountModel, queryInfo.QueryValue, queryInfo, _redditPostResponseHandler);
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            if (_redditPostResponseHandler == null || !_redditPostResponseHandler.Success || _redditPostResponseHandler.NewredditPostList.Count == 0)
            {
                jobProcessResult.HasNoResult = true;
                jobProcessResult.maxId = null;
            }
            else
            {
                GlobusLogHelper.log.Info(Log.FoundXResults, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork, JobProcess.DominatorAccountModel.AccountBaseModel.UserName, _redditPostResponseHandler.NewredditPostList.Count, queryInfo.QueryType, queryInfo.QueryValue, ActivityType);
                if (_redditPostResponseHandler.HasMoreResults == false)
                    jobProcessResult.HasNoResult = true;

                StartProcessForUrlScraper(queryInfo, ref jobProcessResult, _redditPostResponseHandler.NewredditPostList);
                jobProcessResult.maxId = _redditPostResponseHandler.PaginationParameter.LastPaginationId;
            }
        }
        
    }
}
