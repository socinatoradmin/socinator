using DominatorHouseCore;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using RedditDominatorCore.RDLibrary.BrowserManager;
using RedditDominatorCore.RDLibrary.DAL;
using RedditDominatorCore.RDLibrary.Processors.User;
using RedditDominatorCore.Response;
using System;
using System.Linq;

namespace RedditDominatorCore.RDLibrary.Processors.Messanger
{
    internal class AutoReplyProcessor : BaseRedditUserProcessor
    {

        private IRdBrowserManager _browserManager;
        IRedditFunction _redditFunction;
        public AutoReplyProcessor(IRdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbGlobalService globalService,
            IDbCampaignService campaignService, IRedditFunction redditFunction, IRdBrowserManager browserManager)
            : base(jobProcess, dbAccountService, globalService, campaignService, redditFunction, browserManager)
        {

            _browserManager = browserManager;
            _redditFunction = redditFunction;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var userDetails = _redditFunction.GetUserDetailsByUsername(JobProcess.DominatorAccountModel, JobProcess.DominatorAccountModel.UserName);
                var userId = userDetails.RedditUser.Id.ToString();
                var user = userDetails.RedditUser;
                var response = _redditFunction.GetConversationDetails(JobProcess.DominatorAccountModel, user);
                var responseHandler = new RedditChatResponseHandler(response, "GetConversationDetails", user);
                if (responseHandler != null && responseHandler.Conversation.Count > 0)
                {
                    responseHandler.Conversation.RemoveAll(x => x.Messages.LastOrDefault()?.SenderID == userId);
                    if (responseHandler.Conversation.Count > 0)
                        StartMatchLastMessageAndAutoReplyProcess(queryInfo, ref jobProcessResult, responseHandler.Conversation);
                    else
                        GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork, JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"No New Message Found!");
                }
                jobProcessResult.IsProcessCompleted = true;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                jobProcessResult.IsProcessCompleted = true;
            }
            finally { if (_browserManager != null && _browserManager.BrowserWindow != null) _browserManager.CloseBrowser(); }
        }
    }
}
