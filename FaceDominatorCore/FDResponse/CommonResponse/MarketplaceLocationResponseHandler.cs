using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDResponse.BaseResponse;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace FaceDominatorCore.FDResponse.CommonResponse
{
    public class MarketplaceLocationResponseHandler : FdResponseHandler
    {

        public Dictionary<string, string> LocationDictionary { get; set; } = new Dictionary<string, string>();

        public List<Tuple<string, string, string, string>> CityList = new List<Tuple<string, string, string, string>>();


        public MarketplaceLocationResponseHandler(IResponseParameter responseParameter, string keyword)
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;

            try
            {
                JObject jObject = JObject.Parse(responseParameter.Response.Replace("for (;;);", string.Empty));

                var tokenSearch = keyword.Trim();

                JToken jtoken1 = jObject["payload"][tokenSearch];

                var newToken = tokenSearch + " ";
                tokenSearch = jtoken1 == null ? newToken : keyword;

                var noodes = jObject["payload"][tokenSearch]["street_results"]["edges"];
                foreach (JToken node in noodes)
                {
                    string key = node["node"]["page"]["id"].ToString();
                    string value = node["node"]["title"].ToString();

                    string lat = node["node"]["location"]["latitude"].ToString();
                    string Lang = node["node"]["location"]["longitude"].ToString();

                    CityList.Add(new Tuple<string, string, string, string>(key, value, lat, Lang));

                    LocationDictionary.Add(key, value);
                }
            }
            catch (Exception)
            {

            }


        }
    }
}