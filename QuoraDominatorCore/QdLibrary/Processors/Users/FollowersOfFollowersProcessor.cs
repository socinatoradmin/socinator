using System;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using QuoraDominatorCore.Enums;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using QuoraDominatorCore.Response;
using QuoraDominatorCore.QdUtility;

namespace QuoraDominatorCore.QdLibrary.Processors.Users
{
    public class FollowersOfFollowersProcessor : BaseQuoraProcessor
    {
        public FollowersOfFollowersProcessor(IQuoraBrowserManager browser, IQdJobProcess jobProcess,
            IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService, IQuoraFunctions objQuoraFunct, IProcessScopeModel processScopeModel) :
            base(browser, jobProcess, dbAccountService, globalService, campaignService, objQuoraFunct,
                processScopeModel)
        {
        }

        private FollowerResponseHandler ScrapedFollowers { get; set; }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                var url=queryInfo.QueryValue.Contains("http")?queryInfo.QueryValue: $"{QdConstants.HomePageUrl}/profile/{queryInfo.QueryValue.Trim()}";
                var IsBrowser = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser;
                if (!IsBrowser)
                {
                    #region HTTP Automation.
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var FailedCount = 0;
                TryAgain:
                    var scrollResponse = quoraFunct.GetUserActivityDetailsByType(quoraFunct.GetUserId(url), UserActivityType.ProfileFollowers, JobProcess.DominatorAccountModel, ScrapedFollowers == null ? -1 : ScrapedFollowers.PaginationCount, ScrapedFollowers == null ? "MostRecent" : "Paginated").Result;
                    while (FailedCount++ <= 3 && (scrollResponse == null || scrollResponse.HasError || scrollResponse.Response.Contains("Bad Request")))
                        goto TryAgain;
                    ScrapedFollowers = new FollowerResponseHandler(scrollResponse,IsBrowser);
                    if(ScrapedFollowers.Success && ScrapedFollowers.FollowerList.Count > 0)
                    {
                        foreach(var Follower in ScrapedFollowers.FollowerList)
                        {
                            var ProfileUrl = $"{QdConstants.HomePageUrl}/profile/{Follower}";
                            FailedCount = 0;
                            var UserId = quoraFunct.GetUserId(url);
                        TryGetValue:
                            scrollResponse = quoraFunct.GetUserActivityDetailsByType(UserId, UserActivityType.ProfileFollowers, JobProcess.DominatorAccountModel,-1,"MostRecent").Result;
                            while (FailedCount++ <= 3 && (scrollResponse == null || scrollResponse.HasError || scrollResponse.Response.Contains("Bad Request")))
                                goto TryGetValue;
                            var followerResponse = new FollowerResponseHandler(scrollResponse,IsBrowser);
                            if (ScrapedFollowers.Success)
                                jobProcessResult = FilterAndStartFinalProcess(queryInfo, jobProcessResult,
                                    followerResponse.FollowerList);
                            if (jobProcessResult.IsProcessCompleted)
                                return;
                            StartPaginationForFollowers(queryInfo,UserId, followerResponse,ref jobProcessResult);
                        }
                    }
                    #endregion
                }
                else
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    _browser.SearchByCustomUrl(JobProcess.DominatorAccountModel, url);
                    var someonesFollowersResponse = _browser.GetFollwersAndFollowingsFromProfile(JobProcess.DominatorAccountModel, UserFollowTypes.Followers);
                    ScrapedFollowers = new FollowerResponseHandler(someonesFollowersResponse,IsBrowser);
                    foreach (var eachFollower in ScrapedFollowers.FollowerList)
                    {
                        _browser.SearchByCustomUser(JobProcess.DominatorAccountModel, eachFollower);
                        var followersResponse = _browser.GetFollwersAndFollowingsFromProfile(JobProcess.DominatorAccountModel, UserFollowTypes.Followers);
                        var scrapedFollowers = new FollowerResponseHandler(followersResponse,IsBrowser);
                        if (scrapedFollowers.Success)
                            jobProcessResult = FilterAndStartFinalProcess(queryInfo, jobProcessResult, scrapedFollowers.FollowerList);
                        if (jobProcessResult.IsProcessCompleted)
                            return;
                        StartPaginationForFollowers(queryInfo,quoraFunct.GetUserId($"{QdConstants.HomePageUrl}/profile/{eachFollower}"), scrapedFollowers, ref jobProcessResult);
                    }
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

        private void StartPaginationForFollowers(QueryInfo queryInfo, string userId, FollowerResponseHandler followerResponse, ref JobProcessResult jobProcessResult)
        {
            while(followerResponse != null && followerResponse.HasMoreResult)
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var FailedCount = 0;
            TryGetValue:
                var scrollResponse = quoraFunct.GetUserActivityDetailsByType(userId, UserActivityType.ProfileFollowers, JobProcess.DominatorAccountModel,followerResponse.PaginationCount,"Paginated").Result;
                while (FailedCount++ <= 3 && (scrollResponse == null || scrollResponse.HasError || scrollResponse.Response.Contains("Bad Request")))
                    goto TryGetValue;
                followerResponse = new FollowerResponseHandler(scrollResponse, false);
                if (ScrapedFollowers.Success)
                    jobProcessResult = FilterAndStartFinalProcess(queryInfo, jobProcessResult,
                        followerResponse.FollowerList);
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            }
        }
    }
}