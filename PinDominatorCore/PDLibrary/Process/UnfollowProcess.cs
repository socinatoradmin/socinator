using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.PdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using PinDominatorCore.PDFactories;
using PinDominatorCore.PDLibrary.DAL;
using PinDominatorCore.PDModel;
using PinDominatorCore.Request;
using PinDominatorCore.Response;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace PinDominatorCore.PDLibrary.Process
{
    internal class UnfollowProcess : PdJobProcessInteracted<UnfollowedUsers>
    {
        public UnfollowerModel UnfollowerModel { get; set; }
        private int _activityFailedCount = 1;

        public UnfollowProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, 
            IPdQueryScraperFactory queryScraperFactory, IPdHttpHelper pdhttpHelper, IPdLogInProcess pdLogInProcess)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                pdhttpHelper, pdLogInProcess)
        {
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            UnfollowerModel = JsonConvert.DeserializeObject<UnfollowerModel>(templatesFileManager.Get()
                .FirstOrDefault(x => x.Id == processScopeModel.TemplateId)?.ActivitySettings);
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultUser.Username);

            var jobProcessResult = new JobProcessResult();
            try
            {
                CheckAutoFollowUnfollowProcess(scrapeResult);
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var pinterestUser = (PinterestUser)scrapeResult.ResultUser;
                FriendshipsResponse response = null;
                if (DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    response = PdLogInProcess.BrowserManager.UnfollowUser(DominatorAccountModel, pinterestUser.Username,
                        JobCancellationTokenSource);
                }
                else
                    response = PinFunct.UnFollowSingleUser(pinterestUser, DominatorAccountModel);
                if (response != null && response.Success)
                {
                    var lstFollowers = DbAccountService.Get<Friendships>(x => x.FollowType == FollowType.FollowingBack);

                    if (UnfollowerModel.IsChkAddToBlackList)
                    {
                        DbBlackListOperations =
                            new DbOperations(
                                DataBaseConnectionGlb.GetSqlConnection(SocialNetworks, UserType.BlackListedUser));

                        if (UnfollowerModel.IsChkAddToPrivateBlackList)
                        {
                            DbAccountService.Add(
                                new PrivateBlacklist
                                {
                                    UserName = pinterestUser.Username,
                                    InteractionTimeStamp = DateTime.Now.GetCurrentEpochTime()
                                });
                            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest,
                                DominatorAccountModel.AccountBaseModel.UserName,
                                ActivityType, string.Format("LangKeySuccessfullyAddedToSomething".FromResourceDictionary(), pinterestUser.Username, "LangKeyPrivateBlacklist".FromResourceDictionary()));
                        }

                        if (UnfollowerModel.IsChkAddToGroupBlackList)
                        {
                            DbBlackListOperations.Add(
                                new BlackListUser
                                {
                                    UserName = pinterestUser.Username,
                                    AddedDateTime = DateTime.Now
                                });
                            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest,
                                DominatorAccountModel.AccountBaseModel.UserName,
                                ActivityType, string.Format("LangKeySuccessfullyAddedToSomething".FromResourceDictionary(), pinterestUser.Username, "LangKeyGroupBlacklist".FromResourceDictionary()));
                        }
                    }
                    foreach (var user in lstFollowers)
                        if (user.Username == pinterestUser.Username)
                        {
                            pinterestUser.FollowedBack = 1;
                            break;
                        }

                    scrapeResult.ResultUser = pinterestUser;

                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultUser.Username);

                    IncrementCounters();

                    AddUnFollowedDataToDataBase(scrapeResult);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, response?.Issue?.Message);

                    //Reschedule if action block
                    if (UnfollowerModel.IsChkStopActivityAfterXXFailed && _activityFailedCount++ == UnfollowerModel.ActivityFailedCount)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            $"{"LangKeyStopActivityAfter".FromResourceDictionary()} {_activityFailedCount - 1} " +
                            $"{"LangKeyFailedAttemptFor".FromResourceDictionary()} {UnfollowerModel.FailedActivityReschedule} " +
                            $"{"LangKeyHour".FromResourceDictionary()}");

                        StopAndRescheduleJob(UnfollowerModel.FailedActivityReschedule);
                    }
                    jobProcessResult.IsProcessSuceessfull = false;
                }
                DelayBeforeNextActivity();
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return jobProcessResult;
        }

        public void CheckAutoFollowUnfollowProcess(ScrapeResultNew scrapeResult)
        {
            try
            {
                UserNameInfoPtResponseHandler currentUserInfo = null;
                if (UnfollowerModel.IsChkEnableAutoFollowUnfollowChecked &&
                    (UnfollowerModel.IsChkStartFollowToolStopUnFollow ||
                  UnfollowerModel.IsChkStopUnFollowTool))
                    if (DominatorAccountModel.IsRunProcessThroughBrowser)
                        currentUserInfo = PdLogInProcess.BrowserManager.SearchByCustomUser(DominatorAccountModel,
                                                                                            DominatorAccountModel.AccountBaseModel.ProfileId,
                                                                                            JobCancellationTokenSource);
                    else
                        currentUserInfo = PinFunct.GetUserDetails(DominatorAccountModel.AccountBaseModel.ProfileId, DominatorAccountModel).Result;

                #region Process for auto Follow and Unfollow

                if (UnfollowerModel.IsChkEnableAutoFollowUnfollowChecked &&
                    UnfollowerModel.IsChkStartFollowToolStopUnFollow)
                {
                    if (UnfollowerModel.IsChkStopUnFollowToolWhenReachChecked)
                    {
                        if (currentUserInfo != null)
                        {
                            var followingCount = currentUserInfo.FollowingCount;
                            if (UnfollowerModel.IsChkStopUnFollowToolWhenReachChecked &&
                                followingCount >= UnfollowerModel.StopUnFollowToolWhenReachValue.StartValue &&
                                followingCount <= UnfollowerModel.StopUnFollowToolWhenReachValue.EndValue)
                                StartFollow();
                        }
                    }

                    if (UnfollowerModel.IsChkWhenFollowerFollowingsGreater)
                    {
                        if (currentUserInfo != null)
                        {
                            var followFollwingRatio = currentUserInfo.FollowerCount / currentUserInfo.FollowingCount;
                            if (UnfollowerModel.IsChkWhenFollowerFollowingsGreater &&
                                UnfollowerModel.FollowerFollowingsGreaterThanValue < followFollwingRatio)
                                StartFollow();
                        }
                    }
                }

                if (UnfollowerModel.IsChkEnableAutoFollowUnfollowChecked && UnfollowerModel.IsChkStopUnFollowTool)
                {
                    if (UnfollowerModel.IsChkStopUnFollowToolWhenReachChecked)
                    {
                        if (currentUserInfo != null)
                        {
                            var followingCount = currentUserInfo.FollowingCount;
                            if (UnfollowerModel.IsChkStopUnFollowToolWhenReachChecked &&
                                followingCount >= UnfollowerModel.StopUnFollowToolWhenReachValue.StartValue &&
                                followingCount <= UnfollowerModel.StopUnFollowToolWhenReachValue.EndValue)
                                StopUnfollowTool();
                        }
                    }

                    if (UnfollowerModel.IsChkWhenFollowerFollowingsGreater)
                    {
                        if (currentUserInfo != null)
                        {
                            var followFollwingRatio = currentUserInfo.FollowerCount / currentUserInfo.FollowingCount;
                            if (UnfollowerModel.IsChkWhenFollowerFollowingsGreater &&
                                UnfollowerModel.FollowerFollowingsGreaterThanValue < followFollwingRatio)
                                StopUnfollowTool();
                        }
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void StartFollow()
        {
            try
            {
                var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                dominatorScheduler.EnableDisableModules(ActivityType.Unfollow, ActivityType.Follow,
                    DominatorAccountModel.AccountId);
            }
            catch (InvalidOperationException ex)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType,
                    ex.Message.Contains("1001")
                        ? "LangKeyUnfollowActivityHasMetAutoEnableConfigurationForFollowMessage".FromResourceDictionary()
                        : "");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void StopUnfollowTool()
        {
            try
            {
                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleConfiguration =
                    jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];
                var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                dominatorScheduler.StopActivity(DominatorAccountModel, "Unfollow", moduleConfiguration.TemplateId,
                    false);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AddUnFollowedDataToDataBase(ScrapeResultNew scrapeResult)
        {
            try
            {
                IDbAccountService dbAccountService = new DbAccountService(DominatorAccountModel);
                IDbCampaignService dbCampaignService = new DbCampaignService(CampaignId);
                var user = (PinterestUser)scrapeResult.ResultUser;
                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleSetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];
                if (moduleSetting == null)
                    return;
                if (moduleSetting.IsTemplateMadeByCampaignMode)
                    dbCampaignService.Add(new DominatorHouseCore.DatabaseHandler.PdTables.Campaigns.UnfollowedUsers
                    {
                        InteractionDate = DateTimeUtilities.GetEpochTime(),
                        FullName = user.FullName,
                        Username = user.Username,
                        UserId = user.UserId,
                        FollowedBack = user.FollowedBack,
                        OperationType = ActivityType.Unfollow,
                        SinAccId = DominatorAccountModel.AccountBaseModel.UserId,
                        SinAccUsername = DominatorAccountModel.AccountBaseModel.UserName
                    });
                dbAccountService.Add(new UnfollowedUsers
                {
                    InteractionDate = DateTimeUtilities.GetEpochTime(),
                    FullName = user.FullName,
                    OperationType = ActivityType.Unfollow,
                    Username = user.Username,
                    UserId = user.UserId,
                    FollowedBack = user.FollowedBack
                });

                var existed =
                    DbAccountService.GetSingle<Friendships>(x => x.Username == scrapeResult.ResultUser.Username);
                if (existed != null)
                {
                    if (existed.FollowType == FollowType.Mutual)
                    {
                        existed.FollowType = FollowType.FollowingBack;
                        DbAccountService.Update(existed);
                    }
                    else if (existed.FollowType == FollowType.Following)
                    {
                        Expression<Func<Friendships, bool>> expression = x => x.Username == existed.Username;
                        DbAccountService.Remove(expression);
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