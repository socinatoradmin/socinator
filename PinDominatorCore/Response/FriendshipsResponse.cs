using System;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using PinDominatorCore.PDModel;

namespace PinDominatorCore.Response
{
    public class FriendshipsResponse : PdResponseHandler
    {
        public FriendshipsResponse(IResponseParameter response, string userid) : base(response)
        {
            if (string.IsNullOrEmpty(response.Response))
            {
                Success = false;
                return;
            }

            var jsonData = response.Response;
            try
            {
                if (string.IsNullOrEmpty(userid))
                {
                    Success = false;
                    return;
                }

                var jsonHand = new DominatorHouseCore.Utility.JsonHandler(jsonData);
                if (!string.IsNullOrEmpty(jsonHand.GetElementValue("resource_response", "status")))
                    Success = jsonHand.GetElementValue("resource_response", "status") == "success";
                else if (string.IsNullOrWhiteSpace(jsonHand.GetElementValue("resource_response", "data")))
                {
                    Success = false;
                    Issue = new PinterestIssue
                    {
                        Message = jsonHand.GetElementValue("resource_response", "error", "message_detail")
                    };
                    if(string.IsNullOrEmpty(Issue.Message))
                        Issue = new PinterestIssue
                        {
                            Message = jsonHand.GetElementValue("resource_response", "error", "message"),
                            Status = jsonHand.GetElementValue("resource_response", "error", "status")
                        };
                }
                else
                {
                    if(!string.IsNullOrEmpty(jsonData))
                    Issue = new PinterestIssue
                    {
                        Message = DominatorHouseCore.Utility.Utilities.GetBetween(jsonData, "message_detail\":\"", "\"")
                    };
                    Success = false;
                }
                    
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                Success = false;
            }
        }
    }
}