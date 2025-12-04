using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using RedditDominatorCore.RDLibrary.BrowserManager;
using RedditDominatorCore.RDLibrary.DAL;
using RedditDominatorCore.Response;
using System;

namespace RedditDominatorCore.RDLibrary.Processors.Channel
{
    internal class KeywordsChannelProcessor : BaseRedditChannelProcessor
    {
        private readonly IRdBrowserManager _browserManager;

        public KeywordsChannelProcessor(IRdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbGlobalService globalService,
            IDbCampaignService campaignService, IRedditFunction redditFunction, IRdBrowserManager browserManager)
            : base(jobProcess, dbAccountService, campaignService, redditFunction, browserManager)
        {
            _subredditResponseHandler = null;
            _browserManager = browserManager;
        }

        private SubredditResponseHandler _subredditResponseHandler { get; set; }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    _subredditResponseHandler = RedditFunction.ScrapeSubRedditsByKeywords(
                        JobProcess.DominatorAccountModel, queryInfo.QueryValue, queryInfo, _subredditResponseHandler);
                    jobProcessResult.maxId = _subredditResponseHandler.PaginationParameter.LastPaginationId;
                }

                //For browser automation
                else
                {
                    if (_subredditResponseHandler == null)
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        var searchUrl = $"{RdConstants.NewRedditHomePageAPI}/search/?q={queryInfo.QueryValue}&type=sr";
                        var response = _browserManager.TryAndGetResponse(JobProcess.DominatorAccountModel, searchUrl, 3);
                        _subredditResponseHandler = new SubredditResponseHandler(response, false, null);
                    }
                    //For pagination
                    else
                    {
                        var nextPageResponse =
                            _browserManager.ScrollWindowAndGetNextPageData(JobProcess.DominatorAccountModel, "communities", true);
                        _subredditResponseHandler = new SubredditResponseHandler(nextPageResponse, true, null);
                    }
                }

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (_subredditResponseHandler == null || !_subredditResponseHandler.Success ||
                    _subredditResponseHandler.LstSubReddit.Count == 0)
                {
                    jobProcessResult.HasNoResult = true;
                    jobProcessResult.maxId = null;
                    return;
                }

                GlobusLogHelper.log.Info(Log.FoundXResults,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName,
                    _subredditResponseHandler.LstSubReddit.Count,
                    queryInfo.QueryType, queryInfo.QueryValue, ActivityType);

                jobProcessResult.HasNoResult = !JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser
                                               && !_subredditResponseHandler.HasMoreResults;

                StartKeywordSubscribeProcess(queryInfo, ref jobProcessResult, _subredditResponseHandler.LstSubReddit);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception)
            {
            }
            finally { if (_browserManager != null && _browserManager.BrowserWindow != null && jobProcessResult.HasNoResult) _browserManager.CloseBrowser(); }
        }
    }
}