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
    public class SomeonesFollowersUserProcessor : BasePinterestUserProcessor
    {
        private FollowerAndFollowingPtResponseHandler _followerUsers;
        public SomeonesFollowersUserProcessor(IPdJobProcess jobProcess, 
            IDbGlobalService globalService, IDbCampaignService campaignService,
            IPinFunction objPinFunct, IDelayService delayService) :
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
                            listOfScrapedUsers = BrowserManager.GetUserFollowers(JobProcess.DominatorAccountModel, queryInfo.QueryValue,
                                JobProcess.JobCancellationTokenSource, scroll: maxPagination);
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
                                    if (_followerUsers.HasMoreResults == false)
                                    {
                                        jobProcessResult.HasNoResult = true;
                                        jobProcessResult.maxId = null;
                                    }
                                    else
                                        jobProcessResult.maxId = _followerUsers.BookMark;
                                    if (ActivityType == ActivityType.Follow)
                                        listOfScrapedUsers.AddRange(_followerUsers.UsersList.Where(x => !x.IsFollowedByMe).ToList());
                                    else
                                        listOfScrapedUsers.AddRange(_followerUsers.UsersList.ToList());
                                    if (_followerUsers.HasMoreResults == false)
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
                    List<PinterestUser> listOfUsers;
                    if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        var scroll = 0;
                       var isScroll = false;
                       do
                       {
                           JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                           var listOfUsersTemp = BrowserManager.GetUserFollowers(JobProcess.DominatorAccountModel,
                                   queryInfo.QueryValue, JobProcess.JobCancellationTokenSource, isScroll, scroll);
                            scroll = 1;
                           isScroll = true;
                           listOfUsers = ActivityType == ActivityType.Follow ? listOfUsersTemp.Where(x => !x.IsFollowedByMe).ToList() : listOfUsersTemp;
                          listOfUsers = FilterBlackListUser(TemplateModel, listOfUsers);
                           listOfUsers = AlreadyInteractedUser(listOfUsers);

                          GlobusLogHelper.log.Info(Log.FoundXResults, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                                  JobProcess.DominatorAccountModel.AccountBaseModel.UserName, listOfUsers.Count,
                                                  queryInfo.QueryType, queryInfo.QueryValue, ActivityType);
                          System.Threading.Thread.Sleep(10000);
                          StartProcessForListOfUsers(queryInfo, ref jobProcessResult, listOfUsers);
                      }
                       while (listOfUsers.Count > 0);
                    }
                    //else
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
                                if (ActivityType == ActivityType.Follow)
                                    listOfUsers = _followerUsers.UsersList.Where(x => !x.IsFollowedByMe).ToList();
                                else
                                    listOfUsers = _followerUsers.UsersList;

                                listOfUsers = FilterBlackListUser(TemplateModel, listOfUsers);
                                listOfUsers = AlreadyInteractedUser(listOfUsers);

                                GlobusLogHelper.log.Info(Log.FoundXResults, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                                  JobProcess.DominatorAccountModel.AccountBaseModel.UserName, listOfUsers.Count,
                                                  queryInfo.QueryType, queryInfo.QueryValue, ActivityType);

                                StartProcessForListOfUsers(queryInfo, ref jobProcessResult, listOfUsers);
                                jobProcessResult.maxId = _followerUsers.BookMark;
                                if (_followerUsers.HasMoreResults == false)
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