using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDResponse.BaseResponse;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaceDominatorCore.FDResponse.CommonResponse
{
    public class LocationDetailsResponseHandler : FdResponseHandler
    {
        public string Location;
        public List<Tuple<string, string, string>> listOfLatLon = new List<Tuple<string, string, string>>();
        public LocationDetailsResponseHandler(IResponseParameter responseParameter, string eventLocation)
            : base(responseParameter)
        {

            try
            {
                if (responseParameter.HasError)
                    return;

                var jsonValue = responseParameter.Response.Replace("for (;;);", string.Empty);
                var jObject = JObject.Parse(jsonValue);
                var jsonValues = jObject["payload"][eventLocation]["street_results"]["edges"];

                if (jsonValues.Count() > 1)
                {
                    foreach (JToken singleJsonValue in jsonValues)
                    {
                        var title = singleJsonValue["node"]["title"].ToString();
                        var lattitude = singleJsonValue["node"]["location"]["latitude"].ToString();
                        var longitude = singleJsonValue["node"]["location"]["longitude"].ToString();

                        listOfLatLon.Add(new Tuple<string, string, string>(title, lattitude, longitude));
                    }
                    return;
                }

                Location = jObject["payload"][eventLocation]["street_results"]["edges"][0]["node"]["title"].ToString();

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }
    }


    public class LattitudeAndLangitudeResponseHandler : FdResponseHandler
    {
        public string Latitude = string.Empty;
        public string Longitude = string.Empty;

        public LattitudeAndLangitudeResponseHandler(IResponseParameter responseParameter)
            : base(responseParameter)
        {
            try
            {
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(responseParameter.Response);
                string innerText = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='Z0LcW']").InnerText;

                string[] values = innerText.Split(',');

                Latitude = values[0].Trim().Substring(0, 5);//77.98
                Longitude = values[1].Trim().Substring(0, 5); //77.98

            }
            catch (Exception)
            {
                //Ignored
            }




        }
    }
}
