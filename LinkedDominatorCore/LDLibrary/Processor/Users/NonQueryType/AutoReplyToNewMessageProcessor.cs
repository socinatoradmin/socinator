using System;
using System.Text.RegularExpressions;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel.Messenger;
using LinkedDominatorCore.LDModel;
using System.Collections.Generic;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using System.Linq;

namespace LinkedDominatorCore.LDLibrary.Processor.Users.NonQueryType
{
    public class AutoReplyToNewMessageProcessor : BaseLinkedinUserProcessor, IQueryProcessor
    {
        public AutoReplyToNewMessageProcessor(ILdJobProcess jobProcess,
            IDbCampaignService campaignService, ILdFunctionFactory ldFunctionFactory,
            IProcessScopeModel processScopeModel, IDelayService delayService) :
            base(jobProcess, campaignService, ldFunctionFactory, delayService, processScopeModel)
        {
            AutoReplyToNewMessageModel = processScopeModel.GetActivitySettingsAs<AutoReplyToNewMessageModel>();
        }

        private AutoReplyToNewMessageModel AutoReplyToNewMessageModel { get; }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                var dataTimeinmili = DateTime.Now.GetCurrentEpochTimeMilliSeconds();
                var Instance = InstanceProvider.GetInstance<ILdAccountUpdateFactory>();
                Instance.UpdateConnections(DominatorAccountModel, LdFunctions, DbAccountService)
                    .Wait(LdJobProcess.JobCancellationTokenSource.Token);
                var Constructed_ActionUrl = AutoReplyToNewMessageModel.IsReplyToAllUserMessagesWhodidnotReply
                    ? IsBrowser ? "https://www.linkedin.com/messaging/" :
                    $"https://www.linkedin.com/voyager/api/messaging/conversations?keyVersion=LEGACY_INBOX&createdBefore={dataTimeinmili}"
                    : IsBrowser
                        ? "https://www.linkedin.com/messaging/"
                        : "https://www.linkedin.com/voyager/api/messaging/conversations?keyVersion=LEGACY_INBOX&q=search";//&filters=List(UNREAD)
                var actionUrl = Constructed_ActionUrl;
                var PaginationCount = 0;
                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    var specificWords = AutoReplyToNewMessageModel.SpecificWord;
                    var isReplyToAllUsersWhodidnotReply =
                        AutoReplyToNewMessageModel.IsReplyToAllUserMessagesWhodidnotReply;
                    var isReplyToAllMessagesChecked = AutoReplyToNewMessageModel.IsReplyToAllMessagesChecked;
                    var objSearchNewMessageFromConnectionResponseHandler = LdFunctions.SearchForLinkedinConnections(
                        actionUrl, isReplyToAllMessagesChecked, specificWords, isReplyToAllUsersWhodidnotReply,
                        DominatorAccountModel.AccountBaseModel.UserId);

                    objSearchNewMessageFromConnectionResponseHandler.ConnectionsList.RemoveAll(x =>
                        x.ProfileId == DominatorAccountModel.AccountBaseModel.UserId ||
                        string.IsNullOrWhiteSpace(x.MessageContent));

                    try
                    {
                        //if you want to send message only who send connection by software
                        var lstUserssendbysoftware = new List<LinkedinUser>();
                        if (AutoReplyToNewMessageModel.IsCheckedBySoftware)
                        {
                            var lstConnectionRequestSentFromSoftware =
                                DbAccountService.GetConnectionRequestsSendFromSoftware()?.ToList();
                            foreach (var connectionusers in lstConnectionRequestSentFromSoftware)
                                foreach (var messageuser in objSearchNewMessageFromConnectionResponseHandler.ConnectionsList)
                                  if (messageuser.ProfileId==connectionusers.ProfileId || messageuser.PublicIdentifier==connectionusers.PublicIdentifer)
                                    {
                                        lstUserssendbysoftware.Add(messageuser);
                                        break;
                                    }
                            objSearchNewMessageFromConnectionResponseHandler.ConnectionsList.Clear();

                        }
                        var lstusersendmessagebefore = new List<LinkedinUser>();

                        //if user select both checkbox sendconnectionby and lastmessagesendbefore
                        if (AutoReplyToNewMessageModel.IsCheckedLastSendMessageFrom && AutoReplyToNewMessageModel.IsCheckedBySoftware)
                        {
                            foreach (var lstuserfilterbydate in lstUserssendbysoftware)  
                            {
                                long timeStampFromUserInput = 0;
                                var days = AutoReplyToNewMessageModel.Days;
                                var hours = AutoReplyToNewMessageModel.Hours;
                                timeStampFromUserInput =
                                    DateTime.Now.AddDays(-days).AddHours(-hours).GetCurrentEpochTime();

                                if (lstuserfilterbydate.ConnectedTimeStamp >= timeStampFromUserInput)
                                    lstusersendmessagebefore.Add(lstuserfilterbydate);
                            }
                            objSearchNewMessageFromConnectionResponseHandler.ConnectionsList.Clear();
                        }

                        //if user select only last send message checkbox 
                        if (AutoReplyToNewMessageModel.IsCheckedLastSendMessageFrom & !AutoReplyToNewMessageModel.IsCheckedBySoftware)
                        {
                            foreach (var lstuserfilterbydate in objSearchNewMessageFromConnectionResponseHandler.ConnectionsList)
                            {
                                long timeStampFromUserInput = 0;
                                var days = AutoReplyToNewMessageModel.Days;
                                var hours = AutoReplyToNewMessageModel.Hours;
                                timeStampFromUserInput =
                                    DateTime.Now.AddDays(-days).AddHours(-hours).GetCurrentEpochTime();
                                if(lstuserfilterbydate.ConnectedTimeStamp > timeStampFromUserInput)
                                    lstusersendmessagebefore.Add(lstuserfilterbydate);
                            }
                            objSearchNewMessageFromConnectionResponseHandler.ConnectionsList.Clear();
                        }

                        if (AutoReplyToNewMessageModel.IsCheckedBySoftware)
                            objSearchNewMessageFromConnectionResponseHandler.ConnectionsList.AddRange(lstUserssendbysoftware);

                        if (AutoReplyToNewMessageModel.IsCheckedLastSendMessageFrom && lstusersendmessagebefore.Count <= 0)
                            objSearchNewMessageFromConnectionResponseHandler.ConnectionsList.Clear();
                        else
                            foreach (var lstbysoftware in lstUserssendbysoftware)
                                foreach (var lstmessage in lstusersendmessagebefore)
                                    if (!lstbysoftware.PublicIdentifier.Equals(lstmessage.PublicIdentifier))
                                        objSearchNewMessageFromConnectionResponseHandler.ConnectionsList.AddRange(lstusersendmessagebefore);
                                
                        //if user only select lastmessage checkbox
                        if(AutoReplyToNewMessageModel.IsCheckedLastSendMessageFrom && lstusersendmessagebefore.Count>=0 && !AutoReplyToNewMessageModel.IsCheckedBySoftware)
                            objSearchNewMessageFromConnectionResponseHandler.ConnectionsList.AddRange(lstusersendmessagebefore);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                       
                    LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (objSearchNewMessageFromConnectionResponseHandler.Success)
                    {
                        string createdBefore;
                        if (objSearchNewMessageFromConnectionResponseHandler.ConnectionsList.Count > 0)
                        {
                            #region Filter Messages From Users With No Profile Pic 

                            if (AutoReplyToNewMessageModel.LDUserFilterModel.IsCheckedFilterProfileImageCheckbox)
                            {
                                objSearchNewMessageFromConnectionResponseHandler.ConnectionsList.RemoveAll(x =>
                                    x.ProfilePicUrl == null);
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    "successfully filtered messages from connections having no profile picture");
                            }

                            #endregion
                            //Filter BlacklistedUsers
                            if (AutoReplyToNewMessageModel.IsChkSkipBlackListedUser &&
                                (AutoReplyToNewMessageModel.IsChkPrivateBlackList ||
                                 AutoReplyToNewMessageModel.IsChkGroupBlackList))
                                FilterBlacklistedUsers(objSearchNewMessageFromConnectionResponseHandler.ConnectionsList,
                                    AutoReplyToNewMessageModel.IsChkPrivateBlackList,
                                    AutoReplyToNewMessageModel.IsChkGroupBlackList);

                            RemoveOrSkipAlreadyInteractedLinkedInUsers(objSearchNewMessageFromConnectionResponseHandler.ConnectionsList);

                            if (objSearchNewMessageFromConnectionResponseHandler.ConnectionsList.Count > 0)
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage,DominatorAccountModel.AccountBaseModel.AccountNetwork,DominatorAccountModel.AccountBaseModel.UserName, ActivityType,"Successful! To Get All Messages.");
                                ProcessLinkedinUsersFromUserList(QueryInfo.NoQuery, ref jobProcessResult,
                                    objSearchNewMessageFromConnectionResponseHandler.ConnectionsList);
                            }
                            else if(PaginationCount == 0 && !IsBrowser)
                                GlobusLogHelper.log.Info(Log.CustomMessage,DominatorAccountModel.AccountBaseModel.AccountNetwork,DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    "Sorry No New Message Found.Checking Pagination For New Message.");

                            if (IsBrowser)
                            {
                                jobProcessResult.HasNoResult = true;
                                break;
                            }
                            if (PaginationCount > 10)
                                break;
                            #region Pagination

                            try
                            {
                                createdBefore =
                                    (objSearchNewMessageFromConnectionResponseHandler.LastConnectedTimeStamp + 89900000)
                                    .ToString();
                                if (actionUrl.Contains("createdBefore"))
                                {
                                    actionUrl = Regex.Replace(actionUrl, @"[0-9\-]", string.Empty);
                                    actionUrl = $"{actionUrl}{createdBefore}";
                                }
                                else
                                {
                                    actionUrl = Constructed_ActionUrl + "&createdBefore=" + createdBefore;
                                }
                                PaginationCount++;
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }

                            #endregion
                        }
                        else
                        {
                            if (!objSearchNewMessageFromConnectionResponseHandler.HasMoreResults)
                            {
                                if (isReplyToAllUsersWhodidnotReply && AutoReplyToNewMessageModel.IsReplyToMessagesThatContainSpecificWordChecked)
                                {
                                    jobProcessResult.HasNoResult = true;
                                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                        "Sorry! No Unique Message Found As Per Your Filter");
                                    break;
                                }
                                else
                                {
                                    jobProcessResult.HasNoResult = true;
                                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                        "Sorry! No New Messages Found.");
                                    break;
                                }
                            }
                            if (PaginationCount > 10)
                                break;
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                "Sorry! No New Messages Found Navigating to next page.");

                            #region Pagination for getting next bunch of new messages

                            try
                            {
                                createdBefore =
                                    (objSearchNewMessageFromConnectionResponseHandler.LastConnectedTimeStamp + 89900000)
                                    .ToString();
                            }
                            catch (Exception exx)
                            {
                                exx.DebugLog();
                                var count = DbAccountService.GetInteractedUsers(ActivityTypeString).Count;
                                var lastInteractedUser =
                                    DbAccountService.GetSingleInteractedUser(ActivityTypeString, count);
                                createdBefore =
                                    (lastInteractedUser.InteractionDatetime.GetCurrentEpochTimeMilliSeconds() +
                                     89900000).ToString();
                            }

                            actionUrl = Constructed_ActionUrl + "&createdBefore=" + createdBefore;
                            PaginationCount++;
                            #endregion
                        }
                    }
                    else
                    {
                        if (isReplyToAllUsersWhodidnotReply && AutoReplyToNewMessageModel.IsReplyToMessagesThatContainSpecificWordChecked)
                        {
                            jobProcessResult.HasNoResult = true;
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                "Sorry! No Unique Message Found As Per Your Filter");
                        }
                        else
                        {
                            jobProcessResult.HasNoResult = true;
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                "Sorry! No New Messages Found.");
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                jobProcessResult.HasNoResult = true;
                ex.DebugLog();
            }
        }

        
    }
}