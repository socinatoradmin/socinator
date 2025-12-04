using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.DatabaseHandler.TumblrTables.Account;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TumblrFactory;
using TumblrDominatorCore.TumblrLibrary.DAL;
using TumblrDominatorCore.TumblrLibrary.TumblrFunction;
using TumblrDominatorCore.TumblrRequest;
using TumblrDominatorCore.TumblrResponseHandler;
using InteractedPosts = DominatorHouseCore.DatabaseHandler.TumblrTables.Campaign.InteractedPosts;

namespace TumblrDominatorCore.TumblrLibrary.TumblrProcesses
{
    public class FollowJobProcess : TumblrJobProcessInteracted<InteractedUser>
    {
        private readonly ITumblrHttpHelper HttpHelper;
        private readonly ITumblrFunct TumblrFunct;

        private bool _hasNoMorePosts;

        public FollowJobProcess(IProcessScopeModel processScopeModel, IDbAccountServiceScoped _accountService,
            IDbGlobalService _dbGlobalService, IExecutionLimitsManager executionLimitsManager,
            ITumblrFunct _tumblrFunct, ITumblrQueryScraperFactory queryScraperFactory, ITumblrHttpHelper _httpHelper,
            ITumblrLoginProcess _tumblrLoginProcess) :
            base(processScopeModel, _accountService, _dbGlobalService, executionLimitsManager, queryScraperFactory,
                _httpHelper, _tumblrLoginProcess)
        {
            TumblrFunct = _tumblrFunct;
            FollowerModel = processScopeModel.GetActivitySettingsAs<FollowerModel>();
            HttpHelper = _httpHelper;
        }

        // Per day Like count for individual campaigns
        private int UserCountToLike { get; set; }

        // Per day Comment count for individual campaigns
        private int UserCountToComment { get; set; }


        public FollowerModel FollowerModel { get; set; }

        /// <summary>
        ///     FOLLOW POST: to Follow user in Tumblr
        /// </summary>
        /// <param name="scrapeResultNew"></param>
        /// <returns></returns>
        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResultNew)
        {
            FollowResponseHandler response = null;
            var tumblrUser = (TumblrUser)scrapeResultNew.ResultUser;
            var scrapeResult = (TumblrScrapeResult)scrapeResultNew;
            var jobProcessResult = new JobProcessResult();

            #region follow Unique Users

            if (FollowerModel.ChkFollowUniqueUsersInCampaign)
            {
                try
                {
                    var campaignInteractionDetails =
                        InstanceProvider.GetInstance<ICampaignInteractionDetails>();
                    campaignInteractionDetails.AddInteractedData(SocialNetworks.Tumblr, CampaignId,
                        tumblrUser.Username);
                }
                catch (Exception)
                {
                    return jobProcessResult;
                }

                #endregion
            }

            var isFollowed = false;
            if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                response = TumblrFunct.Follow(DominatorAccountModel, tumblrUser, "");
            else
                response = _browserManager.Follow(DominatorAccountModel, tumblrUser);
            if (response != null && response.Success)
            {
                GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType.ToString(),
                    "=> " + scrapeResult.ResultUser.Username);
                IncrementCounters();
                scrapeResult.ResultUser = tumblrUser;
                AddFollowedDataToDataBase(scrapeResult);
                var AccountModel = new AccountModel(DominatorAccountModel);

                if (AccountModel.LstFollowings == null)
                    AccountModel.LstFollowings = new List<TumblrUser>();
                AccountModel.LstFollowings.Add(tumblrUser);

                PostFollowProcess(DominatorAccountModel, tumblrUser, scrapeResult.TumblrFormKey);
                StartOtherConfiguration_now(scrapeResult);
                jobProcessResult.IsProcessSuceessfull = true;
            }
            else
            {
                if (FollowerModel.ChkFollowUniqueUsersInCampaign)
                    try
                    {
                        var campaignInteractionDetails =
                            InstanceProvider.GetInstance<ICampaignInteractionDetails>();

                        campaignInteractionDetails.RemoveIfExist(SocialNetworks.Tumblr, CampaignId,
                            tumblrUser.Username);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                if (response != null && !response.Success)
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType.ToString(),
                        "=> " + scrapeResult.ResultUser.Username);
                    jobProcessResult.IsProcessSuceessfull = false;
                }
                else
                {
                    jobProcessResult.IsProcessSuceessfull = true;
                }
            }

            if (response != null && response.Success || isFollowed)
                DelayBeforeNextActivity();
            return jobProcessResult;
        }

        /// <summary>
        ///     Add Followed Data data to DB
        /// </summary>
        /// <param name="scrapeResult"></param>
        private void AddFollowedDataToDataBase(ScrapeResultNew scrapeResult)
        {
            try
            {
                var user = (TumblrUser)scrapeResult.ResultUser;

                #region  InteractedUser

                if (!string.IsNullOrEmpty(CampaignId))
                {
                    // Add data to respected campaign InteractedUsers table
                    IDbCampaignService CampaignService = new DbCampaignService(CampaignId);
                    CampaignService.Add(new DominatorHouseCore.DatabaseHandler.TumblrTables.Campaign.InteractedUser
                    {
                        AccountEmail = DominatorAccountModel.AccountBaseModel.UserName,
                        ActivityType = ActivityType.ToString(),
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        QueryType = scrapeResult.QueryInfo?.QueryType,
                        QueryValue = scrapeResult.QueryInfo?.QueryValue,
                        UserName = DominatorAccountModel.AccountBaseModel.UserId,
                        UserProfileUrl = user.ProfilePicUrl,
                        PageUrl = user.PageUrl,
                        InteractedUsername = scrapeResult.ResultUser.Username,
                        TemplateId = TemplateId
                    });
                }

                // Add data to respected account friendship table
                DbAccountService.Add(new InteractedUser
                {
                    AccountEmail = DominatorAccountModel.AccountBaseModel.UserName,
                    ActivityType = ActivityType.Follow.ToString(),
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    QueryType = scrapeResult.QueryInfo?.QueryType,
                    QueryValue = scrapeResult.QueryInfo?.QueryValue,
                    UserProfileUrl = user.ProfilePicUrl,
                    PageUrl = user.PageUrl,
                    UserName = DominatorAccountModel.AccountBaseModel.UserName,
                    InteractedUsername = scrapeResult.ResultUser.Username
                });

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        /// <summary>
        ///     Add Liked data to DB
        /// </summary>
        /// <param name="scrapeResult"></param>
        /// <param name="activityType"></param>
        private void AddLikedDataToDataBase(ScrapeResultNew scrapeResult, ActivityType activityType)
        {
            try
            {
                #region InteractedPosts

                var tumblrpost = (TumblrPost)scrapeResult.ResultPost;
                IDbCampaignService _campaignService = new DbCampaignService(CampaignId);
                _campaignService.Add(new InteractedPosts
                {
                    AccountEmail = tumblrpost.OwnerUsername,
                    ActivityType = activityType.ToString(),
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    ContentId = tumblrpost.Id,
                    PostTitle = tumblrpost.Caption
                });

                DbAccountService.Add(new DominatorHouseCore.DatabaseHandler.TumblrTables.Account.InteractedPosts
                {
                    ActivityType = activityType.ToString(),
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    ContentId = tumblrpost.Id,
                    PostTitle = tumblrpost.Caption
                });

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void StartUnFollow()
        {
            try
            {
                var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                var jobActConfigManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var UnfollowmoduleConfiguration = jobActConfigManager[DominatorAccountModel?.AccountId, ActivityType.Unfollow];
                if (UnfollowmoduleConfiguration == null)
                {
                    dominatorScheduler.ChangeAccountsRunningStatus(false, DominatorAccountModel.AccountId, ActivityType.Follow);
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                     DominatorAccountModel.UserName, ActivityType,
                         "Unfollow configuration is not saved Please save manually to restart Follow/Unfollow activity from this account");
                    GlobusLogHelper.log.Info(Log.ProcessCompleted,
                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType);
                    JobProcessResult.IsProcessCompleted = true;
                    return;
                }
                dominatorScheduler.EnableDisableModules(ActivityType.Follow, ActivityType.Unfollow,
                    DominatorAccountModel.AccountId);
            }
            catch (InvalidOperationException ex)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType,
                    ex.Message.Contains("1001")
                        ? "Unfollow configuration is not saved Please save manually to restart Follow/Unfollow activity from this account"
                        : "");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void StartOtherConfiguration_now(ScrapeResultNew scrapeResult)
        {
            try
            {
                if (FollowerModel.IsChkEnableAutoFollowUnfollowChecked)
                {
                    #region Process for auto Follow and Unfollow
                    var tumblrFunction = new TumblrFunct(HttpHelper);
                    var CurrentUserResponse = tumblrFunction.GetUserInfo(DominatorAccountModel);
                    var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                    var random1 = FollowerModel.StopFollowToolWhenReach.GetRandom();
                    var random2 = FollowerModel.FollowerFollowingsMaxValue;
                    if (FollowerModel.IsChkStopFollow)
                    {
                        if ((FollowerModel.IsChkStopFollowToolWhenReachChecked && CurrentUserResponse.TumblrUser.FollowingCount >= random1) || (FollowerModel.IsChkWhenFollowerFollowingsIsSmallerThanChecked && (CurrentUserResponse.TumblrUser.FollowersCount < random2 || CurrentUserResponse.TumblrUser.FollowingCount < random2)))
                        {
                            dominatorScheduler.ChangeAccountsRunningStatus(false, DominatorAccountModel.AccountId, ActivityType.Follow);
                            JobProcessResult.IsProcessCompleted = true;
                            return;
                        }
                        #region OLD Code.
                        //if (FollowerModel.IsChkWhenFollowerFollowingsIsSmallerThanChecked)
                        //{
                        //    var randomMax = FollowerModel.FollowerFollowingsMaxValue;

                        //    var responses = tumblrFunction.GetUserInfo(DominatorAccountModel.AccountBaseModel.UserName);
                        //    FollowingcountafterFollow = responses.TumblrUser.FollowingCount;
                        //    FollowerCount = responses.TumblrUser.FollowersCount;
                        //    if (FollowingcountafterFollow >= randomMax || FollowerCount >= randomMax)
                        //        JobProcessResult.IsProcessCompleted = true;
                        //    StopFollow();
                        //    return;
                        //}
                        #endregion
                    }

                    if (FollowerModel.IsChkStartUnFollow)
                    {
                        try
                        {
                            if (FollowerModel.IsChkStopFollowToolWhenReachChecked && CurrentUserResponse.TumblrUser.FollowingCount >= random1)
                            {
                                StartUnFollow();
                            }
                        }
                        catch (InvalidOperationException ex)
                        {
                            ex.DebugLog();
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                        #endregion
                        // Process for Unfollow previously followed user
                        //if (FollowerModel.IsChkUnfollowUsersChecked && FollowerModel.UnfollowPrevious.GetRandom() > 0)
                        //    UnfollowPreviouslyfollowed(TumblrFunct, scrapeResult);
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }


        #region Process for Unfollow previously followed user

        public void UnfollowPreviouslyfollowed(ITumblrFunct tumblrFunction, ScrapeResultNew scrapeResult)
        {
            var scrapeResultss = (TumblrScrapeResult)scrapeResult;
            var lstTotalFollowedusers =
                CampaignService.GetSelectedUsers(DominatorAccountModel.AccountBaseModel
                    .UserName); //.Get<DominatorHouseCore.DatabaseHandler.TumblrTables.Campaign.InteractedUser>(x => x.AccountEmail == DominatorAccountModel.AccountBaseModel.UserName);

            var maxFollowTime = DateTime.Now.AddHours(FollowerModel.UnfollowPrevious.GetRandom());

            var maxFollowEpochTime = (int)(maxFollowTime - DateTime.Now).TotalSeconds;

            lstTotalFollowedusers.Where(x => x.InteractionTimeStamp < maxFollowEpochTime).ToList().ForEach(x =>
            {
                try
                {

                    var searchUsersForFollowingResponse = TumblrFunct.GetAcccountFollowers(DominatorAccountModel);

                    var lstFollowerUsernames = searchUsersForFollowingResponse.LstTumblrUser
                        .Select(eachFollower => eachFollower.Username).ToList();

                    if (FollowerModel.IsChkUnfollowfollowedbackChecked)
                        if (lstFollowerUsernames.Contains(x.InteractedUsername))
                        {
                            var tumblrUser = new TumblrUser { Username = x.InteractedUsername };
                            if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                                tumblrFunction.UnFollow(DominatorAccountModel, tumblrUser,
                                    scrapeResultss.TumblrFormKey);
                            else
                                _browserManager.UnFollow(DominatorAccountModel, tumblrUser);
                        }

                    if (FollowerModel.IsChkUnfollownotfollowedbackChecked)
                        if (!lstFollowerUsernames.Contains(x.InteractedUsername))
                        {
                            var tumblrUser = new TumblrUser { Username = x.InteractedUsername };
                            if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                                tumblrFunction.UnFollow(DominatorAccountModel, tumblrUser,
                                    scrapeResultss.TumblrFormKey);
                            else
                                _browserManager.UnFollow(DominatorAccountModel, tumblrUser);
                        }
                }
                catch (Exception e)
                {
                    e.DebugLog();
                }
            });
        }

        #endregion


        /// <summary>
        ///     After Follow Actions
        /// </summary>
        /// <param name="tumblrUser"></param>
        /// <param name="tumblrKey"></param>
        public void PostFollowProcess(DominatorAccountModel tumbleraccount, TumblrUser tumblruser, string tumblrKey)
        {
            try
            {
                if (FollowerModel.IsChkLikeUsersLatestPost)
                {
                    Thread.Sleep(6000);
                    LikeUserPosts(tumbleraccount, tumblruser, tumblrKey);
                }
                if (FollowerModel.ChkSendDirectMessageAfterFollowChecked)
                {
                    var sendMessageresp = new MessageUserResponse();
                    var status = false;
                    tumblruser.Message = FollowerModel.Message;
                    if (!tumbleraccount.IsRunProcessThroughBrowser)
                    {

                        if (string.IsNullOrEmpty(tumblruser.Uuid))
                        {
                            var userDetails = TumblrFunct.GetUserDetails(DominatorAccountModel, tumblruser);
                            tumblruser.Uuid = userDetails.tumblrUser.Uuid;
                        }
                        sendMessageresp = TumblrFunct.Messageuser(tumbleraccount, tumblruser, tumblrKey);
                    }
                    else
                        status = _browserManager.BroadCastMessage(tumbleraccount, FollowerModel.Message, "", ref tumblruser);
                    if (sendMessageresp.Success || status)
                    {
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType.BroadcastMessages.ToString(),
                        " To ==> " + tumblruser.Username);
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType.BroadcastMessages.ToString(),
                        " To ==>  " + tumblruser.Username);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        /// <summary>
        ///     Like Users Posts after follow
        /// </summary>
        /// <param name="tumblrUser"></param>
        /// <param name="tumblrFormKey"></param>
        private void LikeUserPosts(DominatorAccountModel tumbleraccount, TumblrUser tumblrUser, string tumblrFormKey)
        {
            var pinLikeCount = FollowerModel.LikeBetweenJobs.GetRandom();
            if (FollowerModel.IsChkMaxLike)
            {
                var maxmimumPinlike = FollowerModel.LikeMaxBetween.GetRandom();
                pinLikeCount = maxmimumPinlike;
            }
            var countPinLiked = 0;
            var currentPage = 0;
            var tumblrFunction = new TumblrFunct(HttpHelper);
            while (countPinLiked <= pinLikeCount)
            {
                try
                {
                    //if (IsPostFollowProcessCompleted(ActivityType.Like))
                    //    return;
                    var postList = new List<TumblrPost>();
                    if (!tumbleraccount.IsRunProcessThroughBrowser)
                        postList = GetUsersPosts(tumbleraccount, tumblrUser, tumblrFunction, currentPage);
                    else
                        postList = _browserManager.GetUserPostsDetails(tumbleraccount, tumblrUser);
                    if (postList.Count == 0 && _hasNoMorePosts) break;
                    postList.RemoveAll(x => x.IsLiked);
                    if (postList.Count == 0)
                    {
                        _hasNoMorePosts = true;
                        break;
                    }
                    #region settings
                    if (FollowerModel.ChkLikeRandomPostsChecked) postList.Shuffle();
                    #endregion
                    foreach (var item in postList)
                        try
                        {
                            TumblrPost post = item;
                            LikePostResponse response = null;
                            var isLiked = false;
                            if (!tumbleraccount.IsRunProcessThroughBrowser)
                                response = tumblrFunction.LikePost(DominatorAccountModel, post, tumblrFormKey);
                            else
                                response = _browserManager.LikePost(tumbleraccount, post);
                            if (response != null && response.Success || isLiked)
                            {
                                countPinLiked++;
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType.ToString(),
                                    "Liked Post of " + post.OwnerUsername + " => " + post.PostUrl);
                                var scrapeResultNew = new ScrapeResultNew();
                                item.OwnerUsername = DominatorAccountModel.AccountBaseModel.UserName;
                                scrapeResultNew.ResultPost = post;

                                AddLikedDataToDataBase(scrapeResultNew, ActivityType.Like);
                            }
                            else
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType.ToString(),
                                        "Failed to Like Post of " + post.OwnerUsername + " => " + post.PostUrl);

                            if (countPinLiked >= pinLikeCount) return;
                            DelayBeforeNextActivity(15);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                    currentPage++;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }

        /// <summary>
        ///     Scrape posts from given User
        /// </summary>
        /// <param name="tumblrUser"></param>
        /// <param name="tumblrFunction"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        public List<TumblrPost> GetUsersPosts(DominatorAccountModel tumbleraccount, TumblrUser tumblrUser,
            TumblrFunct tumblrFunction, int currentPage)
        {
            SearchPostForUserRespones searchPostForUserRespones = null;
            var container = new CookieContainer();

            foreach (Cookie cookie in tumbleraccount.Cookies)
            {
                try
                {
                    if (cookie.Name == "rum" || cookie.Name == "tps")
                        continue;
                    container.Add(new Cookie { Name = cookie.Name, Value = cookie.Value, Domain = cookie.Domain });
                }
                catch (Exception)
                {
                    continue;
                }

            }

            //container.Add(tumbleraccount.Cookies);
            searchPostForUserRespones = tumblrFunction.SearchPostFromUser(tumblrUser.Username,
                tumblrUser.TumblrsFormKey, currentPage, container);
            if (!searchPostForUserRespones.LstTumblrPost.Any() && searchPostForUserRespones.Success)
            {
                _hasNoMorePosts = true;
                return searchPostForUserRespones.LstTumblrPost;
            }

            return searchPostForUserRespones.LstTumblrPost;
        }

        /// <summary>
        ///     Check Post follow process Compliation
        /// </summary>
        /// <param name="activityType"></param>
        /// <returns></returns>
        private bool IsPostFollowProcessCompleted(ActivityType activityType)
        {
            if (activityType == ActivityType.Like)
            {
                var lstInteractedPosts = DbAccountService.GetInteractedPosts(activityType);
                if (FollowerModel.IsChkMaxLike && UserCountToLike <= lstInteractedPosts.Count)
                    return true;
            }
            else if (activityType == ActivityType.BroadcastMessages)
            {
                var lstInteractedPosts = DbAccountService.GetInteractedUsers(activityType);
                if (FollowerModel.IsChkMaxComment && UserCountToComment <= lstInteractedPosts.Count)
                    return true;
            }
            else
            {
                return true;
            }

            return false;
        }


    }
}