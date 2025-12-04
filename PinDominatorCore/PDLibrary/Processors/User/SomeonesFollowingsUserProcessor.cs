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
    public class SomeonesFollowingsUserProcessor : BasePinterestUserProcessor
    {
        private FollowerAndFollowingPtResponseHandler _followingUsers;
        public SomeonesFollowingsUserProcessor(IPdJobProcess jobProcess, IDbGlobalService globalService,
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
                    genericFileManager.GetModel<PinterestOtherConfigModel>(ConstantVariable
                        .GetOtherPinterestSettingsFile());
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                // Here we are scraping data until maximum pagination given by user and that data we will store into
                // database for further use then we will send the data to perform activity.
                if (pinConfig.IsScrapDataBeforeSendToPerformActivity)
                {
                    var maxPagination = pinConfig.PaginationCount;
                    var currentPagination = 0;
                    var listOfScrapedUsers = new List<PinterestUser>();
                    var alreadyScraped = DbAccountService
                        .GetInteractedUsersWithSameQuery(ActivityType + "_Scrap", queryInfo).ToList();
                    if (alreadyScraped.Count == 0)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                         JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                          String.Format("LangKeyWaitForSomeTimeWhileScrappingMessage".FromResourceDictionary(), ActivityType));

                        if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                            listOfScrapedUsers = BrowserManager.GetUserFollowings(JobProcess.DominatorAccountModel, queryInfo.QueryValue,
                                JobProcess.JobCancellationTokenSource, scroll: maxPagination);
                        else
                            while (maxPagination >= currentPagination && jobProcessResult.HasNoResult == false)
                            {
                                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                _followingUsers = PinFunction.GetUserFollowings(queryInfo.QueryValue, JobProcess.DominatorAccountModel, jobProcessResult.maxId);
                                System.Threading.Thread.Sleep(new Random().Next(3, 5));
                                if (_followingUsers == null || !_followingUsers.Success)
                                {
                                    jobProcessResult.HasNoResult = true;
                                    jobProcessResult.maxId = null;
                                }
                                else
                                {
                                    if (_followingUsers.HasMoreResults == false)
                                    {
                                        jobProcessResult.HasNoResult = true;
                                        jobProcessResult.maxId = null;
                                    }
                                    else
                                        jobProcessResult.maxId = _followingUsers.BookMark;
                                    if (ActivityType == ActivityType.Follow)
                                    {
                                        listOfScrapedUsers.AddRange(_followingUsers.UsersList.Where(x => !x.IsFollowedByMe).ToList());
                                    }
                                    else
                                    {
                                        listOfScrapedUsers.AddRange(_followingUsers.UsersList.ToList());
                                    }
                                    if (_followingUsers.HasMoreResults == false)
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
                            var listOfUsersTemp = BrowserManager.GetUserFollowings(JobProcess.DominatorAccountModel,
                                    queryInfo.QueryValue, JobProcess.JobCancellationTokenSource,
                                    isScroll, scroll);
                            scroll = 1;
                            isScroll = true;
                            if (ActivityType == ActivityType.Follow)
                                lstOfUsers = listOfUsersTemp.Where(x => !x.IsFollowedByMe).ToList();
                            else
                                lstOfUsers = listOfUsersTemp;
                            lstOfUsers = FilterBlackListUser(TemplateModel, lstOfUsers);
                            lstOfUsers = AlreadyInteractedUser(lstOfUsers);
                            StartProcessForListOfUsers(queryInfo, ref jobProcessResult, lstOfUsers);
                        }
                        while (lstOfUsers.Count > 0);
                    }
                    else
                        while (jobProcessResult.HasNoResult == false)
                        {
                            _followingUsers = PinFunction.GetUserFollowings(queryInfo.QueryValue, JobProcess.DominatorAccountModel,
                                jobProcessResult.maxId);

                            if (_followingUsers == null || !_followingUsers.Success
                                || _followingUsers.UsersList.Count == 0)
                            {
                                GlobusLogHelper.log.Info(Log.NoMoreDataToPerform, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    JobProcess.DominatorAccountModel.UserName, ActivityType);
                                jobProcessResult.HasNoResult = true;
                                jobProcessResult.maxId = null;
                            }
                            else
                            {
                                if (ActivityType == ActivityType.Follow)
                                    lstOfUsers = _followingUsers.UsersList.Where(x => !x.IsFollowedByMe).ToList();
                                else
                                    lstOfUsers = _followingUsers.UsersList;

                                lstOfUsers = AlreadyInteractedUser(lstOfUsers);
                                lstOfUsers = FilterBlackListUser(TemplateModel, lstOfUsers);
                                StartProcessForListOfUsers(queryInfo, ref jobProcessResult, lstOfUsers);
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
    }
}