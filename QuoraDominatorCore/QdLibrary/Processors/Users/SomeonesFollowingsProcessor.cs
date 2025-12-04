using DominatorHouseCore;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.Enums;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.Response;
using System;

namespace QuoraDominatorCore.QdLibrary.Processors.Users
{
    public class SomeonesFollowingsProcessor : BaseQuoraProcessor
    {
        public SomeonesFollowingsProcessor(IQuoraBrowserManager browser, IQdJobProcess jobProcess,
            IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService, IQuoraFunctions objQuoraFunct, IProcessScopeModel processScopeModel) :
            base(browser, jobProcess, dbAccountService, globalService, campaignService, objQuoraFunct,
                processScopeModel)
        {
        }

        private FollowingsResponseHandler ScrapedFollowing { get; set; }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                var ProfileUrl= queryInfo.QueryValue.Contains("http")?queryInfo.QueryValue:
                    $"{QdConstants.HomePageUrl}/profile/" + queryInfo.QueryValue.Trim();
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var UserId = quoraFunct.GetUserId(ProfileUrl);
                var IsBrowser = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser;
                if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    #region HTTP Automation.
                    var FailedCount = 0;
                    TryAgain:
                    var response = quoraFunct.GetUserActivityDetailsByType(UserId, UserActivityType.ProfileFollowings, JobProcess.DominatorAccountModel, ScrapedFollowing == null ? -1 : ScrapedFollowing.PaginationCount, ScrapedFollowing != null ? "Paginated" : "MostRecent").Result;
                    while (FailedCount++ <= 2 && (response == null || response.HasError || response.Response.Contains("Bad Request")))
                        goto TryAgain;
                    ScrapedFollowing = new FollowingsResponseHandler(response, IsBrowser);
                    #endregion
                }
                else
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    _browser.SearchByCustomUrl(JobProcess.DominatorAccountModel, ProfileUrl);
                    var res = _browser.GetFollwersAndFollowingsFromProfile(JobProcess.DominatorAccountModel, UserFollowTypes.Followings,10);
                    ScrapedFollowing = new FollowingsResponseHandler(res, IsBrowser);
                }
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (ScrapedFollowing.Success && !jobProcessResult.IsProcessCompleted)
                    jobProcessResult = FilterAndStartFinalProcess(queryInfo, jobProcessResult,ScrapedFollowing.FollowingList);
                if (jobProcessResult.IsProcessCompleted)
                    return;
                StartPagination(queryInfo,ScrapedFollowing,ref jobProcessResult,ProfileUrl,UserId);
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
        private void StartPagination(QueryInfo queryInfo, FollowingsResponseHandler followingsResponse, ref JobProcessResult jobProcessResult, string ProfileUrl, string UserId)
        {
            try
            {
                UserId = string.IsNullOrEmpty(UserId) ? quoraFunct.GetUserId(ProfileUrl) : UserId;
                GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    $"Checking For More Results...");
                while (followingsResponse!=null && followingsResponse.HasMoreResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var FailedCount = 0;
                TryAgain:
                    var response = quoraFunct.GetUserActivityDetailsByType(UserId, UserActivityType.ProfileFollowings, JobProcess.DominatorAccountModel, followingsResponse.PaginationCount,"Paginated").Result;
                    while (FailedCount++ <= 2 && (response == null || response.HasError || response.Response.Contains("Bad Request")))
                        goto TryAgain;
                    followingsResponse = new FollowingsResponseHandler(response, false);
                    if (followingsResponse.Success && !jobProcessResult.IsProcessCompleted)
                        jobProcessResult = FilterAndStartFinalProcess(queryInfo, jobProcessResult, followingsResponse.FollowingList);
                    if (jobProcessResult.IsProcessCompleted)
                        return;
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                }
            }
            catch (Exception ex) { ex.DebugLog(); }
            finally { jobProcessResult.IsProcessCompleted = true; }
        }
    }
}