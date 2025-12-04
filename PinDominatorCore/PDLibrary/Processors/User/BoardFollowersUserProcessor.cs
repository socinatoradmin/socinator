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
    public class BoardFollowersUserProcessor : BasePinterestUserProcessor
    {
        private readonly IDelayService _delayService;
        private FollowerAndFollowingPtResponseHandler _followerUsers;
        public BoardFollowersUserProcessor(IPdJobProcess jobProcess, IDbGlobalService globalService,
            IDbCampaignService campaignService, IPinFunction objPinFunct, IDelayService delayService) :
            base(jobProcess, globalService, campaignService, objPinFunct, delayService)
        {
            _delayService = delayService;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                var pinConfig =
                    genericFileManager.GetModel<PinterestOtherConfigModel>(ConstantVariable.GetOtherPinterestSettingsFile());
                var arrUser = queryInfo.QueryValue.Split('/');
                if (arrUser.Length >= 5 && string.IsNullOrEmpty(arrUser[4]) || arrUser.Length < 5)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        String.Format("LangKeyCheckSomeUrlSeemsIncorrect".FromResourceDictionary(), "LangKeyBoard".FromResourceDictionary()));
                    jobProcessResult.IsProcessCompleted = true;
                    return;
                }

                // Here we are scraping data until maximum pagination given by user and that data we will store into
                // database for further use then we will send the data to perform activity.
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
                            var lstOfUsers = BrowserManager.GetBoardFollowers(JobProcess.DominatorAccountModel,
                                  queryInfo.QueryValue, JobProcess.JobCancellationTokenSource, scroll: maxPagination);
                            if (ActivityType == DominatorHouseCore.Enums.ActivityType.Follow)
                                listOfScrapedUsers.AddRange(lstOfUsers.Where(x => !x.IsFollowedByMe).ToList());
                            else
                                listOfScrapedUsers.AddRange(lstOfUsers);
                        }
                        else
                            while (maxPagination >= currentPagination && jobProcessResult.HasNoResult == false)
                            {
                                _followerUsers = PinFunction.GetBoardFollowers(queryInfo.QueryValue, JobProcess.DominatorAccountModel,
                                    jobProcessResult.maxId);
                                _delayService.ThreadSleep(new Random().Next(3, 5));
                                if (listOfScrapedUsers.Count == 0 && (_followerUsers == null || !_followerUsers.Success))
                                {
                                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                    jobProcessResult.HasNoResult = true;
                                    jobProcessResult.maxId = null;
                                }
                                else
                                {
                                    if (_followerUsers.HasMoreResults == false)
                                    {
                                        jobProcessResult.HasNoResult = true;
                                        jobProcessResult.maxId = null;
                                    }
                                    else
                                        jobProcessResult.maxId = _followerUsers.BookMark;
                                    if (ActivityType == DominatorHouseCore.Enums.ActivityType.Follow)
                                        listOfScrapedUsers.AddRange(_followerUsers.UsersList.Where(x => !x.IsFollowedByMe).ToList());
                                    else
                                        listOfScrapedUsers.AddRange(_followerUsers.UsersList.ToList());
                                    if (_followerUsers.HasMoreResults == false)
                                        jobProcessResult.HasNoResult = true;
                                    currentPagination++;
                                }
                            }
                        List<InteractedUsers> dataToBeAdded = new List<InteractedUsers>();
                        foreach (PinterestUser user in listOfScrapedUsers)
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
                        var pinterestUser = new PinterestUser
                        {
                            UserBio = user.Bio,
                            FollowersCount = user.FollowersCount,
                            FollowingsCount = user.FollowingsCount,
                            FullName = user.FullName,
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
                        if (user.HasAnonymousProfilePicture != null)
                            pinterestUser.HasProfilePic = user.HasAnonymousProfilePicture.Value;
                        listOfUsers.Add(pinterestUser);
                    }

                    listOfUsers = AlreadyInteractedUser(listOfUsers);
                    listOfUsers = FilterBlackListUser(TemplateModel, listOfUsers);
                    StartProcessForListOfUsers(queryInfo, ref jobProcessResult, listOfUsers);
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
                            var listOfUsers = BrowserManager.GetBoardFollowers(JobProcess.DominatorAccountModel,
                                    queryInfo.QueryValue, JobProcess.JobCancellationTokenSource,
                                    isScroll, scroll);
                            scroll = 1;
                            isScroll = true;
                            if (ActivityType == ActivityType.Follow)
                                lstOfUsers.AddRange(listOfUsers.Where(x => !x.IsFollowedByMe).ToList());
                            else
                                lstOfUsers.AddRange(listOfUsers);
                            lstOfUsers = FilterBlackListUser(TemplateModel, lstOfUsers);
                            lstOfUsers = AlreadyInteractedUser(lstOfUsers);
                            StartProcessForListOfUsers(queryInfo, ref jobProcessResult, lstOfUsers);
                            lstOfUsers.Clear();
                        }
                        while (lstOfUsers.Count > 0);
                    }
                    else
                        while (jobProcessResult.HasNoResult == false)
                        {
                            _followerUsers = PinFunction.GetBoardFollowers(queryInfo.QueryValue,
                                JobProcess.DominatorAccountModel, jobProcessResult.maxId);
                            if (_followerUsers == null || !_followerUsers.Success || _followerUsers.UsersList.Count == 0)
                            {
                                GlobusLogHelper.log.Info(Log.NoMoreDataToPerform, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    JobProcess.DominatorAccountModel.UserName, ActivityType);
                                jobProcessResult.HasNoResult = true;
                                jobProcessResult.maxId = null;
                            }
                            else
                            {
                                List<PinterestUser> listOfUsers;

                                if (ActivityType == ActivityType.Follow)
                                    listOfUsers = _followerUsers.UsersList.Where(x => !x.IsFollowedByMe).ToList();
                                else
                                    listOfUsers = _followerUsers.UsersList;
                                listOfUsers = AlreadyInteractedUser(listOfUsers);
                                listOfUsers = FilterBlackListUser(TemplateModel, listOfUsers);

                                StartProcessForListOfUsers(queryInfo, ref jobProcessResult, listOfUsers);
                                jobProcessResult.maxId = _followerUsers.BookMark;
                                if (_followerUsers.HasMoreResults == false)
                                    jobProcessResult.HasNoResult = true;
                            }
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