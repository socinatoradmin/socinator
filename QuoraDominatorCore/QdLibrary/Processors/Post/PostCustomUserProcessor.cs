using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using QuoraDominatorCore.Enums;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using QuoraDominatorCore.Request;
using QuoraDominatorCore.Response;
using System;

namespace QuoraDominatorCore.QdLibrary.Processors.Post
{
    public class PostCustomUserProcessor : BaseQuoraProcessor
    {
        public PostCustomUserProcessor(IQuoraBrowserManager browser, IQdJobProcess jobProcess,
            IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService, IQuoraFunctions objQuoraFunct, IQdHttpHelper httpHelper,
            IProcessScopeModel processScopeModel) :
            base(browser, jobProcess, dbAccountService, globalService, campaignService, objQuoraFunct,
                processScopeModel)
        {

        }
        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                if (!queryInfo.QueryValue.Contains(".quora.com"))
                    queryInfo.QueryValue = $"https://www.quora.com/profile/" + queryInfo.QueryValue;
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                CustomUserPostResponseHandler objResponseHandler = null;
                IResponseParameter Response = null;
                var IsBrowser = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser;
                if (!IsBrowser)
                    Response = quoraFunct.GetUserActivityDetailsByType(quoraFunct.GetUserId(queryInfo.QueryValue),UserActivityType.Posts,JobProcess.DominatorAccountModel,OrderBy:"most_recent").Result;
                else
                    Response = _browser.SearchByCustomUrl(JobProcess.DominatorAccountModel, queryInfo.QueryValue+"/posts");
                objResponseHandler = new CustomUserPostResponseHandler(Response, IsBrowser);
                FilterAndStartProcessForPosts(queryInfo, jobProcessResult, objResponseHandler.postDetailsList);
                StartPagination(queryInfo, objResponseHandler, ref jobProcessResult);
            }
            catch (Exception)
            {
            }
            finally
            {
                if (_browser != null && _browser.BrowserWindow != null) _browser.CloseBrowser();
                jobProcessResult.IsProcessCompleted = true;
            }
        }
        private void StartPagination(QueryInfo queryInfo, CustomUserPostResponseHandler objResponseHandler, ref JobProcessResult jobProcessResult)
        {
            try
            {
                while (objResponseHandler != null && objResponseHandler.HasMoreResults)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var Response = quoraFunct.GetUserActivityDetailsByType(quoraFunct.GetUserId(queryInfo.QueryValue), UserActivityType.Posts, JobProcess.DominatorAccountModel,objResponseHandler.PaginationCount, OrderBy: "most_recent").Result;
                    objResponseHandler = new CustomUserPostResponseHandler(Response, false);
                    FilterAndStartProcessForPosts(queryInfo, jobProcessResult, objResponseHandler.postDetailsList);
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                }
            }
            catch (Exception) { }
            finally
            {
                jobProcessResult.IsProcessCompleted = true;
            }
        }
    }
}
