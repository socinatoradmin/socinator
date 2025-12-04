using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.Request;
using QuoraDominatorCore.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace QuoraDominatorCore.QdLibrary.Processors.Answers
{
    public class AnswerOnQuestionKeywordProcessor : BaseQuoraProcessor
    {
        private readonly IQdHttpHelper _httpHelper;
        public AnswerOnQuestionKeywordProcessor(IQuoraBrowserManager browser, IQdJobProcess jobProcess,
            IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService, IQuoraFunctions objQuoraFunct, IQdHttpHelper httpHelper,
            IProcessScopeModel processScopeModel) :
            base(browser, jobProcess, dbAccountService, globalService, campaignService, objQuoraFunct,
                processScopeModel)
        {
            _httpHelper = httpHelper;
        }


        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var url = $"{QdConstants.HomePageUrl}/search?q=" + queryInfo.QueryValue.Replace(" ", "%20") + "&type=question";
                IResponseParameter response=new DominatorHouseCore.Request.ResponseParameter();
                var IsBrowser = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser;
                if (!IsBrowser)
                {
                    var FailedCount= 0;
                TryAgain:
                    response = quoraFunct.SearchQueryByType(QdConstants.GetSearchQueryAPI, Enums.SearchQueryType.Questions, queryInfo.QueryValue, JobProcess.DominatorAccountModel, 0);
                    while(FailedCount ++<=2 && (response is null || string.IsNullOrEmpty(response.Response) || response.Response.Contains("Bad Request")))
                        goto TryAgain;
                }
                else
                {
                    int noOfTimesToCheck = 0;
                    int ScrollCount = 0;
                TryAgain:
                    response = _browser.SearchByCustomUrl(JobProcess.DominatorAccountModel, url);
                    while (noOfTimesToCheck++ <= 5)
                    {
                        if (response.Response.Contains("find any results for"))
                        {
                            _browser.BrowserWindow.Refresh();
                            Thread.Sleep(TimeSpan.FromSeconds(4));
                            response.Response = _browser.BrowserWindow.GetPageSource();
                        }
                        else
                            break;
                    }
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    response = _browser.SearchByCustomUrlAndScrollDown(JobProcess.DominatorAccountModel);
                    if (response.Response.Contains("find any results for") && ScrollCount++ < 3)
                        goto TryAgain;
                }
                var objQuestionByKeywordResponseHandler = new QuestionByKeywordResponseHandler(response,IsBrowser);
                StartAnswerForEachQuestion(queryInfo, objQuestionByKeywordResponseHandler.QuestionList);
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (jobProcessResult.IsProcessCompleted)
                    return;
                StartPagination(queryInfo, objQuestionByKeywordResponseHandler, ref jobProcessResult);
            }
            catch (OperationCanceledException)
            {
                
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally { if (_browser.BrowserWindow != null) _browser.CloseBrowser(); }
        }
        private void StartPagination(QueryInfo queryInfo, QuestionByKeywordResponseHandler questionByKeywordResponse, ref JobProcessResult jobProcessResult)
        {
            try
            {
                while(questionByKeywordResponse != null && questionByKeywordResponse.HasMoreResults)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var FailedCount = 0;
                TryAgain:
                    var response = quoraFunct.SearchQueryByType(QdConstants.GetSearchQueryAPI, Enums.SearchQueryType.Questions, queryInfo.QueryValue, JobProcess.DominatorAccountModel, questionByKeywordResponse.PaginationCount);
                    while (FailedCount++ <= 2 && (response is null || string.IsNullOrEmpty(response.Response) || response.Response.Contains("Bad Request")))
                        goto TryAgain;
                    questionByKeywordResponse = new QuestionByKeywordResponseHandler(response, false);
                    StartAnswerForEachQuestion(queryInfo, questionByKeywordResponse.QuestionList);
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                }
            }
            catch (Exception ex) { ex.DebugLog(); }
            finally { jobProcessResult.IsProcessCompleted=true; }
        }
        private void StartAnswerForEachQuestion(QueryInfo queryInfo, List<QuestionDetails> questionList)
        {
            var SkippedQuestions = 0;
            var InteractedQuestions = DbAccountService?.GetInteractedAnswers().ToList();
            if (InteractedQuestions != null && (SkippedQuestions = questionList.RemoveAll(x=>InteractedQuestions.Any(y=>y.AnswersUrl==x.QuestionUrl)))>0)
                GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    $"Skipped {SkippedQuestions} Interacted Questions.");
            foreach (var question in questionList)
                try
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    
                    var objQuestionDetailsResponseHandler =
                        quoraFunct.QuestionDetails(JobProcess.DominatorAccountModel, question.QuestionUrl);
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var quoraUser = new QuoraUser
                    {
                        Uid=question.QuestionId,
                        Url = question.QuestionUrl
                    };
                    var jobProcessResult =
                        FilterAndStartFinalProcessForAnswerOnQuestion(queryInfo, quoraUser,
                            objQuestionDetailsResponseHandler);
                    if (jobProcessResult.IsProcessCompleted)
                        break;
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

        private new JobProcessResult FilterAndStartFinalProcessForAnswerOnQuestion(QueryInfo queryInfo,
            QuoraUser quoraUser, QuestionDetailsResponseHandler questionresponsehandler)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            if (!QuestionFilterApply(questionresponsehandler))
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                return JobProcess.FinalProcess(new ScrapeResultNew
                {
                    ResultUser = quoraUser,
                    QueryInfo = queryInfo
                });
            }

            return new JobProcessResult();
        }
    }
}