using DominatorHouseCore;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using RedditDominatorCore.RDLibrary.BrowserManager;
using RedditDominatorCore.RDLibrary.DAL;
using RedditDominatorCore.Response;
using System;

namespace RedditDominatorCore.RDLibrary.Processors.CommentScraper
{
    internal class CommunityCommentScraperProcessor : BaseRedditCommentScraperProcessor
    {
        private readonly IRdBrowserManager _browserManager;
        private RedditPostResponseHandler _redditPostResponseHandler;

        public CommunityCommentScraperProcessor(IRdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbGlobalService globalService,
            IDbCampaignService campaignService, IRedditFunction redditFunction, IRdBrowserManager browserManager)
            : base(jobProcess, dbAccountService, globalService, campaignService, redditFunction, browserManager)
        {
            _redditPostResponseHandler = null;
            _browserManager = browserManager;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    _redditPostResponseHandler = RedditFunction.ScrapePostsByUrl(JobProcess.DominatorAccountModel,
                        queryInfo.QueryValue, queryInfo, _redditPostResponseHandler);
                    jobProcessResult.maxId = _redditPostResponseHandler.PaginationParameter.LastPaginationId;
                }
                //For browser automation
                else
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (_redditPostResponseHandler == null)
                    {
                        var searchUrl = queryInfo.QueryValue;
                        var response = _browserManager.SearchByCustomUrl(JobProcess.DominatorAccountModel, searchUrl);
                        _redditPostResponseHandler = new RedditPostResponseHandler(response, false, null);
                    }
                    //For browser pagination
                    else
                    {
                        var response = _browserManager.ScrollWindowAndGetNextPageData(JobProcess.DominatorAccountModel);
                        _redditPostResponseHandler = new RedditPostResponseHandler(response, true, null);
                    }
                }

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (_redditPostResponseHandler == null || !_redditPostResponseHandler.Success ||
                    _redditPostResponseHandler.LstRedditPost.Count == 0)
                {
                    jobProcessResult.HasNoResult = true;
                    jobProcessResult.maxId = null;
                    return;
                }

                GlobusLogHelper.log.Info(Log.FoundXResults,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName,
                    _redditPostResponseHandler.LstRedditPost.Count,
                    queryInfo.QueryType, queryInfo.QueryValue, ActivityType);

                jobProcessResult.HasNoResult = !JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser
                                               && _redditPostResponseHandler.HasMoreResults;

                StartKeywordCommentScraperProcess(queryInfo, ref jobProcessResult,
                    _redditPostResponseHandler.LstRedditPost);
            }
            catch (OperationCanceledException)
            {
                _browserManager.CloseBrowser();
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
