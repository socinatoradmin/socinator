using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.PdTables.Accounts;
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
using System;
using System.Linq;
using static PinDominatorCore.PDEnums.Enums;

namespace PinDominatorCore.PDLibrary.Process
{
    internal class UserScraperProcess : PdJobProcessInteracted<InteractedUsers>
    {
        public UserScraperProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, 
            IPdQueryScraperFactory queryScraperFactory, IPdHttpHelper pdhttpHelper, IPdLogInProcess pdLogInProcess)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                pdhttpHelper, pdLogInProcess)
        {
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            UserScraperModel = JsonConvert.DeserializeObject<UserScraperModel>(templatesFileManager.Get()
                .FirstOrDefault(x => x.Id == processScopeModel.TemplateId)?.ActivitySettings);
        }

        public UserScraperModel UserScraperModel { get; set; }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                PinterestUser pinterestUser = (PinterestUser)scrapeResult.ResultUser;

                var response = PinFunct.GetUserDetails(pinterestUser.Username, DominatorAccountModel).Result;
                GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultUser.Username);

                IncrementCounters();

                AddUserScrapedDataToDataBase(response, scrapeResult.QueryInfo);
                jobProcessResult.IsProcessSuceessfull = true;

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


        private void AddUserScrapedDataToDataBase(Response.UserNameInfoPtResponseHandler userInfo, QueryInfo queryInfo)
        {
            try
            {
                IDbAccountService dbAccountService = new DbAccountService(DominatorAccountModel);
                IDbCampaignService dbCampaignService = new DbCampaignService(CampaignId);
                var user = (PinterestUser)userInfo;
                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleSetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];
                if (moduleSetting == null)
                    return;
                if (moduleSetting.IsTemplateMadeByCampaignMode)
                    dbCampaignService.Add(new DominatorHouseCore.DatabaseHandler.PdTables.Campaigns.InteractedUsers
                    {
                        ActivityType = ActivityType.ToString(),
                        QueryType = queryInfo.QueryType,
                        Query = queryInfo.QueryValue,
                        Username = DominatorAccountModel.AccountBaseModel.UserName,
                        InteractedUsername = user.Username,
                        Bio = user.UserBio,
                        FollowersCount = user.FollowersCount,
                        FollowingsCount = user.FollowingsCount,
                        FullName = user.FullName,
                        HasAnonymousProfilePicture = user.HasProfilePic,
                        PinsCount = user.PinsCount,
                        ProfilePicUrl = user.ProfilePicUrl,
                        InteractionTime = DateTimeUtilities.GetEpochTime(),
                        FollowedBack = user.FollowedBack,
                        InteractedUserId = user.UserId,
                        IsVerified = user.IsVerified,
                        Website = user.WebsiteUrl,
                        SinAccId = DominatorAccountModel.AccountBaseModel.UserId,
                        SinAccUsername = DominatorAccountModel.AccountBaseModel.UserName,
                        Type = PinterestIdentityType.User.ToString()

                    });
                dbAccountService.Add(new InteractedUsers
                {
                    ActivityType = ActivityType.ToString(),
                    QueryType = queryInfo.QueryType,
                    Query = queryInfo.QueryValue,                    
                    InteractedUsername = user.Username,
                    Username = DominatorAccountModel.AccountBaseModel.UserName,
                    Bio = user.UserBio,
                    FollowersCount = user.FollowersCount,
                    FollowingsCount = user.FollowingsCount,
                    FullName = user.FullName,
                    HasAnonymousProfilePicture = user.HasProfilePic,
                    PinsCount = user.PinsCount,
                    ProfilePicUrl = user.ProfilePicUrl,
                    InteractionTime = DateTimeUtilities.GetEpochTime(),
                    FollowedBack = user.FollowedBack,
                    InteractedUserId = user.UserId,
                    IsVerified = user.IsVerified,
                    Website = user.WebsiteUrl,
                    Type = PinterestIdentityType.User.ToString()
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}