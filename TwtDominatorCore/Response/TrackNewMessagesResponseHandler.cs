using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.Response
{
    public class TrackNewMessagesResponseHandler : TdBaseHtmlResponseHandler
    {
        public ObservableCollection<SenderDetails> ChatListNewMessage = new ObservableCollection<SenderDetails>();
        public bool HasMore;
        public List<TwitterUser> ListNewMessage = new List<TwitterUser>();
        public string MinPosition = string.Empty;


        public TrackNewMessagesResponseHandler(IResponseParameter response, bool IsPaginationResponse,
            bool isliveChat = false) : base(response)
        {
            if (!Success)
                return;
            if (response.Response.Contains("{\"inbox_initial_state\":{\""))
                BrowserNewUiResponseHandler(response, new List<TwitterUser>());
            else if (response.Response.Contains("<!DOCTYPE html>"))
                BrowserResponseHandler(response, isliveChat);
            else
                JsonresponseHandler(response, IsPaginationResponse, isliveChat);
        }

        private List<TwitterUser> BrowserNewUiResponseHandler(IResponseParameter response,
            List<TwitterUser> mySentMessages)
        {
            try
            {
                var ownUserId = Utilities.GetBetween(response.Response, "", "\"");
                var usernameLength = ownUserId.Length + 1;
                var Response = response.Response.Substring(usernameLength);
                var jsonhand = new Jsonhandler(Response);
                var messagesInbox = jsonhand.GetJToken("inbox_initial_state", "entries");
                var users = jsonhand.GetJToken("inbox_initial_state", "users");
                var pagination = jsonhand.GetJToken("inbox_initial_state", "inbox_timelines", "trusted");
                var status = jsonhand.GetJTokenValue(pagination, "status");
                MinPosition = jsonhand.GetJTokenValue(pagination, "min_entry_id");
                


                foreach (var singlemessages in messagesInbox)
                {
                    var messagedata = jsonhand.GetJTokenOfJToken(singlemessages, "message", "message_data");
                    var senderid = jsonhand.GetJTokenValue(messagedata, "sender_id");
                    var recipientid = jsonhand.GetJTokenValue(messagedata, "recipient_id");
                    var messageTimeStamp = jsonhand.GetJTokenValue(singlemessages, "message", "time");
                    var messagetext = jsonhand.GetJTokenValue(messagedata, "text");
                    foreach (var userdetails in users)
                    {
                        var Users = userdetails.First;
                        var userid = jsonhand.GetJTokenValue(Users, "id_str");

                        if (userid == senderid)
                        {
                            var username = jsonhand.GetJTokenValue(Users, "screen_name");
                            var fullname = jsonhand.GetJTokenValue(Users, "name");

                            var singleMessageUser = new TwitterUser
                            {
                                UserId = userid,
                                Username = username,
                                FullName = fullname,
                                Message = messagetext,
                                MessageTimeStamp = long.Parse(messageTimeStamp),
                                MessageRecievedUserId = recipientid
                            };

                            if (senderid == ownUserId)
                                mySentMessages.Add(singleMessageUser);
                            else
                                ListNewMessage.Add(singleMessageUser);
                            break;
                        }
                    }
                }


                // taking user last messages and comparing their timestamp with ours 

                ListNewMessage = ListNewMessage.OrderByDescending(x => x.MessageTimeStamp).ToList();
                ListNewMessage = ListNewMessage.GroupBy(x => x.Username).Select(y => y.FirstOrDefault()).ToList();

                mySentMessages = mySentMessages.OrderByDescending(x => x.MessageTimeStamp).ToList();
                mySentMessages = mySentMessages.GroupBy(x => x.MessageRecievedUserId).Select(y => y.FirstOrDefault())
                    .ToList();

                // since if our timestamp is greater than user means we already replied its new message
                ListNewMessage.RemoveAll(user => mySentMessages.Any(x =>
                    x.MessageRecievedUserId == user.UserId && x.MessageTimeStamp > user.MessageTimeStamp));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return mySentMessages;
        }

        private void JsonresponseHandler(IResponseParameter response, bool IsPaginationResponse,
            bool isliveChat = false)
        {
            var Messages = string.Empty;
            var messagesHtmlDoc = new HtmlDocument();
            if (IsPaginationResponse)
            {
                try
                {
                    var JsonObject = JObject.Parse(response.Response)["trusted"];
                    if (!JsonObject["is_empty"].ToString().Equals("True"))
                    {
                        Messages = JsonObject["html"].ToString();
                        MinPosition = JsonObject["min_entry_id"].ToString();
                        var hasmore = JsonObject["has_more"].ToString();
                        HasMore = hasmore.ToLower().Equals("true");
                    }
                }
                catch (Exception)
                {
                }
            }
            else
            {
                var JsonObject = JObject.Parse(response.Response)["inner"]["trusted"];
                Messages = JsonObject["html"].ToString();
                MinPosition = JsonObject["min_entry_id"].ToString();
                HasMore = true;
            }

            if (string.IsNullOrEmpty(Messages))
            {
                Success = false;
                return;
            }

            messagesHtmlDoc.LoadHtml(Messages);

            var ListMessages =
                HtmlAgilityHelper.getListInnerHtmlFromClassName(Messages, "DMInbox-conversationItem", messagesHtmlDoc);
            var ListConversations =
                HtmlAgilityHelper.getListInnerHtmlFromClassName(Messages, "DMInboxItem-snippet ", messagesHtmlDoc);
            var count = 0;
            foreach (var EachMessage in ListMessages)
            {
                ++count;

                if (!isliveChat && EachMessage.Contains("You:") || EachMessage.ToLower().Contains("you sent a"))
                    //  !EachMessage.Contains("DMInboxItem is-unread")
                    continue;
                var message = WebUtility.HtmlDecode(ListConversations[count - 1]).Trim();

                var singleMessageUser = new TwitterUser
                {
                    UserId = Utilities.GetBetween(EachMessage, "data-user-id=\"", "\""),
                    Username = HtmlAgilityHelper
                        .getStringInnerTextFromClassName(EachMessage, "username u-dir u-textTruncate")
                        .Replace("@", ""),
                    Message = message
                };

                var singleMessageUserdetail = new SenderDetails
                {
                    SenderId = Utilities.GetBetween(EachMessage, "data-user-id=\"", "\""),
                    SenderName = HtmlAgilityHelper
                        .getStringInnerTextFromClassName(EachMessage, "fullname"),

                    LastMesseges = message,
                    LastMessageOwnerId = Utilities.GetBetween(EachMessage, "data-last-message-id=\"", "\""),
                    ThreadId = Utilities.GetBetween(EachMessage, "data-thread-id=\"", "\""),
                    SenderImage = Utilities.GetBetween(EachMessage, "src=\"", "\""),
                    LastMessegedate = Utilities.GetBetween(EachMessage, "data-include-sec=\"true\">", "</span>")
                };
                ListNewMessage.Add(singleMessageUser);
                ChatListNewMessage.Add(singleMessageUserdetail);
            }
        }


        private void BrowserResponseHandler(IResponseParameter response, bool isliveChat = false)
        {
            var Messages = response.Response;
            var messagesHtmlDoc = new HtmlDocument();
            var ListMessages =
                HtmlAgilityHelper.getListInnerHtmlFromClassName(Messages, "DMInbox-conversationItem", messagesHtmlDoc);
            var ListConversations =
                HtmlAgilityHelper.getListInnerHtmlFromClassName(Messages, "DMInboxItem-snippet ", messagesHtmlDoc);
            var count = 0;
            foreach (var EachMessage in ListMessages)
            {
                ++count;
                if (!isliveChat && EachMessage.Contains("You:") || EachMessage.ToLower().Contains("you sent a")
                ) //  !EachMessage.Contains("DMInboxItem is-unread")
                    continue;
                var message = WebUtility.HtmlDecode(ListConversations[count - 1]).Trim();

                var singleMessageUser = new TwitterUser
                {
                    UserId = Utilities.GetBetween(EachMessage, "data-user-id=\"", "\""),
                    Username = HtmlAgilityHelper
                        .getStringInnerTextFromClassName(EachMessage, "username u-dir u-textTruncate")
                        .Replace("@", ""),
                    Message = message
                };
                ListNewMessage.Add(singleMessageUser);
            }
        }
    }
}