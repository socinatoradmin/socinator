using System;
using System.Collections.Generic;
using System.Linq;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Campaign;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDModel.GrowConnection;

namespace LinkedDominatorCore.LDLibrary.Processor.Users
{
    public class EmailProcessor : BaseLinkedinUserProcessor, IQueryProcessor
    {
        private readonly ConnectionRequestModel _connectionRequestModel;

        public EmailProcessor(ILdJobProcess jobProcess, IDbCampaignService campaignService,
            ILdFunctionFactory ldFunctionFactory, IProcessScopeModel processScopeModel, IDelayService delayService) :
            base(jobProcess, campaignService, ldFunctionFactory, delayService, processScopeModel)
        {
            _connectionRequestModel = processScopeModel.GetActivitySettingsAs<ConnectionRequestModel>();
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                var emailAddress = string.Empty;
                List<InteractedUsers>
                    listInteractedUsersFromCampaignDb = null;
                if (!string.IsNullOrEmpty(queryInfo.QueryValue) && queryInfo.QueryValue.Contains("@"))
                {
                    emailAddress = queryInfo.QueryValue;
                }
                else
                {
                    jobProcessResult.HasNoResult = true;
                    return;
                }

                #region GetInteractedUsers  

                var listInteractedUsersFromAccountDb =
                    DbAccountService.GetInteractedUsers(ActivityTypeString, emailAddress).ToList();
                if (!string.IsNullOrEmpty(LdJobProcess.CurrentCampaignId))
                    listInteractedUsersFromCampaignDb =
                        DbCampaignService.GetInteractedUsers(ActivityTypeString, emailAddress).ToList();

                #endregion

                if (listInteractedUsersFromAccountDb.Any(x => x.QueryValue == emailAddress) ||
                    listInteractedUsersFromCampaignDb != null &&
                    listInteractedUsersFromCampaignDb.Any(x => x.QueryValue == emailAddress))
                {
                    jobProcessResult.HasNoResult = true;
                    return;
                }
                var isChkPrivateBlackList = _connectionRequestModel.IsChkPrivateBlackList;
                var isChkGroupBlackList = _connectionRequestModel.IsChkGroupBlackList;

                #region Skip Blacklisted User

                if (string.IsNullOrEmpty(emailAddress) || (isChkPrivateBlackList || isChkGroupBlackList) &&
                    manageBlacklistWhitelist.FilterBlackListedUser(emailAddress, isChkPrivateBlackList,
                        isChkGroupBlackList))
                {
                    jobProcessResult.HasNoResult = true;
                    return;
                }

                #endregion

                #region Send LinkedinUser Containing EmailAddress To FinalProcess 

                LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var objLinkedinUser = new LinkedinUser(emailAddress) {EmailAddress = emailAddress};
                LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                jobProcessResult = LdJobProcess.FinalProcess(new ScrapeResultNew
                {
                    ResultUser = objLinkedinUser,
                    QueryInfo = queryInfo
                });
                jobProcessResult.HasNoResult = true;

                #endregion
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                // return;
            }
        }
    }
}