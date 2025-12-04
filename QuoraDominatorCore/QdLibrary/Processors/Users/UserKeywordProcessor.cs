using DominatorHouseCore;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.Response;
using System;
using System.Threading;

namespace QuoraDominatorCore.QdLibrary.Processors.Users
{
    public class UserKeywordProcessor : BaseQuoraProcessor
    {
        public UserKeywordProcessor(IQuoraBrowserManager browser, IQdJobProcess jobProcess,
            IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService, IQuoraFunctions quoraFunctions, IProcessScopeModel processScopeModel) :
            base(browser, jobProcess, dbAccountService, globalService, campaignService, quoraFunctions,
                processScopeModel)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                UserNameByKeywordResponseHandler scrapedUserName=null;
                var url = $"{QdConstants.HomePageUrl}/search?q=" + queryInfo.QueryValue.TrimEnd() + "&type=profile";
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var IsBrowser = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser;
                if (!IsBrowser)
                {
                    scrapedUserName = quoraFunct.UserNameByKeyword(JobProcess.DominatorAccountModel,queryInfo.QueryValue.TrimEnd(),scrapedUserName ==null ?-1:scrapedUserName.PaginationCount,IsBrowser);
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
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
                            _browser.BrowserWindow.Refresh();
                            Thread.Sleep(TimeSpan.FromSeconds(4));
                            response.Response = _browser.BrowserWindow.GetPageSource();
                        }
                        else
                            break;
                    }
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var scrollResponse = _browser.SearchByCustomUrlAndScrollDown(JobProcess.DominatorAccountModel);
                    if (response.Response.Contains("find any results for") && ScrollCount++ < 3)
                        goto LoadAgain;
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    scrapedUserName = new UserNameByKeywordResponseHandler(scrollResponse,IsBrowser);
                }
                if (scrapedUserName.Success && !jobProcessResult.IsProcessCompleted)
                    jobProcessResult = FilterAndStartFinalProcess(queryInfo, jobProcessResult, scrapedUserName.UserList);
                if (jobProcessResult.IsProcessCompleted)
                    return;
                StartPagination(queryInfo, scrapedUserName,ref jobProcessResult);
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

        private void StartPagination(QueryInfo queryInfo, UserNameByKeywordResponseHandler scrapedUserName, ref JobProcessResult jobProcessResult)
        {
            try
            {
                while(scrapedUserName!=null && scrapedUserName.HasMoreResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    scrapedUserName = quoraFunct.UserNameByKeyword(JobProcess.DominatorAccountModel, queryInfo.QueryValue.TrimEnd(),scrapedUserName.PaginationCount, false);
                    if (scrapedUserName.Success && !jobProcessResult.IsProcessCompleted)
                        jobProcessResult = FilterAndStartFinalProcess(queryInfo, jobProcessResult, scrapedUserName.UserList);
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                }
            }catch(Exception ex) { ex.DebugLog(); }
            finally { jobProcessResult.IsProcessCompleted = true; }
        }
    }
}