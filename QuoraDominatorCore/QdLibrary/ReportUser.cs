using CommonServiceLocator;
using DominatorHouseCore;
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
    internal class ReportUser : QdJobProcessInteracted<InteractedUsers>
    {
        private IQuoraBrowserManager _browser;
        private readonly SocialNetworks _networks;
        private readonly ReportUserModel _reportUserModel;
        private readonly IQuoraFunctions quoraFunct;
        private IQDBrowserManagerFactory managerFactory;
        public ReportUser(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped,
            IExecutionLimitsManager executionLimitsManager, IQuoraFunctions qdFunc,
            IQdQueryScraperFactory queryScraperFactory, IQdHttpHelper qdHttpHelper, IQdLogInProcess qdLogInProcess)
            : base(processScopeModel, accountServiceScoped, executionLimitsManager, queryScraperFactory, qdHttpHelper,
                qdLogInProcess)
        {
            quoraFunct = qdFunc;
            _reportUserModel = processScopeModel.GetActivitySettingsAs<ReportUserModel>();
            _networks = SocialNetworks.Quora;
            managerFactory = qdLogInProcess.managerFactory;
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            _browser = managerFactory.QdBrowserManager();
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultUser.Username);
            var quoraUser = (QuoraUser) scrapeResult.ResultUser;
            var jobProcessResult = new JobProcessResult();
            try
            {
                var IsSuccess = false;
                var ErrorMessage = string.Empty;
                var Status = string.Empty;
                if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var ReportUserResponse = quoraFunct.OldReportUser(DominatorAccountModel, scrapeResult).Result;
                    IsSuccess = ReportUserResponse.Success;
                    ErrorMessage = ReportUserResponse?.Issue?.Message;
                    Status = ReportUserResponse?.Status;
                }
                else
                {
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var url = "https://www.quora.com/profile/" + quoraUser.Username;
                    _browser.SearchByCustomUrl(DominatorAccountModel, url);
                    IsSuccess = _browser.ReportUser(url,scrapeResult.QueryInfo.CustomFilters,ref ErrorMessage);
                }
                if (IsSuccess || Status.Contains("success"))
                {
                    AddReportDataToDataBase(scrapeResult, quoraUser);
                    IncrementCounters();
                    if(Status.Contains("user_has_already_reported"))
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"{Status} to "+quoraUser.Username);
                    else
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, quoraUser.Username);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                       $"{scrapeResult.ResultUser.Username} ==> {ErrorMessage}");
                    jobProcessResult.IsProcessSuceessfull = false;
                }
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            }
            catch (Exception e)
            {
                e.DebugLog();
            }
            if (_reportUserModel != null && _reportUserModel.IsEnableAdvancedUserMode && _reportUserModel.EnableDelayBetweenPerformingActionOnSamePost)
                DelayBeforeNextActivity(_reportUserModel.DelayBetweenPerformingActionOnSamePost.GetRandom());
            else
                DelayBeforeNextActivity();
            return jobProcessResult;
        }

        private void AddReportDataToDataBase(ScrapeResultNew scrapeResult, QuoraUser quoraUser)
        {
            try
            {
                #region Save to CampaignDB

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
                        InteractedUserId = quoraUser.UserId,
                        FollowStatus = quoraUser.FollowedBack,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime()
                    });
                }

                #endregion

                #region Save to AccountDB

                var dbAccountService =
                    InstanceProvider.ResolveAccountDbOperations(DominatorAccountModel.AccountId, _networks);
                dbAccountService.Add(new InteractedUsers
                {
                    ActivityType = ActivityType.ToString(),
                    Date = GetEpochTime(),
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    Query = scrapeResult.QueryInfo.QueryValue,
                    Username = DominatorAccountModel.AccountBaseModel.UserName,
                    InteractedUsername = scrapeResult.ResultUser.Username,
                    InteractedUserId = quoraUser.UserId,
                    Time = DateTimeUtilities.GetEpochTime(),
                    FollowedBack = quoraUser.FollowedBack
                });

                #endregion

                #region Save to PrivateBlacklistDb

                if (_reportUserModel.IsChkPrivateblacklisted)
                    dbAccountService.Add(
                        new PrivateBlacklist
                        {
                            UserName = quoraUser.Username,
                            InteractionTimeStamp = DateTime.Now.GetCurrentEpochTime()
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