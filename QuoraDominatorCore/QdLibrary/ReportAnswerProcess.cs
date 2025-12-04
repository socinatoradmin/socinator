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
using QuoraDominatorCore.QDFactories;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using QuoraDominatorCore.Request;
using System;

namespace QuoraDominatorCore.QdLibrary
{
    internal class ReportAnswerProcess : QdJobProcessInteracted<InteractedAnswers>
    {
        private readonly IQdHttpHelper _httpHelper;
        private readonly SocialNetworks _networks;
        private readonly ReportAnswerModel _reportAnswerModel;
        private readonly IQuoraFunctions quoraFunct;
        private IQuoraBrowserManager _browser;
        private IQDBrowserManagerFactory managerFactory;
        public ReportAnswerProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped,
            IExecutionLimitsManager executionLimitsManager, IQuoraFunctions qdFunc,
            IQdQueryScraperFactory queryScraperFactory, IQdHttpHelper qdHttpHelper, IQdLogInProcess qdLogInProcess)
            : base(processScopeModel, accountServiceScoped, executionLimitsManager, queryScraperFactory, qdHttpHelper,
                qdLogInProcess)
        {
            _httpHelper = qdHttpHelper;
            quoraFunct = qdFunc;
            _reportAnswerModel = processScopeModel.GetActivitySettingsAs<ReportAnswerModel>();
            _networks = SocialNetworks.Quora;
            managerFactory = qdLogInProcess.managerFactory;
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            _browser = managerFactory.QdBrowserManager();
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            var quoraUser = (QuoraUser) scrapeResult.ResultUser;
            GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, quoraUser.Url);

            var jobProcessResult = new JobProcessResult();

            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var IsSuccess = false;
                var ErrorMessage = string.Empty;
                var ReportDetails = scrapeResult.QueryInfo.CustomFilters;
                if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    var linkresp = _httpHelper.GetRequest(quoraUser.Url).Response;
                    var response = quoraFunct.ReportAnswer(DominatorAccountModel, linkresp, ReportDetails,quoraUser.Url).Result;
                    IsSuccess = response != null && response.Success;
                    ErrorMessage= response?.Issue?.Message;
                }
                else
                {
                    IsSuccess = _browser.ReportAnswer(quoraUser.Url, ReportDetails, ref ErrorMessage);
                }
                if (IsSuccess)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, quoraUser.Url);
                    AddFollowedDataToDataBase(scrapeResult);
                    IncrementCounters();
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName,
                        ActivityType,$"{quoraUser.Url} ==> {ErrorMessage}");
                }
                jobProcessResult.IsProcessSuceessfull = IsSuccess;
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            }
            catch (Exception e)
            {
                e.DebugLog();
            }
            if (_reportAnswerModel != null && _reportAnswerModel.IsEnableAdvancedUserMode && _reportAnswerModel.EnableDelayBetweenPerformingActionOnSamePost)
                DelayBeforeNextActivity(_reportAnswerModel.DelayBetweenPerformingActionOnSamePost.GetRandom());
            else
                DelayBeforeNextActivity();

            return jobProcessResult;
        }

        private void AddFollowedDataToDataBase(ScrapeResultNew scrapeResult)
        {
            var quorauser = (QuoraUser) scrapeResult.ResultUser;
            try
            {
                #region Save to CampaignDB

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
                        AnsweredUserName = quorauser.Username,
                        Accountusername = DominatorAccountModel.UserName
                    });
                }

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            try
            {
                #region Save to AccountDB

                var dbAccountService =
                    InstanceProvider.ResolveAccountDbOperations(DominatorAccountModel.AccountId, _networks);
                dbAccountService.Add(new InteractedAnswers
                {
                    ActivityType = ActivityType.ToString(),
                    InteractionDateTime = DateTime.Now,
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    AnswersUrl = quorauser.Url,
                    AnsweredUserName = quorauser.Username,
                    Accountusername = DominatorAccountModel.UserName
                });

                #endregion

                #region Save to PrivateBlacklistDB

                if (_reportAnswerModel.IsChkReportAnswerPrivateBlacklist)
                    dbAccountService.Add(
                        new PrivateBlacklist
                        {
                            UserName = quorauser.Username,
                            UserId = quorauser.UserId,
                            InteractionTimeStamp = DateTime.Now.GetCurrentEpochTime()
                        });

                #endregion

                #region Add to GroupBlacklist DB

                if (_reportAnswerModel.IsChkReportAnswerGroupBlacklist)
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