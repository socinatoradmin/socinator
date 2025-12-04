using System;
using System.Linq;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.Enums;
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

namespace LinkedDominatorCore.LDLibrary
{
    public class GroupJoinerProcess : LDJobProcessInteracted<
        InteractedGroups>
    {
        private readonly IDelayService _delayService;
        private readonly ILdFunctions _ldFunctions;
        private int failedGroupJoinRequestCount;


        public GroupJoinerProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ILdQueryScraperFactory queryScraperFactory,
            ILdHttpHelper ldHttpHelper, ILdLogInProcess logInProcess,
            ILdFunctionFactory ldFunctionFactory, IDbInsertionHelper dbInsertionHelper, IDelayService delayService)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                ldHttpHelper, logInProcess, dbInsertionHelper)
        {
            GroupJoinerModel = processScopeModel.GetActivitySettingsAs<GroupJoinerModel>();
            _ldFunctions = ldFunctionFactory.LdFunctions;
            _delayService = delayService;
        }

        public GroupJoinerModel GroupJoinerModel { get; set; }


        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var linkedinGroup = (LinkedinGroup) scrapeResult.ResultGroup;
            var jobProcessResult = new JobProcessResult();
            var LAreadyexistGroup =
                DbAccountService.GetGroups().Where(x => x.GroupUrl == linkedinGroup.GroupUrl).ToList();
            #region Group Joiner Process.
            if (LAreadyexistGroup.Count != 0 && IsBrowser)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    $"is already a member in {linkedinGroup.GroupName}");
                return jobProcessResult;
            }            
            var actionUrl = IsBrowser
                ? linkedinGroup.GroupUrl
                : $"https://www.linkedin.com/voyager/api/groups/groups/urn%3Ali%3Agroup%3A{linkedinGroup.GroupId}";
            if (string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.UserId) && !IsBrowser)
            {
                var Details = _ldFunctions.GetRequestUpdatedUserAgent("https://www.linkedin.com/", true);
                var profileId = Utils.GetBetween(Details, "miniProfile\":\"urn:li:fs_miniProfile:", "\"");
                DominatorAccountModel.AccountBaseModel.UserId = profileId;
            }

            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var responseMemberShip = _ldFunctions.CheckGroupMemberShip(actionUrl);
                if (responseMemberShip.Success)
                {
                    switch (responseMemberShip.Status)
                    {
                        case "REQUEST_PENDING":
                        case "Withdraw_Request":
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                $"group join request already sent to {linkedinGroup.GroupName}");
                            jobProcessResult.IsProcessSuceessfull = false;
                            return jobProcessResult;

                        case "MEMBER":
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                $"is already a member in {linkedinGroup.GroupName}");
                            jobProcessResult.IsProcessSuceessfull = false;
                            return jobProcessResult;

                        case "N/A":
                            {
                                GlobusLogHelper.log.Info(Log.StartedActivity,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, "send group join request to",
                                    linkedinGroup.GroupUrl);

                                var groupJoinRequestActionUrl = IsBrowser
                                    ? linkedinGroup.GroupUrl
                                    : $"https://www.linkedin.com/voyager/api/groups/groups/urn%3Ali%3Agroup%3A{linkedinGroup.GroupId}/members?action=updateMembershipStatus";
                                //$"https://www.linkedin.com/voyager/api/entities/groups/{linkedinGroup.GroupId}?action=sendJoinRequest&nc={Utils.GenerateNc()}" ;
                                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                var response = !IsBrowser ?
                                    _ldFunctions.GroupJoinRequest(linkedinGroup.GroupId)
                                    : _ldFunctions.SendGroupJoinRequest(groupJoinRequestActionUrl);
                                if (response.Contains("membershipStatus\":\"REQUEST_PENDING") ||
                                    response.Contains("urn:li:fs_groupFollowingInfo") || response.Contains("\"status\":\"REQUEST_PENDING\"") || response.Contains("\"entityUrn\":\"urn: li:fsd_groupMembership:")||response.Contains("\"status\":\"MEMBER\""))
                                {
                                    failedGroupJoinRequestCount = 0;
                                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                        DominatorAccountModel.AccountBaseModel.UserName, "send group join request to",
                                        linkedinGroup.GroupUrl);
                                    IncrementCounters();

                                    DbInsertionHelper.GroupJoiner(scrapeResult, linkedinGroup);
                                    jobProcessResult.IsProcessSuceessfull = true;
                                }
                                else if (response.Contains("EXCEED_MAX_PENDING_GROUP_MEMBERSHIPS"))
                                {
                                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                        "EXCEED MAX PENDING GROUP MEMBERSHIPS");
                                    jobProcessResult.IsProcessSuceessfull = false;
                                    jobProcessResult.HasNoResult = true;
                                    jobProcessResult.IsProcessCompleted = true;
                                    return jobProcessResult;
                                }
                                else if (response.Contains("Withdraw Request") || string.IsNullOrEmpty(response))
                                {
                                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                        $"group join request already sent to {linkedinGroup.GroupName}");
                                    jobProcessResult.IsProcessSuceessfull = false;
                                }
                                else
                                {
                                    ++failedGroupJoinRequestCount;
                                    if (response.Contains("You have reached your request limit"))
                                        GlobusLogHelper.log.Info(Log.CustomMessage,
                                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                            "Failed to join groups,Because Your request limit have been reached");
                                    else
                                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                            DominatorAccountModel.AccountBaseModel.UserName,
                                            "LangKeyJoinGroup".FromResourceDictionary(), linkedinGroup.GroupUrl,
                                            "LangKeyAlready10GroupJoinRequestPending".FromResourceDictionary());
                                    if (GroupJoinerModel.IsStopSendGroupJoinRequestOnFailed &&
                                        GroupJoinerModel.StopSendGroupJoinRequestOnCount <= failedGroupJoinRequestCount)
                                    {
                                        var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                                        dominatorScheduler.ChangeAccountsRunningStatus(false, AccountId, ActivityType);
                                        GlobusLogHelper.log.Info(Log.CustomMessage,
                                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                            "LangKeyStoppingActivityReachedStopActivityOnContinuousFailsCount"
                                                .FromResourceDictionary());
                                        return JobProcessResult;
                                    }
                                }

                                break;
                            }
                    }
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, "LangKeyJoinGroup".FromResourceDictionary(),
                        linkedinGroup.GroupUrl);
                    jobProcessResult.IsProcessSuceessfull = false;
                }

                DelayBeforeNextActivity();
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


        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {
            try
            {
                base.StartOtherConfiguration(scrapeResult);
                var dbAccountService = InstanceProvider.GetInstance<DbAccountService>();
                //  var objLdFunctions = new LdFunctions(AccountModel);
                //   var lstGroups = DbAccountService.GetGroups().ToList();
                var lstTotalGroupJoinRequestSent =
                    dbAccountService.GetInteractedGroups(ActivityType.ToString()).ToList();

                #region Process for auto GroupJoiner and GroupUnJoiner

                var totalGroupJoinRequestSent = lstTotalGroupJoinRequestSent.Count;

                if (!GroupJoinerModel.IsCheckedEnableAutoGroupUnJoiner) return;

                if (GroupJoinerModel.IsCheckedStartGroupUnJoinerWhenLimitReach && totalGroupJoinRequestSent >
                    GroupJoinerModel.StartGroupUnJoinerWhenLimitReach.GetRandom()
                    || GroupJoinerModel.IsCheckedGroupJoinerToolGetsTemporaryBlocked && scrapeResult.IsAccountLocked)
                    try
                    {
                        if (GroupJoinerModel.IsCheckedStartGroupUnJoinerBetween)
                        {
                            var minutes = GroupJoinerModel.StartGroupUnJoinerAfter.GetRandom();
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                string.Format("LangKeyAutoGroupUnJoinerWillStartAfterXMinutes".FromResourceDictionary(),
                                    minutes));
                            _delayService.ThreadSleep(minutes * 60000);
                        }

                        var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                        dominatorScheduler.EnableDisableModules(ActivityType.GroupJoiner,
                            ActivityType.GroupUnJoiner, DominatorAccountModel.AccountId);
                    }
                    catch (InvalidOperationException ex)
                    {
                        if (ex.Message.Contains("1001"))
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                                "LangKeyGroupJoinerHasMetAutoEnableConfiguration".FromResourceDictionary());
                        else
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                                "");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                #endregion
            }
            catch (Exception e)
            {
                e.DebugLog();
            }
        }
    }
}