using System;
using System.IO;
using System.Net;
using System.Threading;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDModel.Messenger;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Request;
using LinkedDominatorCore.Utility;
using DominatorHouseCore.BusinessLogic.Scheduler;
using System.Linq;
using System.Collections.Generic;

namespace LinkedDominatorCore.LDLibrary.MessengerProcesses
{
    public class BroadcastMessagesProcess : LDJobProcessInteracted<
        InteractedUsers>
    {
        private readonly IDelayService _delayService;
        private readonly ILdFunctions _ldFunctions;
        private int failedConnectionCount;
        private ILdLogInProcess ldLogInProcess;
        public BroadcastMessagesProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ILdQueryScraperFactory queryScraperFactory,
            ILdHttpHelper ldHttpHelper, ILdLogInProcess logInProcess,
            ILdFunctionFactory ldFunctionFactory, IDbInsertionHelper dbInsertionHelper, IDelayService delayService)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                ldHttpHelper, logInProcess, dbInsertionHelper)
        {
            BroadcastMessagesModel = processScopeModel.GetActivitySettingsAs<BroadcastMessagesModel>();
            _ldFunctions = ldFunctionFactory.LdFunctions;
            _delayService = delayService;
            ldLogInProcess = logInProcess;
            blackListWhitelistHandler =
                new BlackListWhiteListHandler(ModuleSetting, DominatorAccountModel, ActivityType);
        }

        private BlackListWhiteListHandler blackListWhitelistHandler { get; }

        public BroadcastMessagesModel BroadcastMessagesModel { get; set; }
        public string CurrentActivityType { get; set; }


        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            #region BroadCast Message Process.
            try
            {
                var objLinkedinUser = (LinkedinUser)scrapeResult.ResultUser;

                if (BroadcastMessagesModel.IsChkSkipBlackListedUser && BroadcastMessagesModel.IsChkPrivateBlackList || BroadcastMessagesModel.IsChkGroupBlackList)
                {
                    var blackListUser = blackListWhitelistHandler.GetBlackListUsers();
                    if(blackListUser != null && blackListUser.Count > 0 && blackListUser.Any(user=>user.Equals(objLinkedinUser.PublicIdentifier)))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                "Skip User " + objLinkedinUser.PublicIdentifier + ", Present in Blacklist ");
                        return jobProcessResult;
                    }
                }
                if (string.IsNullOrEmpty(objLinkedinUser.ProfileId))
                {
                    if (string.IsNullOrEmpty(objLinkedinUser.PublicIdentifier))
                        objLinkedinUser.PublicIdentifier = Utils.GetBetween($"{objLinkedinUser.ProfileUrl}/",
                            "www.linkedin.com/in/", "/");
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                           | SecurityProtocolType.Tls11
                                                           | SecurityProtocolType.Tls12
                                                           | SecurityProtocolType.Ssl3;
                    var reponse = _ldFunctions.GetInnerHttpHelper().GetRequest(
                        $"https://www.linkedin.com/voyager/api/identity/profiles/{objLinkedinUser.PublicIdentifier}");
                    var jArr = new JsonHandler(reponse.Response);
                    objLinkedinUser.ProfileId = jArr.GetElementValue("entityUrn").Replace("urn:li:fs_miniProfile:", "")
                        .Replace("urn:li:fs_profile:", "");
                }

                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleSetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (!string.IsNullOrEmpty(CampaignId) &&
                    IsCampaignWiseUnique(BroadcastMessagesModel, objLinkedinUser, moduleSetting))
                {
                    jobProcessResult.IsProcessCompleted = false;
                    return jobProcessResult;
                }

                if (!BroadcastMessagesModel.IsFollower && !BroadcastMessagesModel.IsGroup && !new GetDetailedUserInfo(_delayService).IsFirstConnection(
                        DominatorAccountModel,
                        DbAccountService,
                        objLinkedinUser, ActivityType, _ldFunctions))
                    return jobProcessResult;

                if (FilterUser(objLinkedinUser, jobProcessResult))
                    return jobProcessResult;

                string finalMessage;
                var sendMessageToConnectionResponse =
                    MessageProcess(objLinkedinUser, jobProcessResult, out finalMessage);

                if (!string.IsNullOrWhiteSpace(sendMessageToConnectionResponse) &&
                    sendMessageToConnectionResponse.Contains("{\"value\":{\"createdAt\":"))
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objLinkedinUser.FullName);
                    IncrementCounters();
                    DbInsertionHelper.BroadcastMessages(scrapeResult, objLinkedinUser, finalMessage,
                        ActivityStatus.Success.ToString());
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    ++failedConnectionCount;
                    if (BroadcastMessagesModel.IsSaveFailedActivity)
                        DbInsertionHelper.BroadcastMessages(scrapeResult, objLinkedinUser, finalMessage,
                            ActivityStatus.Failed.ToString());

                    var message = string.IsNullOrWhiteSpace(sendMessageToConnectionResponse)
                        ||!sendMessageToConnectionResponse.Contains("Not found suitable message")
                        ? $"Can't Send Message To {objLinkedinUser.FullName}"
                        : sendMessageToConnectionResponse;

                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,message);
                    jobProcessResult.IsProcessSuceessfull = false;
                    var ldCampaignInteractionDetails =
                        InstanceProvider.GetInstance<ICampaignInteractionDetails>();
                    ldCampaignInteractionDetails.RemoveIfExist(SocialNetworks.LinkedIn, CampaignId,
                        objLinkedinUser.ProfileUrl);

                    if (BroadcastMessagesModel.IsStopSendMessageOnFailed &&
                       BroadcastMessagesModel.StopSendMessageOnCount <= failedConnectionCount)
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

        private bool FilterUser(LinkedinUser objLinkedinUser, JobProcessResult jobProcessResult)
        {
            #region Filters After Visiting Profile

            try
            {
                if (LdUserFilterProcess.IsUserFilterActive(BroadcastMessagesModel.LDUserFilterModel) &&
                    !LdUserFilterProcess.GetFilterStatus(objLinkedinUser.ProfileUrl,
                        BroadcastMessagesModel.LDUserFilterModel,
                        _ldFunctions))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        "[ " + objLinkedinUser.FullName + " ] is not a valid user according to the filter.");
                    jobProcessResult.IsProcessSuceessfull = false;
                    Thread.Sleep(5000);
                    return true;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion

            return false;
        }

        private string MessageProcess(LinkedinUser objLinkedinUser, JobProcessResult jobProcessResult,
            out string finalMessage)
        {
            #region  Variables Initializations

            var textMessage = string.Empty;
            var imageSource = string.Empty;
            #endregion

            GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, "send message to ", objLinkedinUser.FullName);

            var filterMessage = new FilterMessage(BroadcastMessagesModel, _delayService);
            finalMessage = filterMessage.GetMessage(DominatorAccountModel, objLinkedinUser, textMessage, _ldFunctions,
                ref imageSource);
            var Medias = new List<string>();
            try
            {
                if (string.IsNullOrEmpty(objLinkedinUser.SelectedSource))
                    objLinkedinUser.SelectedSource = "Custom Group Url";
                Medias = BroadcastMessagesModel.LstDisplayManageMessagesModel?.FirstOrDefault(x => x.SelectedQuery.Any(y => y.Content.QueryValue.ToString() == objLinkedinUser?.SelectedSource))?.Medias?.Select(t=>t.MediaPath)?.ToList();
            }
            catch { Medias = new List<string>(); }
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            return IsBrowser ?
                _ldFunctions.UploadImageAndGetContentIdForMessaging(
                    BroadcastMessagesModel.IsGroup ? objLinkedinUser.FullName : objLinkedinUser.ProfileUrl,
                    string.IsNullOrEmpty(imageSource) ? null : new FileInfo(imageSource), finalMessage,Medias)
                :_ldFunctions.BroadCastMessage(imageSource,objLinkedinUser, BroadcastMessagesModel.IsGroup,finalMessage,string.Empty, Medias);
        }

        public bool IsCampaignWiseUnique(BroadcastMessagesModel broadcastMessagesModel, LinkedinUser objLinkedinUser,
            ModuleConfiguration moduleSetting)
        {
            if (moduleSetting == null || !BroadcastMessagesModel.IsCampaignWiseUniqueChecked ||
                !moduleSetting.IsTemplateMadeByCampaignMode)
                return false;

            #region Check in LDCampaignInteractionDetails

            try
            {
                var ldCampaignInteractionDetails = InstanceProvider.GetInstance<ICampaignInteractionDetails>();
                ldCampaignInteractionDetails.AddInteractedData(SocialNetworks.LinkedIn, CampaignId,
                    objLinkedinUser.ProfileUrl);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return true;
            }

            #endregion

            return false;
        }
    }
}