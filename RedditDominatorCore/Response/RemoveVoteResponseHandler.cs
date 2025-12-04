using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using System;

namespace RedditDominatorCore.Response
{
    public class RemoveVoteResponseHandler : RdResponseHandler
    {
        public RemoveVoteResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {
                if (Success && response.Response.Contains("ok\":true"))
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