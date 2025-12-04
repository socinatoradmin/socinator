using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using System;

namespace TumblrDominatorCore.TumblrResponseHandler
{
    public class CommentPostResponse : ResponseHandler
    {
        public IResponseParameter commentResponse = null;
        public CommentPostResponse(IResponseParameter responeParameter) : base(responeParameter)
        {
            try
            {
                if (responeParameter == null)
                    return;
                if (string.IsNullOrEmpty(responeParameter.Response)) return;
                commentResponse = responeParameter;
                if (responeParameter.Response.Contains("{\"meta\":{\"status\":200,\"msg\":\"OK\"}") && responeParameter.Response.Contains("\"content\":[{\"type\":"))
                    Success = true;

            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
                // ignored
            }
        }
    }
}