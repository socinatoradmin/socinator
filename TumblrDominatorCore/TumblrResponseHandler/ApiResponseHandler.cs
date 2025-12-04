using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using System;

namespace TumblrDominatorCore.TumblrResponseHandler
{
    public class ApiResponseHandler : ResponseHandler
    {
        public ApiResponseHandler(IResponseParameter responseParameter) : base(responseParameter)
        {
            try
            {
                if (responseParameter == null) return;
                if (string.IsNullOrEmpty(responseParameter.Response)) return;
                if (responseParameter.Response.Contains("{\"meta\":{\"status\":200,\"msg\":\"OK\""))
                {
                    Response = base.Response;
                    Success = true;
                }
                else
                {
                    Success = false;
                }
            }
            catch (Exception e)
            {
                e.DebugLog();
            }
        }

        public new IResponseParameter Response { get; set; }
        public bool Success { get; set; }
    }
}