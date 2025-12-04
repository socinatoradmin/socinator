using DominatorHouseCore.Interfaces;
using Newtonsoft.Json.Linq;
using System;

namespace RedditDominatorCore.Response
{
    public class FetchTitleOfUrlResponseHandler : RdResponseHandler
    {
        public string title = string.Empty;

        public FetchTitleOfUrlResponseHandler(IResponseParameter response) : base(response)
        {
            GetTitle(response);
        }


        public string GetTitle(IResponseParameter response)
        {
            try
            {
                var jsonData = response.Response;
                var jObject = JObject.Parse(jsonData);
                title = jObject["json"]["data"]["title"].ToString();
                return title;
            }
            catch (Exception)
            {
                return title = "Title Not Fetched";
            }
        }
    }
}