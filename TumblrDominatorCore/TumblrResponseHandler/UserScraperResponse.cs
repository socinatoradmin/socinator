using DominatorHouseCore.Interfaces;
using System;

namespace TumblrDominatorCore.TumblrResponseHandler
{
    public class UserScraperResponse : ResponseHandler
    {
        public UserScraperResponse(IResponseParameter responeParameter) : base(responeParameter)
        {
            try
            {
                if (!string.IsNullOrEmpty(responeParameter.Response) &&
                    responeParameter.Response.Contains("{\"meta\":{\"status\":200,\"msg\":\"OK\"}")) Success = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}