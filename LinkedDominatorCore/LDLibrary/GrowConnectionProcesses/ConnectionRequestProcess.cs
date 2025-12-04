using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;
using DominatorHouseCore.Enums.LdQuery;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDModel.GrowConnection;
using LinkedDominatorCore.LDModel.LDUtility;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Request;
using LinkedDominatorCore.Utility;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using ThreadUtils;

// ReSharper disable once CheckNamespace
namespace LinkedDominatorCore.LDLibrary
{
    public class ConnectionRequestProcess : LDJobProcessInteracted<InteractedUsers>
    {
        private readonly IDelayService _delayService;
        private ILdFunctions _ldFunctions;

        private bool _isAlreadyGetConnectionCount;
        private JobProcessResult _jobProcessResult;
        private int _totalConnectionRequestSent;
        private int failedConnectionCount;
        private int displayCount;
       

        public ConnectionRequestProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ILdQueryScraperFactory queryScraperFactory,
            ILdHttpHelper ldHttpHelper, ILdLogInProcess logInProcess, ILdFunctionFactory ldFunctionFactory,
            IDbInsertionHelper dbInsertionHelper, IDelayService delayService)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                ldHttpHelper, logInProcess, dbInsertionHelper)
        {
            ConnectionRequestModel = processScopeModel.GetActivitySettingsAs<ConnectionRequestModel>();
            CurrentActivityType = ActivityType.ConnectionRequest.ToString();
            _ldFunctions = ldFunctionFactory.LdFunctions;
            _delayService = delayService;
        }

        public ConnectionRequestModel ConnectionRequestModel { get; set; }
        public string CurrentActivityType { get; set; }
        public string personalNote;
        /// <summary>
        /// </summary>
        /// <param name="scrapeResult"></param>
        /// <returns></returns>
        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            _jobProcessResult = new JobProcessResult();
            #region Connection Request Process.
            var ldCampaignInteractionDetails = InstanceProvider.GetInstance<ICampaignInteractionDetails>();
         
            IResponseParameter sendConnectionRequestResponse = null;
            var finalPersonalNote = string.Empty;

            try
            {
                var objLinkedinUser = (LinkedinUser) scrapeResult.ResultUser;
                var ldDataHelper = LdDataHelper.GetInstance;
                var IsSalesProfile = ldDataHelper.IsSalesProfile(objLinkedinUser.ProfileUrl);
                if (objLinkedinUser != null && IsSalesProfile)
                {
                    _ldFunctions.SetWenRequestparamtersForsalesUrl(objLinkedinUser.ProfileUrl, true);
                    ldDataHelper.UpdateSalesUserProfileDetails(_ldFunctions, ldDataHelper.GetAuthTokenFromSalesProfileUrl(objLinkedinUser.ProfileUrl), ref objLinkedinUser);
                }
                if (FilterBlackListedUser(objLinkedinUser) && !(_jobProcessResult.IsProcessSuceessfull = false))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.LinkedIn, DominatorAccountModel.UserName
                        , UserType.BlackListedUser,
                        string.Format("LangKeyFilteredUser".FromResourceDictionary(),
                            objLinkedinUser.PublicIdentifier));
                    return _jobProcessResult;
                }

                // filtering user for campaign wise unique
                if (new LdUniqueHandler().IsCampaignWiseUnique(new UniquePreRequisticProperties
                {
                    AccountModel = DominatorAccountModel,
                    ActivityType = ActivityType,
                    CampaignId = CampaignId,
                    IsUniqueOperationChecked = ConnectionRequestModel.IsUniqueOperationChecked,
                    ProfileUrl =
                        scrapeResult.QueryInfo.QueryType ==
                        EnumUtility.GetQueryFromEnum(LDGrowConnectionUserQueryParameters.Email)
                            ? objLinkedinUser.EmailAddress
                            : objLinkedinUser.ProfileUrl
                }))
                    return _jobProcessResult;


                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleSetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];
                var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                var linkedinConfig =
                    genericFileManager.GetModel<LinkedInModel>(ConstantVariable.GetOtherLinkedInSettingsFile());

                // filtering user for globally unique
                if (new LdUniqueHandler().IsGlobalUnique(new UniquePreRequisticProperties
                {
                    AccountModel = DominatorAccountModel,
                    ActivityType = ActivityType,
                    CampaignId = CampaignId,
                    IsUniqueOperationChecked = ConnectionRequestModel.IsUniqueOperationChecked,
                    ProfileUrl =
                        scrapeResult.QueryInfo.QueryType ==
                        EnumUtility.GetQueryFromEnum(LDGrowConnectionUserQueryParameters.Email)
                            ? objLinkedinUser.EmailAddress
                            : objLinkedinUser.ProfileUrl
                }))
                    return _jobProcessResult;


                var postString = "";
                var sendRequestActionUrl = "";
                var normalProfileUrl = objLinkedinUser.ProfileUrl;
                if (scrapeResult.QueryInfo.QueryType ==
                    EnumUtility.GetQueryFromEnum(LDGrowConnectionUserQueryParameters.Email))
                {
                    if (IsBrowser)
                    {
                        _ldFunctions.GetRequestUpdatedUserAgent("https://www.linkedin.com/mynetwork/");
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.StartedActivity,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            objLinkedinUser.EmailAddress);
                        sendRequestActionUrl =
                            "https://www.linkedin.com/voyager/api/growth/normInvitations?action=batchCreate";
                        postString = GetPostString(_ldFunctions, objLinkedinUser.EmailAddress);
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        if (string.IsNullOrEmpty(postString))
                            return _jobProcessResult;
                    }
                }
                else
                {
                    var userScraperDetailedInfo = DetailsFetcher.GetUserScraperDetailedInfo(DominatorAccountModel);
                    string firstName = string.Empty, lastName = string.Empty, fullName = string.Empty;

                    var fromFullName = DominatorAccountModel.AccountBaseModel.UserFullName;
                    fullName = RecipientDetails(fullName, objLinkedinUser, ref firstName, ref lastName);

                    #region Filters After Visiting Profile

                    try
                    {
                        if (LdUserFilterProcess.IsUserFilterActive(ConnectionRequestModel.LDUserFilterModel))
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,DominatorAccountModel.AccountBaseModel.AccountNetwork,DominatorAccountModel.AccountBaseModel.UserName, ActivityType,"Filtering User ==> " + objLinkedinUser.FullName);
                            var isValidUser = LdUserFilterProcess.GetFilterStatus(objLinkedinUser.ProfileUrl,
                                ConnectionRequestModel.LDUserFilterModel, _ldFunctions);
                            if (!isValidUser)
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    "[ " + objLinkedinUser.FullName +
                                    " ] is not a valid user according to the filter.");
                                return _jobProcessResult;
                            }else
                                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"All Filter Matched,Proceeding For {ActivityType} Of {objLinkedinUser.FullName}");
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    #endregion

                    #region Check these conditions before sending request

                    if (!string.IsNullOrEmpty(objLinkedinUser.ProfileUrl))

                    {
                        var actionUrl = "";
                        if (scrapeResult.QueryInfo.QueryType ==
                            EnumUtility.GetQueryFromEnum(LDGrowConnectionUserQueryParameters.JobScraperCampaign))
                            actionUrl = "https://www.linkedin.com/voyager/api/identity/profiles/" +
                                        objLinkedinUser.ProfileId + "/profileActions";
                        else
                            //profileId can also be replaced by publicIdentifier
                            actionUrl = !string.IsNullOrEmpty(objLinkedinUser.PublicIdentifier) &&
                                        !objLinkedinUser.ProfileUrl.Contains("https://www.linkedin.com/sales/")
                                ? "https://www.linkedin.com/voyager/api/identity/profiles/" +
                                  objLinkedinUser.PublicIdentifier?.Trim('/') + "/profileActions"
                                : "https://www.linkedin.com/voyager/api/identity/profiles/" +
                                  objLinkedinUser.ProfileId + "/profileActions";


                        if (IsBrowser && IsSalesProfile)
                        {
                            normalProfileUrl = objLinkedinUser.ProfileUrl;
                        }

                        IResponseParameter responseForProfileAction = null;
                        if (!IsSalesProfile && !ConnectionRequestModel.IsCheckedWithoutVisiting && !string.IsNullOrEmpty(normalProfileUrl))
                            responseForProfileAction = IsBrowser
                                ? new ResponseParameter
                                {
                                    Response = _ldFunctions.GetRequestUpdatedUserAgent(normalProfileUrl)
                                }
                                : HttpHelper.HandleGetResponse(actionUrl);

                        if (!string.IsNullOrEmpty(responseForProfileAction?.Response) &&
                            !responseForProfileAction.Response.Contains("profile.actions.Connect"))
                        {
                            if (responseForProfileAction.Response.Contains("profile.actions.InvitationPending") && responseForProfileAction.Response.Contains("Pending"))
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    "already sent connection request to " + objLinkedinUser.FullName +
                                    " from outside of the software");
                                _delayService.ThreadSleep(3000);
                                _jobProcessResult.IsProcessSuceessfull = false;
                                return _jobProcessResult;
                            }

                            if (responseForProfileAction.Response.Contains("profile.actions.Message") || responseForProfileAction.Response.Contains("message-anywhere-button pv-s-profile-actions pv-s-profile-actions--message ml2 artdeco-button artdeco-button--2 artdeco-button--primary"))
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    objLinkedinUser.FullName + "is already 1st connection");
                                _delayService.ThreadSleep(3000);
                                _jobProcessResult.IsProcessSuceessfull = false;
                                return _jobProcessResult;
                            }
                            if(responseForProfileAction.Response.Contains("Connection sent:"))
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    "Connection Already Has Been Sent To " + objLinkedinUser.FullName);
                                _delayService.ThreadSleep(3000);
                                _jobProcessResult.IsProcessSuceessfull = false;
                                return _jobProcessResult;
                            }
                            #region Extra Filtrations to check profile status for connection request sending

                            if (responseForProfileAction.Response.Contains("profile.actions.Accept") &&
                                !responseForProfileAction.Response.Contains("profile.actions.Connect"))
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    "has received connection invitation from " + objLinkedinUser.FullName);
                                _delayService.ThreadSleep(3000);
                                _jobProcessResult.IsProcessSuceessfull = false;
                                return _jobProcessResult;
                            }

                            if (responseForProfileAction.Response.Contains("profile.actions.Follow") &&
                                !responseForProfileAction.Response.Contains("profile.actions.Connect"))
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    objLinkedinUser.FullName + "can only be followed");
                                _delayService.ThreadSleep(3000);
                                _jobProcessResult.IsProcessSuceessfull = false;
                                return _jobProcessResult;
                            }

                            if (responseForProfileAction.Response.Contains("Upgrade for full name"))
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    "please upgrade this account to send connection request!");
                                Thread.Sleep(3000);
                                _jobProcessResult.IsProcessSuceessfull = false;
                                return _jobProcessResult;
                            }

                            #endregion

                            if (responseForProfileAction.Response.Contains("emailRequired\":true"))
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    objLinkedinUser.FullName + " Email is required to connect");
                        }
                    }

                    #endregion

                    GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objLinkedinUser.FullName);


                    //  personal note                    
                    if (ConnectionRequestModel.LstDisplayManagePersonalNoteModel.Count > 0)
                        personalNote = ConnectionRequestModel.LstDisplayManagePersonalNoteModel.GetRandomItem().PersonalNoteText;

                    #region IsChkSpintaxChecked

                    if (ConnectionRequestModel != null && ConnectionRequestModel.IsChkSpintaxChecked)
                    {
                        
                        
                        var lstMessages = SpinTexHelper.GetSpinMessageCollection(personalNote);

                        try
                        {
                            finalPersonalNote =
                                lstMessages[RandomUtilties.GetRandomNumber(lstMessages.Count-1)];
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }
                    else
                    {
                        finalPersonalNote = personalNote;
                    }

                    #endregion

                    #region IsChkTagChecked

                    try
                    {
                        if (finalPersonalNote?.Length > 200 && !string.IsNullOrEmpty(finalPersonalNote))
                        {
                            ++displayCount;
                            if (displayCount <= 1)

                                GlobusLogHelper.log.Info("PersonalNote is more than 200 character.Software will reduce it to 300 character.");
                            finalPersonalNote = finalPersonalNote.Substring(0, 200);
                        }

                        if (ConnectionRequestModel != null && ConnectionRequestModel.IsChkTagChecked &&
                            finalPersonalNote != null)
                        {
                            #region FinalGreeting With Recipient's Tags
                            finalPersonalNote = Regex.Replace(finalPersonalNote, "(<First Name>|<First name>|<first Name>|<first name>){1}", firstName);
                            finalPersonalNote = Regex.Replace(finalPersonalNote, "(<Last Name>|<Last name>|<last Name>|<last name>){1}", lastName);
                            finalPersonalNote = Regex.Replace(finalPersonalNote, "(<Full Name>|<Full name>|<full Name>|<full name>){1}", fullName);
                            #endregion

                            #region FinalGreeting With Sender's Tags
                            finalPersonalNote = Regex.Replace(finalPersonalNote, "(<From First Name>|<From first name>|<from first name>){1}",userScraperDetailedInfo.Firstname);
                            finalPersonalNote = Regex.Replace(finalPersonalNote, "(<From Last Name>|<From last name>|<from last name>){1}",userScraperDetailedInfo.Lastname);
                            finalPersonalNote = Regex.Replace(finalPersonalNote, "(<From Full Name>|<From full name>|<from full name>){1}", fromFullName);
                            #endregion

                            if (scrapeResult.QueryInfo.QueryType ==
                                EnumUtility.GetQueryFromEnum(LDGrowConnectionUserQueryParameters.JobScraperCampaign))
                            {
                                finalPersonalNote =
                                    finalPersonalNote.Replace("<JobTitle>", objLinkedinUser.HeadlineTitle);
                                finalPersonalNote = finalPersonalNote.Replace("<JobLocation>", objLinkedinUser.Location)
                                    ?.Replace("United States".ToLower(), "US")
                                    ?.Replace("United Kingdom".ToLower(), "UK")?.Replace("United States", "US")
                                    ?.Replace("United Kingdom", "UK");
                                finalPersonalNote = finalPersonalNote.Replace("<JobPosterFirstName>", firstName);
                                finalPersonalNote = finalPersonalNote.Replace("<JobPosterLastName>", lastName);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    #endregion

                    #region PostString and ActionUrl

                    if (!string.IsNullOrEmpty(finalPersonalNote) && !IsBrowser)
                    {
                        if (finalPersonalNote.Contains("\\"))
                            finalPersonalNote = finalPersonalNote.Replace("\\", "\\\\");
                        finalPersonalNote = finalPersonalNote.Replace("\r", "").Replace("\n", "\\n")
                            .Replace("\"", "\\\"");
                    }
                    sendRequestActionUrl = IsSalesProfile ? "https://www.linkedin.com/sales-api/salesApiConnection?action=connectV2" :  "https://www.linkedin.com/voyager/api/voyagerRelationshipsDashMemberRelationships?action=verifyQuotaAndCreateV2&decorationId=com.linkedin.voyager.dash.deco.relationships.InvitationCreationResultWithInvitee-2";
                    postString = IsSalesProfile ? $"{{\"member\":\"{objLinkedinUser.ProfileId}\",\"message\":\"{finalPersonalNote}\"}}" 
                        : string.IsNullOrEmpty(finalPersonalNote) ?
                        $"{{\"invitee\":{{\"inviteeUnion\":{{\"memberProfile\":\"urn:li:fsd_profile:{objLinkedinUser.ProfileId}\"}}}}}}"
                        : $"{{\"invitee\":{{\"inviteeUnion\":{{\"memberProfile\":\"urn:li:fsd_profile:{objLinkedinUser.ProfileId}\"}}}},\"customMessage\":\"{finalPersonalNote}\"}}";

                    #endregion
                }
                #region SendConnectionRequest_Response and Activities After

                if (IsBrowser && ConnectionRequestModel.IsCheckedWithoutVisiting && scrapeResult.QueryInfo.QueryType!="Profile Url")
                    sendConnectionRequestResponse =
                        _ldFunctions.SendConnectionRequestWithoutVistingProfile(finalPersonalNote,
                            objLinkedinUser.NodeId, "");
                else
                    sendConnectionRequestResponse = IsBrowser
                        ? _ldFunctions.SendConnectionRequestAlternativeMethod(finalPersonalNote,string.IsNullOrEmpty(normalProfileUrl)? objLinkedinUser.ProfileUrl : normalProfileUrl, "")
                        : SendConnectionRequestResponse(scrapeResult, sendConnectionRequestResponse,
                            sendRequestActionUrl, postString, objLinkedinUser, finalPersonalNote);

                if (ConnectionSuccessForSales(sendConnectionRequestResponse?.Response) || sendConnectionRequestResponse?.Response == string.Empty 
                    || sendConnectionRequestResponse?.Response==LdConstants.InvitationSentSuccessFully
                    || (!string.IsNullOrEmpty(sendConnectionRequestResponse?.Response) && sendConnectionRequestResponse.Response.Contains("InvitationCreationResultWithInvitee")))
                {
                    //resetting continous failed count
                    failedConnectionCount = 0;
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objLinkedinUser.FullName);

                    IncrementCounters();
                    finalPersonalNote = finalPersonalNote?.Replace("\\n", " \n");
                    DbInsertionHelper.ConnectionRequest(scrapeResult, objLinkedinUser, finalPersonalNote);
                    _jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    var message = "";
                    if (sendConnectionRequestResponse?.Response == "User is already your connection" ||
                        sendConnectionRequestResponse?.Response == "An invitation has been sent")
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            sendConnectionRequestResponse.Response);
                        return JobProcessResult;
                    }
                    if(sendConnectionRequestResponse?.Response == null || 
                       sendConnectionRequestResponse?.Response== "You’ve reached the weekly invitation limit" || 
                       sendConnectionRequestResponse?.Response== "Vous avez atteint la limite d’invitations hebdomadaire" ||
                       sendConnectionRequestResponse?.Response== "You’re out of invitations for now" || 
                       sendConnectionRequestResponse.Response.Contains("Too Many Requests"))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            "You’ve reached the weekly invitation limit");
                        var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                        dominatorScheduler.StopActivity(DominatorAccountModel,ActivityType.ToString(), moduleSetting.TemplateId, false);
                        return JobProcessResult;
                    }
                    else
                    {
                        ++failedConnectionCount;
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        DetailsFetcher.DebugLogTrack(sendConnectionRequestResponse, "Failed Connection Request");
                        message = sendConnectionRequestResponse?.Exception?.Message;
                        if (!string.IsNullOrEmpty(message) && message.Contains("(429)"))
                        {
                            message = "LangKeyTooManyRequestsSentsFromThisAccount".FromResourceDictionary();
                            scrapeResult.IsAccountLocked = true;
                        }
                        else
                        {
                            message = string.Format("LangKeyFailedToSendConnectionRequest".FromResourceDictionary(),
                                objLinkedinUser.FullName);
                        }
                        message = (!string.IsNullOrEmpty(sendConnectionRequestResponse.Response) && sendConnectionRequestResponse.Response.Contains(LdConstants.YourInvitationToConnectWasNotSent)) ? $"{message} ==>{LdConstants.AlreadySentConnectionRequest}" : message;
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, message);
                    }

                    #region Remove from CampaignInteractedUtility if failed operation

                    if (ConnectionRequestModel.IsUniqueOperationChecked && moduleSetting.IsTemplateMadeByCampaignMode)
                    {
                        if (scrapeResult.QueryInfo.QueryType ==
                            EnumUtility.GetQueryFromEnum(LDGrowConnectionUserQueryParameters.Email))
                        {
                            AddInteractedData(ldCampaignInteractionDetails, objLinkedinUser.EmailAddress);
                            GlobusLogHelper.log.Info(Log.ActivityFailed,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                objLinkedinUser.EmailAddress, message);
                        }
                        else
                        {
                            AddInteractedData(ldCampaignInteractionDetails, objLinkedinUser.ProfileUrl);
                            GlobusLogHelper.log.Info(Log.ActivityFailed,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objLinkedinUser.FullName,
                                message);
                        }
                    }

                    #endregion

                    #region Remove from GlobalInteractedUtility if failed operation

                    if (linkedinConfig.IsEnableSendConnectionRequestToDifferentUsers)
                    {
                        #region Check in LDGlobalInteractionDetails

                        if (scrapeResult.QueryInfo.QueryType ==
                            EnumUtility.GetQueryFromEnum(LDGrowConnectionUserQueryParameters.Email))
                        {
                            RemoveIfExist(ldCampaignInteractionDetails, objLinkedinUser.EmailAddress);
                            GlobusLogHelper.log.Info(Log.ActivityFailed,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                objLinkedinUser.EmailAddress, message);
                        }
                        else
                        {
                            RemoveIfExist(ldCampaignInteractionDetails, objLinkedinUser.ProfileUrl);
                            GlobusLogHelper.log.Info(Log.ActivityFailed,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objLinkedinUser.FullName,
                                message);
                        }

                        #endregion
                    }

                    #endregion

                    if (ConnectionRequestModel.IsStopSendConnectionRequestOnFailed &&
                        ConnectionRequestModel.StopSendConnectionRequestOnCount <= failedConnectionCount)
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

                #endregion

                StartOtherConfiguration(scrapeResult);
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
            return _jobProcessResult;
        }

        private bool ConnectionSuccessForSales(string response)
        {
            if (string.IsNullOrEmpty(response))
                return false;
            long.TryParse(Utils.GetBetween(response, "\"value\":\"", "\""), out long ID);
            return ID > 0;
        }

        private IResponseParameter SendConnectionRequestResponse(ScrapeResultNew scrapeResult,
            IResponseParameter sendConnectionRequestResponse, string sendRequestActionUrl, string postString,
            LinkedinUser objLinkedinUser, string finalPersonalNote)
        {
            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                // first try send connection request using mobile
                // sometime get too many request issue HTTP 429
                Thread.Sleep(RandomUtilties.GetRandomNumber(10000, 5000));
                var failed = 0;
            CheckAgain:
                var response = _ldFunctions.GetInnerLdHttpHelper().HandlePostResponse(sendRequestActionUrl, postString);
                while (failed++ <= 2 && (response != null && response?.Exception != null && response.Exception.Message.Contains("Unauthorized")))
                    goto CheckAgain;
                if (response?.Response == null && response.HasError ? !string.IsNullOrEmpty(response?.Exception?.Message) ? response.Exception.Message.Contains("Too Many Requests") : false : false)
                    sendConnectionRequestResponse = new ResponseParameter { Response = response.Exception.Message };
                else if (response?.Response == null && response.HasError)
                    sendConnectionRequestResponse = new ResponseParameter { Response = LdConstants.YourInvitationToConnectWasNotSent };
                else
                    sendConnectionRequestResponse = new ResponseParameter { Response = response?.Response };
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (sendConnectionRequestResponse == null &&
                    scrapeResult.QueryInfo.QueryType !=
                    EnumUtility.GetQueryFromEnum(LDGrowConnectionUserQueryParameters.Email))
                {
                    #region Alternative way to send connection required if failed with First PostRequest Above

                    sendRequestActionUrl = "https://www.linkedin.com/voyager/api/growth/normInvitations";
                    var profilePageSource = _ldFunctions.GetHtmlFromUrlNormalMobileRequest(objLinkedinUser.ProfileUrl);
                    var flagship3ProfileViewBase =
                        Utils.GetBetween(profilePageSource, "flagship3_profile_view_base;", "\n");

                    var trackingId = objLinkedinUser.TrackingId ?? Utils.GenerateTrackingId();
                    postString = string.IsNullOrEmpty(finalPersonalNote)
                        ? "{\"emberEntityName\":\"growth/invitation/norm-invitation\",\"invitee\":{\"com.linkedin.voyager.growth.invitation.InviteeProfile\":{\"profileId\":\"" +
                          objLinkedinUser.ProfileId + "\"}},\"trackingId\":\"" + trackingId + "\"}"
                        : "{\"emberEntityName\":\"growth/invitation/norm-invitation\",\"invitee\":{\"com.linkedin.voyager.growth.invitation.InviteeProfile\":{\"profileId\":\"" +
                          objLinkedinUser.ProfileId + "\"}},\"trackingId\":\"" + trackingId +
                          "\",\"message\":\"" + finalPersonalNote + "\"}";
                    sendConnectionRequestResponse =
                        _ldFunctions.SendConnectionRequestAlternativeMethod(postString, sendRequestActionUrl,
                            flagship3ProfileViewBase);

                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    #endregion
                }
            }
            catch (OperationCanceledException)
            {
                LDAccountsBrowserDetails.CloseBrowser(DominatorAccountModel, IsBrowser);
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return sendConnectionRequestResponse;
        }

        private void RemoveIfExist(ICampaignInteractionDetails ldCampaignInteractionDetails, string user)
        {
            try
            {
                ldCampaignInteractionDetails.RemoveIfExist(SocialNetworks.LinkedIn, CampaignId, user);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AddInteractedData(ICampaignInteractionDetails ldCampaignInteractionDetails, string url)
        {
            try
            {
                ldCampaignInteractionDetails.AddInteractedData(SocialNetworks.LinkedIn, CampaignId, url);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private bool FilterBlackListedUser(LinkedinUser objLinkedinUser)
        {
            // if filter is checked go further else no need to go
            // it will only filter for sales navigator search url since we already filtered users of normal search url
            if (!ConnectionRequestModel.IsChkSkipBlackListedUser || !ConnectionRequestModel.IsChkGroupBlackList &&
                !ConnectionRequestModel.IsChkGroupBlackList && !CurrentActivityType.Equals("Sales Navigator SearchUrl"))
                return false;
            var manageBlacklistWhitelist =
                new ManageBlacklistWhitelist(DbAccountService, _delayService);
            return manageBlacklistWhitelist.FilterSalesBlackListedUser(_ldFunctions, objLinkedinUser,
                ConnectionRequestModel.IsChkPrivateBlackList, ConnectionRequestModel.IsChkGroupBlackList);
        }

        private string RecipientDetails(string fullName, LinkedinUser objLinkedinUser, ref string firstName,
            ref string lastName)
        {
            #region Recipent Details

            try
            {
                #region SplitedFullName

                var splitFullName = new string[] { };
                fullName = objLinkedinUser.FullName;
                try
                {
                    splitFullName = Regex.Split(fullName, " ");
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion

                if (splitFullName.Length > 2)
                {
                    #region MyRegion

                    var middleName = string.Empty;
                    try
                    {
                        firstName = Regex.Split(fullName, " ")[0];
                        firstName = firstName.Replace("\"", "");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    try
                    {
                        middleName = Regex.Split(fullName, " ")[1];
                        middleName = middleName.Replace("\"", "");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    try
                    {
                        lastName = middleName + " " + Regex.Split(fullName, " ")[2];
                        lastName = lastName.Replace("\"", "");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    #endregion
                }
                else
                {
                    #region MyRegion

                    var middleName = string.Empty;
                    try
                    {
                        firstName = Regex.Split(fullName, " ")[0];
                        firstName = firstName.Replace("\"", "");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    try
                    {
                        lastName = middleName + " " + Regex.Split(fullName, " ")[1];
                        lastName = lastName.Replace("\"", "");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion

            return fullName;
        }

        public string GetPostString(ILdFunctions objLdFunctions, string emailAddress)
        {
            try
            {
                LdCancellationToken.ThrowIfCancellationRequested();
                var reqParams = objLdFunctions.GetInnerHttpHelper().GetRequestParameter();
                reqParams.UserAgent = null;
                var pgSrcInviteByEmail =
                    objLdFunctions.GetHtmlFromUrlNormalMobileRequest(
                        "https://www.linkedin.com/mynetwork/import-contacts/");

                #region uploadTransactionId and InvitetrackingId
                var uploadTransactionId = Utils.GetBetween(pgSrcInviteByEmail, "FileUploadToken\",\"$id\":\"", ",")
                    .Replace("&#61;", "=");
                var inviteTrackingId = Utils.GetBetween(pgSrcInviteByEmail, "trackingId\":\"", "\"");

                #endregion

                return "{\"defaultCountryCode\":\"us\",\"uploadTransactionId\":\"" + uploadTransactionId +
                       "\",\"invitations\":[{\"trackingId\":\"" + inviteTrackingId +
                       "\",\"invitations\":[],\"invitee\":{\"com.linkedin.voyager.growth.invitation.InviteeEmail\":{\"email\":\"" +
                       emailAddress + "\"}},\"message\":\"I’d like to add you to my professional network\"}]}";
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return "";
            }
        }

        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {
            try
            {
                #region Process for auto ConnectionRequest and WithdrawConnectionRequest

                if (ConnectionRequestModel.IsCheckedEnableAutoWithdrawConnectionRequest)
                {
                    // keep incrementing after succesfully sending connection request
                    if (_jobProcessResult.IsProcessSuceessfull)
                        ++_totalConnectionRequestSent;
                    // here we getting the count once for each job
                    if (ConnectionRequestModel.IsCheckedStartWithdrawConnectionRequestWhenLimitReach &&
                        !_isAlreadyGetConnectionCount)
                    {
                        _isAlreadyGetConnectionCount = true;
                        _totalConnectionRequestSent =
                            LdDataHelper.GetInstance.GetAndSetSentConnectionCount(DominatorAccountModel, _ldFunctions);
                    }

                    if (ConnectionRequestModel.IsCheckedStartWithdrawConnectionRequestWhenLimitReach &&
                        _totalConnectionRequestSent >=
                        ConnectionRequestModel.StartWithdrawConnectionRequestWhenLimitReach.GetRandom()
                        || ConnectionRequestModel.IsCheckedConnectionRequestToolGetsTemporaryBlocked &&
                        scrapeResult.IsAccountLocked)
                        try
                        {
                            var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                            dominatorScheduler.EnableDisableModules(ActivityType.ConnectionRequest,
                                ActivityType.WithdrawConnectionRequest, DominatorAccountModel.AccountId);
                        }
                        catch (InvalidOperationException ex)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                                ex.Message.Contains("1001")
                                    ? "LangKeySendConnectionRequestHasMetAutoEnableConfiguration"
                                        .FromResourceDictionary()
                                    : "");
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
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