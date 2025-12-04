using DominatorHouseCore.Interfaces;
using System;

namespace TumblrDominatorCore.TumblrResponseHandler
{
    public class FollowResponseHandler : ResponseHandler
    {
        public FollowResponseHandler(IResponseParameter responseParameter) : base(responseParameter)
        {
            try
            {
                if (responseParameter == null) Success = false;
                if (string.IsNullOrEmpty(responseParameter.Response)) Success = false;
                if (responseParameter.Response.Contains("\"followed\":true") &&
                    responseParameter.Response.Contains("\"meta\":{\"status\":200,\"msg\":\"OK\"")) Success = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}