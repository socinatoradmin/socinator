using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.Request;
using QuoraDominatorCore.Response;
using System;
using System.Threading;

namespace QuoraDominatorCore.QdLibrary.Processors.Answers
{
    public class AnswerKeywordProcessor : BaseQuoraProcessor
    {
        private readonly IQdHttpHelper _httpHelper;

        public AnswerKeywordProcessor(IQuoraBrowserManager browser, IQdJobProcess jobProcess,
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
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            jobProcessResult = ScrapeAnswerByKeyword(queryInfo, ref jobProcessResult);
        }

        private JobProcessResult ScrapeAnswerByKeyword(QueryInfo queryInfo, ref JobProcessResult objJobProcessResult)
        {
            try
            {
                var url = $"{QdConstants.HomePageUrl}/search?q=" + queryInfo.QueryValue.Trim() + "&type=question";

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                QuestionByKeywordResponseHandler objResponseHandler = null;
                var IsBrowser = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser;
                if (!IsBrowser)
                {
                    var Response = quoraFunct.SearchQueryByType(QdConstants.GetSearchQueryAPI, Enums.SearchQueryType.Questions, queryInfo.QueryValue, JobProcess.DominatorAccountModel, 0);
                    objResponseHandler = new QuestionByKeywordResponseHandler(Response,IsBrowser);
                }
                else
                {
                    int numOfTimesToCheck = 0;
                    int ScrollCount = 0;
                    LoadAgain:
                    var response = _browser.SearchByCustomUrl(JobProcess.DominatorAccountModel, url);
                    while (numOfTimesToCheck++ <= 5)
                    { 
                        if (response.Response.Contains("find any results for"))
                        {
                        _browser.BrowserWindow.Refresh();
                        Thread.Sleep(TimeSpan.FromSeconds(4));
                        response.Response = _browser.BrowserWindow.GetPageSource();
                        }
                        else
                            break;
                        numOfTimesToCheck++;
                    }
                    IResponseParameter scrollResponse = null;
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    scrollResponse = _browser.SearchByCustomUrlAndScrollDown(JobProcess.DominatorAccountModel);
                    if (response.Response.Contains("find any results for") && ScrollCount++ < 3)
                        goto LoadAgain;
                    objResponseHandler = new QuestionByKeywordResponseHandler(scrollResponse,IsBrowser);
                }
                foreach (var eachQuestion in objResponseHandler.QuestionList)
                {
                    var QuestionId = string.Empty;
                    var Id = string.Empty;
                    if (!IsBrowser)
                        Id = quoraFunct.GetAnswerOfQuestionPaginationId(JobProcess.DominatorAccountModel, string.Empty, eachQuestion.QuestionId).Result;
                    var response =!IsBrowser?quoraFunct.GetUserActivityDetailsByType(string.Empty,Enums.UserActivityType.AnswersOfQuestion,JobProcess.DominatorAccountModel,0,string.Empty,Id).Result :_browser.SearchByCustomUrl(JobProcess.DominatorAccountModel, eachQuestion.QuestionUrl);
                    var ansList = objResponseHandler.GetAnswersLink(response.Response, IsBrowser);
                    objJobProcessResult = FilterAndStartFinalProcessForAnswers(queryInfo, objJobProcessResult,
                        ansList);
                }
                if (objJobProcessResult.IsProcessCompleted)
                    return objJobProcessResult;
                StartPagination(queryInfo, objResponseHandler, ref objJobProcessResult);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally { if (_browser != null && _browser.BrowserWindow != null) _browser.CloseBrowser(); objJobProcessResult.IsProcessSuceessfull = true; }
            return objJobProcessResult;
        }

        private void StartPagination(QueryInfo queryInfo, QuestionByKeywordResponseHandler objResponseHandler, ref JobProcessResult jobProcessResult)
        {
            try
            {
                while(objResponseHandler != null && objResponseHandler.HasMoreResults)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var Response = quoraFunct.SearchQueryByType(QdConstants.GetSearchQueryAPI, Enums.SearchQueryType.Questions, queryInfo.QueryValue, JobProcess.DominatorAccountModel, objResponseHandler.PaginationCount);
                    objResponseHandler = new QuestionByKeywordResponseHandler(Response, false);
                    foreach (var eachQuestion in objResponseHandler.QuestionList)
                    {
                        var QuestionId = string.Empty;
                        var Id = quoraFunct.GetAnswerOfQuestionPaginationId(JobProcess.DominatorAccountModel, string.Empty, eachQuestion.QuestionId).Result;
                        var response = quoraFunct.GetUserActivityDetailsByType(string.Empty, Enums.UserActivityType.AnswersOfQuestion, JobProcess.DominatorAccountModel, 0, string.Empty, Id).Result;
                        var ansList = objResponseHandler.GetAnswersLink(response.Response, false);
                        jobProcessResult = FilterAndStartFinalProcessForAnswers(queryInfo, jobProcessResult,
                            ansList);
                    }
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                }
            }catch (Exception) { }
            finally {
                jobProcessResult.IsProcessCompleted = true;
            }
        }
    }
}