using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CommonServiceLocator;
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
using LinkedDominatorCore.LDModel.GrowConnection;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Request;
using LinkedDominatorCore.Utility;

namespace LinkedDominatorCore.LDLibrary
{
    public class WithdrawConnectionRequestProcess : LDJobProcessInteracted<
        InteractedUsers>
    {
        private readonly ILdFunctions _ldFunctions;


        public WithdrawConnectionRequestProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ILdQueryScraperFactory queryScraperFactory,
            ILdHttpHelper ldHttpHelper, ILdLogInProcess logInProcess,
            ILdFunctionFactory ldFunctionFactory, IDbInsertionHelper dbInsertionHelper)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                ldHttpHelper, logInProcess, dbInsertionHelper)
        {
            WithdrawConnectionRequestModel = processScopeModel.GetActivitySettingsAs<WithdrawConnectionRequestModel>();
            _ldFunctions = ldFunctionFactory.LdFunctions;
        }

        public WithdrawConnectionRequestModel WithdrawConnectionRequestModel { get; set; }


        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobProcessResult jobProcessResult = null;
            #region Withdraw Connection Request Process.
            try
            {
                jobProcessResult = new JobProcessResult();

                var linkedinUser = (LinkedinUser) scrapeResult.ResultUser;
                var withDrawUserName =
                    linkedinUser.FullName != "N/A" ? linkedinUser.FullName : linkedinUser.EmailAddress;

                GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, withDrawUserName);
                linkedinUser.PostData = DynamicPostData(linkedinUser.InvitationId);
                if (UserProfileAction(linkedinUser, jobProcessResult))
                    return jobProcessResult;
                var postResponse = _ldFunctions.WithDrawConnectionRequest(linkedinUser);

                if (!string.IsNullOrEmpty(postResponse) &&
                    postResponse.Contains($"urn:li:fs_relInvitation:{linkedinUser.InvitationId}"))
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, withDrawUserName);
                    DbInsertionHelper.WithdrawConnection(scrapeResult, linkedinUser, WithdrawConnectionRequestModel);
                    IncrementCounters();
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, withDrawUserName, "Unknown");
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            StartOtherConfiguration(scrapeResult);
            DelayBeforeNextActivity();
            #endregion
            return jobProcessResult;
        }


        private string DynamicPostData(string invitationId)
        {
            try
            {
                var objJsonElements = new LdJsonElements
                {
                    VisibleToConnectionsOnly = false,
                    InviteActionType = "WITHDRAW",
                    InviteActionData = new List<LdJsonElements>
                    {
                        new LdJsonElements
                        {
                            EntityUrn = $"urn:li:fs_relInvitation:{invitationId}",
                            ValidationToken = "dummy",
                            GenericInvitation = false
                        }
                    }
                };

                return new LdRequestParameters().GenerateStringBody(objJsonElements);
            }
            catch (Exception exception)
            {
                exception.DebugLog();
                return "";
            }
        }

        public bool UserProfileAction(LinkedinUser linkedinUser, JobProcessResult jobProcessResult)
        {
            try
            {
                var ldDataHelper = LdDataHelper.GetInstance;
                if (string.IsNullOrWhiteSpace(linkedinUser.PublicIdentifier) &&
                    string.IsNullOrWhiteSpace(linkedinUser.PublicIdentifier))
                    linkedinUser.PublicIdentifier =
                        LdDataHelper.GetInstance.GetPublicInstanceFromProfileUrl(linkedinUser.ProfileUrl);

                var checkConnection = IsBrowser
                    ? $"https://www.linkedin.com/in/{linkedinUser.PublicIdentifier}"
                    : !string.IsNullOrEmpty(linkedinUser.PublicIdentifier)
                        ? $"https://www.linkedin.com/voyager/api/identity/profiles/{linkedinUser.PublicIdentifier}/profileActions"
                        : $"https://www.linkedin.com/voyager/api/identity/profiles/{linkedinUser.ProfileId}/profileActions";

                var responseForProfileActions = _ldFunctions.GetRequestUpdatedUserAgent(checkConnection);

                if (!string.IsNullOrEmpty(responseForProfileActions) &&
                    !responseForProfileActions.Contains("profile.actions.Connect"))
                {
                    if ((responseForProfileActions.Contains("profile.actions.InvitationPending") ||
                         ldDataHelper.IsAlreadySendConnectionRequest(responseForProfileActions)||
                         responseForProfileActions.Contains("Withdraw invitation sent to")) &&
                        !responseForProfileActions.Contains(
                            "pv-s-profile-actions pv-s-profile-actions--send-in-mail ml2 artdeco-button artdeco-button--2 artdeco-button--primary ember-view")
                    )
                        return false;

                    //removing already connected users
                    if (responseForProfileActions.Contains("profile.actions.Message") ||
                        ldDataHelper.ConnectionType(responseForProfileActions).Contains("1"))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            $"{linkedinUser.FullName} is already 1st connection");
                        Thread.Sleep(3000);
                        var connection = ClassMapper.Instance.LinkedInUserToConnections(linkedinUser);
                        DbAccountService.Add(connection);
                        jobProcessResult.IsProcessSuceessfull = false;
                        return true;
                    }
                }
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }

            return false;
        }

        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {
            try
            {
                // since we have no where to check how many use we have been withdrawn we are getting count from Db
                var lstWithdrawnConnectionRequests = DbAccountService
                    .GetInteractedUsers(ActivityType.WithdrawConnectionRequest.ToString()).ToList();

                #region Process for auto ConnectionRequest 

                var totalWithdrawnConnectionRequests = lstWithdrawnConnectionRequests.Count;

                if (WithdrawConnectionRequestModel.IsCheckedEnableAutoConnectionRequest)
                    if (WithdrawConnectionRequestModel.IsCheckedStartConnectionRequestWhenLimitReach &&
                        totalWithdrawnConnectionRequests >= WithdrawConnectionRequestModel
                            .StartConnectionRequestWhenLimitReach.GetRandom()
                        || WithdrawConnectionRequestModel.IsCheckedWithdrawConnectionRequestToolGetsTemporaryBlocked &&
                        scrapeResult.IsAccountLocked)
                        try
                        {
                            var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                            dominatorScheduler.EnableDisableModules(ActivityType.WithdrawConnectionRequest,
                                ActivityType.ConnectionRequest, DominatorAccountModel.AccountId);
                        }
                        catch (InvalidOperationException ex)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.UserName, ex.Message.Contains("1001")
                                    ? "LangKeyWithdrawConnectionRequestHasMetAutoEnableConfiguration"
                                        .FromResourceDictionary()
                                    : "");
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