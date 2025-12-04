using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.TumblrTables.Account;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TumblrFactory;
using TumblrDominatorCore.TumblrLibrary.DAL;
using TumblrDominatorCore.TumblrLibrary.TumblrFunction;
using TumblrDominatorCore.TumblrRequest;
using TumblrDominatorCore.TumblrResponseHandler;

namespace TumblrDominatorCore.TumblrLibrary.TumblrProcesses
{
    public class UnFollowJobProcess : TumblrJobProcessInteracted<UnFollowedUser>
    {
        private readonly ITumblrHttpHelper HttpHelper;

        private readonly ITumblrFunct tumblrFunction;
        private readonly ITumblrLoginProcess tumblrLogin;

        public UnFollowJobProcess(IProcessScopeModel processScopeModel, IDbAccountServiceScoped _accountService,
            IDbGlobalService _dbGlobalService, IExecutionLimitsManager executionLimitsManager,
            ITumblrFunct _tumblrFunct, ITumblrQueryScraperFactory queryScraperFactory, ITumblrHttpHelper _httpHelper,
            ITumblrLoginProcess _tumblrLoginProcess) : base(processScopeModel, _accountService, _dbGlobalService,
            executionLimitsManager, queryScraperFactory, _httpHelper,
            _tumblrLoginProcess)
        {
            HttpHelper = _httpHelper;
            tumblrFunction = _tumblrFunct;
            tumblrLogin = _tumblrLoginProcess;
            UnFollowerModel = processScopeModel.GetActivitySettingsAs<UnfollowerModel>();
        }

        public UnfollowerModel UnFollowerModel { get; set; }
        public int FollowerCount { get; set; }
        public int CurrentUnfollowedCount { get; set; }
        public int FollowingcountafterUnFollow { get; set; }
        public int FollowingcountafterFollow { get; set; }

        /// <summary>
        ///     Start Unfollow Process
        /// </summary>
        public async void StartUnFollowProcess()
        {
            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();


                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (!await tumblrLogin.CheckAutomationLogin(DominatorAccountModel, JobCancellationTokenSource.Token))
                {
                    GlobusLogHelper.log.Info("Could not log in with account - " +
                                             DominatorAccountModel.AccountBaseModel.UserName);
                    return;
                }

                var tumblrFormKey = string.Empty;
                if (string.IsNullOrEmpty(tumblrFormKey))
                {
                    var responseParameter = HttpHelper.GetRequest("https://www.tumblr.com/following");
                    tumblrFormKey =
                        HtmlParseUtility.GetAttributeValueFromId(responseParameter.Response, "tumblr_form_key",
                            "content");
                }

                var lstUserList = new List<TumblrUser>();

                if (UnFollowerModel.IsChkCustomUsersListChecked)
                {
                    Regex.Split(UnFollowerModel.CustomUsersList, "\r\n").ToList().ForEach(x =>
                    {
                        lstUserList.Add(new TumblrUser
                        {
                            Username = x
                        });
                    });

                    // FilterUsers(lstUserList);

                    #region database checking

                    var usersList = lstUserList.ToList();
                    var alreayUnfollowed = DbAccountService.GetUnfollowedUsers(ActivityType);

                    if (alreayUnfollowed.Any())
                    {
                        var result = alreayUnfollowed.Select(x => x.InteractedUsername).Distinct().ToList();
                        //comapring 2 lists
                        usersList = lstUserList.Where(x => !result.Contains(x.Username)).ToList();
                    }

                    #endregion

                    UnFollowListOfUsers(usersList, tumblrFormKey);
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                }

                if (UnFollowerModel.IsChkPeopleFollowedBySoftwareChecked)
                    try
                    {
                        var getFollowedUsers = DbAccountService.GetInteractedUsers(ActivityType.Follow);

                        getFollowedUsers.ForEach(
                            x =>
                            {
                                try
                                {
                                    lstUserList.Add(new TumblrUser
                                    {
                                        Username = x.InteractedUsername
                                    });
                                }
                                catch (Exception ex)
                                {
                                    ex.DebugLog();
                                }
                            });

                        var usersList = lstUserList.ToList();

                        #region database checking

                        var alreayUnfollowed = DbAccountService.GetUnfollowedUsers(ActivityType);

                        if (alreayUnfollowed.Any())
                        {
                            var result = alreayUnfollowed.Select(x => x.InteractedUsername).Distinct().ToList();
                            usersList = lstUserList.Where(x => !result.Contains(x.Username)).ToList();
                        }

                        #endregion

                        FilterUsers(usersList);
                        UnFollowListOfUsers(usersList, tumblrFormKey);
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    }
                    catch (OperationCanceledException)
                    {
                        throw new OperationCanceledException();
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(Log.CustomMessage,
                            AccountModel.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            " method=>> StartUnFollowProcess",
                            AccountModel.DominatorAccountModel.UserName + " " + ex.Message);
                    }


                if (UnFollowerModel.IsChkPeopleFollowedOutsideSoftwareChecked)
                {
                    var jobProcessResult = new JobProcessResult();
                    while (!jobProcessResult.IsProcessCompleted || IsStopped())
                        try
                        {
                            var softwarefollowedUsers =
                                DbAccountService.GetSelectedUsers(AccountModel.DominatorAccountModel.UserName);
                            var removeUsers = softwarefollowedUsers.Select(x => x.InteractedUsername).ToList();

                            var lstFriendShpip =
                                DbAccountService.GetFriendships(FollowType
                                    .Following); //dbAccountOperation.Get<Friendships>(x => x.FollowType == FollowType.Following);
                            if (!lstFriendShpip.Any())
                            {
                                GlobusLogHelper.log.Info(AccountModel.DominatorAccountModel.UserName + " have " +
                                                         lstFriendShpip.Count + " Followings ");
                                break;
                            }

                            var postdetails = lstFriendShpip.Where(x => !removeUsers.Contains(x.Username)).ToList();
                            postdetails.ForEach(x =>
                            {
                                lstUserList.Add(new TumblrUser
                                {
                                    Username = x.Username
                                });
                            });

                            var usersList = lstUserList.ToList();

                            #region database checking

                            var alreayUnfollowed = DbAccountService.GetUnfollowedUsers(ActivityType);

                            if (alreayUnfollowed.Any())
                            {
                                var result = alreayUnfollowed.Select(x => x.InteractedUsername).ToList();
                                usersList = lstUserList.Where(x => !result.Contains(x.Username)).ToList();
                            }

                            #endregion

                            // FilterUsers(usersList);
                            jobProcessResult = UnFollowListOfUsers(usersList, tumblrFormKey);
                            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        }
                        catch (OperationCanceledException)
                        {
                            throw new OperationCanceledException();
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error(Log.CustomMessage,
                                AccountModel.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                " method=>> StartUnFollowProcess",
                                AccountModel.DominatorAccountModel.UserName + " " + ex.Message);
                        }
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception e)
            {
                GlobusLogHelper.log.Error(Log.CustomMessage,
                    AccountModel.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    " method=>> StartUnFollowProcess", AccountModel.DominatorAccountModel.UserName + " " + e.Message);
            }
        }

        /// <summary>
        ///     Filter Users before Unfollow
        /// </summary>
        /// <param name="lstTumblrUser"></param>
        public List<TumblrUser> FilterUsers(List<TumblrUser> lstTumblrUser)
        {
            try
            {
                if (UnFollowerModel.IsUserFollowedBeforeChecked)
                    lstTumblrUser = FilterIsFollowedBeforeSpecificTime(lstTumblrUser);

                if (UnFollowerModel.IsWhoNotFollowBackChecked || UnFollowerModel.IsWhoFollowBackChecked)
                    lstTumblrUser = FilterIsFollowMeback(lstTumblrUser);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return lstTumblrUser;
        }

        /// <summary>
        ///     Start Final process to Unfollow
        /// </summary>
        /// <param name="lstUserList"></param>
        /// <param name="tumblrFormKey"></param>
        /// <returns></returns>
        private JobProcessResult UnFollowListOfUsers(List<TumblrUser> lstUserList, string tumblrFormKey)
        {
            JobProcessResult jobProcessResult = null;

            try
            {
                lstUserList.ForEach(x =>
                {
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var tumblrScrapeResult = new TumblrScrapeResult
                    {
                        ResultUser = x,
                        TumblrFormKey = tumblrFormKey
                    };
                    jobProcessResult = FinalProcess(tumblrScrapeResult);
                    if (IsStopped() || jobProcessResult.IsProcessCompleted)
                        return;
                });
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception)
            {
                // ignored
            }

            return jobProcessResult;
        }


        /// <summary>
        ///     UNFOLLOW Users of Tumblr
        /// </summary>
        /// <param name="scrapeResultNew"></param>
        /// <returns></returns>
        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResultNew)
        {
            var jobProcessResult = new JobProcessResult();

            var scrapeResult = (TumblrScrapeResult)scrapeResultNew;

            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType.ToString(),
                " Trying to Unfollow user => " + scrapeResult.ResultUser.Username);

            UnFollowResponseHandler response = null;
            try
            {
                var tumblrUser = (TumblrUser)scrapeResult.ResultUser;
                if (string.IsNullOrEmpty(tumblrUser.PageUrl))
                    tumblrUser.PageUrl = $"https://{tumblrUser.Username}.tumblr.com/";
                if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                    response = tumblrFunction.UnFollow(DominatorAccountModel, tumblrUser, scrapeResult.TumblrFormKey);
                else
                    response = _browserManager.UnFollow(DominatorAccountModel, tumblrUser);
                if (response != null && response.Success)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType.ToString(),
                        " User Unfollowed => " + scrapeResult.ResultUser.Username);
                    IncrementCounters();
                    scrapeResult.ResultUser = tumblrUser;
                    StartOtherConfiguration(scrapeResult);
                    AddFollowedDataToDataBase(scrapeResult);
                    var AccountModel = new AccountModel(DominatorAccountModel);
                    if (AccountModel.LstFollowings == null)
                        AccountModel.LstFollowings = new List<TumblrUser>();
                    AccountModel.LstFollowings.Add(tumblrUser);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else if (DominatorAccountModel.IsRunProcessThroughBrowser && !response.Success && !tumblrUser.IsFollowed)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                       DominatorAccountModel.AccountBaseModel.UserName, ActivityType.ToString(),
                       "Unable to Unfollow user => " + scrapeResult.ResultUser.Username + "As Not Followed");
                    jobProcessResult.IsProcessSuceessfull = false;

                }
                else
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType.ToString(),
                        "Unable to Unfollow user => " + scrapeResult.ResultUser.Username);
                    jobProcessResult.IsProcessSuceessfull = false;
                }

                DelayBeforeNextActivity();
            }
            catch (Exception e)
            {
                e.DebugLog();
            }

            return jobProcessResult;
        }


        private List<TumblrUser> FilterIsFollowedBeforeSpecificTime(List<TumblrUser> lstUserList)
        {
            var lstUserListAfterFilter = new List<TumblrUser>();

            try
            {
                if (UnFollowerModel.IsUserFollowedBeforeChecked)
                {
                    var dateTimeToCheckBefore = GetEpochTime(DateTime.Now
                        .AddDays(-UnFollowerModel.FollowedBeforeDay)
                        .AddHours(-UnFollowerModel.FollowedBeforeHour));
                    lstUserList.ForEach(x =>
                    {
                        var interactionTimeStamp = DbAccountService.GetInteractedtime(x.Username, ActivityType);
                        if (interactionTimeStamp <= dateTimeToCheckBefore) lstUserListAfterFilter.Add(x);
                    });
                }
                else
                {
                    lstUserListAfterFilter = lstUserList;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return lstUserListAfterFilter;
        }

        private List<TumblrUser> FilterIsFollowMeback(List<TumblrUser> lstUserList)
        {
            var lstUserListAfterFilter = new List<TumblrUser>();

            try
            {
                lstUserList.ForEach(x =>
                {
                    var isFollowingMeBack = DbAccountService.GetSelectedUsers(x.Username).Count >= 1;

                    if (UnFollowerModel.IsWhoFollowBackChecked && isFollowingMeBack)
                        lstUserListAfterFilter.Add(x);
                    if (UnFollowerModel.IsWhoNotFollowBackChecked && !isFollowingMeBack)
                        lstUserListAfterFilter.Add(x);
                    if (!UnFollowerModel.IsWhoFollowBackChecked && !UnFollowerModel.IsWhoNotFollowBackChecked)
                        lstUserListAfterFilter.Add(x);
                });
            }
            catch (Exception)
            {
                // ignored
            }

            return lstUserListAfterFilter;
        }

        /// <summary>
        ///     Skip Whitelisted Users from Unfollow
        /// </summary>
        /// <param name="lstUserList"></param>
        /// <param name="statusUser"></param>
        /// <returns></returns>


        public static int GetEpochTime(DateTime dateTime)
        {
            return (int)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }

        /// <summary>
        ///     Saving Unfollowed Users data to DB
        /// </summary>
        /// <param name="scrapeResult"></param>
        protected void AddFollowedDataToDataBase(ScrapeResultNew scrapeResult)
        {
            try
            {
                var tumblrUser = scrapeResult.ResultUser;
                if (!string.IsNullOrEmpty(CampaignId))
                {
                    IDbCampaignService CampaignService = new DbCampaignService(CampaignId);
                    //var dbAccountService = InstanceProvider.ResolveAccountDbOperations(DominatorAccountModel.AccountId, _networks);

                    // Add data to respected campaign InteractedUsers table
                    CampaignService.Add(new DominatorHouseCore.DatabaseHandler.TumblrTables.Campaign.UnFollowedUser
                    {
                        ActivityType = ActivityType.ToString(),
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        UserName = DominatorAccountModel.AccountBaseModel.UserId,
                        InteractedUsername = tumblrUser.Username,
                        TemplateId = TemplateId
                    });
                }

                // var dbAccountOperation = new DbOperations(AccountId, SocialNetworks.Tumblr, ConstantVariable.GetAccountDb);
                // Add data to respected account friendship table
                DbAccountService.Add(new UnFollowedUser
                {
                    ActivityType = ActivityType.ToString(),
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    UserName = DominatorAccountModel.AccountBaseModel.UserName,
                    InteractedUsername = tumblrUser.Username
                });

                // Remove user from Friendship after unfollow
                var followedUser =
                    DbAccountService.GetSpecificUser(tumblrUser
                        .Username); //.GetSingle<Friendships>(x => x.Username == tumblrUser.Username);
                if (followedUser != null) DbAccountService.Remove(followedUser);

                #region add to blackist

                if (UnFollowerModel.IsChkAddToPrivateBlackList)
                {
                    DbAccountService.Add(new PrivateBlacklist
                    {
                        UserName = tumblrUser.Username,
                        InteractionTimeStamp = DateTime.Now.GetCurrentEpochTime()
                    });
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType.ToString(),
                        tumblrUser.Username + " => is added to PrivateBlackList.");
                }
                if (UnFollowerModel.IsChkAddToGroupBlackList)
                {
                    //Thread.Sleep(15000);
                    IDbGlobalService dbGlobalService = new DbGlobalService();
                    dbGlobalService.Add(new BlackListUser
                    {
                        UserName = tumblrUser.Username,
                        AddedDateTime = DateTime.Now
                    });
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType.ToString(),
                        tumblrUser.Username + " => is added to GroupBlackList.");
                }

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {
            try
            {
                CurrentUnfollowedCount++;

                base.StartOtherConfiguration(scrapeResult);
                var responses = tumblrFunction.GetUserInfo(DominatorAccountModel);
                FollowingcountafterUnFollow = responses.TumblrUser.FollowingCount;

                // var lstTotalFollowedusers = CampaignService.GetSelectedUsers(DominatorAccountModel.AccountBaseModel.UserName);// dboperationCampaign.Get<DominatorHouseCore.DatabaseHandler.TumblrTables.Campaign.InteractedUser>(x => x.UserName == DominatorAccountModel.AccountBaseModel.ProfileId);

                #region Process for auto Follow and Unfollow

                if (!UnFollowerModel.IsChkEnableAutoFollowUnfollowChecked) return;
                if (UnFollowerModel.IsChkStopFollowToolWhenReachChecked)
                    if (UnFollowerModel.IsChkStopFollowToolWhenReachChecked && FollowingcountafterUnFollow >=
                        UnFollowerModel.StopUnFollowToolWhenReachValue.GetRandom())
                        try
                        {
                            var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                            dominatorScheduler.EnableDisableModules(ActivityType.Unfollow, ActivityType.Follow,
                                DominatorAccountModel.AccountId);
                        }
                        catch (InvalidOperationException ex)
                        {
                            GlobusLogHelper.log.Error(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                                ex.Message.Contains("1001")
                                    ? "Follow activity has met your Auto Enable configuration for unfollow, but you do not have Unfollow configuration saved. Please save the unfollow configuration manually, to restart the Follow/Unfollow activity from this account"
                                    : "");
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                if (UnFollowerModel.IsChkEnableAutoFollowUnfollowChecked && UnFollowerModel.IsChkStopUnFollowTool)
                {
                    if (UnFollowerModel.IsChkStopUnFollowToolWhenReachChecked)
                        // GetFollowingCount();

                        //int followingCount = lstTotalFollowedusers.Count;
                        if (UnFollowerModel.IsChkStopUnFollowToolWhenReachChecked && CurrentUnfollowedCount >=
                            UnFollowerModel.StopUnFollowToolWhenReachValue.GetRandom())
                            StopUnFollowTool();
                    if (UnFollowerModel.IsChkWhenFollowerFollowingsGreater)
                    {
                        var followFollwingRatio = FollowerCount / FollowingcountafterFollow;
                        if (UnFollowerModel.IsChkWhenFollowerFollowingsGreater &&
                            UnFollowerModel.FollowerFollowingsGreaterThanValue > followFollwingRatio)
                            StopUnFollowTool();
                    }
                }

                if (!UnFollowerModel.IsChkWhenFollowerFollowingsGreater) return;
                {
                    var followFollwingRatio = responses.TumblrUser.FollowersCount / responses.TumblrUser.FollowingCount;
                    if (UnFollowerModel.IsChkWhenFollowerFollowingsGreater &&
                        UnFollowerModel.FollowerFollowingsGreaterThanValue < followFollwingRatio)
                        try
                        {
                            var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                            dominatorScheduler.EnableDisableModules(ActivityType.Unfollow, ActivityType.Follow,
                                DominatorAccountModel.AccountId);
                        }
                        catch (InvalidOperationException ex)
                        {
                            GlobusLogHelper.log.Error(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                                ex.Message.Contains("1001")
                                    ? "Unfollow activity has met your Auto Enable configuration for Follow, but you do not have Follow configuration saved. Please save the Follow configuration manually, to restart the Follow/Unfollow activity from this account"
                                    : "");
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                }
                //if (UnFollowerModel.IsChkWhenNoUsersToUnfollow)
                //{

                //}

                #endregion
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.ProfileId, ex.Message + " " + ex.StackTrace);
            }
        }

        private void StopUnFollowTool()
        {
            try
            {
                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleConfiguration =
                    jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];
                var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                dominatorScheduler.StopActivity(DominatorAccountModel, "UnFollow", moduleConfiguration.TemplateId,
                    false);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public bool GetFollowingCount()
        {
            var followinglimitReached = false;
            try
            {
                if (UnFollowerModel.IsChkStopUnFollowTool)
                {
                    var tumblrFunction = new TumblrFunct(HttpHelper);
                    var responses = tumblrFunction.GetUserInfo(DominatorAccountModel);
                    FollowingcountafterFollow = responses.TumblrUser.FollowingCount;
                    FollowerCount = responses.TumblrUser.FollowersCount;
                    if (UnFollowerModel.IsChkStopFollowToolWhenReachChecked && FollowingcountafterFollow >=
                        UnFollowerModel.StopUnFollowToolWhenReachValue.GetRandom()) return followinglimitReached = true;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return followinglimitReached;
        }
    }
}