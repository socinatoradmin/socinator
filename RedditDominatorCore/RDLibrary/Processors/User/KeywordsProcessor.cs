using DominatorHouseCore;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using RedditDominatorCore.RDLibrary.BrowserManager;
using RedditDominatorCore.RDLibrary.DAL;
using RedditDominatorCore.Response;
using System;

namespace RedditDominatorCore.RDLibrary.Processors.User
{
    internal class KeywordsProcessor : BaseRedditUserProcessor
    {
        private readonly IRdBrowserManager _browserManager;
        private RedditUserResponseHandler _redditUserResponseHandler;

        public KeywordsProcessor(IRdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbGlobalService globalService,
            IDbCampaignService campaignService, IRedditFunction redditFunction, IRdBrowserManager browserManager)
            : base(jobProcess, dbAccountService, globalService, campaignService, redditFunction, browserManager)
        {
            _redditUserResponseHandler = null;
            _browserManager = browserManager;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    _redditUserResponseHandler = RedditFunction.ScrapeUsersByKeywords(JobProcess.DominatorAccountModel,
                        queryInfo.QueryValue, queryInfo, _redditUserResponseHandler);
                    jobProcessResult.maxId = _redditUserResponseHandler.PaginationParameter.LastPaginationId;
                }

                //For browser automation
                else
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (_redditUserResponseHandler == null)
                    {
                        var searchUrl = $"https://www.reddit.com/search/?q={queryInfo.QueryValue}&type=people";
                        var response = _browserManager.TryAndGetResponse(JobProcess.DominatorAccountModel, searchUrl, 3);
                        _redditUserResponseHandler = new RedditUserResponseHandler(response, false, null);
                    }
                    //For browser pagination
                    else
                    {
                        var response = _browserManager.ScrollWindowAndGetNextPageData(JobProcess.DominatorAccountModel);
                        _redditUserResponseHandler = new RedditUserResponseHandler(response, true, null);
                    }
                }

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (_redditUserResponseHandler == null || !_redditUserResponseHandler.Success ||
                    _redditUserResponseHandler.LstRedditUser.Count == 0)
                {
                    jobProcessResult.HasNoResult = true;
                    jobProcessResult.maxId = null;
                    return;
                }

                GlobusLogHelper.log.Info(Log.FoundXResults,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName,
                    _redditUserResponseHandler.LstRedditUser.Count,
                    queryInfo.QueryType, queryInfo.QueryValue, ActivityType);

                jobProcessResult.HasNoResult = !JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser
                                               && _redditUserResponseHandler.HasMoreResults;

                StartKeywordUserProcess(queryInfo, ref jobProcessResult, _redditUserResponseHandler.LstRedditUser);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally
            {
                if (_browserManager != null) _browserManager.CloseBrowser();
            }
        }
    }
}