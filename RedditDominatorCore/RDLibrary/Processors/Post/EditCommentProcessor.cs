using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using RedditDominatorCore.RDLibrary.BrowserManager;
using RedditDominatorCore.RDLibrary.DAL;
using RedditDominatorCore.RDLibrary.Processors.Post;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDUtility;
using RedditDominatorCore.Response;
using System;
using System.Linq;

namespace RedditDominatorCore.RDLibrary.Processors.EditComment
{
    internal class EditCommentProcessor : BaseRedditPostProcessor
    {
        private readonly IRdBrowserManager _browserManager;
        private RedditCommentRespondHandler _redditCommentRespondHandler;

        public EditCommentProcessor(IProcessScopeModel processScopeModel, IRdJobProcess jobProcess,
            IDbAccountServiceScoped dbAccountService,
            IDbGlobalService globalService, IDbCampaignService campaignService, IRedditFunction redditFunction,
            IRdBrowserManager browserManager)
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
                var templateFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
                var templateModel = templateFileManager.GetTemplateById(JobProcess.TemplateId);
                var editCommentModel = JsonConvert.DeserializeObject<EditCommentModel>(templateModel.ActivitySettings);

                var lstCommentDetails = editCommentModel.CommentDetails.Where(x =>
                    x.Accounts.Equals(JobProcess.DominatorAccountModel.AccountBaseModel.UserName));

                foreach (var commentDetails in lstCommentDetails)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var url = commentDetails.EditCommentUrl;
                    if (AlreadyInteractedPostForEditComment(url, commentDetails.Message)) continue;
                    queryInfo = new QueryInfo
                    {
                        QueryValue = url
                    };
                    var userId = Utils.GetLastWordFromUrl(queryInfo.QueryValue);

                    if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        _redditCommentRespondHandler = RedditFunction.ScrapeCommentByUrl(
                            JobProcess.DominatorAccountModel, queryInfo.QueryValue, queryInfo, null, userId);
                    }

                    //For browser automation
                    else
                    {
                        var response =
                            _browserManager.SearchByCustomUrl(JobProcess.DominatorAccountModel, queryInfo.QueryValue);
                        _redditCommentRespondHandler = new RedditCommentRespondHandler(response, false, null, userId);
                    }

                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (_redditCommentRespondHandler == null || !_redditCommentRespondHandler.Success ||
                        _redditCommentRespondHandler.LstCommentOnRedditPost.Count == 0)
                    {
                        jobProcessResult.HasNoResult = true;
                        return;
                    }

                    var newredditCommentList = _redditCommentRespondHandler.LstCommentOnRedditPost[0];
                    newredditCommentList.Commenttext = commentDetails.Message;

                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        $"Found 1 result {queryInfo.QueryValue}");

                    StartFinalPostProcess(ref jobProcessResult, newredditCommentList, queryInfo);
                    jobProcessResult.HasNoResult = true;
                    _browserManager.CloseBrowser();
                }
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