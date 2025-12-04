using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using System;

namespace TumblrDominatorCore.TumblrResponseHandler
{
    public class UnFollowResponseHandler : ResponseHandler
    {
        public UnFollowResponseHandler(IResponseParameter responseParameter) : base(responseParameter)
        {
            try
            {
                if (responseParameter == null)
                    return;
                if (responseParameter.Response.Contains("\"followed\":false") && responseParameter.Response.Contains("\"meta\":{\"status\":200,\"msg\":\"OK\"") ||
                    responseParameter.Response == "")
                {
                    IsUnfollowed = true;
                    Success = true;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        public bool IsUnfollowed { get; set; }
    }
}