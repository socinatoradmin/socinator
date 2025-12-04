using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
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
    internal class CustomCommentScraperProcessor : BaseRedditCommentScraperProcessor
    {
        private readonly IRdBrowserManager _browserManager;

        public CustomCommentScraperProcessor(IRdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbGlobalService globalService,
            IDbCampaignService campaignService, IRedditFunction redditFunction, IRdBrowserManager browserManager)
            : base(jobProcess, dbAccountService, globalService, campaignService, redditFunction, browserManager)
        {
            _browserManager = browserManager;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            IResponseParameter response = null;
            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                RedditPostResponseHandler _redditPostResponseHandler;
                if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    _redditPostResponseHandler = RedditFunction.ScrapePostsByUrl(JobProcess.DominatorAccountModel,
                        queryInfo.QueryValue, queryInfo, null);
                }

                //For browser automation
                else
                {
                    response = _browserManager.SearchByCustomUrl(JobProcess.DominatorAccountModel,
                        queryInfo.QueryValue.Replace("www", "new"));
                    _redditPostResponseHandler = new RedditPostResponseHandler(response, false, null);
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

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                StartCustomCommentScraperProcess(queryInfo, ref jobProcessResult,
                    _redditPostResponseHandler.LstRedditPost[0], response);
                jobProcessResult.IsProcessCompleted = true;
                _browserManager.CloseBrowser();
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