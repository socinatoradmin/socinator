using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using System;

namespace RedditDominatorCore.Response
{
    public class UnSubscribeResponseHandler : RdResponseHandler
    {
        public UnSubscribeResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {

                Success = !string.IsNullOrEmpty(response?.Response) && (response.Response.Contains("{\"updateSubredditSubscriptions\":{\"ok\":true") || response.Response.Contains("{}"));
                if (!Success)
                    response.HasError = !string.IsNullOrEmpty(response.Response) && response.Response.Contains("Forbidden");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}