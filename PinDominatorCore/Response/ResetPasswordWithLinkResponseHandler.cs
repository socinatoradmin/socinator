using DominatorHouseCore.Interfaces;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;

namespace PinDominatorCore.Response
{
    public class ResetPasswordWithLinkResponseHandler : PdResponseHandler
    {
        public ResetPasswordWithLinkResponseHandler(IResponseParameter response) : base(response)
        {
            var jsonHand = new DominatorHouseCore.Utility.JsonHandler(response.Response);

            var dataToken = jsonHand.GetJToken("resource_response", "data");
            var errorToken = jsonHand.GetJToken("resource_response", "error");

            if (errorToken.HasValues)
            {
                Issue = new PinterestIssue
                {
                    Message = jsonHand.GetJTokenValue(errorToken, "message")
                };
                Success = false;
            }
            else
            {
                Success = jsonHand.GetJTokenValue(dataToken, "password_changed").ToLower() == "true";
            }
        }
    }
}