using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using System;
using TumblrDominatorCore.TmblrUtility;

namespace TumblrDominatorCore.TumblrResponseHandler
{
    public class ReblogSecurityKeyResponse : ResponseHandler
    {
        public ReblogSecurityKeyResponse(IResponseParameter responeParameter) : base(responeParameter)
        {
            try
            {
                if (string.IsNullOrEmpty(responeParameter.Response)) Success = true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }


    public class ReblogPostResponse : ResponseHandler
    {

        public ReblogPostResponse() { }
        public ReblogPostResponse(bool isSuccess)
        {
            Success = isSuccess;
        }
        public string rebloggedPostId { get; set; }
        public ReblogPostResponse(IResponseParameter responeParameter) : base(responeParameter)
        {
            try
            {
                if (!string.IsNullOrEmpty(responeParameter.Response) &&
                    responeParameter.Response.Contains("{\"meta\":{\"status\":201,\"msg\":\"Created\"}"))
                {
                    Success = true;
                    JsonHandler handler;
                    if (!responeParameter.Response.IsValidJson())
                    {
                        var decodedResponse = TumblrUtility.GetDecodedResponseOfJson(responeParameter.Response);
                        handler = new JsonHandler(decodedResponse);
                    }
                    else
                        handler = new JsonHandler(responeParameter.Response);
                    rebloggedPostId = handler.GetElementValue("response", "id");


                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}