using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using System;
using TumblrDominatorCore.TmblrUtility;

namespace TumblrDominatorCore.TumblrResponseHandler
{
    public class PublishResponseHandler : ResponseHandler
    {
        public PublishResponseHandler() { }
        public PublishResponseHandler(IResponseParameter responeParameter, string userId = "") : base(responeParameter)
        {
            try
            {
                if (!string.IsNullOrEmpty(responeParameter.Response) && (responeParameter.Response.Contains("{\"meta\":{\"status\":201,\"msg\":\"Created\"}") || responeParameter.Response.Contains("\"state\":\"published\"")))
                {

                    Success = true;
                    JsonHandler handler = null;
                    if (!responeParameter.Response.IsValidJson())
                    {
                        var validJsonResponse = TumblrUtility.GetDecodedResponseOfJson(responeParameter.Response);
                        handler = new JsonHandler(validJsonResponse);
                    }
                    else
                        handler = new JsonHandler(responeParameter.Response);
                    PublishedUrl = handler.GetElementValue("response", "post_url");
                    if (string.IsNullOrEmpty(PublishedUrl))
                    {
                        var PublishedUrlId = handler.GetElementValue("response", "id");
                        PublishedUrl = $"https://www.tumblr.com/{userId}/{PublishedUrlId}";
                    }
                    Message = handler.GetElementValue("response", "displayText");

                }
                else
                {
                    Success = false;
                    Message = responeParameter.Response;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public string PublishedUrl { get; set; }

        public string Message { get; set; }
    }
}