using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.DetailedInfo;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDModel.Messenger;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Request;
using LinkedDominatorCore.Utility;
using Newtonsoft.Json;

namespace LinkedDominatorCore.LDLibrary.MessengerProcesses
{
    public class SendGreetingsToConnectionsProcess : LDJobProcessInteracted<
        InteractedUsers>
    {
        private readonly ILdFunctions _ldFunctions;


        public SendGreetingsToConnectionsProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ILdQueryScraperFactory queryScraperFactory,
            ILdHttpHelper ldHttpHelper, ILdLogInProcess logInProcess,
            ILdFunctionFactory ldFunctionFactory, IDbInsertionHelper dbInsertionHelper)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                ldHttpHelper, logInProcess, dbInsertionHelper)
        {
            SendGreetingsToConnectionsModel =
                processScopeModel.GetActivitySettingsAs<SendGreetingsToConnectionsModel>();
            _ldFunctions = ldFunctionFactory.LdFunctions;
        }

        public SendGreetingsToConnectionsModel SendGreetingsToConnectionsModel { get; set; }


        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobProcessResult jobProcessResult = null;
            #region Send Greetings To Connections Process.
            try
            {
                jobProcessResult = new JobProcessResult();
                try
                {
                    var objLinkedinUser = (LinkedinUser)scrapeResult.ResultUser;
                    if (string.IsNullOrEmpty(objLinkedinUser.PublicIdentifier))
                    {
                        var userdetailResponse = _ldFunctions.GetHtmlFromUrlForMobileRequest(
                            $"https://www.linkedin.com/voyager/api/identity/profiles/{objLinkedinUser.ProfileId}", "");
                        objLinkedinUser.PublicIdentifier =
                            Utilities.GetBetween(userdetailResponse, "publicIdentifier\":\"", "\"");
                        objLinkedinUser.ProfileUrl = $"https://www.linkedin.com/in/{objLinkedinUser.PublicIdentifier}/";
                    }

                    if (IsBrowser && string.IsNullOrEmpty(objLinkedinUser.FullName))
                    {
                        var browserUserPageDetails =
                            _ldFunctions.GetHtmlFromUrlForMobileRequest(objLinkedinUser.ProfileUrl, "");
                        var fullname = HtmlAgilityHelper.GetStringInnerTextFromClassName(browserUserPageDetails,
                            "inline t-24 t-black t-normal break-words");
                        objLinkedinUser.FullName = fullname;
                    }

                    #region Filters After Visiting Profile

                    try
                    {
                        if (LdUserFilterProcess.IsUserFilterActive(SendGreetingsToConnectionsModel.LDUserFilterModel))
                        {
                            var isValidUser = LdUserFilterProcess.GetFilterStatus(objLinkedinUser.ProfileUrl,
                                SendGreetingsToConnectionsModel.LDUserFilterModel, _ldFunctions);
                            if (!isValidUser)
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    "[ " + objLinkedinUser.FullName +
                                    " ] is not a valid user according to the filter.");
                                jobProcessResult.IsProcessSuceessfull = false;
                                return jobProcessResult;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    #endregion

                    #region  Variables Initializations

                    var greeting = string.Empty;
                    var finalGreeting = string.Empty;

                    var sendGreetingToConnectionResponse = string.Empty;

                    #endregion

                    #region Sender's Variables Initializtions

                    var fromFullName = string.Empty;
                    var fromFirstName = string.Empty;
                    var fromLastName = string.Empty;

                    #endregion

                    #region Recipient's Variables Initializations

                    var firstName = string.Empty;
                    var lastName = string.Empty;
                    var fullName = string.Empty;

                    #endregion

                    GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, "send greeting to connection",
                        objLinkedinUser.FullName);

                    #region Sender's Details

                    #region FromFullName

                    try
                    {
                        fromFullName = DominatorAccountModel.AccountBaseModel.UserFullName;
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    #endregion

                    #region FromFirstName

                    try
                    {
                        fromFirstName = Regex.Split(fromFullName, " ")[0];
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    #endregion

                    #region fromLastName

                    try
                    {
                        fromLastName = Regex.Split(fromFullName, " ")[1];
                        if (!string.IsNullOrEmpty(fromLastName) || fromLastName.Contains("\t"))
                            fromLastName = Regex.Split(fromFullName, " ")[2];
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    #endregion

                    #endregion

                    #region Recipent Details

                    try
                    {
                        #region SplitedFullName

                        var splitedFullName = new string[] { };
                        try
                        {
                            fullName = objLinkedinUser.FullName;
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                        try
                        {
                            splitedFullName = Regex.Split(fullName, " ");
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                        #endregion

                        if (splitedFullName.Length > 2)
                        {
                            #region MyRegion

                            var middleName = string.Empty;
                            try
                            {
                                firstName = Regex.Split(fullName, " ")[0];
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }

                            try
                            {
                                middleName = Regex.Split(fullName, " ")[1];
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }

                            try
                            {
                                lastName = middleName + " " + Regex.Split(fullName, " ")[2];
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
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }

                            try
                            {
                                lastName = middleName + " " + Regex.Split(fullName, " ")[1];
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

                    #region Greeting

                    try
                    {
                        greeting = GetRandomGreeting(objLinkedinUser.NotificationType);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    #endregion

                    #region IsChkSpintaxChecked

                    if (SendGreetingsToConnectionsModel.IsChkSpintaxChecked)
                    {
                        var lstGreetings = new List<string>();
                        try
                        {
                            lstGreetings = SpinTexHelper.GetSpinMessageCollection(greeting);
                            lstGreetings.Shuffle();
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                        try
                        {
                            finalGreeting =
                                lstGreetings[new RandomNumberGenerator().GenerateRandom(0, lstGreetings.Count - 1)];
                        }
                        catch (Exception ex)
                        {
                            finalGreeting = greeting;
                            ex.DebugLog();
                        }
                    }
                    else
                    {
                        finalGreeting = greeting;
                    }

                    #endregion

                    #region IsChkTagChecked

                    try
                    {
                        if (SendGreetingsToConnectionsModel.IsChkTagChecked)
                        {
                            #region FinalGreeting With Recipient's Tags

                            finalGreeting = finalGreeting.Replace("<First Name>", firstName);
                            finalGreeting = finalGreeting.Replace("<First name>", firstName);
                            finalGreeting = finalGreeting.Replace("<first Name>", firstName);
                            finalGreeting = finalGreeting.Replace("<first name>", firstName);

                            finalGreeting = finalGreeting.Replace("<Last Name>", lastName);
                            finalGreeting = finalGreeting.Replace("<Last name>", lastName);
                            finalGreeting = finalGreeting.Replace("<last Name>", lastName);
                            finalGreeting = finalGreeting.Replace("<last name>", lastName);

                            finalGreeting = finalGreeting.Replace("<Full Name>", fullName);
                            finalGreeting = finalGreeting.Replace("<Full name>", fullName);
                            finalGreeting = finalGreeting.Replace("<full Name>", fullName);
                            finalGreeting = finalGreeting.Replace("<full name>", fullName);

                            #endregion

                            #region FinalGreeting With Sender's Tags

                            finalGreeting = finalGreeting.Replace("<From First Name>", fromFirstName);
                            finalGreeting = finalGreeting.Replace("<From first name>", fromFirstName);
                            finalGreeting = finalGreeting.Replace("<from first name>", fromFirstName);

                            finalGreeting = finalGreeting.Replace("<From Last Name>", fromLastName);
                            finalGreeting = finalGreeting.Replace("<From last name>", fromLastName);
                            finalGreeting = finalGreeting.Replace("<from last name>", fromLastName);

                            finalGreeting = finalGreeting.Replace("<From Full Name>", fromFullName);
                            finalGreeting = finalGreeting.Replace("<From full name>", fromFullName);
                            finalGreeting = finalGreeting.Replace("<from full name>", fromFullName);

                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    #endregion
                    if (IsBrowser)
                    {
                        if (objLinkedinUser?.InvitationId == "https://www.linkedin.com")
                            sendGreetingToConnectionResponse =
                                _ldFunctions.UploadImageAndGetContentIdForMessaging(objLinkedinUser.ProfileUrl, null,
                                    finalGreeting);
                        else
                        {
                            if (Utils.IsJobOrWorkNotification(objLinkedinUser.NotificationType))
                                sendGreetingToConnectionResponse = _ldFunctions.CommentOnCustomPost(finalGreeting, objLinkedinUser.InvitationId);
                            else
                                sendGreetingToConnectionResponse = _ldFunctions.SendGreeting(objLinkedinUser.InvitationId, finalGreeting);
                        }      
                    }
                    else
                    {
                        sendGreetingToConnectionResponse =
                            SendGreetingToConnectionResponse(objLinkedinUser, ref finalGreeting);
                    }


                    #region SendGreetingToConnectionResponse And Actions After that

                    if (sendGreetingToConnectionResponse.Contains("{\"value\":{\"createdAt\":") || sendGreetingToConnectionResponse.Contains("urn:li:fsd_comment") || sendGreetingToConnectionResponse.Contains("Successfully"))
                    {
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName,
                            "send " + objLinkedinUser.NotificationType + " to connection", objLinkedinUser.FullName);
                        IncrementCounters();
                        var detailedInfo = GetJasonString(finalGreeting, objLinkedinUser.UniqueNotificationSuffix);
                        DbInsertionHelper.SendGreetingsToConnections(scrapeResult, objLinkedinUser, detailedInfo);
                        jobProcessResult.IsProcessSuceessfull = true;
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, "send greeting to connection",
                            objLinkedinUser.FullName, "");
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


        private string SendGreetingToConnectionResponse(LinkedinUser objLinkedinUser, ref string finalGreeting)
        {
            string sendGreetingToConnectionResponse;
            finalGreeting = finalGreeting?.Replace("\r\n", "\\n").Replace("\"", "\\\"");
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            var postString =
                "{\"conversationCreate\":{\"eventCreate\":{\"value\":{\"com.linkedin.voyager.messaging.create.MessageCreate\":{\"body\":\"" +
                finalGreeting + "\",\"attachments\":[]}}},\"recipients\":[\"" + objLinkedinUser.ProfileId +
                "\"],\"subtype\":\"MEMBER_TO_MEMBER\"},\"keyVersion\":\"LEGACY_INBOX\"}";
            var api = "https://www.linkedin.com/voyager/api/messaging/conversations?action=create";
            if (Utils.IsJobOrWorkNotification(objLinkedinUser.NotificationType))
                sendGreetingToConnectionResponse = _ldFunctions.CommentOnCustomPost(finalGreeting,$"https://www.linkedin.com/feed/update/" + objLinkedinUser.MessageThreadId);
            else
                sendGreetingToConnectionResponse = _ldFunctions.SendGreeting(api, postString);
            return sendGreetingToConnectionResponse;
        }

        private string GetRandomGreeting(string notificationType)
        {
            try
            {
                var message = SendGreetingsToConnectionsModel.LstDisplayManageMessagesModel
                    .Where(x => x.SelectedQuery.Any(y => y.Content.QueryValue.ToString() == notificationType))
                    .Select(x => x.MessagesText).ToList().GetRandomItem();
                var lstMessage = Regex.Split(message, "<End>").ToList();
                if (lstMessage.Count > 1)
                    message = lstMessage.GetRandomItem();
                return message;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        private string GetJasonString(string finalGreeting, string uniqueNotificationSuffix)
        {
            try
            {
                var detailedInfo = string.Empty;
                var objSendGreetingsToConnectionsDetailedInfo = new SendGreetingsToConnectionsDetailedInfo();
                try
                {
                    objSendGreetingsToConnectionsDetailedInfo.FinalGreeting = finalGreeting;
                    objSendGreetingsToConnectionsDetailedInfo.FinalGreeting =
                        Utils.InsertSpecialCharactersInCsv(objSendGreetingsToConnectionsDetailedInfo.FinalGreeting);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                try
                {
                    objSendGreetingsToConnectionsDetailedInfo.UniqueNotificationSuffix = uniqueNotificationSuffix;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                try
                {
                    detailedInfo = JsonConvert.SerializeObject(objSendGreetingsToConnectionsDetailedInfo);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                return detailedInfo;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return "";
            }
        }
    }
}