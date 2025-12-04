using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FaceDominatorCore.FDResponse.MessagesResponse
{

    public class FdSendTextMessageResponseHandler : FdResponseHandler, IResponseHandler
    {
        public bool HasMoreResults { get; set; }

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
        = new FdScraperResponseParameters();

        public bool IsMessageSent { get; set; }

        public FdSendTextMessageResponseHandler(IResponseParameter responseParameter, SenderDetails senderDetails = null, string textMessage = "")
            : base(responseParameter)
        {
            if (responseParameter.HasError || responseParameter.Response == null)
                return;

            ObjFdScraperResponseParameters = new FdScraperResponseParameters();

            ObjFdScraperResponseParameters.ListSenderDetails = new List<DominatorHouseCore.Models.ChatDetails>();

            if (!responseParameter.Response.Contains("errorSummary") && !responseParameter.Response.Contains("Not Found"))
            {
                if (responseParameter.Response.Contains("message_id"))
                {
                    IsMessageSent = true;

                    Status = true;

                    if (senderDetails != null)
                        GetSentMessageDetails(FdFunctions.GetDecodedResponse(responseParameter.Response), senderDetails, textMessage);
                }
            }
            else if (responseParameter.Response.Contains("errorSummary") && responseParameter.Response.Contains("This person isn't available right now"))
            {
                FbErrorDetails = new FDLibrary.FdClassLibrary.FdErrorDetails
                {
                    Description = "This person isn't available right now"
                };
            }
        }

        private void GetSentMessageDetails(string decodedResponse, SenderDetails senderDetails, string textMessage)
        {
            try
            {
                decodedResponse = decodedResponse.Replace("for (;;);", string.Empty);


                JObject jObject = JObject.Parse(decodedResponse);

                var friendList = jObject["payload"]["actions"][0];

                var otherUserId = friendList["other_user_fbid"].ToString();

                var imageListFull = (from imageToken in friendList["graphql_payload"]
                                     select imageToken["node"]["large_preview"]["uri"].ToString()).ToList();

                var time = friendList["timestamp"].ToString();

                var messageText = textMessage;

                ObjFdScraperResponseParameters.ListSenderDetails.Add(new ChatDetails()
                {
                    MessegesId = friendList["message_id"].ToString(),
                    ListMediaUrls = imageListFull.Count > 0 ? new ObservableCollection<string>(imageListFull) : new ObservableCollection<string>(),
                    Type = ChatMessage.Sent.ToString(),
                    SenderId = otherUserId,
                    Sender = senderDetails.SenderName,
                    Messeges = messageText.ToString(),
                    Time = time,
                    MessegeType = imageListFull.Count > 0 ? ChatMessageType.Media : ChatMessageType.Text

                });


            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
