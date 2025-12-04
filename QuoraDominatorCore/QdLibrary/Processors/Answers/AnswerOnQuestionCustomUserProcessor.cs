using DominatorHouseCore;
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
using System.Linq;

namespace QuoraDominatorCore.QdLibrary.Processors.Answers
{
    public class AnswerOnQuestionCustomUserProcessor : BaseQuoraProcessor
    {
        private readonly IQdHttpHelper _httpHelper;
        private AnswerQuestionModel answerQuestionModel;
        public AnswerOnQuestionCustomUserProcessor(IQuoraBrowserManager browser, IQdJobProcess jobProcess,
            IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService, IQuoraFunctions objQuoraFunct, IQdHttpHelper httpHelper,
            IProcessScopeModel processScopeModel) :
            base(browser, jobProcess, dbAccountService, globalService, campaignService, objQuoraFunct,
                processScopeModel)
        {
            _httpHelper = httpHelper;
            answerQuestionModel = processScopeModel.GetActivitySettingsAs<AnswerQuestionModel>();
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                string url;
                if (queryInfo.QueryValue.Contains("http"))
                    url =queryInfo.QueryValue.EndsWith("questions")? queryInfo.QueryValue:queryInfo.QueryValue+"/questions";
                else
                    url = $"{QdConstants.HomePageUrl}/profile/" + queryInfo.QueryValue.Trim() + "/questions";
                var IsFiltered = FilterBlackListedUser(queryInfo.QueryValue);
                if (IsFiltered)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType, "Skipped BlackListed User");
                    return;
                }
                UserQuestionResponseHandler userQuestionsResponseHandler = null;
                var IsBrowser = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser;
                if (IsBrowser)
                {
                    var response = _browser.SearchByCustomUrl(JobProcess.DominatorAccountModel, url);
                    var totalQuestions = Utilities.GetBetween(response.Response, "\\\"numProfileQuestions\\\":", ",");
                    var noOfTimesToScroll = int.Parse(totalQuestions) / 15;
                    noOfTimesToScroll = noOfTimesToScroll > 7 ? 5 : noOfTimesToScroll;
                    for (int i = 0; i <= noOfTimesToScroll; i++)
                    {
                        var ScrollResponse = _browser.SearchByCustomUrlAndScrollDown(JobProcess.DominatorAccountModel);
                        userQuestionsResponseHandler = new UserQuestionResponseHandler(ScrollResponse);
                    }
                }
                else
                    userQuestionsResponseHandler = quoraFunct.UserQuestion(JobProcess.DominatorAccountModel, url,userQuestionsResponseHandler==null ? -1:userQuestionsResponseHandler.PaginationCount);
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                StartAnswerForEachQuestion(queryInfo, userQuestionsResponseHandler.QuestionUrl, ref jobProcessResult);
                if (jobProcessResult.IsProcessCompleted)
                    return;
                StartPagination(queryInfo,url,ref jobProcessResult,userQuestionsResponseHandler);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally { if (_browser==null?false:_browser.BrowserWindow==null?false:_browser.BrowserWindow!=null) _browser.CloseBrowser(); }
        }

        private bool FilterBlackListedUser(string user)
        {
            var IsFiltered = false;
            var FilteredPrivateBlackListed = false;
            var FilteredGroupBlackListed = false;
            try
            {
                if (answerQuestionModel.IsChkAnswerOnQuestionSkipPrivateBlacklist)
                    FilteredGroupBlackListed = Blacklistuser.Any(x => x == user || x.Contains(user));
                if (answerQuestionModel.IsChkAnswerOnQuestionSkipGroupBlacklist)
                    FilteredPrivateBlackListed = PrivateBlacklistedUser.Any(y => y == user || y.Contains(user));
                IsFiltered = FilteredGroupBlackListed && FilteredPrivateBlackListed || (FilteredPrivateBlackListed || FilteredGroupBlackListed);
            }
            catch { }
            return IsFiltered;
        }
        private void StartPagination(QueryInfo queryInfo, string UserQuestionUrl,ref JobProcessResult jobProcessResult, UserQuestionResponseHandler questionResponseHandler)
        {
            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork, JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                            $"Checking For More Result...");
                while (questionResponseHandler != null && questionResponseHandler.HasMoreResults)
                {
                    questionResponseHandler = quoraFunct.UserQuestion(JobProcess.DominatorAccountModel, UserQuestionUrl, questionResponseHandler.PaginationCount);
                    StartAnswerForEachQuestion(queryInfo, questionResponseHandler.QuestionUrl, ref jobProcessResult);
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                }
            }catch(Exception ex) {ex.DebugLog(ex.Message); }
            finally {jobProcessResult.IsProcessCompleted = true;}
        }
    }
}