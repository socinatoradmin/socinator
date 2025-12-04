using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
using Newtonsoft.Json;

namespace LinkedDominatorCore.LDLibrary.MessengerProcesses
{
    public class DeleteConversationsProcess : LDJobProcessInteracted<
        InteractedUsers>
    {
        private readonly IDelayService _delayService;
        private readonly ILdFunctions _ldFunctions;

        public DeleteConversationsProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ILdQueryScraperFactory queryScraperFactory,
            ILdHttpHelper ldHttpHelper, ILdLogInProcess logInProcess,
            ILdFunctionFactory ldFunctionFactory, IDbInsertionHelper dbInsertionHelper, IDelayService delayService)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                ldHttpHelper, logInProcess, dbInsertionHelper)
        {
            _ldFunctions = ldFunctionFactory.LdFunctions;
            _delayService = delayService;
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            #region Delete Conversation Process.
            try
            {
                var linkedinUser = (LinkedinUser) scrapeResult.ResultUser;
                GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, linkedinUser.FullName);
                var deleteApiUrl = IsBrowser
                    ? $"https://www.linkedin.com/messaging/thread/{linkedinUser.MessageThreadId}/"
                    : $"https://www.linkedin.com/voyager/api/messaging/conversations/{linkedinUser.MessageThreadId}";
                if (IsBrowser)
                {
                    _ldFunctions.GetHtmlFromUrlNormalMobileRequest(deleteApiUrl);
                    linkedinUser = BrowserUsersDetails(linkedinUser);
                }
                var responseParams = _ldFunctions.DeleteUserMessagesResponse(deleteApiUrl);
                if (responseParams != null)
                {
                    if (responseParams.Response == "")
                    {
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, linkedinUser.ProfileUrl);
                        IncrementCounters();
                        var serializedLinkedinUserDetails = JsonConvert.SerializeObject(linkedinUser);
                        DbInsertionHelper.UserScraper(scrapeResult, linkedinUser, serializedLinkedinUserDetails);
                        jobProcessResult.IsProcessSuceessfull = true;
                    }
                    else
                    {
                        var message = responseParams.Response.Contains("This profile is not available")
                            ? $"{linkedinUser.ProfileUrl} is not available"
                            : linkedinUser.ProfileUrl;
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, message);

                        jobProcessResult.IsProcessSuceessfull = false;
                    }
                }

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

        private LinkedinUser BrowserUsersDetails(LinkedinUser linkedinUser)
        {
            try
            {
                var userList = new List<LinkedinUser>();
                var response = _ldFunctions.BrowserWindow.GetPageSource();
                var reponsedetail = HtmlAgilityHelper.GetListNodesFromClassName(response,
                    "shared-title-bar__title msg-title-bar__title-bar-title");
                var publicidentifier = Utilities.GetBetween(reponsedetail.Last().InnerHtml, "href=\"/in/", "/");
                if (string.IsNullOrEmpty(publicidentifier))
                    publicidentifier = Utils.GetBetween(reponsedetail.Last().InnerHtml, "href=\"https://www.linkedin.com/in/", "\"");
                publicidentifier = string.IsNullOrEmpty(publicidentifier) ? "UNKNOWN" : publicidentifier;
                var username = Regex.Replace(
                    Utils.RemoveHtmlTags(HtmlAgilityHelper.GetStringInnerTextFromClassName(
                        reponsedetail.Last().OuterHtml, "msg-entity-lockup__entity-title-wrapper display-flex")), "<.*?>",
                    string.Empty);
                var tempLinkedInUser = new LinkedinUser(publicidentifier)
                {
                    MessageThreadId = linkedinUser.MessageThreadId,
                    FullName = username,
                    MessageContent=linkedinUser.MessageContent,
                    PublicIdentifier = publicidentifier
                };
                userList.Add(tempLinkedInUser);
                linkedinUser = tempLinkedInUser;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return linkedinUser;
        }
    }
}