using System;
using System.Collections.Generic;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.Request;
using QuoraDominatorCore.Response;

namespace QuoraDominatorCore.QdLibrary.Processors.Answers
{
    public class AnswerOnQuestionTopicFaqsProcessor : BaseQuoraProcessor
    {
        private IQdHttpHelper httpHelper;
        private readonly AnswerQuestionModel answerQuestionModel;
        public AnswerOnQuestionTopicFaqsProcessor(IQuoraBrowserManager browser, IQdJobProcess jobProcess,
            IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService, IQuoraFunctions objQuoraFunct, IProcessScopeModel processScopeModel, IQdHttpHelper HttpHelper) :
            base(browser, jobProcess, dbAccountService, globalService, campaignService, objQuoraFunct,
                processScopeModel)
        {
            httpHelper = HttpHelper;
            answerQuestionModel=processScopeModel.GetActivitySettingsAs<AnswerQuestionModel>();
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                string url;
                if (queryInfo.QueryValue.Contains(".quora.com"))
                {
                    url = queryInfo.QueryValue;
                    queryInfo.QueryValue = Utilities.GetBetween(queryInfo.QueryValue, "?q=", "&");
                }
                else
                    url = $"{QdConstants.HomePageUrl}/search?q=" + queryInfo.QueryValue + "&type=topic";
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                TopicResponseHandler topicResponseHandler = null;
                TopicQuestionsResponseHandler topicQuestionsResponse=null;
                var questionUrlList = new List<string>();
                var IsBrowser = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser;
                if (IsBrowser)
                {
                    var resp = _browser.SearchByCustomUrl(JobProcess.DominatorAccountModel, url);
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    topicResponseHandler = new TopicResponseHandler(resp,IsBrowser);
                }
                else
                    topicResponseHandler = quoraFunct.Topic(JobProcess.DominatorAccountModel,QdConstants.GetSearchQueryAPI,Enums.SearchQueryType.Topics,queryInfo.QueryValue);
                if (topicResponseHandler != null && topicResponseHandler.TopicsCollection.Count > 0)
                {
                    foreach (var Topic in topicResponseHandler.TopicsCollection)
                    {
                        var TopicResponse = IsBrowser ? _browser.SearchByCustomUrl(JobProcess.DominatorAccountModel, Topic.TopicUrl) :
                            quoraFunct.GetQuestionsFromTopics(Topic.TopicId, JobProcess.DominatorAccountModel).Result;
                        topicQuestionsResponse = new TopicQuestionsResponseHandler(TopicResponse, IsBrowser);
                        if (topicQuestionsResponse != null && topicQuestionsResponse.QuestionsList.Count > 0)
                            topicQuestionsResponse.QuestionsList.ForEach(Question => questionUrlList.Add(Question.QuestionUrl));
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        StartAnswerForEachQuestion(queryInfo, questionUrlList, ref jobProcessResult);
                    }
                    StartPaginations(jobProcessResult, queryInfo, topicResponseHandler,topicQuestionsResponse);
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally
            {
                if (_browser.BrowserWindow != null) _browser.CloseBrowser();
                jobProcessResult.IsProcessCompleted = true;
            }
        }

        private void StartPaginations(JobProcessResult jobProcessResult, QueryInfo queryInfo, TopicResponseHandler topicResponseHandler, TopicQuestionsResponseHandler topicQuestionsResponse)
        {
            while(topicResponseHandler != null && topicResponseHandler.HasMoreResult)
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var questionUrlList = new List<string>();
                topicResponseHandler = quoraFunct.Topic(JobProcess.DominatorAccountModel,QdConstants.GetSearchQueryAPI,Enums.SearchQueryType.Topics, queryInfo.QueryValue, topicResponseHandler.PaginationCount);
                if (topicResponseHandler != null && topicResponseHandler.TopicsCollection.Count > 0)
                {
                    foreach (var Topic in topicResponseHandler.TopicsCollection)
                    {
                        var TopicResponse = quoraFunct.GetQuestionsFromTopics(Topic.TopicId, JobProcess.DominatorAccountModel).Result;
                        topicQuestionsResponse = new TopicQuestionsResponseHandler(TopicResponse, false);
                        if (topicQuestionsResponse != null && topicQuestionsResponse.QuestionsList.Count > 0)
                            topicQuestionsResponse.QuestionsList.ForEach(Question => questionUrlList.Add(Question.QuestionUrl));
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        StartAnswerForEachQuestion(queryInfo, questionUrlList, ref jobProcessResult);
                    }
                }
            }
        }
    }
}