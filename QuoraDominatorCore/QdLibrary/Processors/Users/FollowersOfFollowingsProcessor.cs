using System;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using QuoraDominatorCore.Enums;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.Response;

namespace QuoraDominatorCore.QdLibrary.Processors.Users
{
    public class FollowersOfFollowingsProcessor : BaseQuoraProcessor
    {
        public FollowersOfFollowingsProcessor(IQuoraBrowserManager browser, IQdJobProcess jobProcess,
            IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService, IQuoraFunctions objQuoraFunct, IProcessScopeModel processScopeModel) :
            base(browser, jobProcess, dbAccountService, globalService, campaignService, objQuoraFunct,
                processScopeModel)
        {
        }

        private FollowingsResponseHandler ScrapedFollowing { get; set; }

        private FollowerResponseHandler ScrapedFollowers { get; set; }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                var url = queryInfo.QueryValue.Contains("http") ? queryInfo.QueryValue : $"{QdConstants.HomePageUrl}/profile/{queryInfo.QueryValue.TrimEnd()}";
                var IsBrowser = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser;
                if (!IsBrowser)
                {
                    #region HTTP Automation.
                    //Getting Someone's Following.
                    var FailedCount = 0;
                    ReTry:
                    var scrollResponse = quoraFunct.GetUserActivityDetailsByType(quoraFunct.GetUserId(url), UserActivityType.ProfileFollowings, JobProcess.DominatorAccountModel,- 1,"MostRecent").Result;
                    while(FailedCount++<=2 && scrollResponse!=null && scrollResponse.HasError)
                        goto ReTry;
                    ScrapedFollowing = new FollowingsResponseHandler(scrollResponse,IsBrowser);
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (ScrapedFollowing.Success && ScrapedFollowing.FollowingList.Count > 0)
                    {
                        //Getting Followers of Following.
                        foreach(var following in ScrapedFollowing.FollowingList)
                        {
                            var ProfileUrl = $"{QdConstants.HomePageUrl}/profile/{following}";
                            FailedCount = 0;
                            var UserId = quoraFunct.GetUserId(ProfileUrl);
                        TryAgain:
                            scrollResponse = quoraFunct.GetUserActivityDetailsByType(UserId, UserActivityType.ProfileFollowers, JobProcess.DominatorAccountModel, -1, "MostRecent").Result;
                            while (FailedCount++ <= 2 && scrollResponse != null && scrollResponse.HasError)
                                goto TryAgain;
                            ScrapedFollowers = new FollowerResponseHandler(scrollResponse, IsBrowser);
                            if(ScrapedFollowers.Success && ScrapedFollowers.FollowerList.Count > 0)
                                jobProcessResult = FilterAndStartFinalProcess(queryInfo, jobProcessResult,
                                    ScrapedFollowers.FollowerList);
                            if (jobProcessResult.IsProcessCompleted)
                                return;
                            StartPaginationForFollowers(queryInfo,UserId,ScrapedFollowers,ref jobProcessResult);
                        }
                    }
                    #endregion
                }
                else
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    _browser.SearchByCustomUrl(JobProcess.DominatorAccountModel, url);
                    var followingsResponse = _browser.GetFollwersAndFollowingsFromProfile(JobProcess.DominatorAccountModel, UserFollowTypes.Followings);
                    ScrapedFollowing = new FollowingsResponseHandler(followingsResponse,IsBrowser);
                    foreach(var eachFollowing in ScrapedFollowing.FollowingList)
                    {
                        _browser.SearchByCustomUser(JobProcess.DominatorAccountModel, eachFollowing);
                        var followersResponse = _browser.GetFollwersAndFollowingsFromProfile(JobProcess.DominatorAccountModel, UserFollowTypes.Followers);
                        ScrapedFollowers = new FollowerResponseHandler(followersResponse,IsBrowser);
                        if(ScrapedFollowers.Success)
                            jobProcessResult = FilterAndStartFinalProcess(queryInfo, jobProcessResult, ScrapedFollowers.FollowerList);
                        if (jobProcessResult.IsProcessCompleted)
                            return;
                        StartPaginationForFollowers(queryInfo,quoraFunct.GetUserId($"{QdConstants.HomePageUrl}/profile/{eachFollowing}"), ScrapedFollowers, ref jobProcessResult);
                    }
                }
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
        private void StartPaginationForFollowers(QueryInfo queryInfo, string userId,FollowerResponseHandler scrapedFollowers, ref JobProcessResult jobProcessResult)
        {
            while(scrapedFollowers !=null && scrapedFollowers.HasMoreResult)
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var FailedCount = 0;
            TryAgain:
                var scrollResponse = quoraFunct.GetUserActivityDetailsByType(userId, UserActivityType.ProfileFollowers, JobProcess.DominatorAccountModel,ScrapedFollowers.PaginationCount,"Paginated").Result;
                while (FailedCount++ <= 2 && scrollResponse != null && scrollResponse.HasError)
                    goto TryAgain;
                scrapedFollowers = new FollowerResponseHandler(scrollResponse, false);
                if (scrapedFollowers.Success && scrapedFollowers.FollowerList.Count > 0)
                    jobProcessResult = FilterAndStartFinalProcess(queryInfo, jobProcessResult,
                        scrapedFollowers.FollowerList);
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            }
        }
    }
}