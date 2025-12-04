using System;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.QdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.Factories;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.Request;

namespace QuoraDominatorCore.QdLibrary
{
    internal class UserScraperProcess : QdJobProcessInteracted<InteractedUsers>
    {
        private readonly SocialNetworks _networks;
        private readonly UserScraperModel _userScraperModel;
        private readonly IDbGlobalService dbGlobalService;

        public UserScraperProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, IQdQueryScraperFactory queryScraperFactory,
            IQdHttpHelper qdHttpHelper, IQdLogInProcess qdLogInProcess)
            : base(processScopeModel, accountServiceScoped, executionLimitsManager, queryScraperFactory, qdHttpHelper,
                qdLogInProcess)
        {
            dbGlobalService = globalService;
            _userScraperModel = processScopeModel.GetActivitySettingsAs<UserScraperModel>();
            _networks = SocialNetworks.Quora;
        }

        public QueryInfo QueryInfo { get; set; }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            QueryInfo = scrapeResult.QueryInfo;

            SaveUserScrapedDataToDb(scrapeResult);
            GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultUser.Username);

            IncrementCounters();
            DelayBeforeNextActivity();
            return new JobProcessResult {IsProcessSuceessfull = true};
        }

        private void SaveUserScrapedDataToDb(ScrapeResultNew scrapeResult)
        {
            try
            {
                var quorauser = (QuoraUser) scrapeResult.ResultUser;

                #region Save to Account DB

                var dbAccountService =
                    InstanceProvider.ResolveAccountDbOperations(DominatorAccountModel.AccountId, _networks);

                dbAccountService.Add(new InteractedUsers
                {
                    ActivityType = ActivityType.ToString(),
                    Date = DateTimeUtilities.GetEpochTime(),
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    Query = scrapeResult.QueryInfo.QueryValue,
                    Username = DominatorAccountModel.AccountBaseModel.UserName,
                    InteractedUsername = scrapeResult.ResultUser.Username,
                    InteractedUserId = quorauser.UserId,
                    FollowedBack = quorauser.FollowedBack
                });

                #endregion

                #region  Save to Campaign DB

                if (!string.IsNullOrEmpty(CampaignId))
                {
                    IDbCampaignService dbCampaignService = new DbCampaignService(CampaignId);
                    dbCampaignService.Add(new DominatorHouseCore.DatabaseHandler.QdTables.Campaigns.InteractedUsers
                    {
                        ActivityType = ActivityType.ToString(),
                        InteractionDateTime = DateTime.Now,
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        SinAccUsername = DominatorAccountModel.AccountBaseModel.UserName,
                        InteractedUsername = scrapeResult.ResultUser.Username,
                        InteractedUserId = quorauser.UserId,
                        FollowStatus = quorauser.FollowedBack,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        FollowersCount = quorauser.FollowerCount,
                        FollowingsCount = quorauser.FollowingCount,
                        QuestionCount = quorauser.QuestionCount,
                        AnswerCount = quorauser.AnswerCount,
                        PostCount = quorauser.NumberOfPost
                    });
                }

                #endregion

                #region  Save to PrivateBlacklist DB

                if (_userScraperModel.IsChkUserScraperPrivateBlacklist)
                    dbAccountService.Add(
                        new PrivateBlacklist
                        {
                            UserName = quorauser.Username,
                            UserId = quorauser.UserId,
                            InteractionTimeStamp = DateTimeUtilities.GetEpochTime()
                        });

                #endregion

                #region Save to GroupBlacklist DB

                if (_userScraperModel.IsChkUserScraperGroupBlacklist)
                    dbGlobalService.Add(new BlackListUser
                    {
                        UserName = quorauser.Username,
                        UserId = quorauser.UserId,
                        AddedDateTime = DateTime.Now
                    });

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}