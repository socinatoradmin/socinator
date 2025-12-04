using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.MessagesResponse
{
    public class GetLastMessageResponseHandler : FdResponseHandler, IResponseHandler
    {
        public bool Status { get; set; }

        public bool HasMoreResults { get; set; } = true;

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();


        public GetLastMessageResponseHandler(IResponseParameter responseParameter, string userId)
            : base(responseParameter)
        {
            if (responseParameter.HasError || responseParameter.Response == null)
                return;

            var messageDetail = GetLastMessageDetails(responseParameter.Response);

            try
            {
                if (messageDetail == null || !messageDetail.Any())
                {
                    Status = false;
                }
                // ReSharper disable once PossibleNullReferenceException
                else if (!messageDetail.LastOrDefault().Message.Contains("Say hi to your new Facebook friend, ") &&
                         // ReSharper disable once PossibleNullReferenceException
                         !messageDetail.LastOrDefault().Message.Contains("You are now connected on Messenger.")
                    && messageDetail.LastOrDefault()?.MessageSenderId == userId && messageDetail.Count != 0)
                {
                    Status = true;
                }
                else
                {
                    var sentMessageList = messageDetail.Where(x => x.MessageSenderId == userId).ToList();

                    Status = sentMessageList.Any(x => !x.Message.Contains("You are now connected on Messenger.")
                                             && !x.Message.Contains("Say hi to your new Facebook friend, "))
                                             || !sentMessageList.Any();
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        private List<FdMessageDetails> GetLastMessageDetails(string response)
        {
            try
            {
                response = Regex.Split(response, "}}}}}")[0] + "}}}}}";

                JObject jObject = JObject.Parse(response);

                List<FdMessageDetails> objListFdMessageDetails = new List<FdMessageDetails>();

                var messageDetails = jObject["o0"]["data"]["message_thread"]["messages"]["nodes"];

                foreach (var token in messageDetails)
                {
                    try
                    {
                        FdMessageDetails objFdMessageDetails = new FdMessageDetails
                        {
                            Message = token["message"]["text"]?.ToString(),
                            MessageSenderId = token["message_sender"]["id"]?.ToString()
                        };



                        objListFdMessageDetails.Add(objFdMessageDetails);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                }


                return objListFdMessageDetails;
            }
            catch (Exception ex)
            {
                if (!response.Contains("\"message_thread\":null"))
                    ex.DebugLog();
                return null;
            }
        }
    }

}
