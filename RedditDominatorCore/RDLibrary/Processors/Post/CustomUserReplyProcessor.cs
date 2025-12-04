using DominatorHouseCore;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using RedditDominatorCore.RDLibrary.BrowserManager;
using RedditDominatorCore.RDLibrary.DAL;
using RedditDominatorCore.RDUtility;
using RedditDominatorCore.Response;
using System;
using System.Linq;

namespace RedditDominatorCore.RDLibrary.Processors.Post
{
    internal class CustomUserReplyProcessor : BaseRedditPostProcessor
    {
        private readonly IRdBrowserManager _browserManager;
        private RedditCommentRespondHandler _redditCommentRespondHandler;

        public CustomUserReplyProcessor(IProcessScopeModel processScopeModel, IRdJobProcess jobProcess,
            IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService, IRedditFunction redditFunction, IRdBrowserManager browserManager)
            : base(processScopeModel, jobProcess, dbAccountService, globalService, campaignService, redditFunction,
                browserManager)
        {
            _redditCommentRespondHandler = null;
            _browserManager = browserManager;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var userId = Utils.GetLastWordFromUrl(queryInfo.QueryValue);

                if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    _redditCommentRespondHandler = RedditFunction.ScrapeCommentByUrl(JobProcess.DominatorAccountModel,
                        queryInfo.QueryValue, queryInfo, _redditCommentRespondHandler, userId);
                }

                //For browser automation
                else
                {
                    var response = _browserManager.TryAndGetResponse(JobProcess.DominatorAccountModel, queryInfo.QueryValue, 3);
                    _redditCommentRespondHandler = new RedditCommentRespondHandler(response, false, null, userId);
                }

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (_redditCommentRespondHandler == null || !_redditCommentRespondHandler.Success ||
                    _redditCommentRespondHandler.LstCommentOnRedditPost.Count == 0)
                {
                    jobProcessResult.HasNoResult = true;
                    jobProcessResult.maxId = null;
                    return;
                }

                GlobusLogHelper.log.Info(Log.FoundXResults,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName, _redditCommentRespondHandler.LstCommentOnRedditPost.Count, queryInfo.QueryType,
                    queryInfo.QueryValue, ActivityType);
                if (_redditCommentRespondHandler != null && _redditCommentRespondHandler.LstCommentOnRedditPost.Count > 0)
                    foreach (var item in _redditCommentRespondHandler.LstCommentOnRedditPost)
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        StartCustomPostProcess(queryInfo, ref jobProcessResult, item);
                    }
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