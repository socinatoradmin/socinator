using System;
using ThreadUtils;
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
using Newtonsoft.Json;
using EmbeddedBrowser;
using System.Windows;
using System.Threading;
using HtmlAgilityPack;
using DominatorHouseCore;

namespace LinkedDominatorCore.LDLibrary.GrowConnectionProcesses
{

    public class SendGroupMemberInvitationProcess : LDJobProcessInteracted<
        InteractedUsers>
    {
        private readonly IDelayService _delayService;
        private readonly ILdFunctions _ldFunctions;
        private BrowserWindow _browserWindow;
        private BrowserAutomationExtension automationExtension;

        public SendGroupMemberInvitationProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ILdQueryScraperFactory queryScraperFactory,
            ILdHttpHelper ldHttpHelper, ILdLogInProcess logInProcess, ILdFunctionFactory ldFunctionFactory,
            IDbInsertionHelper dbInsertionHelper, IDelayService delayService)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                ldHttpHelper, logInProcess, dbInsertionHelper)
        {
            SendInvitationToGroupMemberModel = processScopeModel.GetActivitySettingsAs<SendInvitationToGroupMemberModel>();
            CurrentActivityType = ActivityType.SendPageInvitations.ToString();
            _ldFunctions = ldFunctionFactory.LdFunctions;
            _delayService = delayService;
        }

        public SendInvitationToGroupMemberModel SendInvitationToGroupMemberModel { get; set; }
        public string CurrentActivityType { get; set; }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            #region Send Group Member Invitation Process.
            try
            {
                var linkedinUser = (LinkedinUser)scrapeResult.ResultUser;
                GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, linkedinUser.FullName);
                

                if (_browserWindow == null)
                    LDAccountsBrowserDetails.GetInstance().AccountBrowserCollections
                        .TryGetValue(DominatorAccountModel.UserName, out _browserWindow);
                automationExtension = new BrowserAutomationExtension(_browserWindow);


                if (_browserWindow != null)
                {
                    _browserWindow.Browser.Load(scrapeResult.QueryInfo.QueryValue);
                    _delayService.ThreadSleep(15000);
                }

                var sendInvitationResponse = SendGroupMemberInvitationRequest(linkedinUser.FullName);

                
                if (sendInvitationResponse == null)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, linkedinUser.FullName);
                    var serializedLinkedinUserDetails = JsonConvert.SerializeObject(linkedinUser);
                    DbInsertionHelper.UserScraper(scrapeResult, linkedinUser, serializedLinkedinUserDetails);
                    jobProcessResult.IsProcessSuceessfull = true;
                }

                else if (sendInvitationResponse == "There is no option found in that Page")
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        "Sorry! there is no option to send invitation in this Group member");
                }
                else if (sendInvitationResponse == "You have already invitated to this Group member")
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        $"Sorry! you have already send invitation to {linkedinUser.FullName}");
                }
                else
                {
                    var message = sendInvitationResponse.Contains("This profile is not available")
                        ? $"{linkedinUser.ProfileUrl} is not available"
                        : linkedinUser.ProfileUrl;
                    GlobusLogHelper.log.Info(Log.ActivityFailed,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, message);

                    jobProcessResult.IsProcessSuceessfull = false;
                }
                DelayBeforeNextActivity();
            }
            catch (OperationCanceledException)
            {
                CloseBrowser();
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception)
            {
                CloseBrowser();
            }
            #endregion
            return jobProcessResult;
        }


        public string SendGroupMemberInvitationRequest(string poststring)
        {
            automationExtension.ExecuteScript(AttributeIdentifierType.Xpath, "//span[text()='Invite connections']");
            var Enternameclick = automationExtension.GetXAndY(LDClassesConstant.GrowConnection.SearchUserClass, AttributeIdentifierType.ClassName);
            _browserWindow.MouseClick(Enternameclick.Key + 15, Enternameclick.Value + 15, delayAfter: 5);
            _browserWindow.EnterChars(poststring, delayAtLast: 5);

            var Group = _browserWindow.GetPageSource();
            if (Group.Contains("Invited"))
            {
                return "You have already invitated to this Group member";
            }
            var selecteuser = automationExtension.GetXAndY(LDClassesConstant.GrowConnection.SelectUserClass, AttributeIdentifierType.ClassName);
            _browserWindow.MouseClick(selecteuser.Key + 15, selecteuser.Value + 15, delayAfter: 5);
            var success = automationExtension
                   .ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Span}[text()='Invite 1']").Success;
            Thread.Sleep(10000);
            if (success)
            {
                automationExtension
                   .ExecuteScript(AttributeIdentifierType.Xpath, $"//{HTMLTags.Span}[text()='No thanks']");
                var pageResponse = _browserWindow.GetPageSource();
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(pageResponse);
                var Caption = Utils.RemoveHtmlTags(HtmlAgilityHelper.GetStringInnerHtmlFromClassName("",
                    LDClassesConstant.GrowConnection.SuccessCaptionMessageClass, htmlDoc));
                if (Caption == "1 person invited to follow Page")
                    return "";
                return null;

            }
            else
            {
                return "There is no option found in that Page";
            }
        }

        public void CloseBrowser()
        {
            try
            {               
                if (_browserWindow != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _browserWindow.Close();
                        _browserWindow.Dispose();
                    });
                    LDAccountsBrowserDetails.GetInstance().AccountBrowserCollections
                        .Remove(DominatorAccountModel.UserName);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

}
