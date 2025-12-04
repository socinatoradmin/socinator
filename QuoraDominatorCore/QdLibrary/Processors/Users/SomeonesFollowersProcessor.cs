using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.FileManagers;
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
    public class SomeonesFollowersProcessor : BaseQuoraProcessor
    {
        public SomeonesFollowersProcessor(IQuoraBrowserManager browser, IQdJobProcess jobProcess,
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
                var ProfileUrl = queryInfo.QueryValue.Contains("http") ?
                    queryInfo.QueryValue : $"{QdConstants.HomePageUrl}/profile/" + queryInfo.QueryValue.Trim().Replace(" ", "-");
                var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                genericFileManager.GetModel<QuoraModel>(ConstantVariable.GetOtherQuoraSettingsFile());
                var IsBrowser = JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser;
                var UserId = !IsBrowser ?  quoraFunct.GetUserId(ProfileUrl):string.Empty;
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                
                if (!IsBrowser)
                {
                    var FailedCount = 0;
                TryAgain:
                    var response = quoraFunct.GetUserActivityDetailsByType(UserId, UserActivityType.ProfileFollowers, JobProcess.DominatorAccountModel, ScrapedFollowers == null ? -1 : ScrapedFollowers.PaginationCount, ScrapedFollowers != null ? "Paginated" : "MostRecent").Result;
                    while (FailedCount++ <= 2 && (response == null || response.HasError || response.Response.Contains("Bad Request")))
                        goto TryAgain;
                    ScrapedFollowers = new FollowerResponseHandler(response, IsBrowser);
                }
                else
                {
                    _browser.SearchByCustomUrl(JobProcess.DominatorAccountModel, ProfileUrl);
                    var followerResponse = _browser.GetFollwersAndFollowingsFromProfile(JobProcess.DominatorAccountModel, UserFollowTypes.Followers,10);
                    ScrapedFollowers = new FollowerResponseHandler(followerResponse, IsBrowser);
                }
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (ScrapedFollowers.Success && !jobProcessResult.IsProcessCompleted)
                    jobProcessResult = FilterAndStartFinalProcess(queryInfo, jobProcessResult,ScrapedFollowers.FollowerList);
                if (jobProcessResult.IsProcessCompleted)
                    return;
                StartPagination(queryInfo, ScrapedFollowers, ref jobProcessResult, ProfileUrl,UserId);
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

        private void StartPagination(QueryInfo queryInfo, FollowerResponseHandler followerResponseHandler, ref JobProcessResult jobProcessResult, string UserProfileUrl,string UserId)
        {
            try
            {
                UserId = string.IsNullOrEmpty(UserId) ? quoraFunct.GetUserId(UserProfileUrl) : UserId;
                GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    $"Checking For More Results...");
                while (followerResponseHandler != null && followerResponseHandler.HasMoreResult)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var FailedCount = 0;
                TryAgain:
                    var response = quoraFunct.GetUserActivityDetailsByType(UserId, UserActivityType.ProfileFollowers, JobProcess.DominatorAccountModel, followerResponseHandler.PaginationCount,"Paginated").Result;
                    while (FailedCount++ <= 2 && (response == null || response.HasError || response.Response.Contains("Bad Request")))
                        goto TryAgain;
                    followerResponseHandler = new FollowerResponseHandler(response, false);
                    if (followerResponseHandler.Success && !jobProcessResult.IsProcessCompleted)
                        jobProcessResult = FilterAndStartFinalProcess(queryInfo, jobProcessResult, followerResponseHandler.FollowerList);
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