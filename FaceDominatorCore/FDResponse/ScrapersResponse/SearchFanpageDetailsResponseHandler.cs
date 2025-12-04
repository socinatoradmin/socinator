using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaceDominatorCore.FDResponse.ScrapersResponse
{

    public class SearchPlaceDetailsResponseHandler : FdResponseHandler, IResponseHandler
    {
        public bool HasMoreResults { get; set; }

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
        = new FdScraperResponseParameters();


        public SearchPlaceDetailsResponseHandler(IResponseParameter responseParameter, bool isPagination = false)
            : base(responseParameter)
        {

            if (responseParameter.HasError || responseParameter.Response == null)
                return;

            ObjFdScraperResponseParameters = new FdScraperResponseParameters();

            ObjFdScraperResponseParameters.ListPage = new List<FanpageDetails>();

            string decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);

            GetPlaceId(responseParameter.Response, decodedResponse, isPagination);

            GetPaginationData(decodedResponse, isPagination);

        }

        private void GetPaginationData(string decodedResponse, bool isPagination = false)
        {
            try
            {
                if (!isPagination)
                {
                    ObjFdScraperResponseParameters.Query = FdRegexUtility.FirstMatchExtractor(decodedResponse, "query:\"(.*?)\"");
                    ObjFdScraperResponseParameters.OriginalQuery = FdRegexUtility.FirstMatchExtractor(decodedResponse, "originalQuery:\"(.*?)\"");
                    ObjFdScraperResponseParameters.Cursor = FdRegexUtility.FirstMatchExtractor(decodedResponse, "cursor:\"(.*?)\"");
                }
                else
                    ObjFdScraperResponseParameters.Cursor = FdRegexUtility.FirstMatchExtractor(decodedResponse, "cursor\":\"(.*?)\"");

                HasMoreResults = !string.IsNullOrEmpty(ObjFdScraperResponseParameters.Cursor);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void GetPlaceId(string response, string partialDecodedResponse, bool isPagination = false)
        {
            try
            {
                if (!isPagination)
                    partialDecodedResponse = FdRegexUtility.FirstMatchExtractor(partialDecodedResponse, "props:{data:(.*?),query:\"") + "}";
                else
                    partialDecodedResponse = FdRegexUtility.FirstMatchExtractor(response, "\"payload\":(.*?),\"resultUnitIDs") + "}";
                JObject jObject = JObject.Parse(partialDecodedResponse);

                JToken placeDetails = jObject["results"];

                ObjFdScraperResponseParameters.ListPage = (from token in placeDetails
                                                           let id = token.Path
                                                           let address = token.First["entityInfo"]["aboutInfo"]["address"]
                                                           let category = token.First["entityInfo"]["aboutInfo"]["category"]
                                                           let categoryLink = token.First["entityInfo"]["aboutInfo"]["categoryLink"]
                                                           let name = token.First["entityInfo"]["aboutInfo"]["name"]
                                                           let neighborhood = token.First["entityInfo"]["aboutInfo"]["neighborhood"]
                                                           let phone = token.First["entityInfo"]["aboutInfo"]["phone"]
                                                           let price = token.First["entityInfo"]["aboutInfo"]["price"]
                                                           let rating = token.First["entityInfo"]["aboutInfo"]["rating"]
                                                           let rating_count = token.First["entityInfo"]["aboutInfo"]["rating_count"]
                                                           let rating_url = token.First["entityInfo"]["aboutInfo"]["rating_url"]
                                                           let status = token.First["entityInfo"]["aboutInfo"]["status"]
                                                           let url = token.First["entityInfo"]["aboutInfo"]["url"]
                                                           let userLikesPage = token.First["entityInfo"]["aboutInfo"]["userLikesPage"]
                                                           let menu_url = token.First["entityInfo"]["aboutInfo"]["menu_url"]
                                                           select new FanpageDetails
                                                           {
                                                               FanPageID = FdRegexUtility.FirstMatchExtractor($"{id.ToString()}_", "results.(.*?)_"),
                                                               FanPageName = name == null ? string.Empty : name.ToString(),
                                                               FanPageUrl = url == null ? string.Empty : url.ToString(),
                                                               Address = address == null ? string.Empty : address.ToString(),
                                                               FanPageCategory = category == null ? string.Empty : category.ToString(),
                                                               NeighbourHood = neighborhood == null ? string.Empty : neighborhood.ToString(),
                                                               PhoneNumber = phone == null ? string.Empty : phone.ToString(),
                                                               RatingCount = rating_count == null ? string.Empty : rating_count.ToString(),
                                                               RatingValue = rating == null ? string.Empty : rating.ToString(),
                                                               RatingUrl = rating_url == null ? string.Empty : $"{FdConstants.FbHomeUrl}{rating_url.ToString()}",
                                                               MenuUrl = menu_url == null ? string.Empty : $"{FdConstants.FbHomeUrl}{menu_url.ToString()}",
                                                               Status = status == null ? string.Empty : status.ToString(),
                                                               IsLikedByUser = userLikesPage == null ? false : userLikesPage.ToString().Equals("true") ? true : false
                                                           }).ToList();

                Status = ObjFdScraperResponseParameters.ListPage.Count > 0;

            }
            catch (Exception)
            {

            }
        }

    }
}
