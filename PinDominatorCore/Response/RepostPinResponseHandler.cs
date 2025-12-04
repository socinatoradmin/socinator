using System;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using Newtonsoft.Json.Linq;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;

namespace PinDominatorCore.Response
{
    public class RepostPinResponseHandler : PdResponseHandler
    {
        public string PinId { get; set; }

        public RepostPinResponseHandler(IResponseParameter response) : base(response)
        {
            Success = false;
            Issue = new PinterestIssue { Message = response.Response };
        }

        public RepostPinResponseHandler(IResponseParameter response, string boardUrl) : base(response)
        {
            try
            {
                if (string.IsNullOrEmpty(response.Response))
                {
                    Success = false;
                    return;
                }

                JToken data = null;
                JToken error = string.Empty;

                JsonHandler jsonHand = new JsonHandler(response.Response);
                data = jsonHand.GetJToken("resource_response", "data");
                error = jsonHand.GetJToken("resource_response", "error");

                if (data.HasValues)
                {
                    PinId = jsonHand.GetJTokenValue(data, "id");

                    if (!Success)
                        Success = !error.HasValues;
                }
                else
                    Success = false;

                if (error != null && error.HasValues)
                {
                    Success = false;
                    var message = jsonHand.GetJTokenValue(error, "message_detail");
                    Issue = new PinterestIssue
                    {
                        Message = string.IsNullOrEmpty(message)? jsonHand.GetJTokenValue(error, "message"): message,
                        Status = jsonHand.GetJTokenValue(error, "http_status")
                    };
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}