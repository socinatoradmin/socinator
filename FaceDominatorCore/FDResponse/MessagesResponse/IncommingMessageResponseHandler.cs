using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.MessagesResponse
{
    public class IncommingMessageResponseHandler : FdResponseHandler, IResponseHandler
    {

        public bool HasMoreResults { get; set; }

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();

        public IncommingMessageResponseHandler(IResponseParameter responseParameter,
            string userId, List<FdMessageDetails> incommingMessageList)
            : base(responseParameter)
        {

            if (responseParameter.HasError || responseParameter.Response == null)
                return;

            ObjFdScraperResponseParameters.MessageDetailsList = incommingMessageList ?? new List<FdMessageDetails>();

            var decodedResponse = FdFunctions.GetPrtialDecodedResponse(responseParameter.Response);

            GetRecentMessages(decodedResponse, userId);

            if (ObjFdScraperResponseParameters.MessageDetailsList.Count > 0)
            {
                if (incommingMessageList != null && ObjFdScraperResponseParameters.MessageDetailsList.FirstOrDefault(x => incommingMessageList.Any(y =>
                          x.MessageReceiverId == y.MessageReceiverId &&
                          x.InteractionDate == y.InteractionDate)) == null)
                {
                    Status = true;
                    HasMoreResults = true;
                }
                else if (incommingMessageList == null)
                {
                    Status = true;
                    HasMoreResults = true;
                }

            }

        }



        public void GetRecentMessages(string decodedResponse, string userId)
        {
            var splitMessageResponse = Regex.Split(decodedResponse, "thread_key\":{\"thread_fbid").Skip(1).ToArray();

            FdFunctions objFdFunctions = new FdFunctions();


            foreach (string message in splitMessageResponse)
            {
                FdMessageDetails objMessageDetail = new FdMessageDetails();

                long interactionTimeStamp;

                string cannotReplyReason = objFdFunctions.GetBetween(message, "cannot_reply_reason\":", ",");

                if (string.IsNullOrEmpty(cannotReplyReason))
                    continue;

                var timestamp = objFdFunctions.GetBetween(message, "timestamp_precise\":\"", "\"");

                long.TryParse(timestamp, out interactionTimeStamp);

                try
                {
                    objMessageDetail.InteractionDate = interactionTimeStamp.EpochToDateTimeUtc();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                ObjFdScraperResponseParameters.PaginationTimestamp = interactionTimeStamp.ToString();

                //objMessageDetail.UnreadCount = !string.IsNullOrEmpty(unreadCount) ? Int32.Parse(unreadCount) : 0;

                //if (objMessageDetail.UnreadCount == 0)
                //    continue;

                try
                {

                    var imageDetails = objFdFunctions.GetBetween(message, "blob_attachments\":", "\"MessageImage");

                    var stickerDetails = objFdFunctions.GetBetween(message, "sticker\":{\"id\"", "}");

                    if (!string.IsNullOrEmpty(imageDetails) || !string.IsNullOrEmpty(stickerDetails))
                        continue;


                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                objMessageDetail.OtherUserFbid = objFdFunctions.GetBetween(message, "other_user_id\":\"", "\"");

                objMessageDetail.Message = objFdFunctions.GetBetween(message, "snippet\":\"", "\"");

                objMessageDetail.MessageSenderId = objFdFunctions.GetBetween(message, "messaging_actor\":{\"id\":\"", "\"");//messaging_actor":{"id":"100007464069871                

                if (userId == objMessageDetail.MessageSenderId)
                    continue;

                objMessageDetail.MessageReceiverId = userId;

                var messageFolder = objFdFunctions.GetBetween(message, "folder\":\"", "\"");

                objMessageDetail.MesageType = messageFolder.Contains("INBOX") ? MessageType.Inbox : MessageType.Pending;

                var participantDetails = Regex.Split(message, "all_participants\":{\"nodes\"")[1];

                var splitParticipant = Regex.Split(participantDetails, "messaging_actor").Skip(1).ToArray();

                foreach (string participant in splitParticipant)
                {
                    if (participant.Contains(objMessageDetail.MessageSenderId))
                    {
                        objMessageDetail.MessageSenderName = objFdFunctions.GetBetween(participant, "\"name\":\"", "\"");

                        objMessageDetail.OtherUserGender = objFdFunctions.GetBetween(participant, "gender\":\"", "\"");
                    }

                    else
                    {
                        objMessageDetail.MessageReceiverName = objFdFunctions.GetBetween(participant, "\"name\":\"", "\"");
                    }
                }

                if (ObjFdScraperResponseParameters.MessageDetailsList.FirstOrDefault(x => x.MessageReceiverId == objMessageDetail.MessageReceiverId &&
                                                             x.InteractionDate == objMessageDetail.InteractionDate) == null)
                    ObjFdScraperResponseParameters.MessageDetailsList.Add(objMessageDetail);

            }
        }
    }
}
