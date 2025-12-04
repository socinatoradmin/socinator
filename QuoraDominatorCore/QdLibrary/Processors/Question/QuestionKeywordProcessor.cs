using DominatorHouseCore;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
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
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Utility;

namespace QuoraDominatorCore.QdLibrary.Processors.Question
{
    public class QuestionKeywordProcessor : BaseQuoraProcessor
    {
        private readonly IQdHttpHelper _httpHelper;
        public QuestionKeywordProcessor(IQuoraBrowserManager browser, IQdJobProcess jobProcess,
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
                ScrapeQuestionByKeyword(queryInfo, ref jobProcessResult);
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
        }

        private void ScrapeQuestionByKeyword(QueryInfo queryinfo, ref JobProcessResult jobProcessResult)
        {
            var url = $"{QdConstants.HomePageUrl}/search?q=" + queryinfo.QueryValue.Replace(" ", "%20") + "&type=question";
            try
            {
                QuestionByKeywordResponseHandler objQuestionByKeywordResponseHandler = null;
                var IsBrowser = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser;
                if (!IsBrowser)
                {
                    var response= quoraFunct.SearchQueryByType(QdConstants.GetSearchQueryAPI, Enums.SearchQueryType.Questions, queryinfo.QueryValue, JobProcess.DominatorAccountModel, 0);
                    objQuestionByKeywordResponseHandler = new QuestionByKeywordResponseHandler(response, IsBrowser);
                }
                else
                {
                    int noOfTimesToCheck = 0;
                    int ScrollCount = 0;
                    LoadAgain:
                    var response = _browser.SearchByCustomUrl(JobProcess.DominatorAccountModel, url);
                    while (noOfTimesToCheck++ <= 5) 
                    {
                        if (response.Response.Contains("find any results for"))
                        {
                            _browser.BrowserWindow.ExecuteScript("document.getElementsByClassName(\'q-flex qu-alignItems--center qu-py--tiny qu-flex--auto qu-overflow--hidden\')[7].click();", 2);
                            Thread.Sleep(TimeSpan.FromSeconds(4));
                            response.Response = _browser.BrowserWindow.GetPageSource();
                        }
                        else
                            break;
                    }
                    var scrollResponse = _browser.SearchByCustomUrlAndScrollDown(JobProcess.DominatorAccountModel);
                    if (response.Response.Contains("find any results for") && ScrollCount++ < 3)
                        goto LoadAgain;
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    objQuestionByKeywordResponseHandler = new QuestionByKeywordResponseHandler(scrollResponse,IsBrowser);
                }
                if (!jobProcessResult.IsProcessCompleted)
                    jobProcessResult = FilterAndStartFinalProcessWithQuestionUrl(queryinfo,
                        objQuestionByKeywordResponseHandler, ref jobProcessResult);
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                StartPagination(queryinfo, objQuestionByKeywordResponseHandler, ref jobProcessResult);
            }
            catch (OperationCanceledException)
            {
                if (_browser.BrowserWindow != null) _browser.CloseBrowser();
                throw new OperationCanceledException();
            }

            jobProcessResult.IsProcessCompleted = true;
        }

        private void StartPagination(QueryInfo queryinfo,
            QuestionByKeywordResponseHandler questionByKeywordResponse,
            ref JobProcessResult jobProcessResult)
        {
            try
            {
                while(questionByKeywordResponse!=null && questionByKeywordResponse.HasMoreResults)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var response = quoraFunct.SearchQueryByType(QdConstants.GetSearchQueryAPI, Enums.SearchQueryType.Questions, queryinfo.QueryValue, JobProcess.DominatorAccountModel, questionByKeywordResponse.PaginationCount);
                    questionByKeywordResponse = new QuestionByKeywordResponseHandler(response,false);
                    if (!jobProcessResult.IsProcessCompleted)
                        jobProcessResult = FilterAndStartFinalProcessWithQuestionUrl(queryinfo,
                            questionByKeywordResponse, ref jobProcessResult);
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                }
            }
            catch (Exception){}
            finally { jobProcessResult.IsProcessCompleted=true; }
        }

        private JobProcessResult FilterAndStartFinalProcessWithQuestionUrl(QueryInfo queryInfo,
            QuestionByKeywordResponseHandler questionByKeywordResponse, ref JobProcessResult jobProcessResult)
        {
            var IsBrowser = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser;
            //var QuestionUrlLists = new List<string>();
            //questionByKeywordResponse.QuestionList.ForEach((x) => QuestionUrlLists.Add(x.QuestionUrl));
            //QuestionUrlLists = FilterBlackListedUser(QuestionUrlLists);
            foreach (var questionDetail in questionByKeywordResponse.QuestionList)
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                try
                {   
                    if(ActivityType== ActivityType.DownvoteQuestions)
                        if (questionDetail.ViewerHasDownvoted)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork, JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Successfully Skipped Already Downvoted Question.");
                            continue;
                        }
                    if(IsBrowser)
                        _browser.SearchByCustomUrl(JobProcess.DominatorAccountModel, questionDetail.QuestionUrl);
                    var objQuestionDetailsResponseHandler =
                        quoraFunct.QuestionDetails(JobProcess.DominatorAccountModel, questionDetail.QuestionUrl);
                    var objQuoraUser = new QuoraUser { Url = objQuestionDetailsResponseHandler.QuestionUrl };
                    MapValues(objQuestionDetailsResponseHandler, questionByKeywordResponse.QuestionList, questionDetail.QuestionUrl);
                    try
                    {
                        if (DbAccountService.GetInteractedQuestion().Count(x => x.QuestionUrl == objQuoraUser.Url) != 0)
                            continue;
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    FilterAndStartFinalProcessForEachQuestionUrl(queryInfo, objQuoraUser,
                        objQuestionDetailsResponseHandler, out jobProcessResult);
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                finally { if(_browser!=null && _browser.BrowserWindow!=null)_browser.CloseBrowser(); }
                if (jobProcessResult.IsProcessCompleted)
                    break;
            }

            return jobProcessResult;
        }

        private void MapValues(QuestionDetailsResponseHandler objQuestionDetailsResponseHandler, List<QuestionDetails> question_Details,string QuestionUrl)
        {
            try
            {
                var QuestionDetails = question_Details.FirstOrDefault(x => x.QuestionUrl == QuestionUrl || QuestionUrl.Contains(x.QuestionUrl));
                int.TryParse(QuestionDetails.FollowCount, out objQuestionDetailsResponseHandler.FollowCount);
                int.TryParse(QuestionDetails.AnswerCount, out objQuestionDetailsResponseHandler.AnswerCount);
            }
            catch { }
        }
    }
}