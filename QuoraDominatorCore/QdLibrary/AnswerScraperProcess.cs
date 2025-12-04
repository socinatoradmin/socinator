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
    internal class AnswerScraperProcess : QdJobProcessInteracted<InteractedAnswers>
    {
        private readonly AnswersScraperModel _answerScraperModel;
        private readonly SocialNetworks _networks;

        public AnswerScraperProcess(IProcessScopeModel processScopeModel, IDbAccountServiceScoped accountServiceScoped,
            IExecutionLimitsManager executionLimitsManager, IQdQueryScraperFactory queryScraperFactory,
            IQdHttpHelper qdHttpHelper, IQdLogInProcess qdLogInProcess)
            : base(processScopeModel, accountServiceScoped, executionLimitsManager, queryScraperFactory, qdHttpHelper,
                qdLogInProcess)
        {
            _answerScraperModel = processScopeModel.GetActivitySettingsAs<AnswersScraperModel>();
            _networks = SocialNetworks.Quora;
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            var quorauser = (QuoraUser) scrapeResult.ResultUser;
            AddAnswerScrapDataToDataBase(quorauser, scrapeResult);
            GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, quorauser.Url);
            IncrementCounters();
            DelayBeforeNextActivity();
            return new JobProcessResult {IsProcessSuceessfull = true};
        }

        private void AddAnswerScrapDataToDataBase(QuoraUser quorauser, ScrapeResultNew scrapeResult)
        {
            try
            {
                #region Add to Account DB

                var dbAccountService =
                    InstanceProvider.ResolveAccountDbOperations(DominatorAccountModel.AccountId, _networks);

                dbAccountService.Add(
                    new InteractedAnswers
                    {
                        ActivityType = ActivityType.ToString(),
                        InteractionDateTime = DateTime.Now,
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        AnswersUrl = quorauser.Url,
                        Accountusername = DominatorAccountModel.UserName
                    });

                #endregion

                #region Add to campaign Db

                if (!string.IsNullOrEmpty(CampaignId))
                {
                    IDbCampaignService dbCampaignService = new DbCampaignService(CampaignId);
                    dbCampaignService.Add(new DominatorHouseCore.DatabaseHandler.QdTables.Campaigns.InteractedAnswers
                    {
                        ActivityType = ActivityType.ToString(),
                        InteractionDateTime = DateTime.Now,
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        AnswersUrl = quorauser.Url,
                        Accountusername = DominatorAccountModel.UserName
                    });
                }

                #endregion

                #region Add to PrivateBlacklist DB

                if (_answerScraperModel.IsChkAnswerScraperPrivateList)
                    dbAccountService.Add(
                        new PrivateBlacklist
                        {
                            UserName = quorauser.Username,
                            UserId = quorauser.UserId,
                            InteractionTimeStamp = GetEpochTime()
                        });

                #endregion

                #region Add to GroupBlacklist DB

                if (_answerScraperModel.IsChkAnswerScraperGroupList)
                {
                    IDbGlobalService dbGlobalService = new DbGlobalService();
                    dbGlobalService.Add(new BlackListUser
                    {
                        UserName = quorauser.Username,
                        UserId = quorauser.UserId,
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