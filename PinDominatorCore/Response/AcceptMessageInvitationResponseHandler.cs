using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinDominatorCore.Response
{
    public class AcceptMessageInvitationResponseHandler: PdResponseHandler
    {
        public AcceptMessageInvitationResponseHandler(IResponseParameter response) : base(response)
        {
            if (string.IsNullOrEmpty(response.Response))
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
