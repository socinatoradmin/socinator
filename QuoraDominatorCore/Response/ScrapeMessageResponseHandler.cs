using System;
using System.Collections.Generic;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.QdUtility;

namespace QuoraDominatorCore.Response
{
    public class ScrapeMessageResponseHandler : QuoraResponseHandler
    {
        public List<string> AllMessageIdList;
        public List<string> LstMessageDateTime;

        public List<MessageDetails> LstMessageDetails = new List<MessageDetails>();
        public List<ChatDetails> LstChatDetails = new List<ChatDetails>();
        public List<string> LstUserIdMessageId;
        public List<string> NotRepliedMessageIdList;
        public List<string> RepliedMessageIdList;
        public List<string> UnreadMessageIdList;
        public string Url;
        public int PaginationCount { get; set; }
        public bool HasMoreResult { get; set; } = true;
        public ScrapeMessageResponseHandler(IResponseParameter response,bool IsReadAllMessage=false,bool IsReadAllUsers=false,string UserId="") : base(response)
        {
            try
            {
                if (IsReadAllMessage)
                {
                    HasMoreResult = false;
                    var jsonObject = RespJ is null ? jsonHandler.ParseJsonToJObject(response.Response) : RespJ;
                    var Threads=jsonHandler.GetJTokenOfJToken(jsonObject,"data","thread");
                    var AllMessages=jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(Threads, "allRecentMessages"));
                    if (AllMessages != null && AllMessages.HasValues)
                        AllMessages.ForEach(messageToken =>
                        {
                            var messageString = jsonHandler.GetJTokenValue(messageToken, "contentQtextDocument", "legacyJson");
                            var chatUserName = jsonHandler.GetJTokenValue(messageToken, "author", "names", 0, "givenName") + " " + jsonHandler.GetJTokenValue(messageToken, "author", "names", 0, "familyName");
                            var messageObject = jsonHandler.ParseJsonToJObject(messageString);
                            var messageArray = jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(messageObject, "sections"));
                            if (messageArray != null && messageArray.HasValues)
                                messageArray.ForEach(messageTokenItem =>
                                {
                                    var messageType = jsonHandler.GetJTokenValue(messageTokenItem, "type")?.Trim();
                                    var senderId=jsonHandler.GetJTokenValue(messageToken, "author", "uid")?.Trim();
                                    LstChatDetails.Add(new ChatDetails
                                    {
                                        Sender = chatUserName,
                                        SenderId = senderId,
                                        Time = jsonHandler.GetJTokenValue(messageToken, "time"),
                                        MessegesId = jsonHandler.GetJTokenValue(messageToken, "msgId"),
                                        Messeges = jsonHandler.GetJTokenValue(messageTokenItem, "spans",0,"text"),
                                        MessegeType = messageType=="plain"?ChatMessageType.Text:ChatMessageType.Media,
                                        IsRecieved=senderId!=UserId
                                    });
                                    LstMessageDetails.Add(new MessageDetails
                                    {
                                        UserProfilePic = jsonHandler.GetJTokenValue(messageToken,"author", "profileImageUrl"),
                                        LastMessage = jsonHandler.GetJTokenValue(messageTokenItem, "spans", 0, "text"),
                                        UserFullName = chatUserName,
                                        MessageDateTime = jsonHandler.GetJTokenValue(messageToken, "time"),
                                        MessageId = jsonHandler.GetJTokenValue(messageToken, "msgId"),
                                        UserId = senderId,
                                        UserProfileUrl = $"{QdConstants.HomePageUrl}{jsonHandler.GetJTokenValue(messageToken,"author", "profileUrl")}"
                                    });
                                });
                        });
                    else
                    {
                        var RecentMessage = jsonHandler.GetJTokenOfJToken(Threads, "latestMessage");
                        var messageString = jsonHandler.GetJTokenValue(RecentMessage, "contentQtextDocument", "legacyJson");
                        var chatUserName = jsonHandler.GetJTokenValue(RecentMessage, "author", "names", 0, "givenName") + " " + jsonHandler.GetJTokenValue(RecentMessage, "author", "names", 0, "familyName");
                        var messageObject = jsonHandler.ParseJsonToJObject(messageString);
                        var messageArray = jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(messageObject, "sections"));
                        if (messageArray != null && messageArray.HasValues)
                            messageArray.ForEach(messageTokenItem =>
                            {
                                var messageType = jsonHandler.GetJTokenValue(messageTokenItem, "type")?.Trim();
                                var senderId = jsonHandler.GetJTokenValue(Threads, "otherUsers",0, "uid")?.Trim();
                                LstChatDetails.Add(new ChatDetails
                                {
                                    Sender = chatUserName,
                                    SenderId = senderId,
                                    Time = jsonHandler.GetJTokenValue(RecentMessage, "time"),
                                    MessegesId = jsonHandler.GetJTokenValue(RecentMessage, "msgId"),
                                    Messeges = jsonHandler.GetJTokenValue(messageTokenItem, "spans", 0, "text"),
                                    MessegeType = messageType == "plain" ? ChatMessageType.Text : ChatMessageType.Media,
                                    IsRecieved = senderId != UserId
                                });
                                LstMessageDetails.Add(new MessageDetails
                                {
                                    UserProfilePic = "",
                                    LastMessage = jsonHandler.GetJTokenValue(messageTokenItem, "spans", 0, "text"),
                                    UserFullName = chatUserName,
                                    MessageDateTime = jsonHandler.GetJTokenValue(RecentMessage, "time"),
                                    MessageId = jsonHandler.GetJTokenValue(RecentMessage, "msgId"),
                                    UserId = senderId,
                                    UserProfileUrl = $"{QdConstants.HomePageUrl}{jsonHandler.GetJTokenValue(Threads, "otherUsers",0, "profileUrl")}"
                                });
                            });
                    }
                }
                else
                {
                    var jsonObject = RespJ is null ? jsonHandler.ParseJsonToJObject(response.Response) : RespJ;
                    var eachmesg = jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(jsonObject, "data", "viewer", "threadsConnection", "edges"));
                    var PageInfo = jsonHandler.GetJTokenOfJToken(jsonObject, "data", "viewer", "threadsConnection", "pageInfo");
                    int.TryParse(jsonHandler.GetJTokenValue(PageInfo, "endCursor"),out int CursorPostion);
                    bool.TryParse(jsonHandler.GetJTokenValue(PageInfo, "hasNextPage"), out bool HasNextPage);
                    HasMoreResult = HasNextPage;
                    PaginationCount = CursorPostion;
                    foreach (var eachnode in eachmesg)
                    {
                        try
                        {
                            var userDetails = jsonHandler.GetJTokenOfJToken(eachnode, "node", "otherUsers",0);
                            var chatUsername = jsonHandler.GetJTokenValue(userDetails, "names",0, "givenName") +" "+ jsonHandler.GetJTokenValue(userDetails, "names", 0, "familyName");
                            var chatUserId = jsonHandler.GetJTokenValue(userDetails, "uid");
                            var profileImage = jsonHandler.GetJTokenValue(userDetails, "profileImageUrl");
                            var profileimglURL = string.IsNullOrEmpty(profileImage)?jsonHandler.GetJTokenValue(userDetails, "smallProfileImageUrl") :profileImage;
                            var messageDetails = jsonHandler.GetJTokenOfJToken(eachnode, "node");
                            var messageString = jsonHandler.GetJTokenValue(messageDetails, "latestMessage", "contentQtextDocument", "legacyJson");
                            var messageObject = jsonHandler.ParseJsonToJObject(messageString);
                            var profilelURL = jsonHandler.GetJTokenValue(userDetails, "profileUrl");
                            var lastmesg = jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(messageObject, "sections"));
                            foreach (var eachSec in lastmesg)
                            {
                                try
                                {
                                    var message = string.Empty;
                                    var type = jsonHandler.GetJTokenValue(eachSec, "type");
                                    bool.TryParse(jsonHandler.GetJTokenValue(messageDetails, "isUnreadForViewer"), out bool isReceived);
                                    if (type == "plain") message += jsonHandler.GetJTokenValue(eachSec, "spans",0, "text");
                                    else if (type == "image") message = "image";
                                    if (isReceived || IsReadAllUsers)
                                    {
                                        LstMessageDetails.Add(new MessageDetails
                                        {
                                            UserProfilePic = profileimglURL,
                                            LastMessage = message,
                                            UserFullName = chatUsername,
                                            MessageDateTime = jsonHandler.GetJTokenValue(messageDetails, "latestMessage", "time"),
                                            MessageId = jsonHandler.GetJTokenValue(messageDetails, "threadId"),
                                            UserId = chatUserId,
                                            UserProfileUrl =$"{QdConstants.HomePageUrl}{profilelURL}"
                                        });

                                        LstChatDetails.Add(new ChatDetails
                                        {
                                            Sender = chatUsername,
                                            SenderId = chatUserId,
                                            Messeges = message,
                                            Time = jsonHandler.GetJTokenValue(messageDetails, "latestMessage", "time"),
                                            MessegesId = jsonHandler.GetJTokenValue(messageDetails, "threadId"),
                                            Type = type,
                                            IsRecieved = isReceived
                                        });
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }

                        }
                        catch (Exception) { }

                    }
                    LstMessageDetails.Reverse();
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    public class MessageDetails
    {
        public string UserProfileUrl;
        public string LastMessage;
        public string MessageDateTime;
        public string MessageId;
        public string UserFullName;
        public string UserId;
        public string UserProfilePic;
        public List<string> imgURLs = new List<string>();
    }
}