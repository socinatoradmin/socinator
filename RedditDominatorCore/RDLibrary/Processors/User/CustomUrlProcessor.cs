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

namespace RedditDominatorCore.RDLibrary.Processors.User
{
    internal class CustomUrlProcessor : BaseRedditUserProcessor
    {
        private const string RedditUserUrl = "https://www.reddit.com/user/";
        private readonly IRdBrowserManager _browserManager;
        private UserResponseHandler _redditUserResponseHandler;

        public CustomUrlProcessor(IRdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
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
                var username = Utils.GetLastWordFromUrl(queryInfo.QueryValue).Replace("?to=", string.Empty);
                if (string.IsNullOrEmpty(username))
                    username = Utilities.GetBetween(queryInfo.QueryValue, RedditUserUrl, "/");

                if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    _redditUserResponseHandler =
                        RedditFunction.GetUserDetailsByUsername(JobProcess.DominatorAccountModel, username);
                }

                //For browser automation
                else
                {
                    var url = string.Empty;
                    if (!username.Contains(RedditUserUrl))
                        url = RedditUserUrl + $"{username}";
                    var response = _browserManager.TryAndGetResponse(JobProcess.DominatorAccountModel, url, 3);
                    _redditUserResponseHandler = new UserResponseHandler(response);
                }

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (_redditUserResponseHandler == null || !_redditUserResponseHandler.Success ||
                    _redditUserResponseHandler.Response == null)
                {
                    jobProcessResult.HasNoResult = true;
                    jobProcessResult.maxId = null;
                    return;
                }

                GlobusLogHelper.log.Info(Log.FoundXResults,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName, "1", queryInfo.QueryType,
                    queryInfo.QueryValue, ActivityType);

                StartCustomUserProcess(queryInfo, ref jobProcessResult, _redditUserResponseHandler.RedditUser);
                jobProcessResult.IsProcessCompleted = true;
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
            finally { if (_browserManager != null && _browserManager.BrowserWindow != null) _browserManager.CloseBrowser(); }
        }
    }
}