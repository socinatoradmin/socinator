using System;
using System.Threading;
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
using LinkedDominatorCore.LDModel.GrowConnection;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Request;
using LinkedDominatorCore.Utility;

namespace LinkedDominatorCore.LDLibrary
{
    public class RemoveConnectionProcess : LDJobProcessInteracted<
        RemovedConnections>
    {
        private readonly ILdFunctions _ldFunctions;

        public RemoveConnectionProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ILdQueryScraperFactory queryScraperFactory,
            ILdHttpHelper ldHttpHelper, ILdLogInProcess logInProcess,
            ILdFunctionFactory ldFunctionFactory, IDbInsertionHelper dbInsertionHelper)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                ldHttpHelper, logInProcess, dbInsertionHelper)
        {
            RemoveOrWithdrawConnectionsModel = processScopeModel.GetActivitySettingsAs<RemoveConnectionModel>();
            _ldFunctions = ldFunctionFactory.LdFunctions;
            blackListWhitelistHandler =
                new BlackListWhiteListHandler(ModuleSetting, DominatorAccountModel, ActivityType);
        }

        private BlackListWhiteListHandler blackListWhitelistHandler { get; }

        public RemoveConnectionModel RemoveOrWithdrawConnectionsModel { get; set; }


        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            try
            {
                #region Remove Connection Process.

                var jobProcessResult = new JobProcessResult();
                var linkedinUser = (LinkedinUser)scrapeResult.ResultUser;

                if (RemoveOrWithdrawConnectionsModel.IsChkSkipWhiteListedUser && RemoveOrWithdrawConnectionsModel.IsChkUsePrivateWhiteList || RemoveOrWithdrawConnectionsModel.IsChkUseGroupWhiteList)
                {
                    var whiteListedType = RemoveOrWithdrawConnectionsModel.IsChkUseGroupWhiteList && RemoveOrWithdrawConnectionsModel.IsChkUsePrivateWhiteList
                                            ? Enums.WhitelistblacklistType.Both : RemoveOrWithdrawConnectionsModel.IsChkUsePrivateWhiteList
                                            ? Enums.WhitelistblacklistType.Private : Enums.WhitelistblacklistType.Group;
                    var whiteListUser = blackListWhitelistHandler.GetWhiteListUsers(whiteListedType);
                    foreach (var Items in whiteListUser)
                        if (linkedinUser.PublicIdentifier.Equals(Items))
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                "Skip User " + linkedinUser.PublicIdentifier + ", Present in Whitelist ");
                            return jobProcessResult;
                        }
                }

                #region Remove Connection

                string exceptionText;
                try
                {
                    GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, "remove connection", linkedinUser.FullName);

                    #region PostResponse and Db insertion

                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    if (UserProfileAction(linkedinUser, jobProcessResult))
                        return jobProcessResult;
                    Thread.Sleep(RandomUtilties.GetRandomNumber(5000, 2000));
                    var postResponse = _ldFunctions.RemoveConnection(linkedinUser);
                    if (postResponse == string.Empty)
                    {
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, "remove connection ",
                            linkedinUser.FullName);
                        DbInsertionHelper.RemoveConnection(scrapeResult, linkedinUser,
                            RemoveOrWithdrawConnectionsModel);
                        IncrementCounters();
                        jobProcessResult.IsProcessSuceessfull = true;
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, "remove connection ",
                            linkedinUser.FullName, "Unknown");
                    }

                    #endregion
                }
                catch (Exception ex)
                {
                    exceptionText = ex.Message;
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, "remove connection ", linkedinUser.FullName,
                        exceptionText);
                }

                #endregion

                DelayBeforeNextActivity();

                #endregion

                return jobProcessResult;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return new JobProcessResult();
            }
        }

        /// <summary>
        ///     Checking user is a first connection or not
        /// </summary>
        /// <param name="linkedinUser"></param>
        /// <param name="jobProcessResult"></param>
        /// <returns></returns>
        public bool UserProfileAction(LinkedinUser linkedinUser, JobProcessResult jobProcessResult)
        {
            try
            {
                var ldDataHelper = LdDataHelper.GetInstance;

                if (string.IsNullOrWhiteSpace(linkedinUser.PublicIdentifier) &&
                    string.IsNullOrWhiteSpace(linkedinUser.PublicIdentifier))
                    linkedinUser.PublicIdentifier =
                        ldDataHelper.GetPublicInstanceFromProfileUrl(linkedinUser.ProfileUrl);

                var checkConnection = IsBrowser
                    ? $"https://www.linkedin.com/in/{linkedinUser.PublicIdentifier}"
                    : !string.IsNullOrEmpty(linkedinUser.PublicIdentifier)
                        ? $"https://www.linkedin.com/voyager/api/identity/profiles/{linkedinUser.PublicIdentifier}/profileActions"
                        : $"https://www.linkedin.com/voyager/api/identity/profiles/{linkedinUser.ProfileId}/profileActions";

                var responseForProfileActions = _ldFunctions.GetRequestUpdatedUserAgent(checkConnection);
                if (!string.IsNullOrWhiteSpace(responseForProfileActions) &&
                    !responseForProfileActions.Contains("1st degree connection") &&
                    !ldDataHelper.ConnectionType(responseForProfileActions).Contains("1st") &&
                    !responseForProfileActions.Contains("profile.actions.Message"))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        $"{linkedinUser.FullName} is not 1st connection anymore");
                    Thread.Sleep(3000);
                    DbAccountService.RemoveMatch<Connections>(x =>
                        x.ProfileUrl == linkedinUser.ProfileUrl || x.ProfileId == linkedinUser.ProfileId);
                    jobProcessResult.IsProcessSuceessfull = false;
                    return true;
                }
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }

            return false;
        }
    }
}