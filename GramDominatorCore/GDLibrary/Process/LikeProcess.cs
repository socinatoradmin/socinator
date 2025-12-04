using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using GramDominatorCore.Factories;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.Request;
using GramDominatorCore.Response;
using GramDominatorCore.Utility;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using ThreadUtils;

namespace GramDominatorCore.GDLibrary
{
    public class LikeProcess : GdJobProcessInteracted<InteractedPosts>
    {
        public LikeModel LikeModel { get; set; }
        private int _actionBlockedCount;
        public LikeProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IGdQueryScraperFactory queryScraperFactory, IGdHttpHelper httpHelper, IGdLogInProcess logInProcess, IGdBrowserManager gdBrowser, IDelayService _delayService) :
            base(processScopeModel, accountServiceScoped, queryScraperFactory, httpHelper, gdBrowser, _delayService)
        {
            LikeModel = JsonConvert.DeserializeObject<LikeModel>(templateModel.ActivitySettings);
            loginProcess = logInProcess;
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultPost.Code);
            int delay = ModuleSetting.DelayBetweenEachActionBlock.GetRandom();
            instaFunct = loginProcess.InstagramFunctFactory.InstaFunctions;
            JobProcessResult jobProcessResult = new JobProcessResult();
            try
            {
                if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    AccountModel.WwwClaim = AccountModel.WwwClaim ?? "0";
                    AccountModel.WwwClaim = instaFunct.GetGdHttpHelper().Response.Headers["x-ig-set-www-claim"] ?? AccountModel.WwwClaim;
                }
                InstagramPost instagramPost = (InstagramPost)scrapeResult.ResultPost;
                if (SkipBlackListWhiteListUser(instagramPost?.User))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Skipped User {instagramPost?.User?.Username} As BlackListed User.");
                    return jobProcessResult;
                }
                var browser = GramStatic.IsBrowser;
                var response = 
                    browser ?
                    instaFunct.GdBrowserManager.Like(DominatorAccountModel, AccountModel, instagramPost.Code, JobCancellationTokenSource.Token)
                    : instaFunct.Like(DominatorAccountModel, AccountModel, JobCancellationTokenSource.Token, instagramPost.Code,instagramPost?.User?.Username,instagramPost?.User?.Pk,scrapeResult.QueryInfo).Result;
                var visited = instaFunct.GdBrowserManager.VisitPage(DominatorAccountModel, $"https://www.instagram.com/p/{instagramPost?.Code}/").Result;
                if (response.Success)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultPost.Code);
                    IncrementCounters();
                    AddLikedDataToDataBase(scrapeResult);
                    // Do after Like action
                    DoAfterLikeAction(instagramPost);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else if (response.NotClicked)
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Like button has not clicked");
                else if (!response.Success && response.Issue != null && response.Issue.Message == "You must write ContentLength bytes to the request stream before calling [Begin]GetResponse.")
                {
                    delayservice.ThreadSleep(TimeSpan.FromSeconds(5));
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
                            response = instaFunct.Like(DominatorAccountModel, AccountModel, JobCancellationTokenSource.Token, DominatorAccountModel.IsRunProcessThroughBrowser ? instagramPost.Code : instagramPost.Pk, instagramPost.User.Username, instagramPost.User.Pk, scrapeResult.QueryInfo).Result;
                            if (response != null && response.Success)
                            {
                                GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultPost.Code);
                                IncrementCounters();
                                AddLikedDataToDataBase(scrapeResult);
                                // Do after Like action
                                DoAfterLikeAction(instagramPost);
                                jobProcessResult.IsProcessSuceessfull = true;
                            }
                            else
                            {
                                RemoveFailedLikedDataFromDataBase(scrapeResult);
                                if (!CheckResponse.CheckProcessResponse(response, DominatorAccountModel, ActivityType, scrapeResult, ref _actionBlockedCount, delay))
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
                        RemoveFailedLikedDataFromDataBase(scrapeResult);
                        if (!CheckResponse.CheckProcessResponse(response, DominatorAccountModel, ActivityType, scrapeResult, ref _actionBlockedCount, delay))
                        {
                            Stop();
                            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        }
                        jobProcessResult.IsProcessSuceessfull = false;
                    }
                }
                if (ModuleSetting.EnableDelayBetweenPerformingActionOnSamePost)
                    DelayBeforeNextActivity(ModuleSetting.DelayBetweenPerformingActionOnSamePost.GetRandom());
                else
                    DelayBeforeNextActivity();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }


            return jobProcessResult;
        }

        private bool SkipBlackListWhiteListUser(InstagramUser user)
        {
            try
            {
                if(user != null && LikeModel.SkipBlacklist.IsSkipBlackListUsers && (LikeModel.SkipBlacklist.IsSkipPrivateBlackListUser || LikeModel.SkipBlacklist.IsSkipGroupBlackListUsers))
                {
                    var blackListWhitelistHandler = new BlackListWhitelistHandler(ModuleSetting, DominatorAccountModel, ActivityType);
                    return blackListWhitelistHandler.GetBlackListUsers().Any(x=>x==user?.Username);
                }
                return false;
            }
            catch { return false; }
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
        private void AddLikedDataToDataBase(ScrapeResultNew scrapeResult)
        {
            InstagramPost instagramPost = (InstagramPost)scrapeResult.ResultPost;
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            // Add data to respected campaign InteractedPosts table
            if (!string.IsNullOrEmpty(CampaignId))
            {

                if (ModuleSetting.EnableDelayBetweenPerformingActionOnSamePost || ModuleSetting.IsPerformActionFromRandomPercentageOfAccounts)
                {
                    string permalink = instagramPost.Code.GetUrlFromCode();

                    var interactedPost =
                        CampaignDbOperation.GetSingle<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts>(
                            x => x.Permalink == permalink && x.ActivityType == ActivityType &&
                                 x.Username == DominatorAccountModel.AccountBaseModel.UserName &&
                                 (x.Status == "Pending" || x.Status == "Working"));

                    if (interactedPost != null)
                    {
                        interactedPost.InteractionDate = DateTimeUtilities.GetEpochTime();
                        interactedPost.MediaType = instagramPost.MediaType;
                        interactedPost.ActivityType = ActivityType.Like;
                        interactedPost.PkOwner = instagramPost.Code;
                        interactedPost.UsernameOwner = instagramPost.User.Username;
                        interactedPost.Username = DominatorAccountModel.AccountBaseModel.UserName;
                        interactedPost.QueryType = scrapeResult.QueryInfo.QueryType;
                        interactedPost.QueryValue = scrapeResult.QueryInfo.QueryValue;
                        interactedPost.Status = "Success";
                        CampaignDbOperation.Update(interactedPost);
                    }
                }
                else
                {
                    CampaignDbOperation?.Add(new DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts
                    {
                        InteractionDate = DateTimeUtilities.GetEpochTime(),
                        MediaType = instagramPost.MediaType,
                        ActivityType = ActivityType.Like,
                        PkOwner = instagramPost.Code,
                        UsernameOwner = instagramPost.User.Username,
                        Username = DominatorAccountModel.AccountBaseModel.UserName,
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        Status = "Success"
                    });
                }
            }
            // Add data to respected Account InteractedPosts table
            AccountDbOperation.Add(
                new InteractedPosts()
                {
                    InteractionDate = DateTimeUtilities.GetEpochTime(),
                    MediaType = instagramPost.MediaType,
                    ActivityType = ActivityType.Like,
                    PkOwner = instagramPost.Code,
                    UsernameOwner = instagramPost.User.Username,
                    Username = DominatorAccountModel.AccountBaseModel.UserName,
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    Status = "Success"
                });
        }

        private void DoAfterLikeAction(InstagramPost instagramPost)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            try
            {
                if (LikeModel.ChkEnableLikeCommentsAfterPostIsLiked)
                {
                    int commentLikeCount = LikeModel.CommentToBeLikeAfterEachLikedPost.GetRandom();

                    int likeOnCommentCount = 0;
                    string responsemaxId = null;

                    while (true)
                    {
                        var IsBrowser = DominatorAccountModel.IsRunProcessThroughBrowser;
                        var browser = GramStatic.IsBrowser;
                        var mediaComments = 
                            browser ?
                            instaFunct.GdBrowserManager.GetMediaComments(DominatorAccountModel, $"{Constants.gdHomeUrl}/p/{instagramPost.Code}", JobCancellationTokenSource.Token)
                            : instaFunct.GetMediaComments(DominatorAccountModel, DominatorAccountModel.IsRunProcessThroughBrowser ? instagramPost.Code : instagramPost.Pk, JobCancellationTokenSource.Token, responsemaxId);
                        if (mediaComments.CommentList.Count == 0)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram,
                                DominatorAccountModel.UserName, ActivityType, $"No comment found for media code : {instagramPost.Code}");
                            return;
                        }

                        foreach (var commentDetails in mediaComments.CommentList)
                        {
                            var LikeResponse = instaFunct.LikeOnComment(DominatorAccountModel, AccountModel, JobCancellationTokenSource.Token, instagramPost.Code, commentDetails.CommentId).Result;
                            if (LikeResponse.Success)
                            {
                                likeOnCommentCount++;
                                //  CommentAfterLikeAction(response, instagramPost,scrapeResult);
                                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram,
                                    DominatorAccountModel.UserName, ActivityType, $"Successfully liked comment having id {instagramPost.Pk}");
                            }
                            else
                                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, DominatorAccountModel.UserName, ActivityType, $"Failed to like comment having id {instagramPost.Pk} with error : {LikeResponse.Issue.Message}");

                            DelatBetweenAfterLikeProcesses();

                            if (likeOnCommentCount >= commentLikeCount)
                                break;
                        }
                        if ((likeOnCommentCount >= commentLikeCount) || string.IsNullOrEmpty(mediaComments.MaxId))
                            break;


                        responsemaxId = mediaComments.MaxId;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DelatBetweenAfterLikeProcesses()
        {
            int delay = LikeModel.DelayBetweenLikeComments.GetRandom();
            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Delaying process for {delay} second{(delay > 1 ? "s" : "")} for After activity : Comment like process");
            delayservice.ThreadSleep(TimeSpan.FromSeconds(delay));//Thread.Sleep(delay * 1000);
        }

        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {

        }


        public void RemoveFailedLikedDataFromDataBase(ScrapeResultNew scrapeResult)
        {
            try
            {
                var dboperationCampaign = new DbOperations(CampaignId, SocialNetworks.Instagram, ConstantVariable.GetCampaignDb);
                InstagramPost post = (InstagramPost)scrapeResult.ResultPost;

                var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleConfiguration = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];
                if (moduleConfiguration != null && moduleConfiguration.IsTemplateMadeByCampaignMode)
                {
                    if (ModuleSetting.EnableDelayBetweenPerformingActionOnSamePost || ModuleSetting.IsPerformActionFromRandomPercentageOfAccounts)
                    {
                        string permalink = post.Code.GetUrlFromCode();

                        var interactedPost = dboperationCampaign.GetSingle<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts>(
                            x => x.Permalink == permalink && x.Username == DominatorAccountModel.AccountBaseModel.UserName && (x.Status == "Pending" || x.Status == "Working"));
                        if (interactedPost != null)
                            dboperationCampaign.Remove(interactedPost);

                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
