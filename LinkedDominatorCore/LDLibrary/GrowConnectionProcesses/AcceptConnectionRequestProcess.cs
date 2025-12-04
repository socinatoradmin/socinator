using System;
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
using LinkedDominatorCore.LDModel.GrowConnection;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Request;
using LinkedDominatorCore.Utility;
using System.Linq;

namespace LinkedDominatorCore.LDLibrary
{
    public class AcceptConnectionRequestProcess : LDJobProcessInteracted<
        InteractedUsers>
    {
        private readonly IDelayService _delayService;
        private readonly ILdFunctions _ldFunctions;


        public AcceptConnectionRequestProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ILdQueryScraperFactory queryScraperFactory,
            ILdHttpHelper ldHttpHelper, ILdLogInProcess logInProcess,
            ILdFunctionFactory ldFunctionFactory, IDbInsertionHelper dbInsertionHelper, IDelayService delayService)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                ldHttpHelper, logInProcess, dbInsertionHelper)
        {
            AcceptConnectionRequestModel = processScopeModel.GetActivitySettingsAs<AcceptConnectionRequestModel>();
            _ldFunctions = ldFunctionFactory.LdFunctions;
            _delayService = delayService;
        }

        public AcceptConnectionRequestModel AcceptConnectionRequestModel { get; set; }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobProcessResult jobProcessResult = null;
            #region Accept Connection Request Process.
            try
            {
                jobProcessResult = new JobProcessResult();
                var linkedinUser = (LinkedinUser) scrapeResult.ResultUser;

                GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, linkedinUser.FullName);

                #region PostString
                _delayService.ThreadSleep(RandomUtilties.GetRandomNumber(6000,3000));
                var isSuccess = IsBrowser
                    ? IsSuccessFromBrowserResponse(linkedinUser)
                    : IsSuccessFromNormalResponse(linkedinUser);
                if (isSuccess)
                {
                    var log = "{0}\t {1}\t {2}\t " + "LangKeySuccessfulTo".FromResourceDictionary() + " {3} {4}\t" +
                              CodeConstants.ActivitySuccessful;
                    if (AcceptConnectionRequestModel.IsChkAllInvitations)
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, linkedinUser.FullName);
                    else
                        GlobusLogHelper.log.Info(log, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            "LangKeyIgnoreConnectionRequest".FromResourceDictionary(), linkedinUser.FullName);
                    IncrementCounters();
                    DbInsertionHelper.AcceptConnectionRequest(scrapeResult, linkedinUser);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    if (AcceptConnectionRequestModel.IsChkAllInvitations)
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, "Accept Connection Request from",
                            linkedinUser.FullName, "");
                    else
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName,
                            "LangKeyIgnoreConnectionRequest".FromResourceDictionary(), linkedinUser.FullName, "");
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

        private bool IsSuccessFromNormalResponse(LinkedinUser linkedinUser)
        {
            string actionUrl;
            var invitationId = linkedinUser.InvitationId;
            var invitationSharedSecret = linkedinUser.InvitationSharedSecret;
            string postString = "", acceptConnectionRequestResponse = "";
            var isSuccess = false;

            try
            {
                if (AcceptConnectionRequestModel.IsChkAllInvitations)
                {
                    actionUrl =
                        $"https://www.linkedin.com/voyager/api/relationships/invitations/{invitationId}?action=accept&nc={Utils.GenerateNc()}";
                    postString = "{\"invitationSharedSecret\":\"" + invitationSharedSecret + "\",\"invitationId\":\"" +
                                 invitationId + "\"}";
                }
                else
                {
                    actionUrl =
                        "https://www.linkedin.com/voyager/api/relationships/invitations?action=closeInvitations";
                    postString =
                        "{\"inviteActionType\":\"IGNORE\",\"inviteActionData\":[{\"entityUrn\":\"urn:li:fs_relInvitation:" +
                        invitationId + "\",\"validationToken\":\"" + invitationSharedSecret +
                        "\",\"genericInvitation\":false}]}";
                }

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                acceptConnectionRequestResponse = _ldFunctions.ByteEncodedPostResponse(actionUrl, postString);
                //Accept Invitation of Type Content_Series.
                if (string.IsNullOrEmpty(acceptConnectionRequestResponse))
                    acceptConnectionRequestResponse = AcceptConnectionOfTypeContentSeries(invitationSharedSecret,invitationId);
                var acceptResponse = "value\":{\"invitationType\":\"ACCEPTED\"";
                isSuccess = AcceptConnectionRequestModel.IsChkAllInvitations &&
                            (acceptConnectionRequestResponse.Contains(acceptResponse) ||
                             acceptConnectionRequestResponse.Contains("\"invitationType\": \"ACCEPTED\",")||
                             acceptConnectionRequestResponse.Contains("\"invitationState\":\"PENDING\""))
                            || AcceptConnectionRequestModel.IsChkIgnoreAllInvitations &&
                            (acceptConnectionRequestResponse.Contains(
                                "{\"value\":{\"statusCodeMap\":{\"urn:li:fs_relInvitation:" +
                                invitationId + "\":200}}}")|| acceptConnectionRequestResponse.Contains("\"invitationState\":\"PENDING\""));
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }

            return isSuccess;
        }

        private string AcceptConnectionOfTypeContentSeries(string invitationSharedSecret, string invitationId)
        {
            var actionUrl = $"https://www.linkedin.com/voyager/api/voyagerRelationshipsDashInvitations/urn%3Ali%3Afsd_invitation%3A{invitationId}?action=accept";
            var postString = $"{{\"sharedSecret\":\"{invitationSharedSecret}\",\"invitationType\":\"CONTENT_SERIES\"}}";
            return _ldFunctions.ByteEncodedPostResponse(actionUrl, postString);
        }
        private bool IsSuccessFromBrowserResponse(LinkedinUser linkedinUser)
        {
            var isSuccess = false;
            try
            {
                var automationExtension = new BrowserAutomationExtension(_ldFunctions.BrowserWindow);
                if (AcceptConnectionRequestModel.IsChkAllInvitations)
                {
                    var nodes = HtmlAgilityHelper.GetListNodesFromAttibute(linkedinUser.NodeResponse,"div", AttributeIdentifierType.DataViewName,null, "invitation-action");
                    var target = nodes.FirstOrDefault(x => x.InnerText == "Accept");
                    var className = automationExtension.GetPath(target.InnerHtml, HTMLTags.Button,
                        AttributeIdentifierType.AriaLabel, "Accept");
                    isSuccess = automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToClick,HTMLTags.Button,HTMLTags.HtmlAttribute.AriaLabel, className)).Success;
                }
                else
                {
                    var nodes = HtmlAgilityHelper.GetListNodesFromAttibute(linkedinUser.NodeResponse, "div", AttributeIdentifierType.DataViewName, null, "invitation-action");
                    var target = nodes.FirstOrDefault(x => x.InnerText == "Ignore");
                    var className = automationExtension.GetPath(target.InnerHtml, HTMLTags.Button,
                        AttributeIdentifierType.AriaLabel, "Ignore");
                    var id = automationExtension.GetPath(linkedinUser.NodeResponse,HTMLTags.Button,
                        AttributeIdentifierType.Id, "Ignore");
                    isSuccess = automationExtension.ExecuteScript(string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorToClick, HTMLTags.Button, HTMLTags.HtmlAttribute.AriaLabel, className)).Success;
                }
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }

            return isSuccess;
        }
    }
}