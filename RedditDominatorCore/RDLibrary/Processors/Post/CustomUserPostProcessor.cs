using DominatorHouseCore;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using RedditDominatorCore.RDLibrary.BrowserManager;
using RedditDominatorCore.RDLibrary.DAL;
using RedditDominatorCore.Response;
using System;

namespace RedditDominatorCore.RDLibrary.Processors.Post
{
    internal class CustomUserPostProcessor : BaseRedditPostProcessor
    {
        private readonly IRdBrowserManager _browserManager;
        private RedditPostResponseHandler _redditPostResponseHandler;

        public CustomUserPostProcessor(IProcessScopeModel processScopeModel, IRdJobProcess jobProcess,
            IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService, IRedditFunction redditFunction, IRdBrowserManager browserManager)
            : base(processScopeModel, jobProcess, dbAccountService, globalService, campaignService, redditFunction,
                browserManager)
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
                }

                //For browser automation
                else
                {
                    var SearchByCustomUrlfailedCount = 0;
                    var response =
                        _browserManager.SearchByCustomUrl(JobProcess.DominatorAccountModel, queryInfo.QueryValue);
                    while (SearchByCustomUrlfailedCount++ < 3 && string.IsNullOrEmpty(response.Response))
                        response = _browserManager.SearchByCustomUrl(JobProcess.DominatorAccountModel, queryInfo.QueryValue);
                    _redditPostResponseHandler = new RedditPostResponseHandler(response, false, null);

                    //To accept permission to view adult content
                    if (response != null && (response.Response.Contains("You must be 18+ to view this community")
                                             || response.Response.Contains(
                                                 "You must be at least eighteen years old to view this content. Are you over eighteen and willing to see adult content?")
                        ))
                        AllowPermissionForAdultPageInBrowser(response);
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
                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName, "1", queryInfo.QueryType,
                    queryInfo.QueryValue, ActivityType);

                if (_redditPostResponseHandler.LstRedditPost[0].IsArchived)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                        "This post is archived, New votes cannot be cast");
                    jobProcessResult.HasNoResult = true;
                    return;
                }

                StartCustomPostProcess(queryInfo, ref jobProcessResult, _redditPostResponseHandler.LstRedditPost[0]);
                jobProcessResult.IsProcessCompleted = true;
                _browserManager.CloseBrowser();
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