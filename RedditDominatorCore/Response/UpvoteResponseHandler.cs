using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using System;

namespace RedditDominatorCore.Response
{
    public class UpvoteResponseHandler : RdResponseHandler
    {
        public string ErrorMessage = string.Empty;

        public UpvoteResponseHandler(IResponseParameter response, bool isAlreadyUpvoted = false, bool isBrowser=false) : base(response)
        {
            try
            {
                if (isAlreadyUpvoted) { Success = false;return; }
                if (isBrowser) { }
                else
                {
                    if (Success && response.Response.Contains("ok\":true"))
                    {
                        Success = true;
                    }
                    else
                    {
                        response.HasError = response.Response.Contains("Forbidden");
                        ErrorMessage = new JsonHandler(response.Response).GetElementValue("message");
                        Success = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    public class SubmitPageResponseHandler : RdResponseHandler
    {
        public SubmitPageResponseHandler(IResponseParameter response) : base(response)
        {
            if (Success)
            {
            }
        }
    }
}