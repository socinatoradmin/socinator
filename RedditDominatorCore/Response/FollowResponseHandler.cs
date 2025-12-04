using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using System;

namespace RedditDominatorCore.Response
{
    public class FollowResponseHandler : RdResponseHandler
    {
        public FollowResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {
                if (Success && response.Response.Contains("updateProfileFollowState"))
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