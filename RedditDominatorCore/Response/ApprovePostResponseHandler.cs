using DominatorHouseCore.Interfaces;
using Newtonsoft.Json.Linq;
using System;

namespace RedditDominatorCore.Response
{
    public class ApprovePostResponseHandler
    {
        public string id = string.Empty;
        public string name = string.Empty;
        public string url = string.Empty;

        public ApprovePostResponseHandler(IResponseParameter response)
        {
            ApprovePostData(response);
        }

        public void ApprovePostData(IResponseParameter response)
        {
            try
            {
                var jsonObject = JObject.Parse(response.Response);
                url = jsonObject["json"]["data"]["url"].ToString();
                id = jsonObject["json"]["data"]["id"].ToString();
                name = jsonObject["json"]["data"]["name"].ToString();
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}