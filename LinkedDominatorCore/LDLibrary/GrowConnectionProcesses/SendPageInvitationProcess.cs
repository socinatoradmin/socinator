using System;
using ThreadUtils;
using DominatorHouseCore;
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
using CefSharp;
using System.Text.RegularExpressions;
using CommonServiceLocator;
using DominatorHouseCore.BusinessLogic.Scheduler;

namespace LinkedDominatorCore.LDLibrary.GrowConnectionProcesses
{
    public class SendPageInvitationProcess : LDJobProcessInteracted<
        InteractedUsers>
    {
        private readonly IDelayService _delayService;
        private readonly ILdFunctions _ldFunctions;

        public SendPageInvitationProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ILdQueryScraperFactory queryScraperFactory,
            ILdHttpHelper ldHttpHelper, ILdLogInProcess logInProcess, ILdFunctionFactory ldFunctionFactory,
            IDbInsertionHelper dbInsertionHelper, IDelayService delayService)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                ldHttpHelper, logInProcess, dbInsertionHelper)
        {
            InviteMemberToFollowPageModel = processScopeModel.GetActivitySettingsAs<InviteMemberToFollowPageModel>();
            _ldFunctions = ldFunctionFactory.LdFunctions;
            _delayService = delayService;
        }

        public InviteMemberToFollowPageModel InviteMemberToFollowPageModel { get; set; }
        public int failedCount = 0;
        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            var linkedinUser = (LinkedinUser)scrapeResult.ResultUser;
            GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, linkedinUser.FullName+$" on ==> {scrapeResult?.QueryInfo?.QueryValue}");
            var PageId = string.Empty;
            if (ActivityType == ActivityType.EventInviter)
            {
                #region Send Event Invitation.
                PageId = Regex.Match(scrapeResult.QueryInfo.QueryValue,"[0-9]+").Value;
                var reqParam = _ldFunctions.GetInnerLdHttpHelper().GetRequestParameter();
                reqParam.ContentType = LdConstants.AcceptApplicationOrJson;
                _ldFunctions.GetInnerLdHttpHelper().SetRequestParameter(reqParam);
                var postData = IsBrowser
                        ? linkedinUser.FullName :
                        $"{{\"elements\":[{{\"inviteeMember\":\"urn:li:fsd_profile:{linkedinUser.ProfileId}\",\"genericInvitationType\":\"EVENT\"}}]}}";
                var postUrl = IsBrowser
                        ? scrapeResult.QueryInfo.QueryValue : $"https://www.linkedin.com/voyager/api/voyagerRelationshipsDashInvitations?inviter=(eventUrn:urn%3Ali%3Afsd_professionalEvent%3A{PageId})";
                _delayService.ThreadSleep(RandomUtilties.GetRandomNumber(5000, 2000));
                DominatorAccountModel.CrmUuid = scrapeResult.QueryInfo.CustomFilters;
                var sendInvitationResponse = _ldFunctions.SendEventInvitation(DominatorAccountModel, postUrl, postData).Result;
                if(sendInvitationResponse!= null && sendInvitationResponse.Success)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, linkedinUser.FullName +$" on {scrapeResult?.QueryInfo?.QueryValue}");
                    var serializedLinkedinUserDetails = JsonConvert.SerializeObject(linkedinUser);
                    DbInsertionHelper.UserScraper(scrapeResult, linkedinUser, serializedLinkedinUserDetails);
                    jobProcessResult.IsProcessSuceessfull = true;
                    failedCount = 0;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, sendInvitationResponse?.ErrorMessage);
                    if(sendInvitationResponse != null && sendInvitationResponse.IsCancelled)
                    { 
                        failedCount++;
                        if(failedCount >= 4)
                        {
                            var jobActivityConfigurationManager =
                            InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                            var moduleSetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];
                            var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                            dominatorScheduler.StopActivity(DominatorAccountModel, ActivityType.ToString(), moduleSetting.TemplateId, false);
                        }
                    }
                    jobProcessResult.IsProcessSuceessfull = false;
                }
                #endregion
                DelayBeforeNextActivity();
            }
            else
            {
                #region Send Page Invitation Process.
                try
                {
                    
                    var PageUrl = scrapeResult.QueryInfo.QueryValue;
                    PageId = PageUrl != null ? PageUrl.Contains("admin") ? Utils.GetBetween(PageUrl, "/company/", "/admin") : Utils.GetBetween(PageUrl, "/company/", "/") : "";
                    long.TryParse(PageId, out long pageId);
                    if (pageId == 0 && !IsBrowser)
                    {
                        var PageResponse = _ldFunctions.GetHtmlFromUrlNormalMobileRequest(LdConstants.GetCompanyDetailsAPI(PageId));
                        var jsonHandler = JsonJArrayHandler.GetInstance;
                        var jObject = jsonHandler.ParseJsonToJObject(PageResponse);
                        PageId = jsonHandler.GetJTokenValue(jObject, "data", "organizationDashCompaniesByUniversalName", "elements", 0, "entityUrn")?.Replace("urn:li:fsd_company:", "");
                    }
                    var postData = IsBrowser
                        ? linkedinUser.FullName :
                        $"{{\"elements\":[{{\"inviteeMember\":\"urn:li:fsd_profile:{linkedinUser.ProfileId}\",\"genericInvitationType\":\"ORGANIZATION\"}}]}}";
                        //"{\"emberEntityName\":\"growth/invitation/norm-invitation\",\"invitee\":{\"com.linkedin.voyager.growth.invitation.GenericInvitee\":{\"inviteeUrn\":\"urn:li:fs_normalized_profile:" +
                        //  linkedinUser.ProfileId + "\"}},\"inviterUrn\":\"urn:li:fs_normalized_company:" + PageId +
                        //  "\",\"trackingId\":\"" + Utils.GenerateTrackingId() + "\"}";
                    var postUrl = IsBrowser
                        ? scrapeResult.QueryInfo.QueryValue :
                        $"https://www.linkedin.com/voyager/api/voyagerRelationshipsDashInvitations?inviter=(organizationUrn:urn%3Ali%3Afsd_company%3A{pageId})";
                        //"https://www.linkedin.com/voyager/api/growth/normInvitations";
                    _delayService.ThreadSleep(RandomUtilties.GetRandomNumber(5000, 2000));
                    var sendInvitationResponse = _ldFunctions.SendPageInvitationRequest(postUrl, postData);
                    if (sendInvitationResponse != null)
                    {
                        if (sendInvitationResponse == "" || (!string.IsNullOrEmpty(sendInvitationResponse) && sendInvitationResponse.Contains("\"status\":201")))
                        {
                            GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, linkedinUser.FullName);
                            var serializedLinkedinUserDetails = JsonConvert.SerializeObject(linkedinUser);
                            DbInsertionHelper.UserScraper(scrapeResult, linkedinUser, serializedLinkedinUserDetails);
                            jobProcessResult.IsProcessSuceessfull = true;
                        }

                        else if (sendInvitationResponse == "There is no option found in this page or already sent invitation"
                            || sendInvitationResponse == "There is no option found in that Page")
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                "Sorry! there is no option to send invitation in this page");
                            jobProcessResult.IsProcessSuceessfull = false;
                            var jobActivityConfigurationManager =
                            InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                            var moduleSetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];
                            var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                            dominatorScheduler.StopActivity(DominatorAccountModel, ActivityType.ToString(), moduleSetting.TemplateId, false);
                        }
                        else if (sendInvitationResponse == "You have already invitated to this page" || sendInvitationResponse.Contains("Already Send"))
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                $"Sorry! you have already send invitation to {linkedinUser.FullName}");
                            jobProcessResult.IsProcessSuceessfull = false;
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
            }
            return jobProcessResult;
        }
    }
}