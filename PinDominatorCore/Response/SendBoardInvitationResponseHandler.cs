using DominatorHouseCore.Interfaces;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;

namespace PinDominatorCore.Response
{
    public class SendBoardInvitationResponseHandler : PdResponseHandler
    {
        public SendBoardInvitationResponseHandler(IResponseParameter response) : base(response)
        {
            if (string.IsNullOrEmpty(response.Response))
            {
                Success = false;
                Issue = new PinterestIssue
                {
                    Message = "Invite option is not present."
                };
                return;
            }
            if (response.Response.Contains("Invited"))
            {
                Success = false;
                Issue = new PinterestIssue
                {
                    Message = "User Already Invited"
                };
                return;
            }

            var jsonHand = new DominatorHouseCore.Utility.JsonHandler(response.Response);
            if(jsonHand.GetElementValue("data", "v3InviteBoardCollaboratorsMutation", "__typename") == "V3InviteBoardCollaborators" || jsonHand.GetElementValue("resource_response", "status") =="success")
                Success = true;
            else
            {
                Success = false;
                Issue = new PinterestIssue
                {
                    Message = jsonHand.GetElementValue("resource_response", "error", "message_detail")
                };
                Issue.Message=string.IsNullOrEmpty(Issue.Message)? jsonHand.GetElementValue("resource_response", "error", "message") : Issue.Message;
                Issue.Message=string.IsNullOrEmpty(Issue.Message)? jsonHand.GetElementValue("data", "v3InviteBoardCollaboratorsMutation", "error", "message") : Issue.Message;
            }
        }
    }
}