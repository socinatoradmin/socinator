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
    public class FollowBackProcessor : BasePinterestUserProcessor
    {
        private readonly IDelayService _delayService;
        private FollowerAndFollowingPtResponseHandler _followerAndFollowingPtResponseHandler;
        public FollowBackProcessor(IPdJobProcess jobProcess, IDbGlobalService globalService,
            IDbCampaignService campaignService, IPinFunction objPinFunct, IDelayService delayService) :
            base(jobProcess, globalService, campaignService, objPinFunct, delayService)
        {
            _delayService = delayService;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                var userUrl = JobProcess.DominatorAccountModel.AccountBaseModel.ProfileId;
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                var pinConfig =
                    genericFileManager.GetModel<PinterestOtherConfigModel>(ConstantVariable
                        .GetOtherPinterestSettingsFile());

                GlobusLogHelper.log.Info(Log.CustomMessage,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                    "LangKeySearchingForPeopleToFollowBack".FromResourceDictionary());

                // Here we are scraping data until maximum pagination given by user and that data we will store into
                // database for further use then we will send the data to perform activity.
                if (pinConfig.IsScrapDataBeforeSendToPerformActivity)
                {
                    var maxPagination = pinConfig.PaginationCount;
                    var currentPagination = 0;
                    var listOfScrapedUsers = new List<PinterestUser>();
                    var alreadyScraped = new List<InteractedUsers>();
                    alreadyScraped = DbAccountService.GetInteractedUsers(ActivityType + "_Scrap").ToList();

                    if (alreadyScraped.Count == 0)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                         JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                         $"Please wait for some time while scrapping data to perform Activity => {ActivityType}");

                        if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                        {
                            var lstOfUsers = BrowserManager.GetUserFollowers(JobProcess.DominatorAccountModel, userUrl,
                                JobProcess.JobCancellationTokenSource);
                            if (ActivityType == DominatorHouseCore.Enums.ActivityType.Follow)
                                listOfScrapedUsers.AddRange(lstOfUsers.Where(x => !x.IsFollowedByMe).ToList());
                            else
                                listOfScrapedUsers.AddRange(lstOfUsers);
                        }
                        else
                            while (maxPagination >= currentPagination && jobProcessResult.HasNoResult == false)
                            {
                                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                _followerAndFollowingPtResponseHandler = PinFunction.GetUserFollowers(userUrl, JobProcess.DominatorAccountModel,
                                    jobProcessResult.maxId);
                                _delayService.ThreadSleep(new Random().Next(3, 5));
                                if (_followerAndFollowingPtResponseHandler == null || !_followerAndFollowingPtResponseHandler.Success)
                                {
                                    jobProcessResult.HasNoResult = true;
                                    jobProcessResult.maxId = null;
                                }
                                else
                                {
                                    if (_followerAndFollowingPtResponseHandler.HasMoreResults == false)
                                    {
                                        jobProcessResult.HasNoResult = true;
                                        jobProcessResult.maxId = null;
                                    }
                                    else
                                        jobProcessResult.maxId = _followerAndFollowingPtResponseHandler.BookMark;
                                    listOfScrapedUsers.AddRange(_followerAndFollowingPtResponseHandler.UsersList.ToList());
                                    if (_followerAndFollowingPtResponseHandler.HasMoreResults == false)
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

                    jobProcessResult.HasNoResult = true;
                }
                else
                {
                    var lstPinterestUsers = new List<PinterestUser>();
                    while (jobProcessResult.HasNoResult == false)
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            _followerAndFollowingPtResponseHandler = PinFunction.GetUserFollowers(userUrl, JobProcess.DominatorAccountModel,
                                jobProcessResult.maxId);
                            if (_followerAndFollowingPtResponseHandler == null || !_followerAndFollowingPtResponseHandler.Success)
                            {
                                jobProcessResult.HasNoResult = true;
                                jobProcessResult.maxId = null;
                            }
                            else
                            {
                                lstPinterestUsers = _followerAndFollowingPtResponseHandler.UsersList.Where(x => !x.IsFollowedByMe).ToList();
                                lstPinterestUsers = AlreadyInteractedUser(lstPinterestUsers);
                                lstPinterestUsers = FilterBlackListUser(TemplateModel, lstPinterestUsers);
                                StartProcessForListOfUsers(queryInfo, ref jobProcessResult, lstPinterestUsers);

                                jobProcessResult.maxId = _followerAndFollowingPtResponseHandler.BookMark;
                                if (_followerAndFollowingPtResponseHandler.HasMoreResults == false)
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