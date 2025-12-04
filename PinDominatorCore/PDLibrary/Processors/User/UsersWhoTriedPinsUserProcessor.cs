using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.PdTables.Accounts;
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
    public class UsersWhoTriedPinsUserProcessor : BasePinterestUserProcessor
    {
        private PinLikersResponseHandler _pinLikers;
        public UsersWhoTriedPinsUserProcessor(IPdJobProcess jobProcess, IDbGlobalService globalService,
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
                    var alreadyScraped = new List<InteractedUsers>();
                    alreadyScraped = DbAccountService
                        .GetInteractedUsersWithSameQuery(ActivityType + "_Scrap", queryInfo).ToList();
                    if (alreadyScraped.Count == 0)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                         JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                         String.Format("LangKeyWaitForSomeTimeWhileScrappingMessage".FromResourceDictionary(), ActivityType));
                        if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                            listOfScrapedUsers = BrowserManager.GetUsersWhoTriedPin(JobProcess.DominatorAccountModel, queryInfo.QueryValue,
                                JobProcess.JobCancellationTokenSource, scroll: maxPagination);
                        else
                            while (maxPagination >= currentPagination && jobProcessResult.HasNoResult == false)
                            {
                                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                _pinLikers = PinFunction.GetUsersWhoTriedPin(queryInfo.QueryValue, 
                                             JobProcess.DominatorAccountModel, jobProcessResult.maxId);
                                System.Threading.Thread.Sleep(new Random().Next(3, 5));
                                if (_pinLikers == null || !_pinLikers.Success)
                                {
                                    jobProcessResult.HasNoResult = true;
                                    jobProcessResult.maxId = null;
                                }
                                else
                                {
                                    if (_pinLikers.HasMoreResult == false)
                                    {
                                        jobProcessResult.HasNoResult = true;
                                        jobProcessResult.maxId = null;
                                    }
                                    else
                                        jobProcessResult.maxId = _pinLikers.BookMark;
                                    if (ActivityType == DominatorHouseCore.Enums.ActivityType.Follow)
                                        listOfScrapedUsers.AddRange(_pinLikers.UserList.Where(x => !x.IsFollowedByMe).ToList());
                                    else
                                        listOfScrapedUsers.AddRange(_pinLikers.UserList.ToList());
                                    if (_pinLikers.HasMoreResult == false)
                                        jobProcessResult.HasNoResult = true;
                                    currentPagination++;
                                }
                            }
                        var dataToBeAdded = new List<InteractedUsers>();
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
                    var lstOfUsers = new List<PinterestUser>();
                    if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        var scroll = 0;
                        var isScroll = false;
                        do
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            lstOfUsers = BrowserManager.GetUsersWhoTriedPin(JobProcess.DominatorAccountModel,
                                    queryInfo.QueryValue, JobProcess.JobCancellationTokenSource,
                                    isScroll, scroll);
                            scroll = 1;
                            isScroll = true;
                            lstOfUsers = FilterBlackListUser(TemplateModel, lstOfUsers);
                            lstOfUsers = AlreadyInteractedUser(lstOfUsers);
                            StartProcessForListOfUsers(queryInfo, ref jobProcessResult, lstOfUsers);
                        }
                        while (lstOfUsers.Count > 0);
                    }
                    else
                        while (jobProcessResult.HasNoResult == false)
                        {
                            _pinLikers = PinFunction.GetUsersWhoTriedPin(queryInfo.QueryValue, JobProcess.DominatorAccountModel, jobProcessResult.maxId);
                            if (_pinLikers == null || !_pinLikers.Success || _pinLikers.UserList.Count == 0)
                            {
                                GlobusLogHelper.log.Info(Log.NoMoreDataToPerform, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    JobProcess.DominatorAccountModel.UserName, ActivityType);

                                jobProcessResult.HasNoResult = true;
                                jobProcessResult.maxId = null;
                            }
                            else
                            {
                                List<PinterestUser> listOfUsers = new List<PinterestUser>(_pinLikers.UserList);
                                jobProcessResult.maxId = _pinLikers.BookMark;

                                listOfUsers = AlreadyInteractedUser(listOfUsers);
                                listOfUsers = FilterBlackListUser(TemplateModel, listOfUsers);
                                StartProcessForListOfUsers(queryInfo, ref jobProcessResult, listOfUsers);
                                if (_pinLikers.HasMoreResult == false)
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