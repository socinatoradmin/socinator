using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using System;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;

namespace TumblrDominatorCore.TumblrResponseHandler
{
    public class MediaUploadResponseHandler : ResponseHandler
    {

        public UploadedMediaModel MediaUploaded = new UploadedMediaModel();
        public MediaUploadResponseHandler(IResponseParameter responeParameter) : base(responeParameter)
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
                    MediaUploaded.url = handler.GetElementValue("response", "url");
                    MediaUploaded.type = handler.GetElementValue("response", "type");
                    MediaUploaded.height = Convert.ToInt32(handler.GetElementValue("response", "height"));
                    MediaUploaded.width = Convert.ToInt32(handler.GetElementValue("response", "width"));

                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


    }
}
