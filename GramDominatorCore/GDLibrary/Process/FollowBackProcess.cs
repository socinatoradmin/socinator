using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using GramDominatorCore.Factories;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDModel;
using GramDominatorCore.Request;
using GramDominatorCore.Response;
using System;
using System.Net;
using GramDominatorCore.Utility;

namespace GramDominatorCore.GDLibrary
{
    public class FollowBackProcess : GdJobProcessInteracted<InteractedUsers>
    {
        private int _actionBlockedCount;
        public FollowBackProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IGdQueryScraperFactory queryScraperFactory, IGdHttpHelper httpHelper, IGdBrowserManager gdBrowser, IDelayService _delayService) :
            base(processScopeModel, accountServiceScoped, queryScraperFactory, httpHelper, gdBrowser,_delayService)
        {
        }

        public override DominatorHouseCore.Process.JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            if(ModuleSetting.IsFollowBack)
                GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultUser.Username);
            else
               GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                $"Trying to Accept Follow Request {scrapeResult.ResultUser.Username}");

            DominatorHouseCore.Process.JobProcessResult jobProcessResult = new DominatorHouseCore.Process.JobProcessResult();
            try
            {
                InstagramUser instagramUser = (InstagramUser)scrapeResult.ResultUser;
                var response = 
                    GramStatic.IsBrowser ?
                    instaFunct.GdBrowserManager.Follow(DominatorAccountModel, JobCancellationTokenSource.Token, instagramUser)
                    : ModuleSetting.IsFollowBack
                    ? instaFunct.Follow(DominatorAccountModel, AccountModel, JobCancellationTokenSource.Token,
                        instagramUser.UserId)
                    : instaFunct.AcceptRequest(DominatorAccountModel, AccountModel, JobCancellationTokenSource.Token,
                        instagramUser.UserId);
                var visited = instaFunct.GdBrowserManager.VisitPage(DominatorAccountModel, $"https://www.instagram.com/{instagramUser?.Username}/").Result;
                if (response!=null && response.Success)
                {
                    if(ModuleSetting.IsFollowBack)
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultUser.Username);
                    else
                         GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                $"Successfully Accept Follow Request {instagramUser.Username}");
                    
                    IncrementCounters();
                    AddFollowedBackDataToDataBase(scrapeResult, response);
                    AccountModel.LstFollowings.Add(instagramUser);

                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    if (response.ToString().Contains("This block will expire on"))
                    {
                        string expireDate = Utilities.GetBetween(response.ToString(), "This block will expire on", ".");
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $" action has been blocked.This block will expire on {expireDate}");
                        Stop();
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    }
                    else if (response.ToString().Contains("Action Blocked") && response.ToString().Contains("\"feedback_required\""))
                    {
                        delayservice.ThreadSleep(TimeSpan.FromSeconds(30));
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
                            delayservice.ThreadSleep(TimeSpan.FromSeconds(10));// Thread.Sleep(TimeSpan.FromSeconds(10));
                             response = ModuleSetting.IsFollowBack ? instaFunct.Follow(DominatorAccountModel, AccountModel, JobCancellationTokenSource.Token, instagramUser.UserId) : instaFunct.AcceptRequest(DominatorAccountModel, AccountModel, JobCancellationTokenSource.Token, instagramUser.UserId);
                            if ((response != null && response.Success) && (response.Following || response.IsPrivate))
                            {
                                if (ModuleSetting.IsFollowBack)
                                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultUser.Username);
                                else
                                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                           $"Successfully Accept Follow Request {instagramUser.Username}");
                                IncrementCounters();
                                AddFollowedBackDataToDataBase(scrapeResult, response);
                                AccountModel.LstFollowings.Add(instagramUser);

                                jobProcessResult.IsProcessSuceessfull = true;
                            }
                            else if (response != null && response.Success && !response.Following)
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                   $"Sorry, You are unable to follow this user {instagramUser.Username} as your instagram account blocked , please check once manually");
                                Stop();

                            }
                            else
                            {
                                if (!CheckResponse.CheckProcessResponse(response, DominatorAccountModel, ActivityType, scrapeResult, ref _actionBlockedCount))
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
                        if (!CheckResponse.CheckProcessResponse(response, DominatorAccountModel, ActivityType, scrapeResult, ref _actionBlockedCount))
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
            catch (Exception ex)
            {
                ex.DebugLog();
            }


            return jobProcessResult;
        }

        private void AddFollowedBackDataToDataBase(ScrapeResultNew scrapeResult, FriendshipsResponse response)
        {
            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var instUser = (InstagramUser)scrapeResult.ResultUser;

                // Add data to respected campaign InteractedUsers table
                if (!string.IsNullOrEmpty(CampaignId))
                {
                    CampaignDbOperation?.Add(
                        new DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedUsers()
                        {
                            ActivityType = ActivityType.ToString(),
                            Date = DateTimeUtilities.GetEpochTime(),
                            Username = DominatorAccountModel.AccountBaseModel.UserName,
                            InteractedUsername = scrapeResult.ResultUser.Username,
                            InteractedUserId = ((InstagramUser) scrapeResult.ResultUser).UserId,
                            FollowedBack = response.FollowedBack ? 1 : 0,
                            Time = DateTimeUtilities.GetEpochTime()
                        });
                }

                // Add data to respected Account InteractedUsers table
                AccountDbOperation.Add(new InteractedUsers()
                {
                    ActivityType = ActivityType.ToString(),
                    Date = DateTimeUtilities.GetEpochTime(),
                    Username = DominatorAccountModel.AccountBaseModel.UserName,
                    InteractedUsername = scrapeResult.ResultUser.Username,
                    InteractedUserId = ((InstagramUser) scrapeResult.ResultUser).UserId,
                    FollowedBack = response.FollowedBack ? 1 : 0,
                    Time = DateTimeUtilities.GetEpochTime()
                });


                // Add data to respected account friendship table
                AccountDbOperation.Add(new Friendships()
                {
                    Username = instUser.Username,
                    IsPrivate = instUser.IsPrivate,
                    IsVerified = instUser.IsVerified,
                    UserId = instUser.UserId,
                    FullName = instUser.FullName,
                    HasAnonymousProfilePicture = (instUser.HasAnonymousProfilePicture == true),
                    ProfilePicUrl = instUser.ProfilePicUrl,
                    Followings = 1,
                    FollowType = DominatorHouseCore.DatabaseHandler.GdTables.FollowType.Following,
                    IsFollowBySoftware = true
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {

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
    }
}
