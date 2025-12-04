using System;
using System.Linq;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Request;
using LinkedDominatorCore.Utility;

namespace LinkedDominatorCore.LDLibrary.GroupProcesses
{
    public class GroupInviterProcess : LDJobProcessInteracted<
        InteractedGroups>
    {
        private readonly IDelayService _delayService;
        private readonly ILdFunctions _ldFunctions;
        public GroupInviterModel GroupInviterModel;

        public GroupInviterProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ILdQueryScraperFactory queryScraperFactory,
            ILdHttpHelper ldHttpHelper, ILdLogInProcess logInProcess,
            ILdFunctionFactory ldFunctionFactory, IDbInsertionHelper dbInsertionHelper, IDelayService delayService)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                ldHttpHelper, logInProcess, dbInsertionHelper)
        {
            GroupInviterModel = processScopeModel.GetActivitySettingsAs<GroupInviterModel>();
            _ldFunctions = ldFunctionFactory.LdFunctions;
            _delayService = delayService;
        }


        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            var userDetail = (LinkedinUser) scrapeResult.ResultUser;
            #region Process Of Group Join Invitation.
            try
            {
                var groupsWithUrlAndName = DbAccountService.GetGroups()
                    .Where(x => GroupInviterModel.ListOfGroupUrl.Contains(x.GroupUrl))
                    .ToDictionary(x => x.GroupUrl, y => y.GroupName);

                foreach (var group in groupsWithUrlAndName)
                {
                    var groupId = LdDataHelper.GetInstance.GetGroupIdFromGroupUrl(group.Key);
                    var actionUrl = IsBrowser
                        ? group.Key
                        : "https://www.linkedin.com/voyager/api/groups/groupMemberships";

                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    SetHeaders();
                    var postData = "{\"elements\":[{\"entityUrn\":\"urn:li:fs_groupMembership:(" + groupId + "," +
                                   userDetail.ProfileId + ")\",\"groupUrn\":\"urn:li:fs_group:" + groupId +
                                   "\",\"miniProfileUrn\":\"urn:li:fs_miniProfile:" + userDetail.ProfileId +
                                   "\",\"status\":\"INVITE_PENDING\"}]}";
                    var responseGroupInviter = _ldFunctions.SendGroupInviter(actionUrl, postData);
                    if (!string.IsNullOrWhiteSpace(responseGroupInviter))
                    {
                        DbInsertionHelper.GroupInviter(scrapeResult, new LinkedinGroup(groupId)
                        {
                            GroupName = group.Value,
                            GroupUrl = group.Key
                        });
                        jobProcessResult.IsProcessSuceessfull = true;
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, "send group join request to", group);
                        IncrementCounters();

                        DbInsertionHelper.GroupJoiner(scrapeResult,
                            new LinkedinGroup(groupId) {GroupName = group.Value, GroupUrl = group.Key});
                        jobProcessResult.IsProcessSuceessfull = true;
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName,
                            "LangKeyJoinGroup".FromResourceDictionary(), group);
                        jobProcessResult.IsProcessSuceessfull = false;
                    }

                    DelayBeforeNextActivity();
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception e)
            {
                e.DebugLog();
            }
            #endregion
            return jobProcessResult;
        }

        public void SetHeaders()
        {
            var reqParams = _ldFunctions.GetInnerLdHttpHelper().GetRequestParameter();
            reqParams.ContentType = "application/json; charset=UTF-8";
            reqParams.Accept = "application/vnd.linkedin.normalized+json+2.1";
            if (!reqParams.Headers.ToString().Contains("x-restli-method"))
                reqParams.Headers.Add("x-restli-method", "BATCH_CREATE");
        }
    }
}