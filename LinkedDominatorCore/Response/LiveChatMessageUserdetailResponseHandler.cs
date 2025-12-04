using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Utility;
using Newtonsoft.Json.Linq;

namespace LinkedDominatorCore.Response
{
    public class LiveChatMessageUserdetailResponseHandler : LdResponseHandler
    {
        public ObservableCollection<SenderDetails> ChatListNewMessage = new ObservableCollection<SenderDetails>();
        public List<string> ListConversationId = new List<string>();

        public LiveChatMessageUserdetailResponseHandler(IResponseParameter response)
            : base(response)
        {
            try

            {
                var _jsonJArrayHandler = JsonJArrayHandler.GetInstance;
                var getUsers = _jsonJArrayHandler.GetJArrayElement(_jsonJArrayHandler.GetJTokenValue(RespJ, "elements"));
                var loopCount = 0;
                foreach (var item in getUsers)
                {
                    var lastmessage = _jsonJArrayHandler.GetJTokenValue(item, "events", 0, "eventContent",
                        "com.linkedin.voyager.messaging.event.MessageEvent", "attributedBody", "text");
                    if (string.IsNullOrEmpty(lastmessage))
                        continue;
                    loopCount = ++loopCount;
                    var id = _jsonJArrayHandler.GetJTokenValue(item, "entityUrn");
                    var conversationId =
                        Utils.GetBetween($"{id}/", "urn:li:fs_conversation:", "/");
                    var participantsFirstname = _jsonJArrayHandler.GetJTokenValue(item, "participants", 0,
                        "com.linkedin.voyager.messaging.MessagingMember", "miniProfile", "firstName");
                    var participantsLastname = _jsonJArrayHandler.GetJTokenValue(item, "participants", 0,
                        "com.linkedin.voyager.messaging.MessagingMember", "miniProfile", "lastName");
                    var participantsFullName = $"{participantsFirstname} {participantsLastname}";
                    var Lastmessagetimespan = _jsonJArrayHandler.GetJTokenValue(item, "events", 0, "createdAt");
                    var participantsprofileId = _jsonJArrayHandler.GetJTokenValue(item, "participants", 0,
                        "com.linkedin.voyager.messaging.MessagingMember", "miniProfile", "entityUrn");
                    var UserID = Utils.GetBetween($"{participantsprofileId}/", "urn:li:fs_miniProfile:", "/");

                    var rooturlofpirofilePic = _jsonJArrayHandler.GetJTokenValue(item, "participants", 0,
                        "com.linkedin.voyager.messaging.MessagingMember", "miniProfile", "picture",
                        "com.linkedin.common.VectorImage", "rootUrl");
                    var profilepicUrl = _jsonJArrayHandler.GetJTokenValue(item, "participants", 0,
                        "com.linkedin.voyager.messaging.MessagingMember", "miniProfile", "picture",
                        "com.linkedin.common.VectorImage", "artifacts", 0, "fileIdentifyingUrlPathSegment");
                    var participantsProfilePic = $"{rooturlofpirofilePic}{profilepicUrl}";
                    var lastmessageOwnerid = _jsonJArrayHandler.GetJTokenValue(item, "events", 0, "from",
                        "com.linkedin.voyager.messaging.MessagingMember", "miniProfile", "publicIdentifier");
                    var singleMessageUserdetail = new SenderDetails
                    {
                        SenderId = UserID,
                        SenderName = participantsFullName,
                        LastMesseges = lastmessage,
                        LastMessageOwnerId = lastmessageOwnerid,
                        ThreadId = conversationId,
                        SenderImage = participantsProfilePic,
                        LastMessegedate = Lastmessagetimespan
                    };

                    ChatListNewMessage.Add(singleMessageUserdetail);
                    if (loopCount == getUsers.Count && item["events"][0].ToString()
                            .Contains("com.linkedin.voyager.messaging.MessagingMember"))
                        LastConnectedTimeStamp = (long) item["events"][0]["createdAt"];
                }
            }
            catch (Exception)
            {
            }
        }

        public long LastConnectedTimeStamp { get; set; }
        public bool Hasmore { get; set; }
        public int NextPageCount { get; set; }
    }
}