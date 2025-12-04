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
    internal class CustomUserChannelProcessor : BaseRedditChannelProcessor
    {
        private readonly IRdBrowserManager _browserManager;
        private SubredditResponseHandler _subredditResponseHandler;

        public CustomUserChannelProcessor(IRdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignService campaignService, IRedditFunction redditFunction, IRdBrowserManager browserManager)
            : base(jobProcess, dbAccountService, campaignService, redditFunction, browserManager)
        {
            _subredditResponseHandler = null;
            _browserManager = browserManager;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    _subredditResponseHandler = RedditFunction.ScrapeSubRedditsByUrl(JobProcess.DominatorAccountModel,
                        queryInfo.QueryValue, queryInfo, _subredditResponseHandler);

                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (_subredditResponseHandler == null || !_subredditResponseHandler.Success ||
                        _subredditResponseHandler.LstSubReddit.Count == 0)
                    {
                        // For Subscribing Quarantine Community
                        if (_subredditResponseHandler != null &&
                            _subredditResponseHandler.Response.Contains(
                                "Are you sure you want to view this community?") &&
                            _subredditResponseHandler.Response.Contains("quarantined"))
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            RedditFunction.SubscribeQuarantineCommunity(JobProcess.DominatorAccountModel,
                                _subredditResponseHandler, queryInfo.QueryValue);

                            _subredditResponseHandler =
                                RedditFunction.ScrapeSubRedditsByUrl(JobProcess.DominatorAccountModel,
                                    queryInfo.QueryValue, queryInfo, null);

                            GlobusLogHelper.log.Info(Log.FoundXResults,
                                JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                JobProcess.DominatorAccountModel.AccountBaseModel.UserName, "1", queryInfo.QueryType,
                                queryInfo.QueryValue, ActivityType);

                            StartCustomSubscribeProcess(queryInfo, ref jobProcessResult,
                                _subredditResponseHandler.LstSubReddit[0]);
                        }
                        else if (_subredditResponseHandler != null &&
                                 !_subredditResponseHandler.Response.Contains(JobProcess.DominatorAccountModel
                                     .AccountBaseModel.UserName))
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            _subredditResponseHandler = null;

                            RedditFunction.SubscribeQuarantineCommunity(JobProcess.DominatorAccountModel,
                                _subredditResponseHandler, queryInfo.QueryValue);

                            _subredditResponseHandler = RedditFunction.ScrapeSubRedditsByUrl(
                                JobProcess.DominatorAccountModel, queryInfo.QueryValue, queryInfo,
                                _subredditResponseHandler);

                            GlobusLogHelper.log.Info(Log.FoundXResults,
                                JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                JobProcess.DominatorAccountModel.AccountBaseModel.UserName, "1", queryInfo.QueryType,
                                queryInfo.QueryValue, ActivityType);

                            StartCustomSubscribeProcess(queryInfo, ref jobProcessResult,
                                _subredditResponseHandler.LstSubReddit[0]);
                        }

                        jobProcessResult.HasNoResult = true;
                        jobProcessResult.maxId = null;
                    }
                }
                else
                {
                    var url = _browserManager.GetUrlFromQuery(ActivityType, queryInfo.QueryValue);
                    var response = _browserManager.TryAndGetResponse(JobProcess.DominatorAccountModel, url, 3);
                    _subredditResponseHandler = new SubredditResponseHandler(response, false, null, url);
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
                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName, "1", queryInfo.QueryType,
                    queryInfo.QueryValue, ActivityType);

                StartCustomSubscribeProcess(queryInfo, ref jobProcessResult, _subredditResponseHandler.LstSubReddit[0]);
                jobProcessResult.IsProcessCompleted = true;
                _browserManager.CloseBrowser();
            }
            catch (OperationCanceledException)
            {
                _browserManager.CloseBrowser();
                throw new OperationCanceledException();
            }
            catch (Exception)
            {
            }
        }
    }
}