using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.Response
{
    public class LiveChatUsermessagesResponseHandler : TdBaseHtmlResponseHandler
    {
        public ObservableCollection<ChatDetails> ListNewMessage = new ObservableCollection<ChatDetails>();
        public string MinPosition = string.Empty;
        public bool HasMore;
        public LiveChatUsermessagesResponseHandler(IResponseParameter response, string userId,string senderId) : base(response)
        {
            if (!Success)
                return;
            var Messages = string.Empty;
            var messagesHtmlDoc = new HtmlDocument();

            try
            {
                var JsonObject = JObject.Parse(response.Response);
                Messages = JsonObject["html"].ToString();
                MinPosition = JsonObject["min_entry_id"].ToString();
                var hasmore = JsonObject["has_more"].ToString();
                HasMore = hasmore.ToLower().Equals("true");
                messagesHtmlDoc.LoadHtml(Messages);
                var listMessages = HtmlAgilityHelper.getListInnerHtmlFromClassName(Messages, "DirectMessage-container", messagesHtmlDoc);
                var listmessageAction = HtmlAgilityHelper.getListInnerHtmlFromClassName(Messages, "DirectMessage-footerItem", messagesHtmlDoc);
                var listOfMessgaeDetail = HtmlAgilityHelper.getListInnerHtmlFromClassName(Messages, "_timestamp", messagesHtmlDoc);
                var count = 0;
                foreach (var EachMessage in listMessages)
                {
                    count= count + 2;
                    var message = WebUtility.HtmlDecode(listmessageAction[count - 2]).Trim();
                    var SenderId = Utilities.GetBetween(EachMessage, "data-user-id=\"", "\"");
                    var singleMessageUserdetail = new ChatDetails
                    {
                        Messeges = Utilities.GetBetween(EachMessage, "data-aria-label-part=\"0\">", "</p>"),
                        MessegesId = Utilities.GetBetween(EachMessage, "data-message-id=\"", "\""),
                        SenderId = senderId,
                        IsRecieved = SenderId == senderId ? false : true,
                        Sender = Utilities.GetBetween(EachMessage, "alt=\"", "\""),
                        Time = Utilities.GetBetween(message, "data-time=\"", "\""),
                      Type = SenderId == userId ? ChatMessage.Sent.ToString() : ChatMessage.Received.ToString()
                    };
                    ListNewMessage.Add(singleMessageUserdetail);
                   
                }
            }


            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }
    }
}

