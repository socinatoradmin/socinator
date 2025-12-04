using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.PdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
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
using System.Linq.Expressions;

namespace PinDominatorCore.PDLibrary.Processors.User
{
    public class UnfollowProcessor : BasePinterestUserProcessor
    {
        private UnfollowerModel UnfollowerModel { get; }
        public UnfollowProcessor(IPdJobProcess jobProcess, IDbGlobalService globalService,
            IDbCampaignService campaignService, IPinFunction objPinFunct, IDelayService delayService) :
            base(jobProcess, globalService, campaignService, objPinFunct, delayService)
        {
            UnfollowerModel =
                JsonConvert.DeserializeObject<UnfollowerModel>(TemplateModel.ActivitySettings);
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            if (UnfollowerModel.IsChkPeopleFollowedBySoftwareChecked && !jobProcessResult.IsProcessCompleted)
            {
                queryInfo = new QueryInfo { QueryType = "Followed by software" };
                UnfollowPeopleFollowedBySoftware(queryInfo, out jobProcessResult, TemplateModel);
            }

            if (UnfollowerModel.IsChkPeopleFollowedOutsideSoftwareChecked && !jobProcessResult.IsProcessCompleted)
            {
                queryInfo = new QueryInfo { QueryType = "Followed outside software" };
                UnfollowPeopleFollowedOutsideSoftware(queryInfo, out jobProcessResult);
            }

            if (UnfollowerModel.IsChkCustomUsersListChecked && !jobProcessResult.IsProcessCompleted)
            {
                queryInfo = new QueryInfo { QueryType = "Custom users" };
                UnfollowCustomUserList(queryInfo, out jobProcessResult, TemplateModel);
            }

            jobProcessResult.IsProcessCompleted = true;
        }

        private void UnfollowPeopleFollowedBySoftware(QueryInfo queryInfo, out JobProcessResult jobProcessResult,
            TemplateModel templateModel)
        {
            GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                "LangKeySearchingForPeopleFollowedBySoftware".FromResourceDictionary());
            var followedUserDict = GetUsersFromDb();

            followedUserDict.RemoveAll(x => x.QueryType == "Custom Board");
            jobProcessResult = new JobProcessResult();
            if (followedUserDict.Count > 0)
                try
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    GlobusLogHelper.log.Info(Log.FoundXResults, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.AccountBaseModel.UserName, followedUserDict.Count, queryInfo.QueryType, "", ActivityType);
                    followedUserDict = FilterWhiteListUser(templateModel, new List<InteractedUsers>(followedUserDict));
                    followedUserDict = AlreadyUnfollowedUser(new List<InteractedUsers>(followedUserDict));

                    var listOfUsers = new List<PinterestUser>();
                    foreach (var user in followedUserDict)
                    {
                        var pinterestUser = new PinterestUser
                        {
                            UserBio = user.Bio,
                            FollowersCount = user.FollowersCount,
                            FollowingsCount = user.FollowingsCount,
                            FullName = user.FullName,
                            HasProfilePic = user.HasAnonymousProfilePicture != null && user.HasAnonymousProfilePicture.Value,
                            PinsCount = user.PinsCount,
                            ProfilePicUrl = user.ProfilePicUrl,
                            Username = user.InteractedUsername,
                            UserId = user.InteractedUserId,
                            WebsiteUrl = user.Website,
                            FollowedBack = user.FollowedBack,
                            IsFollowedByMe = user.IsFollowedByMe,
                            IsVerified = user.IsVerified,
                            TriesCount = user.TriesCount
                        };
                        listOfUsers.Add(pinterestUser);
                    }

                    listOfUsers = AlreadyInteractedUser(listOfUsers);
                    listOfUsers = FilterWhiteListUser(TemplateModel, listOfUsers);
                    StartProcessForListOfUsers(queryInfo, ref jobProcessResult, listOfUsers);
                }
                catch (OperationCanceledException)
                {
                    throw new OperationCanceledException();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            else
                GlobusLogHelper.log.Info(Log.CustomMessage,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    "LangKeyNoUsersFoundToUnfollowBySoftware".FromResourceDictionary());
        }

        private void UnfollowPeopleFollowedOutsideSoftware(QueryInfo queryInfo, out JobProcessResult jobProcessResult)
        {
            try
            {
                string userUrl = JobProcess.DominatorAccountModel.AccountBaseModel.ProfileId;
                GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                      JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                      $"LangKeySearchingForPeopleFollowedOutsideSoftware".FromResourceDictionary());
                FollowerAndFollowingPtResponseHandler followerAndFollowingPtResponseHandler;
                jobProcessResult = new JobProcessResult();

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                var pinConfig =
                    genericFileManager.GetModel<PinterestOtherConfigModel>(ConstantVariable
                        .GetOtherPinterestSettingsFile());

                // Here we are scraping data until maximum pagination given by user and that data we will store into
                // database for further use then we will send the data to perform activity.
                if (pinConfig.IsScrapDataBeforeSendToPerformActivity)
                {
                    var maxPagination = 20;
                    var currentPagination = 0;
                    var lstScrapedUsers = new List<PinterestUser>();
                    var alreadyScraped = new List<InteractedUsers>();
                    alreadyScraped = DbAccountService.GetInteractedUsers(ActivityType + "_Scrap").ToList();
                    var lstUsername = alreadyScraped.Select(x => x.InteractedUsername).ToList();
                    lstUsername = AlreadyUnfollowedUser(lstUsername);
                    alreadyScraped = alreadyScraped.Where(x => lstUsername.Contains(x.Username)).ToList();
                    if (alreadyScraped.Count == 0)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        String.Format("LangKeyWaitForSomeTimeWhileScrappingMessage".FromResourceDictionary(), ActivityType));
                        if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                        {
                            lstScrapedUsers = BrowserManager.GetUserFollowings(JobProcess.DominatorAccountModel, userUrl,
                                JobProcess.JobCancellationTokenSource);
                        }
                        else
                            while (maxPagination >= currentPagination && jobProcessResult.HasNoResult == false)
                            {
                                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                followerAndFollowingPtResponseHandler = PinFunction.GetUserFollowings(userUrl, JobProcess.DominatorAccountModel,
                                    jobProcessResult.maxId);
                                System.Threading.Thread.Sleep(new Random().Next(3, 5));
                                if (followerAndFollowingPtResponseHandler == null || !followerAndFollowingPtResponseHandler.Success)
                                {
                                    jobProcessResult.HasNoResult = true;
                                    jobProcessResult.maxId = null;
                                }
                                else
                                {
                                    if (followerAndFollowingPtResponseHandler.HasMoreResults == false)
                                    {
                                        jobProcessResult.HasNoResult = true;
                                        jobProcessResult.maxId = null;
                                    }
                                    else
                                        jobProcessResult.maxId = followerAndFollowingPtResponseHandler.BookMark;
                                    lstScrapedUsers = followerAndFollowingPtResponseHandler.UsersList.Where(x => x.IsFollowedByMe).ToList();
                                    if (followerAndFollowingPtResponseHandler.HasMoreResults == false)
                                        jobProcessResult.HasNoResult = true;
                                    currentPagination++;
                                }
                            }
                        List<InteractedUsers> dataToBeAdded = new List<InteractedUsers>();
                        foreach (PinterestUser user in lstScrapedUsers)
                            if (alreadyScraped.All(x => x.InteractedUsername != user.Username))
                                dataToBeAdded.Add(new InteractedUsers
                                {
                                    ActivityType = ActivityType + "_Scrap",
                                    Date = DateTimeUtilities.GetEpochTime(),
                                    QueryType = queryInfo.QueryType,
                                    Query = queryInfo.QueryValue,
                                    Bio = user.UserBio,
                                    FollowersCount = user.FollowersCount,
                                    FollowingsCount = user.FollowingsCount,
                                    FullName = user.FullName,
                                    HasAnonymousProfilePicture = user.HasProfilePic,
                                    PinsCount = user.PinsCount,
                                    ProfilePicUrl = user.ProfilePicUrl,
                                    Username = JobProcess.DominatorAccountModel.AccountBaseModel.UserName,
                                    InteractedUsername = user.Username,
                                    InteractedUserId = user.UserId,
                                    InteractionTime = DateTimeUtilities.GetEpochTime(),
                                    Website = user.WebsiteUrl,
                                    FollowedBack = user.FollowedBack,
                                    IsFollowedByMe = user.IsFollowedByMe,
                                    IsVerified = user.IsVerified,
                                    TriesCount = user.TriesCount,
                                    Filtered = false,
                                    FullDetailsScraped = false,
                                    Type = "User"
                                });
                        DbAccountService.AddRange(dataToBeAdded);
                        alreadyScraped.AddRange(dataToBeAdded);
                    }

                    var lstUsers = new List<PinterestUser>();
                    foreach (var user in alreadyScraped)
                    {
                        var pinterestUser = new PinterestUser
                        {
                            UserBio = user.Bio,
                            FollowersCount = user.FollowersCount,
                            FollowingsCount = user.FollowingsCount,
                            FullName = user.FullName,
                            HasProfilePic = user.HasAnonymousProfilePicture != null && user.HasAnonymousProfilePicture.Value,
                            PinsCount = user.PinsCount,
                            ProfilePicUrl = user.ProfilePicUrl,
                            Username = user.InteractedUsername,
                            UserId = user.InteractedUserId,
                            WebsiteUrl = user.Website,
                            FollowedBack = user.FollowedBack,
                            IsFollowedByMe = user.IsFollowedByMe,
                            IsVerified = user.IsVerified,
                            TriesCount = user.TriesCount
                        };
                        lstUsers.Add(pinterestUser);
                    }

                    lstUsers = FilterWhiteListUser(TemplateModel, lstUsers);

                    var lstScrapedUsername = new List<string>();
                    lstScrapedUsername = alreadyScraped.Select(x => x.InteractedUsername).ToList();
                    lstScrapedUsername = AlreadyUnfollowedUser(lstScrapedUsername);
                    lstUsers = lstUsers.Where(x => lstScrapedUsername.Contains(x.Username)).ToList();
                    StartProcessForListOfUsers(queryInfo, ref jobProcessResult, lstUsers);
                }
                else
                {
                    List<PinterestUser> lstOfUsers = new List<PinterestUser>();
                    if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        int scroll = 0;
                        bool isScroll = false;
                        do
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            var listOfUsersTemp = BrowserManager.GetUserFollowings(JobProcess.DominatorAccountModel,
                                    userUrl, JobProcess.JobCancellationTokenSource, isScroll, scroll);
                            scroll = 1;
                            isScroll = true;
                            lstOfUsers = listOfUsersTemp.Where(x => x.IsFollowedByMe).ToList();
                            lstOfUsers = FilterBlackListUser(TemplateModel, lstOfUsers);
                            lstOfUsers = AlreadyInteractedUser(lstOfUsers);
                            StartProcessForListOfUsers(queryInfo, ref jobProcessResult, lstOfUsers);
                        }
                        while (lstOfUsers.Count > 0);
                    }
                    else
                        while (jobProcessResult.HasNoResult == false)
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            followerAndFollowingPtResponseHandler = PinFunction.GetUserFollowings
                                                            (userUrl, JobProcess.DominatorAccountModel, jobProcessResult.maxId);
                            if (followerAndFollowingPtResponseHandler == null || !followerAndFollowingPtResponseHandler.Success)
                            {
                                jobProcessResult.HasNoResult = true;
                                jobProcessResult.maxId = null;
                            }
                            else
                            {
                                List<PinterestUser> listOfUsers = new List<PinterestUser>();
                                listOfUsers = followerAndFollowingPtResponseHandler.UsersList.Where(x => x.IsFollowedByMe).ToList();
                                listOfUsers = FilterWhiteListUser(TemplateModel, listOfUsers);

                                StartProcessForListOfUsers(queryInfo, ref jobProcessResult, listOfUsers);
                                jobProcessResult.maxId = followerAndFollowingPtResponseHandler.BookMark;
                                if (followerAndFollowingPtResponseHandler.HasMoreResults == false)
                                    jobProcessResult.HasNoResult = true;

                            }
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
                jobProcessResult = new JobProcessResult();
            }
        }

        private void UnfollowCustomUserList(QueryInfo queryInfo, out JobProcessResult jobProcessResult,
            TemplateModel templateModel)
        {
            try
            {
                var unfollowerModel =
                    JsonConvert.DeserializeObject<UnfollowerModel>(templateModel.ActivitySettings);
                var customUsers = new List<string>();
                foreach (var user in unfollowerModel.ListCustomUsers)
                    customUsers.Add(user.Contains(".pinterest.")
                        ? user.Split('/')[3]
                        : user);

                customUsers = FilterWhiteListUser(templateModel, new List<string>(customUsers));
                customUsers = AlreadyUnfollowedUser(new List<string>(customUsers));

                jobProcessResult = new JobProcessResult();
                if (unfollowerModel.ListCustomUsers == null || unfollowerModel.ListCustomUsers.Count == 0)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        String.Format("LangKeyCustomUserListEmptyForSpecificAccountAndModule".FromResourceDictionary(), JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType.ToString()));
                    return;
                }

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (customUsers.Any())
                    StartProcessForListOfUsers(queryInfo, ref jobProcessResult, customUsers);
                else
                    GlobusLogHelper.log.Info(Log.NoMoreDataToPerform,
                        JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                jobProcessResult = new JobProcessResult();
            }
        }

        private List<InteractedUsers> GetFollowedUserFromDb()
        {
            try
            {
                var filterTimeStamp = 0;

                if (UnfollowerModel.IsUserFollowedBeforeChecked)
                    try
                    {
                        var filterDate = DateTime.UtcNow.AddDays(-UnfollowerModel.FollowedBeforeDay)
                            .AddHours(-UnfollowerModel.FollowedBeforeHour);
                        filterTimeStamp = filterDate.GetCurrentEpochTime();
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                var followActType = ActivityType.Follow.ToString();

                var expression =
                    UnfollowerModel.IsUserFollowedBeforeChecked
                        ? (Expression<Func<InteractedUsers, bool>>)(x =>
                           x.ActivityType == followActType && x.InteractionTime <= filterTimeStamp)
                        : x => x.ActivityType == followActType;

                return GetListFromDb(expression);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return new List<InteractedUsers>();
        }

        private List<InteractedUsers> GetUsersFromDb(bool outsideFollowed = false)
        {
            try
            {
                var interactedUsers = GetFollowedUserFromDb();
                var allFriends = GetAllFriendsFromDb();
                var returnList = new List<InteractedUsers>();
                if (outsideFollowed)
                    allFriends.ForEach(x =>
                    {
                        var users = new InteractedUsers();
                        if (interactedUsers.All(y => x.Username != y.InteractedUsername))
                        {
                            users.InteractedUsername = x.Username;
                            users.Bio = x.Bio;
                            users.FollowersCount = x.Followers;
                            users.FollowingsCount = x.Followings;
                            users.FullName = x.FullName;
                            users.HasAnonymousProfilePicture = x.HasAnonymousProfilePicture;
                            users.PinsCount = x.PinsCount;
                            users.ProfilePicUrl = x.ProfilePicUrl;
                            users.InteractedUserId = x.UserId;
                            users.Website = x.Website;
                            users.IsVerified = x.IsVerified;
                            returnList.Add(users);
                        }
                    });
                else
                    returnList.AddRange(interactedUsers);

                return returnList;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return new List<InteractedUsers>();
        }

        public List<Friendships> GetAllFriendsFromDb()
        {
            try
            {
                var expression =
                    UnfollowerModel.IsWhoFollowBackChecked && !UnfollowerModel.IsWhoDoNotFollowBackChecked
                        ? (Expression<Func<Friendships, bool>>)(x => x.FollowType == FollowType.Mutual)
                        : x => x.FollowType == FollowType.Following;

                return GetListFromDb(expression);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return new List<Friendships>();
        }

        private List<Friendships> GetListFromDb(Expression<Func<Friendships, bool>> unfollowExpression)
        {
            return DbAccountService.GetUnfollowingUsersCustomExpression(unfollowExpression);
        }

        private List<InteractedUsers> GetListFromDb(Expression<Func<InteractedUsers, bool>> unfollowExpression)
        {
            return DbAccountService.GetUnfollowingUsersCustomExpression(unfollowExpression);
        }
    }
}