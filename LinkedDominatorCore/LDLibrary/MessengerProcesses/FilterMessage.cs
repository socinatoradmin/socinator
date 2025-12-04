using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.DetailedInfo;
using LinkedDominatorCore.Enums;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDModel.Messenger;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Utility;

namespace LinkedDominatorCore.LDLibrary.MessengerProcesses
{
    public class FilterMessage
    {
        private readonly IDelayService _delayService;


        private readonly BroadcastMessagesModel BroadcastMessagesModel;

        private readonly JsonJArrayHandler handler = JsonJArrayHandler.GetInstance;
        public bool IsShowFilterMessage = true;

        public FilterMessage(BroadcastMessagesModel broadcastMessagesModel, IDelayService delayService)
        {
            BroadcastMessagesModel = broadcastMessagesModel;
            _delayService = delayService;
        }

        public FilterMessage(IDelayService delayService)
        {
            _delayService = delayService;
        }

        public string GetMessage(DominatorAccountModel dominatorAccountModel, LinkedinUser objLinkedinUser,
            string textMessage, ILdFunctions ldFunctions, ref string imageSource)
        {
            string finalMessage;

            var senderUserDetailedInfo =
                new DetailsFetcher(_delayService).GetUserScraperDetailedInfo(dominatorAccountModel);
            var recipientUserDetailedInfo = new UserScraperDetailedInfo();

            ReciepientDetails(recipientUserDetailedInfo, objLinkedinUser, ldFunctions, BroadcastMessagesModel);
            if (BroadcastMessagesModel.IsFollower)
                objLinkedinUser.SelectedSource = "Own Followers";
            finalMessage = GetRandomMessage(objLinkedinUser.SelectedSource, ref imageSource);
            if (string.IsNullOrEmpty(finalMessage))
                finalMessage = textMessage;

            #region IsChkSpintaxChecked

            if (BroadcastMessagesModel.IsChkSpintaxChecked)
                finalMessage = GetFinalMessage(finalMessage);

            #endregion

            finalMessage = GetTaggedMessage(finalMessage, recipientUserDetailedInfo, senderUserDetailedInfo);
            return finalMessage?.Replace("\"","\\\"");
        }

        public bool FilterMessageFromConversationHistory(ILdFunctions ldFunctions, LinkedinUser objLinkedinUser,
            DominatorAccountModel dominatorAccountModel, ActivityType activityType, string finalMessage)
        {
            try
            {
                #region Skip Duplicate Message By Checking Conversations

                string messageToBeReviewedBeforeSending;
                var IsFiltered = ldFunctions.IsBrowser
                    ? CheckConversationFromBrowser(ldFunctions, objLinkedinUser, finalMessage,dominatorAccountModel,
                        out messageToBeReviewedBeforeSending)
                    : CheckConversation(ldFunctions, objLinkedinUser, finalMessage,dominatorAccountModel,out _);
                if (IsFiltered)
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        dominatorAccountModel.AccountBaseModel.UserName, activityType,
                        "already sent message to " + objLinkedinUser.FullName + "");
                return IsFiltered;

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }

        public bool CheckConversation(ILdFunctions ldFunctions, LinkedinUser objLinkedinUser,string finalMessage,DominatorAccountModel dominatorAccount,out string UpdatedMessage)
        {
            try
            {
                var flagship3ProfileViewBase = string.Empty;
                var responseConversations = string.Empty;
                UpdatedMessage = finalMessage?.Replace("\r\n", "\\n").Replace("\"", "\\\"");
                var Conversations = new List<string>();
                var actionUrl =
                    "https://www.linkedin.com/voyager/api/messaging/conversations?keyVersion=LEGACY_INBOX&q=participants&recipients=List(" +
                    objLinkedinUser.ProfileId + ")";
                var responseMessageButtonClick =
                    ldFunctions.GetHtmlFromUrlForMobileRequest(actionUrl, flagship3ProfileViewBase);
                var conversationThreadId = Utils.GetBetween(responseMessageButtonClick, "urn:li:fs_conversation:", "\"");
                Thread.Sleep(TimeSpan.FromSeconds(2));
                if (string.IsNullOrEmpty(conversationThreadId))
                    return false;
                var conversationActionUrl = "https://www.linkedin.com/voyager/api/messaging/conversations/" +
                                            conversationThreadId +
                                            "/events";
                responseConversations =
                    ldFunctions.GetHtmlFromUrlForMobileRequest(conversationActionUrl, flagship3ProfileViewBase);
                
                if (!string.IsNullOrEmpty(responseConversations))
                {
                    var jObject = handler.ParseJsonToJObject(responseConversations);
                    foreach (var element in handler.GetJArrayElement(handler.GetJTokenValue(jObject, "elements")))
                    {
                        var publicidentifier = handler.GetJTokenValue(element, "from", "com.linkedin.voyager.messaging.MessagingMember", "miniProfile", "publicIdentifier");
                        if (!string.IsNullOrEmpty(publicidentifier))
                            Conversations.Add(publicidentifier);
                    }
                    Conversations = Conversations?.Distinct()?.ToList();
                }
                return Conversations?.Count > 0 && Conversations.Any(z => z == objLinkedinUser?.PublicIdentifier || z == dominatorAccount.AccountBaseModel.ProfilePictureUrl?.Split('/')?.LastOrDefault(x=>x!=string.Empty));
            }
            catch (Exception ex)
            {
                UpdatedMessage = finalMessage?.Replace("\r\n", "\\n").Replace("\"", "\\\"");
                ex.DebugLog();
                return false;
            }
        }

        public bool CheckConversationFromBrowser(ILdFunctions ldFunctions, LinkedinUser objLinkedinUser,
            string finalMessage,DominatorAccountModel dominatorAccount,
            out string messageToBeReviewedBeforeSending)
        {
            var responseConversations = "";
            messageToBeReviewedBeforeSending = finalMessage;
            bool IsFiltered = false;
            try
            {
                var _automationExtension = new BrowserAutomationExtension(ldFunctions.BrowserWindow);
                _automationExtension.LoadPageUrlAndWait(objLinkedinUser.ProfileUrl, 15);
                if (!_automationExtension.LoadAndClick(AttributeTypes.Button, AttributeIdentifierType.Id,
                    "Message"))
                    if (!_automationExtension.LoadAndClick(AttributeTypes.Button, AttributeIdentifierType.ClassName,
                        "Message"))
                        _automationExtension.LoadAndClick(AttributeTypes.Button, AttributeIdentifierType.ClassName,LDClassesConstant.Messenger.MessageButtonClass);
                Thread.Sleep(2000);
                var success=_automationExtension.ExecuteScript("document.getElementsByClassName('message-anywhere-button pvs-profile-actions__action artdeco-button')[0].click()");
                Thread.Sleep(2000);
                _automationExtension.ExecuteScript(
                    "document.getElementsByClassName('msg-s-message-list full-width msg-s-message-list--scroll-buffer ember-view')[0].scrollTop -=5000",
                    10);
                var pageSource = WebUtility.HtmlDecode(ldFunctions.BrowserWindow.GetPageSource());
                var nodeList = HtmlAgilityHelper.GetListNodesFromClassName(pageSource,
                    "msg-s-event-listitem__message-bubble msg-s-event-listitem__message-bubble--msg-fwd-enabled");               
                var firstName =
                    ldFunctions.BrowserWindow.DominatorAccountModel.AccountBaseModel.UserFullName.Split(' ')[0];
                var message = $"from {firstName}";
                foreach (var htmlNode in nodeList)
                {
                    if (!htmlNode.OuterHtml.Contains(message))
                        continue;
                    if (IsFiltered = htmlNode.OuterHtml.Contains(finalMessage))
                        return IsFiltered;
                    responseConversations += $"{Utils.RemoveHtmlTags(htmlNode.InnerHtml)} \n";
                }
                //if in chat conversation happen and responseconversation is getting empty
                return nodeList.Count > 0 && string.IsNullOrEmpty(responseConversations);
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
            return IsFiltered;
        }

        public void ReciepientDetails(UserScraperDetailedInfo recipientUserDetailedInfo, LinkedinUser objLinkedinUser,
            ILdFunctions ldFunctions, BroadcastMessagesModel BroadcastMessagesModel)
        {
            #region Recipient Details

            try
            {
                recipientUserDetailedInfo.AccountUserFullName = objLinkedinUser.FullName;
                var splitedFullName = Regex.Split(recipientUserDetailedInfo.AccountUserFullName, " ");

                if (splitedFullName.Length > 2)
                {
                    recipientUserDetailedInfo.Firstname = splitedFullName[0]?.Replace("\"", "");
                    recipientUserDetailedInfo.Lastname = $"{splitedFullName[1]} {splitedFullName[2]}";
                }
                else
                {
                    recipientUserDetailedInfo.Firstname = splitedFullName[0];
                    recipientUserDetailedInfo.Lastname = $" {splitedFullName[1]}";
                }

                #region Forcomapny Details
                if (!string.IsNullOrEmpty(objLinkedinUser.Occupation))
                {
                    var occupationandcompany = Regex.Split(objLinkedinUser.Occupation, " at");
                    if (occupationandcompany.Count() == 2)
                        recipientUserDetailedInfo.CompanyCurrent = occupationandcompany[1];
                }
                if (string.IsNullOrEmpty(recipientUserDetailedInfo.CompanyCurrent))
                {
                    var message = BroadcastMessagesModel.LstDisplayManageMessagesModel
                                .Select(x => x.MessagesText).ToList().GetRandomItem();
                    //if it tagged company name then only hit reponse to avoid blocking issue
                    if (BroadcastMessagesModel.IsChkTagChecked &&
                        (message.Contains("<Campany Name>") || message.Contains("<Campany name>")))
                    {
                        var res = ldFunctions.GetInnerHttpHelper().GetRequest($"https://www.linkedin.com/voyager/api/identity/dash/profiles?q=memberIdentity&memberIdentity={objLinkedinUser.ProfileId}&decorationId=com.linkedin.voyager.dash.deco.identity.profile.FullProfileWithEntities-35");
                        JsonHandler json = new JsonHandler(res.Response);
                        recipientUserDetailedInfo.CompanyCurrent = json.GetElementValue("elements", 0, "profilePositionGroups", "elements", 0, "companyName");
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion
        }


        public string GetRandomMessage(string selectedSource, ref string imageSource)
        {
            try
            {
                if (string.IsNullOrEmpty(selectedSource))
                    selectedSource = "Custom Group Url";

                var message = BroadcastMessagesModel.LstDisplayManageMessagesModel
                    .Where(x => x.SelectedQuery.Any(y => y.Content.QueryValue.ToString() == selectedSource))
                    .Select(x => x.MessagesText).ToList().GetRandomItem();
                //try
                //{
                //    imageSource = BroadcastMessagesModel.LstDisplayManageMessagesModel
                //        .Where(x => x.SelectedQuery.Any(y => y.Content.QueryValue.ToString() == selectedSource))
                //        .Select(x => x.MediaPath).ToList().GetRandomItem();
                //}
                //catch (Exception ex)
                //{
                //    ex.DebugLog();
                //}

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

                var messageContent = !string.IsNullOrEmpty(imageSource) ? message + "<:>" + imageSource : message;

                return messageContent;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public string GetFinalMessage(string textMessage)
        {
            string finalMessage;
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
                finalMessage = lstMessages[new RandomNumberGenerator().GenerateRandom(0, lstMessages.Count - 1)];
            }
            catch (Exception ex)
            {
                finalMessage = textMessage;
                ex.DebugLog();
            }

            return finalMessage;
        }

        public string GetTaggedMessage(string finalMessage, UserScraperDetailedInfo recipientUserDetailedInfo,
            UserScraperDetailedInfo senderUserDetailedInfo)
        {
            try
            {
                if (BroadcastMessagesModel.IsChkTagChecked)
                {
                    var fullName = string.IsNullOrEmpty(recipientUserDetailedInfo.AccountUserFullName) ?recipientUserDetailedInfo.Firstname+" "+recipientUserDetailedInfo.Lastname: recipientUserDetailedInfo.AccountUserFullName;
                    #region FinalMessage With Recipient's Tags

                    finalMessage = finalMessage.Replace("<First Name>", recipientUserDetailedInfo.Firstname);
                    finalMessage = finalMessage.Replace("<Last Name>", recipientUserDetailedInfo.Lastname);
                    finalMessage = finalMessage.Replace("<Full Name>",fullName);
                    finalMessage = finalMessage.Replace("<Position>", recipientUserDetailedInfo.Occupation.Trim());
                    finalMessage = finalMessage.Replace("<Company Name>", recipientUserDetailedInfo.CompanyCurrent.Trim());
                    finalMessage = finalMessage.Replace("<Company name>", recipientUserDetailedInfo.CompanyCurrent.Trim());

                    #endregion

                    #region FinalMessage With Sender's Tags
                    fullName = string.IsNullOrEmpty(senderUserDetailedInfo.AccountUserFullName) ?senderUserDetailedInfo.Firstname+" "+senderUserDetailedInfo.Lastname: senderUserDetailedInfo.AccountUserFullName;
                    finalMessage = finalMessage.Replace("<From First Name>", senderUserDetailedInfo.Firstname);
                    finalMessage = finalMessage.Replace("<From Last Name>", senderUserDetailedInfo.Lastname);
                    finalMessage = finalMessage.Replace("<From Full Name>",fullName);
                    finalMessage = finalMessage.Replace("<From Company Name>", senderUserDetailedInfo.CompanyCurrent);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return finalMessage;
        }
    }
}