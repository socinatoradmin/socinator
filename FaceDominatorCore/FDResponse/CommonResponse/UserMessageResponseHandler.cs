using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FaceDominatorCore.FDResponse.CommonResponse
{


    public class UserMessageResponseHandler : FdResponseHandler, IResponseHandler
    {

        public bool Status { get; set; }

        public bool HasMoreResults { get; set; } = true;

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();

        public string UserId { get; set; }

        public string AccountId { get; set; }

        public string FriendName { get; set; }

        public UserMessageResponseHandler
            (IResponseParameter responseParameter, string userId, string accountId, string friendName) : base(responseParameter)

        {
            if (responseParameter.HasError || responseParameter.Response == null)
                return;

            UserId = userId;

            AccountId = accountId;

            FriendName = friendName;

            ObjFdScraperResponseParameters = new FdScraperResponseParameters();

            ObjFdScraperResponseParameters.ListChatDetails = new List<ChatDetails>();

            var decodedResponse = responseParameter.Response;

            try
            {

                decodedResponse = Utilities.GetBetween(decodedResponse, "message_thread\":", "}}}}}") + "}}";

                if (decodedResponse.StartsWith("}}"))
                    return;

                GetSendersListFromMessanger(decodedResponse);

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }



        private void GetSendersListFromMessanger(string decodedResponse)
        {
            try
            {
                JObject jObject = JObject.Parse(decodedResponse);

                var friendList = jObject["messages"]["nodes"];

                var otherUserId = jObject["thread_key"]["other_user_id"].ToString();

                ObjFdScraperResponseParameters.ListChatDetails = (from token in friendList
                                                                  where token.Children().Count() == 24
                                                                  let messageStatus =
                                                                      UserId.Contains(token["message_sender"]["id"].ToString())
                                                                     ? "Sent" : "Received"
                                                                  let messageText = token["message"]["text"]
                                                                  let timestamp = token["timestamp_precise"]
                                                                  let messageId = token["message_id"]
                                                                  let blobAttachments = token["blob_attachments"]
                                                                  let imageList = (from imageToken in blobAttachments
                                                                                   select imageToken["large_preview"]["uri"].ToString()).ToList()
                                                                  select new ChatDetails
                                                                  {
                                                                      Type = messageStatus.Contains("Sent") ? ChatMessage.Sent.ToString() : ChatMessage.Received.ToString(),
                                                                      SenderId = otherUserId,
                                                                      Sender = FriendName,
                                                                      Messeges = messageText.ToString(),
                                                                      Time = timestamp.ToString(),
                                                                      MessegesId = messageId.ToString(),
                                                                      ListMediaUrls = imageList.Count > 0 ? new ObservableCollection<string>(imageList) : new ObservableCollection<string>(),
                                                                      MessegeType = imageList.Count > 0 ? ChatMessageType.Media : ChatMessageType.Text
                                                                  }).ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
