using DominatorHouseCore.DatabaseHandler.TumblrTables.Account;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TumblrFactory;
using TumblrDominatorCore.TumblrLibrary.DAL;
using TumblrDominatorCore.TumblrRequest;
using TumblrDominatorCore.TumblrResponseHandler;
using InteractedUser = DominatorHouseCore.DatabaseHandler.TumblrTables.Campaign.InteractedUser;

namespace TumblrDominatorCore.TumblrLibrary.TumblrProcesses
{
    public class UserScraperJobProcess : TumblrJobProcessInteracted<InteractedPosts>
    {
        public UserScraperJobProcess(IProcessScopeModel processScopeModel, IDbAccountServiceScoped _accountService,
            IDbGlobalService _dbGlobalService, IExecutionLimitsManager executionLimitsManager,
            ITumblrQueryScraperFactory queryScraperFactory, ITumblrHttpHelper _httpHelper,
            ITumblrLoginProcess _tumblrLoginProcess) :
            base(processScopeModel, _accountService, _dbGlobalService, executionLimitsManager, queryScraperFactory,
                _httpHelper, _tumblrLoginProcess)
        {
            UserScraperModel = processScopeModel.GetActivitySettingsAs<UserScraperModel>();
        }

        public UserScraperModel UserScraperModel { get; set; }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResultNew)
        {
            var status = "";
            var isUserDetail = false;
            var scrapeResult = (TumblrScrapeResult)scrapeResultNew;
            var tumblrUser = (TumblrUser)scrapeResult.ResultUser;
            var jobProcessResult = new JobProcessResult();
            UserScraperResponse response = null;
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            if (!DominatorAccountModel.IsRunProcessThroughBrowser)
            {
                if (tumblrUser.PageUrl != null)
                    response = new UserScraperResponse(new ResponseParameter
                    { Response = "{\"meta\":{\"status\":200,\"msg\":\"OK\"}" });
            }
            else
            {
                status = _browserManager.SearchUserDetails(DominatorAccountModel, ref tumblrUser);
                isUserDetail = !string.IsNullOrEmpty(status) && status.Contains("true") ? true : false;
            }

            if (response != null && response.Success || isUserDetail)
            {
                GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType.ToString(),
                    " User => " + tumblrUser.Username);

                IncrementCounters();
                scrapeResult.ResultUser = tumblrUser;
                AddFollowedDataToDataBase(scrapeResult, DominatorAccountModel.AccountBaseModel.UserName);
                var AccountModel = new AccountModel(DominatorAccountModel);
                if (AccountModel.LstFollowings == null)
                    AccountModel.LstFollowings = new List<TumblrUser>();
                AccountModel.LstFollowings.Add(tumblrUser);
                jobProcessResult.IsProcessSuceessfull = true;
            }
            else if (!isUserDetail && !string.IsNullOrEmpty(status))
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType.ToString(), "Successfully Skipped As : " + status);
                jobProcessResult.IsProcessSuceessfull = false;
            }
            else
            {
                GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType.ToString(),
                     scrapeResult.ResultUser.UserId);
                jobProcessResult.IsProcessSuceessfull = false;
            }

            DelayBeforeNextActivity();
            return jobProcessResult;
        }

        /// <summary>
        ///     Add Followed User Data to DB
        /// </summary>
        /// <param name="scrapeResult"></param>
        /// <param name="username"></param>
        private void AddFollowedDataToDataBase(ScrapeResultNew scrapeResult, string username)
        {
            try
            {
                var userBrows = (TumblrUser)scrapeResult.ResultUser;

                if (!string.IsNullOrEmpty(CampaignId))
                {
                    // Add data to respected campaign InteractedUsers table
                    IDbCampaignService _campaignService = new DbCampaignService(CampaignId);
                    _campaignService.Add(new InteractedUser
                    {
                        AccountEmail = username,
                        ActivityType = ActivityType.ToString(),
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        InteractedUsername = userBrows.Username,
                        UserFullName = /*(!isBrowser) ? user.OwnerUsername :*/ userBrows.FullName,
                        UserProfileUrl = /*(!isBrowser) ? user.ProfileUrl :*/ userBrows.PageUrl
                    });
                }

                // Add data to respected account interactedusers table
                DbAccountService.Add(new DominatorHouseCore.DatabaseHandler.TumblrTables.Account.InteractedUser
                {
                    AccountEmail = username,
                    ActivityType = ActivityType.ToString(),
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    InteractedUsername = userBrows.Username,
                    UserFullName = /*(!isBrowser) ? user.OwnerUsername :*/ userBrows.FullName,
                    UserProfileUrl = /* (!isBrowser) ? user.ProfileUrl :*/ userBrows.PageUrl
                });
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ex.Message + " " + ex.StackTrace);
            }
        }
    }
}