using System;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.Response;

namespace QuoraDominatorCore.QdLibrary.Processors.Answers
{
    public class AnswerCustomUserProcessor : BaseQuoraProcessor
    {
        public AnswerCustomUserProcessor(IQuoraBrowserManager browser, IQdJobProcess jobProcess,
            IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService, IQuoraFunctions objQuoraFunct, IProcessScopeModel processScopeModel) :
            base(browser, jobProcess, dbAccountService, globalService, campaignService, objQuoraFunct,
                processScopeModel)
        {
        }

        protected override void Process(QueryInfo queryinfo, ref JobProcessResult objJobProcessResult)
        {
            string url;
            try
            {
                if (queryinfo.QueryValue.Contains("http") && queryinfo.QueryValue.Contains("answers"))
                    url = queryinfo.QueryValue;
                else if (queryinfo.QueryValue.Contains("http") && !queryinfo.QueryValue.Contains("answers"))
                    url =queryinfo.QueryValue.EndsWith("/")?queryinfo.QueryValue + "answers": queryinfo.QueryValue + "/answers";
                else
                    url = $"{QdConstants.HomePageUrl}/profile/" + queryinfo.QueryValue + "/answers";
                UserAnswerResponseHandler objUserAnswerResponseHandler = null;
                var IsBrowser = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser;
                var UserId = quoraFunct.GetUserId(url);
                if (!IsBrowser)
                {
                    var SearchFailedCount = 0;
                TryAgain:
                    objUserAnswerResponseHandler = quoraFunct.UserAnswer(JobProcess.DominatorAccountModel,UserId,-1);
                    while (SearchFailedCount++ <= 2 && (objUserAnswerResponseHandler == null || objUserAnswerResponseHandler.AnswerList.Count==0))
                        goto TryAgain;
                }
                else
                {
                    var resp = _browser.SearchByCustomUrl(JobProcess.DominatorAccountModel, url);
                    for(int i = 0; i <= 5; i++)
                    {
                        resp = _browser.SearchByCustomUrlAndScrollDown(JobProcess.DominatorAccountModel);
                    }
                    objUserAnswerResponseHandler = new UserAnswerResponseHandler(resp,IsBrowser);
                }
                objJobProcessResult = FilterAndStartFinalProcessForAnswers(queryinfo, objJobProcessResult,
                    objUserAnswerResponseHandler.AnswerList);
                if (objUserAnswerResponseHandler.AnswerList.Count == 0 || objJobProcessResult.IsProcessCompleted)
                    return;
                StartPagination(queryinfo, ref objJobProcessResult,objUserAnswerResponseHandler,UserId);
            }
            catch (OperationCanceledException)
            {
                if (_browser.BrowserWindow != null) _browser.CloseBrowser();
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void StartPagination(QueryInfo queryinfo, ref JobProcessResult jobProcessResult,
            UserAnswerResponseHandler userAnswerResponseHandler,string UserId)
        {
            try
            {
                while (userAnswerResponseHandler!=null && userAnswerResponseHandler.HasMoreResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    var SearchFailedCount = 0;
                TryAgain:
                    userAnswerResponseHandler = quoraFunct.UserAnswer(JobProcess.DominatorAccountModel, UserId, userAnswerResponseHandler.EndCursor);
                    while (SearchFailedCount++ <= 2 && (userAnswerResponseHandler == null || userAnswerResponseHandler.AnswerList.Count == 0))
                        goto TryAgain;
                    jobProcessResult = FilterAndStartFinalProcessForAnswers(queryinfo, jobProcessResult,
                        userAnswerResponseHandler.AnswerList);
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                }
            }catch (Exception ex)
            {
                ex.DebugLog();
            }finally { jobProcessResult.IsProcessCompleted = true; }
        }
    }
}