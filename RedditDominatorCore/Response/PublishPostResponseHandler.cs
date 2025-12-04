using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using Newtonsoft.Json.Linq;
using System;

namespace RedditDominatorCore.Response
{
    public class PublishPostResponseHandler : RdResponseHandler
    {
        public PublishPostResponseHandler(IResponseParameter response) : base(response)
        {
            if (Success)
            {
                try
                {
                    var jsonHand = new JsonHandler(response.Response);
                    var jErrorResp = jsonHand.GetJToken("json", "errors");
                    if (jErrorResp.HasValues)
                    {
                        FailureMessage = jErrorResp.First.ToString().Split(',')[1].Trim();
                        Success = false;
                    }
                    else if (response.Response.Contains("ratelimit"))
                    {
                        FailureMessage = Utilities.GetBetween(response.Response, "\"RATELIMIT\", \"", "\"");
                        Success = false;
                    }
                    else if (response.Response.Contains("user_submitted_page"))
                    {
                        Success = true;
                        MediaId = Utilities.GetBetween(response.Response, "rte_images/", "?");
                        PostUrl = Utilities.GetBetween(response.Response, "\"user_submitted_page\": \"", "/submitted");
                    }
                    else if (response.Response.Contains("wss://ws-"))
                    {
                        Success = true;
                    }
                    else
                    {
                        var jsonobject1 = JObject.Parse(response.Response);
                        PostUrl = jsonobject1["json"]["data"]["url"]?.ToString();
                        Success = true;
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
            else
            {
                response.HasError = response.Response.Contains("Forbidden");
                FailureMessage = "Blocked";
                Success = false;
            }
        }

        public string PostUrl { get; set; }
        public string MediaId { get; set; }
        public string FailureMessage { get; set; }
    }
}