using System;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using QuoraDominatorCore.Request;
using QuoraDominatorCore.Response;

namespace QuoraDominatorCore.QdLibrary.Processors.Users
{
    public class UserAnswerUpvotersProcessor : BaseQuoraProcessor
    {
        private readonly IQdHttpHelper _httpHelper;

        public UserAnswerUpvotersProcessor(IQuoraBrowserManager browser, IQdJobProcess jobProcess,
            IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService, IQuoraFunctions objQuoraFunct, IQdHttpHelper httpHelper,
            IProcessScopeModel processScopeModel) :
            base(browser, jobProcess, dbAccountService, globalService, campaignService, objQuoraFunct,
                processScopeModel)
        {
            _httpHelper = httpHelper;
        }

        protected override void Process(QueryInfo queryinfo, ref JobProcessResult jobProcessResult)
        {
            var index = 0;
            while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                jobProcessResult = ScrapeAnswerUpvoters(queryinfo, ref jobProcessResult, ref index);
        }

        private JobProcessResult ScrapeAnswerUpvoters(QueryInfo queryinfo, ref JobProcessResult objJobProcessResult, ref int index)
        {
            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var IsBrowser = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser;
                AnswerUpvotersResponseHandler objAnswerUpvotersResponseHandler = null;
                var AnswerId = quoraFunct.GetAnswerId(queryinfo.QueryValue, out string AnswerNodeId);
                if (!IsBrowser)
                {
                    var FailedCount=0;
                TryAgain:
                    objAnswerUpvotersResponseHandler = quoraFunct.AnswerUpvoters(JobProcess.DominatorAccountModel,
                        queryinfo.QueryValue, null, AnswerId, AnswerNodeId, objAnswerUpvotersResponseHandler == null ? 0 : objAnswerUpvotersResponseHandler.PaginationCount).Result;
                    while (FailedCount++ <= 3 && (objAnswerUpvotersResponseHandler == null ||objAnswerUpvotersResponseHandler.AnswerUpvoters.Count==0 || objAnswerUpvotersResponseHandler.Response.Response.Contains("Bad Request")))
                        goto TryAgain;
                }
                else
                {
                    _browser.SearchByCustomUrl(JobProcess.DominatorAccountModel, queryinfo.QueryValue);
                    var answerUpvotersResponse = _browser.GetAnswerUpvoters(JobProcess.DominatorAccountModel, ref index);
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    objAnswerUpvotersResponseHandler = new AnswerUpvotersResponseHandler(answerUpvotersResponse);
                }
                objJobProcessResult = FilterAndStartFinalProcess(queryinfo, objJobProcessResult,
                        objAnswerUpvotersResponseHandler.AnswerUpvoters);
                if (objJobProcessResult.IsProcessCompleted ||objAnswerUpvotersResponseHandler.AnswerUpvoters.Count == 0)
                {
                    objJobProcessResult.IsProcessCompleted = true;
                    return objJobProcessResult;
                }
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                StartPagination(queryinfo, ref objJobProcessResult, objAnswerUpvotersResponseHandler,AnswerId,AnswerNodeId);
            }
            catch (OperationCanceledException)
            {
                if (_browser.BrowserWindow != null) _browser.CloseBrowser();
                throw new OperationCanceledException();                                                    
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            objJobProcessResult.IsProcessCompleted = true;
            return objJobProcessResult;
        }

        private void StartPagination(QueryInfo queryinfo, ref JobProcessResult objJobProcessResult,
            AnswerUpvotersResponseHandler objAnswerUpvotersResponseHandler,string AnswerId,string AnswerNodeId)
        {
            while(objAnswerUpvotersResponseHandler!=null &&
                objAnswerUpvotersResponseHandler.HasMoreResult)
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                objAnswerUpvotersResponseHandler = quoraFunct.AnswerUpvoters(JobProcess.DominatorAccountModel,
                        queryinfo.QueryValue, null, AnswerId, AnswerNodeId,objAnswerUpvotersResponseHandler.PaginationCount).Result;
                objJobProcessResult = FilterAndStartFinalProcess(queryinfo, objJobProcessResult,
                        objAnswerUpvotersResponseHandler.AnswerUpvoters);
                if (objJobProcessResult.IsProcessCompleted || 
                    objAnswerUpvotersResponseHandler.AnswerUpvoters.Count == 0 ||
                    !objAnswerUpvotersResponseHandler.HasMoreResult)
                    break;
            }
        }
    }
}