using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using System;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;

namespace TumblrDominatorCore.TumblrResponseHandler
{
    public class FeedInteractionResponseHandler : ResponseHandler
    {
        public FeedInteractionResponseHandler(IResponseParameter responeParameter) : base(responeParameter)
        {
            try
            {
                if (RespJ["meta"]["msg"].ToString() == "OK")
                {
                    if (!Response.Response.IsValidJson())
                    {
                        var decodedResponse = TumblrUtility.GetDecodedResponseOfJson(Response.Response);
                        FeedInteraction = JsonConvert.DeserializeObject<FeedInteraction>(decodedResponse);
                    }
                    else
                        FeedInteraction = JsonConvert.DeserializeObject<FeedInteraction>(responeParameter.Response);
                    Success = true;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        public FeedInteraction FeedInteraction { get; set; }
    }
}