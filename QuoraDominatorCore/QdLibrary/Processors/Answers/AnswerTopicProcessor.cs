using System;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using QuoraDominatorCore.Request;
using QuoraDominatorCore.Response;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.Enums;
using DominatorHouseCore.LogHelper;

namespace QuoraDominatorCore.QdLibrary.Processors.Answers
{
    public class AnswerTopicProcessor : BaseQuoraProcessor
    {
        private readonly IQdHttpHelper _httpHelper;
        public AnswerTopicProcessor(IQuoraBrowserManager browser, IQdJobProcess jobProcess,
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
            try
            {
                ScrapeAnswerByTopic(queryinfo, ref jobProcessResult);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception)
            {
            }
            finally { if (_browser.BrowserWindow != null) _browser.CloseBrowser(); }
        }

        private void ScrapeAnswerByTopic(QueryInfo queryinfo, ref JobProcessResult objJobProcessResult)
        {
            string url;
            if (queryinfo.QueryValue.Contains(".quora.com"))
            {
                url = queryinfo.QueryValue;
                queryinfo.QueryValue = Utilities.GetBetween(queryinfo.QueryValue, "?q=", "&");
            }
            else
                url = $"{QdConstants.HomePageUrl}/search?q=" + queryinfo.QueryValue + "&type=topic";
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            var IsBrowser = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser;
            TopicResponseHandler topicNamesResponseHandler;
            if (IsBrowser)
            {
                var responseFollowerFirstPage = _browser.TryAndGetResponse(url, "We couldn't find any results for", 4, JobProcess.DominatorAccountModel);
                topicNamesResponseHandler = new TopicResponseHandler(responseFollowerFirstPage,IsBrowser);
            }
            else
                topicNamesResponseHandler = quoraFunct.Topic(JobProcess.DominatorAccountModel, QdConstants.GetSearchQueryAPI, SearchQueryType.Topics, queryinfo.QueryValue, -1);
            foreach (var Topic in topicNamesResponseHandler.TopicsCollection)
            {
                var eachTopicResponse =!IsBrowser? quoraFunct.GetUserActivityDetailsByType(string.Empty,UserActivityType.TopicAnswer,JobProcess.DominatorAccountModel,-1,"MostRecent",Topic.TopicId).Result : _browser.SearchByCustomUrl(JobProcess.DominatorAccountModel, Topic.TopicUrl.EndsWith("/")? Topic.TopicUrl: Topic.TopicUrl+ "/");
                var answerLinkTopicResponseHandler = new AnswerByTopicResponseHandler(eachTopicResponse,IsBrowser);
                if (!objJobProcessResult.IsProcessCompleted && answerLinkTopicResponseHandler.AnswerList.Count > 0)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    objJobProcessResult = FilterAndStartFinalProcessForAnswers(queryinfo, objJobProcessResult,answerLinkTopicResponseHandler.AnswerList);
                }
            }
            if (objJobProcessResult.IsProcessCompleted) return;
            StartPagination(queryinfo,topicNamesResponseHandler,ref objJobProcessResult, queryinfo.QueryValue);
        }
        private void StartPagination(QueryInfo queryInfo, TopicResponseHandler topicResponseHandler,ref JobProcessResult jobProcessResult,string queryValue)
        {
            try
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork, JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                            $"Checking For More Result...");
                while (topicResponseHandler != null && topicResponseHandler.HasMoreResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    topicResponseHandler = quoraFunct.Topic(JobProcess.DominatorAccountModel, QdConstants.GetSearchQueryAPI, SearchQueryType.Topics, queryValue,topicResponseHandler.PaginationCount);
                    foreach (var Topic in topicResponseHandler.TopicsCollection)
                    {
                        var eachTopicResponse = quoraFunct.GetUserActivityDetailsByType(string.Empty, UserActivityType.TopicAnswer, JobProcess.DominatorAccountModel, -1, "MostRecent", Topic.TopicId).Result;
                        var answerLinkTopicResponseHandler = new AnswerByTopicResponseHandler(eachTopicResponse, false);
                        if (!jobProcessResult.IsProcessCompleted && answerLinkTopicResponseHandler.AnswerList.Count > 0)
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            jobProcessResult = FilterAndStartFinalProcessForAnswers(queryInfo, jobProcessResult, answerLinkTopicResponseHandler.AnswerList);
                        }
                    }
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }finally {
            jobProcessResult.IsProcessCompleted = true;
            }
        }
    }
}