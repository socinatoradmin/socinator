using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.DatabaseHandler.GdTables;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.Factories;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.Request;
using GramDominatorCore.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using GramDominatorCore.Utility;

namespace GramDominatorCore.GDLibrary
{
    public class FollowProcess : GdJobProcessInteracted<InteractedUsers>
    {
        private int UserCountToLike { get; set; }

        private int UserCountToComment { get; set; }

        private int UserCountToMessage { get; set; }

        private readonly BlackListWhitelistHandler _blackListWhitelistHandler;

        public int CommentCountFromPercentage { get; set; }

        public int MessageCountFromPercentage { get; set; }

        public FollowerModel FollowerModel { get; set; }

        private int _actionBlockedCount;

        bool _isFollowingCount;

        private int _untillSpecificFollowingCount;
        public FollowProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IGdQueryScraperFactory queryScraperFactory, IGdHttpHelper httpHelper, IGdLogInProcess logInProcess, IGdBrowserManager gdBrowser, IDelayService _delayService)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, httpHelper, gdBrowser, _delayService)
        {
            FollowerModel = JsonConvert.DeserializeObject<FollowerModel>(templateModel.ActivitySettings);
            _blackListWhitelistHandler = new BlackListWhitelistHandler(ModuleSetting, DominatorAccountModel, ActivityType);
             InitializePerDayAndPercentageCount();
            loginProcess = logInProcess;
        }



        /// <summary>
        /// Initializes per day count actions (like, comment, message) 
        /// and percentages for individual campaigns
        /// </summary>
        private void InitializePerDayAndPercentageCount()
        {
            UserCountToLike = FollowerModel.LikeMaxBetween.GetRandom();
            UserCountToComment = FollowerModel.Comments.GetRandom();
            UserCountToMessage = FollowerModel.MessageBetween.GetRandom();

            if (FollowerModel.IsChkCommentPercentage)
            {
                // Calculates comments percentage from max job count
                int commentCount = Utilities.PercentageCalculator(JobConfiguration.ActivitiesPerJob.EndValue,
                    FollowerModel.CommentPercentage);
                CommentCountFromPercentage = commentCount == 0 ? 1 : commentCount;
            }

            if (FollowerModel.IsChkDirectMessagePercentage)
            {
                // Calculates message percentage from max job count
                int messageCount = Utilities.PercentageCalculator(JobConfiguration.ActivitiesPerJob.EndValue,
                    FollowerModel.DirectMessagePercentage);
                MessageCountFromPercentage = messageCount == 0 ? 1 : messageCount;
            }
        }

        public override DominatorHouseCore.Process.JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            int delay = ModuleSetting.DelayBetweenEachActionBlock.GetRandom();
            string status;
            instaFunct = loginProcess.InstagramFunctFactory.InstaFunctions;
            DominatorHouseCore.Process.JobProcessResult jobProcessResult = new DominatorHouseCore.Process.JobProcessResult();
            try
            {
                InstagramUser instagramUser = (InstagramUser)scrapeResult.ResultUser;
                InstagramPost instagramPost = (InstagramPost)scrapeResult?.ResultPost;
                if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    AccountModel.WwwClaim = AccountModel.WwwClaim ?? "0";
                    AccountModel.WwwClaim = instaFunct.GetGdHttpHelper().Response.Headers["x-ig-set-www-claim"] ?? AccountModel.WwwClaim;

                    #region Check for Auto Follow/ Unfollow process
                    CheckAutoFollowUnfollowProcess();
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    #endregion
                    #region Check for campaignwise Unique Follow user
                    var  IsCampaignWiseUnique = CheckForCampaignWiseUnique(instagramUser.Username);
                    #endregion
                }
                if (CheckForCampaignWiseUnique(instagramUser.Username))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    $"Performed {ActivityType} operation by campaign {CampaignId} uniquely.");
                    if (scrapeResult.QueryInfo.QueryType == "Custom Users List")
                    {
                        jobProcessResult.IsProcessCompleted = true;
                        jobProcessResult.HasNoResult = true;
                    }
                    jobProcessResult.IsProcessSuceessfull = false;
                    return jobProcessResult;
                }

                GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultUser.Username);
                if (DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    var gdBrowserManager = instaFunct.GdBrowserManager;
                    FriendshipsResponse response =
                        GramStatic.IsBrowser?
                        gdBrowserManager.Follow(DominatorAccountModel, JobCancellationTokenSource.Token, instagramUser, instagramPost != null ? instagramPost.Id : "")
                        : instaFunct.Follow(DominatorAccountModel,new AccountModel(DominatorAccountModel), JobCancellationTokenSource.Token, instagramUser.Pk,instagramUser.Username);
                    var visited = gdBrowserManager.VisitPage(DominatorAccountModel, $"https://www.instagram.com/{instagramUser?.Username}/").Result;
                    if ((response != null && response.Success) || (response.Following || response.IsPrivate ||response.OutgoingRequest))
                    {
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        if (response.IsPrivate || response.OutgoingRequest)
                        {
                            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    $"Successfully requested To {scrapeResult.ResultUser.Username}");
                            status = "Requested";
                            instagramUser.IsPrivate = response.IsPrivate;
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultUser.Username);
                            status = "Followed";
                            instagramUser.IsFollowing = response.Following;
                        }

                        IncrementCounters();
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        AddFollowedDataToDataBase(scrapeResult, response, status);
                        AccountModel.LstFollowings.Add(instagramUser);
                        if (FollowerModel.ManageBlackWhiteListModel.IsAddToBlackListOnceUnfollowed)
                            _blackListWhitelistHandler.AddToBlackList(instagramUser.UserId, instagramUser.Username);

                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        // Do after Follow activity
                        DoAfterFollowAction(instagramUser, scrapeResult, DominatorAccountModel, AccountModel);

                        jobProcessResult.IsProcessSuceessfull = true;
                    }
                    else if(response != null && !string.IsNullOrEmpty(response.ErrorMessage) && response.ErrorMessage.Contains(ConstantHelpDetails.ActivityLimitMessage))
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,$"\n {response.ErrorMessage}");
                        var schedular = InstanceProvider.GetInstance<IDominatorScheduler>();
                        if (schedular != null)
                            schedular.ChangeAccountsRunningStatus(false, DominatorAccountModel.AccountId, ActivityType);
                    }else if(response != null && !string.IsNullOrEmpty(response.ErrorMessage) && response.ErrorMessage.Contains("Can't Follow Self Account"))
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $" {response.ErrorMessage}");
                    }
                }
                else
                {
                retry:
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    FriendshipsResponse response = instaFunct.Follow(DominatorAccountModel, AccountModel, JobCancellationTokenSource.Token, DominatorAccountModel.IsRunProcessThroughBrowser ? instagramUser.Username : instagramUser.Pk, instagramPost != null ? instagramPost.Id : "");
                    if (response != null && response.Issue != null && response.Issue.Message.Contains("You must provide a request body"))
                    {
                        delayservice.ThreadSleep(TimeSpan.FromSeconds(15));
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        response = instaFunct.Follow(DominatorAccountModel, AccountModel, JobCancellationTokenSource.Token, DominatorAccountModel.IsRunProcessThroughBrowser ? instagramUser.Username : instagramUser.Pk, instagramPost != null ? instagramPost.Id : "");
                    }
                    if ((response != null && response.Success) && (response.Following || response.IsPrivate))
                    {
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        if (response.IsPrivate)
                        {
                            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    $"Successfully requested To {scrapeResult.ResultUser.Username}");
                            status = "Requested";
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultUser.Username);
                            status = "Followed";

                        }

                        IncrementCounters();
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        AddFollowedDataToDataBase(scrapeResult, response, status);
                        AccountModel.LstFollowings.Add(instagramUser);
                        if (FollowerModel.ManageBlackWhiteListModel.IsAddToBlackListOnceUnfollowed)
                            _blackListWhitelistHandler.AddToBlackList(instagramUser.UserId, instagramUser.Username);

                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        // Do after Follow activity
                        DoAfterFollowAction(instagramUser, scrapeResult, DominatorAccountModel, AccountModel);

                        jobProcessResult.IsProcessSuceessfull = true;
                    }
                    else if (response != null && response.IsNotClicked)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                          $"Follow button has not clicked");
                    }
                    else if (response != null && response.Success && !response.Following)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                           $"Sorry, You are unable to follow this user {instagramUser.Username} as your instagram account blocked for follow , please check once manually");
                        Stop();
                    }

                    else if (!response.Success && response.Issue != null && response.Issue.Message == "You must write ContentLength bytes to the request stream before calling [Begin]GetResponse.")
                    {
                        delayservice.ThreadSleep(TimeSpan.FromSeconds(5));
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
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
                        else if (response.ToString().Contains("Action Blocked") && response.ToString().Contains("\"feedback_required\"") || response.ToString().Contains("You've Been Logged Out"))
                        {
                            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            delayservice.ThreadSleep(TimeSpan.FromSeconds(30));
                            bool LoginStatus = false;
                            var BackupCookie = DominatorAccountModel.Cookies;
                            var logOutStatus = instaFunct.Logout(DominatorAccountModel, AccountModel);
                            if (logOutStatus.Success)
                            {
                                ResetCookies(BackupCookie);
                                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                LoginStatus = loginProcess.LoginWithAlternativeMethodForBlocking(DominatorAccountModel);
                            }
                            if (LoginStatus)
                            {
                                delayservice.ThreadSleep(TimeSpan.FromSeconds(10));// Thread.Sleep(TimeSpan.FromSeconds(10));
                                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                response = instaFunct.Follow(DominatorAccountModel, AccountModel, JobCancellationTokenSource.Token, DominatorAccountModel.IsRunProcessThroughBrowser ? instagramUser.Username : instagramUser.Pk, instagramPost != null ? instagramPost.Id : "");
                                if ((response != null && response.Success) && (response.Following || response.IsPrivate))
                                {
                                    if (response.IsPrivate)
                                    {
                                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                                $"Successfully requested To {scrapeResult.ResultUser.Username}");
                                        status = "Requested";
                                    }
                                    else
                                    {
                                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                        GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultUser.Username);
                                        status = "Followed";

                                    }

                                    IncrementCounters();

                                    AddFollowedDataToDataBase(scrapeResult, response, status);
                                    AccountModel.LstFollowings.Add(instagramUser);
                                    if (FollowerModel.ManageBlackWhiteListModel.IsAddToBlackListOnceUnfollowed)
                                    {
                                        // _blackListWhitelistHandler = new BlackListWhitelistHandler(ModuleSetting, DominatorAccountModel, ActivityType.BlockFollower);
                                        _blackListWhitelistHandler.AddToBlackList(instagramUser.UserId, instagramUser.Username);
                                    }
                                    // Do after Follow activity
                                    DoAfterFollowAction(instagramUser, scrapeResult, DominatorAccountModel, AccountModel);

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
                            if (!CheckResponse.CheckProcessResponse(response, DominatorAccountModel, ActivityType, scrapeResult, ref _actionBlockedCount, delay))
                            {
                                Stop();
                                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            }
                            jobProcessResult.IsProcessSuceessfull = false;
                        }
                    }
                }
                
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                // Delay between each activity
                DelayBeforeNextActivity();
                if (scrapeResult.QueryInfo.QueryType == "Custom Users List" && jobProcessResult.IsProcessSuceessfull)
                {
                    jobProcessResult.IsProcessCompleted = true;
                    jobProcessResult.HasNoResult = true;
                }
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
            catch (Exception)
            {
                //ex.DebugLog();
            }
            return jobProcessResult;
        }

        private bool CheckForCampaignWiseUnique(string UserName)
        {
            var IsCampaignWiseUnique = false;
            try
            {
                var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleConfiguration = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];
                if (moduleConfiguration != null && (moduleConfiguration.IsTemplateMadeByCampaignMode && FollowerModel.ChkFollowUniqueUsersInCampaign))
                {
                    var instance = InstanceProvider.GetInstance<ICampaignInteractionDetails>();
                    instance.AddInteractedData(SocialNetworks, CampaignId, UserName);
                }
            }
            catch (Exception)
            {
                IsCampaignWiseUnique = true;
            }
            return IsCampaignWiseUnique;
        }

        private bool IsPostFollowProcessCompleted(ActivityType activityType)
        {
            var dboperationCampaign = new DbOperations(CampaignId, SocialNetworks.Instagram, ConstantVariable.GetCampaignDb);
            if (activityType == ActivityType.Like)
            {
                List<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts> lstInteractedPosts =
                    dboperationCampaign.Get<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts>(x =>
                        x.Username == DominatorAccountModel.UserName && x.ActivityType == activityType);
                if (FollowerModel.IsChkMaxLike && (UserCountToLike <= lstInteractedPosts.Count))
                    return true;
            }
            else if (activityType == ActivityType.Comment)
            {
                List<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts> lstInteractedPosts =
                    dboperationCampaign.Get<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts>(x =>
                        x.Username == DominatorAccountModel.UserName && x.ActivityType == activityType);
                if (FollowerModel.IsChkMaxComment && (UserCountToComment <= lstInteractedPosts.Count))
                    return true;

                if (FollowerModel.IsChkCommentPercentage && (CommentCountFromPercentage <= lstInteractedPosts.Count))
                    return true;
            }
            else if (activityType == ActivityType.BroadcastMessages)
            {
                string activity = ActivityType.BroadcastMessages.ToString();
                List<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedUsers> lstInteractedUser =
                    dboperationCampaign.Get<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedUsers>(x =>
                        x.Username == DominatorAccountModel.UserName && x.ActivityType == activity);
                if (FollowerModel.IsChkMaxMessege && (UserCountToMessage <= lstInteractedUser.Count))
                    return true;
                if (FollowerModel.IsChkDirectMessagePercentage &&
                    (MessageCountFromPercentage <= lstInteractedUser.Count))
                    return true;

            }
            else
                return true;

            return false;
        }

        /// <summary>
        /// After follow action: like followed user's posts
        /// </summary>
        /// <param name="instagramUser"></param>
        private void LikeUsersLatestPosts(InstagramUser instagramUser, ScrapeResultNew scrapeResult)
        {
            try
            {
                int currentLikeCount = 0;

                int likeCountForCurrentUser = FollowerModel.LikeBetweenJobs.GetRandom();
                if (IsPostFollowProcessCompleted(ActivityType.Like))
                    return;

                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, DominatorAccountModel.UserName,
                    ActivityType, "Started Like activity as post after follow action");

                string userFeedMaxId = null;
                var newUserInBrowser = true;
                var browser = GramStatic.IsBrowser;
                while (true)
                {
                    var userFeedDetails = 
                        browser ?
                        instaFunct.GdBrowserManager.GetUserFeed(DominatorAccountModel, instagramUser.Username, JobCancellationTokenSource.Token, likeCountForCurrentUser)
                        : instaFunct.GetUserFeed(DominatorAccountModel, AccountModel,instagramUser.Username, JobCancellationTokenSource.Token, userFeedMaxId, isNewUserBrowser: newUserInBrowser);
                    if (!userFeedDetails.Success)
                        return;

                    List<InstagramPost> lstPosts = userFeedDetails.Items;

                    if (FollowerModel.ChkLikeRandomPostsChecked)
                        lstPosts.Shuffle();

                    foreach (InstagramPost instagramPost in lstPosts)
                    {
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        var likeResponse =
                            browser ?
                            instaFunct.GdBrowserManager.Like(DominatorAccountModel, AccountModel, instagramPost.Code, JobCancellationTokenSource.Token)
                            : instaFunct.Like(DominatorAccountModel, AccountModel, JobCancellationTokenSource.Token, instagramPost.Code , instagramPost.User.Username, instagramPost.User.Pk, scrapeResult.QueryInfo).Result;
                        if (!likeResponse.Success && likeResponse!=null)
                        {
                            delayservice.ThreadSleep(5000);
                            likeResponse = instaFunct.Like(DominatorAccountModel, AccountModel, JobCancellationTokenSource.Token, DominatorAccountModel.IsRunProcessThroughBrowser ? instagramPost.Code : instagramPost.Pk, instagramPost.User.Username, instagramPost.User.Pk, scrapeResult.QueryInfo).Result;
                        }
                        if (likeResponse.Success)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, DominatorAccountModel.UserName, ActivityType, $"Successfully liked {instagramUser.Username}'s post {instagramPost.Code}");

                            // Add data to respected campaign InteractedPosts table
                            if (!string.IsNullOrEmpty(CampaignId))
                            {
                                CampaignDbOperation.Add(new DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts()
                                {
                                    InteractionDate = DateTimeUtilities.GetEpochTime(),
                                    MediaType = instagramPost.MediaType,
                                    ActivityType = ActivityType.Like,
                                    QueryType = scrapeResult.QueryInfo.QueryType,
                                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                                    PkOwner = instagramPost.Code,
                                    UsernameOwner = instagramPost.User.Username,
                                    Username = DominatorAccountModel.AccountBaseModel.UserName,
                                    Status = "Liked"
                                });
                            }

                            // Add data to respected Account InteractedPosts table
                            AccountDbOperation.Add(
                                new InteractedPosts
                                {
                                    InteractionDate = DateTimeUtilities.GetEpochTime(),
                                    MediaType = instagramPost.MediaType,
                                    ActivityType = ActivityType.Follow,
                                    QueryType = scrapeResult.QueryInfo.QueryType,
                                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                                    PkOwner = instagramPost.Code,
                                    UsernameOwner = instagramPost.User.Username,
                                    Username = DominatorAccountModel.AccountBaseModel.UserName,
                                    Status = "Liked"
                                });

                            currentLikeCount++;
                            if (likeCountForCurrentUser <= currentLikeCount)
                                return;
                            DelayBetweenAfterActionProcess(ActivityType.Like);
                        }
                        else
                        {
                            if (!CheckResponse.CheckProcessResponse(likeResponse, DominatorAccountModel, ActivityType.Like, scrapeResult, ref _actionBlockedCount))
                            {
                                FollowerModel.IsChkLikeUsersLatestPost = false;
                                userFeedDetails.MaxId = "";
                                break;
                            }
                        }
                        if (IsPostFollowProcessCompleted(ActivityType.Like) || (likeCountForCurrentUser <= currentLikeCount))
                        {
                            return;
                        }
                    }

                    userFeedMaxId = userFeedDetails.MaxId;
                    if (string.IsNullOrEmpty(userFeedMaxId))
                        break;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void CommentOnUsersLatestPosts(InstagramUser instagramUser, ScrapeResultNew scrapeResult, DominatorAccountModel dominatorAccountModel)
        {
            try
            {
                int currentCommentCount = 0;
                int postCommentCount = FollowerModel.CommentsPerUser.GetRandom();

                if (IsPostFollowProcessCompleted(ActivityType.Comment))
                    return;

                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, DominatorAccountModel.UserName, ActivityType, "Started Comment activity as post after follow action");

                string userFeedMaxId = null;
                FollowerModel.LstComments = Regex.Split(FollowerModel.UploadComment, @"\r\n").ToList();
                UserFeedIgResponseHandler userFeedDetails = null;
                var browser = GramStatic.IsBrowser;
                while (true)
                {
                    userFeedDetails = 
                        browser ?
                        this.instaFunct.GdBrowserManager.GetUserFeed(DominatorAccountModel, instagramUser.Username, JobCancellationTokenSource.Token, postCommentCount)
                        : instaFunct.GetUserFeed(DominatorAccountModel, AccountModel, instagramUser.Username, JobCancellationTokenSource.Token, userFeedMaxId);
                    if (!userFeedDetails.Success)
                        return;

                    List<InstagramPost> lstPosts = userFeedDetails.Items;

                    foreach (InstagramPost instagramPost in lstPosts)
                    {
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        string comment = FollowerModel.LstComments.GetRandomItem();
                        var commentResponse = browser ?
                            instaFunct.GdBrowserManager.Comment(DominatorAccountModel, AccountModel, instagramPost.Code, comment, JobCancellationTokenSource.Token)
                            : instaFunct.Comment(DominatorAccountModel, AccountModel, JobCancellationTokenSource.Token, dominatorAccountModel.IsRunProcessThroughBrowser ? instagramPost.Code : instagramPost.Pk, comment).Result;
                        if (commentResponse.Success)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, DominatorAccountModel.UserName, ActivityType, $"Successfully commented on {instagramUser.Username}'s post {instagramPost.Code}");

                            // Add data to respected campaign InteractedPosts table
                            if (!string.IsNullOrEmpty(CampaignId))
                            {
                                CampaignDbOperation?
                                    .Add(
                                        new DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts()
                                        {
                                            InteractionDate = DateTimeUtilities.GetEpochTime(),
                                            MediaType = instagramPost.MediaType,
                                            ActivityType = ActivityType.Comment,
                                            PkOwner = instagramPost.Code,
                                            QueryType = scrapeResult.QueryInfo.QueryType,
                                            QueryValue = scrapeResult.QueryInfo.QueryValue,
                                            UsernameOwner = instagramPost.User.Username,
                                            Username = DominatorAccountModel.AccountBaseModel.UserName,
                                            Comment = comment,
                                            Status = "Commented"
                                        });
                            }

                            // Add data to respected account InteractedPosts table
                            AccountDbOperation.Add(
                                new InteractedPosts
                                {
                                    InteractionDate = DateTimeUtilities.GetEpochTime(),
                                    MediaType = instagramPost.MediaType,
                                    ActivityType = ActivityType.Follow,
                                    QueryType = scrapeResult.QueryInfo.QueryType,
                                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                                    PkOwner = instagramPost.Code,
                                    UsernameOwner = instagramPost.User.Username,
                                    Username = DominatorAccountModel.AccountBaseModel.UserName,
                                    Status = "Commented",
                                    Comment = comment
                                });
                            currentCommentCount++;
                            if (postCommentCount <= currentCommentCount)
                                return;
                            DelayBetweenAfterActionProcess(ActivityType.Comment);
                        }
                        else
                        {
                            if (!CheckResponse.CheckProcessResponse(commentResponse, DominatorAccountModel, ActivityType.Comment, scrapeResult, ref _actionBlockedCount))
                                FollowerModel.ChkCommentOnUserLatestPostsChecked = false;
                            
                        }

                        if (IsPostFollowProcessCompleted(ActivityType.Comment) || (postCommentCount <= currentCommentCount))
                            return;
                    }
                    userFeedMaxId = userFeedDetails.MaxId;
                    if (string.IsNullOrEmpty((userFeedMaxId)))
                        break;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        private void MuteFollowerPost(InstagramUser instagramUser, DominatorAccountModel dominatorAccountModel, AccountModel accountModel)
        {

            CommonIgResponseHandler commonIgResponseHandler;
            if (FollowerModel.IsCheckedFollowerPostAfterFollow)
            {
                commonIgResponseHandler = instaFunct.MuteFollowerPostUser(instagramUser, dominatorAccountModel, accountModel);
                if (commonIgResponseHandler.Success)
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, DominatorAccountModel.UserName, ActivityType, $"Successfully {instagramUser.Username} post mute ");
                    
            }

            if (FollowerModel.IsCheckedFollowerStoryAfterFollow)
            {
                commonIgResponseHandler = instaFunct.MuteFollowerStoryUser(instagramUser, dominatorAccountModel, accountModel);
                if (commonIgResponseHandler.Success)
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, DominatorAccountModel.UserName, ActivityType, $"Successfully {instagramUser.Username} Story mute ");

            }

            if (FollowerModel.IsCheckedFollowerPostAndStoryAfterFollow)
            {
                commonIgResponseHandler = instaFunct.MuteFollowerPostUser(instagramUser, dominatorAccountModel, accountModel);
                if (commonIgResponseHandler.Success)
                {
                    commonIgResponseHandler = instaFunct.MuteFollowerStoryUser(instagramUser, dominatorAccountModel, accountModel);
                    if (commonIgResponseHandler.Success)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, DominatorAccountModel.UserName, ActivityType, $"Successfully {instagramUser.Username} post and story mute ");
                    }
                }

            }

        }
        /// <summary>
        /// Sends direct message to instagram user
        /// </summary>
        /// <param name="instagramUser">User to send message to</param>
        private void SendMessageToUser(InstagramUser instagramUser, ScrapeResultNew scrapeResult, AccountModel accountModel)
        {
            try
            {
                if (IsPostFollowProcessCompleted(ActivityType.BroadcastMessages))
                    return;
                FollowerModel.LstMessages = Regex.Split(FollowerModel.Message, @"\r\n").ToList();
                SendMessageIgResponseHandler messageResponse = null;
                string message = FollowerModel.Message;
                string mediaPath = FollowerModel.MediaPath;
                string ThreadId = string.Empty;
                var browser = GramStatic.IsBrowser;
                if (ModuleSetting.IsChkMakeCaptionAsSpinText)
                    message = " " + SpinTexHelper.GetSpinText(message) + " ";
                #region Get ThreadID for send message
                //var lstConversationUserList = DbAccountService.GetConversationUser().ToList();
                //if (lstConversationUserList.Any(x => x.SenderName == instagramUser.Username))
                //{
                //    var UserThread = lstConversationUserList.Where(x => x.SenderName == instagramUser.Username).ToList();
                //    ThreadId = UserThread[0].ThreadId;
                //}
                #endregion

                #region Skip Already Interacted.


                var alreadyInteractedUsers = CampaignDbOperation.Get<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedUsers>()?.Where(x=>x.ActivityType == ActivityType.BroadcastMessages.ToString() && x.Status == "Sent Message")?.ToList();
                if(alreadyInteractedUsers !=null && alreadyInteractedUsers.Any(z=>z.Username == instagramUser.Username))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, DominatorAccountModel.UserName, ActivityType, $"Skipped {instagramUser.Username} As Interacted User For An Activity Send Message.");
                    return;
                }
                #endregion
                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, DominatorAccountModel.UserName, ActivityType, "Started Message activity as post after follow action");
                {
                    var userInfo = 
                        browser ?
                        instaFunct.GdBrowserManager.GetUserInfo(DominatorAccountModel, instagramUser.Username, JobCancellationTokenSource.Token)
                        : instaFunct.SearchUserInfoById(DominatorAccountModel,new AccountModel(DominatorAccountModel), instagramUser.Username, JobCancellationTokenSource.Token).Result;
                    if (userInfo != null && userInfo.Success)
                        instagramUser.UserDetails = userInfo.instaUserDetails;
                    if (instagramUser != null && instagramUser.UserDetails != null && string.IsNullOrEmpty(instagramUser.UserDetails.ThreadId))
                    {
                        var threadIDDetails = instaFunct.GetThreadID(DominatorAccountModel, instagramUser.UserId ?? instagramUser.Pk, instagramUser.Username).Result;
                        if(userInfo != null && !userInfo.CanMessage)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, DominatorAccountModel.UserName, ActivityType, $"User is not allowed to message");
                            return;
                        }
                        instagramUser.UserDetails.ThreadId = threadIDDetails?.ThreadId;
                    }
                    messageResponse = 
                        browser ?
                        instaFunct.GdBrowserManager.SendMessage(DominatorAccountModel, instagramUser, instagramUser.UserDetails.ThreadId, message, mediaPath, JobCancellationTokenSource.Token)
                        : instaFunct.SendMessage(DominatorAccountModel,new AccountModel(DominatorAccountModel), instagramUser?.Pk,message, instagramUser.UserDetails.ThreadId, JobCancellationTokenSource.Token).Result;
                    if(!string.IsNullOrEmpty(mediaPath))
                        messageResponse = instaFunct.SendPhotoAsDirectMessage(DominatorAccountModel, new AccountModel(DominatorAccountModel), JobCancellationTokenSource.Token, instagramUser?.Pk, mediaPath,messageResponse?.ThreadId ?? instagramUser?.UserDetails?.ThreadId).Result;
                }

                if (messageResponse != null && messageResponse.Success)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, DominatorAccountModel.UserName, ActivityType, $"Successfully sent message to {instagramUser.Username}");

                    if (!string.IsNullOrEmpty(CampaignId))
                    {
                        // Add data to respected campaign InteractedUsers table
                        CampaignDbOperation.Add(new DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedUsers()
                        {
                            ActivityType = ActivityType.BroadcastMessages.ToString(),
                            Date = DateTimeUtilities.GetEpochTime(),
                            DirectMessage = message,
                            InteractedUsername = instagramUser.Username,
                            InteractedUserId = instagramUser.Pk,
                            Username = DominatorAccountModel.AccountBaseModel.UserName,
                            QueryType = scrapeResult.QueryInfo.QueryType,
                            Query = scrapeResult.QueryInfo.QueryValue,
                            Status = "Sent Message"
                        });
                    }

                    // Add data to respected account InteractedUsers table
                    AccountDbOperation.Add(
                        new InteractedUsers()
                        {
                            ActivityType = ActivityType.Follow.ToString(),
                            Date = DateTimeUtilities.GetEpochTime(),
                            DirectMessage = message,
                            InteractedUsername = instagramUser.Username,
                            InteractedUserId = instagramUser.Pk,
                            Username = DominatorAccountModel.AccountBaseModel.UserName,
                            QueryType = scrapeResult.QueryInfo.QueryType,
                            Query = scrapeResult.QueryInfo.QueryValue,
                            Status = "Sent Message"
                        });
                }
                else
                {
                    if (!CheckResponse.CheckProcessResponse(messageResponse, DominatorAccountModel, ActivityType.BroadcastMessages, scrapeResult, ref _actionBlockedCount))
                        FollowerModel.ChkSendDirectMessageAfterFollowChecked = false;                   
                }
            }
            catch (Exception ex)
            {
                ex.TraceLog();
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

                if (FollowerModel.IsChkEnableAutoFollowUnfollowChecked)
                {
                    if (FollowerModel.IsCheckedStopFollowStartUnfollow)
                    {
                        #region Stop Follow and Start Unfollow activity
                        if (IsStartAutoFollowUnfollow())
                        {
                            try
                            {
                                var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                                return dominatorScheduler.EnableDisableModules(ActivityType.Follow, ActivityType.Unfollow, DominatorAccountModel.AccountId);
                            }
                            catch (InvalidOperationException ex)
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork, ActivityType,
                                    DominatorAccountModel.UserName,
                                    ex.Message.Contains("1001")
                                        ?
                                        "Follow activity has set your Auto Enable configuration for Unfollow, but you do not have Unfollow configuration saved. Please save the Unfollow configuration manually, to restart the Follow/Unfollow activity from this account"
                                        : "");
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }
                        }
                        #endregion
                    }
                    else if (FollowerModel.IsChkOnlyStopFollowTool)
                    {
                        #region Only stop Follow activity
                        if (IsStartAutoFollowUnfollow())
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, ActivityType, DominatorAccountModel.UserName, "Your Stop follow tool limit has been reached");
                            var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                            return dominatorScheduler.ChangeAccountsRunningStatus(false, AccountId, ActivityType);

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

        private void DelayBetweenAfterActionProcess(ActivityType activityTypeAfterFollow)
        {
            int delay;

            switch (activityTypeAfterFollow)
            {
                case ActivityType.Like:
                    delay = FollowerModel.DelayBetweenLikesForAfterActivity.GetRandom();
                    break;
                case ActivityType.Comment:
                    delay = FollowerModel.DelayBetweenCommentsForAfterActivity.GetRandom();
                    break;
                case ActivityType.BroadcastMessages:
                    delay = FollowerModel.DelayBetweenMessagesForAfterActivity.GetRandom();
                    break;
                default:
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"No such activity found for AfterActivity : {activityTypeAfterFollow} process");
                    return;
            }

            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Next Process For {activityTypeAfterFollow} Will Start After {delay} Second{(delay > 1 ? "s" : "")}");

            delayservice.ThreadSleep(TimeSpan.FromSeconds(delay));//Thread.Sleep(delay * 1000);
        }


        private bool IsStartAutoFollowUnfollow()
        {
            if (!_isFollowingCount)
            {
                _untillSpecificFollowingCount = FollowerModel.StopFollowToolWhenReachSpecifiedFollowings.GetRandom();
                _isFollowingCount = true;
            }
            try
            {
                if (FollowerModel.IsChkStopFollowToolWhenReachedSpecifiedFollowings ||
                    FollowerModel.IsChkWhenFollowerFollowingsIsSmallerThanChecked)
                {
                    var followersCount = DominatorAccountModel.DisplayColumnValue1;

                    var followingsCount = instaFunct.SearchUserInfoById(DominatorAccountModel,AccountModel,
                        DominatorAccountModel.AccountBaseModel.UserId, JobCancellationTokenSource.Token).Result;
                    delayservice.ThreadSleep(TimeSpan.FromSeconds(3));//Thread.Sleep(TimeSpan.FromSeconds(3));
                    if (FollowerModel.IsChkStopFollowToolWhenReachedSpecifiedFollowings && _untillSpecificFollowingCount <= followingsCount.FollowingCount)
                        return true;

                    if (FollowerModel.IsChkWhenFollowerFollowingsIsSmallerThanChecked)
                    {
                        var followerFollwingRatio = followersCount / followingsCount.FollowingCount;
                        if (followerFollwingRatio < FollowerModel.FollowerFollowingsMaxValue)
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

        /// <summary>
        /// Checkes whether we have to like or comment latest post,  or send direct message
        /// </summary>
        /// <param name="instagramUser"></param>
        private void DoAfterFollowAction(InstagramUser instagramUser, ScrapeResultNew scrapeResult, DominatorAccountModel dominatorAccountModel, AccountModel accountModel)
        {
            try
            {
                if (!instagramUser.IsFollowing && instagramUser.IsPrivate && FollowerModel.IsChkLikeUsersLatestPost)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, DominatorAccountModel.UserName, ActivityType, $"This user {instagramUser.Username} is private so we can not like post");
                    return;
                }
                if (!instagramUser.IsFollowing && instagramUser.IsPrivate && FollowerModel.ChkCommentOnUserLatestPostsChecked)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, DominatorAccountModel.UserName, ActivityType, $"This user {instagramUser.Username} is private so we can not Comment on post");
                    return;
                }
                if (FollowerModel.IsChkLikeUsersLatestPost || FollowerModel.ChkCommentOnUserLatestPostsChecked ||
                    FollowerModel.ChkSendDirectMessageAfterFollowChecked)
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, DominatorAccountModel.UserName, ActivityType, "Started \"After Follow Action\" functionality");


                #region Like Post process after Follow

                if (FollowerModel.IsChkLikeUsersLatestPost)
                    LikeUsersLatestPosts(instagramUser, scrapeResult);

                #endregion

                #region Comment on post after Follow process

                if (FollowerModel.ChkCommentOnUserLatestPostsChecked)
                    CommentOnUsersLatestPosts(instagramUser, scrapeResult, dominatorAccountModel);
                #endregion

                #region Send Message after Follow process

                if (FollowerModel.ChkSendDirectMessageAfterFollowChecked)
                    SendMessageToUser(instagramUser, scrapeResult, accountModel);
                #endregion

                #region Mute Post after Follow Process
                if (FollowerModel.ChkMuteFollowerAfterFollowChecked)
                    MuteFollowerPost(instagramUser, dominatorAccountModel, accountModel);

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AddFollowedDataToDataBase(ScrapeResultNew scrapeResult, FriendshipsResponse response, string status)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            try
            {
                var instaUser = (InstagramUser)scrapeResult.ResultUser;

                // Add data to respected campaign InteractedUsers table
                if (!string.IsNullOrEmpty(CampaignId))
                {
                    // CampaignDbOperation

                    // Thread.Sleep(15000);
                    CampaignDbOperation?.Add(
                            new DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedUsers()
                            {
                                ActivityType = ActivityType.ToString(),
                                Date = DateTimeUtilities.GetEpochTime(),
                                QueryType = scrapeResult.QueryInfo.QueryType,
                                Query = scrapeResult.QueryInfo.QueryValue,
                                Username = DominatorAccountModel.AccountBaseModel.UserName,
                                InteractedUsername = scrapeResult.ResultUser.Username,
                                InteractedUserId = ((InstagramUser)scrapeResult.ResultUser).Pk,
                                FollowedBack = response.FollowedBack ? 1 : 0,
                                IsPrivate = response.IsPrivate,
                                Time = DateTimeUtilities.GetEpochTime(),
                                Status = status
                            });
                }

                // Add data to respected Account InteractedUsers table;
                AccountDbOperation.Add(new InteractedUsers()
                {
                    ActivityType = ActivityType.ToString(),
                    Date = DateTimeUtilities.GetEpochTime(),
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    Query = scrapeResult.QueryInfo.QueryValue,
                    Username = DominatorAccountModel.AccountBaseModel.UserName,
                    InteractedUsername = scrapeResult.ResultUser.Username,
                    InteractedUserId = ((InstagramUser)scrapeResult.ResultUser).Pk,
                    FollowedBack = response.FollowedBack ? 1 : 0,
                    IsPrivate = response.IsPrivate,
                    Time = DateTimeUtilities.GetEpochTime(),
                    Status = status

                });

                if (status.Equals("Requested"))
                {
                    AccountDbOperation.Add(new Friendships()
                    {
                        Username = instaUser.Username,
                        IsPrivate = instaUser.IsPrivate,
                        IsVerified = instaUser.IsVerified,
                        UserId = instaUser.Pk,
                        FullName = instaUser.FullName,
                        HasAnonymousProfilePicture = (instaUser.HasAnonymousProfilePicture == true),
                        ProfilePicUrl = instaUser.ProfilePicUrl,
                        FollowType = FollowType.Requested,
                        IsFollowBySoftware = true,
                        Time = DateTimeUtilities.GetEpochTime()
                    });
                }
                else
                {
                    AccountDbOperation.Add(new Friendships()
                    {
                        Username = instaUser.Username,
                        IsPrivate = instaUser.IsPrivate,
                        IsVerified = instaUser.IsVerified,
                        UserId = instaUser.Pk,
                        FullName = instaUser.FullName,
                        HasAnonymousProfilePicture = (instaUser.HasAnonymousProfilePicture == true),
                        ProfilePicUrl = instaUser.ProfilePicUrl,
                        Followings = 1,
                        FollowType = FollowType.Following,
                        IsFollowBySoftware = true,
                        Time = DateTimeUtilities.GetEpochTime()
                    });

                }
                // Add data to respected account friendship table

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
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
