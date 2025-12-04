using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Utility;
using Newtonsoft.Json.Linq;

namespace LinkedDominatorCore.Response
{
    public class MessageConversationResponseHandler : LdResponseHandler
    {
        public List<KeyValuePair<string, string>> attachmentDetails = new List<KeyValuePair<string, string>>();

        
        public MessageConversationResponseHandler(IResponseParameter response, LinkedinUser Userdetails) :
            base(response)
        {
            try
            {
                Success = !string.IsNullOrEmpty(response?.Response) && (response.Response.Contains("com.linkedin.voyager.messaging.MessagingMember") || response.Response.Contains("<!DOCTYPE html>"));
                if (!Success || Response.ToString().Contains("element\":[]"))
                    return;
                if (response.Response.Contains("<!DOCTYPE html>"))
                {
                    var messages = HtmlAgilityHelper.GetListHtmlFromClassName(response.Response,
                        "msg-s-message-list__event clearfix");
                    var userName = "";
                    var publicidentifier = "";
                    foreach (var messageDetail in messages)
                    {
                        var messageDetails = HtmlAgilityHelper.GetListHtmlFromClassName(messageDetail,
                            "msg-s-event-listitem__options-trigger-icon");
                        if(messageDetails.Count<=0)
                        {
                            messageDetails = HtmlAgilityHelper.GetListHtmlFromClassName(messageDetail,
                                                        "msg-s-message-list__event clearfix");
                        }
                        if (messageDetails.Count() != 0)
                        {
                            userName = Userdetails.FullName;
                            publicidentifier = Userdetails.PublicIdentifier;
                        }

                        var attachmentname = HtmlAgilityHelper.GetListInnerHtmlFromClassName(messageDetail,
                            "msg-s-event-listitem__attachment-item msg-s-event-listitem__attachment-item--msg-fwd-enabled ember-view");
                        if (attachmentname.Count <= 0)
                            attachmentname = HtmlAgilityHelper.GetListHtmlFromClassName(messageDetail,
                                "msg-s-event-listitem__image-container");
                        if (attachmentname.Count <= 0)
                            attachmentname = HtmlAgilityHelper.GetListHtmlFromClassName(messageDetail, "msg-s-event-listitem__attachment-item--msg-fwd-enabled");
                        foreach (var attachmentDetails in attachmentname)
                        {
                            userName = Userdetails.FullName;
                            publicidentifier = Userdetails.PublicIdentifier;
                        }

                        var message = HtmlAgilityHelper.GetListInnerHtmlFromClassName(messageDetail,
                            "msg-s-event-listitem__body t-14 t-black--light t-normal").LastOrDefault();
                        var linkedinUser = new LinkedinUser
                        {
                            MessageContent = string.IsNullOrEmpty(message)? message : message.Replace("<!---->", ""),
                            Username = userName,
                            PublicIdentifier = publicidentifier
                        };
                        foreach (var attachment in attachmentname)
                        {
                            var attachmentdetails =WebUtility.HtmlDecode(Utils.GetBetween(attachment, "<h3 class=\"ui-attachment__filename\">", "</h3>"))?.Replace("\n","").Replace("\r","").Trim();
                            var imageurl =
                                HttpUtility.HtmlDecode(
                                    Utils.GetBetween(attachment, " <img src=\"", "\""));
                            if (string.IsNullOrEmpty(imageurl))
                            {
                                imageurl = Utils.GetBetween(attachment, "<a href=\"", "\" class=\"");
                            }
                            var attachname = Utils.GetBetween(attachment, "alt=\"", "\"").Trim();
                            if (string.IsNullOrEmpty(attachname))
                                attachname = attachmentdetails;
                            if (string.IsNullOrEmpty(attachname))
                                attachname = linkedinUser.MessageContent;
                            linkedinUser.FileNameAndUrls.Add(
                                new KeyValuePair<string, string>(attachname?.Replace("\n", ""), imageurl));
                            linkedinUser.attachmentDetails.Add(
                                new KeyValuePair<string, string>(attachname, imageurl));
                            attachmentDetails.Add(new KeyValuePair<string, string>(attachname,
                                imageurl));
                        }

                        ListMessageWithUserDetails.Add(linkedinUser);
                    }
                }
                else
                {
                    var jsonJArrayHandler = JsonJArrayHandler.GetInstance;
                    var messageJArray = jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJTokenValue(RespJ, "elements"));
                    foreach (var notificationItems in messageJArray)
                        try
                        {
                            var message = jsonJArrayHandler.GetJTokenValue(notificationItems, "eventContent",
                                "com.linkedin.voyager.messaging.event.MessageEvent", "attributedBody", "text");
                            var userfirstName = jsonJArrayHandler.GetJTokenValue(notificationItems, "from",
                                "com.linkedin.voyager.messaging.MessagingMember", "miniProfile", "firstName");
                            var userlastName = jsonJArrayHandler.GetJTokenValue(notificationItems, "from",
                                "com.linkedin.voyager.messaging.MessagingMember", "miniProfile", "lastName");
                            var userId = jsonJArrayHandler.GetJTokenValue(notificationItems, "from",
                                "com.linkedin.voyager.messaging.MessagingMember", "miniProfile", "publicIdentifier");
                            var userFullName = $"{userfirstName} {userlastName}";
                            var attachments = jsonJArrayHandler.GetTokenElement(notificationItems, "eventContent",
                                "com.linkedin.voyager.messaging.event.MessageEvent", "attachments");
                            var linkedinUser = new LinkedinUser
                            {
                                MessageContent = message,
                                Username = userFullName,
                                PublicIdentifier = userId
                            };
                            if (attachments != null)
                                foreach (var attach in attachments)
                                {
                                    var attachmentsFileName = jsonJArrayHandler.GetJTokenValue(attach, "name");
                                    var attachmentFileDownloadLink =
                                        jsonJArrayHandler.GetJTokenValue(attach, "reference", "string");
                                    var attachmentid = jsonJArrayHandler.GetJTokenValue(attach, "id");
                                    linkedinUser.FileNameAndUrls.Add(
                                        new KeyValuePair<string, string>(attachmentsFileName,
                                            attachmentFileDownloadLink));
                                    linkedinUser.attachmentDetails.Add(
                                        new KeyValuePair<string, string>(attachmentsFileName, attachmentid));
                                    attachmentDetails.Add(
                                        new KeyValuePair<string, string>(attachmentsFileName, attachmentid));
                                }

                            if (string.IsNullOrEmpty(message))
                                jsonJArrayHandler.GetJTokenValue(notificationItems, "content", "text");


                            long tempConnectedTimeStamp = 0;
                            long.TryParse(jsonJArrayHandler.GetJTokenValue(notificationItems, "createdAt"),
                                out tempConnectedTimeStamp);
                            linkedinUser.ConnectedTimeStamp = tempConnectedTimeStamp;

                            ListMessageWithUserDetails.Add(linkedinUser);
                        }

                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public List<LinkedinUser> ListMessageWithUserDetails { get; set; } = new List<LinkedinUser>();
        public string forBrowserSenderReciever { get; set; }
    }
}