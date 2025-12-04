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
    public class FollowersOfSomeonesFollowersUserProcessor : BasePinterestUserProcessor
    {
        private FollowerAndFollowingPtResponseHandler _followerUsers;

        public FollowersOfSomeonesFollowersUserProcessor(IPdJobProcess jobProcess, IDbGlobalService globalService,
            IDbCampaignService campaignService, IPinFunction objPinFunct, IDelayService delayService) :
            base(jobProcess, globalService, campaignService, objPinFunct, delayService)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                var pinConfig =
                    genericFileManager.GetModel<PinterestOtherConfigModel>(ConstantVariable.GetOtherPinterestSettingsFile());

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (pinConfig.IsScrapDataBeforeSendToPerformActivity)
                {
                    var maxPagination = pinConfig.PaginationCount;
                    var currentPagination = 0;
                    var listOfScrapedUsers = new List<PinterestUser>();
                    var alreadyScraped = new List<InteractedUsers>();

                    alreadyScraped = DbAccountService
                        .GetInteractedUsersWithSameQuery(ActivityType + "_Scrap", queryInfo).ToList();

                    // Here we are scraping data until maximum pagination given by user and that data we will store into
                    // database for further use then we will send the data to perform activity.
                    if (alreadyScraped.Count == 0)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                         JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                         String.Format("LangKeyWaitForSomeTimeWhileScrappingMessage".FromResourceDictionary(), ActivityType));

                        if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                        {
                            while (maxPagination >= currentPagination && jobProcessResult.HasNoResult == false)
                            {
                                bool isScroll = false;
                                int scroll = 0;
                                List<PinterestUser> followers = new List<PinterestUser>();
                                do
                                {
                                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                    followers = BrowserManager.GetUserFollowers(JobProcess.DominatorAccountModel,
                                        queryInfo.QueryValue, JobProcess.JobCancellationTokenSource, isScroll, scroll);
                                    scroll++;
                                    foreach (PinterestUser user in followers)
                                    {
                                        try
                                        {
                                            BrowserManager.AddNew(JobProcess.JobCancellationTokenSource, $"https://{BrowserManager.Domain}");
                                            bool isScrollInner = false;
                                            int scrollInner = 0;
                                            List<PinterestUser> followersOfFollowers = new List<PinterestUser>();
                                            do
                                            {
                                                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                                followersOfFollowers = BrowserManager.GetUserFollowers(JobProcess.DominatorAccountModel,
                                                user.Username, JobProcess.JobCancellationTokenSource, isScrollInner, scrollInner);

                                                if (ActivityType == ActivityType.Follow)
                                                    followersOfFollowers = followersOfFollowers.Where(x => !x.IsFollowedByMe).ToList();

                                                followersOfFollowers = AlreadyInteractedUser(followersOfFollowers);
                                                followersOfFollowers = FilterBlackListUser(TemplateModel, followersOfFollowers);
                                                StartProcessForListOfUsers(queryInfo, ref jobProcessResult, followersOfFollowers);
                                                scrollInner = 1;
                                                isScrollInner = true;
                                            }
                                            while (followersOfFollowers.Count > 0);
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
                                while (followers.Count > 0);
                            }
                        }
                        else
                            while (maxPagination >= currentPagination && jobProcessResult.HasNoResult == false)
                            {
                                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                _followerUsers = PinFunction.GetUserFollowers(queryInfo.QueryValue, JobProcess.DominatorAccountModel,
                                    jobProcessResult.maxId);
                                System.Threading.Thread.Sleep(new Random().Next(3, 5));
                                if (_followerUsers == null || !_followerUsers.Success)
                                {
                                    jobProcessResult.HasNoResult = true;
                                    jobProcessResult.maxId = null;
                                }
                                else
                                {
                                    foreach (PinterestUser pinterestUser in _followerUsers.UsersList)
                                    {
                                        #region Inner loop mentioning Followers of Followers Opertaion

                                        try
                                        {
                                            while (maxPagination >= currentPagination && jobProcessResult.HasNoResult == false)
                                            {
                                                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                                // Get Followers of Followers Users from intermediate users
                                                FollowerAndFollowingPtResponseHandler followerOfFollowersUsers =
                                                    PinFunction.GetUserFollowers(pinterestUser.Username, JobProcess.DominatorAccountModel,
                                                        jobProcessResult.maxId);
                                                System.Threading.Thread.Sleep(new Random().Next(3, 5));
                                                if (followerOfFollowersUsers != null && followerOfFollowersUsers.Success)
                                                {
                                                    jobProcessResult.maxId = followerOfFollowersUsers.BookMark;
                                                    if (ActivityType == DominatorHouseCore.Enums.ActivityType.Follow)
                                                    {
                                                        listOfScrapedUsers.AddRange(followerOfFollowersUsers.UsersList
                                                            .Where(x => !x.IsFollowedByMe).ToList());
                                                    }
                                                    else
                                                    {
                                                        listOfScrapedUsers.AddRange(followerOfFollowersUsers.UsersList
                                                            .ToList());
                                                    }

                                                    if (followerOfFollowersUsers.HasMoreResults == false)
                                                        jobProcessResult.HasNoResult = true;
                                                    currentPagination++;
                                                }
                                                else
                                                {
                                                    jobProcessResult.HasNoResult = true;
                                                    jobProcessResult.maxId = _followerUsers.BookMark;
                                                }
                                            }
                                            jobProcessResult.HasNoResult = false;
                                            jobProcessResult.maxId = null;
                                            if (maxPagination <= currentPagination)
                                                break;
                                        }
                                        catch (OperationCanceledException)
                                        {
                                            throw new OperationCanceledException();
                                        }
                                        catch (Exception ex)
                                        {
                                            ex.DebugLog();
                                        }
                                        #endregion
                                    }

                                    if (_followerUsers.HasMoreResults == false)
                                    {
                                        jobProcessResult.HasNoResult = true;
                                        jobProcessResult.maxId = null;
                                    }
                                    else
                                        jobProcessResult.maxId = _followerUsers.BookMark;
                                    if (_followerUsers.HasMoreResults == false)
                                        jobProcessResult.HasNoResult = true;
                                }
                            }

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
                    }


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
                    listOfUsers = FilterBlackListUser(TemplateModel, listOfUsers);

                    StartProcessForListOfUsers(queryInfo, ref jobProcessResult, listOfUsers);
                }
                else
                {
                    List<PinterestUser> followers = new List<PinterestUser>();
                    if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        bool isScroll = false;
                        int scroll = 0;
                        do
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            followers = BrowserManager.GetUserFollowers(JobProcess.DominatorAccountModel,
                                queryInfo.QueryValue, JobProcess.JobCancellationTokenSource, isScroll, scroll);
                            scroll = 1;
                            foreach (PinterestUser user in followers)
                            {
                                try
                                {
                                    BrowserManager.AddNew(JobProcess.JobCancellationTokenSource, $"https://{BrowserManager.Domain}");
                                    bool isScrollInner = false;
                                    int scrollInner = 0;
                                    List<PinterestUser> followersOfFollowers = new List<PinterestUser>();
                                    do
                                    {
                                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                        followersOfFollowers = BrowserManager.GetUserFollowers(JobProcess.DominatorAccountModel,
                                        user.Username, JobProcess.JobCancellationTokenSource, isScrollInner, scrollInner);

                                        if (ActivityType == ActivityType.Follow)
                                            followersOfFollowers = followersOfFollowers.Where(x => !x.IsFollowedByMe).ToList();

                                        followersOfFollowers = AlreadyInteractedUser(followersOfFollowers);
                                        followersOfFollowers = FilterBlackListUser(TemplateModel, followersOfFollowers);
                                        StartProcessForListOfUsers(queryInfo, ref jobProcessResult, followersOfFollowers);
                                        scrollInner = 1;
                                        isScrollInner = true;
                                    }
                                    while (followersOfFollowers.Count > 0);
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
                        while (followers.Count > 0);
                    }
                    else
                    {
                        while (jobProcessResult.HasNoResult == false)
                        {
                            _followerUsers = PinFunction.GetUserFollowers(queryInfo.QueryValue, JobProcess.DominatorAccountModel, jobProcessResult.maxId);

                            if (_followerUsers == null || !_followerUsers.Success || _followerUsers.UsersList.Count == 0)
                            {
                                GlobusLogHelper.log.Info(Log.NoMoreDataToPerform, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    JobProcess.DominatorAccountModel.UserName, ActivityType);
                                jobProcessResult.HasNoResult = true;
                                jobProcessResult.maxId = null;
                            }
                            else
                            {
                                foreach (PinterestUser pinterestUser in _followerUsers.UsersList)
                                {
                                    #region Inner loop mentioning Followers of Followers Opertaion
                                    try
                                    {
                                        jobProcessResult.HasNoResult = false;
                                        while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                                        {
                                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                            // Get Followers of Followers Users from intermediate users
                                            var followerOfFollowersUsers = PinFunction.
                                                GetUserFollowers(pinterestUser.Username, JobProcess.DominatorAccountModel, jobProcessResult.maxId);

                                            if (followerOfFollowersUsers.Success && followerOfFollowersUsers.UsersList.Count != 0)
                                            {
                                                var LstPinUser = new List<PinterestUser>();

                                                if (ActivityType == ActivityType.Follow)
                                                    LstPinUser = followerOfFollowersUsers.UsersList.Where(x => !x.IsFollowedByMe).ToList();
                                                else
                                                    LstPinUser = followerOfFollowersUsers.UsersList;

                                                LstPinUser = AlreadyInteractedUser(LstPinUser);
                                                LstPinUser = FilterBlackListUser(TemplateModel, LstPinUser);
                                                GlobusLogHelper.log.Info(Log.FoundXResults, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                                   JobProcess.DominatorAccountModel.AccountBaseModel.UserName, LstPinUser.Count,
                                                   queryInfo.QueryType, queryInfo.QueryValue, ActivityType);

                                                StartProcessForListOfUsers(queryInfo, ref jobProcessResult, LstPinUser);

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
                                    catch (OperationCanceledException)
                                    {
                                        throw new OperationCanceledException();
                                    }
                                    catch (Exception ex)
                                    {
                                        ex.DebugLog();
                                    }
                                    #endregion
                                }
                                jobProcessResult.maxId = _followerUsers.BookMark;
                                if (_followerUsers.HasMoreResults == false)
                                    jobProcessResult.HasNoResult = true;
                            }
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
    }
}