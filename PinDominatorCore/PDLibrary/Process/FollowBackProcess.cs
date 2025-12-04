using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.PdTables.Accounts;
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

namespace PinDominatorCore.PDLibrary.Process
{
    public class FollowBackProcess : PdJobProcessInteracted<InteractedUsers>
    {
        private readonly IDbCampaignService _dbCampaignService;
        private int _activityFailedCount = 1;
        private FollowBackModel FollowBackModel { get; }

        public FollowBackProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService, IDbCampaignService dbCampaignService,
            IExecutionLimitsManager executionLimitsManager,
            IPdQueryScraperFactory queryScraperFactory, IPdHttpHelper pdhttpHelper, IPdLogInProcess pdLogInProcess)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                pdhttpHelper, pdLogInProcess)
        {
            _dbCampaignService = dbCampaignService;
            FollowBackModel = JsonConvert.DeserializeObject<FollowBackModel>(TemplateModel.ActivitySettings);
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultUser.Username);

            var jobProcessResult = new JobProcessResult();
            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                PinterestUser pinterestUser = (PinterestUser)scrapeResult.ResultUser;
                FriendshipsResponse response = null;
                if (DominatorAccountModel.IsRunProcessThroughBrowser)
                    response = PdLogInProcess.BrowserManager.FollowUser(DominatorAccountModel, pinterestUser.Username,
                        JobCancellationTokenSource);
                else
                    response = PinFunct.FollowUserSingle(DominatorAccountModel, pinterestUser.Username);

                if (response != null && response.Success)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultUser.Username);
                    IncrementCounters();
                    AddFollowedBackDataToDataBase(scrapeResult);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, response?.Issue?.Message);

                    //Reschedule if action block
                    if (FollowBackModel.IsChkStopActivityAfterXXFailed && _activityFailedCount++ == FollowBackModel.ActivityFailedCount)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            $"{"LangKeyStopActivityAfter".FromResourceDictionary()} {_activityFailedCount - 1} " +
                            $"{"LangKeyFailedAttemptFor".FromResourceDictionary()} {FollowBackModel.FailedActivityReschedule} " +
                            $"{"LangKeyHour".FromResourceDictionary()}");

                        StopAndRescheduleJob(FollowBackModel.FailedActivityReschedule);
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

        private void AddFollowedBackDataToDataBase(ScrapeResultNew scrapeResult)
        {
            try
            {
                var user = (PinterestUser)scrapeResult.ResultUser;
                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleSetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];
                if (moduleSetting == null)
                    return;
                if (moduleSetting.IsTemplateMadeByCampaignMode)
                    _dbCampaignService.Add(new DominatorHouseCore.DatabaseHandler.PdTables.Campaigns.InteractedUsers
                    {
                        ActivityType = ActivityType.ToString(),
                        InteractionTime = DateTimeUtilities.GetEpochTime(),
                        Bio = user.UserBio,
                        FollowersCount = user.FollowersCount,
                        FollowingsCount = user.FollowingsCount,
                        FullName = user.FullName,
                        HasAnonymousProfilePicture = user.HasProfilePic,
                        InteractedUserId = user.UserId,
                        InteractedUsername = user.Username,
                        IsVerified = user.IsVerified,
                        PinsCount = user.PinsCount,
                        ProfilePicUrl = user.ProfilePicUrl,
                        TriesCount = user.TriesCount,
                        Website = user.WebsiteUrl,
                        SinAccId = DominatorAccountModel.AccountBaseModel.UserId,
                        SinAccUsername = DominatorAccountModel.AccountBaseModel.UserName
                    });
                DbAccountService.Add(new InteractedUsers
                {
                    ActivityType = ActivityType.ToString(),
                    InteractionTime = DateTimeUtilities.GetEpochTime(),
                    Bio = user.UserBio,
                    FollowersCount = user.FollowersCount,
                    FollowingsCount = user.FollowingsCount,
                    FullName = user.FullName,
                    HasAnonymousProfilePicture = user.HasProfilePic,
                    InteractedUserId = user.UserId,
                    InteractedUsername = user.Username,
                    IsVerified = user.IsVerified,
                    PinsCount = user.PinsCount,
                    ProfilePicUrl = user.ProfilePicUrl,
                    TriesCount = user.TriesCount,
                    Website = user.WebsiteUrl,
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}