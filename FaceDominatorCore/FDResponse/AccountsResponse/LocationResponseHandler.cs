using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace FaceDominatorCore.FDResponse.AccountsResponse
{
    public class LocationResponseHandler : FdResponseHandler, IResponseHandler
    {
        public string LocationId { get; set; } = string.Empty;

        public bool Status { get; set; }

        public bool HasMoreResults { get; set; } = true;

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();

        public LocationResponseHandler(IResponseParameter responseParameter)
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;

            var response = Utilities.GetBetween(responseParameter.Response, "props:{data:", ",query:\"");
            response += "}";

            GetLocationFromJson(response);
        }

        private void GetLocationFromJson(string response)
        {
            try
            {

                JObject jObject = JObject.Parse(response);

                var friendList = jObject["results"];

                var locationList = (from token in friendList
                                    let id = token.Path.Split('.')[1]
                                    let category = token.First()["entityInfo"]["aboutInfo"]["category"].ToString()
                                    let name = token.First()["entityInfo"]["aboutInfo"]["name"].ToString()
                                    let ratingUrl = token.First()["entityInfo"]["aboutInfo"]["rating_url"].ToString()
                                    select new LocationDetails
                                    {
                                        Id = id,
                                        Category = category,
                                        Name = name,
                                        Rating = ratingUrl

                                    }).ToList();

                LocationId = locationList.FirstOrDefault(x => x.Category == "Town/City" ||
                                x.Category == "Region" || x.Category == "Country" || x.Category == "Village")
                    ?.Id;
                Status = true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
