using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using GramDominatorCore.Factories;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDModel;
using GramDominatorCore.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using static GramDominatorCore.GDModel.UserScraperModel;
using GramDominatorCore.Utility;

namespace GramDominatorCore.GDLibrary
{
    public class UserScrapeProcess : GdJobProcessInteracted<InteractedUsers>
    {
        public UserScraperModel UserScraperModel { get; set; }
        private static readonly object UniqueUserAccrossAllAccount = new object();
        private static readonly object UniqueUserCampaignWise = new object();
        public UserScrapeProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IGdQueryScraperFactory queryScraperFactory, IGdHttpHelper httpHelper, IGdBrowserManager gdBrowser, IDelayService _delayService) :
            base(processScopeModel, accountServiceScoped, queryScraperFactory, httpHelper, gdBrowser, _delayService)
        {
            UserScraperModel = JsonConvert.DeserializeObject<UserScraperModel>(templateModel.ActivitySettings);
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            JobProcessResult jobProcessResult = new JobProcessResult();
            try
            {
                InstagramUser instagramUser = (InstagramUser)scrapeResult.ResultUser;

                if (instagramUser != null)
                {
                    if (ModuleSetting.IsScrpeUniqueUserForThisCampaign)
                    {
                        lock (UniqueUserCampaignWise)
                        {
                            try
                            {
                                var instance = InstanceProvider.GetInstance<ICampaignInteractionDetails>();
                                instance.AddInteractedData(SocialNetworks, $"{CampaignId}.UserScraper", instagramUser.Pk ?? instagramUser.UserId);
                                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            }
                            catch (Exception)
                            {
                                //ex.DebugLog();
                                delayservice.ThreadSleep(TimeSpan.FromSeconds(1));
                                return jobProcessResult;

                            }
                        }
                    }
                    GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultUser.Username);
                    string requiredData = GetRequiredData(instagramUser);
                    #region Unique User Added througout all accounts
                    lock (UniqueUserAccrossAllAccount)
                    {
                        var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                        var instaConfig = genericFileManager.GetModel<InstagramUserModel>(ConstantVariable.GetOtherInstagramSettingsFile());
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        try
                        {
                            if (instaConfig != null && instaConfig.IsEnableScrapeDiffrentUserChecked && !ModuleSetting.IsScrpeUniqueUserForThisCampaign)
                                GlobalInteractionDetails.AddInteractedData(SocialNetworks.Instagram, ActivityType, instagramUser.Username);
                            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        }
                        catch (Exception)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType,
                                $"This {instagramUser.Username} is already Scraped");
                            if (UserScraperModel.IsChkRequiredData)
                                DelayBeforeNextActivity();

                            delayservice.ThreadSleep(TimeSpan.FromSeconds(1));
                            return jobProcessResult;
                        }
                    }
                    #endregion

                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                       DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultUser.Username);

                    AddScrapedUserDataIntoDataBase(scrapeResult, requiredData);
                    IncrementCounters();
                    jobProcessResult.IsProcessSuceessfull = true;
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (scrapeResult.QueryInfo.QueryType.Equals("Custom Users List"))
                    {
                        jobProcessResult.IsProcessCompleted = true;
                        jobProcessResult.HasNoResult = true;
                    } 
                }
                else
                {
                    // Here issue will only arise when "InstagramUser" class object will be null,
                    // and that should not be happen.
                    jobProcessResult.IsProcessSuceessfull = false;
                }
                
                DelayBeforeNextActivity();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return jobProcessResult;
        }

        private void AddScrapedUserDataIntoDataBase(ScrapeResultNew scrapeResult, string reqData)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            InstagramUser userDetails = (InstagramUser)scrapeResult.ResultUser;
            // Add data to respected campaign InteractedUsers table
            try
            {
                if (!string.IsNullOrEmpty(CampaignId))
                {
                    CampaignDbOperation.Add(new DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedUsers()
                    {
                        ActivityType = ActivityType.UserScraper.ToString(),
                        Date = DateTimeUtilities.GetEpochTime(),
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        Query = scrapeResult.QueryInfo.QueryValue,
                        Username = DominatorAccountModel.AccountBaseModel.UserName,
                        InteractedUsername = scrapeResult.ResultUser.Username,
                        InteractedUserId = ((InstagramUser)scrapeResult.ResultUser).Pk != null ? ((InstagramUser)scrapeResult.ResultUser).Pk : ((InstagramUser)scrapeResult.ResultUser).UserId,
                        IsPrivate = userDetails.IsPrivate,
                        IsBusiness = userDetails.IsBusiness,
                        IsVerified = userDetails.IsVerified,
                        IsProfilePicAvailable = !userDetails.HasAnonymousProfilePicture,
                        ProfilePicUrl = userDetails.ProfilePicUrl,
                        TaggedUser = ModuleSetting.IsTaggedPostUser ? userDetails.Username : string.Empty,
                        Gender = userDetails.Gender.ToString(),
                        RequiredData = reqData
                    });
                }

                // Add data to respected Account InteractedUsers table
                AccountDbOperation.Add(
                    new InteractedUsers()
                    {
                        ActivityType = ActivityType.UserScraper.ToString(),
                        Date = DateTimeUtilities.GetEpochTime(),
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        Query = scrapeResult.QueryInfo.QueryValue,
                        Username = DominatorAccountModel.AccountBaseModel.UserName,
                        InteractedUsername = scrapeResult.ResultUser.Username,
                        InteractedUserId = ((InstagramUser)scrapeResult.ResultUser).Pk != null ? ((InstagramUser)scrapeResult.ResultUser).Pk : ((InstagramUser)scrapeResult.ResultUser).UserId,
                        IsPrivate = userDetails.IsPrivate,
                        IsBusiness = userDetails.IsBusiness,
                        IsVerified = userDetails.IsVerified,
                        IsProfilePicAvailable = !userDetails.HasAnonymousProfilePicture,
                        ProfilePicUrl = userDetails.ProfilePicUrl,
                        TaggedUser = ModuleSetting.IsTaggedPostUser ? userDetails.Username : string.Empty,
                        Gender = userDetails.Gender.ToString(),
                        RequiredData = reqData
                    });
            }
            catch (Exception)
            {
                //ignored
            }
        }

        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {

        }
        private string GetRequiredData(InstagramUser objInstagram)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            UserRequiredDatas objUserRequiredData = new UserRequiredDatas();
            string userReqData = string.Empty;
            int commentCount = 0;
            int likeCount = 0;

            if (UserScraperModel.IsChkRequiredData)
            {
                var reqData = instaFunct.SearchUsername(DominatorAccountModel, objInstagram.Username, JobCancellationTokenSource.Token);
                //if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                //    reqData = instaFunct.SearchUserInfoById(DominatorAccountModel, AccountModel, objInstagram.Pk, JobCancellationTokenSource.Token);
                //else
                //    reqData = instaFunct.GdBrowserManager.GetUserInfo(DominatorAccountModel, objInstagram.Username, JobCancellationTokenSource.Token);
                ObservableCollection<UserRequiredData> lstReqData = UserScraperModel.ListUserRequiredData;
                if (UserScraperModel.IsPostCommentAndLikeCount)
                {
                    GetPostCommentAndLikecount(objInstagram, reqData, ref commentCount, ref likeCount);
                }
                foreach (UserRequiredData dataList in lstReqData)
                {
                    if (dataList.ItemName.Contains("Profile Picture Url") && dataList.IsSelected)
                        objUserRequiredData.ProfilePictureUrl = reqData.ProfilePicUrl;
                    else if (dataList.ItemName.Contains("User Name") && dataList.IsSelected)
                        objUserRequiredData.UserName = reqData.Username;
                    else if (dataList.ItemName.Contains("User ID") && dataList.IsSelected)
                        objUserRequiredData.UserId = reqData.Pk;
                    else if (dataList.ItemName.Contains("User Full Name") && dataList.IsSelected)
                        if (!string.IsNullOrEmpty(reqData.FullName))
                            objUserRequiredData.UserFullName = reqData.FullName;
                        else
                            objUserRequiredData.UserFullName = "";

                    else if (dataList.ItemName.Contains("Is Followed Already") && dataList.IsSelected)
                        objUserRequiredData.IsFollowedAlready = reqData.IsVerified;
                    else if (dataList.ItemName.Contains("Post Count") && dataList.IsSelected)
                        objUserRequiredData.PostCount = reqData.MediaCount;
                    else if (dataList.ItemName.Contains("Follower Count") && dataList.IsSelected)
                        objUserRequiredData.FollowerCount = reqData.FollowerCount;
                    else if (dataList.ItemName.Contains("Following Count") && dataList.IsSelected)
                        objUserRequiredData.FollowingCount = reqData.FollowingCount;
                    else if (dataList.ItemName.Contains("Email Id") && dataList.IsSelected)
                        if (!string.IsNullOrEmpty(reqData?.instaUserDetails?.Email)&&reqData?.instaUserDetails?.Email !="N/A")
                            objUserRequiredData.Email_Id = reqData?.instaUserDetails?.Email;
                        else
                            objUserRequiredData.Email_Id = "N/A";
                    else if (dataList.ItemName.Contains("Contact Number") && dataList.IsSelected)
                        if (!string.IsNullOrEmpty(reqData?.instaUserDetails?.PhoneNumber)&& reqData?.instaUserDetails?.PhoneNumber != "N/A")
                            objUserRequiredData.Phone_Number = "+" + (string.IsNullOrEmpty(reqData.PublicPhoneCountryCode)? "91": reqData.PublicPhoneCountryCode) + " " + reqData?.instaUserDetails?.PhoneNumber;
                        else
                            objUserRequiredData.Phone_Number = "N/A";
                    else if (dataList.ItemName.Contains("Engagement Rate") && dataList.IsSelected)
                    {
                        string engageMentRate = GetEnageMentRate(objInstagram, reqData);
                        objUserRequiredData.EngagementRate = engageMentRate;
                    }
                    else if (dataList.ItemName.Contains("Comment count") && dataList.IsSelected)
                    {
                        string postcommentCount = Convert.ToString(commentCount);
                        objUserRequiredData.CommentCount = postcommentCount;
                    }
                    else if (dataList.ItemName.Contains("Like count") && dataList.IsSelected)
                    {
                        string postlikeCount = Convert.ToString(likeCount);
                        objUserRequiredData.LikeCount = postlikeCount;
                    }
                    else if (dataList.ItemName.Contains("Biography") && dataList.IsSelected)
                    {
                        objUserRequiredData.Biography = reqData.Biography;
                    }
                    else if (dataList.ItemName.Contains("Business Acocunts") && dataList.IsSelected)
                    {
                        objUserRequiredData.IsBusiness = reqData.IsBusiness;
                    }
                    else if (dataList.ItemName.Contains("Business Category") && dataList.IsSelected)
                        if (!string.IsNullOrEmpty(reqData.BusinessCategory))
                            objUserRequiredData.BusinessCategory = reqData.BusinessCategory;
                        else
                            objUserRequiredData.BusinessCategory = "";
                }
                userReqData = JsonConvert.SerializeObject(objUserRequiredData);
            }
            return userReqData;
        }

        public string GetEnageMentRate(InstagramUser instagramUser, UsernameInfoIgResponseHandler userinfo)
        {
            string engagementAvg = string.Empty;
            string responseMaxId = string.Empty;
            List<InstagramPost> totalPostData = new List<InstagramPost>();
            try
            {
                if (UserScraperModel.IsEnagementRate)
                {
                    int noOfEngagementPost = UserScraperModel.MaxPost;
                    do
                    {
                        var browser = GramStatic.IsBrowser;
                        var userFeedDetails = 
                            browser ?
                            instaFunct.GdBrowserManager.GetUserFeed(DominatorAccountModel, instagramUser.Username, JobCancellationTokenSource.Token)
                            : instaFunct.GetUserFeed(DominatorAccountModel, AccountModel, instagramUser.Username, JobCancellationTokenSource.Token, responseMaxId);

                        if (userFeedDetails == null || !userFeedDetails.Success)
                        {
                            delayservice.ThreadSleep(TimeSpan.FromSeconds(5));//Thread.Sleep(5 * 1000);
                            userFeedDetails = instaFunct.GetUserFeed(DominatorAccountModel, AccountModel, instagramUser.Username, JobCancellationTokenSource.Token, responseMaxId);
                        }
                        if (userFeedDetails.Items.Count == 0)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType,
                                $"{userinfo.Username} engagement count will be 0 because no post available in users profile");
                            break;
                        }
                        if (userinfo.MediaCount < noOfEngagementPost && userinfo.MediaCount != 1)
                            noOfEngagementPost = userinfo.MediaCount - 1;
                        if (userinfo.MediaCount == 1)
                            noOfEngagementPost = userinfo.MediaCount;


                        if (userFeedDetails.Items.Count < noOfEngagementPost)
                            totalPostData.AddRange(userFeedDetails.Items);
                        else
                        {
                            totalPostData.AddRange(userFeedDetails.Items);
                            break;
                        }
                        if (totalPostData.Count >= noOfEngagementPost)
                            break;

                        responseMaxId = userFeedDetails.MaxId;
                        if (!string.IsNullOrEmpty(responseMaxId))
                            delayservice.ThreadSleep(TimeSpan.FromSeconds(2));//Thread.Sleep(TimeSpan.FromSeconds(2));
                    } while (!string.IsNullOrEmpty(responseMaxId));
                    int totalNotEngagementCount = 0;

                    for (int post = 0; post < noOfEngagementPost; post++)
                    {
                        int noOfLike = 0;
                        int noOfComment = 0;
                        int totalNoCommentsPerPost = 0;
                        noOfLike = totalPostData[post].LikeCount;
                        noOfComment = totalPostData[post].CommentCount;
                        totalNoCommentsPerPost = noOfLike + noOfComment;
                        totalNotEngagementCount += totalNoCommentsPerPost;
                    }

                    // ReSharper disable once PossibleLossOfFraction
                    float count1 = totalNotEngagementCount / noOfEngagementPost;
                    float count2 = count1 / userinfo.FollowerCount;
                    float count3 = count2 * 100;
                    engagementAvg = count3.ToString(CultureInfo.InvariantCulture);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return engagementAvg;
        }

        public void GetPostCommentAndLikecount(InstagramUser instagramUser, UsernameInfoIgResponseHandler userinfo, ref int totalNoOfComments, ref int totalNoOfLike)
        {

            string responseMaxId = string.Empty;
            List<InstagramPost> totalPostData = new List<InstagramPost>();
            try
            {
                if (UserScraperModel.IsPostCommentAndLikeCount)
                {
                    int noOfPost = UserScraperModel.maxPostForCommentAndLIkeCount;
                    var browser = GramStatic.IsBrowser;
                    do
                    {
                        var userFeedDetails = 
                            browser ?
                            instaFunct.GdBrowserManager.GetUserFeed(DominatorAccountModel, instagramUser.Username, JobCancellationTokenSource.Token)
                            : instaFunct.GetUserFeed(DominatorAccountModel, AccountModel, instagramUser.Username, JobCancellationTokenSource.Token, responseMaxId);
                        if (userFeedDetails == null || !userFeedDetails.Success)
                        {
                            delayservice.ThreadSleep(TimeSpan.FromSeconds(5));//Thread.Sleep(5 * 1000);
                            userFeedDetails = instaFunct.GetUserFeed(DominatorAccountModel, AccountModel, instagramUser.Username, JobCancellationTokenSource.Token, responseMaxId);
                        }
                        if (userFeedDetails.Items.Count == 0)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType,
                                $"{userinfo.Username} post is not available in users profile");
                            break;
                        }
                        if (userinfo.MediaCount < noOfPost && userinfo.MediaCount != 1)
                            noOfPost = userinfo.MediaCount - 1;
                        if (userinfo.MediaCount == 1)
                        {
                            noOfPost = userinfo.MediaCount;
                        }

                        if (userFeedDetails.Items.Count < noOfPost)
                            totalPostData.AddRange(userFeedDetails.Items);
                        else
                        {
                            totalPostData.AddRange(userFeedDetails.Items);
                            break;
                        }
                        if (totalPostData.Count >= noOfPost)
                        {
                            break;
                        }
                        responseMaxId = userFeedDetails.MaxId;
                        if (!string.IsNullOrEmpty(responseMaxId))
                            delayservice.ThreadSleep(TimeSpan.FromSeconds(2));//Thread.Sleep(TimeSpan.FromSeconds(2));
                    } while (!string.IsNullOrEmpty(responseMaxId));

                    for (int post = 0; post < noOfPost; post++)
                    {
                        var noOfLike = totalPostData[post].LikeCount;
                        var noOfComment = totalPostData[post].CommentCount;
                        totalNoOfComments += noOfComment;
                        totalNoOfLike += noOfLike;
                    }

                }
            }
            catch (Exception)
            {
                // ex.DebugLog();
            }
        }
    }

}
