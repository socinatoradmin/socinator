using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using System;

namespace RedditDominatorCore.Response
{
    public class ApproveResponseHandler : RdResponseHandler
    {
        public ApproveResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {
                if (Success && response.Response == "{}")
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