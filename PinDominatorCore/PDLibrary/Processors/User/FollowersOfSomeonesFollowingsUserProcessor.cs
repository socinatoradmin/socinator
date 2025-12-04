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
using PinDominatorCore.PDLibrary.DAL;
using PinDominatorCore.PDModel;
using PinDominatorCore.Response;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PinDominatorCore.PDLibrary.Processors.User
{
    public class FollowersOfSomeonesFollowingsUserProcessor : BasePinterestUserProcessor
    {
        private FollowerAndFollowingPtResponseHandler _followingUsers;
        public FollowersOfSomeonesFollowingsUserProcessor(IPdJobProcess jobProcess, IDbGlobalService globalService,
            IDbCampaignService campaignService, IPinFunction objPinFunct, IPdBrowserManager browserManager, IDelayService delayService) :
            base(jobProcess, globalService, campaignService, objPinFunct, delayService)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                var pinConfig =
                    genericFileManager.GetModel<PinterestOtherConfigModel>(ConstantVariable
                        .GetOtherPinterestSettingsFile());

                // Here we are scraping data until maximum pagination given by user and that data we will store into
                // database for further use then we will send the data to perform activity.
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (pinConfig.IsScrapDataBeforeSendToPerformActivity)
                {
                    var maxPagination = pinConfig.PaginationCount;
                    var currentPagination = 0;
                    var listOfScrapedUsers = new List<PinterestUser>();
                    var alreadyScraped = new List<InteractedUsers>();
                    alreadyScraped = DbAccountService
                        .GetInteractedUsersWithSameQuery(ActivityType + "_Scrap", queryInfo).ToList();
                    if (alreadyScraped.Count == 0)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                         JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                       String.Format("LangKeyWaitForSomeTimeWhileScrappingMessage".FromResourceDictionary(), ActivityType));

                        if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                        {
                            int scroll = 0;
                            bool isScroll = false;
                            while (maxPagination >= currentPagination && jobProcessResult.HasNoResult == false)
                            {
                                List<PinterestUser> listOfPinterestUsers = BrowserManager.GetUserFollowings(JobProcess.DominatorAccountModel,
                                    queryInfo.QueryValue, JobProcess.JobCancellationTokenSource, isScroll, scroll);
                            }
                            isScroll = true;
                            scroll = 1;
                        }
                        else
                            listOfScrapedUsers=GetUserFollowings(ref maxPagination, ref currentPagination, ref jobProcessResult,ref queryInfo);
                    }
                    else
                        listOfScrapedUsers= GetUserFollowings(ref maxPagination, ref currentPagination, ref jobProcessResult, ref queryInfo);
                    var dataToBeAdded = new List<InteractedUsers>();
                    foreach (var user in listOfScrapedUsers)
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

                    var listOfUsers = new List<PinterestUser>();
                    foreach (var user in alreadyScraped)
                    {
                        var pinterestUser = new PinterestUser();
                        pinterestUser.UserBio = user.Bio;
                        pinterestUser.FollowersCount = user.FollowersCount;
                        pinterestUser.FollowingsCount = user.FollowingsCount;
                        pinterestUser.FullName = user.FullName;
                        pinterestUser.HasProfilePic = user.HasAnonymousProfilePicture.Value;
                        pinterestUser.PinsCount = user.PinsCount;
                        pinterestUser.ProfilePicUrl = user.ProfilePicUrl;
                        pinterestUser.Username = user.InteractedUsername;
                        pinterestUser.UserId = user.InteractedUserId;
                        pinterestUser.WebsiteUrl = user.Website;
                        pinterestUser.FollowedBack = user.FollowedBack;
                        pinterestUser.IsFollowedByMe = user.IsFollowedByMe;
                        pinterestUser.IsVerified = user.IsVerified;
                        pinterestUser.TriesCount = user.TriesCount;
                        listOfUsers.Add(pinterestUser);
                    }

                    listOfUsers = AlreadyInteractedUser(listOfUsers);
                    listOfUsers = FilterBlackListUser(TemplateModel, new List<PinterestUser>(listOfUsers));

                    StartProcessForListOfUsers(queryInfo, ref jobProcessResult, listOfUsers);
                }
                else
                {
                    List<PinterestUser> followings = new List<PinterestUser>();
                    if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        bool isScroll = false;
                        int scroll = 0;
                        do
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            followings = BrowserManager.GetUserFollowings(JobProcess.DominatorAccountModel,
                                queryInfo.QueryValue, JobProcess.JobCancellationTokenSource,
                                isScroll, scroll);
                            scroll = 1;
                            foreach (PinterestUser user in followings)
                            {
                                try
                                {
                                    BrowserManager.AddNew(JobProcess.JobCancellationTokenSource, $"https://{BrowserManager.Domain}");
                                    bool isScrollInner = false;
                                    int scrollInner = 0;
                                    List<PinterestUser> followersOfFollowings = new List<PinterestUser>();
                                    do
                                    {
                                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                        followersOfFollowings = BrowserManager.GetUserFollowers(JobProcess.DominatorAccountModel,
                                        user.Username, JobProcess.JobCancellationTokenSource, isScrollInner, scrollInner);

                                        if (ActivityType == ActivityType.Follow)
                                            followersOfFollowings = followersOfFollowings.Where(x => !x.IsFollowedByMe).ToList();

                                        followersOfFollowings = AlreadyInteractedUser(followersOfFollowings);
                                        followersOfFollowings = FilterBlackListUser(TemplateModel, followersOfFollowings);
                                        StartProcessForListOfUsers(queryInfo, ref jobProcessResult, followersOfFollowings);
                                        scrollInner = 1;
                                        isScrollInner = true;
                                    }
                                    while (followersOfFollowings.Count > 0);
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
                                    BrowserManager.CloseLast();
                                }
                            }
                        }
                        while (followings.Count > 0);
                    }
                    else
                        while (jobProcessResult.HasNoResult == false)
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            _followingUsers = PinFunction.GetUserFollowings(queryInfo.QueryValue, JobProcess.DominatorAccountModel, jobProcessResult.maxId);

                            if (_followingUsers == null || !_followingUsers.Success || _followingUsers.UsersList.Count == 0)
                            {
                                GlobusLogHelper.log.Info(Log.NoMoreDataToPerform, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    JobProcess.DominatorAccountModel.UserName, ActivityType);
                                jobProcessResult.HasNoResult = true;
                                jobProcessResult.maxId = null;
                            }
                            else
                            {
                                foreach (PinterestUser pinterestUser in _followingUsers.UsersList)
                                {
                                    #region Inner loop mentioning Followers of Followers Opertaion
                                    try
                                    {
                                        while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                                        {
                                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                            // Get Followers of Followers Users from intermediate users
                                            FollowerAndFollowingPtResponseHandler followerOfFollowersUsers = PinFunction.
                                                GetUserFollowers(pinterestUser.Username, JobProcess.DominatorAccountModel, jobProcessResult.maxId);

                                            if (followerOfFollowersUsers.Success)
                                            {
                                                List<PinterestUser> listOfUsers = new List<PinterestUser>();

                                                if (ActivityType == ActivityType.Follow)
                                                    listOfUsers = followerOfFollowersUsers.UsersList.Where(x => !x.IsFollowedByMe).ToList();
                                                else
                                                    listOfUsers = followerOfFollowersUsers.UsersList;

                                                listOfUsers = AlreadyInteractedUser(listOfUsers);
                                                listOfUsers = FilterBlackListUser(TemplateModel, new List<PinterestUser>(listOfUsers));

                                                GlobusLogHelper.log.Info(Log.FoundXResults, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                                   JobProcess.DominatorAccountModel.AccountBaseModel.UserName, listOfUsers.Count,
                                                   queryInfo.QueryType, queryInfo.QueryValue, ActivityType);

                                                StartProcessForListOfUsers(queryInfo, ref jobProcessResult, listOfUsers);
                                                jobProcessResult.maxId = followerOfFollowersUsers.BookMark;
                                                if (followerOfFollowersUsers.HasMoreResults == false)
                                                    jobProcessResult.HasNoResult = true;
                                            }
                                            else
                                            {
                                                jobProcessResult.HasNoResult = true;
                                                jobProcessResult.maxId = null;
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        ex.DebugLog();
                                    }
                                    #endregion
                                }
                                jobProcessResult.maxId = _followingUsers.BookMark;
                                if (_followingUsers.HasMoreResults == false)
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
            }
        }

        private List<PinterestUser> GetUserFollowings(ref int maxPagination, ref int currentPagination, ref JobProcessResult jobProcessResult,ref QueryInfo queryInfo)
        {
            var users = new List<PinterestUser>();
            while (maxPagination >= currentPagination && jobProcessResult.HasNoResult == false)
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                _followingUsers = PinFunction.GetUserFollowings(queryInfo.QueryValue, JobProcess.DominatorAccountModel,
                    jobProcessResult.maxId);
                System.Threading.Thread.Sleep(new Random().Next(3, 5));
                if (_followingUsers == null || !_followingUsers.Success)
                {
                    jobProcessResult.HasNoResult = true;
                    jobProcessResult.maxId = null;
                }
                else
                {
                    foreach (PinterestUser pinterestUser in _followingUsers.UsersList)
                    {
                        #region Inner loop mentioning Followers of Followers Opertaion

                        try
                        {
                            while (maxPagination >= currentPagination && jobProcessResult.HasNoResult == false)
                            {
                                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                // Get Followers of Followers Users from intermediate users
                                FollowerAndFollowingPtResponseHandler followerOfFollowingsUsers = PinFunction.
                                                    GetUserFollowers(pinterestUser.Username, JobProcess.DominatorAccountModel,
                                        jobProcessResult.maxId);
                                System.Threading.Thread.Sleep(new Random().Next(3, 5));
                                if (followerOfFollowingsUsers != null && followerOfFollowingsUsers.Success)
                                {
                                    jobProcessResult.maxId = followerOfFollowingsUsers.BookMark;
                                    if (ActivityType == DominatorHouseCore.Enums.ActivityType.Follow)
                                    {
                                        users.AddRange(followerOfFollowingsUsers.UsersList
                                            .Where(x => !x.IsFollowedByMe).ToList());
                                    }
                                    else
                                    {
                                        users.AddRange(followerOfFollowingsUsers.UsersList
                                            .ToList());
                                    }

                                    if (followerOfFollowingsUsers.HasMoreResults == false)
                                        jobProcessResult.HasNoResult = true;
                                    currentPagination++;
                                }
                                else
                                {
                                    jobProcessResult.HasNoResult = true;
                                    jobProcessResult.maxId = _followingUsers.BookMark;
                                }
                            }
                            jobProcessResult.HasNoResult = false;
                            jobProcessResult.maxId = null;
                            if (maxPagination <= currentPagination)
                                break;
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                        #endregion
                    }

                    if (_followingUsers.HasMoreResults == false)
                    {
                        jobProcessResult.HasNoResult = true;
                        jobProcessResult.maxId = null;
                    }
                    else
                        jobProcessResult.maxId = _followingUsers.BookMark;
                    if (_followingUsers.HasMoreResults == false)
                        jobProcessResult.HasNoResult = true;
                }
            }
            return users;
        }
    }
}