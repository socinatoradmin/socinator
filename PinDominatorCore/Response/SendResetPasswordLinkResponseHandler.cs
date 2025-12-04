using System;
using DominatorHouseCore.Interfaces;
using PinDominatorCore.PDUtility;
using PinDominatorCore.PDModel;

namespace PinDominatorCore.Response
{
    public class SendResetPasswordLinkResponseHandler : PdResponseHandler
    {
        public SendResetPasswordLinkResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {
                var jsonHand = new DominatorHouseCore.Utility.JsonHandler(response.Response);
                
                var errorToken = jsonHand.GetJToken("resource_response", "error");

                if (errorToken == null && jsonHand.GetElementValue("resource_response", "status") == "success")
                    Success = true;
                if (errorToken != null && errorToken.HasValues)
                {
                    Success = false;
                    Issue = new PinterestIssue
                    {
                        Message = jsonHand.GetJTokenValue(errorToken, "message")
                    };
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}