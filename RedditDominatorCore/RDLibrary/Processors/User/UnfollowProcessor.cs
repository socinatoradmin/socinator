using DominatorHouseCore;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using RedditDominatorCore.RDLibrary.BrowserManager;
using RedditDominatorCore.RDLibrary.DAL;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.Response;
using System;
using System.Linq;

namespace RedditDominatorCore.RDLibrary.Processors.User
{
    internal class UnfollowProcessor : BaseRedditUserProcessor
    {
        private readonly IRdBrowserManager _browserManager;
        private readonly IDbAccountServiceScoped _dbAccountService;

        public UnfollowProcessor(IProcessScopeModel processScopeModel, IRdJobProcess jobProcess,
            IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService,
            IRedditFunction redditFunction, IRdBrowserManager browserManager)
            : base(jobProcess, dbAccountService, globalService, campaignService, redditFunction, browserManager)
        {
            _dbAccountService = dbAccountService;
            _browserManager = browserManager;
            UnfollowerModel = processScopeModel.GetActivitySettingsAs<UnfollowerModel>();
        }

        private UnfollowerModel UnfollowerModel { get; }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            if (UnfollowerModel.IsChkPeopleFollowedBySoftwareChecked && !jobProcessResult.IsProcessCompleted)
                FilterAndStartUnfollowBySoftware(ref jobProcessResult);

            if (UnfollowerModel.IsChkCustomUsersListChecked && !jobProcessResult.IsProcessCompleted)
                FilterAndStartUnfollowCustom(ref jobProcessResult);

            if (UnfollowerModel.IsChkPeopleFollowedOutsideSoftwareChecked && !jobProcessResult.IsProcessCompleted)
                FilterAndStartUnfollowByOutsideSoftware(ref jobProcessResult);

            _browserManager.CloseBrowser();
            jobProcessResult.HasNoResult = true;
        }

        public void FilterAndStartUnfollowBySoftware(ref JobProcessResult jobProcessResult)
        {
            try
            {
                var lstInteractedUsers = _dbAccountService.GetInteractedFollower().ToList();

                if (UnfollowerModel.IsUserFollowedBeforeChecked)
                {
                    var updateDateTime = DateTime.Now.Subtract(TimeSpan.FromDays(UnfollowerModel.FollowedBeforeDay));
                    updateDateTime = updateDateTime.Subtract(TimeSpan.FromHours(UnfollowerModel.FollowedBeforeHour));
                    lstInteractedUsers = lstInteractedUsers
                        .Where(x => DateTime.Compare(updateDateTime, x.InteractionDateTime) > 0).ToList();

                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.AccountBaseModel.UserName, "Unfollow",
                        $"Found {lstInteractedUsers.Count} result after filter checked follow before Days/Hour");
                }

                for (var i = 0; i < lstInteractedUsers.Count && !jobProcessResult.IsProcessCompleted; i++)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var userName = lstInteractedUsers[i].InteractedUsername;
                    if (_dbAccountService.GetInteractedUserName(ActivityType, userName).Count > 0)
                        continue;

                    var newUserFilter = new ScrapeFilter.User(JobProcess.ModuleSetting);
                    var userDetails = new RedditUser();

                    if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        userDetails = RedditFunction.GetUserDetailsByUsername(JobProcess.DominatorAccountModel,
                            lstInteractedUsers[i].InteractedUsername).RedditUser;
                    }

                    //For browser automation
                    else
                    {
                        var url = GetUserUrl(userName);
                        var response = _browserManager.TryAndGetResponse(JobProcess.DominatorAccountModel, url, 3, string.Empty, true);
                        var userResponseHandler = new UserResponseHandler(response);
                        userDetails = userResponseHandler.RedditUser;
                    }

                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (newUserFilter.IsFilterApplied() && !newUserFilter.AppplyFilters(userDetails))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork, JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        $"Filter Not Matched for {userDetails.Username}");
                        continue;
                    }
                    var queryInfo = new QueryInfo { QueryType = "By software" };

                    StartFinalUserProcess(ref jobProcessResult, userDetails, queryInfo);
                }
            }
            catch (OperationCanceledException)
            {
                _browserManager.CloseBrowser();
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void FilterAndStartUnfollowCustom(ref JobProcessResult jobProcessResult)
        {
            try
            {
                var lstCustomUsers = UnfollowerModel.LstCustomUser;
                for (var i = 0; i < lstCustomUsers.Count && !jobProcessResult.IsProcessCompleted; i++)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var userName = lstCustomUsers[i];
                    //var queryInfo = new QueryInfo { QueryType = "Custom User" };

                    if (_dbAccountService.GetInteractedUserName(ActivityType, userName).Count > 0)
                        continue;

                    var newUserFilter = new ScrapeFilter.User(JobProcess.ModuleSetting);
                    var userDetails = new RedditUser();

                    if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        userDetails = RedditFunction
                            .GetUserDetailsByUsername(JobProcess.DominatorAccountModel, lstCustomUsers[i]).RedditUser;
                    }

                    //For browser automation
                    else
                    {
                        var url = GetUserUrl(userName);
                        var response = _browserManager.TryAndGetResponse(JobProcess.DominatorAccountModel, url, 3, string.Empty, true);
                        var userResponseHandler = new UserResponseHandler(response);
                        userDetails = userResponseHandler.RedditUser;
                    }

                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (newUserFilter.IsFilterApplied() && !newUserFilter.AppplyFilters(userDetails))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                        JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.AccountBaseModel.UserName, "Unfollow",
                        $"filter not matched user");
                        continue;
                    }
                    var queryInfo = new QueryInfo { QueryType = "Custom User" };

                    StartFinalUserProcess(ref jobProcessResult, userDetails, queryInfo);
                }
            }
            catch (OperationCanceledException)
            {
                _browserManager.CloseBrowser();
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void FilterAndStartUnfollowByOutsideSoftware(ref JobProcessResult jobProcessResult)
        {
            try
            {
                var lstInteractedUsers = _dbAccountService.GetInteractedFollower().ToList();
                var lstFollowedUser = RedditFunction.FollowedUserList(JobProcess.DominatorAccountModel);

                lstFollowedUser.RemoveAll(z => lstInteractedUsers.Any(y => y.DisplayName.Equals(z)));

                for (var i = 0; i < lstFollowedUser.Count && !jobProcessResult.IsProcessCompleted; i++)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var userName = lstFollowedUser[i];
                    var queryInfo = new QueryInfo { QueryValue = userName, QueryType = "UnFollow By Outside Software" };

                    var userDetails = new RedditUser();
                    if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        userDetails = RedditFunction
                            .GetUserDetailsByUsername(JobProcess.DominatorAccountModel, queryInfo.QueryValue)
                            .RedditUser;
                    }

                    //For browser automation
                    else
                    {
                        var url = GetUserUrl(userName);
                        var response = _browserManager.TryAndGetResponse(JobProcess.DominatorAccountModel, url, 3, string.Empty, true);
                        var userResponseHandler = new UserResponseHandler(response);
                        userDetails = userResponseHandler.RedditUser;
                    }
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    StartFinalUserProcess(ref jobProcessResult, userDetails, queryInfo);
                }
            }
            catch (OperationCanceledException)
            {
                _browserManager.CloseBrowser();
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public string GetUserUrl(string userName)
        {
            var url = userName;
            if (!url.Contains("https://www.reddit.com/user/")) url = $"https://www.reddit.com/user/{userName}";
            return url;
        }
    }
}