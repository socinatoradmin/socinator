using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.PdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using PinDominatorCore.PDLibrary.DAL;
using PinDominatorCore.PDModel;
using PinDominatorCore.Response;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PinDominatorCore.PDLibrary.Processors.User
{
    public abstract class BasePinterestUserProcessor : BasePinterestProcessor
    {
        private readonly IDelayService _delayService;
        protected BasePinterestUserProcessor(IPdJobProcess jobProcess, IDbGlobalService globalService,
            IDbCampaignService campaignService, IPinFunction objPinFunct, IDelayService delayService) :
            base(jobProcess, globalService, campaignService, objPinFunct)
        {
            _delayService = delayService;
        }

        public void StartProcessForListOfUsers(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
            List<string> newPinterestUsersList)
        {
            foreach (var user in newPinterestUsersList)
                try
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                   
                   var userNameInfoPtResponseHandler = PinFunction.GetUserDetails(user, JobProcess.DominatorAccountModel).Result;

                    if (userNameInfoPtResponseHandler.Success)
                    {
                        if (ActivityType == ActivityType.Follow && userNameInfoPtResponseHandler.IsFollowed)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                String.Format("LangKeyYouAreAlreadyFollowingThisUser".FromResourceDictionary(), user));
                            _delayService.ThreadSleep(1000);
                            continue;
                        }

                        if (ActivityType == ActivityType.Unfollow && !userNameInfoPtResponseHandler.IsFollowed)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                String.Format("LangKeyYouAreNotFollowingThisUser".FromResourceDictionary(), user));
                            _delayService.ThreadSleep(1000);
                            continue;
                        }

                        StartCustomUserProcess(queryInfo, ref jobProcessResult, userNameInfoPtResponseHandler);
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.NoMoreDataToPerform, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType);
                        jobProcessResult.HasNoResult = true;
                        jobProcessResult.maxId = null;
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
                finally
                {
                    if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser && BrowserManager.BrowserWindows.Count > 1)
                        BrowserManager.CloseLast();
                }
        }

        public void StartProcessForListOfUsers(QueryInfo queryInfo, ref JobProcessResult jobProcessResult, List<PinterestUser> lstPinUser)
        {
            foreach (var user in lstPinUser)
            {                
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                try
                {
                    if (ActivityType == ActivityType.Follow && user.IsFollowedByMe)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            String.Format("LangKeyYouAreAlreadyFollowingThisUser".FromResourceDictionary(), user));
                        _delayService.ThreadSleep(1000);
                        continue;
                    }
                    if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser && ActivityType != ActivityType.Comment &&
                        ActivityType != ActivityType.UserScraper)
                        BrowserManager.AddNew(JobProcess.DominatorAccountModel.CancellationSource, $"https://{BrowserManager.Domain}");

                    StartCustomUserProcess(queryInfo, ref jobProcessResult, user);
                }
                catch (OperationCanceledException)
                {
                    throw new OperationCanceledException();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                finally
                {
                    if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser && BrowserManager.BrowserWindows.Count > 1)
                        BrowserManager.CloseLast();
                }
            }
        }

        public void StartCustomUserProcess(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
            PinterestUser pinUser)
        {
            try
            {
                var unfollowerModel =
                           JsonConvert.DeserializeObject<UnfollowerModel>(TemplateModel.ActivitySettings);
                var followerModel =
                           JsonConvert.DeserializeObject<FollowerModel>(TemplateModel.ActivitySettings);
                if (ActivityType == ActivityType.Unfollow)
                {
                    var followersList =
                        DbAccountService.GetFriendships(FollowType.FollowingBack, FollowType.Mutual)
                            .Select(x => x.Username).ToList();
                    var isFollower = followersList.Contains(pinUser.Username);
                    if (unfollowerModel.IsWhoDoNotFollowBackChecked && !unfollowerModel.IsWhoFollowBackChecked &&
                        isFollower)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            String.Format("LangKeyFilterAppliedOnUsersDoNotFollowBack".FromResourceDictionary(), pinUser.Username));
                        return;
                    }

                    if (unfollowerModel.IsWhoFollowBackChecked && !unfollowerModel.IsWhoDoNotFollowBackChecked &&
                        !isFollower)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            String.Format("LangKeyFilterAppliedOnUsersFollowBack".FromResourceDictionary(), pinUser.Username));
                        return;
                    }
                }
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                //Get all user list who is connected through messages
                if (ActivityType == ActivityType.BroadcastMessages || ActivityType == ActivityType.AutoReplyToNewMessage)
                {
                    var broadCastMessageModel = JsonConvert.DeserializeObject<BroadcastMessagesModel>(TemplateModel.ActivitySettings);
                    var autoReplyToNewMessageModel = JsonConvert.DeserializeObject<AutoReplyToNewMessageModel>(TemplateModel.ActivitySettings);
                    if (broadCastMessageModel.SkipUserWhoHasEverReceivedMessage || autoReplyToNewMessageModel.SkipUserWhoHasEverReceivedMessage)
                    {
                        var lstUserConnectWithMessage = PinFunction.UserConnectedWithMessage(JobProcess.DominatorAccountModel);

                        if (lstUserConnectWithMessage.Any(x => x == pinUser.Username))
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                JobProcess.DominatorAccountModel.UserName, ActivityType,
                                "LangKeySkippingThisUserMessageAlreadySent".FromResourceDictionary() + $" {pinUser.Username}");
                            return;
                        }
                    }
                }

                if (FilterPinActionList.Count > 0)
                {
                    var allPins = PinFunction.
                        GetPinsFromSpecificUser(pinUser.Username, JobProcess.DominatorAccountModel,NeedCommentCount:unfollowerModel.PostFilterModel.FilterComments);
                    if (allPins.LstUserPin.Count == 0 && allPins.HasMoreResults)
                        allPins = PinFunction.
                        GetPinsFromSpecificUser(pinUser.Username, JobProcess.DominatorAccountModel, allPins.BookMark, NeedCommentCount: unfollowerModel.PostFilterModel.FilterComments);

                    foreach (var pin in allPins.LstUserPin)
                        if (FilterPinApply(pin, 1))
                            return;
                }

                if (FilterUserActionList.Count > 0)
                {
                    InteractedUsers user = null;

                    if (user == null || user.FullDetailsScraped == null || !user.FullDetailsScraped.Value)
                    {
                        if (user == null)
                            user = new InteractedUsers();

                        pinUser = PinFunction.GetUserDetails(pinUser.Username, JobProcess.DominatorAccountModel).Result;

                        user.Bio = pinUser.UserBio;
                        user.FollowingsCount = pinUser.FollowingsCount;
                        user.FollowersCount = pinUser.FollowersCount;
                        user.FullName = pinUser.FullName;
                        user.PinsCount = pinUser.PinsCount;
                        user.IsVerified = pinUser.IsVerified;
                        user.ProfilePicUrl = pinUser.ProfilePicUrl;
                        user.FullDetailsScraped = true;
                        user.HasAnonymousProfilePicture = pinUser.HasProfilePic;
                        user.IsFollowedByMe = pinUser.IsFollowedByMe;
                        user.IsVerified = pinUser.IsVerified;
                        user.InteractedUsername = pinUser.Username;
                        user.InteractedUserId = pinUser.UserId;
                        user.Username = JobProcess.DominatorAccountModel.AccountBaseModel.UserName;
                        user.Website = pinUser.WebsiteUrl;

                        if (user.ActivityType == null)
                        {
                            user.Query = queryInfo.QueryValue;
                            user.QueryType = queryInfo.QueryType;
                            user.ActivityType = ActivityType + "_Scrap";
                            DbAccountService.Add(user);
                        }
                        else
                            DbAccountService.Update(user);
                    }
                    else
                    {
                        pinUser.UserBio = user.Bio;
                        pinUser.FollowersCount = user.FollowersCount;
                        pinUser.FollowingsCount = user.FollowingsCount;
                        pinUser.FullName = user.FullName;
                        pinUser.HasProfilePic = user.HasAnonymousProfilePicture.Value;
                        pinUser.PinsCount = user.PinsCount;
                        pinUser.ProfilePicUrl = user.ProfilePicUrl;
                        pinUser.Username = user.InteractedUsername;
                        pinUser.UserId = user.InteractedUserId;
                        pinUser.WebsiteUrl = user.Website;
                        pinUser.FollowedBack = user.FollowedBack;
                        pinUser.IsFollowedByMe = user.IsFollowedByMe;
                        pinUser.IsVerified = user.IsVerified;
                        pinUser.TriesCount = user.TriesCount;
                    }

                    if (FilterUserApply(pinUser, 1))
                    {
                        user.Filtered = true;
                        DbAccountService.Update(user);
                        return;
                    }
                }
                GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,JobProcess.DominatorAccountModel.UserName, ActivityType,$"Please Wait....Fetching Details of {{ {pinUser.FullName} }}");
                UserNameInfoPtResponseHandler UserDetails = null;
                if ((ActivityType.Follow == ActivityType || ActivityType.Unfollow == ActivityType) 
                    && !(followerModel.UserFilterModel.FilterPostCounts||unfollowerModel.UserFilterModel.FilterPostCounts
                    ||followerModel.ChkCommentOnUserLatestPostsChecked))
                    UserDetails = PinFunction.GetUserDetails(pinUser.Username, JobProcess.DominatorAccountModel,false).Result;
                else
                    UserDetails = PinFunction.GetUserDetails(pinUser.Username, JobProcess.DominatorAccountModel).Result;
                pinUser = UserDetails;
                StartFinalProcess(ref jobProcessResult, pinUser, queryInfo);
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

        public void StartFinalProcess(ref JobProcessResult jobProcessResult, PinterestUser newPinterestUser,
            QueryInfo queryInfo)
        {
            jobProcessResult = JobProcess.FinalProcess(new ScrapeResultNew
            {
                ResultUser = newPinterestUser,
                QueryInfo = queryInfo
            });
        }
    }
}