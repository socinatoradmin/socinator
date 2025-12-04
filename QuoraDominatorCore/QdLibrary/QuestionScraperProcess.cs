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
    public class QuestionScraperProcess : QdJobProcessInteracted<InteractedQuestion>
    {
        private readonly SocialNetworks _networks;
        private readonly QuestionsScraperModel _questionScraperModel;

        public QuestionScraperProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped,
            IExecutionLimitsManager executionLimitsManager, IQdQueryScraperFactory queryScraperFactory,
            IQdHttpHelper qdHttpHelper, IQdLogInProcess qdLogInProcess)
            : base(processScopeModel, accountServiceScoped, executionLimitsManager, queryScraperFactory, qdHttpHelper,
                qdLogInProcess)
        {
            _questionScraperModel = processScopeModel.GetActivitySettingsAs<QuestionsScraperModel>();
            _networks = SocialNetworks.Quora;
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            AddUpvotedQuestionDataToDataBase(scrapeResult);
            GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                ((QuoraUser) scrapeResult.ResultUser).Url);
            IncrementCounters();
            DelayBeforeNextActivity();
            return new JobProcessResult {IsProcessSuceessfull = true};
        }

        private void AddUpvotedQuestionDataToDataBase(ScrapeResultNew scrapeResult)
        {
            var quoraUser = (QuoraUser) scrapeResult.ResultUser;
            try
            {
                #region Save To CampaignDb

                if (!string.IsNullOrEmpty(CampaignId))
                {
                    IDbCampaignService dbCampaignService = new DbCampaignService(CampaignId);
                    dbCampaignService.Add(new DominatorHouseCore.DatabaseHandler.QdTables.Campaigns.InteractedQuestion
                    {
                        ActivityType = ActivityType.ToString(),
                        InteractionDateTime = DateTime.Now,
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        QuestionUrl = quoraUser.Url,
                        Accountusername = DominatorAccountModel.UserName
                    });
                }

                #endregion

                #region Save to AccountDb

                var dbAccountService =
                    InstanceProvider.ResolveAccountDbOperations(DominatorAccountModel.AccountId, _networks);

                dbAccountService.Add(new InteractedQuestion
                {
                    ActivityType = ActivityType.ToString(),
                    InteractionDateTime = DateTime.Now,
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    QuestionUrl = quoraUser.Url,
                    Accountusername = DominatorAccountModel.UserName
                });

                #endregion

                #region Save to PrivateBlacklist DB

                if (_questionScraperModel.IsChkPrivateblacklist)
                    dbAccountService.Add(
                        new PrivateBlacklist
                        {
                            UserName = quoraUser.Username,
                            UserId = quoraUser.UserId,
                            InteractionTimeStamp = DateTime.Now.GetCurrentEpochTime()
                        });

                #endregion

                #region Save to GroupBlacklist DB

                if (_questionScraperModel.IsChkGroupblacklist)
                {
                    IDbGlobalService dbGlobalService = new DbGlobalService();
                    dbGlobalService.Add(new BlackListUser
                    {
                        UserName = quoraUser.Username,
                        UserId = quoraUser.UserId,
                        AddedDateTime = DateTime.Now
                    });
                }

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}