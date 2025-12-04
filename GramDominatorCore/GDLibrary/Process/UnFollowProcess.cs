using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.DatabaseHandler.GdTables;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using GramDominatorCore.Factories;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDModel;
using GramDominatorCore.Request;
using GramDominatorCore.Response;
using Newtonsoft.Json;
using System;
using System.Net;
using GramDominatorCore.Utility;

namespace GramDominatorCore.GDLibrary
{
    public class UnFollowProcess : GdJobProcessInteracted<UnfollowedUsers>
    {
        public UnfollowerModel UnfollowerModel { get; set; }

        private BlackListWhitelistHandler BlackListWhitelistHandler { get; }

        private int UnfollowIterationCount { get; set; }
        private int ActionBlockedCount;
        private int SpecificUnfollowCount;
        private bool _isSpecificUnfollowcount;

        public UnFollowProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IGdQueryScraperFactory queryScraperFactory, IGdHttpHelper httpHelper, IGdBrowserManager gdBrowser, IDelayService _delayService)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, httpHelper, gdBrowser, _delayService)
        {
            UnfollowerModel = JsonConvert.DeserializeObject<UnfollowerModel>(templateModel.ActivitySettings);
            BlackListWhitelistHandler = new BlackListWhitelistHandler(ModuleSetting, DominatorAccountModel, ActivityType);
        }


        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobProcessResult jobProcessResult = new JobProcessResult();
            try
            {
                #region Check for Auto Follow/ Unfollow process
                CheckAutoFollowUnfollowProcess();
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                #endregion
                GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultUser.Username);
                FriendshipsResponse response = null;
                InstagramUser instagramUser = (InstagramUser)scrapeResult.ResultUser;
                var browser = GramStatic.IsBrowser;
                if (!DominatorAccountModel.IsRunProcessThroughBrowser && string.IsNullOrEmpty(instagramUser.UserId))
                {
                    instagramUser.UserId = instagramUser.Pk;
                    if (string.IsNullOrEmpty(instagramUser.UserId))
                    {
                        var userInfoResponse = instaFunct.SearchUsername(DominatorAccountModel, instagramUser.Username, JobCancellationTokenSource.Token);
                        instagramUser.UserId = userInfoResponse.Pk;

                        delayservice.ThreadSleep(TimeSpan.FromSeconds(1));// Thread.Sleep(1000);
                    }
                }
                if (UnfollowerModel.IsUnfollowFollowings)
                {
                    if (UnfollowerModel.IsBlockUnBlockUnfollow)
                    {
                        var block = instaFunct.Block(DominatorAccountModel, AccountModel, JobCancellationTokenSource.Token, instagramUser.UserId ?? instagramUser.Pk).Result;
                        if (block.Success)
                        {
                            delayservice.ThreadSleep(TimeSpan.FromSeconds(3));
                            response = instaFunct.UnBlock(DominatorAccountModel, AccountModel, JobCancellationTokenSource.Token, instagramUser.UserId).Result;
                        }

                    }
                    else
                    {
                        response = 
                            browser ?
                            instaFunct.GdBrowserManager.Unfollow(DominatorAccountModel, AccountModel, JobCancellationTokenSource.Token, instagramUser)
                            : instaFunct.Unfollow(DominatorAccountModel, AccountModel, JobCancellationTokenSource.Token, instagramUser?.UserId ?? instagramUser?.Pk).Result;
                    }
                    if (UnfollowerModel.IsBlockUnBlockUnfollow && !response.Success)
                        response = instaFunct.Unfollow(DominatorAccountModel, AccountModel, JobCancellationTokenSource.Token, instagramUser.UserId).Result;
                }
                else
                {
                    response = 
                        browser ?
                        instaFunct.GdBrowserManager.RemoveFollowers(DominatorAccountModel, AccountModel, instagramUser.Username, JobCancellationTokenSource.Token)
                        : instaFunct.RemoveFollowers(DominatorAccountModel, AccountModel, instagramUser.UserId ?? instagramUser?.Pk, JobCancellationTokenSource.Token, DominatorAccountModel?.AccountBaseModel?.ProfileId).Result;
                }

                if (response != null && response.Success)
                {
                    if (instagramUser.OutgoingRequest)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType, $"Successfully unfollowed Requested user  {scrapeResult.ResultUser.Username}");
                        instagramUser.OutgoingRequest = false;
                    }
                    else
                    {
                        if (UnfollowerModel.IsUnfollowFollowings)
                        {
                            GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            scrapeResult.ResultUser.Username);
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                            $"Successfully removed follwer {scrapeResult.ResultUser.Username}");
                        }
                    }

                    UnfollowIterationCount++;
                    IncrementCounters();

                    AddUnFollowedDataToDataBase(scrapeResult, response);

                    // Add to Blacklist
                    if (UnfollowerModel.ManageBlackWhiteListModel.IsAddToBlackListOnceUnfollowed)
                    {
                        BlackListWhitelistHandler.AddToBlackList(instagramUser.UserId, instagramUser.Username);
                    }

                    // Actions required after Unfollow operation(e.g. Add to blacklist)
                    //DoAfterUnfollowAction();

                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    if (response.ToString().Contains("This block will expire on"))
                    {
                        string expireDate = Utilities.GetBetween(response.ToString(), "This block will expire on", ".");
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $" action has been blocked.This block will expire on {expireDate}");
                        Stop();
                    }
                    else if (response.ToString().Contains("Action Blocked") && response.ToString().Contains("\"feedback_required\""))
                    {
                        delayservice.ThreadSleep(TimeSpan.FromSeconds(30));//Thread.Sleep(TimeSpan.FromSeconds(30));
                        bool LoginStatus = false;
                        var BackupCookie = DominatorAccountModel.Cookies;
                        var logOutStatus = instaFunct.Logout(DominatorAccountModel, AccountModel);
                        if (logOutStatus.Success)
                        {
                            ResetCookies(BackupCookie);
                            LoginStatus = loginProcess.LoginWithAlternativeMethodForBlocking(DominatorAccountModel);
                        }
                        if (LoginStatus)
                        {
                            delayservice.ThreadSleep(TimeSpan.FromSeconds(10));//Thread.Sleep(TimeSpan.FromSeconds(10));
                            response = instaFunct.Unfollow(DominatorAccountModel, AccountModel, JobCancellationTokenSource.Token, instagramUser.UserId).Result;
                            if (response != null && response.Success)
                            {
                                if (instagramUser.OutgoingRequest)
                                {
                                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType, $"Successfully unfollowed Requested user  {scrapeResult.ResultUser.Username}");
                                    instagramUser.OutgoingRequest = false;
                                }
                                else
                                {
                                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    scrapeResult.ResultUser.Username);
                                }

                                UnfollowIterationCount++;
                                IncrementCounters();

                                AddUnFollowedDataToDataBase(scrapeResult, response);

                                // Add to Blacklist
                                if (UnfollowerModel.ManageBlackWhiteListModel.IsAddToBlackListOnceUnfollowed)
                                {
                                    BlackListWhitelistHandler.AddToBlackList(instagramUser.UserId, instagramUser.Username);
                                }

                                // Actions required after Unfollow operation(e.g. Add to blacklist)
                                DoAfterUnfollowAction();

                                jobProcessResult.IsProcessSuceessfull = true;
                            }
                            else
                            {
                                if (!CheckResponse.CheckProcessResponse(response, DominatorAccountModel, ActivityType, scrapeResult, ref ActionBlockedCount))
                                {
                                    Stop();
                                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                }
                                jobProcessResult.IsProcessSuceessfull = false;
                            }
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                  $"please check your account manually once");

                            Stop();
                        }
                    }
                    else
                    {
                        if (!CheckResponse.CheckProcessResponse(response, DominatorAccountModel, ActivityType, scrapeResult, ref ActionBlockedCount))
                        {
                            Stop();
                            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        }
                        jobProcessResult.IsProcessSuceessfull = false;
                    }
                }

                // Delay between each activity
                DelayBeforeNextActivity();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (AggregateException e)
            {
                foreach (Exception ex in e.InnerExceptions)
                    Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return jobProcessResult;
        }
        public void ResetCookies(CookieCollection Cookies)
        {
            DominatorAccountModel.Cookies = new CookieCollection();
            foreach (Cookie cookie in Cookies)
            {
                var cookieHelper = new CookieHelper();
                cookieHelper.Name = cookie.Name;
                cookieHelper.Value = cookie.Value;
                cookieHelper.Domain = cookie.Domain;
                cookieHelper.Expires = cookie.Expires;
                cookieHelper.HttpOnly = cookie.HttpOnly;
                cookieHelper.Secure = cookie.Secure;

                if (cookie.Name.Contains("mid") || cookie.Name.Contains("csrftoken") || cookie.Name.Contains("sessionid") || cookie.Name.Contains("ds_user_id")
                    || cookie.Name.Contains("rur") || cookie.Name.Contains("ds_user") || cookie.Name.Contains("igfl"))
                {
                    DominatorAccountModel.CookieHelperList.Add(cookieHelper);
                    DominatorAccountModel.Cookies.Add(cookie);
                }

            }
        }
        private void DoAfterUnfollowAction()
        {
            //if (UnfollowerModel.IsChkAddToBlackList)
            //{
            //    var globalDbOperation = new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetDbContext());

            //    globalDbOperation.Add(new BlackWhiteListUser()
            //    {
            //        UserName = instagramUser.Username,
            //        Network = SocinatorInitialize.ActiveSocialNetwork.ToString(),
            //        CategoryType = UserType.BlackListedUser.ToString(),
            //        AddedDateTime = DateTime.Now
            //    });
            //}
        }

        private void AddUnFollowedDataToDataBase(ScrapeResultNew scrapeResult, FriendshipsResponse unfollowResponse)
        {
            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                // Add data to respected campaign InteractedUsers table
                if (!string.IsNullOrEmpty(CampaignId))
                {
                    CampaignDbOperation?.Add(
                        new DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.UnfollowedUsers()
                        {
                            AccountUsername = DominatorAccountModel.UserName,
                            FollowType = FollowType.Unfollowed,
                            InteractionDate = DateTimeUtilities.GetEpochTime(),
                            UnfollowedUsername = scrapeResult.ResultUser.Username,
                            FollowedBack = unfollowResponse.FollowedBack ? 1 : 0
                        });
                }

                // Add data to respected Account InteractedUsers table          
                AccountDbOperation.Add(
                    new UnfollowedUsers
                    {
                        AccountUsername = DominatorAccountModel.UserName,
                        FollowType = FollowType.Unfollowed,
                        InteractionDate = DateTimeUtilities.GetEpochTime(),
                        UnfollowedUsername = scrapeResult.ResultUser.Username,
                        FollowedBack = unfollowResponse.FollowedBack ? 1 : 0
                    });

                // Update status for Account FriednShip table
                var getUser =
                    AccountDbOperation.GetSingle<DominatorHouseCore.DatabaseHandler.GdTables.Accounts.Friendships>(
                        x => (x.Username == scrapeResult.ResultUser.Username) && (x.FollowType == FollowType.Following));

                if (getUser != null)
                {
                    getUser.FollowType = FollowType.Unfollowed;

                    AccountDbOperation.Update(getUser);
                }
                //Remove user from friendsship table of account db
                AccountDbOperation.Remove<Friendships>(user => user.Username == getUser.Username);
                AccountDbOperation.Remove<InteractedUsers>(user => user.InteractedUsername == scrapeResult.ResultUser.Username);
            }
            catch (Exception)
            {
                //ignored
            }
        }

        /// <summary>
        /// Overrides abstract method of JobProcess. Will be called when JobProcess completes.
        /// </summary>
        /// <param name="scrapeResult"></param>
        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {

        }


        // ReSharper disable once UnusedMethodReturnValue.Local
        private bool CheckAutoFollowUnfollowProcess()
        {
            try
            {
                // Process for auto Follow and Unfollow
                #region Process for auto Follow and Unfollow

                if (UnfollowerModel.IsChkEnableAutoFollowUnfollowChecked)
                {
                    if (UnfollowerModel.IsCheckedStopUnfollowStartFollow)
                    {
                        #region Stop Follow and Start Unfollow activity
                        if (IsStartAutoFollowUnfollow())
                        {
                            try
                            {
                                var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                                dominatorScheduler.EnableDisableModules(ActivityType.Unfollow, ActivityType.Follow, DominatorAccountModel.AccountId);
                            }
                            catch (InvalidOperationException ex)
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.UserName, ActivityType,
                                    ex.Message.Contains("1001")
                                        ?
                                        "Unfollow activity has set your Auto Enable configuration for Follow, but you do not have Follow configuration saved. Please save the Follow configuration manually, to restart the Follow/Unfollow activity from this account"
                                        : "");
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }
                        }
                        #endregion
                    }
                    else if (UnfollowerModel.IschkOnlyStopUnfollowTool)
                    {
                        #region Only stop Unfollow activity
                        if (IsStartAutoFollowUnfollow())
                        {
                            var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                            dominatorScheduler.ChangeAccountsRunningStatus(false, AccountId, ActivityType);
                        }
                        #endregion
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }


        private bool IsStartAutoFollowUnfollow()
        {
            if (!_isSpecificUnfollowcount)
            {
                SpecificUnfollowCount = UnfollowerModel.StopUnfollowToolWhenReachSpecifiedFollowings.GetRandom();
                _isSpecificUnfollowcount = true;
            }
            try
            {
                if (UnfollowerModel.IsChkStopUnfollowToolWhenReachedSpecifiedFollowings ||
                    UnfollowerModel.IsChkWhenFollowerFollowingsGreater)
                {
                    // Getting actual Followers and Followings count by creating webRequest

                    var followersCount = DominatorAccountModel.DisplayColumnValue1;
                    var followingsCount = DominatorAccountModel.DisplayColumnValue2 - UnfollowIterationCount;

                    // StopStartAutoFollowUnfollow when specified followings reached
                    if (UnfollowerModel.IsChkStopUnfollowToolWhenReachedSpecifiedFollowings &&
                        SpecificUnfollowCount >= followingsCount)
                    {
                        return true;
                    }

                    // When Followers/Followings is lesser than specified count
                    if (UnfollowerModel.IsChkWhenFollowerFollowingsGreater)
                    {
                        var followerFollwingRatio = followersCount / followingsCount;

                        if (followerFollwingRatio > UnfollowerModel.FollowerFollowingsRatioValue)
                            return true;
                    }
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

    }
}
