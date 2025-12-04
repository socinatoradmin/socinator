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

namespace QuoraDominatorCore.QdLibrary.Processors.Post
{
    public class PostKeywordProcessor : BaseQuoraProcessor
    {
        public PostKeywordProcessor(IQuoraBrowserManager browser, IQdJobProcess jobProcess,
            IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService, IQuoraFunctions objQuoraFunct, IQdHttpHelper httpHelper,
            IProcessScopeModel processScopeModel) :
            base(browser, jobProcess, dbAccountService, globalService, campaignService, objQuoraFunct,
                processScopeModel)
        {

        }
        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            jobProcessResult = ScrapePostByKeyword(queryInfo, ref jobProcessResult);
        }

        private JobProcessResult ScrapePostByKeyword(QueryInfo queryInfo, ref JobProcessResult objJobProcessResult)
        {
            try
            {
                var url = $"{QdConstants.HomePageUrl}/search?q=" + queryInfo.QueryValue.Trim() + "&type=post";

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                PostByKeywordResponseHandler objResponseHandler = null;
                var IsBrowser = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser;
                if (!IsBrowser)
                {
                    var Response = quoraFunct.SearchQueryByType(QdConstants.GetSearchQueryAPI, Enums.SearchQueryType.Posts, queryInfo.QueryValue, JobProcess.DominatorAccountModel, 0);
                    objResponseHandler = new PostByKeywordResponseHandler(Response, IsBrowser);
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
                    objResponseHandler = new PostByKeywordResponseHandler(scrollResponse, IsBrowser);
                }
                if (!objJobProcessResult.IsProcessCompleted)
                {
                    FilterAndStartProcessForPosts(queryInfo, objJobProcessResult, objResponseHandler.postDetailsList);
                    //StartPagination(queryInfo, objResponseHandler, ref objJobProcessResult);
                }
                
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally
            {
                if (_browser != null && _browser.BrowserWindow != null) _browser.CloseBrowser();
                objJobProcessResult.IsProcessCompleted = true;
            }
            return objJobProcessResult;
        }

        private void StartPagination(QueryInfo queryInfo, PostByKeywordResponseHandler objResponseHandler, ref JobProcessResult jobProcessResult)
        {
            try
            {
                while (objResponseHandler != null && objResponseHandler.HasMoreResults)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var Response = quoraFunct.SearchQueryByType(QdConstants.GetSearchQueryAPI, Enums.SearchQueryType.Posts, queryInfo.QueryValue, JobProcess.DominatorAccountModel, objResponseHandler.PaginationCount);
                    objResponseHandler = new PostByKeywordResponseHandler(Response, false);
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
