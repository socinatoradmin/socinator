using DominatorHouseCore.Interfaces;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;

namespace PinDominatorCore.Response
{
    public class MessageResponseHandler : PdResponseHandler
    {
        public MessageResponseHandler(IResponseParameter response) : base(response)
        {
            if (string.IsNullOrEmpty(response.Response))
            {
                Success = false;
                return;
            }

            var jsonHand = new DominatorHouseCore.Utility.JsonHandler(response.Response);

            if (jsonHand.GetJToken("resource_response", "error").HasValues)
            {
                Success = false;
                Issue = new PinterestIssue();
                Issue.Message = jsonHand.GetElementValue("resource_response", "error", "message");
                Issue.Status = jsonHand.GetElementValue("resource_response", "error", "status");
            }
        }
    }
}