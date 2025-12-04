using System;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using QuoraDominatorCore.Response;
using System.Collections.Generic;
using QuoraDominatorCore.QdUtility;
using DominatorHouseCore.Utility;

namespace QuoraDominatorCore.QdLibrary.Processors.Question
{
    public class QuestionTopicProcessor : BaseQuoraProcessor
    {
        public QuestionTopicProcessor(IQuoraBrowserManager browser, IQdJobProcess jobProcess,
            IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService, IQuoraFunctions objQuoraFunct, IProcessScopeModel processScopeModel) :
            base(browser, jobProcess, dbAccountService, globalService, campaignService, objQuoraFunct,
                processScopeModel)
        {
        }

        protected override void Process(QueryInfo queryinfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                if (queryinfo.QueryValue.Contains("http"))
                    queryinfo.QueryValue = Utilities.GetBetween(queryinfo?.QueryValue, "?q=", "&");
                TopicResponseHandler topicResponseHandler = null;
                TopicQuestionsResponseHandler topicQuestionsResponse = null;
                var questionLinks = new List<string>();
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var IsBrowser = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser;
                if (IsBrowser)
                {
                    var url = $"{QdConstants.HomePageUrl}/search?q={queryinfo.QueryValue}&type=topic";
                    var resp = _browser.SearchByCustomUrl(JobProcess.DominatorAccountModel, url);
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    topicResponseHandler = new TopicResponseHandler(resp,IsBrowser);
                }
                else
                    topicResponseHandler = quoraFunct.Topic(JobProcess.DominatorAccountModel,QdConstants.GetSearchQueryAPI,Enums.SearchQueryType.Topics, queryinfo.QueryValue);
                foreach (var topic in topicResponseHandler.TopicsCollection)
                {
                    var topicResponse =IsBrowser?_browser.SearchByCustomUrl(JobProcess.DominatorAccountModel, topic.TopicUrl):
                        quoraFunct.GetQuestionsFromTopics(topic.TopicId, JobProcess.DominatorAccountModel).Result;
                    topicQuestionsResponse = new TopicQuestionsResponseHandler(topicResponse,IsBrowser);
                    if (topicQuestionsResponse != null && topicQuestionsResponse.QuestionsList.Count > 0)
                        topicQuestionsResponse.QuestionsList.ForEach(Question => questionLinks.Add(Question.QuestionUrl));
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    FilterAndStartFinalProcessWithQuestionUrl(queryinfo, jobProcessResult,
                        questionLinks);
                }
                jobProcessResult.IsProcessCompleted = true;
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
    }
}