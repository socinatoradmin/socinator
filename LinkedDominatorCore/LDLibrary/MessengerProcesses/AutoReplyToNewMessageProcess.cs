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
using LinkedDominatorCore.LDModel.Messenger;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Request;
using LinkedDominatorCore.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace LinkedDominatorCore.LDLibrary.MessengerProcesses
{
    public class AutoReplyToNewMessageProcess : LDJobProcessInteracted<
        InteractedUsers>
    {
        private readonly ILdFunctions _ldFunctions;
        private readonly LdDataHelper dataHelper = LdDataHelper.GetInstance;
        public AutoReplyToNewMessageProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ILdQueryScraperFactory queryScraperFactory,
            ILdHttpHelper ldHttpHelper, ILdLogInProcess logInProcess,
            ILdFunctionFactory ldFunctionFactory, IDbInsertionHelper dbInsertionHelper)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                ldHttpHelper, logInProcess, dbInsertionHelper)
        {
            AutoReplyToNewMessageModel = processScopeModel.GetActivitySettingsAs<AutoReplyToNewMessageModel>();
            _ldFunctions = ldFunctionFactory.LdFunctions;
        }

        public AutoReplyToNewMessageModel AutoReplyToNewMessageModel { get; set; }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobProcessResult jobProcessResult = null;
            #region Auto Reply To New Message Process.
            try
            {
                jobProcessResult = new JobProcessResult();

                var objLinkedinUser = (LinkedinUser) scrapeResult.ResultUser;

                #region Filters After Visiting Profile

                try
                {
                    if (LdUserFilterProcess.IsUserFilterActive(AutoReplyToNewMessageModel.LDUserFilterModel))
                    {
                        var isValidUser = LdUserFilterProcess.GetFilterStatus(objLinkedinUser.ProfileUrl,
                            AutoReplyToNewMessageModel.LDUserFilterModel, _ldFunctions);
                        if (!isValidUser)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                "[ " + objLinkedinUser.FullName + " ] is not a valid user according to the filter.");
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

                var messageContent = string.Empty;
                var textMessage = string.Empty;
                var imageSource = string.Empty;
                var finalMessage = string.Empty;
                var postString = string.Empty;

                var sendMessageToConnectionResponse = string.Empty;

                #endregion

                #region Sender's Variables Initializtions

                var fromFullName = string.Empty;
                var fromFirstName = string.Empty;
                var fromLastName = string.Empty;
                var fromCompanyName = string.Empty;

                #endregion

                #region Recipient's Variables Initializations

                var firstName = string.Empty;
                var lastName = string.Empty;
                var fullName = string.Empty;
                var CompanyName = string.Empty;

                #endregion


                GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, "reply to message from", objLinkedinUser.FullName);

                #region Sender's Details

                //FromFullName
                fromFullName = DominatorAccountModel?.AccountBaseModel?.UserFullName;

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
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion

                #region fromcompanyname
                //if sender company name is null then by default it will take sender occupation
                try
                {
                    var companynameandoccupation = Regex.Split(DominatorAccountModel.ExtraParameters.Keys.Contains("Occupation") ? DominatorAccountModel.ExtraParameters["Occupation"] : "", " at");
                    if (companynameandoccupation.Count() == 2)
                        fromCompanyName = companynameandoccupation[1];
                    else
                        fromCompanyName = companynameandoccupation[0];
                }
                catch (Exception)
                {
                }
                #endregion


                #endregion

                #region Recipent Details

                try
                {
                    #region SplitedFullName

                    var splitedFullName = new string[] { };
                    fullName = objLinkedinUser.FullName;
                    splitedFullName = Regex.Split(fullName, " ");

                    #endregion

                    if (splitedFullName.Length > 2)
                    {
                        #region MyRegion

                        var middleName = splitedFullName[1];
                        firstName = splitedFullName[0];
                        lastName = $"{middleName} {splitedFullName[2]}";

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

                #region company name                
                try
                {
                    var checkmessage=AutoReplyToNewMessageModel.LstDisplayManageMessagesModel.Select(x => x.MessagesText).ToList().GetRandomItem();
                    var occupationandcompany = Regex.Split(objLinkedinUser.Occupation, " at");
                    if (occupationandcompany.Count() == 2)
                        CompanyName = occupationandcompany[1];
                    //if tagged contains company name then only hit response to avoid blocking account
                    if ( string.IsNullOrEmpty(CompanyName)&&(checkmessage.Contains("<Company Name>") ||checkmessage.Contains("<company name>")) )
                    {
                        var res = _ldFunctions.GetInnerHttpHelper().GetRequest($"https://www.linkedin.com/voyager/api/identity/dash/profiles?q=memberIdentity&memberIdentity={objLinkedinUser.ProfileId}&decorationId=com.linkedin.voyager.dash.deco.identity.profile.FullProfileWithEntities-35");
                        JsonHandler json = new JsonHandler(res.Response);
                        CompanyName = json.GetElementValue("elements", 0, "profilePositionGroups", "elements", 0, "companyName");
                    }
                }
                catch (Exception)
                {                    
                }
                #endregion

                #region Message

                try
                {
                    messageContent = GetRandomMessage(objLinkedinUser.SelectedMessageFilter);
                    //filter if already send message
                    if(AutoReplyToNewMessageModel.IsCheckedIgnoreAlreadySendMessage)
                    {
                        int i = 0;
                        while(objLinkedinUser.MessageContent==messageContent && i<2)
                        {
                            messageContent = GetRandomMessage(objLinkedinUser.SelectedMessageFilter);
                            i++;
                        }
                    }
                    if (messageContent == null)
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName,
                            "reply to message [ " + objLinkedinUser.MessageContent + " ] from",
                            objLinkedinUser.FullName, " because no suitable reply back message found.");
                        jobProcessResult.IsProcessSuceessfull = false;
                        return jobProcessResult;
                    }

                    if (objLinkedinUser.MessageContent == string.Empty)
                        messageContent = "Thank you " + objLinkedinUser.FullName +
                                         " for connecting with me here on LinkedIn";

                    textMessage = Regex.Split(messageContent, "<:>")[0];
                    try
                    {
                        imageSource = Regex.Split(messageContent, "<:>")[1];
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion

                #region IsChkSpintaxChecked

                if (AutoReplyToNewMessageModel.IsChkSpintaxChecked)
                {
                    var lstMessages = new List<string>();
                    try
                    {
                        lstMessages = SpinTexHelper.GetSpinMessageCollection(textMessage);
                        lstMessages.Shuffle();
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    try
                    {
                        finalMessage =
                            lstMessages[new RandomNumberGenerator().GenerateRandom(0, lstMessages.Count - 1)];
                    }
                    catch (Exception ex)
                    {
                        finalMessage = textMessage;
                        ex.DebugLog();
                    }
                }
                else
                {
                    finalMessage = textMessage;
                }

                #endregion

                #region IsChkTagChecked

                try
                {
                    if (AutoReplyToNewMessageModel.IsChkTagChecked)
                    {
                        #region FinalMessage With Recipient's Tags

                        finalMessage = finalMessage.Replace("<First Name>", firstName);
                        finalMessage = finalMessage.Replace("<First name>", firstName);
                        finalMessage = finalMessage.Replace("<first Name>", firstName);
                        finalMessage = finalMessage.Replace("<first name>", firstName);

                        finalMessage = finalMessage.Replace("<Last Name>", lastName);
                        finalMessage = finalMessage.Replace("<Last name>", lastName);
                        finalMessage = finalMessage.Replace("<last Name>", lastName);
                        finalMessage = finalMessage.Replace("<last name>", lastName);

                        finalMessage = finalMessage.Replace("<Full Name>", fullName);
                        finalMessage = finalMessage.Replace("<Full name>", fullName);
                        finalMessage = finalMessage.Replace("<full Name>", fullName);
                        finalMessage = finalMessage.Replace("<full name>", fullName);

                        finalMessage = finalMessage.Replace("<Company Name>", CompanyName);
                        finalMessage = finalMessage.Replace("<company Name>", CompanyName);
                        finalMessage = finalMessage.Replace("<Company name>", CompanyName);
                        finalMessage = finalMessage.Replace("<company name>", CompanyName);
                        #endregion

                        #region FinalMessage With Sender's Tags

                        finalMessage = finalMessage.Replace("<From First Name>", fromFirstName);
                        finalMessage = finalMessage.Replace("<From first name>", fromFirstName);
                        finalMessage = finalMessage.Replace("<from first name>", fromFirstName);

                        finalMessage = finalMessage.Replace("<From Last Name>", fromLastName);
                        finalMessage = finalMessage.Replace("<From last name>", fromLastName);
                        finalMessage = finalMessage.Replace("<from last name>", fromLastName);

                        finalMessage = finalMessage.Replace("<From Full Name>", fromFullName);
                        finalMessage = finalMessage.Replace("<From full name>", fromFullName);
                        finalMessage = finalMessage.Replace("<from full name>", fromFullName);

                        finalMessage = finalMessage.Replace("<From Company Name>", fromCompanyName);
                        finalMessage = finalMessage.Replace("<From company name>", fromCompanyName);
                        finalMessage = finalMessage.Replace("< from company name>", fromCompanyName);
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion
                //finalMessage = _ldFunctions.GenerateAIPrompt(DominatorAccountModel,objLinkedinUser.MessageContent).Result;
                sendMessageToConnectionResponse = IsBrowser
                    ? _ldFunctions.UploadImageAndGetContentIdForMessaging(objLinkedinUser.ProfileUrl,
                        string.IsNullOrEmpty(imageSource) ? null : new FileInfo(imageSource),
                        finalMessage)
                    :
                    _ldFunctions.BroadCastMessage(imageSource, objLinkedinUser, false, finalMessage, string.Empty);
                if (IsBrowser && !string.IsNullOrEmpty(imageSource))
                    finalMessage = $"{finalMessage}<:>{imageSource}";
                if (string.IsNullOrEmpty(sendMessageToConnectionResponse)?false:sendMessageToConnectionResponse.Contains("{\"value\":{\"createdAt\":"))
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objLinkedinUser.FullName);

                    IncrementCounters();
                    DbInsertionHelper.AutoReplyToNewMessage(scrapeResult, objLinkedinUser, finalMessage);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objLinkedinUser.FullName, "");
                    jobProcessResult.IsProcessSuceessfull = false;
                }
                DelayBeforeNextActivity();
            }
            catch (OperationCanceledException)
            {
                jobProcessResult.IsProcessSuceessfull = false;
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            #endregion
            return jobProcessResult;
        }
        public string GetRandomMessage(string selectedSource)
        {
            try
            {
                string messageContent;
                var imageSource = string.Empty;

                var message = "";
                if (selectedSource == null)
                {
                    message = AutoReplyToNewMessageModel.LstDisplayManageMessagesModel
                        .Where(x => x.SelectedQuery.Any(y =>
                            y.Content.QueryValue.ToString() == "Reply To All Messages" ||
                            y.Content.QueryValue.ToString() == "All" || y.Content.QueryValue.ToString() ==
                            "Reply To All User Messages Who didn't reply"))
                        ?.Select(x => x.MessagesText).ToList().GetRandomItem();

                    imageSource = AutoReplyToNewMessageModel.LstDisplayManageMessagesModel
                        .Where(x => x.SelectedQuery.Any(y =>
                            y.Content.QueryValue.ToString() == "Reply To All Messages" ||
                            y.Content.QueryValue.ToString() == "All" || y.Content.QueryValue.ToString() ==
                            "Reply To All User Messages Who didn't reply"))
                        .Select(x => x.MediaPath).ToList().GetRandomItem();
                }
                else
                {
                    message = AutoReplyToNewMessageModel.LstDisplayManageMessagesModel
                        .Where(x => x.SelectedQuery.Any(y => y.Content.QueryValue.ToString() == selectedSource))
                        .Select(x => x.MessagesText).ToList().GetRandomItem();
                    try
                    {
                        imageSource = AutoReplyToNewMessageModel.LstDisplayManageMessagesModel
                            .Where(x => x.SelectedQuery.Any(y => y.Content.QueryValue.ToString() == selectedSource))
                            .Select(x => x.MediaPath).ToList().GetRandomItem();
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }

                var lstMessage = new List<string>();
                try
                {
                    lstMessage = Regex.Split(message, "<End>").ToList();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                if (lstMessage.Count > 1)
                    message = lstMessage.GetRandomItem();

                if (!string.IsNullOrEmpty(imageSource))
                    messageContent = message + "<:>" + imageSource;
                else
                    messageContent = message;


                return IsBrowser ? messageContent:dataHelper.ReplaceSpecialCharacter(messageContent);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }
    }
}