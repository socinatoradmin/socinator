using ThreadUtils;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
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
using System.Linq;
using System.Net;
using GramDominatorCore.Utility;

namespace GramDominatorCore.GDLibrary.Process
{
    public class ReplyCommentProcess :GdJobProcessInteracted<InteractedPosts>
    {

        public ReplyCommentModel ReplyCommentModel { get; set; }
        private int ActionBlockedCount;
        public ReplyCommentProcess(IProcessScopeModel processScopeModel,
          IDbAccountServiceScoped accountServiceScoped, IGdQueryScraperFactory queryScraperFactory, IGdHttpHelper httpHelper, IGdLogInProcess logInProcess, IGdBrowserManager gdBrowser, IDelayService _delayService) :
          base(processScopeModel, accountServiceScoped, queryScraperFactory, httpHelper, gdBrowser, _delayService)
        {
            ReplyCommentModel = JsonConvert.DeserializeObject<ReplyCommentModel>(templateModel.ActivitySettings);
            loginProcess = logInProcess;
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"{scrapeResult.ResultPost.Code}/c/{scrapeResult.ResultPostComment.CommentId}");
            JobProcessResult jobProcessResult = new JobProcessResult();          
            try
            {
                var comments = ReplyCommentModel.LstDisplayManageCommentModel.Last().CommentText;
                if (ModuleSetting.IsChkMakeCaptionAsSpinText)
                    comments = " " + SpinTexHelper.GetSpinText(comments) + " ";
                var browser = GramStatic.IsBrowser;
                retry:
                var PostUrl = $"{Constants.gdHomeUrl}/p/{scrapeResult.ResultPost.Code}/c/{scrapeResult.ResultPostComment.CommentId}";
                var Post = scrapeResult?.ResultPostComment as ResultCommentItemUser;
                var response = 
                    browser ?
                    instaFunct.GdBrowserManager.ReplyComment(DominatorAccountModel, PostUrl, comments, JobCancellationTokenSource.Token)
                    : instaFunct.ReplyComment(DominatorAccountModel, AccountModel, scrapeResult.ResultPostComment.CommentId, comments, scrapeResult.ResultPost?.Code, JobCancellationTokenSource.Token,Post?.ItemUser?.Username).Result;
                if (response != null && response.Success)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"{Constants.gdHomeUrl}/p/{scrapeResult.ResultPost.Code}/c/{scrapeResult.ResultPostComment.CommentId}");

                    IncrementCounters();

                    AddCommentedDataToDataBase(scrapeResult, comments);

                    jobProcessResult.IsProcessSuceessfull = true;

                }
                else if (response != null && (response.ToString().Contains("Comments on this post have been limited") || response?.Issue?.Message == "CommentLimited"))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                             $"{scrapeResult.ResultPost.Code} Comments on this post have been limited by instagram, Please comment on diffrent post");
                    return jobProcessResult;
                }
                else if (!response.Success && response.Issue != null && response?.Issue?.Message == "You must write ContentLength bytes to the request stream before calling [Begin]GetResponse.")
                {
                    delayservice.ThreadSleep(TimeSpan.FromSeconds(5));
                    goto retry;
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

                            CheckOffensiveCommentResponseHandler checkOffensiveComment = instaFunct.CheckOffensiveComment(DominatorAccountModel, AccountModel, JobCancellationTokenSource.Token, scrapeResult.ResultPost.Id, comments);
                            if (checkOffensiveComment.is_offensive)
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Your Comment is Offensive, please change your Comment");
                                return jobProcessResult;
                            }
                            response = instaFunct.ReplyComment(DominatorAccountModel, AccountModel, scrapeResult.ResultPostComment.CommentId, comments, scrapeResult.ResultPost.Id, JobCancellationTokenSource.Token,Post?.ItemUser?.Username).Result;
                            if (response != null && response.Success)
                            {
                                GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultPost.Code);

                                IncrementCounters();

                                AddCommentedDataToDataBase(scrapeResult, comments);

                                jobProcessResult.IsProcessSuceessfull = true;
                            }
                            else if (response != null && (response.ToString().Contains("Comments on this post have been limited") || response?.Issue?.Message == "CommentLimited"))
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                         $"{scrapeResult.ResultPost.Code} Comments on this post have been limited by instagram, Please comment on diffrent post");
                                return jobProcessResult;
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
                DelayBeforeNextActivity();
            }
            catch (Exception)
            {
               
            }
            return jobProcessResult;
        }

        private void AddCommentedDataToDataBase(ScrapeResultNew scrapeResult, string comment)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            InstagramPost instagramPost = (InstagramPost)scrapeResult.ResultPost;

            // Add data to respected campaign InteractedPosts table
            if (!string.IsNullOrEmpty(CampaignId))
            {
                    CampaignDbOperation?.Add(new DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts()
                    {
                        InteractionDate = DateTimeUtilities.GetEpochTime(),
                        MediaType = instagramPost.MediaType,
                        ActivityType = ActivityType,
                        PkOwner = instagramPost.Code,
                        UsernameOwner = instagramPost?.User?.Username,
                        Username = DominatorAccountModel.AccountBaseModel.UserName,
                        Comment = comment,
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        Status = "Success"
                    });
                
            }

            // Add data to respected Account InteractedPosts table
            AccountDbOperation.Add(
       new InteractedPosts()
       {
           InteractionDate = DateTimeUtilities.GetEpochTime(),
           MediaType = instagramPost.MediaType,
           ActivityType = ActivityType,
           PkOwner = instagramPost.Code,
           UsernameOwner = instagramPost?.User?.Username,
           Username = DominatorAccountModel.AccountBaseModel.UserName,
           Comment = comment,
           QueryType = scrapeResult.QueryInfo.QueryType,
           QueryValue = scrapeResult.QueryInfo.QueryValue
       });
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
