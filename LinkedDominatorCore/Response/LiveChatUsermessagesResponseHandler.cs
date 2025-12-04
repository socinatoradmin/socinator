using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using LinkedDominatorCore.LDLibrary;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Utility;
using Newtonsoft.Json.Linq;

namespace LinkedDominatorCore.Response
{
    public class LiveChatUsermessagesResponseHandler : LdResponseHandler
    {
        public bool HasMore;
        public ObservableCollection<ChatDetails> ListNewMessage = new ObservableCollection<ChatDetails>();

        public List<KeyValuePair<string, string>> ListofimagesandAttachmenturl =
            new List<KeyValuePair<string, string>>();

        public string MinPosition = string.Empty;

        public LiveChatUsermessagesResponseHandler(IResponseParameter response, string senderId,
            ILdFunctions ldFunction) : base(response)
        {
            var attachmentsFileName = "";
            var downloadPath = Utils.GetLivechatAttachmentFilePath();
            var _jsonJArrayHandler = JsonJArrayHandler.GetInstance;

            var getmessages =_jsonJArrayHandler.GetJArrayElement(_jsonJArrayHandler.GetJTokenValue(RespJ, "elements"));
            foreach (var item in getmessages)
            {
                ListofimagesandAttachmenturl.Clear();
                var message = _jsonJArrayHandler.GetJTokenValue(item, "eventContent",
                    "com.linkedin.voyager.messaging.event.MessageEvent", "attributedBody", "text");
                var Attachment = _jsonJArrayHandler.GetTokenElement(item, "eventContent",
                    "com.linkedin.voyager.messaging.event.MessageEvent", "attachments");
                if (Attachment != null)
                {
                    foreach (var attach in Attachment)
                    {
                        attachmentsFileName = _jsonJArrayHandler.GetJTokenValue(attach, "name");
                        var attachmentFileDownloadLink =
                            _jsonJArrayHandler.GetJTokenValue(attach, "reference", "string");
                        ListofimagesandAttachmenturl.Add(
                            new KeyValuePair<string, string>($@"{downloadPath}\{attachmentsFileName}",
                                attachmentFileDownloadLink));
                    }

                    foreach (var eachAttachment in ListofimagesandAttachmenturl)
                        ldFunction.GetInnerLdHttpHelper().DownloadFile(eachAttachment.Value, eachAttachment.Key);
                }

                var USerProfileId = _jsonJArrayHandler.GetJTokenValue(item, "from",
                    "com.linkedin.voyager.messaging.MessagingMember", "miniProfile", "entityUrn");
                var UserID = Utils.GetBetween($"{USerProfileId}/", "urn:li:fs_miniProfile:", "/");
                var messageTime = _jsonJArrayHandler.GetJTokenValue(item, "createdAt");
                var singleMessageUserdetail = new ChatDetails
                {
                    Messeges = message,
                    SenderId = senderId,
                    MessegeType = ListofimagesandAttachmenturl.Count > 0 && !string.IsNullOrEmpty(message)
                        ? ChatMessageType.TextAndMedia
                        : string.IsNullOrEmpty(message)
                            ? ChatMessageType.Media
                            : ChatMessageType.Text,
                    ListMediaUrls =
                        new ObservableCollection<string>(ListofimagesandAttachmenturl.Select(x => x.Key).ToList()),
                    IsRecieved = senderId == UserID ? true : false,
                    Time = messageTime,
                    MessegesId = $"{messageTime}_{senderId}",
                    Type = senderId != UserID ? ChatMessage.Sent.ToString() : ChatMessage.Received.ToString()
                };
                ListNewMessage.Add(singleMessageUserdetail);
            }
        }
    }
}