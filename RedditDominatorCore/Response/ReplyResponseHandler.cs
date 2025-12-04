using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using System;

namespace RedditDominatorCore.Response
{
    public class ReplyResponseHandler : RdResponseHandler
    {
        public ReplyResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {
                if (Success && response.Response.Contains("\"send_replies\": true"))
                {
                    Success = true;
                }
                else
                {
                    response.HasError = response.Response.Contains("Forbidden");
                    Success = false;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}