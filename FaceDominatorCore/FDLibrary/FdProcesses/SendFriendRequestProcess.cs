using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDModel.FriendsModel;
using FaceDominatorCore.FDRequest;
using FaceDominatorCore.FDResponse.MessagesResponse;
using FaceDominatorCore.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using ThreadUtils;
using Unity;
using AccountInteractedUsres = DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedUsers;
using CampaignInteractedUsres = DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedUsers;
using InteractedPosts = DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedPosts;

namespace FaceDominatorCore.FDLibrary.FdProcesses
{
    public class SendFriendRequestProcess : FdJobProcessInteracted<AccountInteractedUsres>
    {
        public SendFriendRequestModel SendRequestModel { get; set; }

        public DominatorAccountModel Account { get; set; }

        private readonly IFdRequestLibrary FdRequestLibrary;

        private readonly IAccountScopeFactory _accountScopeFactory;

        private readonly IFdRequestLibrary _fdRequestLibrary;
        private readonly IDelayService _delayService;

        public SendFriendRequestProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IFdQueryScraperFactory queryScraperFactory,
            IFdHttpHelper fdHttpHelper, IFdLoginProcess fdLogInProcess, IFdRequestLibrary fdRequestLibrary,
            IDbCampaignServiceScoped dbCampaignServiceScoped)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, fdHttpHelper, fdLogInProcess, dbCampaignServiceScoped)
        {
            _delayService = InstanceProvider.GetInstance<IDelayService>();
            _fdRequestLibrary = fdRequestLibrary;
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            SendRequestModel = processScopeModel.GetActivitySettingsAs<SendFriendRequestModel>();
            AccountModel = DominatorAccountModel;
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobProcessResult jobProcessResult = new JobProcessResult();

            FacebookUser objFacebookUser = (FacebookUser)scrapeResult.ResultUser;

            var query = scrapeResult.QueryInfo;

            try
            {
                var fdCampaignInteractionDetails = InstanceProvider.GetInstance<ICampaignInteractionDetails>();


                var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

                var modulesetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (modulesetting == null)
                    return jobProcessResult;

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (modulesetting.IsTemplateMadeByCampaignMode && SendRequestModel.IschkUniqueRequestChecked)
                {
                    try
                    {
                        fdCampaignInteractionDetails.AddInteractedData(SocialNetworks, CampaignId, objFacebookUser.UserId);
                    }
                    catch (Exception)
                    {
                        jobProcessResult.IsProcessSuceessfull = false;
                        return jobProcessResult;
                    }
                }

                string requestStatus = string.Empty;

                if (AccountModel.IsRunProcessThroughBrowser)
                {
                    string url = string.IsNullOrEmpty(objFacebookUser.ScrapedProfileUrl)
                        ? objFacebookUser.ProfileUrl
                        : objFacebookUser.ScrapedProfileUrl;
                    url = string.IsNullOrEmpty(url) ? $"{FdConstants.FbHomeUrl}{objFacebookUser.UserId}" : url;
                    FdLogInProcess._browserManager.LoadPageSource(AccountModel, url);
                    requestStatus = FdLogInProcess._browserManager.SendFriendRequestSingleUser(AccountModel, objFacebookUser.UserId);

                }
                else
                    requestStatus = _fdRequestLibrary.SendFriendRequest(AccountModel, objFacebookUser.UserId);

                if (requestStatus == "success" || requestStatus == "successfollowing")
                {
                    if (requestStatus == "success")
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objFacebookUser.UserId);
                    else
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Successfully Followed {objFacebookUser.UserId}");
                    IncrementCounters();
                    AddSendRequestDataToDatabase(scrapeResult);
                    StartAfterAction(scrapeResult);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    if (requestStatus.Contains("Request blocked by facebook to user"))
                        AddSkippedDataToDb(objFacebookUser.UserId);

                    //AddSendRequestDataToDatabase(scrapeResult);

                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"{requestStatus} {objFacebookUser.UserId}");

                    if (modulesetting.IsTemplateMadeByCampaignMode && SendRequestModel.IschkUniqueRequestChecked)
                        fdCampaignInteractionDetails.RemoveIfExist(SocialNetworks, CampaignId, objFacebookUser.UserId);

                    jobProcessResult.IsProcessSuceessfull = false;
                }
                DelayBeforeNextActivity();
            }
            catch (OperationCanceledException)
            {
                _accountScopeFactory[$"{AccountModel.AccountId}{objFacebookUser.UserId}"].Resolve<IFdBrowserManager>().CloseBrowser(AccountModel);
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return jobProcessResult;
        }

        private void AddSkippedDataToDb(string userId)
        {
            try
            {
                DbAccountService.Add(new AccountInteractedUsres
                {
                    ActivityType = "SkippedUsers",
                    UserId = userId,
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        private void StartAfterAction(ScrapeResultNew scrapeResult)
        {
            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                FacebookUser objFacebookUser = (FacebookUser)scrapeResult.ResultUser;

                if (SendRequestModel.ChkSendDirectMessageAfterFollowChecked ||
                    SendRequestModel.IsChkLikeUsersLatestPost || SendRequestModel.ChkCommentOnUserLatestPostsChecked)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Started After Action For User {FdConstants.FbHomeUrl}{objFacebookUser.UserId}");
                }

                int delayBetweenMessages = SendRequestModel.DelayBetweenMessagesForAfterActivity.GetRandom();
                var pageSource = "";
                if (SendRequestModel.ChkSendDirectMessageAfterFollowChecked)
                {
                    if (objFacebookUser.CanSendMessage)
                    {
                        string url = string.IsNullOrEmpty(objFacebookUser.ScrapedProfileUrl)
                            ? objFacebookUser.ProfileUrl
                            : objFacebookUser.ScrapedProfileUrl;

                        url = string.IsNullOrEmpty(url) ? $"{FdConstants.FbHomeUrl}{objFacebookUser.UserId}" : url;

                        IFdBrowserManager browserManager = null;
                        if (AccountModel.IsRunProcessThroughBrowser)
                        {
                            browserManager = _accountScopeFactory[$"{objFacebookUser.UserId}{AccountModel.AccountId}"]
                                .Resolve<IFdBrowserManager>();
                            pageSource = browserManager.LoadPageSource(AccountModel, url);
                        }
                        //to check if the ACCOUNT IS blocked you or not
                        if (!string.IsNullOrEmpty(pageSource) && pageSource.Contains("This content isn't available at the moment"))
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Failed to send message to User {FdConstants.FbHomeUrl}{objFacebookUser.UserId}");

                        }
                        else
                        {
                            string messageTag = SendRequestModel.IsTagChecked
                                ? ReplaceTagWithValue(objFacebookUser, SendRequestModel.Message)
                                : SendRequestModel.Message;

                            FdSendTextMessageResponseHandler isSuccess = AccountModel.IsRunProcessThroughBrowser
                                ? browserManager.SendMessage(AccountModel, objFacebookUser.UserId, messageTag, openWindow: true)
                                : _fdRequestLibrary.SendTextMessage(AccountModel, objFacebookUser.UserId,
                                    messageTag);
                            if (isSuccess.Status)
                            {
                                KeyValuePair<string, string> message =
                                    new KeyValuePair<string, string>($"{messageTag}", "");
                                //      AddMessageRequestDataToDatabase(scrapeResult, message);
                                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Successful to send message to User {FdConstants.FbHomeUrl}{objFacebookUser.UserId}");
                                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Added {delayBetweenMessages} sec delay between message to user");
                                _delayService.ThreadSleep(delayBetweenMessages * 1000);
                            }
                            else
                            {
                                var error = isSuccess != null && isSuccess.FbErrorDetails != null ? isSuccess.FbErrorDetails.Description : "Unknown Error";
                                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Failed to send message to User {FdConstants.FbHomeUrl}{objFacebookUser.UserId} error:{error}");
                            }


                        }
                        browserManager.CloseBrowser(AccountModel);
                    }
                    else
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Cannot send message to User {FdConstants.FbHomeUrl}{objFacebookUser.UserId}");

                }
                if (SendRequestModel.IsChkLikeUsersLatestPost || SendRequestModel.ChkCommentOnUserLatestPostsChecked)
                {
                    LikerCommentOnPost(scrapeResult);
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        public string ReplaceTagWithValue(FacebookUser objFacebookUser, string message)
        {
            try
            {
                var splitUserName = Regex.Split(objFacebookUser.FullName, " ");
                var myFirstName = Regex.Split(AccountModel.AccountBaseModel.UserFullName, " ")[0];
                var firstName = splitUserName[0];
                var lastName = splitUserName.Length > 2 ? splitUserName[2] : (splitUserName.Length > 1 ? splitUserName[1] : string.Empty);
                message = Regex.Replace(message, "<FULLNAME>", objFacebookUser.FullName);
                message = Regex.Replace(message, "<FIRSTNAME>", firstName);
                message = Regex.Replace(message, "<LASTNAME>", lastName);
                if ((message.Contains("<MYNAME>") || message.Contains("<MYFIRSTNAME>")) && !string.IsNullOrEmpty(AccountModel.AccountBaseModel.UserFullName))
                {
                    message = Regex.Replace(message, "<MYNAME>", AccountModel.AccountBaseModel.UserFullName);
                    message = Regex.Replace(message, "<MYFIRSTNAME>", myFirstName);
                }


                return message.Trim();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return string.Empty;
        }

        private void LikerCommentOnPost(ScrapeResultNew scrapeResult)
        {
            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                FacebookUser objFacebookUser = (FacebookUser)scrapeResult.ResultUser;

                int delayBetweenComments = SendRequestModel.DelayBetweenCommentsForAfterActivity.GetRandom();

                int delayBetweenLikes = SendRequestModel.DelayBetweenLikesForAfterActivity.GetRandom();

                int noOfLikes = SendRequestModel.LikeBetweenJobs.GetRandom();

                int noOfComments = SendRequestModel.CommentsPerUser.GetRandom();

                int totalLikes = 0;

                int totalCommments = 0;

                IResponseHandler scrapPostListFromTimelineResponseHandler = null;

                while (scrapPostListFromTimelineResponseHandler == null || scrapPostListFromTimelineResponseHandler.HasMoreResults)
                {
                    string url = string.IsNullOrEmpty(objFacebookUser.ScrapedProfileUrl)
                        ? objFacebookUser.ProfileUrl
                        : objFacebookUser.ScrapedProfileUrl;

                    url = string.IsNullOrEmpty(url) ? $"{FdConstants.FbHomeUrl}{objFacebookUser.UserId}" : url;

                    IFdBrowserManager browserManager = null;
                    if (AccountModel.IsRunProcessThroughBrowser)
                    {
                        browserManager = _accountScopeFactory[$"{objFacebookUser.UserId}{AccountModel.AccountId}"]
                            .Resolve<IFdBrowserManager>();
                        browserManager.LoadPageSource(AccountModel, url, clearandNeedResource: true);
                    }

                    scrapPostListFromTimelineResponseHandler = AccountModel.IsRunProcessThroughBrowser
                        ? browserManager.ScrollWindowAndGetDataForPost(AccountModel, FbEntityType.Friends, 7, 0)
                        : _fdRequestLibrary.GetPostListFromFriendTimelineNew(AccountModel, scrapPostListFromTimelineResponseHandler, objFacebookUser.UserId);

                    foreach (var post in scrapPostListFromTimelineResponseHandler.ObjFdScraperResponseParameters.ListPostDetails)
                    {

                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        var userSpecificWindow = _accountScopeFactory[$"{AccountModel.AccountId}{post.Id}"]
                                .Resolve<IFdBrowserManager>();
                        if (string.IsNullOrEmpty(post.PostUrl) || string.IsNullOrEmpty(post.OwnerId))
                        {
                            if (AccountModel.IsRunProcessThroughBrowser)
                                userSpecificWindow.GetFullPostDetails(AccountModel, post);
                            else
                                _fdRequestLibrary.GetPostDetailNew(AccountModel, post);
                        }
                        if (!string.IsNullOrEmpty(post.PostUrl) || post.OwnerId == objFacebookUser.UserId || objFacebookUser.ScrapedProfileUrl.Contains(post.OwnerId))
                        {
                            if (SendRequestModel.IsChkLikeUsersLatestPost && totalLikes < noOfLikes)
                            {
                                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                                var isSuccess = AccountModel.IsRunProcessThroughBrowser
                                    ? userSpecificWindow.LikePost(AccountModel, post, ReactionType.Like)
                                    : _fdRequestLibrary.LikeUnlikePost(AccountModel, post.Id, ReactionType.Like);

                                if (isSuccess)
                                {
                                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                        $"Successful to Like posts with url {FdConstants.FbHomeUrl}{post.Id}");
                                    AddProfileScraperDataToDatabase(post, string.Empty, ActivityType.Like);

                                }
                                else
                                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                        $"Failed to Like posts with url {FdConstants.FbHomeUrl}{post.Id}");

                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    $"Added {delayBetweenLikes} sec delay between Like post");
                                Thread.Sleep(delayBetweenLikes * 1000);

                                totalLikes++;
                            }


                            if (SendRequestModel.ChkCommentOnUserLatestPostsChecked && totalCommments < noOfComments)
                            {
                                var comment = GetComments();
                                List<string> commentList = new List<string>();
                                commentList.Add(comment);

                                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                var isSuccess = AccountModel.IsRunProcessThroughBrowser
                                    ? userSpecificWindow.CommentOnSinglePost(AccountModel, post, commentList)
                                    : _fdRequestLibrary.CommentOnPost(AccountModel, post.Id, comment).ObjFdScraperResponseParameters.IsCommentedOnPost;

                                if (isSuccess)
                                {
                                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Successful to comment on posts with url {FdConstants.FbHomeUrl}{post.Id}");
                                    AddProfileScraperDataToDatabase(post, comment, ActivityType.Comment);

                                }
                                else
                                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Failed to comment posts with url {FdConstants.FbHomeUrl}{post.Id}");

                                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Added {delayBetweenComments} sec delay between comment on post");
                                _delayService.ThreadSleep(delayBetweenComments * 1000);

                                totalCommments++;
                            }

                            if ((SendRequestModel.ChkCommentOnUserLatestPostsChecked && totalCommments <= noOfComments && !SendRequestModel.IsChkLikeUsersLatestPost)
                                || (SendRequestModel.IsChkLikeUsersLatestPost && totalLikes <= noOfLikes && !SendRequestModel.ChkCommentOnUserLatestPostsChecked) ||
                                (totalCommments <= noOfComments && totalLikes <= noOfLikes))
                            {
                                userSpecificWindow.CloseBrowser(AccountModel);
                                break;
                            }
                        }
                        userSpecificWindow.CloseBrowser(AccountModel);
                    }

                    if ((SendRequestModel.ChkCommentOnUserLatestPostsChecked && totalCommments <= noOfComments && !SendRequestModel.IsChkLikeUsersLatestPost)
                                                    || (SendRequestModel.IsChkLikeUsersLatestPost && totalLikes <= noOfLikes && !SendRequestModel.ChkCommentOnUserLatestPostsChecked) ||
                                                    (totalCommments <= noOfComments && totalLikes <= noOfLikes))
                    {
                        browserManager.CloseBrowser(AccountModel);
                        break;
                    }

                }

            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private string GetComments()
        {
            var commentList = Regex.Split(SendRequestModel.UploadComment, "\r\n");
            commentList.Shuffle();
            return commentList.FirstOrDefault();
        }

        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {
            try
            {

                base.StartOtherConfiguration(scrapeResult);

                _fdRequestLibrary.ChangeLanguage(AccountModel, FdConstants.AccountLanguage[AccountModel.AccountBaseModel.UserId]);


                List<CampaignInteractedUsres> lstTotalRequestedUsers = ObjDbCampaignService.GetAllInteractedData<CampaignInteractedUsres>(x => x.ActivityType == ActivityType.ToString());


                GlobusLogHelper.log.Info(Log.OtherConfigurationStarted, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType);


                #region Sttop Send Request and Start Withdraw
                if (SendRequestModel.IsChkEnableAutoSendRequestWithdrawChecked)
                {

                    if (SendRequestModel.IsChkStopFriendToolWhenReachChecked)
                    {
                        if (SendRequestModel.IsChkStopFriendToolWhenReachChecked && (lstTotalRequestedUsers.Count >= SendRequestModel.StopFollowToolWhenReach.GetRandom()))
                        {
                            try
                            {
                                var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                                if (!dominatorScheduler.EnableDisableModules(ActivityType.SendFriendRequest, ActivityType.Unfriend, DominatorAccountModel.AccountId))
                                {
                                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType, "Send Friend activity has met your Auto Enable configuration for Unfriend, but you do not have Unfriend configuration saved. Please save the Unfriend configuration manually, to restart the Send Friend/Unfriend activity from this account");
                                }

                            }
                            catch (InvalidOperationException ex)
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.UserName, ActivityType,
                                    ex.Message.Contains("1001") ? "Send Friend activity has met your Auto Enable configuration for Unfriend, but you do not have Unfriend configuration saved. Please save the Unfriend configuration manually, to restart the Send Friend/Unfriend activity from this account" : "");
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }
                        }
                    }
                }

                #endregion



            }
            catch (Exception ex)
            {
                ex.DebugLog();
                //   GlobusLogHelper.log.Error($"TwtDominator : [Account: {DominatorAccountModel.AccountBaseModel.UserName}] => Error : {ex.Message}. StackTrace => {ex.StackTrace}  (Module => {ActivityType.ToString()})");
            }

        }


        private void AddSendRequestDataToDatabase(ScrapeResultNew scrapeResult)
        {
            try
            {

                FacebookUser user = (FacebookUser)scrapeResult.ResultUser;

                var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

                var modulesetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (modulesetting == null)
                    return;

                if (modulesetting.IsTemplateMadeByCampaignMode)
                {

                    DbCampaignService.Add(new CampaignInteractedUsres
                    {
                        ActivityType = ActivityType.ToString(),
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        AccountEmail = DominatorAccountModel.AccountBaseModel.UserName,
                        UserId = user.UserId,
                        Username = user.Familyname,
                        UserProfileUrl = user.ProfileUrl,
                        DetailedUserInfo = JsonConvert.SerializeObject(user),
                        InteractionTimeStamp = DateTime.UtcNow.GetCurrentEpochTime(),
                        InteractionDateTime = DateTime.Now,
                        ScrapedProfileUrl = user.ScrapedProfileUrl,
                        Gender = user.Gender,
                        University = user.University,
                        Workplace = user.WorkPlace,
                        CurrentCity = user.Currentcity,
                        HomeTown = user.Hometown,
                        BirthDate = user.DateOfBirth,
                        ContactNo = user.ContactNo,
                        ProfilePic = user.ProfilePicUrl
                    });
                }

                DbAccountService.Add(new AccountInteractedUsres
                {
                    ActivityType = ActivityType.ToString(),
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    UserId = user.UserId,
                    Username = user.Familyname,
                    UserProfileUrl = user.ProfileUrl,
                    DetailedUserInfo = JsonConvert.SerializeObject(user),
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    InteractionDateTime = DateTime.Now,
                    ScrapedProfileUrl = user.ScrapedProfileUrl
                });
            }
            catch (Exception ex)
            {
                ex.ErrorLog(ex.Message);
            }
        }



        void AddProfileScraperDataToDatabase(FacebookPostDetails postDetails, string comment,
            ActivityType activityType, string mentions = "")
        {
            try
            {
                var likeType = ReactionType.Like.ToString();


                var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

                var modulesetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (modulesetting == null)
                    return;

                if (modulesetting.IsTemplateMadeByCampaignMode)
                {
                    ObjDbCampaignService.Add(new InteractedPosts
                    {
                        ActivityType = activityType.ToString(),
                        QueryType = postDetails.QueryType,
                        QueryValue = postDetails.QueryValue,
                        AccountEmail = DominatorAccountModel.AccountBaseModel.UserName,
                        PostId = postDetails.Id,
                        LikeType = likeType,
                        Comment = comment,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        InteractionDateTime = DateTime.Now,
                        PostDescription = JsonConvert.SerializeObject(postDetails),
                        Mentions = mentions
                    });
                }

                ObjDbAccountService.Add(new DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedPosts
                {
                    ActivityType = activityType.ToString(),
                    QueryType = postDetails.QueryType,
                    QueryValue = postDetails.QueryValue,
                    PostId = postDetails.Id,
                    LikeType = likeType,
                    Comment = comment,
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    InteractionDateTime = DateTime.Now,
                    PostDescription = JsonConvert.SerializeObject(postDetails),
                    Mentions = mentions

                });
            }
            catch (Exception ex)
            {
                ex.ErrorLog(ex.Message);
            }
        }

        private void AddMessageRequestDataToDatabase(ScrapeResultNew scrapeResult, KeyValuePair<string, string> message)
        {
            try
            {

                FacebookUser user = (FacebookUser)scrapeResult.ResultUser;

                var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

                var modulesetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (modulesetting == null)
                    return;

                if (modulesetting.IsTemplateMadeByCampaignMode)
                {
                    DbCampaignService.Add(new CampaignInteractedUsres
                    {
                        ActivityType = ActivityType.BroadcastMessages.ToString(),
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        AccountEmail = DominatorAccountModel.AccountBaseModel.UserName,
                        UserId = user.UserId,
                        Username = user.Familyname,
                        UserProfileUrl = user.ProfileUrl,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        InteractionDateTime = DateTime.Now,
                        DetailedUserInfo = JsonConvert.SerializeObject(message)

                    });
                }

                DbAccountService.Add(new AccountInteractedUsres
                {
                    ActivityType = ActivityType.BroadcastMessages.ToString(),
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    UserId = user.UserId,
                    Username = user.Familyname,
                    UserProfileUrl = user.ProfileUrl,
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    InteractionDateTime = DateTime.Now,
                    DetailedUserInfo = JsonConvert.SerializeObject(message)
                });
            }
            catch (Exception ex)
            {
                ex.ErrorLog(ex.Message);
            }
        }
    }


}
