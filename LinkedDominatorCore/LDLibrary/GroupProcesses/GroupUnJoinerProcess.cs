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
using LinkedDominatorCore.Request;
using LinkedDominatorCore.Utility;

namespace LinkedDominatorCore.LDLibrary
{
    public class GroupUnJoinerProcess : LDJobProcessInteracted<
        InteractedGroups>
    {
        private readonly IDelayService _delayService;
        private readonly ILdFunctions _ldFunctions;


        public GroupUnJoinerProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ILdQueryScraperFactory queryScraperFactory,
            ILdHttpHelper ldHttpHelper, ILdLogInProcess logInProcess,
            ILdFunctionFactory ldFunctionFactory, IDbInsertionHelper dbInsertionHelper)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                ldHttpHelper, logInProcess, dbInsertionHelper)
        {
            _delayService = InstanceProvider.GetInstance<IDelayService>();
            GroupUnJoinerModel = processScopeModel.GetActivitySettingsAs<GroupUnJoinerModel>();
            _ldFunctions = ldFunctionFactory.LdFunctions;
            CurrentActivityType = processScopeModel.ActivityType.ToString();
        }

        public GroupUnJoinerModel GroupUnJoinerModel { get; set; }

        public string CurrentActivityType { get; set; }


        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobProcessResult jobProcessResult = null;
            #region Group Unjoiner Process.
            try
            {
                var objLinkedinGroup = (LinkedinGroup) scrapeResult.ResultGroup;
                jobProcessResult = new JobProcessResult();
                GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, "unJoin group", objLinkedinGroup.GroupUrl);

                #region UnJoinGroupsRequest

                var unJoinGroupsRequestResponse = string.Empty;
                var groupId = objLinkedinGroup.GroupUrl.Split('/').Last();
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                var actionUrl = IsBrowser
                    ? objLinkedinGroup.GroupUrl
                    : $"https://www.linkedin.com/voyager/api/groups/groups/urn%3Ali%3Agroup%3A{groupId}/members?action=updateMembershipStatus";

                var postString = IsBrowser
                    ? groupId
                    : "{\"memberProfileId\":\"" + DominatorAccountModel.AccountBaseModel.UserId +
                      "\",\"actionType\":\"LEAVE_GROUP\"}";
                unJoinGroupsRequestResponse = _ldFunctions.UnJoinGroupsRequest(actionUrl, postString);

                if (unJoinGroupsRequestResponse == null)
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, "LangKeyUnjoinGroup".FromResourceDictionary(),
                        objLinkedinGroup.GroupName);
                    jobProcessResult.IsProcessSuceessfull = false;
                }
                else if (unJoinGroupsRequestResponse.Contains($"urn:li:fs_group:{groupId}"))
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, "LangKeyUnjoinGroup".FromResourceDictionary(),
                        objLinkedinGroup.GroupName);
                    IncrementCounters();
                    DbInsertionHelper.GroupUnJoiner(scrapeResult, objLinkedinGroup);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, "LangKeyUnjoinGroup".FromResourceDictionary(),
                        objLinkedinGroup.GroupName, "");
                    jobProcessResult.IsProcessSuceessfull = false;
                }

                #endregion

                DelayBeforeNextActivity();
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            #endregion
            return jobProcessResult;
        }


        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {
            try
            {
                base.StartOtherConfiguration(scrapeResult);
                var lstUnJoinedGroups = DbAccountService.GetInteractedGroups(CurrentActivityType).ToList();

                #region Process for auto Group Joiner 

                var totalUnJoinedGroups = lstUnJoinedGroups.Count;

                if (GroupUnJoinerModel.IsCheckedEnableAutoGroupJoiner)
                    if (GroupUnJoinerModel.IsCheckedStartGroupJoinerWhenLimitReach && totalUnJoinedGroups >
                        GroupUnJoinerModel.StartGroupJoinerWhenLimitReach.GetRandom()
                        || GroupUnJoinerModel.IsCheckedGroupUnJoinerToolGetsTemporaryBlocked &&
                        scrapeResult.IsAccountLocked)
                        try
                        {
                            if (GroupUnJoinerModel.IsCheckedStartGroupJoinerBetween)
                            {
                                var minutes = GroupUnJoinerModel.StartGroupJoinerAfter.GetRandom();
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    string.Format(
                                        "LangKeyAutoGroupJoinerWillStartAfterXMinutes".FromResourceDictionary(),
                                        minutes));
                                _delayService.ThreadSleep(minutes * 60000);
                            }

                            var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                            dominatorScheduler.EnableDisableModules(ActivityType.GroupUnJoiner,
                                ActivityType.GroupJoiner, DominatorAccountModel.AccountId);
                        }
                        catch (InvalidOperationException ex)
                        {
                            if (ex.Message.Contains("1001"))
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.UserName,
                                    "LangKeyGroupUnjoinerHasMetAutoEnableConfiguration".FromResourceDictionary());
                            else
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.UserName, "");
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
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