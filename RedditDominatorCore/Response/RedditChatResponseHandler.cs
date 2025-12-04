using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using RedditDominatorCore.RDLibrary;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RedditDominatorCore.Response
{
    public class RedditChatResponseHandler : RdResponseHandler
    {
        public List<string> ChatChannelUrls = new List<string>();
        public RedditUser user = new RedditUser();
        public List<ConversationDetails> Conversation = new List<ConversationDetails>();
        private Dictionary<int, string> RoomTypes = new Dictionary<int, string> { { 0, "join" }, { 1, "invite" }, { 2, "peek" }, { 3, "leave" } };
        public RedditChatResponseHandler(IResponseParameter response, string purpose, RedditUser redditUser = null) : base(response)
        {
            try
            {
                if (purpose == "ChatUrls")
                {
                    var document = new HtmlDocument();
                    document.LoadHtml(response.Response);
                    var data = Regex.Match(document.ParsedText, "<div class(.*?)html>").Value;

                    HtmlNode[] userChatNodes = document.DocumentNode.SelectNodes("//a[@class=\'_3X4hbg4asgVvLaVYU6dUzs \']").ToArray();
                    foreach (var urlString in userChatNodes)
                    {
                        var dupUrlStrig = Utils.GetBetween(urlString.OuterHtml, "href=\"", "\"");
                        ChatChannelUrls.Add("https://www.reddit.com" + dupUrlStrig);

                    }
                }
                else if (purpose == "LastMessage")
                {

                    var document = new HtmlDocument();
                    document.LoadHtml(response.Response);
                    var userNameNodes = document.DocumentNode.SelectNodes("//span[@class='_2MAeUeEw0C9eRjZ5ETlKtA']").ToArray();
                    var data = Regex.Match(document.ParsedText, "<div class(.*?)html>").Value;
                    var User = userNameNodes[0].InnerText;
                    var OwnUser = userNameNodes[0].InnerText;
                    var Data1 = Regex.Match(document.ParsedText, "<div class(.*?)html>").Value;
                    var MessageNodes = document.DocumentNode.SelectNodes("//div[@class='lh9ssPWZKUR3-eXgoIPnX']").ToArray();
                    var Data = Regex.Match(document.ParsedText, "<div class(.*?)html>").Value;
                    var Message = document.DocumentNode.SelectNodes("//div[@class=\'lh9ssPWZKUR3-eXgoIPnX\']").LastOrDefault().InnerText.ToString();
                    string userName = MessageNodes.LastOrDefault().OuterHtml.Contains(OwnUser) ? OwnUser : User;

                    user.Username = userName;
                    user.Text = Message.ToString();
                }
                else if (redditUser != null)
                {
                    //Scrap Messages.
                    JToken Rooms = null;
                    var MessageRooms = new List<RoomDetails>();
                    string roomType = string.Empty;
                    var jsonObject = jsonHandler.ParseJsonToJObject(response.Response);
                    for (int i = 0; i < RoomTypes.Count; i++)
                    {
                        RoomTypes.TryGetValue(i, out roomType);
                        Rooms = jsonHandler.GetJTokenOfJToken(jsonObject, "rooms", roomType);
                        if (Rooms.HasValues)
                            MessageRooms.Add(new RoomDetails { RoomType = roomType, Rooms = Rooms });
                    }
                    var rooms = new List<RoomDetails>();
                    foreach (var room in MessageRooms)
                    {
                        if (room != null && room.Rooms.HasValues)
                        {
                            foreach (var item in room.Rooms)
                            {
                                var RoomID = Utils.GetBetween(item.Path.ToString(), "[\'", "\']")?.Replace("\"", "");
                                rooms.Add(new RoomDetails { Rooms = item, RoomID = RoomID, RoomType = room.RoomType });
                            }
                        }
                    }
                    if (rooms.Count > 0)
                    {
                        foreach (var room in rooms)
                        {
                            var conversation = new ConversationDetails();
                            conversation.RoomID = room.RoomID;
                            conversation.PaginationToken = jsonHandler.GetJTokenValue(jsonObject, "next_batch");
                            int.TryParse(jsonHandler.GetJTokenValue(jsonObject, "rooms", room.RoomType, room.RoomID, "unread_notifications", "notification_count"), out int UnreadNotificationCount);
                            conversation.UnreadMessageCount = UnreadNotificationCount;
                            var Messages = jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(jsonObject, "rooms", room.RoomType, room.RoomID, "timeline", "events"));
                            Messages = Messages == null || !Messages.HasValues ? jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(jsonObject, "rooms", room.RoomType, room.RoomID, "invite_state", "events")) : Messages;
                            if (Messages != null && Messages.HasValues)
                            {
                                foreach (var messageDetails in Messages)
                                {
                                    var type = jsonHandler.GetJTokenValue(messageDetails, "type");
                                    if (string.IsNullOrEmpty(type) ? false : type.Contains("m.room.create"))
                                        conversation.RoomCreatorID = jsonHandler.GetJTokenValue(messageDetails, "content", "creator")?.Replace(":reddit.com", "")?.Replace("@", "");
                                    if (string.IsNullOrEmpty(conversation.username))
                                    {
                                        var userName = jsonHandler.GetJTokenValue(messageDetails, "content", "displayname");
                                        AssignUserName(userName, redditUser.Username, conversation);
                                        var UnsignedUser = jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(messageDetails, "unsigned", "invite_room_state"));
                                        if (UnsignedUser != null && UnsignedUser.HasValues)
                                        {
                                            foreach (var user in UnsignedUser)
                                            {
                                                userName = jsonHandler.GetJTokenValue(messageDetails, "content", "displayname");
                                                AssignUserName(userName, redditUser.Username, conversation);
                                            }
                                        }
                                    }
                                    if (string.IsNullOrEmpty(type) ? false : type.Contains("m.room.message"))
                                    {
                                        var senderID = jsonHandler.GetJTokenValue(messageDetails, "sender")?.Replace(":reddit.com", "")?.Replace("@", "");
                                        var eventID = jsonHandler.GetJTokenValue(messageDetails, "event_id");
                                        long.TryParse(jsonHandler.GetJTokenValue(messageDetails, "origin_server_ts"), out long time);
                                        var messageTimeStamp = time <= 0 ? DateTime.Now : DateTimeUtilities.EpochToDateTimeLocal(time);
                                        var msg = jsonHandler.GetJTokenValue(messageDetails, "content", "body");
                                        if (!string.IsNullOrEmpty(msg))
                                            conversation.Messages.Add(new MessageInfo { Message = msg, SenderID = senderID, EventID = eventID, MessageTimeStamp = messageTimeStamp, IsPending = room.RoomType == "peek" });
                                    }
                                }
                            }
                            var events = jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(jsonObject, "rooms", room.RoomType, room.RoomID, "state", "events"));
                            if (events != null && events.HasValues)
                            {
                                foreach (var Event in events)
                                {
                                    var userName = jsonHandler.GetJTokenValue(Event, "content", "displayname");
                                    AssignUserName(userName, redditUser.Username, conversation);
                                }
                            }
                            conversation.username = string.IsNullOrEmpty(conversation.username) ? redditUser.Username : conversation.username;
                            conversation.ProfileUrl = string.IsNullOrEmpty(conversation.ProfileUrl) ? $"https://www.reddit.com/user/{redditUser.Username}/" : conversation.ProfileUrl;
                            Conversation.Add(conversation);
                        }
                        rooms.Clear();
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void AssignUserName(string userName, string OwnerName, ConversationDetails conversation)
        {
            if (!string.IsNullOrEmpty(userName) && userName != OwnerName && string.IsNullOrEmpty(conversation.username))
            {
                conversation.username = userName;
                conversation.ProfileUrl = $"https://www.reddit.com/user/{userName}/";
            }
        }

        public PaginationParameter PaginationParameter { get; set; }
        public class RoomDetails
        {
            public string RoomType { get; set; }
            public string RoomID { get; set; }
            public JToken Rooms { get; set; }
            public bool IsPending { get; set; }
        }
    }
}