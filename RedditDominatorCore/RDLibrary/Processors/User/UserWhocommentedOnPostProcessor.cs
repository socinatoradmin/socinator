using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using RedditDominatorCore.RDLibrary.BrowserManager;
using RedditDominatorCore.RDLibrary.DAL;
using RedditDominatorCore.Response;
using System;
using System.Linq;

namespace RedditDominatorCore.RDLibrary.Processors.User
{
    internal class UserWhocommentedOnPostProcessor : BaseRedditUserProcessor
    {
        private readonly IRdBrowserManager _browserManager;
        private ScrapeCommentedUsersResponseHandler _scrapeCommentedUsersResponseHandler;

        public UserWhocommentedOnPostProcessor(IRdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbGlobalService globalService,
            IDbCampaignService campaignService, IRedditFunction redditFunction, IRdBrowserManager browserManager)
            : base(jobProcess, dbAccountService, globalService, campaignService, redditFunction, browserManager)
        {
            _scrapeCommentedUsersResponseHandler = null;
            _browserManager = browserManager;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    _scrapeCommentedUsersResponseHandler = RedditFunction.ScrapeCommentedUsersOnPosts(
                        JobProcess.DominatorAccountModel, queryInfo.QueryValue, queryInfo,
                        _scrapeCommentedUsersResponseHandler);
                    jobProcessResult.maxId = _scrapeCommentedUsersResponseHandler.PaginationParameter.LastPaginationId;
                }

                //For browser automation
                else
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (_scrapeCommentedUsersResponseHandler == null)
                    {
                        var response =
                            _browserManager.SearchByCustomUrl(JobProcess.DominatorAccountModel, queryInfo.QueryValue);
                        _scrapeCommentedUsersResponseHandler =
                            new ScrapeCommentedUsersResponseHandler(response, false, null);
                    }
                    //For browser pagination
                    else
                    {
                        var response = _browserManager.ScrollWindowAndGetNextPageData(JobProcess.DominatorAccountModel);
                        _scrapeCommentedUsersResponseHandler =
                            new ScrapeCommentedUsersResponseHandler(response, true, null);
                    }
                }

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (_scrapeCommentedUsersResponseHandler == null || !_scrapeCommentedUsersResponseHandler.Success ||
                    _scrapeCommentedUsersResponseHandler.LstCommentedUser.Count == 0)
                {
                    jobProcessResult.HasNoResult = true;
                    jobProcessResult.maxId = null;
                    return;
                }

                var userList = _scrapeCommentedUsersResponseHandler.LstCommentedUser.Distinct().ToList();
                GlobusLogHelper.log.Info(Log.FoundXResults,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName,
                    _scrapeCommentedUsersResponseHandler.LstCommentedUser.Count,
                    queryInfo.QueryType, queryInfo.QueryValue, ActivityType);

                jobProcessResult.HasNoResult = !JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser
                                               && _scrapeCommentedUsersResponseHandler.HasMoreResults;

                StartDataOfUsersWhoCommentedProcess(queryInfo, ref jobProcessResult, userList);
            }
            catch (OperationCanceledException)
            {
                _browserManager.CloseBrowser();
                throw new OperationCanceledException();
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}