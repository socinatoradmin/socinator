using DominatorHouseCore.Interfaces;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using System;

namespace PinDominatorCore.Response
{
    public class DeletePinResponseHandler : PdResponseHandler
    {
        public DeletePinResponseHandler(IResponseParameter response) : base(response)
        {
            if (string.IsNullOrEmpty(response.Response))
            {
                Success = false;
                return;
            }
            try
            {
                var jsonHand = new DominatorHouseCore.Utility.JsonHandler(response.Response);
                var errorToken = jsonHand.GetJToken("resource_response", "error");

                if (errorToken.HasValues)
                {
                    Issue = new PinterestIssue
                    {
                        Message = jsonHand.GetJTokenValue(errorToken, "message")
                    };
                    Success = false;
                }
            }
            catch(Exception)
            {
                Success = false;
            }
        }
    }
}