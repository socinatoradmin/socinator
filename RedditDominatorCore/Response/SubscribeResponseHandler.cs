using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using System;

namespace RedditDominatorCore.Response
{
    public class SubscribeResponseHandler : RdResponseHandler
    {
        public SubscribeResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {
                if (Success && response.Response.Contains("updateSubredditSubscriptions"))
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