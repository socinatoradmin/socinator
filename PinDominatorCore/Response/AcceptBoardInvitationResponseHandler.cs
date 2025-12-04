using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;

namespace PinDominatorCore.Response
{
    public class AcceptBoardInvitationResponseHandler : PdResponseHandler
    {
        public AcceptBoardInvitationResponseHandler(IResponseParameter response) : base(response)
        {
            if(string.IsNullOrEmpty(response.Response))
            {
                Success = false;
                return;
            }
            var jsonHand = new DominatorHouseCore.Utility.JsonHandler(response.Response);

            if (jsonHand.GetJToken("resource_response", "status")?.ToString() != "success")
            {
                Success = false;
                Issue = new PinterestIssue
                {
                    Message = jsonHand.GetElementValue("resource_response", "error", "message")
                };
            }
            else
            {
                Success = true;
            }
        }
    }
}