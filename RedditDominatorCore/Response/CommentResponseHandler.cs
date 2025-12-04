using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using System;

namespace RedditDominatorCore.Response
{
    public class CommentResponseHandler : RdResponseHandler
    {
        public CommentResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {
                if (Success && !HasError)
                {
                    Success = true;
                }
                else if (response.Response.Contains("RATELIMIT"))
                {
                    Success = false;
                    ErrorMessage = Utilities.GetBetween(response.Response, "\"RATELIMIT\", \"", "\"");
                }
                else if (response.Response.Contains("TOO_OLD"))
                {
                    Success = false;
                    ErrorMessage = Utilities.GetBetween(response.Response, "TOO_OLD\", \"", "\"");
                }
                //For EditComment module
                else if (response.Response.Contains("NOT_AUTHOR"))
                {
                    Success = false;
                    ErrorMessage = "Not Author, you can't do that";
                }
                else
                {
                    response.HasError = response.Response.Contains("Forbidden");
                    Success = false;
                    ErrorMessage = "Reason: Blocked";
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public string ErrorMessage { get; set; }
    }
}