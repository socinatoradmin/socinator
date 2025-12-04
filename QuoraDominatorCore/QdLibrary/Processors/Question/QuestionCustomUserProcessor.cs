using System;
using System.Collections.Generic;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Request;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.Request;
using QuoraDominatorCore.Response;

namespace QuoraDominatorCore.QdLibrary.Processors.Question
{
    public class QuestionCustomUserProcessor : BaseQuoraProcessor
    {
        private readonly IQdHttpHelper _httpHelper;
        public QuestionCustomUserProcessor(IQuoraBrowserManager browser, IQdJobProcess jobProcess,
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
            var objJobProcessResult = new JobProcessResult();
            string url;
            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (queryinfo.QueryValue.Contains("http") && queryinfo.QueryValue.Contains("questions"))
                    url = queryinfo.QueryValue;
                else if (queryinfo.QueryValue.Contains("http") && !queryinfo.QueryValue.Contains("questions"))
                    url =queryinfo.QueryValue.EndsWith("/")? queryinfo.QueryValue + "questions":queryinfo.QueryValue+ "/questions";
                else
                    url = $"{QdConstants.HomePageUrl}/profile/" + queryinfo.QueryValue.Trim() + "/questions";
                var IsBrowser = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser;
                if(FilterBlackListedUser(new List<string>() { queryinfo.QueryValue.Replace("/questions", "") }).Count == 0)return;
                UserQuestionResponseHandler objUserQuestionResponseHandler;
                var noOfTimesToScroll = 0;
                if (!IsBrowser)
                {
                    var SearchFailedCount = 0;
                TryAgain:
                    objUserQuestionResponseHandler = quoraFunct.UserQuestion(JobProcess.DominatorAccountModel, url,-1);
                    while (SearchFailedCount++ <= 2 && (objUserQuestionResponseHandler == null || objUserQuestionResponseHandler.QuestionUrl.Count == 0))
                        goto TryAgain;
                }
                else
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var resp = _browser.SearchByCustomUrl(JobProcess.DominatorAccountModel, url);
                    var userInfoResponseHandler = new UserInfoResponseHandler(resp);
                    noOfTimesToScroll = userInfoResponseHandler.UserQuestionsCount / 9;
                    noOfTimesToScroll = noOfTimesToScroll > 5 ? 5 : noOfTimesToScroll;
                    for (int i = 0; i < noOfTimesToScroll; i++)
                        resp = _browser.SearchByCustomUrlAndScrollDown(JobProcess.DominatorAccountModel);
                    objUserQuestionResponseHandler = new UserQuestionResponseHandler(resp);
               }
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (!objJobProcessResult.IsProcessCompleted)
                    objJobProcessResult = FilterAndStartFinalProcessWithQuestionUrl(queryinfo, objJobProcessResult,
                        objUserQuestionResponseHandler.QuestionUrl);
                if (objJobProcessResult.IsProcessCompleted || objUserQuestionResponseHandler.QuestionUrl.Count==0)
                    return;
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                StartPagination(queryinfo,objUserQuestionResponseHandler, noOfTimesToScroll,IsBrowser, ref objJobProcessResult,url);
                objJobProcessResult.IsProcessCompleted = true;
            }
            catch (OperationCanceledException)
            {
                if (_browser.BrowserWindow != null) _browser.CloseBrowser();
                throw new OperationCanceledException();
            }
            catch (Exception)
            {
            }
        }

        private void StartPagination(QueryInfo queryInfo, UserQuestionResponseHandler questionResponseHandler,int QuestionCount,bool IsBrowser,
            ref JobProcessResult objJobProcessResult, string DestinationUrl)
        {
            #region NEW Pagination Logic.
            try
            {
                IResponseParameter resp=new ResponseParameter();
                UserQuestionResponseHandler objUserQuestionResponseHandler;
                while (questionResponseHandler != null && questionResponseHandler.HasMoreResults)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (!IsBrowser)
                    {
                        var SearchFailedCount = 0;
                    TryAgain:
                        objUserQuestionResponseHandler = quoraFunct.UserQuestion(JobProcess.DominatorAccountModel, DestinationUrl, questionResponseHandler.PaginationCount);
                        while (SearchFailedCount++ <= 2 && (objUserQuestionResponseHandler == null || objUserQuestionResponseHandler.QuestionUrl.Count == 0))
                            goto TryAgain;
                    }
                    else
                    {
                        if (QuestionCount == 0 || QuestionCount <= 5)
                            break;
                        for (int i = 0; i < QuestionCount; i++)
                            resp = _browser.SearchByCustomUrlAndScrollDown(JobProcess.DominatorAccountModel);
                        QuestionCount /= 5;
                        objUserQuestionResponseHandler = new UserQuestionResponseHandler(resp);
                    }
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (!objJobProcessResult.IsProcessCompleted)
                        objJobProcessResult = FilterAndStartFinalProcessWithQuestionUrl(queryInfo, objJobProcessResult,
                            objUserQuestionResponseHandler.QuestionUrl);
                    if (objJobProcessResult.IsProcessCompleted || objUserQuestionResponseHandler.QuestionUrl.Count == 0)
                        break;
                }
            }
            catch(Exception ex)
            {
                ex.DebugLog(ex.Message);
            }
            objJobProcessResult.IsProcessCompleted = true;
            
            #endregion
        }
    }
}