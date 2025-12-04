using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using Newtonsoft.Json.Linq;
using RedditDominatorCore.RDLibrary;
using System;

namespace RedditDominatorCore.Response
{
    public class PaginationResponseHandler : RdResponseHandler
    {
        public PaginationResponseHandler(IResponseParameter responseParameter) : base(responseParameter)
        {
            try
            {
                var jsonResponse = RdConstants.GetJsonPageResponse(responseParameter.Response);
                var jsonObject = JObject.Parse(jsonResponse);
                SessionTracker = jsonObject["user"]["sessionTracker"].ToString();
                var loidLoid = jsonObject["user"]["loid"]["loid"].ToString();
                var loidBlob = jsonObject["user"]["loid"]["blob"].ToString();
                var loidCreated = jsonObject["user"]["loid"]["loidCreated"].ToString();
                var loidVersion = jsonObject["user"]["loid"]["version"].ToString();
                Loid = loidLoid + "." + loidVersion + "." + loidCreated + "." + loidBlob;
                AccessToken = jsonObject["user"]["session"]["accessToken"].ToString();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public string Loid { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string SessionTracker { get; set; } = string.Empty;
    }
}