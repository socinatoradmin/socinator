using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDModel.Scraper;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Response;
using LinkedDominatorCore.Utility;
using Newtonsoft.Json.Linq;

namespace LinkedDominatorCore.LDLibrary.Processor.Users.NonQueryType
{
    internal class MessageConversationProcessor : BaseLinkedinUserProcessor, IQueryProcessor
    {
        private MessageConversationResponseHandler messageConversationResponseHandler;

        public MessageConversationProcessor(ILdJobProcess jobProcess, IDbCampaignService campaignService,
            ILdFunctionFactory ldFunctionFactory, IDelayService delayService, IProcessScopeModel processScopeModel) :
            base(jobProcess, campaignService, ldFunctionFactory, delayService, processScopeModel)
        {
            MessageConversationScraperModel =
                processScopeModel.GetActivitySettingsAs<MessageConversationScraperModel>();
        }

        public MessageConversationScraperModel MessageConversationScraperModel { get; set; }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            var nonQueryClass = new MapperModel();
            ClassMapper.SetModelClass(ref nonQueryClass, ActivityType, LdJobProcess);
            nonQueryClass.SetCustomList();

            var userList = new List<LinkedinUser>();
            var recentUser = new List<LinkedinUser>();

            if (nonQueryClass.IsCheckedLangKeyCustomUserList)
            {
                foreach (var customUserProfileUrl in nonQueryClass.UrlList)
                    AddCustomUrlToList(customUserProfileUrl, userList);
            }
            else
            {
                var connections = DbAccountService.GetConnections().ToList();
                var tempUserList = new List<LinkedinUser>();
                tempUserList = RecentConversationsUsers(recentUser, connections, tempUserList);
                userList.AddRange(recentUser);

                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName,
                    ActivityType, $"Your total number of Connections Conversation is {userList.Count()}");
                
                userList = userList.Take(LdJobProcess.CurrentJobCount).ToList();
            }
            var TotalUsersCount = userList.Count;
            //filter black list User
            foreach(var user in userList.ToArray())
            {
                var IsblackListedUser = manageBlacklistWhitelist.FilterBlackListedUser(user.PublicIdentifier, nonQueryClass.IsChkPrivateBlackList, nonQueryClass.IsChkGroupBlackList);
                if (IsblackListedUser)
                    userList.Remove(user);
            }
            if(TotalUsersCount-userList.Count > 0)
            {
                var message = nonQueryClass.IsChkGroupBlackList && nonQueryClass.IsChkPrivateBlackList?
                    "Skipped Private And Group BlackListed User" :
                    nonQueryClass.IsChkGroupBlackList ?
                    "Skipped Group BlackListed User" :
                    nonQueryClass.IsChkPrivateBlackList ?
                    "Skipped Private BlackListed User" : "Skipped Users";
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName,
                    ActivityType,message);
            }
            foreach (var linkedinUser in userList)
                try
                {
                    LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var ldDataHelper = LdDataHelper.GetInstance;
                    var conversationId = "";


                    var tempLinkedInUser = linkedinUser.DeepCloneObject();

                    
                    if (string.IsNullOrEmpty(tempLinkedInUser.ProfileId))
                        tempLinkedInUser = GetUserInformation(linkedinUser.ProfileUrl, false);

                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName,
                        ActivityType,
                        $"Getting conversation details with {tempLinkedInUser.FullName?.Replace("/", "")?.Replace("\"", "")}.");

                    var getConversationIdUrl = IsBrowser
                        ? tempLinkedInUser.ProfileUrl
                        : $"https://www.linkedin.com/voyager/api/messaging/conversations?keyVersion=LEGACY_INBOX&q=participants&recipients=List({tempLinkedInUser.ProfileId})";
                    if (!IsBrowser)
                    {
                        if (LdFunctions.GetInnerHttpHelper().GetRequestParameter().Headers.ToString()
                            .Contains("X-li-page-instance"))
                            LdFunctions.GetInnerHttpHelper().GetRequestParameter().Headers.Remove("X-li-page-instance");

                        var getConversationIdResponse =
                            LdFunctions.GetInnerLdHttpHelper().HandleGetResponse(getConversationIdUrl);

                        if (getConversationIdResponse.Response.Contains("elements\":[]"))
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName,
                                ActivityType,
                                $"No conversation found with {tempLinkedInUser.FullName?.Replace("/", "")?.Replace("\"", "")}.");
                            Thread.Sleep(new Random().Next(8000, 15000));
                            continue;
                        }

                        conversationId =
                            ldDataHelper.GetConversationIdFromConversationApi(getConversationIdResponse.Response);
                        var getConversationResponseUrl =
                            $"https://www.linkedin.com/voyager/api/messaging/conversations/{conversationId}/events";
                        var getConversationResponse = LdFunctions.GetInnerLdHttpHelper()
                            .HandleGetResponse(getConversationResponseUrl);
                        messageConversationResponseHandler =
                            new MessageConversationResponseHandler(getConversationResponse, tempLinkedInUser);
                        if (!messageConversationResponseHandler.attachmentDetails.Any())
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName,
                                ActivityType,
                                $"No attachments found with {tempLinkedInUser.FullName?.Replace("/", "")?.Replace("\"", "")}.");
                            continue;
                        }

                        messageConversationResponseHandler.ListMessageWithUserDetails =
                            messageConversationResponseHandler.ListMessageWithUserDetails
                                .Where(x => x.FileNameAndUrls.Count != 0).ToList();
                    }
                    else
                    {
                        var userpageResponse = LdFunctions.MessageDetails(getConversationIdUrl);
                        messageConversationResponseHandler = new MessageConversationResponseHandler(
                            new ResponseParameter {Response = userpageResponse},
                            tempLinkedInUser);
                        if (!messageConversationResponseHandler.attachmentDetails.Any())
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName,
                                ActivityType,
                                $"No attachments found with {tempLinkedInUser.FullName?.Replace("/", "")?.Replace("\"", "")}.");
                            continue;
                        }

                        messageConversationResponseHandler.ListMessageWithUserDetails =
                            messageConversationResponseHandler.ListMessageWithUserDetails
                                .Where(x => x.FileNameAndUrls.Count != 0).ToList();

                        if (messageConversationResponseHandler.ListMessageWithUserDetails.Count == 0)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName,
                                ActivityType,
                                $"No conversation found with {tempLinkedInUser.FullName?.Replace("/", "")?.Replace("\"", "")}.");
                            Thread.Sleep(new Random().Next(8000, 15000));
                            continue;
                        }
                    }

                    if (RemoveOrSkipAlreadyInteractedmessageLinkedInUsers(messageConversationResponseHandler
                        .ListMessageWithUserDetails))
                        continue;
                    messageConversationResponseHandler.ListMessageWithUserDetails.Reverse();

                    ProcessLinkedinUsersFromUserList(queryInfo, ref jobProcessResult,
                        messageConversationResponseHandler.ListMessageWithUserDetails);
                    if (MessageConversationScraperModel.IsDeleteAllConversations)
                    {
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName,
                            "Delete AllConversations",
                            $"Trying to delete all conversation of {tempLinkedInUser.FullName?.Replace("/", "")?.Replace("\"", "")}.");
                        var deleteApiUrl = IsBrowser
                            ? $"https://www.linkedin.com/messaging/thread/{conversationId}/"
                            : $"https://www.linkedin.com/voyager/api/messaging/conversations/{conversationId}";
                        var responseParams = LdFunctions.DeleteUserMessagesResponse(deleteApiUrl);
                        if (responseParams.Response == "")
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName,
                                "Delete AllConversations",
                                $"Successfully delete all Conversations of {tempLinkedInUser.FullName?.Replace("/", "")?.Replace("\"", "")}.");
                        }

                        else
                        {
                            var message = responseParams.Response.Contains("This profile is not available")
                                ? $"{linkedinUser.ProfileUrl} is not available"
                                : linkedinUser.ProfileUrl;
                            GlobusLogHelper.log.Info(Log.ActivityFailed,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, message);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    throw new OperationCanceledException("Operation Cancelled!");
                }
                catch (Exception exception)
                {
                    exception.DebugLog();
                }

            jobProcessResult.HasNoResult = true;
        }

        private List<LinkedinUser> RecentConversationsUsers(List<LinkedinUser> recentUser,
            List<Connections> connections, List<LinkedinUser> tempUserList)
        {
            ClassMapper.MapListOfModelClass(connections, ref tempUserList);
            var hasNoMoreResult = false;
            var apiAssist = new ApiAssist();
            long paginationTimeId = 0;
            paginationTimeId = DateTime.Now.GetCurrentEpochTimeMilliSeconds();
            while (!hasNoMoreResult)
            {
                var actionUrl = apiAssist.GetDeleteConversationApiUrl(paginationTimeId);
                string conversationReponse;
                if (IsBrowser)
                {
                    conversationReponse = LdFunctions.GetInnerHttpHelper().GetRequest(actionUrl).Response;
                    if (string.IsNullOrEmpty(conversationReponse))
                        conversationReponse = LdFunctions.GetInnerHttpHelper().GetRequest(actionUrl).Response;
                }
                else
                {
                    conversationReponse = LdFunctions.GetHtmlFromUrlNormalMobileRequest(actionUrl);
                    if (string.IsNullOrEmpty(conversationReponse))
                        conversationReponse = LdFunctions.GetHtmlFromUrlNormalMobileRequest(actionUrl);
                }


                if (conversationReponse.Contains("\"elements\":[]"))
                {
                    hasNoMoreResult = true;
                    break;
                }

                var jarray = new JsonJArrayHandler();

                var jobj = JObject.Parse(conversationReponse);
                var elements = jobj["elements"];

                foreach (var token in elements)
                {
                    var elementstoken = token;
                    var messageThreadId = jarray.GetJTokenValue(elementstoken, "entityUrn")
                        .Replace("urn:li:fs_conversation:", "");
                    var participants = jarray.GetJTokenOfJToken(elementstoken, "participants", 0,
                        "com.linkedin.voyager.messaging.MessagingMember");
                    var profileId = jarray.GetJTokenValue(participants, "entityUrn")
                        .Replace("urn:li:fs_messagingMember:", "").Replace($"{messageThreadId},", "").Replace("(", "")
                        .Replace(")", "");
                    var miniProfile = jarray.GetJTokenOfJToken(participants, "miniProfile");
                    var FullName = jarray.GetJTokenValue(miniProfile, "firstName");

                    var publicIdentifier = jarray.GetJTokenValue(miniProfile, "publicIdentifier");
                    if (FullName != "LinkedIn Member" && !string.IsNullOrEmpty(FullName))
                    {
                        var tempLinkedInUser = new LinkedinUser(publicIdentifier)
                        {
                            MessageThreadId = messageThreadId,
                            FullName = jarray.GetJTokenValue(miniProfile, "firstName") + " " +
                                       jarray.GetJTokenValue(miniProfile, "lastName"),
                            ProfileId = profileId,
                            PublicIdentifier = publicIdentifier
                        };
                        recentUser.Add(tempLinkedInUser);
                    }
                }

                paginationTimeId = long.Parse(jarray.GetJTokenValue(elements?.Last, "events", 0, "createdAt")) - 1;
            }

            return tempUserList;
        }


        public bool RemoveOrSkipAlreadyInteractedmessageLinkedInUsers(List<LinkedinUser> interactedLinkedInUsers)
        {
            try
            {
                //since we can send connection request only a user therefore here also taking connections request
                if (string.IsNullOrEmpty(LdJobProcess.CurrentCampaignId))
                {
                    var listInteractedUsersFromAccountDb = DbAccountService.GetInteractedUsers(ActivityTypeString);
                    interactedLinkedInUsers.RemoveAll(x =>
                        listInteractedUsersFromAccountDb.Any(y =>
                            x.attachmentDetails.Where(z => y.AttachmentId.Contains(z.Value)).Count() != 0));
                }
                else
                {
                    var listInteractedUsersFromCampaignD = DbCampaignService.GetInteractedUsers(ActivityTypeString);
                    interactedLinkedInUsers.RemoveAll(x =>
                        listInteractedUsersFromCampaignD.Any(y =>
                            x.attachmentDetails.Where(z => y.AttachmentId.Contains(z.Value)).Count() != 0));
                }


                if (interactedLinkedInUsers.Count <= 0)
                    return true;
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }

            return false;
        }
    }
}