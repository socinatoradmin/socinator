using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDModel.Messenger;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Utility;
using Newtonsoft.Json.Linq;

namespace LinkedDominatorCore.LDLibrary.Processor.Users.NonQueryType
{
    public class DeleteConversationsProcessor : BaseLinkedinUserProcessor, IQueryProcessor
    {
        public DeleteConversationsProcessor(ILdJobProcess jobProcess,
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
                BrowserAutomationExtension browserAutomation = null;
                long paginationTimeId = 0;
                var apiAssist = new ApiAssist();
                paginationTimeId = DateTime.Now.GetCurrentEpochTimeMilliSeconds();

                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    var actionUrl = IsBrowser
                        ? "https://www.linkedin.com/messaging/"
                        : apiAssist.GetDeleteConversationApiUrl(paginationTimeId);
                    var userList = new List<LinkedinUser>();
                    if (IsBrowser)
                    {
                        browserAutomation = new BrowserAutomationExtension(LdFunctions.BrowserWindow);
                        browserAutomation.LoadAndScroll(actionUrl, 15, true);
                        var browserPageResponse = LdFunctions.BrowserWindow.GetPageSource();
                        var list = HtmlAgilityHelper.GetListNodesFromClassName(browserPageResponse,
                            "ember-view  scaffold-layout__list-item msg-conversation-listitem msg-conversations-container__convo-item msg-conversations-container__pillar ");

                        foreach (var item in list)
                        {
                            var messageContent = HtmlAgilityHelper.GetListNodesFromAttibute(item.OuterHtml,"p",AttributeIdentifierType.ClassName,null, "msg-conversation-card__message-snippet")[0].InnerText;
                            messageContent = Utils.RemoveHtmlTags(messageContent);
                            var tempLinkedInUser = new LinkedinUser
                            {
                                MessageThreadId = Utilities.GetBetween(item.OuterHtml, "/messaging/thread/", "/"),
                                MessageContent = string.IsNullOrEmpty(messageContent) ? "N/A": messageContent?.Replace("\n"," ").Replace("\r"," ").Replace("\t"," ").Trim(),
                                FullName = Regex.Replace(
                                    Utils.RemoveHtmlTags(
                                        HtmlAgilityHelper.GetStringInnerHtmlFromClassName(item.OuterHtml,
                                            "msg-conversation-listitem__participant-names msg-conversation-card__participant-names truncate")),
                                    "<.*?>", string.Empty)
                            };
                            userList.Add(tempLinkedInUser);
                        }
                    }
                    else
                    {
                        var conversationReponse = LdFunctions.GetHtmlFromUrlNormalMobileRequest(actionUrl);
                        var jarray = JsonJArrayHandler.GetInstance;
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
                                .Replace("urn:li:fs_messagingMember:", "").Replace($"{messageThreadId},", "")
                                .Replace("(", "").Replace(")", "");
                            var miniProfile = jarray.GetJTokenOfJToken(participants, "miniProfile");
                            var publicIdentifier = jarray.GetJTokenValue(miniProfile, "publicIdentifier");
                            var messageContent = jarray.GetJTokenValue(elementstoken, "events",0, "eventContent", "com.linkedin.voyager.messaging.event.MessageEvent", "attributedBody", "text");
                            var tempLinkedInUser = new LinkedinUser(publicIdentifier)
                            {
                                MessageThreadId = messageThreadId,
                                FullName = jarray.GetJTokenValue(miniProfile, "firstName") + " " +
                                           jarray.GetJTokenValue(miniProfile, "lastName"),
                                MessageContent=string.IsNullOrEmpty(messageContent) ? "N/A" : messageContent,
                                ProfileId = profileId,
                                PublicIdentifier = publicIdentifier
                            };
                            userList.Add(tempLinkedInUser);
                        }

                        paginationTimeId = long.Parse(jarray.GetJTokenValue(elements?.Last, "events", 0, "createdAt")) -
                                           1;
                    }

                    ProcessLinkedinUsersFromUserList(QueryInfo.NoQuery, ref jobProcessResult, userList);
                    jobProcessResult.HasNoResult = userList.Count == 0;
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

        public string Replacer(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;
            try
            {
                var temp = Regex.Replace(input, @"[\(\)]:,", "");
                return temp;
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}