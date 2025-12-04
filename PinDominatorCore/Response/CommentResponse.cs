using System;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;

namespace PinDominatorCore.Response
{
    public class CommentResponse : PdResponseHandler
    {
        public CommentResponse(IResponseParameter response, string pin) : base(response)
        {
            if (string.IsNullOrEmpty(response.Response))
            {
                Success = false;
                return;
            }

            var jsonHand = new DominatorHouseCore.Utility.JsonHandler(response.Response);
            string pinId = null;

            try
            {
                var isError = jsonHand.GetJToken("resource_response", "error").HasValues;
                var Message = jsonHand.GetElementValue("resource_response", "error", "message_detail");
                if (isError)
                {
                    Issue = new PinterestIssue
                    {
                        Message =string.IsNullOrEmpty(Message)? jsonHand.GetElementValue("resource_response", "error", "message"):Message
                    };
                    Success = false;
                    return;
                }

                pinId = jsonHand.GetElementValue("resource", "options", "pinId");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            if (!pin.Equals(pinId)) Success = false;
        }
    }
}